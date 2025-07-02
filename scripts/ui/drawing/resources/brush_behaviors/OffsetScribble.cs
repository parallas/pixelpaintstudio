using Godot;
using System;

[GlobalClass]
public partial class OffsetScribble : BrushBehavior
{
    [Export] private float Radius = 7.5f;
    [Export] private bool UseEvaluatedDirection = false;

    private Vector2 _direction = Vector2.Zero;

    public override bool CanContinueProcess(BrushDefinition brushDefinition)
    {
        return brushDefinition.CursorStartPosition.DistanceSquaredTo(brushDefinition.CursorPosition) > 0.01;
    }

    public override void Process(BrushDefinition brushDefinition, double delta)
    {
        base.Process(brushDefinition, delta);

        var start = UseEvaluatedDirection ? brushDefinition.LastEvaluatedPosition : brushDefinition.LastCursorPosition;
        var end = UseEvaluatedDirection ? brushDefinition.EvaluatedPosition : brushDefinition.CursorPosition;
        if (start.DistanceSquaredTo(end) > 1f) _direction = (start - end).Normalized();
        // Vector2 offsetDirection = _direction.Rotated(Mathf.Pi * 0.5f);
        Vector2 offsetDirection = new Vector2(_direction.Y, -_direction.X);

        offsetDirection *= Radius;

        brushDefinition.EvaluatedPosition += offsetDirection;
    }
}
