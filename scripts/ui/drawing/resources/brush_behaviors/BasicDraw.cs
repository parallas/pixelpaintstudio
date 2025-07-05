using Godot;
using System;
using System.Collections.Generic;
using Parallas;

[GlobalClass]
public partial class BasicDraw : BrushBehavior
{
    [Export] public float Radius = 10f;

    private readonly List<Vector2> _points = [];
    private readonly List<Vector2> _velocities = [];

    public override void Initialize(Vector2 cursorPosition, Color cursorColor)
    {
        base.Initialize(cursorPosition, cursorColor);

        _points.Clear();
        _velocities.Clear();
    }

    public override void Draw(DrawState drawState, CanvasItem canvasItem)
    {
        base.Draw(drawState, canvasItem);
        _points.Add(drawState.EvaluatedPosition);
        _velocities.Add(_velocities.Count >= 2 ? (_points[^1] - _points[^2]) : Vector2.Zero);

        canvasItem.DrawCircle(drawState.LastEvaluatedPosition, Radius, drawState.EvaluatedColor);

        if (_points.Count >= 2)
        {
            int maxSteps = 10;
            var velocityPrev = _velocities[^2];
            var velocity = _velocities[^1];
            // var distance = _points[^1].DistanceTo(_points[^2]);

            Vector2[] blendPoints = new Vector2[maxSteps];
            for (int i = 0; i < maxSteps; i++)
            {
                float percentPerStep = 1f / maxSteps;
                float percentNext = (i + 1) * percentPerStep;

                var bezierPoint = MathUtil.BezierPoint(
                    percentNext,
                    _points[^2],
                    _points[^2] + velocityPrev,
                    _points[^1]
                );

                blendPoints[i] = bezierPoint;
            }

            var prevPoint = _points[^2];
            for (int i = 0; i < blendPoints.Length; i++)
            {
                var point = blendPoints[i];
                canvasItem.DrawLine(
                    prevPoint,
                    point,
                    Colors.Black.Lerp(drawState.EvaluatedColor, (float)i / blendPoints.Length),
                    2
                );
                // canvasItem.DrawCircle(midPoint2, 4, Colors.Black.Lerp(drawState.EvaluatedColor, percent));
                prevPoint = point;
            }
        }
        canvasItem.DrawCircle(_points[^1], Radius, drawState.EvaluatedColor);

    }
}
