using Godot;
using System;
using Godot.Collections;

public partial class MainEditor : Control
{
    [Export] public ToolState ToolState { get; set; }
    [Export] public Toolbar Toolbar { get; set; }
    [Export] public Topbar Topbar { get; set; }
    [Export] public Array<GameCursor> GameCursors { get; set; }

    public override void _Ready()
    {
        base._Ready();

        Toolbar.SetToolState(ToolState);
        Topbar.SetToolState(ToolState);
        foreach (var gameCursor in GameCursors)
        {
            gameCursor.SetToolState(ToolState);
        }

    }
}
