using Godot;
using System;

[GlobalClass]
public partial class OffsetRandomRadius : BrushBehavior
{
    [Export] public float InnerRadius = 0;
    [Export] public float OuterRadius = 16;

    private Vector2 _offset;

    public override void Process(DrawState drawState, double delta)
    {
        base.Process(drawState, delta);

        float angle = GD.Randf() * 2 * Mathf.Pi;
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        offset *= (float)GD.RandRange(InnerRadius, OuterRadius);
        drawState.EvaluatedPosition += offset;
    }

    public override void Draw(DrawState drawState, CanvasItem canvasItem)
    {
        base.Draw(drawState, canvasItem);

        float angle = GD.Randf() * 2 * Mathf.Pi;
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        offset *= (float)GD.RandRange(InnerRadius, OuterRadius);
        drawState.EvaluatedPosition += offset;
    }
}
