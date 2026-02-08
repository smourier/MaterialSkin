namespace MaterialSkin;

public static class ColorHelper
{
    /// <summary>
    /// Tints the color by the given percent.
    /// </summary>
    /// <param name="color">The color being tinted.</param>
    /// <param name="percent">The percent to tint. Ex: 0.1 will make the color 10% lighter.</param>
    /// <returns>The new tinted color.</returns>
    public static Color Lighten(this Color color, float percent)
    {
        var lighting = color.GetBrightness();
        lighting += lighting * percent;
        if (lighting > 1.0)
        {
            lighting = 1;
        }
        else if (lighting <= 0)
        {
            lighting = 0.1f;
        }

        var tintedColor = FromHsl(color.A, color.GetHue(), color.GetSaturation(), lighting);
        return tintedColor;
    }

    /// <summary>
    /// Tints the color by the given percent.
    /// </summary>
    /// <param name="color">The color being tinted.</param>
    /// <param name="percent">The percent to tint. Ex: 0.1 will make the color 10% darker.</param>
    /// <returns>The new tinted color.</returns>
    public static Color Darken(this Color color, float percent)
    {
        var lighting = color.GetBrightness();
        lighting -= lighting * percent;
        if (lighting > 1.0)
        {
            lighting = 1;
        }
        else if (lighting <= 0)
        {
            lighting = 0;
        }

        var tintedColor = FromHsl(color.A, color.GetHue(), color.GetSaturation(), lighting);
        return tintedColor;
    }

    /// <summary>
    /// Converts the HSL values to a Color.
    /// </summary>
    /// <param name="alpha">The alpha.</param>
    /// <param name="hue">The hue.</param>
    /// <param name="saturation">The saturation.</param>
    /// <param name="lighting">The lighting.</param>
    /// <returns></returns>
    public static Color FromHsl(int alpha, float hue, float saturation, float lighting)
    {
        if (0 > alpha || 255 < alpha)
            throw new ArgumentOutOfRangeException(nameof(alpha));

        if (0 > hue || 360 < hue)
            throw new ArgumentOutOfRangeException(nameof(hue));

        if (0 > saturation || 1 < saturation)
            throw new ArgumentOutOfRangeException(nameof(saturation));

        if (0 > lighting || 1 < lighting)
            throw new ArgumentOutOfRangeException(nameof(lighting));

        if (0 == saturation)
            return Color.FromArgb(alpha, (int)(lighting * 255), (int)(lighting * 255), (int)(lighting * 255));

        float fMax, fMid, fMin;
        int iSextant, iMax, iMid, iMin;

        if (0.5 < lighting)
        {
            fMax = lighting - (lighting * saturation) + saturation;
            fMin = lighting + (lighting * saturation) - saturation;
        }
        else
        {
            fMax = lighting + (lighting * saturation);
            fMin = lighting - (lighting * saturation);
        }

        iSextant = (int)Math.Floor(hue / 60f);
        if (300f <= hue)
        {
            hue -= 360f;
        }

        hue /= 60f;
        hue -= 2f * (float)Math.Floor((iSextant + 1f) % 6f / 2f);
        if (0 == iSextant % 2)
        {
            fMid = hue * (fMax - fMin) + fMin;
        }
        else
        {
            fMid = fMin - hue * (fMax - fMin);
        }

        iMax = (int)(fMax * 255);
        iMid = (int)(fMid * 255);
        iMin = (int)(fMin * 255);

        return iSextant switch
        {
            1 => Color.FromArgb(alpha, iMid, iMax, iMin),
            2 => Color.FromArgb(alpha, iMin, iMax, iMid),
            3 => Color.FromArgb(alpha, iMin, iMid, iMax),
            4 => Color.FromArgb(alpha, iMid, iMin, iMax),
            5 => Color.FromArgb(alpha, iMax, iMin, iMid),
            _ => Color.FromArgb(alpha, iMax, iMid, iMin),
        };
    }

    /// <summary>
    /// Removes alpha value without changing Color.
    /// </summary>
    /// <param name="foreground">The foreground color.</param>
    /// <param name="background">The background color.</param>
    /// <returns></returns>
    public static Color RemoveAlpha(Color foreground, Color background)
    {
        if (foreground.A == 255)
            return foreground;

        var alpha = foreground.A / 255.0;
        var diff = 1 - alpha;
        return Color.FromArgb(255,
            (byte)(foreground.R * alpha + background.R * diff),
            (byte)(foreground.G * alpha + background.G * diff),
            (byte)(foreground.B * alpha + background.B * diff));
    }

}
