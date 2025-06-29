using Godot;
using System;

[GlobalClass]
public partial class SetColor : BrushBehavior
{
    [Export] public Color Color = Colors.Red;
    public override void Process(BrushDefinition brushDefinition)
    {
        base.Process(brushDefinition);

        brushDefinition.EvaluatedColor = Color;
    }
}
