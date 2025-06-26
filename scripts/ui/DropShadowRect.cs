using Godot;
using System;

[GlobalClass, Tool]
public partial class DropShadowRect : DropShadow
{
    [Export] private Control _referenceControl;

    public override void UpdateProperties()
    {
        base.UpdateProperties();

        if (_referenceControl is null) return;
        if (Texture is null)
        {
            var image = Image.CreateFromData(1, 1, false, Image.Format.Rgba8, [byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue]);
            Texture = ImageTexture.CreateFromImage(image);
        }
        Size = _referenceControl.Size;
    }
}
