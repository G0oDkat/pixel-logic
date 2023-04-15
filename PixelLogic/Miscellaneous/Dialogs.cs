namespace GOoDkat.PixelLogic.Miscellaneous;

using System;
using System.Runtime.InteropServices;
using System.Text;
using Interop.Comdlg32;

internal static class Dialogs
{
    public static bool OpenSaveFileDialog(SaveFileDialogOptions options, out string fileName)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));


        string lpstrFilter;
        if (options.Filters.Count > 0)
        {
            var builder = new StringBuilder();

            foreach ((string key, string value) in options.Filters)
            {
                builder.Append(key);
                builder.Append('\0');
                builder.Append(value);
                builder.Append('\0');
            }

            lpstrFilter = builder.ToString();
        }
        else
        {
            lpstrFilter = null;
        }

        var buffer = new char[1024];

        options.FileName?.CopyTo(0, buffer, 0, Math.Min(options.FileName.Length, 1024));

        GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

        try
        {
            var arg = new Comdlg32.LPOPENFILENAMEW
            {
                lStructSize = (uint)Marshal.SizeOf<Comdlg32.LPOPENFILENAMEW>(),
                hwndOwner = IntPtr.Zero,
                hInstance = IntPtr.Zero,
                lpstrFilter = lpstrFilter,
                lpstrCustomFilter = null,
                nMaxCustFilter = 0,
                nFilterIndex = 0,
                lpstrFile = handle.AddrOfPinnedObject(),
                nMaxFile = 1024,
                lpstrFileTitle = null,
                nMaxFileTitle = 0,
                lpstrInitialDir = null,
                lpstrTitle = null,
                Flags = Comdlg32.OFN_PATHMUSTEXIST | Comdlg32.OFN_OVERWRITEPROMPT,
                nFileOffset = 0,
                nFileExtension = 0,
                lpstrDefExt = options.DefaultExtension,
                lCustData = IntPtr.Zero,
                lpfnHook = IntPtr.Zero,
                lpTemplateName = null,
                //lpEditInfo = IntPtr.Zero,
                //lpstrPrompt = null,
                pvReserved = IntPtr.Zero,
                dwReserved = 0,
                FlagsEx = 0
            };

            if (Comdlg32.GetSaveFileNameW(ref arg))
            {
                fileName = new string(buffer, 0, Array.IndexOf(buffer, '\0'));
                return true;
            }
        }
        finally
        {
            if (handle.IsAllocated)
            {
                handle.Free();
            }
        }
            
        //uint error = Comdlg32.CommDlgExtendedError();

        fileName = null;
        return false;
    }
}