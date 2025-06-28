using Godot;
using System;

public partial class Toolbar : MarginContainer
{
    [Signal] public delegate void OnToolSelectedEventHandler(ToolState.DrawingTools tool);

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

    public void SetDrawingTool(ToolState.DrawingTools tool)
    {
        ToolState.DrawingTool = tool;
        EmitSignalOnToolSelected(tool);
    }

}
