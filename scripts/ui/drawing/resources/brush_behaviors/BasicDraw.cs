using Godot;
using System;

[GlobalClass]
public partial class BasicDraw : BrushBehavior
{
    [Export] public float Radius = 10f;

    public override void Draw(DrawState drawState, CanvasItem canvasItem)
    {
        base.Draw(drawState, canvasItem);

        var radius = Radius * drawState.EvaluatedScaleUniform;

        canvasItem.DrawCircle(drawState.LastEvaluatedPosition, radius, drawState.EvaluatedColor);
        canvasItem.DrawLine(drawState.LastEvaluatedPosition, drawState.EvaluatedPosition, drawState.EvaluatedColor, radius * 2);
        canvasItem.DrawCircle(drawState.EvaluatedPosition, radius, drawState.EvaluatedColor);
    }
}
