using Godot;
using System;
using System.Linq;

public partial class InkMenuButtonSync : Node
{
    [Export] public PaintColorButton InkMenuButton { get; private set; }
    [Export] public MainEditorValueController MainEditorValueController { get; private set; }

    public override void _Process(double delta)
    {
        base._Process(delta);

        var primaryPlayerId = MainEditorValueController.Editor.PrimaryPlayerId;
        if (!MainEditorValueController.Editor.PlayerToolStates.TryGetValue(primaryPlayerId, out ToolState toolState)) return;
        InkMenuButton.SetInkDefinition(toolState.InkDefinition);
    }
}
