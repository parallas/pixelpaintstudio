using Godot;
using System;
using System.Linq;
using Parallas;

public partial class PlayerCursorSpawnManager : Node
{
    [Export] private PackedScene _cursorScene;
    [Export] private Control _parentForNewCursors;

    public override void _Ready()
    {
        base._Ready();

        PlayerDeviceMapper.PlayerCreated += i =>
        {
            var newCursor = _cursorScene.Instantiate<GameCursor>();
            newCursor.SetPlayerId(i);
            newCursor.Position = _parentForNewCursors.GetGlobalRect().GetCenter();
            if (PlayerDeviceMapper.TryGetPlayerDeviceMap(i, out var deviceMap))
                deviceMap.MousePosition = newCursor.Position;
            _parentForNewCursors.AddChild(newCursor);
        };

        PlayerDeviceMapper.PlayerRemoved += i =>
        {
            var existingCursor = GetTree().GetNodesInGroup("player_cursors").OfType<GameCursor>()
                .FirstOrDefault(cursor => cursor.PlayerId == i);
            if (!IsInstanceValid(existingCursor)) return;
            existingCursor.QueueFree();
        };

        PlayerDeviceMapper.RegisterNewPlayer(0);

        Input.UseAccumulatedInput = true;
    }
}
