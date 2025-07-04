using Godot;
using System;

[GlobalClass]
public partial class BasicDraw : BrushBehavior
{
    [Export] public float Radius = 10f;

    public override void Draw(DrawState drawState, CanvasItem canvasItem)
    {
        base.Draw(drawState, canvasItem);

        canvasItem.DrawCircle(drawState.LastEvaluatedPosition, Radius, drawState.EvaluatedColor);
        canvasItem.DrawLine(drawState.LastEvaluatedPosition, drawState.EvaluatedPosition, drawState.EvaluatedColor, Radius * 2);
        canvasItem.DrawCircle(drawState.EvaluatedPosition, Radius, drawState.EvaluatedColor);
    }
}
