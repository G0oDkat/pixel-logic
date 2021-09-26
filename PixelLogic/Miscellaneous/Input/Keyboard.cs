namespace PixelLogic.Miscellaneous.Input
{
    using System;
    using System.Collections.Generic;
    using WinApi.User32;
    using WinApi.Windows;

    public class Keyboard
    {
        private readonly HashSet<VirtualKey> keys;

        public Keyboard()
        {
            keys = new HashSet<VirtualKey>();
        }

        public event EventHandler<VirtualKey> KeyDown;
        public event EventHandler<VirtualKey> KeyUp;

        public void ProcessKey(ref KeyPacket packet)
        {
            VirtualKey key = packet.Key;

            if (packet.IsKeyDown)
            {
                if (!packet.InputState.IsPreviousKeyStatePressed)
                {
                    keys.Add(key);
                    RaiseKeyDown(key);
                }
            }
            else
            {
                keys.Remove(key);
                RaiseKeyUp(key);
            }
        }

        public bool IsKeyDown(VirtualKey key)
        {
            return keys.Contains(key);
        }

        public bool IsKeyUp(VirtualKey key)
        {
            return !keys.Contains(key);
        }

        private void RaiseKeyDown(VirtualKey key)
        {
            KeyDown?.Invoke(this, key);
        }

        private void RaiseKeyUp(VirtualKey key)
        {
            KeyUp?.Invoke(this, key);
        }
    }
}