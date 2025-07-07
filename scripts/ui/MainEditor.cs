using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Parallas;

public partial class MainEditor : Control
{
    [Export] public Toolbar Toolbar { get; set; }
    [Export] public Topbar Topbar { get; set; }

    [Export] public DrawCanvas TargetDrawCanvas;
    [Export] public Control CanvasRoot;

    [Export] public ToolState DefaultToolState;

    public readonly Dictionary<int, ToolState> PlayerToolStates = new Dictionary<int, ToolState>();
    private readonly Dictionary<int, DrawState> PlayerDrawStates = new Dictionary<int, DrawState>();

    public static readonly List<InkDefinition> AllInkDefinitions =
    [
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_00_red.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_01_orange.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_02_yellow.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_03_lime.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_04_green.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_05_blue.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_06_indigo.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_07_violet.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_08_pink.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_09_skin_light.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_10_skin_medium.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_11_skin_tan.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_12_skin_dark.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_13_black.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_14_gray.tres"),
        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_main_15_white.tres"),

        GD.Load<InkDefinition>("res://drawing_resources/ink/ink_rainbow.tres"),
        new InkDefinition
        {

        }
    ];

    public override void _Ready()
    {
        base._Ready();

        Input.SetMouseMode(Input.MouseModeEnum.Hidden);
        Godot.Engine.MaxFps = 120;
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        AddToGroup("main_editor");
        PlayerDeviceMapper.PlayerCreated += AddPlayerToolState;
        PlayerDeviceMapper.PlayerRemoved += RemovePlayerToolState;
        TargetDrawCanvas.Draw += DrawAllDrawStates;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        RemoveFromGroup("main_editor");
        PlayerDeviceMapper.PlayerCreated -= AddPlayerToolState;
        PlayerDeviceMapper.PlayerRemoved -= RemovePlayerToolState;
        TargetDrawCanvas.Draw -= DrawAllDrawStates;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        var cursors = GetTree().GetNodesInGroup("player_cursors").Cast<GameCursor>();
        var gameCursors = cursors as GameCursor[] ?? cursors.ToArray();

        foreach (var (playerId, drawState) in PlayerDrawStates)
        {
            var drawingCursor = gameCursors.FirstOrDefault(cursor => cursor.PlayerId == playerId, null);
            if (drawingCursor is null) continue;
            if (!PlayerToolStates.TryGetValue(playerId, out var toolState)) continue;

            var color = toolState.InkDefinition.SampleColor(drawState.DrawingTime);
            drawState.Process(TargetDrawCanvas.GetCanvasPosition(drawingCursor.GlobalPosition), color, delta);
            toolState.ToolDefinition.BrushDefinition?.Process(drawState, delta);

            drawState.CanvasItem?.QueueRedraw();
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        var deviceId = PlayerDeviceMapper.GetControllerOffsetDeviceId(@event);
        if (!PlayerDeviceMapper.TryGetPlayerDeviceMapFromDevice(deviceId, out PlayerDeviceMap deviceMap)) return;
        int playerId = deviceMap.PlayerId;

        var gameCursor = GetTree().GetNodesInGroup("player_cursors").Cast<GameCursor>()
            .FirstOrDefault(cursor => cursor.PlayerId == playerId);
        if (gameCursor is null) return;
        if (!PlayerToolStates.TryGetValue(playerId, out ToolState toolState)) return;

        if (@event.IsActionPressed("click") && TargetDrawCanvas.IsWithinCanvas(gameCursor.GlobalPosition))
        {
            if (PlayerDrawStates.ContainsKey(playerId)) return;

            var color = toolState.InkDefinition.SampleColor(0);
            var drawState = DrawState.Start(
                TargetDrawCanvas,
                TargetDrawCanvas.GetCanvasPosition(gameCursor.GlobalPosition),
                color
            );
            PlayerDrawStates.TryAdd(playerId, drawState);

            toolState.ToolDefinition.BrushDefinition?.Start(drawState);
        }
        if (@event.IsActionReleased("click"))
        {
            if (!PlayerDrawStates.TryGetValue(playerId, out var drawState)) return;
            var color = toolState.InkDefinition.SampleColor(drawState.DrawingTime);
            drawState.Finish(
                TargetDrawCanvas.GetCanvasPosition(gameCursor.GlobalPosition),
                color,
                0
            );
            toolState.ToolDefinition.BrushDefinition?.Finish(drawState, 0);
        }
    }

    private void DrawAllDrawStates()
    {
        foreach (var (playerId, drawState) in PlayerDrawStates)
        {
            if (!PlayerToolStates.TryGetValue(playerId, out var toolState)) continue;

            toolState.ToolDefinition.BrushDefinition.Draw(drawState);

            // Set the last frame properties
            drawState.LastCursorPosition = drawState.CursorPosition;
            drawState.LastEvaluatedPosition = drawState.EvaluatedPosition;

            if (drawState.State == DrawState.States.Start) drawState.State = DrawState.States.Draw;
            if (drawState.State == DrawState.States.Finish) PlayerDrawStates.Remove(playerId);
        }
    }

    private void AddPlayerToolState(int playerId)
    {
        var toolState = DefaultToolState.Duplicate() as ToolState;
        PlayerToolStates.TryAdd(playerId, toolState);
    }

    private void RemovePlayerToolState(int playerId)
    {
        if (!PlayerToolStates.ContainsKey(playerId)) return;
        PlayerToolStates.Remove(playerId);
    }
}
