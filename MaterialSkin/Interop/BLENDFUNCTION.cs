namespace MaterialSkin.Interop;

// https://learn.microsoft.com/windows/win32/api/wingdi/ns-wingdi-blendfunction
public partial struct BLENDFUNCTION
{
    public byte BlendOp;
    public byte BlendFlags;
    public byte SourceConstantAlpha;
    public byte AlphaFormat;
}
