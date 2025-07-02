using Godot;
using System;
using Parallas;

[GlobalClass]
public partial class ToolbarButton : VirtualCursorButton
{
    [Signal] public delegate void OnToolSelectedEventHandler(ToolDefinition tool);

    [Export] public ToolState ToolState;
    [Export] public ToolDefinition ToolDefinition { get; private set; }
    [Export] private Control _visualRoot;
    [Export] private Node3D _modelRoot;

    private Node3D _model;

    private AnimationPlayer _animationPlayer;
    private float _hoverTime = 0;
    private float _colorBlend = 0f;

    private Vector2 _hoverScale = Vector2.One;
    private float _squashStretchAmount = 0f;
    private float _squashStretchVelocity = 0f;
    private Vector2 _squashStretchScale = Vector2.One;

    private float _tabLiftTarget = 0f;
    private float _tabLiftVelocity = 0f;

    private bool IsSelected => ToolState.ToolDefinition == ToolDefinition;

    public override void _Ready()
    {
        base._Ready();

        _visualRoot.Material = null;
        _visualRoot.UseParentMaterial = true;
        _visualRoot.SetInstanceShaderParameter("color_blend", _colorBlend);

        _model = ToolDefinition.ModelScene.Instantiate<Node3D>();
        _modelRoot.AddChild(_model);
        _animationPlayer = _model.GetNode<AnimationPlayer>("AnimationPlayer");
        _animationPlayer.Play("RESET");
        _animationPlayer.Advance(0);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if ((IsHovered() || IsSelected) && _animationPlayer.AssignedAnimation != "Open") _animationPlayer.Play("Open");
        if ((!IsHovered() && !IsSelected) && _animationPlayer.AssignedAnimation != "Close" && _animationPlayer.AssignedAnimation != "RESET") _animationPlayer.Play("Close");
        _visualRoot.SetInstanceShaderParameter("color_blend", _colorBlend);

        MathUtil.Spring(
            ref _squashStretchAmount,
            ref _squashStretchVelocity,
            0f,
            0.3f,
            20f,
            (float)delta
        );

        var y = Position.Y;
        MathUtil.Spring(
            ref y,
            ref _tabLiftVelocity,
            _tabLiftTarget,
            0.4f,
            30f,
            (float)delta
        );
        Position = Position with { Y = y };

        _hoverScale = MathUtil.ExpDecay(
            _hoverScale,
            IsHovered() ? Vector2.One * 1.15f : Vector2.One,
            16f,
            (float)delta
        );

        _squashStretchScale = MathUtil.SquashScale(1f + _squashStretchAmount).ToVector2();

        _visualRoot.Scale = _hoverScale * _squashStretchScale;
        _model?.SetQuaternion(MathUtil.ExpDecay(
            _model.Quaternion,
            Quaternion.FromEuler(new Vector3(0f, _hoverTime * 2f, 0f)),
            16f,
            (float)delta
        ));

        _tabLiftTarget = (IsHovered() ? -50 : 0) + (IsSelected ? -25 : 0);

        if (IsHovered() || IsSelected)
        {
            if (_hoverTime == 0) _squashStretchAmount = 0.25f;
            _hoverTime += (float)delta;
        }
        else
        {
            if (_hoverTime > 0) _squashStretchAmount = 0.25f;
            _hoverTime = 0;
        }

        if (IsHovered() || IsSelected)
        {
            _colorBlend = MathUtil.ExpDecay(_colorBlend, 1f, 40f, (float)delta);
        }
        else
        {
            _colorBlend = MathUtil.ExpDecay(_colorBlend, 0f, 5f, (float)delta);
        }
    }

    protected override void VirtualCursorPressed(InputEventMouseButton eventMouseButton)
    {
        base.VirtualCursorPressed(eventMouseButton);

        ToolState.SetTool(ToolDefinition);
        EmitSignalOnToolSelected(ToolDefinition);
    }
}
