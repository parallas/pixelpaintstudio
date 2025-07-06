using Godot;
using System;

[GlobalClass]
public partial class ToolState : Resource
{
    [Signal] public delegate void ToolChangedEventHandler(ToolDefinition toolDefinition);
    [Signal] public delegate void BrushChangedEventHandler(BrushDefinition brushDefinition);
    [Signal] public delegate void InkChangedEventHandler(InkDefinition ink);
    [Export] public ToolDefinition ToolDefinition { get; private set; }
    [Export] public InkDefinition InkDefinition { get; private set; }

    public void SetTool(ToolDefinition tool)
    {
        if (tool == ToolDefinition) return;
        ToolDefinition = tool;
        EmitSignalToolChanged(tool);
    }

    public void SetInk(InkDefinition ink)
    {
        if (ink == InkDefinition) return;
        InkDefinition = ink;
        EmitSignalInkChanged(ink);
    }
}
