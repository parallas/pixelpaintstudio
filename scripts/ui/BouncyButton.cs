using Godot;
using System;
using Parallas;

[GlobalClass]
public partial class BouncyButton : Node
{
    [Export] public VirtualCursorButton TargetButton { get; set; }
    [Export] public Control VisualRoot { get; set; }
    [Export] private bool _scaleWhenSelected = true;

    private Vector2 _scale = Vector2.One;
    private Vector2 _scaleVelocity = Vector2.Zero;

    public override void _Process(double delta)
    {
        base._Process(delta);

        var scaleTarget = 1f;
        if (TargetButton.IsHoveredVirtually) scaleTarget = 1.15f;
        if (TargetButton.IsPressedVirtually) scaleTarget = 1f;
        if (_scaleWhenSelected && TargetButton.IsSelected) scaleTarget += 0.45f;
        var (x, y) = _scale;
        var (xVel, yVel) = _scaleVelocity;
        MathUtil.Spring(
            ref x,
            ref xVel,
            scaleTarget,
            0.1f,
            25f,
            (float)delta
        );
        MathUtil.Spring(
            ref y,
            ref yVel,
            scaleTarget,
            0.25f,
            20f,
            (float)delta
        );
        _scale = new Vector2(x, y);
        _scaleVelocity = new Vector2(xVel, yVel);

        VisualRoot.Scale = _scale;
    }
}
