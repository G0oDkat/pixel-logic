
namespace Interop.Shell32
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Shell32
    {
        private const string LibraryName = "shell32.dll";

        [DllImport(LibraryName, ExactSpelling = true, SetLastError = false)]
        public static extern void DragAcceptFiles(IntPtr hWnd, bool fAccept);

        [DllImport(LibraryName, ExactSpelling = true, SetLastError = false)]
        public static extern uint DragQueryFileW(IntPtr hDrop, uint iFile, IntPtr lpszFile, uint cch);

        [DllImport(LibraryName, ExactSpelling = true, SetLastError = false)]
        public static extern void DragFinish(IntPtr hDrop);
    }
}
