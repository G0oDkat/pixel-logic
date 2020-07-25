using System;
using System.Text;

namespace PixelLogic.Shell32
{
    using System.Runtime.InteropServices;

    public static class Shell32Methods
    {
        public const string LibraryName = "shell32";

        [DllImport(LibraryName, ExactSpelling = true)]
        public static extern void DragAcceptFiles(IntPtr hwnd, bool fAccept);

        [DllImport(LibraryName, ExactSpelling = true)]
        public static extern uint DragQueryFile(IntPtr hDrop, uint iFile, [Out] StringBuilder filename, uint cch);

        [DllImport(LibraryName, ExactSpelling = true)]
        public static extern void DragFinish(IntPtr hDrop);
    }
}
