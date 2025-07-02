using Godot;
using System;

[GlobalClass]
public abstract partial class BrushBehavior : Resource
{
    public virtual void Initialize(Vector2 cursorPosition, Color cursorColor) { }
    public virtual void Process(BrushDefinition brushDefinition, double delta) {}
    public virtual void Draw(BrushDefinition brushDefinition, CanvasItem canvasItem) {}
    public virtual bool CanContinueProcess(BrushDefinition brushDefinition) => true;
    public virtual bool CanContinueDraw(BrushDefinition brushDefinition) => true;
}
