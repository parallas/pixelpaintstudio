using Godot;
using System;
using System.Linq;
using Godot.Collections;
using Parallas;

public partial class PlayerCanvas : TextureRect
{
    [Export] private Array<SubViewport> _subViewports;
    [Export] public DrawCanvas DrawCanvas;
    [Export] public TextureRect OutputTextureTarget { get; private set; }
    [Export] public Texture2D MaskTexture { get; private set; }

    public override void _Ready()
    {
        if (_subViewports.Last() is not { } subViewport) return;
        Material = Material.Duplicate() as ShaderMaterial;
        SetTexture(subViewport.GetTexture());
        SetOutputTextureTarget(OutputTextureTarget);
        SetMaskTexture(MaskTexture);
    }

    public void SetResolution(Vector2I resolution)
    {
        DrawCanvas.SetResolution(resolution);
        foreach (var subViewport in _subViewports)
        {
            subViewport.Size = resolution;
        }

        if (Material is ShaderMaterial shaderMaterial)
        {
            shaderMaterial.SetShaderParameter("texture_size", resolution.ToVector2());
        }
    }

    public void SetOutputTextureTarget(TextureRect outputTextureTarget)
    {
        OutputTextureTarget = outputTextureTarget;
        DrawCanvas.OutputTextureTarget = outputTextureTarget;
    }

    public void SetMaskTexture(Texture2D maskTexture)
    {
        MaskTexture = maskTexture;
        if (Material is ShaderMaterial shaderMaterial)
        {
            shaderMaterial.SetShaderParameter("stencil_tex", maskTexture);
            shaderMaterial.SetShaderParameter("stencil_size", maskTexture?.GetSize() ?? Vector2.One);
        }
    }
}
