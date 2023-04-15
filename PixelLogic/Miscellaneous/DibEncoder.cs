namespace GOoDkat.PixelLogic.Miscellaneous;

using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.PixelFormats;

internal class DibEncoder : IImageEncoder
{
    private readonly BmpEncoder encoder;

    public DibEncoder()
    {
        encoder = new BmpEncoder()
        {
            BitsPerPixel = BmpBitsPerPixel.Pixel32,
            SupportTransparency = true
        };
    }

    public void Encode<TPixel>(Image<TPixel> image, Stream stream) where TPixel : unmanaged, IPixel<TPixel>
    {
        int headerSize = Marshal.SizeOf(typeof(BITMAPFILEHEADER));

        using (var s = new MemoryStream())
        {
            encoder.Encode<TPixel>(image, s);

            s.Seek(headerSize, SeekOrigin.Begin);
            s.CopyTo(stream);
        }
    }

    public async Task EncodeAsync<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
    {
        int headerSize = Marshal.SizeOf(typeof(BITMAPFILEHEADER));

        // ReSharper disable once UseAwaitUsing
        using (var s = new MemoryStream())
        {
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            encoder.Encode<TPixel>(image, s);

            s.Seek(headerSize, SeekOrigin.Begin);
            await s.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
        }
    }
}