using Godot;
using System;

[GlobalClass]
public partial class SnapToCursor : BrushBehavior
{
    public override void Process(BrushDefinition brushDefinition, double delta)
    {
        base.Process(brushDefinition, delta);

        brushDefinition.EvaluatedPosition = brushDefinition.CursorPosition;
    }
}
