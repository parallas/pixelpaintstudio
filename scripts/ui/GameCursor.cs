using System;
using Godot;
using Godot.Collections;
using Parallas;

public partial class GameCursor : CenterContainer
{
    [Export] public int PlayerId { get; private set; } = 0;
    [Export] private Node3D _contentRoot3d;

    [Export] public ToolState ToolState { get; private set; }

    public bool IsHighlighted { get; private set; }

    private float _squashStretchAmount = 0f;
    private float _squashStretchVelocity = 0f;

    private String _currentIconName;
    private Node3D _currentIconNode;
    private AnimationPlayer _currentAnimationPlayer;

    private bool _clickHeld = false;
    private Vector2 _moveAmount = Vector2.Zero;

    public override void _EnterTree()
    {
        base._EnterTree();
        AddToGroup("player_cursors");

        SetToolState(ToolState);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        RemoveFromGroup("player_cursors");
    }

    public override void _Process(double delta)
    {
        MathUtil.Spring(ref _squashStretchAmount, ref _squashStretchVelocity, 0f, 0.2f, 20f, (float)delta);
        Scale = MathUtil.SquashScale(1f + _squashStretchAmount).ToVector2();

        if (PlayerDeviceMapper.TryGetPlayerDeviceMap(PlayerId, out PlayerDeviceMap deviceMap))
        {
            Position = deviceMap.MousePosition;
        }

        if (Input.IsKeyPressed(Key.Escape)) Input.SetMouseMode(Input.MouseModeEnum.Visible);
        if (Input.IsMouseButtonPressed(MouseButton.Left)) Input.SetMouseMode(Input.MouseModeEnum.Hidden);

        if (_clickHeld)
        {
            if (_currentAnimationPlayer?.CurrentAnimation != "Cursor")
                _currentAnimationPlayer?.Play("Cursor");
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (!PlayerDeviceMapper.IsInputFromPlayer(@event, PlayerId)) return;

        InputFixer.UpdateInput(@event);

        if (@event.IsActionPressed("click"))
        {
            _clickHeld = true;
            _squashStretchAmount += 0.2f;
        }
        if (@event.IsActionReleased("click"))
        {
            _clickHeld = false;
        }
    }

    public void SetPlayerId(int playerId)
    {
        PlayerId = playerId;
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
}
