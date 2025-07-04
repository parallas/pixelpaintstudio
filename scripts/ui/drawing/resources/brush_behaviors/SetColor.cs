using Godot;
using System;

[GlobalClass]
public partial class SetColor : BrushBehavior
{
    [Export] public Color Color = Colors.Red;
    public override void Draw(DrawState drawState, CanvasItem canvasItem)
    {
        base.Draw(drawState, canvasItem);
        drawState.EvaluatedColor = Color;
    }
}
