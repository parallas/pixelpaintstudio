using Godot;
using System;

[GlobalClass]
public partial class ToolDefinition : Resource
{
    [Export] public BrushDefinition BrushDefinition { get; private set; }
    [Export] public PackedScene ModelScene { get; private set; }
    [Export] public ToolMenus ToolMenu { get; private set; } = ToolMenus.Stencils;

    public enum ToolMenus
    {
        Stencils,
        Images,
        Wands
    }

    public void SetBrushDefinition(BrushDefinition brushDefinition)
    {
        BrushDefinition = brushDefinition;
    }
}
