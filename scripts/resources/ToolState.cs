using Godot;
using System;

[GlobalClass]
public partial class ToolState : Resource
{
    [Signal] public delegate void ToolChangedEventHandler(ToolDefinition toolDefinition);
    [Signal] public delegate void BrushChangedEventHandler(BrushDefinition brushDefinition);
    [Signal] public delegate void ColorChangedEventHandler(Color color);
    [Export] public ToolDefinition ToolDefinition { get; private set; }
    [Export] public Color BrushColor { get; private set; } = Colors.Red;

    public void SetTool(ToolDefinition tool)
    {
        if (tool == ToolDefinition) return;
        ToolDefinition = tool;
        EmitSignalToolChanged(tool);
    }

    public void SetColor(Color color)
    {
        if (color == BrushColor) return;
        BrushColor = color;
        EmitSignalColorChanged(color);
    }
}
