namespace PixelLogic.Miscellaneous
{
    using System;
    using System.Drawing.Imaging;
    using System.IO;
    using SharpDX;
    using SharpDX.Direct2D1;
    using SharpDX.WIC;
    using PixelFormat = SharpDX.WIC.PixelFormat;

    internal class Image
    {
        private readonly uint[] data;

        public Image(int width, int height)
        {
            data = new uint[width * height];
            Width = width;
            Height = height;
        }

        public int Width { get; }

        public int Height { get; }

        public uint GetPixel(int x, int y)
        {
            return data[y * Width + x];
        }

        public void SetPixel(int x, int y, uint value)
        {
            data[y * Width + x] = value;
        }

        public static Image FromFormatConverter(FormatConverter converter)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            if (converter.PixelFormat != PixelFormat.Format32bppBGRA)
                throw new ArgumentException("invalid pixel format");

            Size2 size = converter.Size;

            var image = new Image(size.Width, size.Height);

            converter.CopyPixels(image.data);

            return image;
        }

        public static Image FromDrawing(System.Drawing.Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            int width = bitmap.Width;
            int height = bitmap.Height;
            var image = new Image(width, height);


            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    image.SetPixel(x,y, (uint)bitmap.GetPixel(x, y).ToArgb());
                }
            }

            return image;
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
}