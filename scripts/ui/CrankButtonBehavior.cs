using Godot;
using System;
using Parallas;

public partial class CrankButtonBehavior : Node
{
    [Export] public Toolbar Toolbar;
    [Export] public Node3D CrankNode;

    public override void _Process(double delta)
    {
        base._Process(delta);

        var targetQuaternion = Quaternion.FromEuler(new Vector3(0f, 0f, Toolbar.IsMenuOpen ? 0f : Mathf.Pi * 0.5f));
        CrankNode.Quaternion = MathUtil.ExpDecay(CrankNode.Quaternion, targetQuaternion, 20f, (float)delta);
    }

    public void Toggle()
    {
        Toolbar.SetMenuOpen(!Toolbar.IsMenuOpen);
    }
}
