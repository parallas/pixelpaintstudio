using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;
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

    public void NextPage()
    {
        GD.Print("Next page");
        var newPage = Mathf.Wrap(_page + 1, 0, Mathf.FloorToInt((float)MainEditor.AllInkDefinitions.Count / ItemsPerPage));
        if (_page == newPage) return;
        _page = newPage;
        SetPageValues();

        for (var i = 0; i < _paintColorButtons.Length; i++)
        {
            _paintColorButtons[i].RandomizeOrientation();
        }
        AnimateBounceDirection(-1);
    }

    public void PreviousPage()
    {
        GD.Print("Prev page");
        var newPage = Mathf.Wrap(_page - 1, 0, Mathf.FloorToInt((float)MainEditor.AllInkDefinitions.Count / ItemsPerPage));
        if (_page == newPage) return;
        _page = newPage;
        SetPageValues();

        for (var i = 0; i < _paintColorButtons.Length; i++)
        {
            _paintColorButtons[i].RandomizeOrientation();
        }
        AnimateBounceDirection(-1);
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
            var ink = inkDefinitions[i];
            if (i < inkDefinitions.Length && ink is not null) {
                button.SetVisible(true);
                button.ProcessMode = ProcessModeEnum.Always;
                button.SetInkDefinition(ink);
            }
            else
            {
                button.SetVisible(false);
                button.ProcessMode = ProcessModeEnum.Disabled;
            }
        }
    }

    private async Task AnimateBounceDirection(int direction)
    {
        if (direction != 1 && direction != -1) direction = 1;

        var start = direction == 1 ? 0 : _paintColorButtons.Length - 1;
        var end = direction == 1 ? _paintColorButtons.Length : 0;
        for (var i = start; i != end; i += direction * 2)
        {
            _paintColorButtons[i].Bounce(5f);
            _paintColorButtons[i + direction].Bounce(5f);
            await Task.Delay(1);
        }
    }
}
