using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;
using Godot.Collections;

public partial class StencilBar : PanelContainer
{
    [Signal] public delegate void OnStencilChangeEventHandler(StencilData stencilData);

    private StencilButton[] _stencilButtons;

    private int ItemsPerPage => _stencilButtons.Length;
    private int _page = 0;

    public override void _Ready()
    {
        base._Ready();

        _stencilButtons = FindChildren("*", "StencilButton").OfType<StencilButton>().ToArray();
        GD.Print($"Found {_stencilButtons.Length} color palette buttons");

        SetPageValues();
    }

    public void NextPage()
    {
        GD.Print("Next page");
        var newPage = Mathf.Wrap(_page + 1, 0, Mathf.FloorToInt((float)(MainEditor.AllStencilData.Count) / ItemsPerPage));
        if (_page == newPage) return;
        _page = newPage;
        SetPageValues();

        AnimateBounceDirection(-1);
    }

    public void PreviousPage()
    {
        GD.Print("Prev page");
        var newPage = Mathf.Wrap(_page - 1, 0, Mathf.FloorToInt((float)(MainEditor.AllStencilData.Count) / ItemsPerPage));
        if (_page == newPage) return;
        _page = newPage;
        SetPageValues();

        AnimateBounceDirection(-1);
    }

    private void SetPageValues()
    {
        GD.Print($"Set Page values for {_page}");
        int startIndex = (_page * ItemsPerPage);
        int endIndex = (_page + 1) * ItemsPerPage;
        endIndex = Math.Min(endIndex, MainEditor.AllStencilData.Count);
        SetValueArray(MainEditor.AllStencilData[startIndex..endIndex].ToArray());
    }

    public void SetValueArray(StencilData[] stencilDatas)
    {
        var min = Mathf.Min(_stencilButtons.Length, stencilDatas.Length);
        for (var i = 0; i < min; i++)
        {
            var button = _stencilButtons[i];
            if (!IsInstanceValid(button)) continue;
            var stencil = stencilDatas[i];
            if (i < stencilDatas.Length && stencil is not null) {
                button.SetVisible(true);
                button.ProcessMode = ProcessModeEnum.Always;
                button.SetStencilDefinition(stencil);
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

        var start = direction == 1 ? 0 : _stencilButtons.Length - 1;
        var end = direction == 1 ? _stencilButtons.Length : 0;
        for (var i = start; i != end; i += direction * 2)
        {
            _stencilButtons[i].Bounce(5f);
            _stencilButtons[i + direction].Bounce(5f);
            await Task.Delay(1);
        }
    }
}
