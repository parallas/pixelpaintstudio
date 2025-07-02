using Godot;
using System;
using Parallas;

[GlobalClass]
public partial class SpringToCursor : BrushBehavior
{
    [Export] private float _dampingRatio = 0.5f;
    [Export] private float _frequency = 5f;

    private Vector2 _springVelocity = Vector2.Zero;

    public override void Initialize(Vector2 cursorPosition, Color cursorColor)
    {
        base.Initialize(cursorPosition, cursorColor);

        _springVelocity = Vector2.Zero;
    }

    public override void Process(BrushDefinition brushDefinition, double delta)
    {
        base.Process(brushDefinition, delta);

        MathUtil.Spring(ref brushDefinition.EvaluatedPosition, ref _springVelocity,
            brushDefinition.CursorPosition, _dampingRatio, _frequency, (float)delta);
    }
}
