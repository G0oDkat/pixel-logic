namespace Interop.Comdlg32
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Comdlg32
    {
        private const string LibraryName = "comdlg32.dll";


        public const uint OFN_ALLOWMULTISELECT = 0x00000200;
        public const uint OFN_CREATEPROMPT = 0x00002000;
        public const uint OFN_DONTADDTORECENT = 0x02000000;
        public const uint OFN_ENABLEHOOK = 0x00000020;
        public const uint OFN_ENABLEINCLUDENOTIFY = 0x00400000;
        public const uint OFN_ENABLESIZING = 0x00800000;
        public const uint OFN_ENABLETEMPLATE = 0x00000040;
        public const uint OFN_ENABLETEMPLATEHANDLE = 0x00000080;
        public const uint OFN_EXPLORER = 0x00080000;
        public const uint OFN_EXTENSIONDIFFERENT = 0x00000400;
        public const uint OFN_FILEMUSTEXIST = 0x00001000;
        public const uint OFN_FORCESHOWHIDDEN = 0x10000000;
        public const uint OFN_HIDEREADONLY = 0x00000004;
        public const uint OFN_LONGNAMES = 0x00200000;
        public const uint OFN_NOCHANGEDIR = 0x00000008;
        public const uint OFN_NODEREFERENCELINKS = 0x00100000;
        public const uint OFN_NOLONGNAMES = 0x00040000;
        public const uint OFN_NONETWORKBUTTON = 0x00020000;
        public const uint OFN_NOREADONLYRETURN = 0x00008000;
        public const uint OFN_NOTESTFILECREATE = 0x00010000;
        public const uint OFN_NOVALIDATE = 0x00000100;
        public const uint OFN_OVERWRITEPROMPT = 0x00000002;
        public const uint OFN_PATHMUSTEXIST = 0x00000800;
        public const uint OFN_READONLY = 0x00000001;
        public const uint OFN_SHAREAWARE = 0x00004000;
        public const uint OFN_SHOWHELP = 0x00000010;

        [DllImport(LibraryName, ExactSpelling = true, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetSaveFileNameW(ref LPOPENFILENAMEW Arg1);

        [DllImport(LibraryName, ExactSpelling = true, SetLastError = false)]
        public static extern uint CommDlgExtendedError();

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LPOPENFILENAMEW
        {
            public uint lStructSize;

            public IntPtr hwndOwner;

            public IntPtr hInstance;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpstrFilter;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpstrCustomFilter;

            public uint nMaxCustFilter;

            public uint nFilterIndex;

            public IntPtr lpstrFile;

            public uint nMaxFile;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpstrFileTitle;

            public uint nMaxFileTitle;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpstrInitialDir;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpstrTitle;

            public uint Flags;

            public ushort nFileOffset;

            public ushort nFileExtension;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpstrDefExt;

            public IntPtr lCustData;

            public IntPtr lpfnHook;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpTemplateName;

            //public IntPtr lpEditInfo;

            //[MarshalAs(UnmanagedType.LPStr)]
            //public string lpstrPrompt;

            public IntPtr pvReserved;

            public uint dwReserved;

            public uint FlagsEx;
        }
    }
}
