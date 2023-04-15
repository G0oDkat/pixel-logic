namespace GOoDkat.PixelLogic.Windows;

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Interop.Shell32;
using Miscellaneous;
using WinApi.User32;
using WinApi.Windows;

internal partial class MainWindow
{
    protected override void OnMessage(ref WindowMessage msg)
    {
        switch (msg.Id)
        {
            case WM.DROPFILES:
                DropFilesPacketizer.ProcessDropFiles(ref msg, this);
                break;
            default:
                base.OnMessage(ref msg);
                break;
        }
    }

    public void OnDropFiles(ref DropFilesPacket packet)
    {
        IntPtr hDrop = packet.HDrop;

        try
        {
            uint count = Shell32.DragQueryFileW(hDrop, 0xFFFFFFFF, IntPtr.Zero, 0);

            var paths = new string[(int)count];

            for (uint i = 0; i < count; i++)
            {
                uint size = Shell32.DragQueryFileW(hDrop, i, IntPtr.Zero, 0);

                char[] chars = ArrayPool<char>.Shared.Rent((int)(size + 1));

                try
                {
                    GCHandle handle = GCHandle.Alloc(chars, GCHandleType.Pinned);

                    try
                    {
                        Shell32.DragQueryFileW(hDrop, i, handle.AddrOfPinnedObject(), size + 1);

                        paths[i] = new string(chars.AsSpan(0, (int)size));
                    }
                    finally
                    {
                        if (handle.IsAllocated)
                            handle.Free();
                    }
                }
                finally
                {
                    ArrayPool<char>.Shared.Return(chars);
                }
            }

            OnFilesDropped(paths);
        }
        finally
        {
            Shell32.DragFinish(hDrop);
        }
    }

    private void OnFilesDropped(ICollection<string> paths)
    {
        string path = paths.First();

        //fileSystemWatcher.EnableRaisingEvents = false;

        try
        {
            Image image = CreateImageFromFile(path);
            LoadCircuitBoard(image);

            //fileSystemWatcher.Path = Path.GetDirectoryName(path);
            //fileSystemWatcher.Filter = Path.GetFileName(path);
            //fileSystemWatcher.EnableRaisingEvents = true;
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
        }
    }
}