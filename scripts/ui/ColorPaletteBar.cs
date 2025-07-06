using Godot;
using System;
using Godot.Collections;

public partial class ColorPaletteBar : PanelContainer
{
    [Signal] public delegate void OnColorChangeEventHandler(Color color);

    [Export] private Array<PaintColorButton> _paintColorButtons;

    public void SetInkArray(Array<InkDefinition> inkDefinitions)
    {
        var min = Math.Min(inkDefinitions.Count, _paintColorButtons.Count);
        for (var i = 0; i < min; i++)
        {
            _paintColorButtons[i].SetInkDefinition(inkDefinitions[i]);
        }
    }
}
