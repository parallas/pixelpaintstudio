using Godot;
using System;
using Parallas;

public partial class DrawCanvas : Control
{
    [Export] public TextureRect OutputTextureTarget;
    [Export] private SubViewport _subViewport;
    public SubViewport SubViewport => _subViewport;

    public Vector2I Resolution { get; private set; } = new Vector2I(640, 360);
    public Rect2I SizeRect => new Rect2I(Vector2I.Zero, Resolution);

    public override void _Ready()
    {
        base._Ready();

        SetResolution(Resolution);
    }

    public void SetResolution(Vector2I resolution)
    {
        Resolution = resolution;
        _subViewport.Size = Resolution;
    }
}
