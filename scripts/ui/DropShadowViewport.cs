using Godot;

[GlobalClass, Tool]
public partial class DropShadowViewport : DropShadow
{
    [Export] public required Viewport Viewport;

    public override void _Ready()
    {
        if (Viewport is null) return;

        Texture = Viewport.GetTexture();

        base._Ready();
    }
}
