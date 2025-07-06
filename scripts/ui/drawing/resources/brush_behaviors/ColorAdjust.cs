using Godot;
using System;

[GlobalClass]
public partial class ColorAdjust : BrushBehavior
{
    [Export] private float _hueChange = 0f;
    [Export] private float _saturationMultiplier = 1f;
    [Export] private float _valueMultiplier = 1f;

    public override void Draw(DrawState drawState, CanvasItem canvasItem)
    {
        base.Draw(drawState, canvasItem);

        drawState.EvaluatedColor.ToHsv(out var hue, out var saturation, out var value);
        hue += _hueChange;
        saturation *= _saturationMultiplier;
        value *= _valueMultiplier;

        drawState.EvaluatedColor = Color.FromHsv(hue, saturation, value);
    }
}
