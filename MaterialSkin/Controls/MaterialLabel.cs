namespace MaterialSkin.Controls;

public class MaterialLabel : Label, IMaterialControl
{
    private TextAlignFlags _alignment;
    private ContentAlignment _textAlign = ContentAlignment.TopLeft;
    private FontType _fontType = FontType.Body1;

    [Browsable(false)]
    public int Depth { get; set; }

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
        get => _fontType;
        set
        {
            _fontType = value;
            Font = SkinManager.GetFontByType(_fontType);
            Refresh();
        }
    }

    public MaterialLabel()
    {
        FontType = FontType.Body1;
        TextAlign = ContentAlignment.TopLeft;
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
        if (AutoSize)
        {
            Size strSize;
            using (NativeTextRenderer NativeText = new(CreateGraphics()))
            {
                strSize = NativeText.MeasureLogString(Text, SkinManager.GetLogFontByType(_fontType));
                strSize.Width += 1; // necessary to avoid a bug when autosize = true
            }
            return strSize;
        }
        else
        {
            return proposedSize;
        }
    }

    private void UpdateAligment()
    {
        _alignment = _textAlign switch
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
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.Clear(Parent.BackColor);

        // Draw Text
        using NativeTextRenderer NativeText = new(g);
        NativeText.DrawMultilineTransparentText(
            Text,
            SkinManager.GetLogFontByType(_fontType),
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

    protected override void InitLayout()
    {
        Font = SkinManager.GetFontByType(_fontType);
    }
}
