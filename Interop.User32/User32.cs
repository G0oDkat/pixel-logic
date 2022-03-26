namespace GOoDkat.Interop.User32
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class User32
    {
        private const string LibraryName = "user32.dll";

        public const uint CF_BITMAP = 2u;
        public const uint CF_DIB = 8u;
        public const uint CF_DIBV5 = 17u;
        public const uint CF_DIF = 5u;
        public const uint CF_DSPBITMAP = 0x0082u;
        public const uint CF_DSPENHMETAFILE = 0x008Eu;
        public const uint CF_DSPMETAFILEPICT = 0x0083u;
        public const uint CF_DSPTEXT = 0x0081u;
        public const uint CF_ENHMETAFILE = 14u;
        public const uint CF_GDIOBJFIRST = 0x0300u;
        public const uint CF_GDIOBJLAST = 0x03FFu;
        public const uint CF_HDROP = 15u;
        public const uint CF_LOCALE = 16u;
        public const uint CF_METAFILEPICT = 3u;
        public const uint CF_OEMTEXT = 7u;
        public const uint CF_OWNERDISPLAY = 0x0080u;
        public const uint CF_PALETTE = 9u;
        public const uint CF_PENDATA = 10u;
        public const uint CF_PRIVATEFIRST = 0x0200u;
        public const uint CF_PRIVATELAST = 0x02FFu;
        public const uint CF_RIFF = 11u;
        public const uint CF_SYLK = 4u;
        public const uint CF_TEXT = 1u;
        public const uint CF_TIFF = 6u;
        public const uint CF_UNICODETEXT = 13u;
        public const uint CF_WAVE = 12u;

        [DllImport(LibraryName, ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport(LibraryName, ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseClipboard();

        [DllImport(LibraryName, ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EmptyClipboard();

        [DllImport(LibraryName, ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsClipboardFormatAvailable(uint format);

        [DllImport(LibraryName, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport(LibraryName, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);
    }
}
