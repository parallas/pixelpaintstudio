using Godot;

public partial class PaintBlob : Node3D
{
    [Export] private MeshInstance3D _meshInstance3D;

    private Material _material;

    public override void _Ready()
    {
        base._Ready();

        _material = (Material)_meshInstance3D.GetActiveMaterial(0).Duplicate();
        _meshInstance3D.SetMaterialOverride(_material);
    }

    public void SetColor(Color color)
    {
        if (_material is ShaderMaterial shaderMaterial)
        {
            shaderMaterial.SetShaderParameter("paint_tint", color);
        }
    }

    public void SetTexture(Texture texture)
    {
        if (_material is ShaderMaterial shaderMaterial)
        {
            shaderMaterial.SetShaderParameter("paint_tex", texture);
        }
    }
}
