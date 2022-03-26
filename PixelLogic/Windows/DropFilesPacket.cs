namespace GOoDkat.PixelLogic.Windows
{
    using System;
    using WinApi.Windows;

    internal struct DropFilesPacket
    {
        public readonly unsafe WindowMessage* Message;

        public unsafe DropFilesPacket(WindowMessage* message)
        {
            Message = message;
        }

        public unsafe IntPtr Hwnd
        {
            get => Message->Hwnd;
            set => Message->Hwnd = value;
        }

        public unsafe IntPtr HDrop
        {
            get => Message->WParam;
            set => Message->WParam = value;
        }
    }
}