namespace GOoDkat.PixelLogic;

using System.Drawing;
using System.IO;
using System.Reflection;
using Windows;
using Serilog;
using WinApi.Windows;

internal class PixelLogicApp : Application
{
    private readonly Icon icon;
    private readonly WindowFactory factory;
    private readonly MainWindow window;
        
    public PixelLogicApp()
    {
        icon = LoadIcon();
        factory = WindowFactory.Create(hIcon: icon.Handle);
        window = MainWindow.Create(factory);
        window.Destroyed += OnWindowDestroyed;
    }

    protected override void OnStart()
    {
        Log.Information("Application started");

        window.CenterToScreen();
        window.Show();
    }

    protected override void OnStop()
    {
        Log.Information("Application stopped");
    }

    private void OnWindowDestroyed()
    {
        Exit();
    }

    private static Icon LoadIcon()
    {
        var assembly = Assembly.GetExecutingAssembly();

        using (Stream stream = assembly.GetManifestResourceStream("GOoDkat.PixelLogic.Logo.ico"))
        {
            var loadIcon = new Icon(stream, 128, 128);
            return loadIcon;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            window?.Dispose();
            icon?.Dispose();
        }

        base.Dispose(disposing);
    }
}