using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;
using Parallas;

public partial class MainEditor : Control
{
    [Export] public Toolbar Toolbar { get; set; }
    [Export] public Topbar Topbar { get; set; }

    [Export] public DrawCanvas TargetDrawCanvas;
    [Export] public Control CanvasRoot;

    private List<GameCursor> _drawingCursors = new List<GameCursor>();

    public override void _Ready()
    {
        base._Ready();

        Input.SetMouseMode(Input.MouseModeEnum.Hidden);
        Godot.Engine.MaxFps = 120;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        foreach (var drawingCursor in _drawingCursors)
        {
            drawingCursor.ToolState?.ToolDefinition.BrushDefinition?.Process(
                TargetDrawCanvas.GetCanvasPosition(drawingCursor.GlobalPosition),
                drawingCursor.ToolState.BrushColor,
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
        var toolState = gameCursor.ToolState;

        if (@event.IsActionPressed("click"))
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
}
