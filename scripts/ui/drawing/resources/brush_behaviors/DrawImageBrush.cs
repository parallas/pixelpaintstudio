using Godot;
using System.Collections.Generic;
using System.Linq;
using Parallas;

[GlobalClass]
public partial class DrawImageBrush : BrushBehavior
{
    [Export] public Texture2D Texture { get; set; }
    [Export] public Vector2 SizeOverride { get; set; }
    [Export] public bool TintUsingColor { get; set; }
    [Export] public bool MultiplyByColor { get; set; }
    [Export] public bool FillGapsBetweenDraws { get; set; }

    [ExportGroup("Randomness")]
    [Export] public Vector2 RandomAngleRange { get; set; }
    [Export] public Vector2 RandomScaleRange { get; set; } = Vector2.One;

    private Vector2 _lastPosition = Vector2.Zero;
    private Vector2 _lastScale = Vector2.One;

    private BrushUtils.ImageDataCache _imageDataCache;

    public override void Initialize(Vector2 cursorPosition, Color cursorColor)
    {
        base.Initialize(cursorPosition, cursorColor);

        _lastPosition = -Vector2.Inf;
        _lastScale = -Vector2.Inf;
    }

    public override void Process(DrawState drawState, double delta)
    {
        base.Process(drawState, delta);

        _imageDataCache = BrushUtils.ImageDataCacher.CreateOrGet(Texture, _imageDataCache);
    }

    public override void Draw(DrawState drawState, CanvasItem canvasItem)
    {
        base.Draw(drawState, canvasItem);

        if (TintUsingColor) BrushUtils.ColorizeBasic(_imageDataCache, drawState.EvaluatedColor);

        if (_lastPosition == -Vector2.Inf)
            _lastPosition = drawState.EvaluatedPosition;
        if (_lastScale == -Vector2.Inf)
            _lastScale = drawState.EvaluatedScale;

        if (!FillGapsBetweenDraws) _lastPosition = drawState.EvaluatedPosition;

        var size = SizeOverride;
        if (size == Vector2.Zero) size = _imageDataCache.ImageTexture.GetSize();

        var linePoints = Geometry2D.BresenhamLine(_lastPosition.ToVector2I(), drawState.EvaluatedPosition.ToVector2I());
        for (int i = 0; i < linePoints.Count; i++)
        {
            float percent = (float)i / (linePoints.Count);
            Vector2 scaleAmount = _lastScale.Lerp(drawState.EvaluatedScale, percent);
            Color color = MultiplyByColor ? drawState.LastEvaluatedColor.Lerp(drawState.EvaluatedColor, percent) : Colors.White;
            DrawAt(canvasItem, _imageDataCache.ImageTexture, size * scaleAmount, linePoints[i], color);
        }

        _lastPosition = drawState.EvaluatedPosition;
    }

    private void DrawAt(CanvasItem canvasItem, Texture2D texture, Vector2 size, Vector2 position, Color color)
    {
        var transform = Transform2D.Identity
                .Translated(-size * 0.5f)
                .Scaled(Vector2.One * (float)GD.RandRange(RandomScaleRange.X, RandomScaleRange.Y))
                .Rotated(Mathf.DegToRad((float)GD.RandRange(RandomAngleRange.X, RandomAngleRange.Y)))
                .Translated(position)
            ;
        canvasItem.DrawSetTransformMatrix(transform);

        canvasItem.DrawTextureRect(texture, new Rect2(Vector2.Zero, size), false, color);

        canvasItem.DrawSetTransformMatrix(Transform2D.Identity);
    }
}
