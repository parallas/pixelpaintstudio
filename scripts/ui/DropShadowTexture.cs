using Godot;
using System;

[GlobalClass, Tool]
public partial class DropShadowTexture : DropShadow
{
    [Export] private TextureRect _referenceTextureRect;

    public override void UpdateProperties()
    {
        base.UpdateProperties();

        if (_referenceTextureRect is null) return;
        Texture = _referenceTextureRect.Texture;
        Size = _referenceTextureRect.Size;
        ExpandMode = _referenceTextureRect.ExpandMode;
        FlipH = _referenceTextureRect.FlipH;
        FlipV = _referenceTextureRect.FlipV;
        StretchMode = _referenceTextureRect.StretchMode;
        TextureFilter = _referenceTextureRect.TextureFilter;
        TextureRepeat = _referenceTextureRect.TextureRepeat;
    }
}
