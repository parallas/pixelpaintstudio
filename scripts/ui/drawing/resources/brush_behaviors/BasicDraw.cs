using Godot;
using System;

[GlobalClass]
public partial class BasicDraw : BrushBehavior
{
    [Export] public float Radius = 10f;

    public override void Draw(BrushDefinition brushDefinition, CanvasItem canvasItem)
    {
        base.Draw(brushDefinition, canvasItem);

        canvasItem.DrawCircle(brushDefinition.LastEvaluatedPosition, Radius, brushDefinition.EvaluatedColor);
        canvasItem.DrawLine(brushDefinition.LastEvaluatedPosition, brushDefinition.EvaluatedPosition, brushDefinition.EvaluatedColor, Radius * 2);
        canvasItem.DrawCircle(brushDefinition.EvaluatedPosition, Radius, brushDefinition.EvaluatedColor);
    }
}
