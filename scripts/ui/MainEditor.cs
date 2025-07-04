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

    private readonly List<GameCursor> _drawingCursors = new List<GameCursor>();
    public readonly Dictionary<int, ToolState> PlayerToolStates = new Dictionary<int, ToolState>();

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
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        RemoveFromGroup("main_editor");
        PlayerDeviceMapper.PlayerCreated -= AddPlayerToolState;
        PlayerDeviceMapper.PlayerRemoved -= RemovePlayerToolState;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        foreach (var drawingCursor in _drawingCursors)
        {
            if (!PlayerToolStates.TryGetValue(drawingCursor.PlayerId, out ToolState toolState)) continue;
            toolState.ToolDefinition.BrushDefinition?.Process(
                TargetDrawCanvas.GetCanvasPosition(drawingCursor.GlobalPosition),
                toolState.BrushColor,
                delta
            );
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
        if (!PlayerToolStates.TryGetValue(gameCursor.PlayerId, out ToolState toolState)) return;

        if (@event.IsActionPressed("click") && TargetDrawCanvas.IsWithinCanvas(gameCursor.GlobalPosition))
        {
            if (_drawingCursors.Contains(gameCursor)) return;

            toolState.ToolDefinition.BrushDefinition?.Start(
                TargetDrawCanvas,
                TargetDrawCanvas.GetCanvasPosition(gameCursor.GlobalPosition),
                toolState.BrushColor,
                0
            );
            _drawingCursors.Add(gameCursor);
        }
        if (@event.IsActionReleased("click"))
        {
            if (!_drawingCursors.Contains(gameCursor)) return;
            toolState.ToolDefinition.BrushDefinition?.Finish(
                TargetDrawCanvas.GetCanvasPosition(gameCursor.GlobalPosition),
                toolState.BrushColor,
                0
            );
            _drawingCursors.Remove(gameCursor);
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
