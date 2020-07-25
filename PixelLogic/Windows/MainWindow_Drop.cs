using System;
using System.Collections.Generic;
using System.Text;

namespace PixelLogic.Windows
{
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Models;
    using SharpDX;
    using SharpDX.Direct2D1;
    using SharpDX.DirectWrite;
    using SharpDX.DXGI;
    using Shell32;
    using WinApi.User32;
    using WinApi.Windows;
    using AlphaMode = SharpDX.Direct2D1.AlphaMode;
    using Image = Miscellaneous.Image;

    partial class MainWindow
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

            uint count = Shell32Methods.DragQueryFile(hDrop, 0xFFFFFFFF, null, 0);

            List<string> paths = new List<string>((int)count);

            for (uint i = 0; i < count; i++)
            {
                uint size = Shell32Methods.DragQueryFile(hDrop, i, null, 0);

                var path = new StringBuilder((int)size + 1);
                Shell32Methods.DragQueryFile(hDrop, i, path, size + 1);

                paths.Add(path.ToString());
            }

            OnFilesDropped(paths);

            Shell32Methods.DragFinish(hDrop);
        }

        private void OnFilesDropped(ICollection<string> paths)
        {
            string path = paths.First();

            fileSystemWatcher.EnableRaisingEvents = false;

            try
            {
                LoadCircuitBoard(path);

                fileSystemWatcher.Path = Path.GetDirectoryName(path);
                fileSystemWatcher.Filter = Path.GetFileName(path);
                fileSystemWatcher.EnableRaisingEvents = true;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

    }
}
