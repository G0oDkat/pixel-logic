namespace PixelLogic
{
    using System;

    internal interface IDispatcher
    {
        void Invoke(Action action);
    }
}