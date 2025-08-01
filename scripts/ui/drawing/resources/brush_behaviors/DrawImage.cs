using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class DrawImage : BrushBehavior
{
    [Export] public Array<Texture2D> Textures { get; set; }
    [Export] public Vector2 Size { get; set; }
    [Export] public Vector2 Offset { get; set; }
    [Export] public bool TintUsingColor { get; set; }
    [Export] public bool MultiplyByColor { get; set; }

    [ExportGroup("Randomness")]
    [Export] public Vector2 RandomAngleRange { get; set; }
    [Export] public Vector2 RandomScaleRange { get; set; } = Vector2.One;

    private BrushUtils.ImageDataCache _imageDataCache;

    public override void Process(DrawState drawState, double delta)
    {
        base.Process(drawState, delta);

        _imageDataCache =
            BrushUtils.ImageDataCacher.CreateOrGet(Textures[GD.RandRange(0, Textures.Count - 1)], _imageDataCache);
    }

    public override void Draw(DrawState drawState, CanvasItem canvasItem)
    {
        base.Draw(drawState, canvasItem);

        var scale = Vector2.One * (float)GD.RandRange(RandomScaleRange.X, RandomScaleRange.Y) * drawState.EvaluatedScaleUniform;

        var transform = Transform2D.Identity
                .Translated(Offset)
                .Scaled(scale)
                .Rotated(Mathf.DegToRad((float)GD.RandRange(RandomAngleRange.X, RandomAngleRange.Y)))
                .Translated(drawState.EvaluatedPosition)
            ;
        canvasItem.DrawSetTransformMatrix(transform);

        if (TintUsingColor) BrushUtils.ColorizeBasic(_imageDataCache, drawState.EvaluatedColor);
        Color color = MultiplyByColor ? drawState.EvaluatedColor : Colors.White;
        canvasItem.DrawTextureRect(_imageDataCache.ImageTexture, new Rect2(Vector2.Zero, Size), false, color);

        canvasItem.DrawSetTransformMatrix(Transform2D.Identity);
    }
}
