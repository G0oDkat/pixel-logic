namespace GOoDkat.PixelLogic;

using System;
using Serilog;

internal static class Program
{
    private static void Main()
    {
        Log.Logger = new LoggerConfiguration()
                     .MinimumLevel.Information()
                     .WriteTo.Debug()
                     .CreateLogger();

        try
        {
            using (var application = new PixelLogicApp())
            {
                application.Run();
            }
        }
        catch (Exception exception)
        {
            Log.Fatal(exception, "A fatal exception occurred.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}