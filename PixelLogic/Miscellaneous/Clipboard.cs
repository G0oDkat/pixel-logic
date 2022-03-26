namespace GOoDkat.PixelLogic.Miscellaneous
{
    using System;
    using System.Buffers;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.InteropServices;
    using Interop.Kernel32;
    using Interop.User32;
    using SixLabors.ImageSharp.PixelFormats;

    internal static class Clipboard
    {
        public static void SetContent<T>(ClipboardFormat format, ReadOnlySpan<T> content) where T : unmanaged
        {
            ReadOnlySpan<byte> byteContent = MemoryMarshal.Cast<T, byte>(content);

            OpenClipboard();

            try
            {
                EmptyClipboard();

                IntPtr clipboard = AllocateClipboard(byteContent.Length);

                try
                {
                    IntPtr contentPointer = LockClipboard(clipboard);

                    try
                    {
                        unsafe
                        {
                            fixed(void* pinnedContent = byteContent) 
                            {
                                CopyClipboard((IntPtr)pinnedContent, contentPointer, byteContent.Length);
                            }
                        }
                    }
                    finally
                    {
                        UnlockClipboard(clipboard);
                    }

                    SetClipboard(format, clipboard);
                }
                catch (Exception)
                {
                    FreeClipboard(clipboard);
                    throw;
                }
            }
            finally
            {
                CloseClipboard();
            }
        }

        public static byte[] GetContent(ClipboardFormat format)
        {
            if (!IsClipboardFormatAvailable(format))
            {
                throw new InvalidOperationException($"Clipboard contains no content for specified format ({format}).");
            }

            OpenClipboard();

            try
            {
                IntPtr clipboard = GetClipboard(format);

                int size = GetClipboardSize(clipboard);

                var content = new byte[size];

                IntPtr contentPointer = LockClipboard(clipboard);

                try
                {
                    GCHandle handle = GCHandle.Alloc(content, GCHandleType.Pinned);

                    try
                    {
                        CopyClipboard(contentPointer, handle.AddrOfPinnedObject(), size);

                        return content;
                    }
                    finally
                    {
                        if (handle.IsAllocated)
                        {
                            handle.Free();
                        }
                    }
                }
                finally
                {
                    UnlockClipboard(clipboard);
                }
            }
            finally
            {
                CloseClipboard();
            }
        }

        public static bool TryGetContent(ClipboardFormat format, out byte[] content)
        {
            if (IsClipboardFormatAvailable(format))
            {
                OpenClipboard();

                try
                {
                    IntPtr clipboard = GetClipboard(format);

                    int size = GetClipboardSize(clipboard);

                    content = new byte[size];

                    IntPtr contentPointer = LockClipboard(clipboard);

                    try
                    {
                        GCHandle handle = GCHandle.Alloc(content, GCHandleType.Pinned);

                        try
                        {
                            CopyClipboard(contentPointer, handle.AddrOfPinnedObject(), size);
                        }
                        finally
                        {
                            if (handle.IsAllocated)
                                handle.Free();
                        }
                    }
                    finally
                    {
                        UnlockClipboard(clipboard);
                    }
                }
                finally
                {
                    CloseClipboard();
                }

                return true;
            }

            content = null;

            return false;
        }

        public static bool HasContent(ClipboardFormat format)
        {
            return IsClipboardFormatAvailable(format);
        }

        #region Text

        public static void SetText(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            int size = (text.Length + 1);

            char[] array = ArrayPool<char>.Shared.Rent(size);

            try
            {
                text.AsSpan().CopyTo(array);
                array[text.Length] = '\0';

                Span<char> content = array.AsSpan(0, size);

                SetContent<char>(ClipboardFormat.UNICODETEXT, content);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(array);
            }
        }

        public static string GetText()
        {
            byte[] content = GetContent(ClipboardFormat.UNICODETEXT);

            Span<char> span = MemoryMarshal.Cast<byte, char>(content);

            return new string(span.Slice(0, span.Length - 1));
        }

        public static bool TryGetText(out string text)
        {
            if (TryGetContent(ClipboardFormat.UNICODETEXT, out byte[] content))
            {
                Span<char> span = MemoryMarshal.Cast<byte, char>(content);

                text = new string(span.Slice(0, span.Length - 1));

                return true;
            }

            text = null;
            return false;
        }

        #endregion

        #region Image
        public static void SetImage(SixLabors.ImageSharp.Image image)
        {
            using (var stream = new MemoryStream())
            {
                image.Save(stream, new DibEncoder());

                SetContent<byte>(ClipboardFormat.DIB, stream.GetBuffer().AsSpan(0, (int)stream.Length));
            }
        }

        public static SixLabors.ImageSharp.Image<T> GetImage<T>() where T : unmanaged, IPixel<T>
        {
            byte[] content = GetContent(ClipboardFormat.DIB);

            return SixLabors.ImageSharp.Image.Load<T>(content, new DibDecoder());
        }
        
        public static bool TryGetImage<T>(out SixLabors.ImageSharp.Image<T> image) where T : unmanaged, IPixel<T>
        {
            if (TryGetContent(ClipboardFormat.DIB, out byte[] content))
            {
                image = SixLabors.ImageSharp.Image.Load<T>(content, new DibDecoder());
                return true;
            }

            image = null;
            return false;
        }

        #endregion
        private static void OpenClipboard(IntPtr window = default)
        {
            if (!User32.OpenClipboard(window))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        private static void EmptyClipboard()
        {
            if (!User32.EmptyClipboard())
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        private static void CloseClipboard()
        {
            if (!User32.CloseClipboard())
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        private static IntPtr AllocateClipboard(int size)
        {
            IntPtr pointer = Kernel32.GlobalAlloc(Kernel32.GMEM_MOVEABLE, (UIntPtr)size);

            if (pointer == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return pointer;
        }

        private static void FreeClipboard(IntPtr pointer)
        {
            pointer = Kernel32.GlobalFree(pointer);

            if (pointer != IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        private static IntPtr LockClipboard(IntPtr pointer)
        {
            IntPtr lockedPointer = Kernel32.GlobalLock(pointer);

            if (lockedPointer == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            return lockedPointer;
        }

        private static void UnlockClipboard(IntPtr pointer)
        {
            bool isLocked = Kernel32.GlobalUnlock(pointer);

            if (!isLocked)
            {
                int lastWin32Error = Marshal.GetLastWin32Error();

                if (lastWin32Error != 0)
                {
                    throw new Win32Exception(lastWin32Error);
                }
            }
        }

        private static void CopyClipboard(IntPtr source, IntPtr destination, int size)
        {
            Kernel32.CopyMemory(destination, source, (UIntPtr)size);
        }

        private static void SetClipboard(ClipboardFormat format, IntPtr pointer)
        {
            if (User32.SetClipboardData((uint) format, pointer) == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        private static IntPtr GetClipboard(ClipboardFormat format)
        {
            IntPtr pointer = User32.GetClipboardData((uint) format);
            
            if (pointer == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return pointer;
        }

        private static int GetClipboardSize(IntPtr pointer)
        {
            UIntPtr size = Kernel32.GlobalSize(pointer);

            if (size == UIntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return (int) size;
        }

        private static bool IsClipboardFormatAvailable(ClipboardFormat format)
        {
            return User32.IsClipboardFormatAvailable((uint)format);
        }
    }

    internal enum ClipboardFormat : uint
    {
        BITMAP = 2u,
        DIB = 8u,
        DIBV5 = 17u,
        DIF = 5u,
        DSPBITMAP = 0x0082u,
        DSPENHMETAFILE = 0x008Eu,
        DSPMETAFILEPICT = 0x0083u,
        DSPTEXT = 0x0081u,
        ENHMETAFILE = 14u,
        GDIOBJFIRST = 0x0300u,
        GDIOBJLAST = 0x03FFu,
        HDROP = 15u,
        LOCALE = 16u,
        METAFILEPICT = 3u,
        OEMTEXT = 7u,
        OWNERDISPLAY = 0x0080u,
        PALETTE = 9u,
        PENDATA = 10u,
        PRIVATEFIRST = 0x0200u,
        PRIVATELAST = 0x02FFu,
        RIFF = 11u,
        SYLK = 4u,
        TEXT = 1u,
        TIFF = 6u,
        UNICODETEXT = 13u,
        WAVE = 12u
    }
}
