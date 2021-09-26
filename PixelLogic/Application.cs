namespace PixelLogic
{
    using System;
    using System.Collections.Generic;
    using WinApi.User32;
    using WinApi.Windows;
    using WinApi.Windows.Helpers;

    internal class Application : IDisposable
    {
        private readonly IEventLoop loop;
        private readonly WindowFactory windowFactory;
        private readonly HashSet<WindowCore> windows;
        //private readonly Icon icon;

        public Application()
        {
            loop = new DispatcherEventLoop();
            //icon = LoadIcon();
            windowFactory = WindowFactory.Create(/*hIcon:icon.Handle*/);
            windows = new HashSet<WindowCore>();
        }

        public void Dispose()
        {
            if (windows != null)
                foreach (WindowCore window in windows)
                    window?.Dispose();
        }

        public int Run()
        {
            OnStart();

            int result = loop.Run();

            OnStop();

            return result;
        }

        public void Exit()
        {
            MessageHelpers.PostQuitMessage();
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnStop()
        {
        }

        protected TWindow CreateWindow<TWindow>(Func<TWindow> func, string title) where TWindow : WindowCore
        {
            var constructionParams = new FrameWindowConstructionParams();
            WindowExStyles exStyles = WindowExStyles.WS_EX_APPWINDOW | WindowExStyles.WS_EX_NOREDIRECTIONBITMAP;

            TWindow window =
                windowFactory.CreateWindow(func, title, constructionParams: constructionParams, exStyles: exStyles);

            windows.Add(window);

            return window;
        }

        //private Icon LoadIcon()
        //{
        //    string fileName = Process.GetCurrentProcess().MainModule.FileName;
        //    return Icon.ExtractAssociatedIcon(fileName);
        //}
    }
}