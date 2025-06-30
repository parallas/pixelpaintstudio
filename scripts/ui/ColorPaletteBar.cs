using Godot;
using System;
using Godot.Collections;

public partial class ColorPaletteBar : PanelContainer
{
    [Signal] public delegate void OnColorChangeEventHandler(Color color);

    [Export] public ToolState ToolState { get; private set; }
    [Export] private Array<PaintColorButton> _paintColorButtons = new Array<PaintColorButton>();

    public void SetToolState(ToolState toolState)
    {
        ToolState = toolState;
        foreach (var button in _paintColorButtons)
        {
            button.SetToolState(toolState);
        }
    }
}
