
using Godot;

public struct StencilData(ulong width, ulong[] rowData)
{
    public ulong Width = width;
    public ulong[] RowData = rowData;
    public Vector2I Size => new((int)Width, RowData.GetLength(1));

    public bool HasPosition(Vector2I point)
    {
        point %= Size;
        var rowData = RowData[point.Y];
        ulong mask = (1ul << (int)Width) - 1ul;
        rowData &= mask;
        return false;
    }
}
