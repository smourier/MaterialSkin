namespace MaterialSkin.Interop;

// https://learn.microsoft.com/windows/win32/api/shtypes/ns-shtypes-logfontw
public partial struct LOGFONTW
{
    public int lfHeight;
    public int lfWidth;
    public int lfEscapement;
    public int lfOrientation;
    public int lfWeight;
    public byte lfItalic;
    public byte lfUnderline;
    public byte lfStrikeOut;
    public byte lfCharSet;
    public byte lfOutPrecision;
    public byte lfClipPrecision;
    public byte lfQuality;
    public byte lfPitchAndFamily;
    public InlineArraySystemChar_32 lfFaceName;
}
