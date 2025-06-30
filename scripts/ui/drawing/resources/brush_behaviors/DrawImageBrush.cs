using Godot;
using System;

[GlobalClass]
public partial class DrawImageBrush : BrushBehavior
{
    [Export] public Texture2D Texture { get; set; }
    [Export] public Vector2 SizeOverride { get; set; }
    [Export] public bool TintUsingColor { get; set; }
    [Export] public bool FillGapsBetweenDraws { get; set; }

    [ExportGroup("Randomness")]
    [Export] public Vector2 RandomAngleRange { get; set; }
    [Export] public Vector2 RandomScaleRange { get; set; } = Vector2.One;

    private Vector2 _lastPosition = Vector2.Zero;

    public override void Initialize(Vector2 cursorPosition, Color cursorColor)
    {
        base.Initialize(cursorPosition, cursorColor);

        _lastPosition = -Vector2.Inf;
    }

    public override void Draw(BrushDefinition brushDefinition, CanvasItem canvasItem)
    {
        base.Draw(brushDefinition, canvasItem);

        var texture = Texture;
        if (TintUsingColor) texture = BrushUtils.Colorize(Texture, brushDefinition.EvaluatedColor);

        if (_lastPosition == -Vector2.Inf)
            _lastPosition = brushDefinition.EvaluatedPosition;

        if (!FillGapsBetweenDraws) _lastPosition = brushDefinition.EvaluatedPosition;

        var size = SizeOverride;
        if (size == Vector2.Zero) size = Texture.GetSize();

        int distanceInPixels = (int)brushDefinition.EvaluatedPosition.DistanceTo(_lastPosition);
        for (int i = 0; i <= distanceInPixels; i++)
        {
            float percent = i / (float)distanceInPixels;
            Vector2 position = _lastPosition.Lerp(brushDefinition.EvaluatedPosition, percent);

            var transform = Transform2D.Identity
                    .Translated(-size * 0.5f)
                    .Scaled(Vector2.One * (float)GD.RandRange(RandomScaleRange.X, RandomScaleRange.Y))
                    .Rotated(Mathf.DegToRad((float)GD.RandRange(RandomAngleRange.X, RandomAngleRange.Y)))
                    .Translated(position)
                ;
            canvasItem.DrawSetTransformMatrix(transform);

            canvasItem.DrawTextureRect(texture, new Rect2(Vector2.Zero, size), false, Colors.White);
        }

        canvasItem.DrawSetTransformMatrix(Transform2D.Identity);

        _lastPosition = brushDefinition.EvaluatedPosition;
    }
}
