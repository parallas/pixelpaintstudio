using Godot;
using System;
using Parallas;

[GlobalClass]
public partial class ToolbarButton : Panel
{
    [Export] private Control VisualRoot;
    [Export] private Node3D Model;

    private AnimationPlayer _animationPlayer;
    private bool _isSelected = false;
    private bool _isHovered = false;
    private float _hoverTime = 0;
    private float _colorBlend = 0f;

    private float _squashStretchAmount = 0f;
    private float _squashStretchVelocity = 0f;

    private float _tabLiftTarget = 0f;
    private float _tabLiftVelocity = 0f;

    public override void _Ready()
    {
        base._Ready();

        MouseEntered += () => { _isHovered = true; _animationPlayer.Play("Open"); };
        MouseExited += () => { _isHovered = false; _animationPlayer.Play("Close");  };

        VisualRoot.Material = null;
        VisualRoot.UseParentMaterial = true;
        VisualRoot.SetInstanceShaderParameter("color_blend", _colorBlend);

        _animationPlayer = Model.GetNode<AnimationPlayer>("AnimationPlayer");
        _animationPlayer.Play("RESET");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

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

        // VisualRoot.Scale = MathUtil.ExpDecay(
        //     VisualRoot.Scale,
        //     _isHovered ? Vector2.One * 1.25f : Vector2.One,
        //     16f,
        //     (float)delta
        // );

        VisualRoot.Scale = MathUtil.SquashScale(1f + _squashStretchAmount).ToVector2();
        Model?.SetQuaternion(MathUtil.ExpDecay(
            Model.Quaternion,
            Quaternion.FromEuler(new Vector3(0f, _hoverTime * 2f, 0f)),
            16f,
            (float)delta
        ));

        if (_isHovered)
        {
            if (_hoverTime == 0) _squashStretchAmount = 0.25f;
            _hoverTime += (float)delta;
            _tabLiftTarget = -50;
        }
        else
        {
            if (_hoverTime > 0) _squashStretchAmount = 0.25f;
            _hoverTime = 0;
            _tabLiftTarget = 0;
        }

        if (_isHovered || _isSelected)
        {
            _colorBlend = MathUtil.ExpDecay(_colorBlend, 1f, 13f, (float)delta);
        }
        else
        {
            _colorBlend = MathUtil.ExpDecay(_colorBlend, 0f, 13f, (float)delta);
        }
    }
}
