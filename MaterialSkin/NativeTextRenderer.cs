namespace MaterialSkin;

#pragma warning disable CA1806 // Do not ignore method results
public sealed partial class NativeTextRenderer : IDisposable
{
    private static readonly int[] _charFit = new int[1];
    private static readonly int[] _charFitWidth = new int[1000];
    private static readonly Dictionary<string, Dictionary<float, Dictionary<FontStyle, nint>>> _fontsCache = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Graphics _g;
    private nint _hdc;

    public NativeTextRenderer(Graphics g)
    {
        _g = g;
        var clip = _g.Clip.GetHrgn(_g);

        _hdc = _g.GetHdc();
        SetBkMode(_hdc, 1);
        SelectClipRgn(_hdc, clip);
        DeleteObject(clip);
    }

    public Size MeasureString(string str, Font font)
    {
        SetFont(font);

        var size = new Size();
        if (string.IsNullOrEmpty(str)) return size;
        GetTextExtentPoint32(_hdc, str, str.Length, ref size);
        return size;
    }

    public Size MeasureLogString(string str, nint LogFont)
    {
        SelectObject(_hdc, LogFont);

        var size = new Size();
        if (string.IsNullOrEmpty(str)) return size;
        GetTextExtentPoint32(_hdc, str, str.Length, ref size);
        return size;
    }

    public Size MeasureString(string str, Font font, float maxWidth, out int charFit, out int charFitWidth)
    {
        SetFont(font);

        var size = new Size();
        GetTextExtentExPoint(_hdc, str, str.Length, (int)Math.Round(maxWidth), _charFit, _charFitWidth, ref size);
        charFit = _charFit[0];
        charFitWidth = charFit > 0 ? _charFitWidth[charFit - 1] : 0;
        return size;
    }

    public void DrawString(string str, Font font, Color color, Point point)
    {
        SetFont(font);
        SetTextColor(color);

        TextOut(_hdc, point.X, point.Y, str, str.Length);
    }

    public void DrawString(string str, Font font, Color color, Rectangle rect, TextFormatFlags flags)
    {
        SetFont(font);
        SetTextColor(color);

        var rect2 = new Rect(rect);
        DrawText(_hdc, str, str.Length, ref rect2, (uint)flags);
    }

    public void DrawTransparentText(string? str, Font font, Color color, Point point, Size size, TextAlignFlags flags) => DrawTransparentText(GetCachedHFont(font), str, color, point, size, flags, false);
    public void DrawTransparentText(string? str, nint LogFont, Color color, Point point, Size size, TextAlignFlags flags) => DrawTransparentText(LogFont, str, color, point, size, flags, false);
    public void DrawMultilineTransparentText(string? str, Font font, Color color, Point point, Size size, TextAlignFlags flags) => DrawTransparentText(GetCachedHFont(font), str, color, point, size, flags, true);
    public void DrawMultilineTransparentText(string? str, nint LogFont, Color color, Point point, Size size, TextAlignFlags flags) => DrawTransparentText(LogFont, str, color, point, size, flags, true);
    private void DrawTransparentText(nint fontHandle, string? str, Color color, Point point, Size size, TextAlignFlags flags, bool multilineSupport)
    {
        if (str == null)
            return;

        // Create a memory DC so we can work off-screen
        var memoryHdc = CreateCompatibleDC(_hdc);
        SetBkMode(memoryHdc, 1);

        // Create a device-independent bitmap and select it into our DC
        var info = new BitMapInfo();
        info.biSize = Marshal.SizeOf(info);
        info.biWidth = size.Width;
        info.biHeight = -size.Height;
        info.biPlanes = 1;
        info.biBitCount = 32;
        info.biCompression = 0; // BI_RGB
        var dib = CreateDIBSection(_hdc, ref info, 0, out _, 0, 0);
        SelectObject(memoryHdc, dib);

        try
        {
            // copy target background to memory HDC so when copied back it will have the proper background
            BitBlt(memoryHdc, 0, 0, size.Width, size.Height, _hdc, point.X, point.Y, 0x00CC0020);

            // Create and select font
            SelectObject(memoryHdc, fontHandle);
            SetTextColor(memoryHdc, (color.B & 0xFF) << 16 | (color.G & 0xFF) << 8 | color.R);

            var strSize = new Size();
            var pos = new Point();

            if (multilineSupport)
            {
                var fmtFlags = TextFormatFlags.WordBreak;

                // Aligment
                if (flags.HasFlag(TextAlignFlags.Center))
                {
                    fmtFlags |= TextFormatFlags.Center;
                }

                if (flags.HasFlag(TextAlignFlags.Right))
                {
                    fmtFlags |= TextFormatFlags.Right;
                }

                // Calculate the string size
                var strRect = new Rect(new Rectangle(point, size));
                DrawText(memoryHdc, str, str.Length, ref strRect, TextFormatFlags.CalcRect | fmtFlags);

                if (flags.HasFlag(TextAlignFlags.Middle))
                {
                    pos.Y = ((size.Height) >> 1) - (strRect.Height >> 1);
                }

                if (flags.HasFlag(TextAlignFlags.Bottom))
                {
                    pos.Y = size.Height - strRect.Height;
                }

                // Draw Text for multiline format
                var region = new Rect(new Rectangle(pos, size));
                DrawText(memoryHdc, str, -1, ref region, fmtFlags);
            }
            else
            {
                // Calculate the string size
                GetTextExtentPoint32(memoryHdc, str, str.Length, ref strSize);

                // Aligment
                if (flags.HasFlag(TextAlignFlags.Center))
                {
                    pos.X = ((size.Width) >> 1) - (strSize.Width >> 1);
                }
                if (flags.HasFlag(TextAlignFlags.Right))
                {
                    pos.X = size.Width - strSize.Width;
                }

                if (flags.HasFlag(TextAlignFlags.Middle))
                {
                    pos.Y = ((size.Height) >> 1) - (strSize.Height >> 1);
                }

                if (flags.HasFlag(TextAlignFlags.Bottom))
                {
                    pos.Y = size.Height - strSize.Height;
                }

                // Draw text to memory HDC
                TextOut(memoryHdc, pos.X, pos.Y, str, str.Length);
            }

            // copy from memory HDC to normal HDC with alpha blend so achieve the transparent text
            AlphaBlend(_hdc, point.X, point.Y, size.Width, size.Height, memoryHdc, 0, 0, size.Width, size.Height, new BlendFunction(color.A));
        }
        finally
        {
            DeleteObject(dib);
            DeleteDC(memoryHdc);
        }
    }

    public void Dispose()
    {
        if (_hdc != 0)
        {
            SelectClipRgn(_hdc, 0);
            _g.ReleaseHdc(_hdc);
            _hdc = 0;
        }
    }

    private void SetFont(Font font) => SelectObject(_hdc, GetCachedHFont(font));
    private static nint GetCachedHFont(Font font)
    {
        nint hfont = 0;
        if (_fontsCache.TryGetValue(font.Name, out var dic1))
        {
            if (dic1.TryGetValue(font.Size, out var dic2))
            {
                dic2.TryGetValue(font.Style, out hfont);
            }
            else
            {
                dic1[font.Size] = [];
            }
        }
        else
        {
            _fontsCache[font.Name] = new Dictionary<float, Dictionary<FontStyle, nint>>
            {
                [font.Size] = []
            };
        }

        if (hfont == 0)
        {
            _fontsCache[font.Name][font.Size][font.Style] = hfont = font.ToHfont();
        }

        return hfont;
    }

    private void SetTextColor(Color color)
    {
        var rgb = (color.B & 0xFF) << 16 | (color.G & 0xFF) << 8 | color.R;
        SetTextColor(_hdc, rgb);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int DrawText(nint hdc, string lpchText, int cchText, ref Rect lprc, TextFormatFlags dwDTFormat);

    [DllImport("gdi32.dll")]
    private static extern int SetBkMode(nint hdc, int mode);

    [DllImport("gdi32.dll")]
    private static extern nint SelectObject(nint hdc, nint hgdiObj);

    [DllImport("gdi32.dll")]
    private static extern int SetTextColor(nint hdc, int color);

    [DllImport("gdi32.dll", EntryPoint = "GetTextExtentPoint32W")]
    private static extern int GetTextExtentPoint32(nint hdc, [MarshalAs(UnmanagedType.LPWStr)] string str, int len, ref Size size);

    [DllImport("gdi32.dll", EntryPoint = "GetTextExtentExPointW")]
    private static extern bool GetTextExtentExPoint(nint hDc, [MarshalAs(UnmanagedType.LPWStr)] string str, int nLength, int nMaxExtent, int[] lpnFit, int[] alpDx, ref Size size);

    [DllImport("gdi32.dll", EntryPoint = "TextOutW")]
    private static extern bool TextOut(nint hdc, int x, int y, [MarshalAs(UnmanagedType.LPWStr)] string str, int len);

    [DllImport("gdi32.dll")]
    public static extern int SetTextAlign(nint hdc, uint fMode);

    [DllImport("user32.dll", EntryPoint = "DrawTextW")]
    private static extern int DrawText(nint hdc, [MarshalAs(UnmanagedType.LPWStr)] string str, int len, ref Rect rect, uint uFormat);

    [DllImport("gdi32.dll")]
    private static extern int SelectClipRgn(nint hdc, nint hrgn);

    [DllImport("gdi32.dll")]
    public static extern bool DeleteObject(nint hObject);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern bool DeleteDC(nint hdc);

    [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
    internal static extern nint CreateFontIndirect([In, MarshalAs(UnmanagedType.LPStruct)] LogFont lplf);

    [DllImport("gdi32.dll", ExactSpelling = true)]
    public static extern nint AddFontMemResourceEx(byte[] pbFont, int cbFont, nint pdv, out uint pcFonts);

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool BitBlt(nint hdc, int nXDest, int nYDest, int nWidth, int nHeight, nint hdcSrc, int nXSrc, int nYSrc, uint dwRop);

    [DllImport("gdi32.dll", EntryPoint = "GdiAlphaBlend")]
    private static extern bool AlphaBlend(nint hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, nint hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, BlendFunction blendFunction);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    private static extern nint CreateCompatibleDC(nint hdc);

    [DllImport("gdi32.dll")]
    private static extern nint CreateDIBSection(nint hdc, [In] ref BitMapInfo pbmi, uint iUsage, out nint ppvBits, nint hSection, uint dwOffset);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal class LogFont
    {
        public int lfHeight = 0;
        public int lfWidth = 0;
        public int lfEscapement = 0;
        public int lfOrientation = 0;
        public int lfWeight = 0;
        public byte lfItalic = 0;
        public byte lfUnderline = 0;
        public byte lfStrikeOut = 0;
        public byte lfCharSet = 0;
        public byte lfOutPrecision = 0;
        public byte lfClipPrecision = 0;
        public byte lfQuality = 0;
        public byte lfPitchAndFamily = 0;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string lfFaceName = string.Empty;
    }

    private readonly struct Rect(Rectangle r)
    {
        private readonly int _left = r.Left;
        private readonly int _top = r.Top;
        private readonly int _right = r.Right;
        private readonly int _bottom = r.Bottom;

        public int Height => _bottom - _top;
    }

    private struct BlendFunction(byte alpha)
    {
        public byte BlendOp = 0;
        public byte BlendFlags = 0;
        public byte SourceConstantAlpha = alpha;
        public byte AlphaFormat = 0;
    }

    private struct BitMapInfo
    {
        public int biSize;
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public int biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;
        public byte bmiColors_rgbBlue;
        public byte bmiColors_rgbGreen;
        public byte bmiColors_rgbRed;
        public byte bmiColors_rgbReserved;
    }
}
#pragma warning restore CA1806 // Do not ignore method results
