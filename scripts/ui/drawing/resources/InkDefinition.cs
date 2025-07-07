using Godot;
using System;

[GlobalClass]
public partial class InkDefinition : Resource
{
    [Export(PropertyHint.ColorNoAlpha)] public Color Color;
    [Export] public Gradient Gradient;
    [Export] public ColorModes ColorMode;
    [Export] public Texture2D IconTexture;
    public enum ColorModes
    {
        Single,
        Ordered,
        Random
    }

    public Color SampleColor(float drawTime)
    {
        switch (ColorMode)
        {
            case ColorModes.Single:
                return Color;
            case ColorModes.Ordered:
                return Gradient.Sample(drawTime % 1f);
            case ColorModes.Random:
                return Gradient.Sample(GD.Randf());
            default:
                return Color;
        }
    }
}
