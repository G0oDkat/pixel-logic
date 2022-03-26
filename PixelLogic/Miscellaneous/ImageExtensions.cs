namespace GOoDkat.PixelLogic.Miscellaneous
{
    using SixLabors.ImageSharp.PixelFormats;

    internal static class ImageExtensions
    {
        private static readonly Bgra32 Black = new (0x00, 0x00, 0x00);
        private static readonly Bgra32 Gray = new (0x80, 0x80, 0x80);

        public static bool IsWire(this Image image, int x, int y)
        {
            Bgra32 color = image[x, y];

            return color != Black && color != Gray;
        }
    }
}