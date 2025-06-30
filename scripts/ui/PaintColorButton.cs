using Godot;
using System;
using Parallas;

public partial class PaintColorButton : Button
{
    [Export] private PaintBlob _paintBlob;
    [Export] private Color _paintColor = Colors.Red;
    private Vector2 _scale = Vector2.One;
    private Vector2 _scaleVelocity = Vector2.Zero;

    public override void _Ready()
    {
        base._Ready();

        _paintBlob.SetColor(_paintColor);

        _paintBlob.RotateZ(GD.Randf() * 2f * Mathf.Pi);
        if (GD.Randf() < 0.5f) _paintBlob.RotateX(Mathf.Pi);
        if (GD.Randf() < 0.5f) _paintBlob.RotateY(Mathf.Pi);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        SetPivotOffset(Size * 0.5f);

        var scaleTarget = 1f;
        if (IsHovered()) scaleTarget = 1.15f;
        if (IsPressed()) scaleTarget = 1f;
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

        Scale = _scale;
    }
}
