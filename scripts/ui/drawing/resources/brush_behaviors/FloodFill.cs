using Godot;
using System;
using Parallas;

[GlobalClass]
public partial class FloodFill : BrushBehavior
{
    private bool _hasFilled = false;
    private Color[,] _colorData;

    public override void Initialize(Vector2 cursorPosition, Color cursorColor)
    {
        base.Initialize(cursorPosition, cursorColor);
        _hasFilled = false;
    }

    public override void Draw(DrawState drawState, CanvasItem canvasItem)
    {
        base.Draw(drawState, canvasItem);
        if (_hasFilled) return;
        _hasFilled = true;

        if (canvasItem is not DrawCanvas drawCanvas) return;
        var existingTexture = drawCanvas.OutputTextureTarget.Texture;
        var image = existingTexture.GetImage();
        var imageData = (byte[])image.Data["data"];

        Vector2I pos = drawState.EvaluatedPosition.ToVector2I();
        int width = image.GetWidth();
        int height = image.GetHeight();
        _colorData ??= new Color[height, width];

        Color sampledColor = Colors.Transparent;
        for (var i = 0; i < imageData.Length; i += 4)
        {
            Color color = Color.Color8(
                imageData[i + 0],
                imageData[i + 1],
                imageData[i + 2],
                imageData[i + 3]
            );
            int y = (i / 4) % width;
            int x = (i / 4) / width;
            _colorData[x, y] = color;

            if (pos.X == y && pos.Y == x) sampledColor = color;
        }
        MyFill(_colorData, pos.X, pos.Y, sampledColor, drawState.EvaluatedColor);

        int indexCounter = 0;
        for (int y = 0; y < _colorData.GetLength(0); y++)
        for (int x = 0; x < _colorData.GetLength(1); x++)
        {
            var color = _colorData[y, x];
            imageData[indexCounter + 0] = (byte)color.R8;
            imageData[indexCounter + 1] = (byte)color.G8;
            imageData[indexCounter + 2] = (byte)color.B8;
            imageData[indexCounter + 3] = (byte)color.A8;
            indexCounter += 4;
        }

        image.SetData(width, height, false, image.GetFormat(), imageData);

        canvasItem.DrawTextureRect(ImageTexture.CreateFromImage(image), new Rect2(Vector2.Zero, image.GetSize()), false, Colors.White);
    }

    public static void MyFill(Color[,] array, int x, int y, Color oldColor, Color newColor)
    {
        if (array[y, x] == oldColor) _MyFill(array, x, y, array.GetLength(1), array.GetLength(0), oldColor, newColor);
    }

    static void _MyFill(Color[,] array, int x, int y, int width, int height, Color oldColor, Color newColor)
    {
        // at this point, we know array[y,x] is clear, and we want to move as far as possible to the upper-left. moving
        // up is much more important than moving left, so we could try to make this smarter by sometimes moving to
        // the right if doing so would allow us to move further up, but it doesn't seem worth the complexity
        while (true)
        {
            int ox = x, oy = y;
            while (y != 0 && array[y - 1, x] == oldColor) y--;
            while (x != 0 && array[y, x - 1] == oldColor) x--;
            if (x == ox && y == oy) break;
        }

        MyFillCore(array, x, y, width, height, oldColor, newColor);
    }

    static void MyFillCore(Color[,] array, int x, int y, int width, int height, Color oldColor, Color newColor)
    {
        // at this point, we know that array[y,x] is clear, and array[y-1,x] and array[y,x-1] are set.
        // we'll begin scanning down and to the right, attempting to fill an entire rectangular block
        int lastRowLength = 0; // the number of cells that were clear in the last row we scanned
        do
        {
            int rowLength = 0,
                sx = x; // keep track of how long this row is. sx is the starting x for the main scan below
            // now we want to handle a case like |***|, where we fill 3 cells in the first row and then after we move to
            // the second row we find the first  | **| cell is filled, ending our rectangular scan. rather than handling
            // this via the recursion below, we'll increase the starting value of 'x' and reduce the last row length to
            // match. then we'll continue trying to set the narrower rectangular block
            if (lastRowLength != 0 && array[y, x] != oldColor) // if this is not the first row and the leftmost cell is filled...
            {
                do
                {
                    if (--lastRowLength == 0) return; // shorten the row. if it's full, we're done
                } while (array[y, ++x] != oldColor); // otherwise, update the starting point of the main scan to match

                sx = x;
            }
            // we also want to handle the opposite case, | **|, where we begin scanning a 2-wide rectangular block and
            // then find on the next row that it has     |***| gotten wider on the left. again, we could handle this
            // with recursion but we'd prefer to adjust x and lastRowLength instead
            else
            {
                for (; x != 0 && array[y, x - 1] == oldColor; rowLength++, lastRowLength++)
                {
                    array[y, --x] =
                        newColor; // to avoid scanning the cells twice, we'll fill them and update rowLength here
                    // if there's something above the new starting point, handle that recursively. this deals with cases
                    // like |* **| when we begin filling from (2,0), move down to (2,1), and then move left to (0,1).
                    // the  |****| main scan assumes the portion of the previous row from x to x+lastRowLength has already
                    // been filled. adjusting x and lastRowLength breaks that assumption in this case, so we must fix it
                    if (y != 0 && array[y - 1, x] == oldColor)
                        _MyFill(array, x, y - 1, width, height, oldColor, newColor); // use _Fill since there may be more up and left
                }
            }

            // now at this point we can begin to scan the current row in the rectangular block. the span of the previous
            // row from x (inclusive) to x+lastRowLength (exclusive) has already been filled, so we don't need to
            // check it. so scan across to the right in the current row
            for (; sx < width && array[y, sx] == oldColor; rowLength++, sx++) array[y, sx] = newColor;
            // now we've scanned this row. if the block is rectangular, then the previous row has already been scanned,
            // so we don't need to look upwards and we're going to scan the next row in the next iteration so we don't
            // need to look downwards. however, if the block is not rectangular, we may need to look upwards or rightwards
            // for some portion of the row. if this row was shorter than the last row, we may need to look rightwards near
            // the end, as in the case of |*****|, where the first row is 5 cells long and the second row is 3 cells long.
            // we must look to the right  |*** *| of the single cell at the end of the second row, i.e. at (4,1)
            if (rowLength < lastRowLength)
            {
                for (int end = x + lastRowLength;
                     ++sx < end;) // 'end' is the end of the previous row, so scan the current row to
                {
                    // there. any clear cells would have been connected to the previous
                    if (array[y, sx] == oldColor)
                        MyFillCore(array, sx, y, width,
                            height, oldColor, newColor); // row. the cells up and left must be set so use FillCore
                }
            }
            // alternately, if this row is longer than the previous row, as in the case |*** *| then we must look above
            // the end of the row, i.e at (4,0)                                         |*****|
            else if (rowLength > lastRowLength && y != 0) // if this row is longer and we're not already at the top...
            {
                for (int ux = x + lastRowLength; ++ux < sx;) // sx is the end of the current row
                {
                    if (array[y - 1, ux] == oldColor)
                        _MyFill(array, ux, y - 1, width,
                            height, oldColor, newColor); // since there may be clear cells up and left, use _Fill
                }
            }

            lastRowLength = rowLength; // record the new row length
        } while (lastRowLength != 0 && ++y < height); // if we get to a full row or to the bottom, we're done
    }
}
