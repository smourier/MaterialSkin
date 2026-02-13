namespace MaterialSkin.Controls;

public class MaterialSlider : Control, IMaterialControl
{
    private const int _activeTrack = 6;
    private const int _inactiveTrack = 4;
    private const int _thumbRadius = 20;
    private const int _thumbRadiusHoverPressed = 40;

    private bool _mousePressed;
    private int _mouseX;
    private bool _hovered;
    private Rectangle _indicatorRectangle;
    private Rectangle _indicatorRectangleNormal;
    private Rectangle _indicatorRectanglePressed;
    private Rectangle _textRectangle;
    private Rectangle _valueRectangle;
    private Rectangle _sliderRectangle;
    private string? _text;

    [Category("Behavior")]
    [Description("Occurs when value change.")]
    public event EventHandler? OnValueChanged;

    public MaterialSlider()
    {
        SetStyle(ControlStyles.Selectable, true);
        ForeColor = MaterialSkinManager.Instance.TextHighEmphasisColor; // Color.Black;
        ValueMax = 100;
        Size = new Size(250, _thumbRadiusHoverPressed);
        Text = Functions.LoadDllString("shell32.dll", 4256) ?? "(None)";
        Value = 50;
        ShowText = true;
        ShowValue = true;
        UpdateRects();
        DoubleBuffered = true;
    }

    [Browsable(false)]
    public Func<double, int>? ValueUpdateFunc { get; set; } // Optional function to update value based on mouse X position (between 0 and 1), if not set, default linear mapping is used

    [DefaultValue(50)]
    [Category("Material Skin")]
    [Description("Define control value")]
    public int Value
    {
        get;
        set
        {
            value = Math.Max(ValueMin, Math.Min(ValueMax, value));
            if (field == value)
                return;

            field = value;
            OnValueChanged?.Invoke(this, EventArgs.Empty);
            _mouseX = _sliderRectangle.X + ((int)(field / (double)(ValueMax - ValueMin) * (_sliderRectangle.Width - _thumbRadius)));
            UpdateRects();
        }
    }

    [DefaultValue(100)]
    [Category("Material Skin")]
    [Description("Define control maximum value")]
    public int ValueMax
    {
        get;
        set
        {
            field = value;
            if (ValueMin >= ValueMax)
            {
                ValueMin = ValueMax - 1;
            }

#pragma warning disable CA2245 // Do not assign a property to itself
            Value = Value; // Ensure value is within new range
#pragma warning restore CA2245 // Do not assign a property to itself
            UpdateRects();
        }
    }

    [DefaultValue(0)]
    [Category("Material Skin")]
    [Description("Define control minimum value")]
    public int ValueMin
    {
        get;
        set
        {
            field = value;
            if (ValueMax <= ValueMin)
            {
                ValueMax = ValueMin + 1;
            }
#pragma warning disable CA2245 // Do not assign a property to itself
            Value = Value; // Ensure value is within new range
#pragma warning restore CA2245 // Do not assign a property to itself
            UpdateRects();
        }
    }

    [DefaultValue("MyData")]
    [Category("Material Skin")]
    [Description("Set control text")]
    [AllowNull]
    public override string Text { get => _text ?? string.Empty; set { _text = value; UpdateRects(); } }

    [DefaultValue("")]
    [Category("Material Skin")]
    [Description("Set control value format string")]
    public string? ValueFormat { get; set { field = value; UpdateRects(); } }

    [DefaultValue(true)]
    [Category("Material Skin"), DisplayName("Show text")]
    [Description("Show text")]
    public bool ShowText { get; set { field = value; UpdateRects(); } }

    [DefaultValue(true)]
    [Category("Material Skin"), DisplayName("Show value")]
    [Description("Show value")]
    public bool ShowValue { get; set { field = value; UpdateRects(); } }

    [Category("Material Skin"), DefaultValue(false), DisplayName("Use Accent Color")]
    public bool UseAccentColor { get; set { field = value; Invalidate(); } }

    [Category("Material Skin"),
    DefaultValue(typeof(FontType), "Body1")]
    public FontType FontType
    {
        get;
        set
        {
            field = value;
            Font = MaterialSkinManager.Instance.GetFontByType(field);
            Refresh();
        }
    } = FontType.Body1;

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        Height = _thumbRadiusHoverPressed;
        UpdateRects();
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        _hovered = true;
        Invalidate();
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
        _hovered = false;
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button == MouseButtons.Left && e.Y > _indicatorRectanglePressed.Top && e.Y < _indicatorRectanglePressed.Bottom)
        {
            _mousePressed = true;
            UpdateValue(e);
        }
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        _hovered = true;

        if (!Focused)
        {
            Focus();
        }

        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _hovered = false;

        if (Focused)
        {
            Parent?.Focus();
        }

        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        _mousePressed = false;
        Invalidate();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (_mousePressed)
        {
            UpdateValue(e);
        }
    }

    private void UpdateValue(MouseEventArgs e)
    {
        // Calculate normalized position (0 to 1)
        double normalizedPosition;
        if (e.X <= _sliderRectangle.X + (_thumbRadius / 2))
        {
            normalizedPosition = 0;
        }
        else if (e.X >= _sliderRectangle.Right - _thumbRadius / 2)
        {
            normalizedPosition = 1;
        }
        else
        {
            normalizedPosition = (e.X - _sliderRectangle.X - (_thumbRadius / 2)) / (double)(_sliderRectangle.Width - _thumbRadius);
        }

        // Use custom function if provided, otherwise use default linear mapping
        int v;
        if (ValueUpdateFunc != null)
        {
            v = ValueUpdateFunc(normalizedPosition);
        }
        else
        {
            v = (int)(normalizedPosition * (ValueMax - ValueMin) + ValueMin);
        }

        Value = v;
    }

    private void UpdateRects()
    {
        using var renderer = new NativeTextRenderer(CreateGraphics());
        var textSize = renderer.MeasureLogString(ShowText ? Text : string.Empty, MaterialSkinManager.Instance.GetLogFontByType(FontType));

        var format = ValueFormat ?? "{0}";
        var valueSize = renderer.MeasureLogString(ShowValue ? string.Format(format, ValueMax) : string.Empty, MaterialSkinManager.Instance.GetLogFontByType(FontType));

        // drawn as:
        // text | slider | value

        _valueRectangle = new Rectangle(Width - valueSize.Width - _thumbRadiusHoverPressed / 4, 0, valueSize.Width + _thumbRadiusHoverPressed / 4, Height);
        _textRectangle = new Rectangle(0, 0, textSize.Width + _thumbRadiusHoverPressed / 4, Height);
        _sliderRectangle = new Rectangle(_textRectangle.Right, 0, _valueRectangle.Left - _textRectangle.Right, _thumbRadius);

        // recalculate _mouseX based on the updated _sliderRectangle
        _mouseX = _sliderRectangle.X + (int)((Value - ValueMin) / (double)(ValueMax - ValueMin) * (_sliderRectangle.Width - _thumbRadius));

        _indicatorRectangle = new Rectangle(_mouseX, (Height - _thumbRadius) / 2, _thumbRadius, _thumbRadius);
        _indicatorRectangleNormal = new Rectangle(_indicatorRectangle.X, Height / 2 - _thumbRadius / 2, _thumbRadius, _thumbRadius);
        _indicatorRectanglePressed = new Rectangle(_indicatorRectangle.X + _thumbRadius / 2 - _thumbRadiusHoverPressed / 2, Height / 2 - _thumbRadiusHoverPressed / 2, _thumbRadiusHoverPressed, _thumbRadiusHoverPressed);
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.AntiAlias;

        if (Parent != null)
        {
            g.Clear(Parent.BackColor);
        }

        Color accentColor;
        if (UseAccentColor)
        {
            accentColor = MaterialSkinManager.Instance.ColorScheme.AccentColor;
        }
        else
        {
            accentColor = MaterialSkinManager.Instance.ColorScheme.PrimaryColor;
        }

        using var accentBrush = new SolidBrush(accentColor);
        using var disabledBrush = new SolidBrush(Color.FromArgb(255, 158, 158, 158));

        Color inactiveTrackColor;
        Color disabledColor;
        if (MaterialSkinManager.Instance.Theme == Themes.DARK)
        {
            disabledColor = Color.FromArgb((int)(2.55 * 30), 255, 255, 255);
            inactiveTrackColor = accentColor.Darken(0.25f);
        }
        else
        {
            disabledColor = Color.FromArgb((int)(2.55 * (_hovered ? 38 : 26)), 0, 0, 0);
            inactiveTrackColor = accentColor.Lighten(0.6f);
        }

        var thumbHoverColor = Color.FromArgb((int)(2.55 * 15), accentColor);
        var thumbPressedColor = Color.FromArgb((int)(2.55 * 30), accentColor);

        var _inactiveTrackPath = DrawHelper.CreateRoundRect(_sliderRectangle.X + (_thumbRadius / 2), _sliderRectangle.Y + Height / 2 - _inactiveTrack / 2, _sliderRectangle.Width - _thumbRadius, _inactiveTrack, 2);
        var _activeTrackPath = DrawHelper.CreateRoundRect(_sliderRectangle.X + (_thumbRadius / 2), _sliderRectangle.Y + Height / 2 - _activeTrack / 2, _indicatorRectangleNormal.X - _sliderRectangle.X, _activeTrack, 2);

        if (Enabled)
        {
            //Draw inactive track
            using var inactiveTrackBrush = new SolidBrush(inactiveTrackColor);
            g.FillPath(inactiveTrackBrush, _inactiveTrackPath);
            g.FillPath(accentBrush, _activeTrackPath);

            if (_mousePressed)
            {
                g.FillEllipse(accentBrush, _indicatorRectangleNormal);
                using var thumbPressedBrush = new SolidBrush(thumbPressedColor);
                g.FillEllipse(thumbPressedBrush, _indicatorRectanglePressed);
            }
            else
            {
                g.FillEllipse(accentBrush, _indicatorRectangleNormal);

                if (_hovered)
                {
                    using var thumbHoverBrush = new SolidBrush(thumbHoverColor);
                    g.FillEllipse(thumbHoverBrush, _indicatorRectanglePressed);
                }
            }
        }
        else
        {
            //Draw inactive track
            using var inactiveTrackBrush = new SolidBrush(disabledColor.Lighten(0.25f));
            g.FillPath(inactiveTrackBrush, _inactiveTrackPath);

            //Draw active track
            g.FillPath(disabledBrush, _activeTrackPath);
            g.FillEllipse(disabledBrush, _indicatorRectangleNormal);
        }

        using var renderer = new NativeTextRenderer(g);
        if (ShowText)
        {
            // Draw text
            renderer.DrawTransparentText(
                Text,
                MaterialSkinManager.Instance.GetLogFontByType(FontType),
                Enabled ? MaterialSkinManager.Instance.TextHighEmphasisColor : MaterialSkinManager.Instance.TextDisabledOrHintColor,
                _textRectangle.Location,
                _textRectangle.Size,
                TextAlignFlags.Left | TextAlignFlags.Middle);
        }

        if (ShowValue)
        {
            var format = ValueFormat ?? "{0}";
            // Draw value
            renderer.DrawTransparentText(
                string.Format(format, Value),
                MaterialSkinManager.Instance.GetLogFontByType(FontType),
                Enabled ? MaterialSkinManager.Instance.TextHighEmphasisColor : MaterialSkinManager.Instance.TextDisabledOrHintColor,
                _valueRectangle.Location,
                _valueRectangle.Size,
                TextAlignFlags.Right | TextAlignFlags.Middle);
        }
    }
}
