namespace PixelLogic
{
    using System;
    using WinApi.User32;
    using WinApi.Windows;

    internal class DispatcherEventLoop : EventLoopCore
    {
        private readonly Dispatcher dispatcher;

        public DispatcherEventLoop(object state = null) : base(state)
        {
            dispatcher = new Dispatcher();
            Dispatcher.Current = dispatcher;
        }

        public override int RunCore()
        {
            Message msg;
            int res;
            while ((res = User32Methods.GetMessage(out msg, IntPtr.Zero, 0, 0)) > 0)
            {
                User32Methods.TranslateMessage(ref msg);
                User32Methods.DispatchMessage(ref msg);
                dispatcher.InvokePending();
            }

            return res;
        }
    }
}