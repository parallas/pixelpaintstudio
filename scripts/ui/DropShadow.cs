using Godot;
using System;

[GlobalClass, Tool]
public partial class DropShadow : TextureRect
{
    [Export] public Vector2 Offset = new Vector2(8, 8);

    public override void _EnterTree()
    {
        base._EnterTree();

        UpdateProperties();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        UpdateProperties();
    }

    public virtual void UpdateProperties()
    {
        Modulate = new Color(0f, 0f, 0f, 0.25f);
        Position = Offset;
        ShowBehindParent = true;
    }
}
