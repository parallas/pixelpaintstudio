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
    [Export] private Node3D _modelRoot;
    [Export] private Control _visualRoot;
    [Export] private StencilData _stencilData;
    [Export] private bool _scaleWhenSelected = true;
    [Export] private bool _setToolValueWhenSelected = true;
    private Vector2 _scale = Vector2.One;
    private Vector2 _scaleVelocity = Vector2.Zero;
    private Quaternion _modelInitialRotation;
    private Quaternion _modelTargetRotation;

    private MainEditor _editor;
    private float _hoverTime = 0f;

    public override void _Ready()
    {
        base._Ready();
        _modelInitialRotation = _modelRoot.Quaternion;
    }

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
        if (IsHoveredVirtually) scaleTarget = 1.25f;
        if (IsPressedVirtually) scaleTarget = 1f;
        if (_scaleWhenSelected && IsSelected) scaleTarget += 0.45f;
        var (x, y) = _scale;
        var (xVel, yVel) = _scaleVelocity;
        MathUtil.Spring(
            ref x,
            ref xVel,
            scaleTarget,
            0.5f,
            30f,
            (float)delta
        );
        MathUtil.Spring(
            ref y,
            ref yVel,
            scaleTarget,
            0.5f,
            30f,
            (float)delta
        );
        _scale = new Vector2(x, y);
        _scaleVelocity = new Vector2(xVel, yVel);

        _hoverTime += (float)delta;
        if (IsSelected)
        {
            _modelTargetRotation = Quaternion.FromEuler(new Vector3(Mathf.Cos(_hoverTime * 6f) * 0.3f, Mathf.Sin(_hoverTime * 6f) * 0.3f, 0f));
            _modelRoot.Quaternion = MathUtil.ExpDecay(_modelRoot.Quaternion, _modelTargetRotation, 16f, (float)delta);
        }
        else if (IsHoveredVirtually)
        {
            _modelRoot.Quaternion = MathUtil.ExpDecay(_modelRoot.Quaternion, Quaternion.Identity, 16f, (float)delta);
        }
        else
        {
            _modelRoot.Quaternion = MathUtil.ExpDecay(_modelRoot.Quaternion, _modelInitialRotation, 32f, (float)delta);
        }

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

        _paintBlob.SetTexture(stencilData.MaskTextureScaled);
        float sizeBasedPercent = MathUtil.InverseLerp01(64, 2, stencilData.MaskTexture.GetHeight());
        _paintBlob.SetTextureScale(Vector2.One * 7f * Mathf.Pow(sizeBasedPercent, 3f));
    }
}
