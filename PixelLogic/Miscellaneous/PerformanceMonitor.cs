namespace PixelLogic.Miscellaneous
{
    using System.Diagnostics;

    internal class PerformanceMonitor
    {
        private const int Interval = 500;

        private readonly Stopwatch stopwatch;
        private int fps;

        private int frames;

        public PerformanceMonitor()
        {
            stopwatch = new Stopwatch();
        }

        public void Start()
        {
            stopwatch.Start();
        }

        public void Stop()
        {
            stopwatch.Stop();
            frames = 0;
        }

        public void NotifyFrameChanged()
        {
            frames++;
        }

        public int GetFps()
        {
            long elapsed = stopwatch.ElapsedMilliseconds;

            if (elapsed > Interval)
            {
                fps = frames * 1000 / (int)elapsed;
                frames = 0;
                stopwatch.Restart();
            }

            return fps;
        }
    }
}