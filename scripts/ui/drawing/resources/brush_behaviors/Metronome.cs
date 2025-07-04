using Godot;
using System;

[GlobalClass]
public partial class Metronome : BrushBehavior
{
    [Export] public float Interval = 1.0f;

    private float _lastTriggerTime = 0f;
    private bool _triggered = false;

    public override void Process(DrawState drawState, double delta)
    {
        base.Process(drawState, delta);

        if (_triggered) return;
        if (drawState.DrawingTime - _lastTriggerTime > Interval)
        {
            _triggered = true;
            _lastTriggerTime = drawState.DrawingTime;
        }
    }

    public override bool CanContinueDraw(DrawState drawState)
    {
        if (!_triggered) return false;
        _triggered = false;
        return true;
    }
}
