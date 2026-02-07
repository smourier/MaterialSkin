namespace MaterialSkin;

internal static class Extensions
{
    public static Color ToColor(this int argb) => Color.FromArgb((argb & 0xFF0000) >> 16, (argb & 0x00FF00) >> 8, argb & 0x0000FF);
    public static Color RemoveAlpha(this Color color) => Color.FromArgb(color.R, color.G, color.B);
}