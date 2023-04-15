namespace GOoDkat.PixelLogic.Miscellaneous;

using System;
using System.Diagnostics;

internal class PerformanceMonitor
{
    private readonly Stopwatch stopwatch;

    private readonly TimeSpan interval;

    private int frameCount;
    private int tickCount;


    public PerformanceMonitor(TimeSpan interval)
    {
        this.interval = interval;
        stopwatch = new Stopwatch();
    }

    public double FramesPerSecond{ get; private set; }

    public double TicksPerSecond { get; private set; }

    public void Start()
    {
        stopwatch.Start();
    }

    public void Stop()
    {
        stopwatch.Stop();
        frameCount = 0;
        tickCount = 0;
    }

    public void IncrementTicks()
    {
        tickCount++;
    }

    public void IncermentFrames()
    {
        frameCount++;
    }

    public bool TryCalculate()
    {
        TimeSpan elapsed = stopwatch.Elapsed;

        if (elapsed > interval)
        {
            FramesPerSecond = frameCount / elapsed.TotalSeconds;
            TicksPerSecond = tickCount / elapsed.TotalSeconds;
            frameCount = 0;
            tickCount = 0;
            stopwatch.Restart();

            return true;
        }

        return false;
    }
}