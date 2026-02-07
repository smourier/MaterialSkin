namespace MaterialSkin;

[Flags]
public enum TextFormatFlags : uint
{
    Default = 0x00000000,
    Center = 0x00000001,
    Right = 0x00000002,
    VCenter = 0x00000004,
    Bottom = 0x00000008,
    WordBreak = 0x00000010,
    SingleLine = 0x00000020,
    ExpandTabs = 0x00000040,
    TabStop = 0x00000080,
    NoClip = 0x00000100,
    ExternalLeading = 0x00000200,
    CalcRect = 0x00000400,
    NoPrefix = 0x00000800,
    Internal = 0x00001000,
    EditControl = 0x00002000,
    PathEllipsis = 0x00004000,
    EndEllipsis = 0x00008000,
    ModifyString = 0x00010000,
    RtlReading = 0x00020000,
    WordEllipsis = 0x00040000,
    NoFullWidthCharBreak = 0x00080000,
    HidePrefix = 0x00100000,
    ProfixOnly = 0x00200000,
}
