using Godot;
using System;
using Parallas;

public partial class CursorDrawToolVisual : Node3D
{
    [Export] public GameCursor GameCursor { get; set; } = null;
    private MainEditor _editor;

    private String _currentIconScenePath;
    private Node3D _currentIconNode;
    private AnimationPlayer _currentAnimationPlayer;

    public override void _Process(double delta)
    {
        base._Process(delta);

        _editor ??= GetTree().GetFirstNodeInGroup("main_editor") as MainEditor;

        UpdateIcon();

        if (GameCursor.ClickHeld)
        {
            if (_currentAnimationPlayer?.CurrentAnimation != "Cursor")
                _currentAnimationPlayer?.Play("Cursor");
        }
    }

    private void UpdateIcon()
    {
        if (_editor is null) return;
        if (!_editor.PlayerToolStates.TryGetValue(GameCursor.PlayerId, out ToolState toolState)) return;
        var toolStateModelPath = toolState.ToolDefinition.ModelScene.ResourcePath;
        if (_currentIconScenePath == toolStateModelPath) return;

        _currentIconScenePath = toolStateModelPath;

        if (IsInstanceValid(_currentIconNode)) _currentIconNode.QueueFree();
        var toolIconNode = GetIconNode(toolState);
        AddChild(toolIconNode);
        _currentIconNode = toolIconNode;

        _currentAnimationPlayer = toolIconNode.FindChild("AnimationPlayer") as AnimationPlayer;
        if (_currentAnimationPlayer is not null && _currentAnimationPlayer.HasAnimation("Cursor"))
            _currentAnimationPlayer?.Play("Cursor");
        _currentAnimationPlayer?.Advance(0);
    }

    private Node3D GetIconNode(ToolState toolState)
    {
        return toolState.ToolDefinition.ModelScene.Instantiate<Node3D>();
    }
}
