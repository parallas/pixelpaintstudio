using Godot;
using System;
using System.Linq;
using Godot.Collections;

public partial class ColorPaletteBar : PanelContainer
{
    [Signal] public delegate void OnColorChangeEventHandler(Color color);

    private PaintColorButton[] _paintColorButtons;

    private int ItemsPerPage => _paintColorButtons.Length;
    private int _page = 0;

    public override void _Ready()
    {
        base._Ready();

        _paintColorButtons = FindChildren("*", "PaintColorButton").OfType<PaintColorButton>().ToArray();
        GD.Print($"Found {_paintColorButtons.Length} color palette bars");

        SetPageValues();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        var pageBefore = _page;
        if (Input.IsActionJustPressed("cursor_right"))
        {
            _page++;
        }
        if (Input.IsActionJustPressed("cursor_left"))
        {
            _page--;
        }
        _page = (int)Mathf.Clamp(_page, 0f, Mathf.FloorToInt((float)MainEditor.AllInkDefinitions.Count / ItemsPerPage));

        if (pageBefore != _page)
        {
            SetPageValues();
        }
    }

    private void SetPageValues()
    {
        int startIndex = (_page * ItemsPerPage);
        int endIndex = (_page + 1) * ItemsPerPage;
        endIndex = Math.Min(endIndex, MainEditor.AllInkDefinitions.Count);
        SetInkArray(MainEditor.AllInkDefinitions[startIndex..endIndex].ToArray());
    }

    public void SetInkArray(InkDefinition[] inkDefinitions)
    {
        var min = _paintColorButtons.Length;
        for (var i = 0; i < min; i++)
        {
            var button = _paintColorButtons[i];
            if (!IsInstanceValid(button)) continue;
            if (i < inkDefinitions.Length) {
                button.SetVisible(true);
                button.SetProcess(true);
                button.SetInkDefinition(inkDefinitions[i]);
            }
            else
            {
                button.SetVisible(false);
                button.SetProcess(false);
            }
        }
    }
}
