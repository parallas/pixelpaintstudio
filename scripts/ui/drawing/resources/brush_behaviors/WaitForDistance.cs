using Godot;
using System;

[GlobalClass]
public partial class WaitForDistance : BrushBehavior
{
    [Export] public float Distance = 30f;
    [Export] public bool BlockActualDistance = false;
    [Export] public bool BlockProcessInsteadOfDraw = true;

    private float _accumulatedDistance;
    private Vector2 _lastPassedPosition = Vector2.Zero;

    public override void Initialize(Vector2 cursorPosition, Color cursorColor)
    {
        base.Initialize(cursorPosition, cursorColor);
        _lastPassedPosition = cursorPosition;
    }

    public override bool CanContinueProcess(BrushDefinition brushDefinition)
    {
        if (!BlockProcessInsteadOfDraw) return true;
        var distance = brushDefinition.EvaluatedPosition.DistanceTo(brushDefinition.LastEvaluatedPosition);
        _accumulatedDistance += distance;
        if (BlockActualDistance)
        {
            _accumulatedDistance = brushDefinition.EvaluatedPosition.DistanceTo(_lastPassedPosition);
        }

        bool pass = _accumulatedDistance >= Distance;
        if (!pass) return false;

        _accumulatedDistance -= Distance;
        _lastPassedPosition = brushDefinition.EvaluatedPosition;
        return true;
    }

    public override bool CanContinueDraw(BrushDefinition brushDefinition)
    {
        if (BlockProcessInsteadOfDraw) return true;
        var distance = brushDefinition.EvaluatedPosition.DistanceTo(brushDefinition.LastEvaluatedPosition);
        _accumulatedDistance += distance;
        if (BlockActualDistance)
        {
            _accumulatedDistance = brushDefinition.EvaluatedPosition.DistanceTo(_lastPassedPosition);
        }

        bool pass = _accumulatedDistance >= Distance;
        if (!pass) return false;

        _accumulatedDistance -= Distance;
        _lastPassedPosition = brushDefinition.EvaluatedPosition;
        return true;
    }
}
