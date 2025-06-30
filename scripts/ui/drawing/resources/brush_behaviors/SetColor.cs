using Godot;
using System;

[GlobalClass]
public partial class SetColor : BrushBehavior
{
    [Export] public Color Color = Colors.Red;
    public override void Draw(BrushDefinition brushDefinition, CanvasItem canvasItem)
    {
        base.Draw(brushDefinition, canvasItem);
        brushDefinition.EvaluatedColor = Color;
    }
}
