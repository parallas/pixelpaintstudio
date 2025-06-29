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

    public CanvasItem CanvasItem;
    public Vector2 CursorPosition;
    public Vector2 CursorStartPosition;
    public Vector2 LastCursorPosition;
    public Vector2 EvaluatedPosition;
    public Vector2 LastEvaluatedPosition;
    public float DrawingTime;

    public enum DrawStates
    {
        Start,
        Draw,
        Finish,
    }
    public DrawStates DrawState;

    private void Deinitialize()
    {
        CanvasItem.Draw -= Draw;
    }

    public void Start(CanvasItem canvasItem, Vector2 cursorPosition)
    {
        DrawState = DrawStates.Start;
        CanvasItem = canvasItem;
        canvasItem.Draw += Draw;
        foreach (var brushBehavior in Behaviors)
        {
            brushBehavior.Initialize();
        }
        foreach (var brushBehavior in StartBehaviors)
        {
            brushBehavior.Initialize();
        }
        foreach (var brushBehavior in FinishBehaviors)
        {
            brushBehavior.Initialize();
        }

        CursorPosition = cursorPosition;
        CursorStartPosition = cursorPosition;
        LastCursorPosition = cursorPosition;
        EvaluatedPosition = cursorPosition;
        LastEvaluatedPosition = cursorPosition;
        DrawingTime = 0f;

        // Run behavior logic
        foreach (var brushBehavior in StartBehaviors)
        {
            brushBehavior.Process(this);
            if (!brushBehavior.CanContinueProcess(this)) break;
        }

        CanvasItem.QueueRedraw();
    }

    public void Process(Vector2 cursorPosition, double deltaTime)
    {
        CursorPosition = cursorPosition;

        // Run behavior logic
        foreach (var brushBehavior in Behaviors)
        {
            brushBehavior.Process(this);
            if (!brushBehavior.CanContinueProcess(this)) break;
        }

        CanvasItem.QueueRedraw();

        DrawingTime += (float)deltaTime;
    }

    public void Finish(Vector2 cursorPosition)
    {
        DrawState = DrawStates.Finish;
        CursorPosition = cursorPosition;

        // Run behavior logic
        foreach (var brushBehavior in FinishBehaviors)
        {
            brushBehavior.Process(this);
            if (!brushBehavior.CanContinueProcess(this)) break;
        }

        CanvasItem.QueueRedraw();
    }

    public void Draw()
    {
        // Run drawing logic
        var behaviorsToDraw = DrawState switch
        {
            DrawStates.Start => StartBehaviors,
            DrawStates.Draw => Behaviors,
            DrawStates.Finish => FinishBehaviors,
            _ => Behaviors
        };

        foreach (var brushBehavior in behaviorsToDraw)
        {
            brushBehavior.Draw(this, CanvasItem);
            if (!brushBehavior.CanContinueDraw(this)) break;
        }

        // Set the last frame properties
        LastCursorPosition = CursorPosition;
        LastEvaluatedPosition = EvaluatedPosition;

        if (DrawState == DrawStates.Start) DrawState = DrawStates.Draw;
        if (DrawState == DrawStates.Finish) Deinitialize();
    }
}
