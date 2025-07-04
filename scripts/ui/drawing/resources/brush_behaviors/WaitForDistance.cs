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

    public override bool CanContinueProcess(DrawState drawState)
    {
        if (!BlockProcessInsteadOfDraw) return true;
        var distance = drawState.EvaluatedPosition.DistanceTo(drawState.LastEvaluatedPosition);
        _accumulatedDistance += distance;
        if (BlockActualDistance)
        {
            _accumulatedDistance = drawState.EvaluatedPosition.DistanceTo(_lastPassedPosition);
        }

        bool pass = _accumulatedDistance >= Distance;
        if (!pass) return false;

        _accumulatedDistance -= Distance;
        _lastPassedPosition = drawState.EvaluatedPosition;
        return true;
    }

    public override bool CanContinueDraw(DrawState drawState)
    {
        if (BlockProcessInsteadOfDraw) return true;
        var distance = drawState.EvaluatedPosition.DistanceTo(drawState.LastEvaluatedPosition);
        _accumulatedDistance += distance;
        if (BlockActualDistance)
        {
            _accumulatedDistance = drawState.EvaluatedPosition.DistanceTo(_lastPassedPosition);
        }

        bool pass = _accumulatedDistance >= Distance;
        if (!pass) return false;

        _accumulatedDistance -= Distance;
        _lastPassedPosition = drawState.EvaluatedPosition;
        return true;
    }
}
