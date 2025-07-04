using Godot;
using System;

[GlobalClass]
public partial class OnOffTimer : BrushBehavior
{
    [Export] public float CycleDuration = 0.5f;
    public override bool CanContinueDraw(DrawState drawState)
    {
        return (drawState.DrawingTime % CycleDuration) <= CycleDuration * 0.5f;
    }
}
