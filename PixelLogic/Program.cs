namespace GOoDkat.PixelLogic
{
    internal static class Program
    {
        private static void Main()
        {
            using (var application = new PixelLogicApp())
            {
                application.Run();
            }
        }
    }
}