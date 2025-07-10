using System.Collections.Generic;
using System.Linq;
using Godot;
using Parallas;

[GlobalClass]
public partial class VirtualCursorButton : Button
{
    [Signal] public delegate void PressedVirtuallyEventHandler();
    public bool IsHoveredVirtually => _hoveredPlayerIds.Count > 0;
    public bool IsPressedVirtually => _pressedPlayerIds.Count > 0;
    private readonly HashSet<int> _hoveredPlayerIds = [];
    private readonly HashSet<int> _pressedPlayerIds = [];
    public bool IsSelected { get; protected set; }

    protected GameCursor[] GameCursors { get; private set; }

    public override void _Process(double delta)
    {
        base._Process(delta);

        GameCursors = GetTree().GetNodesInGroup("player_cursors").OfType<GameCursor>().ToArray();
        foreach (var gameCursor in GameCursors)
        {
            if (IsIntersecting(gameCursor.GlobalPosition))
            {
                _hoveredPlayerIds.Add(gameCursor.PlayerId);
            }
            else
            {
                _hoveredPlayerIds.Remove(gameCursor.PlayerId);
            }
        }
    }

    private bool IsIntersecting(Vector2 cursorPosition)
    {
        var globalRect = GetGlobalRect();
        return globalRect.HasPoint(cursorPosition);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        HandleInput(@event);
    }

    protected virtual void VirtualCursorPressed(InputEvent @event, int playerId) { }

    private void HandleInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton eventMouseButton) return;

        var deviceId = PlayerDeviceMapper.GetControllerOffsetDeviceId(@event);
        if (!PlayerDeviceMapper.TryGetPlayerDeviceMapFromDevice(deviceId, out var deviceMap)) return;
        var playerId = deviceMap.PlayerId;
        bool wasPressed = _pressedPlayerIds.Contains(playerId);
        bool wasHovered = _hoveredPlayerIds.Contains(playerId);

        bool isPressed = eventMouseButton.IsPressed();
        bool pressed = isPressed && !wasPressed && wasHovered;
        bool released = !isPressed && wasPressed && wasHovered;

        if (pressed) _pressedPlayerIds.Add(playerId);
        if (!pressed) _pressedPlayerIds.Remove(playerId);

        if (GetActionMode() == ActionModeEnum.Release)
        {
            if (!released) return;
            VirtualCursorPressed(@event, playerId);
            // EmitSignalPressed();
            EmitSignalPressedVirtually();
        }
        else
        {
            if (!pressed) return;
            VirtualCursorPressed(@event, playerId);
            // EmitSignalPressed();
            EmitSignalPressedVirtually();
        }
    }
}
