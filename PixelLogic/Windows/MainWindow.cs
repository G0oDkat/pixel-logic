namespace PixelLogic.Windows
{
    using System;
    using Miscellaneous;
    using Models;
    using NetCoreEx.Geometry;
    using SharpDX;
    using SharpDX.Direct2D1;
    using SharpDX.DirectWrite;
    using SharpDX.DXGI;
    using SharpDX.IO;
    using SharpDX.Mathematics.Interop;
    using SharpDX.WIC;
    using WinApi.DxUtils;
    using WinApi.DxUtils.Component;
    using WinApi.User32;
    using WinApi.Windows;
    using AlphaMode = SharpDX.Direct2D1.AlphaMode;
    using BitmapInterpolationMode = SharpDX.Direct2D1.BitmapInterpolationMode;
    using Image = Miscellaneous.Image;
    using PixelFormat = SharpDX.Direct2D1.PixelFormat;
    using Point = NetCoreEx.Geometry.Point;
    using Rectangle = NetCoreEx.Geometry.Rectangle;

    internal class MainWindow : DxWindow
    {
        private readonly RawColor4 clearColor;

        private readonly PerformanceMonitor performanceMonitor;
        private readonly RawColor4 textColor;
        private Bitmap1 bitmap;

        private CircuitBoard circuitBoard;
        private readonly RawMatrix3x2 identityMatrix;

        private bool isRightMouseButtonDown;
        private Point lastMousePosition;
        private readonly float maxZoom;
        private readonly float minZoom;
        private Matrix3x2 scalingMatrix;

        private SolidColorBrush textBrush;
        private TextFormat textFormat;
        private RawMatrix3x2 transformationMatrix;
        private Vector2 translation;
        private Matrix3x2 translationMatrix;

        private float zoom;
        private readonly float zoomFactor;

        private bool isWireForced;
        private int forcedWireX;
        private int forcedWireY;

        public MainWindow()
        {
            clearColor = new RawColor4(0f, 0f, 0f, 1f);
            textColor = new RawColor4(1f, 1f, 1f, 1f);

            zoom = 4f;
            zoomFactor = 2f;
            minZoom = 1f;
            maxZoom = 64f;
            translation = Vector2.Zero;
            identityMatrix = Matrix3x2.Identity;

            performanceMonitor = new PerformanceMonitor();
        }

        protected override void OnCreate(ref CreateWindowPacket packet)
        {
            base.OnCreate(ref packet);

            Image image = CreateImage("D:\\Projects\\PixelLogic\\PixelLogic\\demo.png");

            circuitBoard = CircuitBoard.FromImage(image);

            bitmap = new Bitmap1(Dx.D2D.Context, new Size2(image.Width, image.Height),
                                 new BitmapProperties1(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Ignore)));
            textBrush = new SolidColorBrush(Dx.D2D.Context, textColor);
            textFormat = new TextFormat(Dx.TextFactory, "Consolas", 16);

            Size size = GetClientSize();

            translation = new Vector2(size.Width / (zoom * 2f), size.Height / (zoom * 2f));
            UpdateTransformation();
            performanceMonitor.Start();
        }

        private Image CreateImage(string path)
        {
            using (var factory = new ImagingFactory())
            using (var stream = new NativeFileStream(path, NativeFileMode.Open, NativeFileAccess.Read))
            using (var decoder = new BitmapDecoder(factory, stream, DecodeOptions.CacheOnDemand))
            {
                BitmapFrameDecode frame = decoder.GetFrame(0);

                using (var converter = new FormatConverter(factory))
                {
                    converter.Initialize(frame, SharpDX.WIC.PixelFormat.Format32bppBGRA);
                    return Image.FromFormatConverter(converter);
                }
            }
        }

        protected override void OnDestroy(ref Packet packet)
        {
            performanceMonitor.Stop();
            base.OnDestroy(ref packet);
        }

        protected override void OnPaint(ref PaintPacket packet)
        {
            Dx.EnsureInitialized();

            try
            {
                OnDxPaint(Dx);
                Dx.D3D.SwapChain.Present(1, 0);
            }
            catch (SharpDXException ex)
            {
                if (!Dx.PerformResetOnException(ex)) throw;
            }
        }

        protected override void OnDxPaint(Dx11Component resource)
        {
            DeviceContext context = resource.D2D.Context;

            Size clientSize = GetClientSize();

            circuitBoard.Update();

            circuitBoard.Image.CopyToBitmap(bitmap);

            context.BeginDraw();
            context.Clear(clearColor);

            context.Transform = transformationMatrix;

            context.DrawBitmap(bitmap, CreateImageRect(bitmap.PixelSize, clientSize), 1f,
                               BitmapInterpolationMode.NearestNeighbor);

            context.Transform = identityMatrix;

            context.DrawText($"{performanceMonitor.GetFps()} FPS | 0 TPS", textFormat,
                             new RawRectangleF(0f, 0f, clientSize.Width, clientSize.Height), textBrush);
            context.EndDraw();
            performanceMonitor.NotifyFrameChanged();
        }

        private RawRectangleF CreateImageRect(Size2 imageSize, Size clientSize)
        {
            //int left = (clientSize.Width - imageSize.Width) / 2;
            //int top = (clientSize.Height - imageSize.Height) / 2;
            //int right = left + imageSize.Width;
            //int bottom = top + imageSize.Height;
            float left = -imageSize.Width / 2f;
            float top = -imageSize.Height / 2f;
            float right = imageSize.Width / 2f;
            float bottom = imageSize.Height / 2f;

            return new RawRectangleF(left, top, right, bottom);
        }

        protected override void OnMouseWheel(ref MouseWheelPacket packet)
        {
            Point mousePosition = packet.Point;

            //var bla = new Vector2(mousePosition.X/zoom, mousePosition.Y/zoom);

            if (packet.WheelDelta > 0)
            {
                float z = zoom * zoomFactor;

                if (z <= maxZoom)
                {
                    zoom = z;

                    //var bla2 = new Vector2(mousePosition.X / zoom, mousePosition.Y / zoom);

                    //var offsetX = bla2.X - bla.X;
                    //var offsetY = bla2.Y - bla.Y;

                    translation = new Vector2(translation.X / zoomFactor, translation.Y / zoomFactor);
                }

                //translation = new Vector2((mousePosition.X - (mousePosition.X - translation.X) * zoomFactor), (mousePosition.Y - (mousePosition.Y - translation.Y) * zoomFactor));
            }
            else
            {
                float z = zoom / zoomFactor;

                if (z >= minZoom)
                {
                    zoom = z;
                    translation = new Vector2(translation.X * zoomFactor, translation.Y * zoomFactor);
                }

                //translation = new Vector2((mousePosition.X - (mousePosition.X - translation.X) / zoomFactor), (mousePosition.Y - (mousePosition.Y - translation.Y) / zoomFactor));
            }

            UpdateTransformation();
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

            float x = point.X / zoom - translation.X + (width / 2f);
            float y = point.Y / zoom - translation.Y + (height / 2f);

            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                circuitBoard.ToggleForceWireAt((int)x, (int)y);
                isWireForced = true;
                forcedWireX = (int)x;
                forcedWireY = (int)y;
            }
        }

        protected void OnLeftMouseButtonUp(Point point)
        {
            if (isWireForced)
            {
                circuitBoard.UnforceWireAt(forcedWireX, forcedWireY);
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

        protected override void OnKey(ref KeyPacket packet)
        {
            if (packet.IsKeyDown)
            {
            }
            else
            {
                if (packet.Key == VirtualKey.F11)
                    ToggleFullScreen();
            }
        }

        private void UpdateTransformation()
        {
            transformationMatrix = Matrix3x2.Translation(translation) * Matrix.Scaling(zoom);
        }

        private void ToggleFullScreen()
        {
            if (!isFullScreen)
            {
                isMaximized = User32Methods.IsZoomed(Handle);

                //if (isMaximized)
                //    SetState(ShowWindowCommands.SW_RESTORE);

                windowStyles = GetStyles();
                windowExStyles = GetExStyles();
                GetWindowRect(out windowRect);

                SetStyle(windowStyles & ~(WindowStyles.WS_CAPTION | WindowStyles.WS_THICKFRAME));
                SetExStyles(windowExStyles & ~(WindowExStyles.WS_EX_DLGMODALFRAME 
                                               | WindowExStyles.WS_EX_WINDOWEDGE
                                               | WindowExStyles.WS_EX_CLIENTEDGE
                                               | WindowExStyles.WS_EX_STATICEDGE));

                IntPtr monitor = User32Methods.MonitorFromWindow(Handle, MonitorFlag.MONITOR_DEFAULTTONEAREST);

                User32Helpers.GetMonitorInfo(monitor, out MonitorInfo monitorInfo);

                SetPosition(monitorInfo.MonitorRect,
                            WindowPositionFlags.SWP_NOZORDER 
                            | WindowPositionFlags.SWP_NOACTIVATE
                            | WindowPositionFlags.SWP_FRAMECHANGED);
            }
            else
            {
                SetStyle(windowStyles);
                SetExStyles(windowExStyles);

                SetPosition(windowRect,
                            WindowPositionFlags.SWP_NOZORDER 
                            | WindowPositionFlags.SWP_NOACTIVATE
                            | WindowPositionFlags.SWP_FRAMECHANGED);

                if (isMaximized)
                {
                    SetState(ShowWindowCommands.SW_MAXIMIZE);
                }
            }

            isFullScreen = !isFullScreen;
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                textBrush?.Dispose();
                textFormat?.Dispose();
                bitmap?.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Fullscreen

        private bool isFullScreen;
        private bool isMaximized;
        private WindowStyles windowStyles;
        private WindowExStyles windowExStyles;
        private Rectangle windowRect;

        #endregion
    }
}