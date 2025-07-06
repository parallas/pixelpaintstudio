using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Parallas;

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

    public static void ColorizeBasic(ImageTexture imageTexture, Image image, byte[] imageData, Color color)
    {
        Color newColor = color;
        Color okColor = Color.FromOkHsl(newColor.OkHslH, newColor.OkHslS, newColor.OkHslL);
        okColor.ToHsv(out var newHsvH, out var newHsvS, out var newHsvV);
        var okH = okColor.OkHslH;
        var okS = okColor.OkHslS;
        var okL = okColor.OkHslL;

        byte r = 0;
        byte g = 0;
        byte b = 0;
        byte a = 0;

        var editingData = new byte[imageData.Length];
        for (int i = 0; i < imageData.Length; i+=4)
        {
            r = imageData[i];
            g = imageData[i + 1];
            b = imageData[i + 2];
            a = imageData[i + 3];

            Color c = new Color(r, g, b, a);
            c.ToHsv(out var h, out var s, out var v);
            h = newHsvH;
            v *= Mathf.Lerp(1f, newHsvV, s);
            s *= newHsvS;
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

    public static void ColorizeBasic(ImageDataCache imageDataCache, Color color)
    {
        var imageData = imageDataCache.ImageData;
        var image = imageDataCache.Image;
        var imageTexture = imageDataCache.ImageTexture;

        ColorizeBasic(imageTexture, image, imageData, color);
    }

    public static void Colorize(ImageTexture imageTexture, Image image, byte[] imageData, Color color)
    {
        var editingData = new byte[imageData.Length];
        Color newColor = color;
        Color okColor = Color.FromOkHsl(newColor.OkHslH, newColor.OkHslS, newColor.OkHslL);
        okColor.ToHsv(out var newHsvH, out var newHsvS, out var newHsvV);
        var okH = okColor.OkHslH;
        var okS = okColor.OkHslS;
        var okL = okColor.OkHslL;

        byte r = 0;
        byte g = 0;
        byte b = 0;
        byte a = 0;
        for (int i = 0; i < editingData.Length; i+=4)
        {
            r = imageData[i];
            g = imageData[i + 1];
            b = imageData[i + 2];
            a = imageData[i + 3];
            Color oldColor = Color.Color8(r, g, b, a);

            var h = okH;
            var l = Mathf.Lerp(
                oldColor.OkHslL >= okL ? okL : oldColor.OkHslL,
                oldColor.OkHslL >= okL ? oldColor.OkHslL : okL,
                oldColor.OkHslL
            );
            var s = okS * Mathf.Lerp(1f, newHsvS, l);
            Color finalColor = Color.FromOkHsl(h, s, l, oldColor.A);

            editingData[i] = (byte)finalColor.R8;
            editingData[i + 1] = (byte)finalColor.G8;
            editingData[i + 2] = (byte)finalColor.B8;
            editingData[i + 3] = (byte)oldColor.A8;
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
        public static Dictionary<ulong, ImageDataCache> ImageDataCaches { get; private set; } = new Dictionary<ulong, ImageDataCache>();

        private static ImageDataCache CacheImageData(Texture2D texture)
        {
            var key = texture.GetInstanceId();
            var newData = ImageDataCache.CreateFromTexture(texture);
            ImageDataCaches.Add(key, newData);
            return newData;
        }

        public static ImageDataCache CreateOrGet(Texture2D texture, ImageDataCache? current = null)
        {
            var key = texture.GetInstanceId();
            // if (current.GetValueOrDefault() is var currentValue && currentValue.InstanceId == key)
                // return currentValue;
            if (TryGet(key, out ImageDataCache cache)) return cache;
            return CacheImageData(texture);
        }

        public static bool TryGet(ulong uniqueId, out ImageDataCache result)
        {
            if (ImageDataCaches.TryGetValue(uniqueId, out result)) return true;
            return false;
        }
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
