using Godot;
using System;

public partial class Topbar : MarginContainer
{
    [Signal] public delegate void OnSettingsChangeEventHandler(ToolState.DrawingTools tool);

    [Export] public ToolState ToolState { get; private set; }

    [Export] public ColorPaletteBar ColorPaletteBar { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        ColorPaletteBar.SetToolState(ToolState);
    }

    public void SetToolState(ToolState toolState)
    {
        ToolState = toolState;
        ColorPaletteBar.SetToolState(toolState);
    }
}
