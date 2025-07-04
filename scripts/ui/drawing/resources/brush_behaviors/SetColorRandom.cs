using Godot;
using System;

[GlobalClass]
public partial class SetColorRandom : BrushBehavior
{
    [Export] public Gradient Gradient;

    public override void Draw(DrawState drawState, CanvasItem canvasItem)
    {
        base.Draw(drawState, canvasItem);
        drawState.EvaluatedColor = Gradient.Sample(Random.Shared.NextSingle());
    }
}
