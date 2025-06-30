using Godot;
using System;

[GlobalClass]
public partial class ToolState : Resource
{
    [Signal] public delegate void DrawingToolChangedEventHandler(DrawingTools drawingTool);
    [Signal] public delegate void BrushChangedEventHandler(BrushDefinition brushDefinition);
    [Signal] public delegate void ColorChangedEventHandler(Color color);
    public enum DrawingTools
    {
        PenSmall,
        PenMedium,
        PenLarge,
        BubbleWand,
        Sponge,
        Stamp,
        Sticker,
        Zoom,
        Wand,
    }
    [Export] public DrawingTools DrawingTool { get; private set; } = DrawingTools.PenMedium;
    [Export] public BrushDefinition BrushDefinition { get; private set; }
    [Export] public Color BrushColor { get; private set; }

    public void SetDrawingTool(DrawingTools tool)
    {
        if (DrawingTool == tool) return;
        DrawingTool = tool;
        EmitSignalDrawingToolChanged(tool);
    }

    public void SetBrush(BrushDefinition brushDefinition)
    {
        if (BrushDefinition == brushDefinition) return;
        BrushDefinition = brushDefinition;
        EmitSignalBrushChanged(brushDefinition);
    }

    public void SetColor(Color color)
    {
        if (color == BrushColor) return;
        BrushColor = color;
        EmitSignalColorChanged(color);
    }
}
