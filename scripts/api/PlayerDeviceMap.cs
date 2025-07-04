using System.Collections.Generic;
using Godot;

namespace Parallas;

public class PlayerDeviceMap
{
    public required int PlayerId;
    public required List<int> DeviceIds = [];
    public Vector2 MousePosition;
    public Vector2 ConstantMoveDirection;

    private float _cursorCurrentSpeed = 1f;
    private float _baseSpeed = 400f;
    private Vector2 _minMaxSpeedMultiplier = new Vector2(1, 5);

    public void Process(double delta)
    {
        Vector2 newPos = MousePosition;

        if (ConstantMoveDirection.LengthSquared() > 0)
            _cursorCurrentSpeed = MathUtil.ExpDecay(_cursorCurrentSpeed, _minMaxSpeedMultiplier.Y, 1f, (float)delta);
        else
            _cursorCurrentSpeed = _minMaxSpeedMultiplier.X;
        newPos += ConstantMoveDirection * _baseSpeed * _cursorCurrentSpeed * (float)delta;

        MousePosition = newPos;
    }
}
