using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Parallas;

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

    public static InkDefinition FromColors(String[] colors, float expandPercent = 0f, bool halfOffset = false, int repeatCount = 1)
    {
        Color[] colorArray = colors.Select(s => new Color(s)).ToArray();
        return FromColors(colorArray, expandPercent, halfOffset, repeatCount);
    }

    public static InkDefinition FromColors(Color[] colors, float expandPercent = 0f, bool halfOffset = false, int repeatCount = 1)
    {
        var repeatedColors = new List<Color>();
        for (int i = 0; i < repeatCount; i++)
        {
            repeatedColors.AddRange(colors);
        }

        expandPercent = MathUtil.Clamp01(expandPercent) * 0.99f;
        List<float> offsetsList = new List<float>();
        List<Color> colorList = new List<Color>();

        float sliceSize = (1f / repeatedColors.Count);
        float expandedSliceSize = sliceSize * expandPercent;
        for (int i = 0; i < repeatedColors.Count; i++)
        {
            float offset = i * sliceSize;
            Color color = repeatedColors[i];
            offsetsList.Add(offset);
            colorList.Add(color);

            if (expandPercent > 0f)
            {
                float offsetWithMargin = i * sliceSize + expandedSliceSize;
                offsetsList.Add(offsetWithMargin);
                colorList.Add(color);
            }
        }

        if (halfOffset)
        {
            offsetsList.RemoveAt(0);
            offsetsList.Add(1);

            var firstColor = colorList[0];
            colorList.RemoveAt(0);
            colorList.Add(firstColor);

            for (int i = 0; i < offsetsList.Count; i++)
            {
                offsetsList[i] -= expandedSliceSize * 0.5f;
            }
        }
        else
        {
            colorList.Add(repeatedColors[0]);
            offsetsList.Add(1);
        }

        var result = new InkDefinition
        {
            ColorMode = InkDefinition.ColorModes.Ordered,
            Gradient = new Gradient
            {
                InterpolationColorSpace = Gradient.ColorSpace.LinearSrgb,
                InterpolationMode = Gradient.InterpolationModeEnum.Linear,
                Colors = colorList.ToArray(),
                Offsets = offsetsList.ToArray(),
            }
        };

        return result;
    }
}
