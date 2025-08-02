using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;
using Godot.Collections;

public partial class CrystalBallBar : PanelContainer
{
    [Signal] public delegate void OnSelectionChangeEventHandler(BrushDefinition brushDefinition);

    private CrystalBallButton[] _buttons;

    private int ItemsPerPage => _buttons.Length;
    private int _page = 0;

    public override void _Ready()
    {
        base._Ready();

        _buttons = FindChildren("*", "CrystalBallButton").OfType<CrystalBallButton>().ToArray();
        GD.Print($"Found {_buttons.Length} crystal ball buttons");

        SetPageValues();

        VisibilityChanged += SetPageValues;
    }

    public void NextPage()
    {
        var newPage = Mathf.PosMod(_page + 1, 1 + Mathf.FloorToInt((float)(MainEditor.MagicWandDefinitions.Count - 1) / ItemsPerPage));
        // if (_page == newPage) return;
        _page = newPage;
        SetPageValues();

        AnimateBounceDirection(-1);
    }

    public void PreviousPage()
    {
        var newPage = Mathf.PosMod(_page - 1, 1 + Mathf.FloorToInt((float)(MainEditor.MagicWandDefinitions.Count - 1) / ItemsPerPage));
        // if (_page == newPage) return;
        _page = newPage;
        SetPageValues();

        AnimateBounceDirection(-1);
    }

    private void SetPageValues()
    {
        int startIndex = (_page * ItemsPerPage);
        int endIndex = (_page + 1) * ItemsPerPage;
        endIndex = Math.Min(endIndex, MainEditor.MagicWandDefinitions.Count);
        SetValueArray(MainEditor.MagicWandDefinitions[startIndex..endIndex].ToArray());
    }

    public void SetValueArray(BrushDefinition[] values)
    {
        var min = _buttons.Length;
        for (var i = 0; i < min; i++)
        {
            var button = _buttons[i];
            if (!IsInstanceValid(button)) continue;
            bool render = i < values.Length && values[i] is not null;
            if (render) {
                var stencil = values[i];
                button.SetVisible(true);
                button.ProcessMode = ProcessModeEnum.Always;
                button.SetBrushDefinition(stencil);
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

        var start = direction == 1 ? 0 : _buttons.Length - 1;
        var end = direction == 1 ? _buttons.Length : 0;
        for (var i = start; i != end; i += direction * 2)
        {
            _buttons[i].Bounce(5f);
            _buttons[i + direction].Bounce(5f);
            await Task.Delay(1);
        }
    }
}
