namespace PixelLogic.Windows
{
    using WinApi.Windows;

    internal static class DropFilesPacketizer
    {
        public static unsafe void ProcessDropFiles(ref WindowMessage msg, MainWindow window)
        {
            fixed (WindowMessage* ptr = &msg)
            {
                var packet = new DropFilesPacket(ptr);
                window.OnDropFiles(ref packet);
            }
        }
    }
}