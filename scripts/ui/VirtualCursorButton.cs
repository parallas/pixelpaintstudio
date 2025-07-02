using Godot;

public partial class VirtualCursorButton : Button
{
    public bool IsHoveredVirtually { get; private set; } = false;
    private int _pressedDeviceId = -1;


    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (!IsHovered())
        {
            if (_pressedDeviceId == @event.Device)
                _pressedDeviceId = -1;
            return;
        }
        if (@event is not InputEventMouseButton eventMouseButton) return;
        if (GetActionMode() == ActionModeEnum.Release)
        {
            if (_pressedDeviceId > -1 && eventMouseButton.Device != _pressedDeviceId) return;
            if (eventMouseButton.IsPressed())
                _pressedDeviceId = eventMouseButton.Device;
            if (!eventMouseButton.IsReleased()) return;
            _pressedDeviceId = -1;
            VirtualCursorPressed(eventMouseButton);
        }
        else
        {
            if (!eventMouseButton.IsPressed()) return;
            VirtualCursorPressed(eventMouseButton);
        }
    }

    protected virtual void VirtualCursorPressed(InputEventMouseButton eventMouseButton) { }
}
