namespace PixelLogic.Miscellaneous
{
    using System;

    public static class RandomExtension
    {
        public static bool NextBoolean(this Random random)
        {
            return random.Next() > int.MaxValue / 2;
        }
    }
}