using Godot;
using System;

[GlobalClass]
public partial class OffsetRandomXY : BrushBehavior
{
    [Export] public bool FromCenter = true;
    [Export] public Vector2 OffsetMin = new Vector2();
    [Export] public Vector2 OffsetMax = new Vector2();

    public override void Process(DrawState drawState, double delta)
    {
        base.Process(drawState, delta);

        var offset = new Vector2(
            (float)GD.RandRange(OffsetMin.X, OffsetMax.X),
            (float)GD.RandRange(OffsetMin.Y, OffsetMax.Y)
        );
        if (!FromCenter) offset -= offset * 0.5f;
        drawState.EvaluatedPosition += offset;
    }

    public override void Draw(DrawState drawState, CanvasItem canvasItem)
    {
        base.Draw(drawState, canvasItem);

        var offset = new Vector2(
            (float)GD.RandRange(OffsetMin.X, OffsetMax.X),
            (float)GD.RandRange(OffsetMin.Y, OffsetMax.Y)
        );
        if (!FromCenter) offset -= offset * 0.5f;
        drawState.EvaluatedPosition += offset;
    }
}
