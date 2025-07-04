using System;
using Godot;
using Godot.Collections;
using Parallas;

public partial class GameCursor : CenterContainer
{
    [Export] public int PlayerId { get; private set; } = 0;
    [Export] private Node3D _contentRoot3d;
    public bool ClickHeld { get; private set; } = false;

    private float _squashStretchAmount = 0f;
    private float _squashStretchVelocity = 0f;

    private Vector2 _moveAmount = Vector2.Zero;

    public override void _Ready()
    {
        base._Ready();

        var cursorDrawVisual = new CursorDrawToolVisual { GameCursor = this };
        _contentRoot3d.AddChild(cursorDrawVisual);
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        AddToGroup("player_cursors");
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
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (!PlayerDeviceMapper.IsInputFromPlayer(@event, PlayerId)) return;

        InputFixer.UpdateInput(@event);

        if (@event.IsActionPressed("click"))
        {
            _squashStretchAmount += 0.2f;
            ClickHeld = true;
        }
        if (@event.IsActionReleased("click"))
        {
            ClickHeld = false;
        }
    }

    public void SetPlayerId(int playerId)
    {
        PlayerId = playerId;
    }
}
