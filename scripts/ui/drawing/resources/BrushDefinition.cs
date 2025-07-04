using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class BrushDefinition : Resource
{
    [Export] public string Name;
    [Export] public Array<BrushBehavior> Behaviors;
    [Export] public Array<BrushBehavior> StartBehaviors;
    [Export] public Array<BrushBehavior> FinishBehaviors;

    public void Start(DrawState drawState)
    {
        foreach (var brushBehavior in Behaviors)
        {
            brushBehavior.Initialize(drawState.CursorPosition, drawState.CursorColor);
        }
        foreach (var brushBehavior in StartBehaviors)
        {
            brushBehavior.Initialize(drawState.CursorPosition, drawState.CursorColor);
        }
        foreach (var brushBehavior in FinishBehaviors)
        {
            brushBehavior.Initialize(drawState.CursorPosition, drawState.CursorColor);
        }

        // Run behavior logic
        foreach (var brushBehavior in StartBehaviors)
        {
            brushBehavior.Process(drawState, 0);
            if (!brushBehavior.CanContinueProcess(drawState)) break;
        }
    }

    public void Process(DrawState drawState, double delta)
    {
        // Run behavior logic
        foreach (var brushBehavior in Behaviors)
        {
            brushBehavior.Process(drawState, delta);
            if (!brushBehavior.CanContinueProcess(drawState)) break;
        }
    }

    public void Finish(DrawState drawState, double delta)
    {
        // Run behavior logic
        foreach (var brushBehavior in FinishBehaviors)
        {
            brushBehavior.Process(drawState, delta);
            if (!brushBehavior.CanContinueProcess(drawState)) break;
        }
    }

    public void Draw(DrawState drawState)
    {
        // Run drawing logic
        var behaviorsToDraw = drawState.State switch
        {
            DrawState.States.Start => StartBehaviors,
            DrawState.States.Draw => Behaviors,
            DrawState.States.Finish => FinishBehaviors,
            _ => Behaviors
        };

        foreach (var brushBehavior in behaviorsToDraw)
        {
            brushBehavior.Draw(drawState, drawState.CanvasItem);
            if (!brushBehavior.CanContinueDraw(drawState)) break;
        }
    }
}
