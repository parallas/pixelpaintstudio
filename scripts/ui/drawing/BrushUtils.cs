using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class BrushUtils
{
    #region Image Manipulation

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

    public static void Colorize(ImageTexture imageTexture, Image image, byte[] imageData, Color color)
    {
        var editingData = imageData.ToArray();
        for (int i = 0; i < editingData.Length; i+=4)
        {
            var r = editingData[i];
            var g = editingData[i + 1];
            var b = editingData[i + 2];
            var a = editingData[i + 3];

            Color c = new Color(r, g, b, 1);
            c.ToHsv(out var h, out var s, out var v);
            color.ToHsv(out var newH, out var newS, out var newV);
            h = newH;
            v *= Mathf.Lerp(1f, newV, s);
            s *= newS;
            Color cFromHsv = Color.FromHsv(h, s, v, a);
            editingData[i] = (byte)cFromHsv.R;
            editingData[i + 1] = (byte)cFromHsv.G;
            editingData[i + 2] = (byte)cFromHsv.B;
            editingData[i + 3] = (byte)cFromHsv.A;
        }

        image.SetData(image.GetWidth(), image.GetHeight(), image.HasMipmaps(), image.GetFormat(), editingData);
        if (image.HasMipmaps()) image.GenerateMipmaps(true);

        imageTexture.Update(image);
    }

    public static void Colorize(ImageDataCache imageDataCache, Color color)
    {
        var imageData = imageDataCache.ImageData;
        var image = imageDataCache.Image;
        var imageTexture = imageDataCache.ImageTexture;

        Colorize(imageTexture, image, imageData, color);
    }

    #endregion

    #region Classes

    public static class ImageDataCacher
    {
        public static Dictionary<ulong, ImageDataCache> ImageDataCaches = new Dictionary<ulong, ImageDataCache>();

    }

    #endregion

    #region Structs

    public struct ImageDataCache
    {
        public ulong InstanceId;
        public Image Image;
        public byte[] ImageData;
        public ImageTexture ImageTexture;

        public static ImageDataCache CreateFromTexture(Texture2D texture2D)
        {
            var image = texture2D.GetImage();
            var imageTexture = ImageTexture.CreateFromImage(image);
            var imageData = image.GetData();
            return new ImageDataCache
            {
                InstanceId = texture2D.GetInstanceId(),
                Image = image,
                ImageData = imageData,
                ImageTexture = imageTexture,
            };
        }
    }

    #endregion
}
