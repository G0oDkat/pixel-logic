namespace GOoDkat.PixelLogic;

using System;

internal interface IDispatcher
{
    void Invoke(Action action);
}