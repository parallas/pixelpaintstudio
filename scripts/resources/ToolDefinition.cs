using Godot;
using System;

[GlobalClass]
public partial class ToolDefinition : Resource
{
    [Export] public BrushDefinition BrushDefinition { get; private set; }
    [Export] public PackedScene ModelScene { get; private set; }
}
