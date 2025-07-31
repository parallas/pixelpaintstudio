using Godot;
using System;
using Parallas;

public partial class Toolbar : MarginContainer
{
    [Signal] public delegate void OnToolSelectedEventHandler(ToolDefinition tool);
    [Signal] public delegate void OnMenuOpenStateChangeEventHandler(bool state);

    [Export] public ToolState ToolState { get; private set; }
    [Export] public Control SettingsBarNode { get; private set; }

    [Export] public Godot.Collections.Array<ToolbarButton> ToolbarButtons;

    public bool IsMenuOpen { get; private set; }

    public override void _Process(double delta)
    {
        base._Process(delta);

        SettingsBarNode.Position = SettingsBarNode.Position with
        {
            Y = MathUtil.ExpDecay(SettingsBarNode.Position.Y, IsMenuOpen ? -200 : 0, 20f, (float)delta)
        };
    }

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

    public void SetMenuOpen(bool state)
    {
        IsMenuOpen = state;
        EmitSignalOnMenuOpenStateChange(state);
    }

}
