using Godot;
using System;

[GlobalClass]
public partial class SnapToCursor : BrushBehavior
{
    public override void Process(DrawState drawState, double delta)
    {
        base.Process(drawState, delta);

        drawState.EvaluatedPosition = drawState.CursorPosition;
    }
}
