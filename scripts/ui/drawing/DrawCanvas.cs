using Godot;
using System;
using Parallas;

public partial class DrawCanvas : Control
{
    [Export] public TextureRect OutputTextureTarget;
    [Export] private SubViewport _subViewport;
    public SubViewport SubViewport => _subViewport;
    [Export] private AspectRatioContainer _aspectRatioContainer;
    [Export] public Vector2I Resolution = new Vector2I(640, 360);

    public Rect2I SizeRect => new Rect2I(Vector2I.Zero, Resolution);

    public override void _Ready()
    {
        base._Ready();

        OutputTextureTarget.SetTexture(_subViewport.GetTexture());
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        _aspectRatioContainer.Ratio = (float)Resolution.X / Resolution.Y;
        _subViewport.Size = Resolution;
    }

    public Vector2 GetCanvasPosition(Vector2 cursorPosition)
    {
        if (GetViewport().GetParent() is SubViewportContainer)
        {
            var finalValue = GetScreenTransform().AffineInverse().BasisXform(
                cursorPosition - GetScreenPosition()
            );
            return finalValue;
        }

        var posLocalToRect = cursorPosition - OutputTextureTarget.GlobalPosition;
        var scaleFactor = this.Resolution / OutputTextureTarget.Size;
        var canvasRelativePosition = posLocalToRect * scaleFactor;
        var pixelPosition = canvasRelativePosition.Round();
        return pixelPosition;
    }

    public bool IsWithinCanvas(Vector2 cursorPosition)
    {
        var canvasPosition = GetCanvasPosition(cursorPosition).ToVector2I();
        return SizeRect.HasPoint(canvasPosition);
    }
}
