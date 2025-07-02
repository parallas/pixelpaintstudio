using Godot;
using System;

[GlobalClass]
public partial class OffsetRandomRadius : BrushBehavior
{
    [Export] public float InnerRadius = 0;
    [Export] public float OuterRadius = 16;

    public override void Process(BrushDefinition brushDefinition, double delta)
    {
        base.Process(brushDefinition, delta);

        float angle = GD.Randf() * 2 * Mathf.Pi;
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        offset *= (float)GD.RandRange(InnerRadius, OuterRadius);
        brushDefinition.EvaluatedPosition += offset;
    }
}
