namespace PixelLogic.Windows
{
    using System;
    using SharpDX;
    using WinApi.User32;
    using WinApi.Windows;
    using Image = Miscellaneous.Image;
    using MouseButton = WinApi.Windows.MouseButton;
    using Point = NetCoreEx.Geometry.Point;
    using Rectangle = NetCoreEx.Geometry.Rectangle;

    internal partial class MainWindow
    {
        #region Keyboard

        protected override void OnKey(ref KeyPacket packet)
        {
            keyboard.ProcessKey(ref packet);
        }

        private void OnKeyboardKeyDown(object sender, VirtualKey key)
        {
            switch (key)
            {
                case VirtualKey.F11:
                    ToggleFullScreen();
                    break;
                case VirtualKey.ADD:
                    IncreaseTicks();
                    break;
                case VirtualKey.SUBTRACT:
                    DecreaseTicks();
                    break;
                case VirtualKey.SPACE:
                    ResetTransformation();
                    break;
                case VirtualKey.ESCAPE:
                    Destroy();
                    break;
                case VirtualKey.S:
                    if (keyboard.IsKeyDown(VirtualKey.CONTROL))
                    {
                        SaveCircuitBoardWithDialog();
                    }
                    break;
                case VirtualKey.C:
                    if (keyboard.IsKeyDown(VirtualKey.CONTROL))
                    {
                        CopyCircuitBoard();
                    }
                    break;
                case VirtualKey.V:
                    if (keyboard.IsKeyDown(VirtualKey.CONTROL))
                    {
                        PasteCircuitBoard();
                    }
                    break;
            }
        }

        #endregion

        #region Mouse

        protected override void OnMouseMove(ref MousePacket packet)
        {
            if (isRightMouseButtonDown)
            {
                Point mousePosition = packet.Point;

                float diffX = mousePosition.X - lastMousePosition.X;
                float diffY = mousePosition.Y - lastMousePosition.Y;

                translation = new Vector2(translation.X + diffX / zoom, translation.Y + diffY / zoom);

                lastMousePosition = mousePosition;
                UpdateTransformation();
            }
        }

        protected override void OnMouseButton(ref MouseButtonPacket packet)
        {
            if (packet.IsButtonDown)
                switch (packet.Button)
                {
                    case MouseButton.Left:
                        OnLeftMouseButtonDown(packet.Point);
                        break;
                    case MouseButton.Right:
                        OnRightMouseButtonDown(packet.Point);
                        break;
                }
            else
                switch (packet.Button)
                {
                    case MouseButton.Left:
                        OnLeftMouseButtonUp(packet.Point);
                        break;
                    case MouseButton.Right:
                        OnRightMouseButtonUp(packet.Point);
                        break;
                }
        }

        protected void OnLeftMouseButtonDown(Point point)
        {
            Image image = circuitBoard.Image;

            float width = image.Width;
            float height = image.Height;

            float x = point.X / zoom - translation.X + width / 2f;
            float y = point.Y / zoom - translation.Y + height / 2f;

            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                circuitBoard.ForceWireAt((int) x, (int) y);
                isWireForced = true;
                forcedWireX = (int) x;
                forcedWireY = (int) y;
            }
        }

        protected void OnLeftMouseButtonUp(Point point)
        {
            if (isWireForced)
            {
                if (!User32Methods.GetKeyState(VirtualKey.SHIFT).IsPressed)
                    circuitBoard.UnforceWireAt(forcedWireX, forcedWireY);

                isWireForced = false;
            }
        }

        protected void OnRightMouseButtonDown(Point point)
        {
            isRightMouseButtonDown = true;
            lastMousePosition = point;
            User32Methods.SetCapture(Handle);
        }

        protected void OnRightMouseButtonUp(Point point)
        {
            isRightMouseButtonDown = false;
            User32Methods.ReleaseCapture();
        }

        protected override void OnMouseWheel(ref MouseWheelPacket packet)
        {
            Point mousePosition = ScreenToClient(packet.Point);

            var positionWorld = new Vector2(mousePosition.X / zoom - translation.X,
                                            mousePosition.Y / zoom - translation.Y);

            if (packet.WheelDelta > 0)
            {
                float z = zoom * zoomFactor;

                if (z <= maxZoom)
                {
                    zoom = z;
                    translation = new Vector2((translation.X - (positionWorld.X * zoomFactor - positionWorld.X)) / zoomFactor, (translation.Y - (positionWorld.Y * zoomFactor - positionWorld.Y)) / zoomFactor);
                }
            }
            else
            {
                float z = zoom / zoomFactor;

                if (z >= minZoom)
                {
                    zoom = z;
                    translation = new Vector2((translation.X + (positionWorld.X - (positionWorld.X / zoomFactor))) * zoomFactor, (translation.Y + (positionWorld.Y - (positionWorld.Y / zoomFactor))) * zoomFactor);
                }
            }

            UpdateTransformation();
        }

        private Point ScreenToClient(Point point)
        {
            var client = new Rectangle();

            User32Methods.AdjustWindowRectEx(ref client, GetStyles(), User32Methods.GetMenu(Handle) != IntPtr.Zero,
                                             GetExStyles());
            GetWindowRect(out Rectangle window);

            return new Point(point.X - window.Left + client.Left, point.Y - window.Top + client.Top);
        }

        #endregion
    }
}