namespace PixelLogic
{
    using System;
    using System.Collections.Concurrent;

    internal class Dispatcher : IDispatcher
    {
        public static IDispatcher Current;

        private readonly ConcurrentQueue<Action> pending;

        public Dispatcher()
        {
            pending = new ConcurrentQueue<Action>();
        }

        public void Invoke(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            pending.Enqueue(action);
        }

        public void InvokePending()
        {
            while (pending.TryDequeue(out Action action))
            {
                action.Invoke();
            }
        }
    }
}