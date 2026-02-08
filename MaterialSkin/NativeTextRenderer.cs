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
        Functions.SetBkMode(_hdc, 1);
        Functions.SelectClipRgn(_hdc, clip);
        Functions.DeleteObject(clip);
    }

    public Size MeasureString(string str, Font font)
    {
        SetFont(font);

        if (string.IsNullOrEmpty(str))
            return new();

        Functions.GetTextExtentPoint32W(_hdc, PWSTR.From(str), str.Length, out var size);
        return new Size(size.cx, size.cy);
    }

    public Size MeasureLogString(string str, nint LogFont)
    {
        Functions.SelectObject(_hdc, LogFont);

        if (string.IsNullOrEmpty(str))
            return new();

        Functions.GetTextExtentPoint32W(_hdc, PWSTR.From(str), str.Length, out var size);
        return new Size(size.cx, size.cy);
    }

    public void DrawString(string str, Font font, Color color, Point point)
    {
        SetFont(font);
        SetTextColor(color);
        Functions.TextOutW(_hdc, point.X, point.Y, PWSTR.From(str), str.Length);
    }

    public void DrawString(string str, Font font, Color color, Rectangle rect, TextFormatFlags flags)
    {
        SetFont(font);
        SetTextColor(color);
        var rc = new RECT
        {
            left = rect.Left,
            top = rect.Top,
            right = rect.Right,
            bottom = rect.Bottom
        };
        Functions.DrawTextW(_hdc, PWSTR.From(str), str.Length, ref rc, (DRAW_TEXT_FORMAT)flags);
    }

    public void DrawTransparentText(string? str, Font? font, Color color, Point point, Size size, TextAlignFlags flags) => DrawTransparentText(GetCachedHFont(font), str, color, point, size, flags, false);
    public void DrawTransparentText(string? str, nint LogFont, Color color, Point point, Size size, TextAlignFlags flags) => DrawTransparentText(LogFont, str, color, point, size, flags, false);
    public void DrawMultilineTransparentText(string? str, Font font, Color color, Point point, Size size, TextAlignFlags flags) => DrawTransparentText(GetCachedHFont(font), str, color, point, size, flags, true);
    public void DrawMultilineTransparentText(string? str, nint LogFont, Color color, Point point, Size size, TextAlignFlags flags) => DrawTransparentText(LogFont, str, color, point, size, flags, true);
    private void DrawTransparentText(nint fontHandle, string? str, Color color, Point point, Size size, TextAlignFlags flags, bool multilineSupport)
    {
        if (str == null)
            return;

        // Create a memory DC so we can work off-screen
        var memoryHdc = Functions.CreateCompatibleDC(_hdc);
        Functions.SetBkMode(memoryHdc, 1);

        // Create a device-independent bitmap and select it into our DC
        var info = new BITMAPINFO();
        unsafe
        {
            info.bmiHeader.biSize = (uint)sizeof(BITMAPINFO);
        }
        info.bmiHeader.biWidth = size.Width;
        info.bmiHeader.biHeight = -size.Height;
        info.bmiHeader.biPlanes = 1;
        info.bmiHeader.biBitCount = 32;
        info.bmiHeader.biCompression = 0; // BI_RGB
        var dib = Functions.CreateDIBSection(_hdc, info, 0, out _, 0, 0);
        Functions.SelectObject(memoryHdc, dib);

        try
        {
            // copy target background to memory HDC so when copied back it will have the proper background
            Functions.BitBlt(memoryHdc, 0, 0, size.Width, size.Height, _hdc, point.X, point.Y, ROP_CODE.SRCCOPY);

            // Create and select font
            Functions.SelectObject(memoryHdc, fontHandle);
            Functions.SetTextColor(memoryHdc, (color.B & 0xFF) << 16 | (color.G & 0xFF) << 8 | color.R);

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
                var strRect = new RECT
                {
                    left = 0,
                    top = 0,
                    right = size.Width,
                    bottom = size.Height
                };
                Functions.DrawTextW(memoryHdc, PWSTR.From(str), str.Length, ref strRect, (DRAW_TEXT_FORMAT)(TextFormatFlags.CalcRect | fmtFlags));
                var h = strRect.bottom - strRect.top;

                if (flags.HasFlag(TextAlignFlags.Middle))
                {
                    pos.Y = ((size.Height) >> 1) - (h >> 1);
                }

                if (flags.HasFlag(TextAlignFlags.Bottom))
                {
                    pos.Y = size.Height - h;
                }

                // Draw Text for multiline format
                var region = new RECT
                {
                    left = pos.X,
                    top = pos.Y,
                    right = pos.X + size.Width,
                    bottom = pos.Y + size.Height
                };
                Functions.DrawTextW(memoryHdc, PWSTR.From(str), -1, ref region, (DRAW_TEXT_FORMAT)fmtFlags);
            }
            else
            {
                // Calculate the string size
                Functions.GetTextExtentPoint32W(memoryHdc, PWSTR.From(str), str.Length, out var strSize2);
                strSize = new Size(strSize2.cx, strSize2.cy);

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
                Functions.TextOutW(memoryHdc, pos.X, pos.Y, PWSTR.From(str), str.Length);
            }

            // copy from memory HDC to normal HDC with alpha blend so achieve the transparent text
            var func = new BLENDFUNCTION
            {
                BlendOp = 0, // AC_SRC_OVER
                BlendFlags = 0,
                SourceConstantAlpha = color.A,
                AlphaFormat = 0 // AC_SRC_ALPHA
            };
            Functions.AlphaBlend(_hdc, point.X, point.Y, size.Width, size.Height, memoryHdc, 0, 0, size.Width, size.Height, func);
        }
        finally
        {
            Functions.DeleteObject(dib);
            Functions.DeleteDC(memoryHdc);
        }
    }

    public void Dispose()
    {
        if (_hdc != 0)
        {
            Functions.SelectClipRgn(_hdc, 0);
            _g.ReleaseHdc(_hdc);
            _hdc = 0;
        }
    }

    private void SetFont(Font font) => Functions.SelectObject(_hdc, GetCachedHFont(font));
    private static nint GetCachedHFont(Font? font)
    {
        nint hfont = 0;
        font ??= SystemFonts.DefaultFont;

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
        Functions.SetTextColor(_hdc, rgb);
    }
}
#pragma warning restore CA1806 // Do not ignore method results
