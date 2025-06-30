using Godot;
using System;

[GlobalClass]
public partial class WaitForDistance : BrushBehavior
{
    [Export] public float Distance = 30f;
    [Export] public bool BlockProcessInsteadOfDraw = true;

    private Vector2 _lastPassedPosition = Vector2.Zero;

    public override void Initialize(Vector2 cursorPosition, Color cursorColor)
    {
        base.Initialize(cursorPosition, cursorColor);
        _lastPassedPosition = cursorPosition;
    }

    public override bool CanContinueProcess(BrushDefinition brushDefinition)
    {
        var distance = brushDefinition.EvaluatedPosition.DistanceTo(_lastPassedPosition);
        bool pass = distance >= Distance;
        if (!BlockProcessInsteadOfDraw) return true;
        if (!pass) return false;

        _lastPassedPosition = brushDefinition.EvaluatedPosition;
        return true;
    }

    public override bool CanContinueDraw(BrushDefinition brushDefinition)
    {
        var distance = brushDefinition.EvaluatedPosition.DistanceTo(_lastPassedPosition);
        bool pass = distance >= Distance;
        if (BlockProcessInsteadOfDraw) return true;
        if (!pass) return false;

        _lastPassedPosition = brushDefinition.EvaluatedPosition;
        return true;
    }
}
