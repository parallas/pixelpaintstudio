using Godot;
using System;

[GlobalClass]
public partial class SetColorFromPalette : BrushBehavior
{
    public override void Draw(DrawState drawState, CanvasItem canvasItem)
    {
        base.Draw(drawState, canvasItem);
        drawState.EvaluatedColor = drawState.CursorColor;
    }
}
