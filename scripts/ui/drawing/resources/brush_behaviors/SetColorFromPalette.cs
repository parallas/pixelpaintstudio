using Godot;
using System;

[GlobalClass]
public partial class SetColorFromPalette : BrushBehavior
{
    public override void Process(BrushDefinition brushDefinition)
    {
        base.Process(brushDefinition);

        brushDefinition.EvaluatedColor = brushDefinition.CursorColor;
    }
}
