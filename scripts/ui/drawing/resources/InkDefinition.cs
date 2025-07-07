using Godot;
using System;

[GlobalClass]
public partial class InkDefinition : Resource
{
    [Export(PropertyHint.ColorNoAlpha)] private Color _color;
    [Export] private Gradient _gradient;
    [Export] private ColorModes _colorMode;
    [Export] private Texture2D _iconTexture;
    public enum ColorModes
    {
        Single,
        Ordered,
        Random
    }

    public Color SampleColor(float drawTime)
    {
        switch (_colorMode)
        {
            case ColorModes.Single:
                return _color;
            case ColorModes.Ordered:
                return _gradient.Sample(drawTime % 1f);
            case ColorModes.Random:
                return _gradient.Sample(GD.Randf());
            default:
                return _color;
        }
    }
}
