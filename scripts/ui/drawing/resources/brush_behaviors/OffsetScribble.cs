using Godot;
using System;

[GlobalClass]
public partial class OffsetScribble : BrushBehavior
{
    [Export] private float Radius = 7.5f;
    [Export] private bool UseEvaluatedDirection = false;

    private Vector2 _direction = Vector2.Zero;

    public override bool CanContinueProcess(DrawState drawState)
    {
        return drawState.CursorStartPosition.DistanceSquaredTo(drawState.CursorPosition) > 0.01;
    }

    public override void Process(DrawState drawState, double delta)
    {
        base.Process(drawState, delta);

        var start = UseEvaluatedDirection ? drawState.LastEvaluatedPosition : drawState.LastCursorPosition;
        var end = UseEvaluatedDirection ? drawState.EvaluatedPosition : drawState.CursorPosition;
        if (start.DistanceSquaredTo(end) > 1f) _direction = (start - end).Normalized();
        // Vector2 offsetDirection = _direction.Rotated(Mathf.Pi * 0.5f);
        Vector2 offsetDirection = new Vector2(_direction.Y, -_direction.X);

        offsetDirection *= Radius;

        drawState.EvaluatedPosition += offsetDirection;
    }
}
