using Godot;
using System;
using Parallas;

public partial class DrawCanvas : Control
{
    [Export] private SubViewport SubViewport;
    [Export] private AspectRatioContainer _aspectRatioContainer;
    [Export] public Vector2I Resolution = new Vector2I(640, 360);

    public override void _Process(double delta)
    {
        base._Process(delta);

        _aspectRatioContainer.Ratio = (float)Resolution.X / Resolution.Y;
        SubViewport.Size = Resolution;
    }
}
