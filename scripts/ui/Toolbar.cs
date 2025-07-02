using Godot;
using System;

public partial class Toolbar : MarginContainer
{
    [Signal] public delegate void OnToolSelectedEventHandler(ToolDefinition tool);

    [Export] public ToolState ToolState { get; private set; }

    [Export] public Godot.Collections.Array<ToolbarButton> ToolbarButtons;

    public void SetToolState(ToolState toolState)
    {
        ToolState = toolState;
        foreach (var toolbarButton in ToolbarButtons)
        {
            toolbarButton.ToolState = toolState;
        }
    }

    public void SetDrawingTool(ToolDefinition tool)
    {
        ToolState.SetTool(tool);
        EmitSignalOnToolSelected(tool);
    }

}
