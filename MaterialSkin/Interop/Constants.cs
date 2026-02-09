namespace MaterialSkin.Interop;

internal static class Constants
{
    public const int IDC_HAND = 32649;
    public const int SB_LINEUP = 0;
    public const int SB_LINEDOWN = 1;

    public const int SC_MOVE = 0xF010;

    public const int WM_SETCURSOR = 0x0020;
    public const int WM_VSCROLL = 0x0115;
    public const int WM_ENABLE = 0x0A;
    public const int WM_PAINT = 0xF;
    public const int WM_USER = 0x0400;
    public const int WM_KILLFOCUS = 0x0008;
    public const int WM_MOUSEWHEEL = 0x20A;
    public const int WM_SYSCOMMAND = 0x0112;
    public const int WM_SETREDRAW = 0xB;

    public const int EM_SETBKGNDCOLOR = WM_USER + 67;
    public const int EM_SETCUEBANNER = 0x1501;

    public static readonly PWSTR IDI_APPLICATION = new('\u7f00');
}
