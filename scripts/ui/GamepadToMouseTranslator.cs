using Godot;
using Parallas;

public partial class GamepadToMouseTranslator : Node
{
    private Vector2 _physicalMousePosition;
    public override void _Ready()
    {
        base._Ready();

        PlayerDeviceMapper.RegisterNewPlayer(0);

        Input.JoyConnectionChanged += JoyConnectionChanged;

        Input.UseAccumulatedInput = true;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        PlayerDeviceMapper.Process(delta);

        if (PlayerDeviceMapper.TryGetPlayerDeviceMapFromDevice(0, out var deviceMap))
        {
            var currentPhysicalMousePosition = GetViewport().GetMousePosition();
            if (currentPhysicalMousePosition != _physicalMousePosition)
            {
                _physicalMousePosition = currentPhysicalMousePosition;
                deviceMap.MousePosition = _physicalMousePosition;
            }
        }

        foreach (var playerDeviceMap in PlayerDeviceMapper.GetAllPlayerDeviceMaps())
        {
            playerDeviceMap.MousePosition =
                playerDeviceMap.MousePosition.Clamp(Vector2.Zero, GetViewport().GetVisibleRect().Size);
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        var deviceId = PlayerDeviceMapper.GetControllerOffsetDeviceId(@event);
        if (!PlayerDeviceMapper.TryGetPlayerDeviceMapFromDevice(deviceId, out var deviceMap))
            return;

        switch (@event)
        {
            case InputEventJoypadMotion joypadMotion:
            {
                var inputVector = Input.GetVector(
                    "cursor_left",
                    "cursor_right",
                    "cursor_up",
                    "cursor_down"
                );
                deviceMap.ConstantMoveDirection = inputVector.Normalized() * inputVector.Length();

                break;
            }
            case InputEventJoypadButton joypadButton:
            {
                if (joypadButton.IsActionPressed("click"))
                {
                    Input.ParseInputEvent(new InputEventMouseButton
                    {
                        Device = deviceId,
                        Pressed = true,
                        ButtonIndex = MouseButton.Left,
                        Position = deviceMap.MousePosition,
                    });
                }
                if (joypadButton.IsActionReleased("click"))
                {
                    Input.ParseInputEvent(new InputEventMouseButton
                    {
                        Device = deviceId,
                        Pressed = false,
                        ButtonIndex = MouseButton.Left,
                        Position = deviceMap.MousePosition,
                    });
                }
                break;
            }
        }
    }

    private void JoyConnectionChanged(long device, bool connected)
    {
        device += 1;
        GD.Print($"JoyConnectionChanged: device: {device}, connected: {connected}");

        if (connected)
            PlayerDeviceMapper.RegisterNewPlayer((int)device);
        else
            PlayerDeviceMapper.DeregisterDevice((int)device);
    }
}
