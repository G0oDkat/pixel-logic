namespace GOoDkat.PixelLogic;

using System;
using WinApi.Windows.Helpers;

internal class Application : IDisposable
{
    private readonly DispatcherEventLoop loop;

    public Application()
    {
        Dispatcher = new Dispatcher();
        loop = new DispatcherEventLoop(Dispatcher);
    }

    protected Dispatcher Dispatcher { get; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Run()
    {
        OnStart();
        loop.Run();
        OnStop();
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

    protected virtual void Dispose(bool disposing)
    {
    }
}