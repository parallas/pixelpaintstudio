using Godot;
using System;

[GlobalClass]
public partial class ToolState : Resource
{
    [Signal] public delegate void DrawingToolChangedEventHandler(DrawingTools drawingTool);
    public enum DrawingTools
    {
        PenSmall,
        PenMedium,
        PenLarge,
        BubbleWand,
        Stamp,
        Sticker,
        Zoom,
    }
    [Export] public DrawingTools DrawingTool { get; private set; } = DrawingTools.PenMedium;

    public void SetDrawingTool(DrawingTools tool)
    {
        if (DrawingTool == tool) return;
        DrawingTool = tool;
        EmitSignalDrawingToolChanged(tool);
    }
}
