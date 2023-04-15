namespace GOoDkat.PixelLogic;

using System;
using WinApi.User32;

internal class DispatcherEventLoop
{
    private readonly Dispatcher dispatcher;

    public DispatcherEventLoop(Dispatcher dispatcher)
    {
        this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
    }

    public void Run()
    {
        while (User32Methods.GetMessage(out Message message, IntPtr.Zero, 0, 0) > 0)
        {
            User32Methods.TranslateMessage(ref message);
            User32Methods.DispatchMessage(ref message);
            dispatcher.InvokePending();
        }
    }
}