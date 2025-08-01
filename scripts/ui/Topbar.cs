using Godot;
using System;
using Godot.Collections;
using Parallas;

public partial class Topbar : MarginContainer
{
    [Signal] public delegate void OnSettingsChangeEventHandler(ToolDefinition tool);

    [Export] public MainEditorValueController MainEditorValueController { get; private set; }

    [Export] public ColorPaletteBar ColorPaletteBar { get; private set; }
    [Export] public Array<Control> MenuBars { get; private set; }
    [Export] public Control CurrentMenuBar { get; private set; }
    [Export] public Control InkMenu { get; private set; }
    [Export] public Control SecondarySettingsRootNode { get; private set; }
    [Export] public Control ToolOptionsMenu { get; private set; }
    [Export] public Control WandsMenu { get; private set; }

    [Export] public PaintColorButton OpenColorsButton { get; private set; }

    private float _squashStretchAmount = 0f;
    private float _squashStretchVelocity = 0f;

    private bool _secondaryMenuState = false;

    public override void _Ready()
    {
        base._Ready();
        foreach (var menuBar in MenuBars)
        {
            bool isCurrent = menuBar == CurrentMenuBar;
            menuBar.Visible = isCurrent;
            menuBar.MouseFilter = isCurrent ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
            menuBar.ProcessMode = isCurrent ? ProcessModeEnum.Always : ProcessModeEnum.Disabled;
            menuBar.SetDrawBehindParent(true);
            menuBar.SetPivotOffset(new Vector2(0f, menuBar.Size.Y * 0.5f));
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (_secondaryMenuState)
        {
            if (MainEditorValueController.Editor.PlayerToolStates.TryGetValue(MainEditorValueController.Editor.PrimaryPlayerId, out ToolState toolState))
            {
                switch (toolState.ToolDefinition?.ToolMenu)
                {
                    case ToolDefinition.ToolMenus.Stencils:
                        CurrentMenuBar = ToolOptionsMenu;
                        break;
                    case ToolDefinition.ToolMenus.Wands:
                        CurrentMenuBar = WandsMenu;
                        break;
                }
            }
        }

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
            menuBar.ProcessMode = isCurrent ? ProcessModeEnum.Always : ProcessModeEnum.Disabled;
            menuBar.SetDrawBehindParent(true);
        }
    }

    private void SetColorMenuButtonColor(InkDefinition ink)
    {
        OpenColorsButton.SetInkDefinition(ink);
    }

    public void SwitchToColorsMenu()
    {
        _secondaryMenuState = false;
        CurrentMenuBar = InkMenu;
        _squashStretchAmount = 1f;
    }

    public void SwitchToToolOptionsMenu()
    {
        _secondaryMenuState = true;
        _squashStretchAmount = 1f;
    }
}
