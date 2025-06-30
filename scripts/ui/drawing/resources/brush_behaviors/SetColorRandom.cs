using Godot;
using System;

[GlobalClass]
public partial class SetColorRandom : BrushBehavior
{
    [Export] public Gradient Gradient;

    public override void Draw(BrushDefinition brushDefinition, CanvasItem canvasItem)
    {
        base.Draw(brushDefinition, canvasItem);
        brushDefinition.EvaluatedColor = Gradient.Sample(GD.Randf());
    }
}
