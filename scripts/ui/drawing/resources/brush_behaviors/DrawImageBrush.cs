using Godot;
using System.Collections.Generic;

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

    private BrushUtils.ImageDataCache _imageDataCache;

    public override void Initialize(Vector2 cursorPosition, Color cursorColor)
    {
        base.Initialize(cursorPosition, cursorColor);

        _lastPosition = -Vector2.Inf;
    }

    public override void Process(DrawState drawState, double delta)
    {
        base.Process(drawState, delta);

        _imageDataCache = BrushUtils.ImageDataCacher.CreateOrGet(Texture, _imageDataCache);
    }

    public override void Draw(DrawState drawState, CanvasItem canvasItem)
    {
        base.Draw(drawState, canvasItem);

        BrushUtils.Colorize(_imageDataCache, drawState.EvaluatedColor);

        if (_lastPosition == -Vector2.Inf)
            _lastPosition = drawState.EvaluatedPosition;

        if (!FillGapsBetweenDraws) _lastPosition = drawState.EvaluatedPosition;

        var size = SizeOverride;
        if (size == Vector2.Zero) size = _imageDataCache.ImageTexture.GetSize();

        int distanceInPixels = (int)drawState.EvaluatedPosition.DistanceTo(_lastPosition);
        for (int i = 0; i < distanceInPixels; i++)
        {
            float percent = i / (float)distanceInPixels;
            Vector2 position = _lastPosition.Lerp(drawState.EvaluatedPosition, percent);

            DrawAt(canvasItem, _imageDataCache.ImageTexture, size, position);
        }

        DrawAt(canvasItem, _imageDataCache.ImageTexture, size, drawState.EvaluatedPosition);

        _lastPosition = drawState.EvaluatedPosition;
    }

    private void DrawAt(CanvasItem canvasItem, Texture2D texture, Vector2 size, Vector2 position)
    {
        var transform = Transform2D.Identity
                .Translated(-size * 0.5f)
                .Scaled(Vector2.One * (float)GD.RandRange(RandomScaleRange.X, RandomScaleRange.Y))
                .Rotated(Mathf.DegToRad((float)GD.RandRange(RandomAngleRange.X, RandomAngleRange.Y)))
                .Translated(position)
            ;
        canvasItem.DrawSetTransformMatrix(transform);

        canvasItem.DrawTextureRect(texture, new Rect2(Vector2.Zero, size), false, Colors.White);

        canvasItem.DrawSetTransformMatrix(Transform2D.Identity);
    }
}
