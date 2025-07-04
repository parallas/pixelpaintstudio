using Godot;
using System;

[GlobalClass]
public abstract partial class BrushBehavior : Resource
{
    public virtual void Initialize(Vector2 cursorPosition, Color cursorColor) { }
    public virtual void Process(DrawState drawState, double delta) {}
    public virtual void Draw(DrawState drawState, CanvasItem canvasItem) {}
    public virtual bool CanContinueProcess(DrawState drawState) => true;
    public virtual bool CanContinueDraw(DrawState drawState) => true;
}
