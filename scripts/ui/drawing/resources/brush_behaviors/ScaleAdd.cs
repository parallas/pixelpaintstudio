using Godot;
using System;

[GlobalClass]
public partial class ScaleAdd : BrushBehavior
{
    [Export] public float AddPerSecond = 1f;

    public override void Process(DrawState drawState, double delta)
    {
        base.Process(drawState, delta);

        drawState.EvaluatedScale += Vector2.One * (AddPerSecond * (float)delta);
    }
}
