using Godot;
using System;

[GlobalClass]
public partial class Metronome : BrushBehavior
{
    [Export] public float Interval = 1.0f;

    private float _lastTriggerTime = 0f;
    private bool _triggered = false;

    public override void Process(BrushDefinition brushDefinition)
    {
        base.Process(brushDefinition);

        if (_triggered) return;
        if (brushDefinition.DrawingTime - _lastTriggerTime > Interval)
        {
            _triggered = true;
            _lastTriggerTime = brushDefinition.DrawingTime;
        }
    }

    public override bool CanContinueDraw(BrushDefinition brushDefinition)
    {
        if (!_triggered) return false;
        _triggered = false;
        return true;
    }
}
