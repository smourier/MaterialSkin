namespace MaterialSkin;

public static class MaterialSkinExtensions
{
    public static Color ToColor(this int argb) => Color.FromArgb((argb & 0xFF0000) >> 16, (argb & 0x00FF00) >> 8, argb & 0x0000FF);
    public static Color RemoveAlpha(this Color color) => Color.FromArgb(color.R, color.G, color.B);

    public static Icon? LoadApplicationIcon(int size)
    {
        var path = Path.GetFileName(Process.GetCurrentProcess().MainModule?.FileName);
        if (path == null)
            return null;

        var exeHandle = Functions.GetModuleHandleW(PWSTR.From(path));
        return Icon.FromHandle(Functions.LoadImageW(exeHandle, Constants.IDI_APPLICATION, GDI_IMAGE_TYPE.IMAGE_ICON, size, size, 0));
    }

    public static byte[]? GetBytesFromResource(string resourceName, bool throwOnError = true) => GetBytesFromResource(null, resourceName, throwOnError);
    public static byte[]? GetBytesFromResource(this Assembly? assembly, string resourceName, bool throwOnError = true)
    {
        ArgumentNullException.ThrowIfNull(resourceName, nameof(resourceName));
        assembly ??= Assembly.GetEntryAssembly(); // tailored for AOT publishing where the entry assembly is the one containing the resources, not necessarily the one containing this code
        using var stream = assembly?.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.{resourceName}");
        if (stream == null)
        {
            if (throwOnError)
                throw new InvalidOperationException($"Resource '{resourceName}' not found in assembly '{assembly?.GetName().Name}'.");

            return null;
        }

        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }

    public static Bitmap? GetBitmapFromResource(string resourceName, bool throwOnError = true) => GetBitmapFromResource(null, resourceName, throwOnError);
    public static Bitmap? GetBitmapFromResource(this Assembly? assembly, string resourceName, bool throwOnError = true)
    {
        ArgumentNullException.ThrowIfNull(resourceName, nameof(resourceName));
        assembly ??= Assembly.GetEntryAssembly(); // tailored for AOT publishing where the entry assembly is the one containing the resources, not necessarily the one containing this code
        using var stream = assembly?.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.{resourceName}");
        if (stream == null)
        {
            if (throwOnError)
                throw new InvalidOperationException($"Resource '{resourceName}' not found in assembly '{assembly?.GetName().Name}'.");

            return null;
        }

        return new Bitmap(stream);
    }
}