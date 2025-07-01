using Godot;
using System;
using Godot.Collections;

public partial class Topbar : MarginContainer
{
    [Signal] public delegate void OnSettingsChangeEventHandler(ToolState.DrawingTools tool);

    [Export] public ToolState ToolState { get; private set; }

    [Export] public ColorPaletteBar ColorPaletteBar { get; private set; }

    [Export] public Array<Control> MenuBars { get; private set; } = new Array<Control>();
    [Export] public Control CurrentMenuBar { get; private set; }
    [Export] public Control InkMenu { get; private set; }
    [Export] public Control ToolOptionsMenu { get; private set; }

    [Export] public PaintColorButton OpenColorsButton { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        foreach (var menuBar in MenuBars)
        {
            menuBar.Visible = true;
            float moveTarget = -500f;
            if (menuBar == CurrentMenuBar) moveTarget = 0f;
            menuBar.Position = menuBar.Position with { Y = moveTarget };
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        if (ToolState is not null) ToolState.ColorChanged -= SetColorMenuButtonColor;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        foreach (var menuBar in MenuBars)
        {
            float moveTarget = -500f;
            if (menuBar == CurrentMenuBar) moveTarget = 0f;
            menuBar.Position = menuBar.Position with { Y = moveTarget };
        }
    }

    public void SetToolState(ToolState toolState)
    {
        if (ToolState is not null) ToolState.ColorChanged -= SetColorMenuButtonColor;

        ToolState = toolState;
        ColorPaletteBar.SetToolState(toolState);
        OpenColorsButton.SetToolState(toolState);

        ToolState.ColorChanged += SetColorMenuButtonColor;
    }

    private void SetColorMenuButtonColor(Color color)
    {
        OpenColorsButton.SetDisplayColor(color);
    }

    public void SwitchToColorsMenu()
    {
        CurrentMenuBar = InkMenu;
    }

    public void SwitchToToolOptionsMenu()
    {
        CurrentMenuBar = ToolOptionsMenu;
    }
}
