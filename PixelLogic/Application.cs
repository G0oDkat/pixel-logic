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

        public Application()
        {
            loop = new DispatcherEventLoop();
            IntPtr icon = User32Helpers.LoadIcon(IntPtr.Zero, SystemIcon.IDI_APPLICATION);
            windowFactory = WindowFactory.Create(hIcon: icon);
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
    }
}