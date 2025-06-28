using Godot;
using System;

[GlobalClass]
public partial class ToolState : Resource
{
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
    [Export] public DrawingTools DrawingTool = DrawingTools.PenMedium;
}
