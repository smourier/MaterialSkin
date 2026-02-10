namespace MaterialSkin.Controls;

public class MaterialLabel : Label, IMaterialControl
{
    private TextAlignFlags _alignment;
    private ContentAlignment _textAlign = ContentAlignment.TopLeft;

    public MaterialLabel()
    {
        FontType = FontType.Body1;
        TextAlign = ContentAlignment.TopLeft;
    }

    [Browsable(false)]
    public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

    [Browsable(false)]
    public MouseState MouseState { get; set; }

    [DefaultValue(typeof(ContentAlignment), "TopLeft")]
    public override ContentAlignment TextAlign
    {
        get => _textAlign;
        set
        {
            _textAlign = value;
            UpdateAligment();
            Invalidate();
        }
    }

    [Category("Material Skin"),
    DefaultValue(false)]
    public bool HighEmphasis { get; set; }

    [Category("Material Skin"),
    DefaultValue(false)]
    public bool UseAccent { get; set; }

    [Category("Material Skin"),
    DefaultValue(typeof(FontType), "Body1")]
    public FontType FontType
    {
        get;
        set
        {
            field = value;
            Font = SkinManager.GetFontByType(field);
            Refresh();
        }
    } = FontType.Body1;

    public override Size GetPreferredSize(Size proposedSize)
    {
        if (AutoSize)
        {
            using var NativeText = new NativeTextRenderer(CreateGraphics());
            var strSize = NativeText.MeasureLogString(Text, SkinManager.GetLogFontByType(FontType));
            strSize.Width += 1; // necessary to avoid a bug when autosize = true
            return strSize;
        }
        return proposedSize;
    }

    private void UpdateAligment() => _alignment = _textAlign switch
    {
        ContentAlignment.TopLeft => TextAlignFlags.Top | TextAlignFlags.Left,
        ContentAlignment.TopCenter => TextAlignFlags.Top | TextAlignFlags.Center,
        ContentAlignment.TopRight => TextAlignFlags.Top | TextAlignFlags.Right,
        ContentAlignment.MiddleLeft => TextAlignFlags.Middle | TextAlignFlags.Left,
        ContentAlignment.MiddleCenter => TextAlignFlags.Middle | TextAlignFlags.Center,
        ContentAlignment.MiddleRight => TextAlignFlags.Middle | TextAlignFlags.Right,
        ContentAlignment.BottomLeft => TextAlignFlags.Bottom | TextAlignFlags.Left,
        ContentAlignment.BottomCenter => TextAlignFlags.Bottom | TextAlignFlags.Center,
        ContentAlignment.BottomRight => TextAlignFlags.Bottom | TextAlignFlags.Right,
        _ => TextAlignFlags.Top | TextAlignFlags.Left,
    };

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        if (Parent != null)
        {
            g.Clear(Parent.BackColor);
        }

        // Draw Text
        using var NativeText = new NativeTextRenderer(g);
        NativeText.DrawMultilineTransparentText(
            Text,
            SkinManager.GetLogFontByType(FontType),
            Enabled ? HighEmphasis ? UseAccent ?
            SkinManager.ColorScheme.AccentColor : // High emphasis, accent
            (SkinManager.Theme == Themes.LIGHT) ?
            SkinManager.ColorScheme.PrimaryColor : // High emphasis, primary Light theme
            SkinManager.ColorScheme.PrimaryColor.Lighten(0.25f) : // High emphasis, primary Dark theme
            SkinManager.TextHighEmphasisColor : // Normal
            SkinManager.TextDisabledOrHintColor, // Disabled
            ClientRectangle.Location,
            ClientRectangle.Size,
            _alignment);
    }

    protected override void InitLayout() => Font = SkinManager.GetFontByType(FontType);
}
