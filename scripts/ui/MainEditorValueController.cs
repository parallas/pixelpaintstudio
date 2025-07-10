using Godot;
using System;

[GlobalClass]
public partial class MainEditorValueController : Node
{
    public MainEditor Editor { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        Editor ??= GetTree().GetFirstNodeInGroup("main_editor") as MainEditor;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Editor ??= GetTree().GetFirstNodeInGroup("main_editor") as MainEditor;
    }

    public void SetEditorPlayerValue(int playerId)
    {
        Editor ??= GetTree().GetFirstNodeInGroup("main_editor") as MainEditor;
        if (Editor is null) return;

        Editor.PrimaryPlayerId = playerId;
    }
}
