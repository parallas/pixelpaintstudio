using Godot;
using System;
using System.Linq;
using Parallas;

public partial class PaintColorButton : VirtualCursorButton
{
    [Export] private PaintBlob _paintBlob;
    [Export] private Control _visualRoot;
    [Export] private Color _paintColor = Colors.Red;
    [Export] private bool _scaleWhenSelected = true;
    private Vector2 _scale = Vector2.One;
    private Vector2 _scaleVelocity = Vector2.Zero;
    private ToolState _toolState;

    private bool IsSelected => _paintColor == _toolState?.BrushColor;

    public override void _Ready()
    {
        base._Ready();

        _paintBlob.SetColor(_paintColor);
        RandomizeOrientation();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

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

        var gameCursorFetched = GetTree().GetNodesInGroup("player_cursors").Cast<GameCursor>()
            .FirstOrDefault(cursor => cursor.PlayerId == playerId);
        if (gameCursorFetched is not { } gameCursor) return;

        _toolState = gameCursor.ToolState;
        SetToolToColor();
    }

    public void SetToolToColor()
    {
        _toolState.SetColor(_paintColor);
        RandomizeOrientation();
        _scaleVelocity = Vector2.One * 10f;
    }

    public void SetToolState(ToolState toolState)
    {
        _toolState = toolState;

        if (toolState.BrushColor == _paintColor) return;
        RandomizeOrientation();
    }

    private void RandomizeOrientation()
    {
        _paintBlob.RotateZ(GD.Randf() * 2f * Mathf.Pi);
        if (GD.Randf() < 0.5f) _paintBlob.RotateX(Mathf.Pi);
        if (GD.Randf() < 0.5f) _paintBlob.RotateY(Mathf.Pi);
    }

    public void SetDisplayColor(Color color)
    {
        _paintColor = color;
        _paintBlob.SetColor(color);
    }
}
