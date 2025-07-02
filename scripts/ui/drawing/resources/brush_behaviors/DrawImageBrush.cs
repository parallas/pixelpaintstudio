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

    private ulong _imageInstanceId;
    private Dictionary<ulong, BrushUtils.ImageDataCache> _imageDataCaches;
    private BrushUtils.ImageDataCache _imageDataCache;

    public override void Initialize(Vector2 cursorPosition, Color cursorColor)
    {
        base.Initialize(cursorPosition, cursorColor);

        _lastPosition = -Vector2.Inf;

        _imageDataCaches = new Dictionary<ulong, BrushUtils.ImageDataCache>();
    }

    public override void Draw(BrushDefinition brushDefinition, CanvasItem canvasItem)
    {
        base.Draw(brushDefinition, canvasItem);

        var thisImageInstanceId = Texture.GetInstanceId();
        if (!_imageDataCaches.ContainsKey(thisImageInstanceId))
        {
            _imageDataCaches.Add(thisImageInstanceId, BrushUtils.ImageDataCache.CreateFromTexture(Texture));
        }
        if (thisImageInstanceId != _imageInstanceId)
        {
            _imageInstanceId = thisImageInstanceId;
            _imageDataCache = _imageDataCaches[thisImageInstanceId];
        }

        BrushUtils.Colorize(_imageDataCache, brushDefinition.EvaluatedColor);

        if (_lastPosition == -Vector2.Inf)
            _lastPosition = brushDefinition.EvaluatedPosition;

        if (!FillGapsBetweenDraws) _lastPosition = brushDefinition.EvaluatedPosition;

        var size = SizeOverride;
        if (size == Vector2.Zero) size = _imageDataCache.ImageTexture.GetSize();

        int distanceInPixels = (int)brushDefinition.EvaluatedPosition.DistanceTo(_lastPosition);
        for (int i = 0; i < distanceInPixels; i++)
        {
            float percent = i / (float)distanceInPixels;
            Vector2 position = _lastPosition.Lerp(brushDefinition.EvaluatedPosition, percent);

            DrawAt(canvasItem, _imageDataCache.ImageTexture, size, position);
        }

        DrawAt(canvasItem, _imageDataCache.ImageTexture, size, brushDefinition.EvaluatedPosition);

        _lastPosition = brushDefinition.EvaluatedPosition;
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
