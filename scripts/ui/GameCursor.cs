using System;
using Godot;
using Godot.Collections;
using Parallas;

public partial class GameCursor : Control
{
    [Export] private Node3D _contentRoot3d;
    [Export] private Node3D _iconToolPenSmall;
    [Export] private Node3D _iconToolPenMedium;
    [Export] private Node3D _iconToolPenLarge;
    [Export] private Node3D _iconToolBubble;
    [Export] private Node3D _iconToolStamp;
    [Export] private Node3D _iconToolSticker;
    [Export] private Node3D _iconToolZoom;

    [Export] public ToolState ToolState { get; private set; }

    public bool IsHighlighted { get; private set; }
    private float _cursorCurrentSpeed = 1f;

    private float _squashStretchAmount = 0f;
    private float _squashStretchVelocity = 0f;

    private Array<Node> _contents3d;
    private String _currentIconName;

    public override void _Ready()
    {
        base._Ready();

        Input.SetMouseMode(Input.MouseModeEnum.Hidden);
        Godot.Engine.MaxFps = 120;

        _contents3d = _contentRoot3d.GetChildren();
        HideAllObjects();
    }

    public override void _Process(double delta)
    {
        MathUtil.Spring(ref _squashStretchAmount, ref _squashStretchVelocity, 0f, 0.2f, 20f, (float)delta);
        Scale = MathUtil.SquashScale(1f + _squashStretchAmount).ToVector2();

        Vector2 newPos = Position;
        Vector2 moveAmount = Input.GetVector(
            "cursor_left",
            "cursor_right",
            "cursor_up",
            "cursor_down"
        ).Clamp(-1f, 1f);
        if (moveAmount.LengthSquared() > 0)
            _cursorCurrentSpeed = MathUtil.ExpDecay(_cursorCurrentSpeed, 5f, 1f, (float)delta);
        else
            _cursorCurrentSpeed = 1f;
        newPos += moveAmount * 128f * _cursorCurrentSpeed * (float)delta;
        newPos = newPos.Clamp(Vector2.Zero, GetViewportRect().Size);
        SetPosition(newPos);

        if (Input.IsKeyPressed(Key.Escape)) Input.SetMouseMode(Input.MouseModeEnum.Visible);
        if (Input.IsMouseButtonPressed(MouseButton.Left)) Input.SetMouseMode(Input.MouseModeEnum.Hidden);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        InputFixer.UpdateInput(@event);

        if (@event is InputEventMouseMotion mouseEvent)
        {
            SetPosition(mouseEvent.Position, true);
        }
    }

    public void SetToolState(ToolState toolState)
    {
        if (ToolState is not null) ToolState.DrawingToolChanged -= ReactToDrawingToolChange;
        ToolState = toolState;
        toolState.DrawingToolChanged += ReactToDrawingToolChange;
        UpdateIcon();
    }

    public void SetHighlighted(bool highlighted)
    {
        bool justStartedHighlight = !IsHighlighted && highlighted;
        bool justStoppedHighlight = IsHighlighted && !highlighted;
        IsHighlighted = highlighted;

        if (justStartedHighlight)
        {
            _squashStretchAmount = 0.3f;
        }

        if (justStoppedHighlight)
        {
            _squashStretchAmount = -0.3f;
        }
    }

    private void HideAllObjects()
    {
        foreach (var child in _contents3d)
        {
            if (child is not Node3D node3d) continue;
            node3d.SetVisible(false);
        }
    }

    private Node3D GetIconNode()
    {
        switch (ToolState.DrawingTool)
        {
            case ToolState.DrawingTools.PenSmall:
                return _iconToolPenSmall;
            case ToolState.DrawingTools.PenMedium:
                return _iconToolPenMedium;
            case ToolState.DrawingTools.PenLarge:
                return _iconToolPenLarge;
            case ToolState.DrawingTools.BubbleWand:
                return _iconToolBubble;
            case ToolState.DrawingTools.Stamp:
                return _iconToolStamp;
            case ToolState.DrawingTools.Sticker:
                return _iconToolSticker;
            case ToolState.DrawingTools.Zoom:
                return _iconToolZoom;
        }

        return null;
    }

    private void UpdateIcon()
    {
        var toolIconNode = GetIconNode();
        if (toolIconNode is null) return;
        if (toolIconNode.Name == _currentIconName) return;

        HideAllObjects();
        toolIconNode.SetVisible(true);
        if (toolIconNode.FindChild("AnimationPlayer") is AnimationPlayer animationPlayer)
        {
            if (animationPlayer.HasAnimation("Cursor"))
                animationPlayer.Play("Cursor");
        }
        _squashStretchAmount = 0.25f;
    }

    private void ReactToDrawingToolChange(ToolState.DrawingTools drawingTool)
    {
        UpdateIcon();
    }
}
