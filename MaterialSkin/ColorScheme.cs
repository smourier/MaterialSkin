namespace MaterialSkin;

public class ColorScheme
{
    public readonly Color PrimaryColor, DarkPrimaryColor, LightPrimaryColor, AccentColor, TextColor;
    public readonly Pen PrimaryPen, DarkPrimaryPen, LightPrimaryPen, AccentPen, TextPen;
    public readonly Brush PrimaryBrush, DarkPrimaryBrush, LightPrimaryBrush, AccentBrush, TextBrush;

    public ColorScheme()
        : this(Primary.Indigo500, Primary.Indigo700, Primary.Indigo100, Accent.Pink200, TextShade.WHITE)
    {
    }

    public ColorScheme(Primary primary, Primary darkPrimary, Primary lightPrimary, Accent accent, TextShade textShade)
    {
        //Color
        PrimaryColor = ((int)primary).ToColor();
        DarkPrimaryColor = ((int)darkPrimary).ToColor();
        LightPrimaryColor = ((int)lightPrimary).ToColor();
        AccentColor = ((int)accent).ToColor();
        TextColor = ((int)textShade).ToColor();

        //Pen
        PrimaryPen = new Pen(PrimaryColor);
        DarkPrimaryPen = new Pen(DarkPrimaryColor);
        LightPrimaryPen = new Pen(LightPrimaryColor);
        AccentPen = new Pen(AccentColor);
        TextPen = new Pen(TextColor);

        //Brush
        PrimaryBrush = new SolidBrush(PrimaryColor);
        DarkPrimaryBrush = new SolidBrush(DarkPrimaryColor);
        LightPrimaryBrush = new SolidBrush(LightPrimaryColor);
        AccentBrush = new SolidBrush(AccentColor);
        TextBrush = new SolidBrush(TextColor);
    }

    public ColorScheme(int primary, int darkPrimary, int lightPrimary, int accent, TextShade textShade)
    {
        //Color
        PrimaryColor = ((int)primary).ToColor();
        DarkPrimaryColor = ((int)darkPrimary).ToColor();
        LightPrimaryColor = ((int)lightPrimary).ToColor();
        AccentColor = ((int)accent).ToColor();
        TextColor = ((int)textShade).ToColor();

        //Pen
        PrimaryPen = new Pen(PrimaryColor);
        DarkPrimaryPen = new Pen(DarkPrimaryColor);
        LightPrimaryPen = new Pen(LightPrimaryColor);
        AccentPen = new Pen(AccentColor);
        TextPen = new Pen(TextColor);

        //Brush
        PrimaryBrush = new SolidBrush(PrimaryColor);
        DarkPrimaryBrush = new SolidBrush(DarkPrimaryColor);
        LightPrimaryBrush = new SolidBrush(LightPrimaryColor);
        AccentBrush = new SolidBrush(AccentColor);
        TextBrush = new SolidBrush(TextColor);
    }

    public ColorScheme(Color primary, Color darkPrimary, Color lightPrimary, Color accent, TextShade textShade)
    {
        //Color
        PrimaryColor = primary;
        DarkPrimaryColor = darkPrimary;
        LightPrimaryColor = lightPrimary;
        AccentColor = accent;
        TextColor = ((int)textShade).ToColor();

        //Pen
        PrimaryPen = new Pen(PrimaryColor);
        DarkPrimaryPen = new Pen(DarkPrimaryColor);
        LightPrimaryPen = new Pen(LightPrimaryColor);
        AccentPen = new Pen(AccentColor);
        TextPen = new Pen(TextColor);

        //Brush
        PrimaryBrush = new SolidBrush(PrimaryColor);
        DarkPrimaryBrush = new SolidBrush(DarkPrimaryColor);
        LightPrimaryBrush = new SolidBrush(LightPrimaryColor);
        AccentBrush = new SolidBrush(AccentColor);
        TextBrush = new SolidBrush(TextColor);
    }
}
