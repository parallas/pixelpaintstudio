using Godot;
using System;

[GlobalClass]
public partial class SnapToCursor : BrushBehavior
{
    public override void Process(BrushDefinition brushDefinition)
    {
        base.Process(brushDefinition);

        brushDefinition.EvaluatedPosition = brushDefinition.CursorPosition;
    }
}
