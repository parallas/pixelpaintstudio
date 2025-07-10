using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class TempState : BrushBehavior
{
    [Export] private bool _resampleDistance = false;
    [Export] private Array<BrushBehavior> _brushBehaviors;

    public override void Process(DrawState drawState, double delta)
    {
        base.Process(drawState, delta);

        var dist = drawState.EvaluatedVelocity.Length() + 1;
        for (int i = 0; i < dist; i++)
        {
            var state = drawState.Duplicate();
            if (_resampleDistance)
                state.EvaluatedPosition = drawState.LastEvaluatedPosition.Lerp(drawState.EvaluatedPosition, i / dist);
            foreach (var brushBehavior in _brushBehaviors)
            {
                brushBehavior.Process(state, delta);
            }
        }
    }

    public override void Draw(DrawState drawState, CanvasItem canvasItem)
    {
        base.Draw(drawState, canvasItem);

        var dist = drawState.EvaluatedVelocity.Length() + 1;
        for (int i = 0; i < dist; i++)
        {
            var state = drawState.Duplicate();
            if (_resampleDistance)
            {
                float percent = i / dist;
                state.EvaluatedPosition = drawState.LastEvaluatedPosition.Lerp(drawState.EvaluatedPosition, percent);
                state.EvaluatedScale = drawState.LastEvaluatedScale.Lerp(drawState.EvaluatedScale, percent);
            }
            foreach (var brushBehavior in _brushBehaviors)
            {
                brushBehavior.Draw(state, canvasItem);
            }
        }

    }
}
