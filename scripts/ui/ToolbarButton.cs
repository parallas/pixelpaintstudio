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

        // VisualRoot.Scale = MathUtil.ExpDecay(
        //     VisualRoot.Scale,
        //     _isHovered ? Vector2.One * 1.25f : Vector2.One,
        //     16f,
        //     (float)delta
        // );
        Model?.SetQuaternion(MathUtil.ExpDecay(
            Model.Quaternion,
            Quaternion.FromEuler(new Vector3(0f, _hoverTime * 2f, 0f)),
            16f,
            (float)delta
        ));

        if (_isHovered)
        {
            _hoverTime += (float)delta;
            Position = Position with { Y = MathUtil.ExpDecay(Position.Y, -50, 16f, (float)delta) };
        }
        else
        {
            _hoverTime = 0;
            Position = Position with { Y = MathUtil.ExpDecay(Position.Y, 0, 16f, (float)delta) };
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
