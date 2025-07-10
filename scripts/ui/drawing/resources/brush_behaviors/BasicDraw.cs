using Godot;
using System;
using Parallas;

[GlobalClass]
public partial class BasicDraw : BrushBehavior
{
    [Export] public float Radius = 10f;

    public override void Draw(DrawState drawState, CanvasItem canvasItem)
    {
        base.Draw(drawState, canvasItem);

        var linePoints = Geometry2D.BresenhamLine(drawState.LastEvaluatedPosition.ToVector2I(), drawState.EvaluatedPosition.ToVector2I());
        for (int i = 0; i < linePoints.Count; i++)
        {
            float percent = (float)i / (linePoints.Count);
            float scaleAmount = Mathf.Lerp(drawState.LastEvaluatedScaleUniform, drawState.EvaluatedScaleUniform, percent);
            Color color = drawState.LastEvaluatedColor.Lerp(drawState.EvaluatedColor, percent);
            canvasItem.DrawCircle(linePoints[i], Radius * scaleAmount, color);
        }

        canvasItem.DrawCircle(
            drawState.EvaluatedPosition,
            Radius * drawState.EvaluatedScaleUniform,
            drawState.EvaluatedColor
        );
    }
}
