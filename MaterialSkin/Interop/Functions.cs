using System.Collections.Concurrent;

namespace MaterialSkin.Interop;

internal static partial class Functions
{
    // https://learn.microsoft.com/windows/win32/api/wingdi/nf-wingdi-createroundrectrgn
    [LibraryImport("GDI32")]
    [PreserveSig]
    public static partial nint CreateRoundRectRgn(int x1, int y1, int x2, int y2, int w, int h);

    // https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-drawtextw
    [LibraryImport("USER32", StringMarshalling = StringMarshalling.Utf16)]
    [PreserveSig]
    public static partial int DrawTextW(nint hdc, [MarshalUsing(CountElementName = nameof(cchText))] PWSTR lpchText, int cchText, ref RECT lprc, DRAW_TEXT_FORMAT format);

    // https://learn.microsoft.com/windows/win32/api/libloaderapi/nf-libloaderapi-getmodulehandlew
    [LibraryImport("KERNEL32", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    [PreserveSig]
    public static partial nint GetModuleHandleW(PWSTR lpModuleName);

    // https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-loadimagew
    [LibraryImport("USER32", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    [PreserveSig]
    public static partial nint LoadImageW(nint hInst, PWSTR name, GDI_IMAGE_TYPE type, int cx, int cy, IMAGE_FLAGS fuLoad);

    // https://learn.microsoft.com/windows/win32/api/wingdi/nf-wingdi-setbkmode
    [LibraryImport("GDI32")]
    [PreserveSig]
    public static partial int SetBkMode(nint hdc, int mode);

    // https://learn.microsoft.com/windows/win32/api/wingdi/nf-wingdi-selectobject
    [LibraryImport("GDI32")]
    [PreserveSig]
    public static partial nint SelectObject(nint hdc, nint h);

    // https://learn.microsoft.com/windows/win32/api/wingdi/nf-wingdi-settextcolor
    [LibraryImport("GDI32")]
    [PreserveSig]
    public static partial int SetTextColor(nint hdc, int color);

    // https://learn.microsoft.com/windows/win32/api/wingdi/nf-wingdi-gettextextentpoint32w
    [LibraryImport("GDI32", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    [PreserveSig]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetTextExtentPoint32W(nint hdc, [MarshalUsing(CountElementName = nameof(c))] PWSTR lpString, int c, out SIZE psizl);

    // https://learn.microsoft.com/windows/win32/api/wingdi/nf-wingdi-textoutw
    [LibraryImport("GDI32", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    [PreserveSig]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool TextOutW(nint hdc, int x, int y, [MarshalUsing(CountElementName = nameof(c))] PWSTR lpString, int c);

    // https://learn.microsoft.com/windows/win32/api/wingdi/nf-wingdi-settextalign
    [LibraryImport("GDI32")]
    [PreserveSig]
    public static partial uint SetTextAlign(nint hdc, TEXT_ALIGN_OPTIONS align);

    // https://learn.microsoft.com/windows/win32/api/wingdi/nf-wingdi-selectcliprgn
    [LibraryImport("GDI32")]
    [SupportedOSPlatform("windows5.0")]
    [PreserveSig]
    public static partial GDI_REGION_TYPE SelectClipRgn(nint hdc, nint hrgn);

    // https://learn.microsoft.com/windows/win32/api/wingdi/nf-wingdi-deleteobject
    [LibraryImport("GDI32", SetLastError = true)]
    [PreserveSig]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool DeleteObject(nint ho);

    // https://learn.microsoft.com/windows/win32/api/wingdi/nf-wingdi-deletedc
    [LibraryImport("GDI32", SetLastError = true)]
    [PreserveSig]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool DeleteDC(nint hdc);

    // https://learn.microsoft.com/windows/win32/api/wingdi/nf-wingdi-createfontindirectw
    [LibraryImport("GDI32", StringMarshalling = StringMarshalling.Utf16)]
    [PreserveSig]
    public static partial nint CreateFontIndirectW(in LOGFONTW lplf);

    // https://learn.microsoft.com/windows/win32/api/wingdi/nf-wingdi-addfontmemresourceex
    [LibraryImport("GDI32")]
    [PreserveSig]
    public static partial nint AddFontMemResourceEx(nint pFileView, int cjSize, nint pvResrved, in uint pNumFonts);

    // https://learn.microsoft.com/windows/win32/api/wingdi/nf-wingdi-bitblt
    [LibraryImport("GDI32", SetLastError = true)]
    [PreserveSig]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool BitBlt(nint hdc, int x, int y, int cx, int cy, nint hdcSrc, int x1, int y1, ROP_CODE rop);

    // https://learn.microsoft.com/windows/win32/api/wingdi/nf-wingdi-alphablend
    [LibraryImport("MSIMG32", SetLastError = true)]
    [PreserveSig]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool AlphaBlend(nint hdcDest, int xoriginDest, int yoriginDest, int wDest, int hDest, nint hdcSrc, int xoriginSrc, int yoriginSrc, int wSrc, int hSrc, BLENDFUNCTION ftn);

    // https://learn.microsoft.com/windows/win32/api/wingdi/nf-wingdi-createcompatibledc
    [LibraryImport("GDI32")]
    [PreserveSig]
    public static partial nint CreateCompatibleDC(nint hdc);

    // https://learn.microsoft.com/windows/win32/api/wingdi/nf-wingdi-createdibsection
    [LibraryImport("GDI32", SetLastError = true)]
    [PreserveSig]
    public static partial nint CreateDIBSection(nint hdc, in BITMAPINFO pbmi, DIB_USAGE usage, out nint ppvBits, nint hSection, uint offset);

    // https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-getwindowlongptrw
    [LibraryImport("USER32", SetLastError = true)]
    [PreserveSig]
    public static partial nint GetWindowLongPtrW(nint hWnd, WINDOW_LONG_PTR_INDEX nIndex);

    // https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-getwindowlongw
    [LibraryImport("USER32", SetLastError = true)]
    [PreserveSig]
    public static partial int GetWindowLongW(nint hWnd, WINDOW_LONG_PTR_INDEX nIndex);

    // https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-setwindowlongptrw
    [LibraryImport("USER32", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    [PreserveSig]
    public static partial nint SetWindowLongPtrW(nint hWnd, WINDOW_LONG_PTR_INDEX nIndex, nint dwNewLong);

    // https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-setwindowlongw
    [LibraryImport("USER32", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    [PreserveSig]
    public static partial int SetWindowLongW(nint hWnd, WINDOW_LONG_PTR_INDEX nIndex, int dwNewLong);

    // https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-sendmessagew
    [LibraryImport("USER32", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    [PreserveSig]
    public static partial nint SendMessageW(nint hWnd, int Msg, nint wParam, nint lParam);

    // https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-releasecapture
    [LibraryImport("USER32", SetLastError = true)]
    [PreserveSig]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool ReleaseCapture();

    // https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-trackpopupmenuex
    [LibraryImport("USER32", SetLastError = true)]
    [PreserveSig]
    public static partial int TrackPopupMenuEx(nint hMenu, uint uFlags, int x, int y, nint hwnd, nint lptpm);

    // https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-getsystemmenu
    [LibraryImport("USER32")]
    [PreserveSig]
    public static partial nint GetSystemMenu(nint hWnd, [MarshalAs(UnmanagedType.Bool)] bool bRevert);

    // https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-loadcursorw
    [LibraryImport("USER32", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    [PreserveSig]
    public static partial nint LoadCursorW(nint hInstance, PWSTR lpCursorName);

    // https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-setcursor
    [LibraryImport("USER32")]
    [PreserveSig]
    public static partial nint SetCursor(nint hCursor);

    // https://learn.microsoft.com/windows/win32/api/libloaderapi/nf-libloaderapi-loadlibraryexw
    [LibraryImport("KERNEL32", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    [PreserveSig]
    public static partial nint LoadLibraryExW(PWSTR lpLibFileName, nint hFile, LOAD_LIBRARY_FLAGS dwFlags);

    // https://learn.microsoft.com/windows/win32/api/libloaderapi/nf-libloaderapi-freelibrary
    [LibraryImport("KERNEL32", SetLastError = true)]
    [PreserveSig]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool FreeLibrary(nint hLibModule);

    // https://learn.microsoft.com/windows/win32/api/winnls/nf-winnls-getthreaduilanguage
    [LibraryImport("KERNEL32")]
    [PreserveSig]
    public static partial ushort GetThreadUILanguage();

    // https://learn.microsoft.com/windows/win32/api/winnls/nf-winnls-setthreaduilanguage
    [LibraryImport("KERNEL32", SetLastError = true)]
    [PreserveSig]
    public static partial ushort SetThreadUILanguage(ushort LangId);

    // https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-loadstringw
    [LibraryImport("USER32", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    [PreserveSig]
    public static partial int LoadStringW(nint hInstance, uint uID, PWSTR lpBuffer, int cchBufferMax);

    private static readonly ConcurrentDictionary<string, string?> _loadedStrings = new(StringComparer.OrdinalIgnoreCase);
    public unsafe static string? LoadDllString(string libPath, uint id, int lcid = -1)
    {
        ArgumentNullException.ThrowIfNull(libPath);
        if (lcid == -1) // default => current UI culture
        {
            lcid = Thread.CurrentThread.CurrentUICulture.LCID;
        }

        var key = lcid + "!" + id + "!" + libPath;
        if (_loadedStrings.TryGetValue(key, out var str))
            return str;

        var h = LoadLibraryExW(PWSTR.From(libPath), 0, LOAD_LIBRARY_FLAGS.LOAD_LIBRARY_AS_IMAGE_RESOURCE);
        if (h == 0)
        {
            _loadedStrings[key] = null;
            return null;
        }

        var oldLcid = GetThreadUILanguage();
        SetThreadUILanguage((ushort)lcid);
        try
        {
            long ptr = 0;
            var ret = LoadStringW(h, id, new PWSTR((nint)(&ptr)), 0);
            if (ret == 0)
            {
                _loadedStrings[key] = null;
                return null;
            }

            var pwstr = new PWSTR((nint)ptr);
            str = pwstr.ToString(ret) ?? key;
            _loadedStrings[key] = str;
            return str;
        }
        finally
        {
            SetThreadUILanguage(oldLcid);
            FreeLibrary(h);
        }
    }
}
