namespace MaterialSkin;

[Flags]
public enum TextAlignFlags : uint
{
    Left = 1 << 0,
    Center = 1 << 1,
    Right = 1 << 2,
    Top = 1 << 3,
    Middle = 1 << 4,
    Bottom = 1 << 5
}
