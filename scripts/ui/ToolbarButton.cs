using Godot;
using System;
using Parallas;

[GlobalClass]
public partial class ToolbarButton : Panel
{
    [Signal] public delegate void OnToolSelectedEventHandler(ToolState.DrawingTools tool);

    [Export] public ToolState ToolState;
    [Export] public ToolState.DrawingTools Tool { get; private set; }
    [Export] private Control VisualRoot;
    [Export] private Node3D Model;

    private AnimationPlayer _animationPlayer;
    private bool _isHovered = false;
    private float _hoverTime = 0;
    private float _colorBlend = 0f;

    private Vector2 _hoverScale = Vector2.One;
    private float _squashStretchAmount = 0f;
    private float _squashStretchVelocity = 0f;
    private Vector2 _squashStretchScale = Vector2.One;

    private float _tabLiftTarget = 0f;
    private float _tabLiftVelocity = 0f;

    private bool IsSelected => ToolState.DrawingTool == Tool;

    public override void _Ready()
    {
        base._Ready();

        MouseEntered += () => { _isHovered = true; };
        MouseExited += () => { _isHovered = false; };

        VisualRoot.Material = null;
        VisualRoot.UseParentMaterial = true;
        VisualRoot.SetInstanceShaderParameter("color_blend", _colorBlend);

        _animationPlayer = Model.GetNode<AnimationPlayer>("AnimationPlayer");
        _animationPlayer.Play("RESET");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if ((_isHovered || IsSelected) && _animationPlayer.AssignedAnimation != "Open") _animationPlayer.Play("Open");
        if ((!_isHovered && !IsSelected) && _animationPlayer.AssignedAnimation != "Close" && _animationPlayer.AssignedAnimation != "RESET") _animationPlayer.Play("Close");
        VisualRoot.SetInstanceShaderParameter("color_blend", _colorBlend);

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
            _isHovered ? Vector2.One * 1.15f : Vector2.One,
            16f,
            (float)delta
        );

        _squashStretchScale = MathUtil.SquashScale(1f + _squashStretchAmount).ToVector2();

        VisualRoot.Scale = _hoverScale * _squashStretchScale;
        Model?.SetQuaternion(MathUtil.ExpDecay(
            Model.Quaternion,
            Quaternion.FromEuler(new Vector3(0f, _hoverTime * 2f, 0f)),
            16f,
            (float)delta
        ));

        _tabLiftTarget = (_isHovered ? -50 : 0) + (IsSelected ? -25 : 0);

        if (_isHovered || IsSelected)
        {
            if (_hoverTime == 0) _squashStretchAmount = 0.25f;
            _hoverTime += (float)delta;
        }
        else
        {
            if (_hoverTime > 0) _squashStretchAmount = 0.25f;
            _hoverTime = 0;
        }

        if (_isHovered || IsSelected)
        {
            _colorBlend = MathUtil.ExpDecay(_colorBlend, 1f, 40f, (float)delta);
        }
        else
        {
            _colorBlend = MathUtil.ExpDecay(_colorBlend, 0f, 5f, (float)delta);
        }

        if (_isHovered && Input.IsActionPressed("click"))
        {
            ToolState.SetDrawingTool(Tool);
            EmitSignalOnToolSelected(Tool);
        }
    }
}
