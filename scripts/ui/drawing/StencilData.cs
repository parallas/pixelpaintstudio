
using Godot;

public partial class StencilData : Resource
{
    public readonly ulong Width;
    public readonly ulong[] RowData;
    public Vector2I Size => new((int)Width, RowData.GetLength(1));

    public Texture2D MaskTexture;
    public Texture2D MaskTextureScaled;

    public StencilData(ulong width, ulong[] rowData)
    {
        Width = width;
        RowData = rowData;

        byte[] imageData = new byte[(int)width * rowData.Length];
        for (int row = 0; row < rowData.Length; row++)
        for (int column = 0; column < (int)Width; column++)
        {
            int index = (row * (int)Width) + column;
            int bitIndex = (int)Width - column - 1; // idk why this has to be inverted. guess bc bits are RtoL ???
            imageData[index] = IsBitActive(rowData[row], bitIndex) ? byte.MaxValue : byte.MinValue;
        }
        Image image = Image.CreateFromData(
            (int)Width,
            RowData.Length,
            false,
            Image.Format.R8,
            imageData
        );
        MaskTexture = ImageTexture.CreateFromImage(image);

        image.Resize((int)Width * 4, RowData.Length * 4, Image.Interpolation.Nearest);
        MaskTextureScaled = ImageTexture.CreateFromImage(image);
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
        return (value & (1ul << bitNumber)) != 0;
    }
}
