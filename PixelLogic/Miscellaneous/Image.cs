namespace GOoDkat.PixelLogic.Miscellaneous;

using System;
using SharpDX.Direct2D1;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

internal class Image
{
    private readonly Bgra32[] data;

    public Image(int width, int height)
    {
        data = new Bgra32[width * height];
        Width = width;
        Height = height;
    }

    public int Width { get; }

    public int Height { get; }

    public Bgra32 this[int x, int y]
    {
        get => data[y * Width + x];
        set => data[y * Width + x] = value;
    }
        
    public static Image FromImageSharp(Image<Bgra32> image)
    {
        if (image == null)
            throw new ArgumentNullException(nameof(image));

        int width = image.Width;
        int height = image.Height;
        var result = new Image(width, height);

        image.CopyPixelDataTo(result.data);

        return result;
    }

    public Image<Bgra32> ToImageSharp()
    {
        return SixLabors.ImageSharp.Image.LoadPixelData(data, Width, Height);
    }

    public void CopyToBitmap(Bitmap1 bitmap)
    {
        bitmap.CopyFromMemory(data, bitmap.PixelSize.Width * 4);
    }

    public Image Clone()
    {
        var image = new Image(Width, Height);

        Array.Copy(data, image.data, data.Length);

        return image;
    }
}