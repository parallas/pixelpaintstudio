using Godot;
using System;
using System.Collections.Generic;
using Parallas;

[GlobalClass]
public partial class FloodFill : BrushBehavior
{
    private bool _hasFilled = false;
    private Image _fillImage;
    private ImageTexture _fillImageTexture;
    private int _fillCount = 0;
    private int _fillStep = 0;
    private bool[,] _imageMask;
    private bool[,] _fillMask;
    private byte[] _fillImageData;
    private List<Vector2I> _fillPoints = new List<Vector2I>();

    public override void Initialize(Vector2 cursorPosition, Color cursorColor)
    {
        base.Initialize(cursorPosition, cursorColor);
        _hasFilled = false;
        _fillCount = 0;
        _fillStep = 0;
        _imageMask = null;
        _fillMask = null;
        _fillImageData = null;
        _fillPoints = new List<Vector2I>();
        _fillImage = null;
        _fillImageTexture = null;
    }

    public override void Process(DrawState drawState, double delta)
    {
        base.Process(drawState, delta);

        if (drawState.CanvasItem is not DrawCanvas drawCanvas) return;

        if (!_hasFilled)
        {
            var existingTexture = drawCanvas.OutputTextureTarget.Texture;
            using var image = existingTexture.GetImage();
            var imageData = image.Data["data"].AsByteArray();
            _fillImageData ??= new byte[imageData.Length];

            Vector2I pos = drawState.EvaluatedPosition.ToVector2I();
            int width = image.GetWidth();
            int height = image.GetHeight();
            _imageMask ??= new bool[height, width];
            _fillMask ??= new bool[height, width];
            _fillImage ??= Image.CreateEmpty(width, height, false, image.GetFormat());
            _fillImageTexture ??= ImageTexture.CreateFromImage(_fillImage);

            int clickedIndex = PosToIndex(pos, width);
            Color sampledColor = GetColorAtByteIndex(imageData, clickedIndex);
            if (sampledColor == drawState.EvaluatedColor) return;

            for (var i = 0; i < imageData.Length; i += 4)
            {
                Color color = GetColorAtByteIndex(imageData, i);
                var fillPos = GetFillPosFromByteIndex(i, width);
                _fillMask[fillPos.X, fillPos.Y] = false;
                _imageMask[fillPos.X, fillPos.Y] = color != sampledColor;
            }

            MyFill(_imageMask, _fillMask, pos.X, pos.Y);

            for (int y = 0; y < _fillMask.GetLength(0); y++)
            for (int x = 0; x < _fillMask.GetLength(1); x++)
            {
                var fillThis = _fillMask[y, x];
                if (!fillThis) continue;
                _fillPoints.Add(new Vector2I(x, y));
                _fillCount++;
            }
            _hasFilled = true;
        }
    }

    public override void Draw(DrawState drawState, CanvasItem canvasItem)
    {
        base.Draw(drawState, canvasItem);

        var h = drawState.EvaluatedColor.OkHslH;
        var s = drawState.EvaluatedColor.OkHslS;
        var l = drawState.EvaluatedColor.OkHslL;
        var evaluatedColor = Color.FromOkHsl(h, s, l);

        for (int i = 0; i < 200; i++)
        {
            if (_fillStep >= _fillCount) break;
            var fillPos = _fillPoints[_fillStep];
            int fillIndex = PosToIndex(fillPos, _fillImage.GetWidth());
            _fillImageData[fillIndex + 0] = (byte)evaluatedColor.R8;
            _fillImageData[fillIndex + 1] = (byte)evaluatedColor.G8;
            _fillImageData[fillIndex + 2] = (byte)evaluatedColor.B8;
            _fillImageData[fillIndex + 3] = (byte)evaluatedColor.A8;
            _fillStep++;
        }
        _fillImage.SetData(_fillImage.GetWidth(), _fillImage.GetHeight(), false, _fillImage.GetFormat(), _fillImageData);
        _fillImageTexture.Update(_fillImage);
        canvasItem.DrawTextureRect(_fillImageTexture, new Rect2(Vector2.Zero, _fillImage.GetSize()), false, Colors.White);

    }

    private int PosToIndex(Vector2I pos, int imageWidth)
    {
        int byteWidth = (imageWidth * 4);
        int clickedIndex = ((pos.X * 4) % byteWidth) + pos.Y * byteWidth;
        return clickedIndex;
    }

    /// <summary>
    /// Converts an image data byte index to a fill mask position. The fill mask has X and Y swapped.
    /// </summary>
    /// <param name="index">The starting index of the color in an images byte array data.</param>
    /// <param name="imageWidth">The width in pixels of the image.</param>
    /// <returns></returns>
    private Vector2I GetFillPosFromByteIndex(int index, int imageWidth) =>
        new Vector2I((index / 4) / imageWidth, (index / 4) % imageWidth);

    private Color GetColorAtByteIndex(byte[] imageData, int index) =>
        Color.Color8(
            imageData[index + 0],
            imageData[index + 1],
            imageData[index + 2],
            imageData[index + 3]
        );

    public static void MyFill(bool[,] array, bool[,] fillMask, int x, int y)
    {
        if (!array[y, x]) _MyFill(array, fillMask, x, y, array.GetLength(1), array.GetLength(0));
    }

    static void _MyFill(bool[,] array, bool[,] fillMask, int x, int y, int width, int height)
    {
        // at this point, we know array[y,x] is clear, and we want to move as far as possible to the upper-left. moving
        // up is much more important than moving left, so we could try to make this smarter by sometimes moving to
        // the right if doing so would allow us to move further up, but it doesn't seem worth the complexity
        while (true)
        {
            int ox = x, oy = y;
            while (y != 0 && !array[y - 1, x]) y--;
            while (x != 0 && !array[y, x - 1]) x--;
            if (x == ox && y == oy) break;
        }

        MyFillCore(array, fillMask, x, y, width, height);
    }

    static void MyFillCore(bool[,] array, bool[,] fillMask, int x, int y, int width, int height)
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
            if (lastRowLength != 0 && array[y, x]) // if this is not the first row and the leftmost cell is filled...
            {
                do
                {
                    if (--lastRowLength == 0) return; // shorten the row. if it's full, we're done
                } while (array[y, ++x]); // otherwise, update the starting point of the main scan to match

                sx = x;
            }
            // we also want to handle the opposite case, | **|, where we begin scanning a 2-wide rectangular block and
            // then find on the next row that it has     |***| gotten wider on the left. again, we could handle this
            // with recursion but we'd prefer to adjust x and lastRowLength instead
            else
            {
                for (; x != 0 && !array[y, x - 1]; rowLength++, lastRowLength++)
                {
                    var xValue = --x;
                    fillMask[y, xValue] = true;
                    array[y, xValue] =
                        true; // to avoid scanning the cells twice, we'll fill them and update rowLength here
                    // if there's something above the new starting point, handle that recursively. this deals with cases
                    // like |* **| when we begin filling from (2,0), move down to (2,1), and then move left to (0,1).
                    // the  |****| main scan assumes the portion of the previous row from x to x+lastRowLength has already
                    // been filled. adjusting x and lastRowLength breaks that assumption in this case, so we must fix it
                    if (y != 0 && !array[y - 1, x])
                        _MyFill(array, fillMask, x, y - 1, width, height); // use _Fill since there may be more up and left
                }
            }

            // now at this point we can begin to scan the current row in the rectangular block. the span of the previous
            // row from x (inclusive) to x+lastRowLength (exclusive) has already been filled, so we don't need to
            // check it. so scan across to the right in the current row
            for (; sx < width && !array[y, sx]; rowLength++, sx++)
            {
                array[y, sx] = true;
                fillMask[y, sx] = true;
            }
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
                    if (!array[y, sx])
                        MyFillCore(array, fillMask, sx, y, width,
                            height); // row. the cells up and left must be set so use FillCore
                }
            }
            // alternately, if this row is longer than the previous row, as in the case |*** *| then we must look above
            // the end of the row, i.e at (4,0)                                         |*****|
            else if (rowLength > lastRowLength && y != 0) // if this row is longer and we're not already at the top...
            {
                for (int ux = x + lastRowLength; ++ux < sx;) // sx is the end of the current row
                {
                    if (!array[y - 1, ux])
                        _MyFill(array, fillMask, ux, y - 1, width,
                            height); // since there may be clear cells up and left, use _Fill
                }
            }

            lastRowLength = rowLength; // record the new row length
        } while (lastRowLength != 0 && ++y < height); // if we get to a full row or to the bottom, we're done
    }
}
