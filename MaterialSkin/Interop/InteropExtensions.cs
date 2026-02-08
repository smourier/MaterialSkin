namespace MaterialSkin.Interop;

internal static class InteropExtensions
{
    public static void CopyFrom<T>(string? str, Span<char> chars, int length)
    {
        for (var i = 0; i < length; i++)
        {
            if (str != null && i < str.Length)
            {
                chars[i] = str[i];
            }
            else
            {
                chars[i] = '\0';
            }
        }
    }
}
