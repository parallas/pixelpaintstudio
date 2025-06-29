using Godot;
using System;
using Parallas;

public partial class DrawCanvas : Control
{
    [Export] private AspectRatioContainer _aspectRatioContainer;
    [Export] private Vector2I _resolution = new Vector2I(640, 360);

    public override void _Process(double delta)
    {
        base._Process(delta);

        _aspectRatioContainer.Ratio = (float)_resolution.X / _resolution.Y;
    }
}
