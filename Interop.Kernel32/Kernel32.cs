namespace GOoDkat.Interop.Kernel32
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Kernel32
    {
        private const string LibraryName = "kernel32.dll";

        public const uint GHND = 0x0042u;
        public const uint GMEM_FIXED = 0x0000u;
        public const uint GMEM_MOVEABLE = 0x0002u;
        public const uint GPTR = 0x0040u;

        [DllImport(LibraryName, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

        [DllImport(LibraryName, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GlobalFree(IntPtr hMem);

        [DllImport(LibraryName, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport(LibraryName, ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport(LibraryName, ExactSpelling = true, SetLastError = true)]
        public static extern UIntPtr GlobalSize(IntPtr hMem);

        [DllImport(LibraryName, EntryPoint = "RtlCopyMemory", ExactSpelling = true, SetLastError = false)]
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, UIntPtr Length);
    }
}
