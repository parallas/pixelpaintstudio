using Godot;
using System;

[GlobalClass]
public partial class DrawImage : BrushBehavior
{
    [Export] public Texture2D Texture { get; set; }
    [Export] public Vector2 Size { get; set; }
    [Export] public Vector2 Offset { get; set; }
    [Export] public bool TintUsingColor { get; set; }

    [ExportGroup("Randomness")]
    [Export] public Vector2 RandomAngleRange { get; set; }
    [Export] public Vector2 RandomScaleRange { get; set; } = Vector2.One;

    private BrushUtils.ImageDataCache _imageDataCache;

    public override void Process(BrushDefinition brushDefinition)
    {
        base.Process(brushDefinition);

        _imageDataCache = BrushUtils.ImageDataCacher.CreateOrGet(Texture, _imageDataCache);
    }

    public override void Draw(BrushDefinition brushDefinition, CanvasItem canvasItem)
    {
        base.Draw(brushDefinition, canvasItem);

        var transform = Transform2D.Identity
                .Translated(Offset)
                .Scaled(Vector2.One * (float)GD.RandRange(RandomScaleRange.X, RandomScaleRange.Y))
                .Rotated(Mathf.DegToRad((float)GD.RandRange(RandomAngleRange.X, RandomAngleRange.Y)))
                .Translated(brushDefinition.EvaluatedPosition)
            ;
        canvasItem.DrawSetTransformMatrix(transform);

        BrushUtils.Colorize(_imageDataCache, brushDefinition.EvaluatedColor);
        canvasItem.DrawTextureRect(_imageDataCache.ImageTexture, new Rect2(Vector2.Zero, Size), false, Colors.White);

        canvasItem.DrawSetTransformMatrix(Transform2D.Identity);
    }
}
