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

    public override void Process(DrawState drawState, double delta)
    {
        base.Process(drawState, delta);

        MathUtil.Spring(ref drawState.EvaluatedPosition, ref _springVelocity,
            drawState.CursorPosition, _dampingRatio, _frequency, (float)delta);
    }
}
