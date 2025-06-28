using Godot;
using System;

public partial class MainEditor : Control
{
    [Export] public ToolState ToolState { get; set; }
    [Export] public Toolbar Toolbar { get; set; }
    [Export] public GameCursor GameCursor { get; set; }

    public override void _Ready()
    {
        base._Ready();

        Toolbar.SetToolState(ToolState);
        GameCursor.SetToolState(ToolState);
    }
}
