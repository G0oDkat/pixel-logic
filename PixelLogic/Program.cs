using System;

namespace PixelLogic
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var application = new PixelLogicApp())
            {
                application.Run();
            }
        }
    }
}
