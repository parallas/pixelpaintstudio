using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class ShadedDraw : BrushBehavior
{
    [Export] public float Radius = 10f;

    private List<Vector2> _linePoints = new List<Vector2>();
    private List<Vector2> _linePointsLight = new List<Vector2>();
    private List<Vector2> _linePointsDark = new List<Vector2>();

    public override void Initialize(Vector2 cursorPosition, Color cursorColor)
    {
        base.Initialize(cursorPosition, cursorColor);

        _linePoints.Clear();
        _linePointsLight.Clear();
        _linePointsDark.Clear();
    }

    public override void Draw(BrushDefinition brushDefinition, CanvasItem canvasItem)
    {
        base.Draw(brushDefinition, canvasItem);

        Color colorLight = brushDefinition.EvaluatedColor.Lerp(Colors.White, 0.75f);
        Color colorDark = brushDefinition.EvaluatedColor.Lerp(Colors.Black, 0.75f);

        if (brushDefinition.EvaluatedPosition == brushDefinition.LastEvaluatedPosition) return;
        Vector2 direction = (brushDefinition.EvaluatedPosition - brushDefinition.LastEvaluatedPosition).Normalized();
        Vector2 offsetDirection = new Vector2(direction.Y, -direction.X);
        Vector2 offset = offsetDirection * Radius;

        _linePoints.Add(brushDefinition.EvaluatedPosition);
        _linePointsLight.Add(brushDefinition.EvaluatedPosition - offset);
        _linePointsDark.Add(brushDefinition.EvaluatedPosition + offset);

        if (_linePoints.Count >= 2)
        {
            float rimWidth = 4f;
            canvasItem.DrawPolyline(_linePointsLight.ToArray(), colorLight, rimWidth, false);
            canvasItem.DrawPolyline(_linePointsDark.ToArray(), colorDark, rimWidth, false);
            canvasItem.DrawPolyline(_linePoints.ToArray(), brushDefinition.EvaluatedColor, Radius * 2f, false);
        }

        // var shadeAmount = Mathf.Abs((brushDefinition.EvaluatedPosition - brushDefinition.LastEvaluatedPosition).Normalized().Dot(Vector2.Up));
        // offset *= Mathf.Pow(shadeAmount, 1f);
        // canvasItem.DrawLine(brushDefinition.LastEvaluatedPosition + offset, brushDefinition.EvaluatedPosition + offset, colorLight, Radius * 2);
        // canvasItem.DrawLine(brushDefinition.LastEvaluatedPosition - offset, brushDefinition.EvaluatedPosition - offset, colorDark, Radius * 2);
        //
        // canvasItem.DrawCircle(brushDefinition.LastEvaluatedPosition, Radius, brushDefinition.EvaluatedColor);
        // canvasItem.DrawLine(brushDefinition.LastEvaluatedPosition, brushDefinition.EvaluatedPosition, brushDefinition.EvaluatedColor, Radius * 2);
        // canvasItem.DrawCircle(brushDefinition.EvaluatedPosition, Radius, brushDefinition.EvaluatedColor);
    }
}
