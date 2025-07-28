using Godot;

public partial class StencilMenuButtonSync : Node
{
    [Export] public StencilButton StencilButton { get; private set; }
    [Export] public MainEditorValueController MainEditorValueController { get; private set; }

    public override void _Process(double delta)
    {
        base._Process(delta);

        var primaryPlayerId = MainEditorValueController.Editor.PrimaryPlayerId;
        if (!MainEditorValueController.Editor.PlayerToolStates.TryGetValue(primaryPlayerId, out ToolState toolState)) return;
        StencilButton.SetStencilDefinition(toolState.StencilData);
    }
}
