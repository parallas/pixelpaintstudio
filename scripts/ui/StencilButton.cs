using Godot;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Parallas;

[GlobalClass]
public partial class StencilButton : VirtualCursorButton
{
    [Export] private PaintBlob _paintBlob;
    [Export] private Control _visualRoot;
    [Export] private StencilData _stencilData;
    [Export] private bool _scaleWhenSelected = true;
    [Export] private bool _setToolValueWhenSelected = true;
    private Vector2 _scale = Vector2.One;
    private Vector2 _scaleVelocity = Vector2.Zero;

    private MainEditor _editor;

    public override void _Process(double delta)
    {
        base._Process(delta);

        _editor ??= GetTree().GetFirstNodeInGroup("main_editor") as MainEditor;
        if (_editor is null) return;

        IsSelected = false;
        foreach (var (key, value) in _editor.PlayerToolStates)
        {
            if (value.StencilData == _stencilData) IsSelected = true;
        }

        _visualRoot.SetPivotOffset(_visualRoot.Size * 0.5f);

        var scaleTarget = 1f;
        if (IsHoveredVirtually) scaleTarget = 1.15f;
        if (IsPressedVirtually) scaleTarget = 1f;
        if (_scaleWhenSelected && IsSelected) scaleTarget += 0.45f;
        var (x, y) = _scale;
        var (xVel, yVel) = _scaleVelocity;
        MathUtil.Spring(
            ref x,
            ref xVel,
            scaleTarget,
            0.1f,
            25f,
            (float)delta
        );
        MathUtil.Spring(
            ref y,
            ref yVel,
            scaleTarget,
            0.25f,
            20f,
            (float)delta
        );
        _scale = new Vector2(x, y);
        _scaleVelocity = new Vector2(xVel, yVel);

        _visualRoot.Scale = _scale;

        SetZIndex(IsHoveredVirtually ? 2 : IsSelected ? 1 : 0);
    }

    protected override void VirtualCursorPressed(InputEvent @event, int playerId)
    {
        base.VirtualCursorPressed(@event, playerId);

        if (_editor is null) return;

        if (!_editor.PlayerToolStates.TryGetValue(playerId, out var toolState)) return;

        if (_setToolValueWhenSelected)
            SetToolValue(toolState);
        else
        {
            Bounce();
        }
    }

    public void SetToolValue(ToolState toolState)
    {
        toolState.SetStencil(_stencilData);
        Bounce();
    }

    public void Bounce(float intensity = 10f)
    {
        _scaleVelocity = Vector2.One * intensity;
    }

    public void SetStencilDefinition([NotNull]StencilData stencilData)
    {
        _stencilData = stencilData;

        _paintBlob.SetTexture(stencilData.MaskTexture);
        _paintBlob.SetTextureScale(Vector2.One * 10f);
    }
}
