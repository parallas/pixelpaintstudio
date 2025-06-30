using System;
using Godot;

public static class BrushUtils
{
    public static Texture2D HueShift(Texture2D texture, float hueShift)
    {
        var image = texture.GetImage();
        var imageData = image.GetData();
        for (int i = 0; i < imageData.Length; i+=4)
        {
            var r = imageData[i];
            var g = imageData[i + 1];
            var b = imageData[i + 2];
            var a = imageData[i + 3];

            Color c = new Color(r, g, b, a);
            c.ToHsv(out var h, out var s, out var v);
            h += hueShift;
            Color cFromHsv = Color.FromHsv(h, s, v);
            imageData[i] = (byte)cFromHsv.R;
            imageData[i + 1] = (byte)cFromHsv.G;
            imageData[i + 2] = (byte)cFromHsv.B;
            imageData[i + 3] = a;
        }

        image.SetData(image.GetWidth(), image.GetHeight(), image.HasMipmaps(), image.GetFormat(), imageData);
        return ImageTexture.CreateFromImage(image);
    }

    public static Texture2D Colorize(Texture2D texture, Color color)
    {
        var image = texture.GetImage();
        var imageData = image.GetData();
        for (int i = 0; i < imageData.Length; i+=4)
        {
            var r = imageData[i];
            var g = imageData[i + 1];
            var b = imageData[i + 2];
            var a = imageData[i + 3];

            Color c = new Color(r, g, b, a);
            c.ToHsv(out var h, out var s, out var v);
            color.ToHsv(out var newH, out var newS, out var newV);
            h = newH;
            v *= Mathf.Lerp(1f, newV, s);
            s *= newS;
            Color cFromHsv = Color.FromHsv(h, s, v);
            imageData[i] = (byte)cFromHsv.R;
            imageData[i + 1] = (byte)cFromHsv.G;
            imageData[i + 2] = (byte)cFromHsv.B;
            imageData[i + 3] = a;
        }

        image.SetData(image.GetWidth(), image.GetHeight(), image.HasMipmaps(), image.GetFormat(), imageData);
        return ImageTexture.CreateFromImage(image);
    }
}
