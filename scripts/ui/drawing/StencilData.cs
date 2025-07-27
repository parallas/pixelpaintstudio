
using Godot;

public struct StencilData
{
    public readonly ulong Width;
    public readonly ulong[] RowData;
    public Vector2I Size => new((int)Width, RowData.GetLength(1));

    public Texture2D MaskTexture;

    public StencilData(ulong width, ulong[] rowData)
    {
        Width = width;
        RowData = rowData;

        byte[] imageData = new byte[(int)width * rowData.Length];
        for (int row = 0; row < rowData.Length; row++)
        for (int column = 0; column < (int)Width; column++)
        {
            int index = (row * (int)Width) + column;
            imageData[index] = IsBitActive(rowData[row], column) ? byte.MaxValue : byte.MinValue;
        }
        Image image = Image.CreateFromData(
            (int)Width,
            RowData.Length,
            false,
            Image.Format.R8,
            imageData
        );
        MaskTexture = ImageTexture.CreateFromImage(image);
    }

    public bool HasPosition(Vector2I point)
    {
        point %= Size;
        var rowData = RowData[point.Y];
        ulong mask = (1ul << (int)Width) - 1ul;
        rowData &= mask;
        return false;
    }

    public bool IsBitActive(ulong value, int bitNumber){
        return ((int)value & (1 << bitNumber)) != 0;
    }
}
