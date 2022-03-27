namespace GOoDkat.PixelLogic.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using Interop.Shell32;
    using Miscellaneous;
    using Miscellaneous.Input;
    using Models;
    using SharpDX;
    using SharpDX.Direct2D1;
    using SharpDX.DirectWrite;
    using SharpDX.DXGI;
    using SharpDX.Mathematics.Interop;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Formats;
    using SixLabors.ImageSharp.Formats.Bmp;
    using SixLabors.ImageSharp.Formats.Gif;
    using SixLabors.ImageSharp.Formats.Png;
    using SixLabors.ImageSharp.Formats.Tga;
    using SixLabors.ImageSharp.PixelFormats;
    using WinApi.DxUtils;
    using WinApi.DxUtils.Component;
    using WinApi.User32;
    using WinApi.Windows;
    using AlphaMode = SharpDX.Direct2D1.AlphaMode;
    using BitmapInterpolationMode = SharpDX.Direct2D1.BitmapInterpolationMode;
    using Image = Miscellaneous.Image;
    using ImageSharp = SixLabors.ImageSharp.Image;
    using PixelFormat = SharpDX.Direct2D1.PixelFormat;
    using Point = NetCoreEx.Geometry.Point;
    using Rectangle = NetCoreEx.Geometry.Rectangle;
    using Size = NetCoreEx.Geometry.Size;

    internal partial class MainWindow : DxWindow
    {
        private readonly RawColor4 clearColor;

        //private readonly FileSystemWatcher fileSystemWatcher;
        private readonly RawMatrix3x2 identityMatrix;
        private readonly float maxZoom;
        private readonly float minZoom;

        private readonly PerformanceMonitor performanceMonitor;
        private readonly RawColor4 textColor;
        private readonly float zoomFactor;
        private Bitmap1 bitmap;

        private CircuitBoard circuitBoard;
        private int forcedWireX;
        private int forcedWireY;
        private int frames;
        private int framesPerTick;
        private bool update;

        private bool isFullScreen;
        private bool isMaximized;

        private bool isRightMouseButtonDown;

        private bool isWireForced;
        private Point lastMousePosition;

        private string performaceText;

        private SolidColorBrush textBrush;
        private TextFormat textFormat;

        private int ticksPerFrame;
        private RawMatrix3x2 transformationMatrix;
        private Vector2 translation;
        private WindowExStyles windowExStyles;
        private Rectangle windowRect;

        private WindowStyles windowStyles;

        private float zoom;
        private readonly Keyboard keyboard;

        public MainWindow()
        {
            keyboard = new Keyboard();
            keyboard.KeyDown += OnKeyboardKeyDown;

            clearColor = new RawColor4(0f, 0f, 0f, 1f);
            textColor = new RawColor4(1f, 1f, 1f, 1f);

            zoom = 1f;
            zoomFactor = 2f;
            minZoom = 1f;
            maxZoom = 64f;
            translation = Vector2.Zero;
            identityMatrix = Matrix3x2.Identity;

            ticksPerFrame = 1;
            framesPerTick = 1;

            performanceMonitor = new PerformanceMonitor(TimeSpan.FromMilliseconds(1000));
            performaceText = "0 FPS | 0 TPS";

            //fileSystemWatcher = new FileSystemWatcher {NotifyFilter = NotifyFilters.LastWrite};
            //fileSystemWatcher.Changed += OnFileSystemWatcherChanged;
        }
        
        //private void OnFileSystemWatcherChanged(object sender, FileSystemEventArgs e)
        //{
        //    Dispatcher.Current.Invoke(() =>
        //    {
        //        try
        //        {
        //            Image image = CreateImageFromFile(e.FullPath);
        //            LoadCircuitBoard(image);
        //        }
        //        catch (Exception exception)
        //        {
        //            Debug.WriteLine(exception);
        //        }
        //    });
        //}

        protected override void OnCreate(ref CreateWindowPacket packet)
        {
            base.OnCreate(ref packet);

            Shell32.DragAcceptFiles(Handle, true);

            textBrush = new SolidColorBrush(Dx.D2D.Context, textColor);
            textFormat = new TextFormat(Dx.TextFactory, "Consolas", 16);

            LoadDefaultCircuitBoard();
            performanceMonitor.Start();
        }

        private void LoadDefaultCircuitBoard()
        {
            Image image = CreateImageFromResource("GOoDkat.PixelLogic.Samples.Tutorial.png");

            LoadCircuitBoard(image);
        }

        private void LoadCircuitBoard(Image image)
        {
            circuitBoard = CircuitBoard.FromImage(image);
            update = true;
            bitmap?.Dispose();
            bitmap = new Bitmap1(Dx.D2D.Context, new Size2(image.Width, image.Height),
                                 new BitmapProperties1(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Ignore)));

            ResetTransformation();
        }

        private void SaveCircuitBoardWithDialog()
        {
            try
            {
                var options = new SaveFileDialogOptions
                {
                    FileName = "CircuitBoard.png",
                    DefaultExtension = "png",
                    Filters =
                    {
                        new KeyValuePair<string, string>("Portable Network Graphics (*.png)", "*.png"),
                        new KeyValuePair<string, string>("Windows Bitmap (*.bmp)", "*.bmp"),
                        new KeyValuePair<string, string>("Targa Image File (*.tga)", "*.tga"),
                        new KeyValuePair<string, string>("All Files (*.*)", "*.*")
                    }
                };

                if (Dialogs.OpenSaveFileDialog(options, out string path))
                {
                    SaveCircuitBoard(path);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        private void SaveCircuitBoard(string path)
        {
            string extension = Path.GetExtension(path).ToLowerInvariant();

            IImageEncoder encoder;

            switch (extension)
            {
                case ".png":
                    encoder = new PngEncoder
                    {
                        ColorType = PngColorType.RgbWithAlpha,
                    };
                    break;
                case ".bmp":
                    encoder = new BmpEncoder
                    {
                        BitsPerPixel = BmpBitsPerPixel.Pixel32,
                        SupportTransparency = true
                    };
                    break;
                case ".tga":
                    encoder = new TgaEncoder
                    {
                        BitsPerPixel = TgaBitsPerPixel.Pixel32
                    };
                    break;
                default:
                    throw new NotSupportedException($"No image encoder available for file extension \"{extension}\".");
            }

            using (Image<Bgra32> image = circuitBoard.ToImage().ToImageSharp())
            {
                image.Save(path, encoder);
            }
        }
        private void CopyCircuitBoard()
        {
            try
            {
                using (Image<Bgra32> image = circuitBoard.ToImage().ToImageSharp())
                {
                    Clipboard.SetImage(image);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        private void PasteCircuitBoard()
        {
            try
            {
                if (Clipboard.TryGetImage<Bgra32>(out Image<Bgra32> image))
                {
                    using (image)
                    {
                        //fileSystemWatcher.EnableRaisingEvents = false;

                        LoadCircuitBoard(Image.FromImageSharp(image));
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        private Image CreateImageFromResource(string resource)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
            {
                return CreateImageFromStream(stream);
            }
        }

        private Image CreateImageFromFile(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                return CreateImageFromStream(stream);
            }
        }

        private Image CreateImageFromStream(Stream stream)
        {
            using (Image<Bgra32> image = ImageSharp.Load<Bgra32>(stream))
            {
                return Image.FromImageSharp(image);
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

            if (frames % framesPerTick == 0)
            {
                for (int i = 0; i < ticksPerFrame; i++)
                {
                    update |= circuitBoard.Update();
                    performanceMonitor.IncrementTicks();
                }

                if (update)
                {
                    circuitBoard.Image.CopyToBitmap(bitmap);
                    update = false;
                }

                frames = 0;
            }

            frames++;

            context.BeginDraw();
            context.Clear(clearColor);

            context.Transform = transformationMatrix;

            context.DrawBitmap(bitmap, CreateImageRect(bitmap.PixelSize, clientSize), 1f,
                               BitmapInterpolationMode.NearestNeighbor);

            context.Transform = identityMatrix;

            if (performanceMonitor.TryCalculate())
                performaceText =
                    $"{performanceMonitor.FramesPerSecond:F0} FPS | {performanceMonitor.TicksPerSecond:F0} TPS";

            context.DrawText(performaceText, textFormat, new RawRectangleF(0f, 0f, clientSize.Width, clientSize.Height),
                             textBrush);
            context.EndDraw();
            performanceMonitor.IncermentFrames();
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

        private void IncreaseTicks()
        {
            if (framesPerTick > 1)
                framesPerTick /= 2;
            else
                ticksPerFrame *= 2;
        }

        private void DecreaseTicks()
        {
            if (ticksPerFrame > 1)
                ticksPerFrame /= 2;
            else
                framesPerTick *= 2;
        }

        private void ResetTransformation()
        {
            Size clientSize = GetClientSize();
            Size2 bitmapSize = bitmap.PixelSize;

            float widthRatio = clientSize.Width / (float) bitmapSize.Width;
            float heightRatio = clientSize.Height / (float) bitmapSize.Height;

            float minRatio = Math.Min(widthRatio, heightRatio);

            zoom = minRatio < 1f
                       ? 1f
                       : (float)Math.Pow(zoomFactor, Math.Floor(Math.Log(minRatio) / Math.Log(zoomFactor)));

            translation = new Vector2(clientSize.Width / (zoom * 2f), clientSize.Height / (zoom * 2f));

            UpdateTransformation();
        }

        private void UpdateTransformation()
        {
            transformationMatrix = Matrix3x2.Translation(translation) * Matrix3x2.Scaling(zoom);
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
                SetExStyles(windowExStyles & ~(WindowExStyles.WS_EX_DLGMODALFRAME | WindowExStyles.WS_EX_WINDOWEDGE
                                                                                  | WindowExStyles.WS_EX_CLIENTEDGE
                                                                                  | WindowExStyles.WS_EX_STATICEDGE));

                IntPtr monitor = User32Methods.MonitorFromWindow(Handle, MonitorFlag.MONITOR_DEFAULTTONEAREST);

                User32Helpers.GetMonitorInfo(monitor, out MonitorInfo monitorInfo);

                SetPosition(monitorInfo.MonitorRect,
                            WindowPositionFlags.SWP_NOZORDER | WindowPositionFlags.SWP_NOACTIVATE
                                                             | WindowPositionFlags.SWP_FRAMECHANGED);
            }
            else
            {
                SetStyle(windowStyles);
                SetExStyles(windowExStyles);

                SetPosition(windowRect,
                            WindowPositionFlags.SWP_NOZORDER | WindowPositionFlags.SWP_NOACTIVATE
                                                             | WindowPositionFlags.SWP_FRAMECHANGED);

                if (isMaximized)
                    SetState(ShowWindowCommands.SW_MAXIMIZE);
            }

            isFullScreen = !isFullScreen;
            ResetTransformation();
        }



        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                textBrush?.Dispose();
                textFormat?.Dispose();
                bitmap?.Dispose();
                //fileSystemWatcher?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}