using Godot;
using System;

[GlobalClass]
public partial class ToolState : Resource
{
    [Signal] public delegate void ToolChangedEventHandler(ToolDefinition toolDefinition);
    [Signal] public delegate void BrushChangedEventHandler(BrushDefinition brushDefinition);
    [Signal] public delegate void InkChangedEventHandler(InkDefinition ink);
    [Signal] public delegate void StencilChangedEventHandler(StencilData stencilData);
    [Export] public ToolDefinition ToolDefinition { get; private set; }
    [Export] public InkDefinition InkDefinition { get; private set; }
    [Export] public StencilData StencilData { get; private set; }

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

    public void SetStencil(StencilData stencilData)
    {
        if (stencilData == StencilData) return;
        StencilData = stencilData;
        EmitSignalStencilChanged(stencilData);
    }
}
