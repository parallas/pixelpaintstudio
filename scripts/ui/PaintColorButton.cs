using Godot;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Parallas;

[GlobalClass]
public partial class PaintColorButton : VirtualCursorButton
{
    [Export] private PaintBlob _paintBlob;
    [Export] private Control _visualRoot;
    [Export] private InkDefinition _inkDefinition;
    [Export] private bool _scaleWhenSelected = true;
    [Export] private bool _setToolColorWhenClicked = true;
    private Vector2 _scale = Vector2.One;
    private Vector2 _scaleVelocity = Vector2.Zero;

    private MainEditor _editor;

    public override void _Ready()
    {
        base._Ready();

        RandomizeOrientation();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        // if (_inkDefinition is not null) _paintBlob.SetColor(_inkDefinition.SampleColor(Time.GetTicksMsec() / 1000.0f));

        _editor ??= GetTree().GetFirstNodeInGroup("main_editor") as MainEditor;
        if (_editor is null) return;

        IsSelected = false;
        foreach (var (key, value) in _editor.PlayerToolStates)
        {
            if (value.InkDefinition == _inkDefinition) IsSelected = true;
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

        if (_setToolColorWhenClicked)
            SetToolToColor(toolState);
        else
        {
            RandomizeOrientation();
            Bounce();
        }
    }

    public void SetToolToColor(ToolState toolState)
    {
        toolState.SetInk(_inkDefinition);
        RandomizeOrientation();
        Bounce();
    }

    public void RandomizeOrientation()
    {
        _paintBlob.RotateZ(GD.Randf() * 2f * Mathf.Pi);
        if (GD.Randf() < 0.5f) _paintBlob.RotateX(Mathf.Pi);
        if (GD.Randf() < 0.5f) _paintBlob.RotateY(Mathf.Pi);
    }

    public void Bounce(float intensity = 10f)
    {
        _scaleVelocity = Vector2.One * intensity;
    }

    public void SetInkDefinition([NotNull]InkDefinition ink)
    {
        _inkDefinition = ink;

        if (ink.Gradient is { } gradient)
        {
            _paintBlob.SetColor(Colors.White);
            _paintBlob.SetTexture(new GradientTexture2D
            {
                Gradient = gradient,
                Width = 64,
                Height = 64,
                Fill = GradientTexture2D.FillEnum.Linear,
                FillFrom = Vector2.Right * 0.0f,
                FillTo = Vector2.Right * 1.05f,
                Repeat = GradientTexture2D.RepeatEnum.Repeat,
            });
        }
        else
        {
            _paintBlob.SetTexture(null);
            _paintBlob.SetColor(_inkDefinition.Color);
        }
    }
}
