using System;
using Godot;
using Godot.Collections;
using Parallas;

public partial class GameCursor : Control
{
    [Export] private float _baseSpeed = 128f;
    [Export] private Vector2 _minMaxSpeedMultiplier = new Vector2(1, 5);
    [Export] private Node3D _contentRoot3d;

    [Export] public ToolState ToolState { get; private set; }

    [Export] public DrawCanvas TargetDrawCanvas;
    [Export] public Control CanvasRoot;

    public bool IsHighlighted { get; private set; }
    private float _cursorCurrentSpeed = 1f;

    private float _squashStretchAmount = 0f;
    private float _squashStretchVelocity = 0f;

    private String _currentIconName;
    private Node3D _currentIconNode;
    private AnimationPlayer _currentAnimationPlayer;

    private bool _clickHeld = false;

    public override void _Ready()
    {
        base._Ready();

        Input.SetMouseMode(Input.MouseModeEnum.Hidden);
        Godot.Engine.MaxFps = 120;
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
            _cursorCurrentSpeed = MathUtil.ExpDecay(_cursorCurrentSpeed, _minMaxSpeedMultiplier.Y, 1f, (float)delta);
        else
            _cursorCurrentSpeed = _minMaxSpeedMultiplier.X;
        newPos += moveAmount * _baseSpeed * _cursorCurrentSpeed * (float)delta;
        newPos = newPos.Clamp(Vector2.Zero, GetViewportRect().Size);
        SetPosition(newPos);

        if (Input.IsKeyPressed(Key.Escape)) Input.SetMouseMode(Input.MouseModeEnum.Visible);
        if (Input.IsMouseButtonPressed(MouseButton.Left)) Input.SetMouseMode(Input.MouseModeEnum.Hidden);

        if (_clickHeld)
        {
            ToolState?.ToolDefinition.BrushDefinition?.Process(GetCanvasPosition(), ToolState.BrushColor, delta);
            if (_currentAnimationPlayer?.CurrentAnimation != "Cursor")
                _currentAnimationPlayer?.Play("Cursor");
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        InputFixer.UpdateInput(@event);

        if (@event is InputEventJoypadMotion joypadMotion || @event is InputEventJoypadButton joypadButton)
        {
            Input.ParseInputEvent(new InputEventMouseMotion() { Position = Position });
        }
        if (@event is InputEventMouseMotion mouseEvent)
        {
            SetPosition(mouseEvent.Position, true);
        }

        if (@event.IsActionPressed("click"))
        {
            _clickHeld = true;
            _squashStretchAmount += 0.2f;
            ToolState?.ToolDefinition.BrushDefinition?.Start(TargetDrawCanvas, GetCanvasPosition(), ToolState.BrushColor, 0);
        }
        if (@event.IsActionReleased("click"))
        {
            _clickHeld = false;
            ToolState?.ToolDefinition.BrushDefinition?.Finish(GetCanvasPosition(), ToolState.BrushColor, 0);
        }
    }

    public void SetToolState(ToolState toolState)
    {
        if (ToolState is not null) ToolState.ToolChanged -= ReactToDrawingToolChange;
        ToolState = toolState;
        toolState.ToolChanged += ReactToDrawingToolChange;
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

    private Node3D GetIconNode()
    {
        return ToolState.ToolDefinition.ModelScene.Instantiate<Node3D>();
    }

    private void UpdateIcon()
    {
        if (IsInstanceValid(_currentIconNode)) _currentIconNode.QueueFree();
        var toolIconNode = GetIconNode();
        _contentRoot3d.AddChild(toolIconNode);
        _currentIconNode = toolIconNode;

        _currentAnimationPlayer = toolIconNode.FindChild("AnimationPlayer") as AnimationPlayer;
        if (_currentAnimationPlayer is not null && _currentAnimationPlayer.HasAnimation("Cursor"))
            _currentAnimationPlayer?.Play("Cursor");
        _currentAnimationPlayer?.Advance(0);

        _squashStretchAmount = 0.25f;
    }

    private void ReactToDrawingToolChange(ToolDefinition tool)
    {
        UpdateIcon();
    }

    private Vector2 GetCanvasPosition()
    {
        if (TargetDrawCanvas.GetViewport().GetParent() is SubViewportContainer)
        {
            var finalValue = TargetDrawCanvas.GetScreenTransform().AffineInverse().BasisXform(
                GlobalPosition - TargetDrawCanvas.GetScreenPosition()
            );
            return finalValue;
        }

        var posLocalToRect = GlobalPosition - CanvasRoot.GlobalPosition;
        var scaleFactor = TargetDrawCanvas.Resolution / CanvasRoot.Size;
        var canvasRelativePosition = posLocalToRect * scaleFactor;
        var pixelPosition = canvasRelativePosition.Round();
        return pixelPosition;
    }
}
