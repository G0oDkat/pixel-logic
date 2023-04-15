namespace GOoDkat.PixelLogic.Windows;

using WinApi.User32;
using WinApi.Windows;

public class FrameWindowNoRedirectionConstructionParams : ConstructionParams
{
    public override WindowStyles Styles => WindowStyles.WS_OVERLAPPEDWINDOW | WindowStyles.WS_CLIPCHILDREN | WindowStyles.WS_CLIPSIBLINGS;

    public override WindowExStyles ExStyles => WindowExStyles.WS_EX_APPWINDOW | WindowExStyles.WS_EX_NOREDIRECTIONBITMAP;
}