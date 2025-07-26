using Godot;
using System;

public partial class CanvasStencilTexture : Control
{
    [Export] private SubViewport _subViewport;
    [Export] private Texture2D _maskTexture;

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Material is ShaderMaterial shaderMaterial)
        {
            shaderMaterial.SetShaderParameter("stencil_texture", _maskTexture);
            shaderMaterial.SetShaderParameter("stencil_size", _maskTexture.GetSize());
            shaderMaterial.SetShaderParameter("texture_size", _subViewport.Size);
        }
    }
}
