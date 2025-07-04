using Godot;
using System;
using Godot.Collections;
using Parallas;

public partial class Topbar : MarginContainer
{
    [Signal] public delegate void OnSettingsChangeEventHandler(ToolDefinition tool);

    [Export] public ToolState ToolState { get; private set; }

    [Export] public ColorPaletteBar ColorPaletteBar { get; private set; }

    [Export] public Array<Control> MenuBars { get; private set; } = new Array<Control>();
    [Export] public Control CurrentMenuBar { get; private set; }
    [Export] public Control InkMenu { get; private set; }
    [Export] public Control ToolOptionsMenu { get; private set; }

    [Export] public PaintColorButton OpenColorsButton { get; private set; }

    private float _squashStretchAmount = 0f;
    private float _squashStretchVelocity = 0f;

    public override void _Ready()
    {
        base._Ready();
        foreach (var menuBar in MenuBars)
        {
            bool isCurrent = menuBar == CurrentMenuBar;
            menuBar.Visible = isCurrent;
            menuBar.MouseFilter = isCurrent ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
            menuBar.SetDrawBehindParent(true);
            menuBar.SetPivotOffset(new Vector2(0f, menuBar.Size.Y * 0.5f));
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

        SetColorMenuButtonColor(OpenColorsButton.ToolState?.BrushColor ?? Colors.Red);

        MathUtil.Spring(
            ref _squashStretchAmount,
            ref _squashStretchVelocity,
            0f,
            0.1f,
            33f,
            (float)delta
        );

        Vector2 squashStretch = MathUtil.SquashScale(1f + _squashStretchAmount * 0.1f).ToVector2();
        CurrentMenuBar.Scale = new Vector2(Mathf.Lerp(squashStretch.X, 1f, 0.7f), squashStretch.Y);

        foreach (var menuBar in MenuBars)
        {
            bool isCurrent = menuBar == CurrentMenuBar;
            menuBar.Visible = isCurrent;
            menuBar.MouseFilter = isCurrent ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
            menuBar.SetDrawBehindParent(true);
        }
    }

    public void SetToolState(ToolState toolState)
    {
        if (ToolState is not null) ToolState.ColorChanged -= SetColorMenuButtonColor;

        ToolState = toolState;
        ColorPaletteBar.SetToolState(toolState);

        ToolState.ColorChanged += SetColorMenuButtonColor;
    }

    private void SetColorMenuButtonColor(Color color)
    {
        OpenColorsButton.SetDisplayColor(color);
    }

    public void SwitchToColorsMenu()
    {
        CurrentMenuBar = InkMenu;
        _squashStretchAmount = 1f;
    }

    public void SwitchToToolOptionsMenu()
    {
        CurrentMenuBar = ToolOptionsMenu;
        _squashStretchAmount = 1f;
    }
}
