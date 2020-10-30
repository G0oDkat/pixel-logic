namespace PixelLogic.Miscellaneous
{
    using SixLabors.ImageSharp.PixelFormats;

    internal static class ImageExtensions
    {
        private static readonly Bgra32 Black = new Bgra32(0x00, 0x00, 0x00);
        private static readonly Bgra32 Gray = new Bgra32(0x80, 0x80, 0x80);

        public static bool IsWire(this Image image, int x, int y)
        {
            Bgra32 pixel = image[x, y];

            return pixel != Black && pixel != Gray;
        }
    }
}