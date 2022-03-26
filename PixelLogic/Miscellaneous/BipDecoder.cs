namespace GOoDkat.PixelLogic.Miscellaneous
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Formats;
    using SixLabors.ImageSharp.Formats.Bmp;
    using SixLabors.ImageSharp.PixelFormats;

    internal class DibDecoder : IImageDecoder
    {
        private readonly BmpDecoder decoder;

        public DibDecoder()
        {
            decoder = new BmpDecoder();
        }

        public Image<TPixel> Decode<TPixel>(Configuration configuration, Stream stream, CancellationToken token) where TPixel : unmanaged, IPixel<TPixel>
        {
            using (Stream bmpStream = DibToBmpStream(stream))
            {
                return decoder.Decode<TPixel>(configuration, bmpStream, token);
            }
        }

        public SixLabors.ImageSharp.Image Decode(Configuration configuration, Stream stream, CancellationToken token)
        {
            using (Stream bmpStream = DibToBmpStream(stream))
            {
                return decoder.Decode(configuration, bmpStream, token);
            }
        }

        public async Task<Image<TPixel>> DecodeAsync<TPixel>(Configuration configuration, Stream stream, CancellationToken token) where TPixel : unmanaged, IPixel<TPixel>
        {
            // ReSharper disable once UseAwaitUsing
            using (Stream bmpStream = await DibToBmpStreamAsync(stream, token).ConfigureAwait(false))
            {
                return decoder.Decode<TPixel>(configuration, bmpStream, token);
            }
        }

        public async Task<SixLabors.ImageSharp.Image> DecodeAsync(Configuration configuration, Stream stream, CancellationToken token)
        {
            // ReSharper disable once UseAwaitUsing
            using (Stream bmpStream = await DibToBmpStreamAsync(stream, token).ConfigureAwait(false))
            {
                // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                return decoder.Decode(configuration, bmpStream, token);
            }
        }

        private Stream DibToBmpStream(Stream stream)
        {
            int dibSize = (int)stream.Length;
            int fileHeaderSize = Marshal.SizeOf<BITMAPFILEHEADER>();
            int bmpSize = fileHeaderSize + dibSize;

            var buffer = new byte[bmpSize];

            stream.ReadFixed(buffer.AsSpan(fileHeaderSize, dibSize));

            var infoHeader = MemoryMarshal.Read<BITMAPINFOHEADER>(buffer.AsSpan(fileHeaderSize));

            var fileHeader = new BITMAPFILEHEADER
            {
                bfType = BITMAPFILEHEADER.BM,
                bfSize = fileHeaderSize + infoHeader.biSize + infoHeader.biSizeImage,
                bfReserved1 = 0,
                bfReserved2 = 0,
                bfOffBits = fileHeaderSize + infoHeader.biSize + infoHeader.biClrUsed * 4
            };

            MemoryMarshal.Write(buffer, ref fileHeader);

            return new MemoryStream(buffer);
        }

        private async Task<Stream> DibToBmpStreamAsync(Stream stream, CancellationToken token)
        {
            int dibSize = (int)stream.Length;
            int fileHeaderSize = Marshal.SizeOf<BITMAPFILEHEADER>();
            int bmpSize = fileHeaderSize + dibSize;

            var buffer = new byte[bmpSize];

            await stream.ReadFixedAsync(buffer.AsMemory(fileHeaderSize, dibSize), token).ConfigureAwait(false);

            var infoHeader = MemoryMarshal.Read<BITMAPINFOHEADER>(buffer.AsSpan(fileHeaderSize));

            var fileHeader = new BITMAPFILEHEADER
            {
                bfType = BITMAPFILEHEADER.BM,
                bfSize = fileHeaderSize + infoHeader.biSize + infoHeader.biSizeImage,
                bfReserved1 = 0,
                bfReserved2 = 0,
                bfOffBits = fileHeaderSize + infoHeader.biSize + infoHeader.biClrUsed * 4
            };

            MemoryMarshal.Write(buffer, ref fileHeader);

            return new MemoryStream(buffer);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct BITMAPFILEHEADER
    {
        public static readonly short BM = 0x4d42; // BM

        public short bfType;
        public int bfSize;
        public short bfReserved1;
        public short bfReserved2;
        public int bfOffBits;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct BITMAPINFOHEADER
    {
        public int biSize;
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public int biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;
    }

    //private static SixLabors.ImageSharp.Image<T> ImageFromRawDIB<T>(byte[] buffer) where T : unmanaged, IPixel<T>
    //{
    //    BITMAPINFOHEADER infoHeader = MemoryMarshal.Read<BITMAPINFOHEADER>(buffer);

    //    int fileHeaderSize = Marshal.SizeOf(typeof(BITMAPFILEHEADER));
    //    int infoHeaderSize = infoHeader.biSize;
    //    int fileSize = fileHeaderSize + infoHeader.biSize + infoHeader.biSizeImage;

    //    var fileHeader = new BITMAPFILEHEADER
    //    {
    //        bfType = BITMAPFILEHEADER.BM,
    //        bfSize = fileSize,
    //        bfReserved1 = 0,
    //        bfReserved2 = 0,
    //        bfOffBits = fileHeaderSize + infoHeaderSize + infoHeader.biClrUsed * 4
    //    };

    //    var bmpBuffer = new byte[fileHeaderSize + buffer.Length];

    //    MemoryMarshal.Write(bmpBuffer, ref fileHeader);
    //    Buffer.BlockCopy(buffer, 0, bmpBuffer, fileHeaderSize, buffer.Length);

    //    return SixLabors.ImageSharp.Image.Load<T>(bmpBuffer, new BmpDecoder());
    //}
}
