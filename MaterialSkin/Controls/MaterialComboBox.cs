namespace MaterialSkin.Controls;

public class MaterialComboBox : ComboBox, IMaterialControl
{
    private const int _textSmallSize = 18;
    private const int _textSmallY = 4;
    private const int _bottomPadding = 3;

    // For some reason, even when overriding the AutoSize property, it doesn't appear on the properties panel, so we have to create a new one.
    private readonly AnimationManager _animationManager;
    private int _height = 50;
    private int _lineY;
    private bool _hasHint;

    public MaterialComboBox()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

        // Material Properties
        Hint = string.Empty;
        UseAccent = true;
        UseTallSize = true;
        MaxDropDownItems = 4;

        Font = SkinManager.GetFontByType(FontType.Subtitle2);
        BackColor = SkinManager.BackgroundColor;
        ForeColor = SkinManager.TextHighEmphasisColor;
        DrawMode = DrawMode.OwnerDrawVariable;
        DropDownStyle = ComboBoxStyle.DropDownList;
        DropDownWidth = Width;

        // Animations
        _animationManager = new AnimationManager(true)
        {
            Increment = 0.08,
            AnimationType = AnimationType.EaseInOut
        };

        _animationManager.OnAnimationProgress += (sender, e) => Invalidate();
        _animationManager.OnAnimationFinished += (sender, e) => _animationManager.SetProgress(0);

        DropDownClosed += (sender, args) =>
        {
            MouseState = MouseState.OUT;
            if (SelectedIndex < 0 && !Focused) _animationManager.StartNewAnimation(AnimationDirection.Out);
        };

        LostFocus += (sender, args) =>
        {
            MouseState = MouseState.OUT;
            if (SelectedIndex < 0) _animationManager.StartNewAnimation(AnimationDirection.Out);
        };

        DropDown += (sender, args) =>
        {
            _animationManager.StartNewAnimation(AnimationDirection.In);
        };

        GotFocus += (sender, args) =>
        {
            _animationManager.StartNewAnimation(AnimationDirection.In);
            Invalidate();
        };

        MouseEnter += (sender, args) =>
        {
            MouseState = MouseState.HOVER;
            Invalidate();
        };

        MouseLeave += (sender, args) =>
        {
            MouseState = MouseState.OUT;
            Invalidate();
        };

        SelectedIndexChanged += (sender, args) =>
        {
            Invalidate();
        };

        KeyUp += (sender, args) =>
        {
            if (Enabled && DropDownStyle == ComboBoxStyle.DropDownList && (args.KeyCode == Keys.Delete || args.KeyCode == Keys.Back))
            {
                SelectedIndex = -1;
                Invalidate();
            }
        };
    }

    [Category("Material Skin"), DefaultValue(""), Localizable(true)]
    public string Hint
    {
        get;
        set
        {
            field = value;
            _hasHint = !string.IsNullOrEmpty(Hint);
            Invalidate();
        }
    } = string.Empty;

    public int StartIndex
    {
        get;
        set
        {
            field = value;
            try
            {
                if (Items.Count > 0)
                {
                    base.SelectedIndex = value;
                }
            }
            catch
            {
            }
            Invalidate();
        }
    }
    [Category("Material Skin"), DefaultValue(true), Description("Using a larger size enables the hint to always be visible")]
    public bool UseTallSize
    {
        get;
        set
        {
            field = value;
            SetHeightVars();
            Invalidate();
        }
    }

    [Category("Material Skin"), DefaultValue(true)]
    public bool UseAccent { get; set; }

    [Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Category("Layout")]
    public bool AutoResize
    {
        get;
        set
        {
            field = value;
            RecalculateAutoSize();
        }
    }

    //Properties for managing the material design properties
    [Browsable(false)]
    public int Depth { get; set; }

    [Browsable(false)]
    public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

    [Browsable(false)]
    public MouseState MouseState { get; set; }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        var g = pevent.Graphics;

        if (Parent != null)
        {
            g.Clear(Parent.BackColor);
        }

        g.FillRectangle(Enabled ? Focused ?
            SkinManager.BackgroundFocusBrush : // Focused
            MouseState == MouseState.HOVER ?
            SkinManager.BackgroundHoverBrush : // Hover
            SkinManager.BackgroundAlternativeBrush : // normal
            SkinManager.BackgroundDisabledBrush // Disabled
            , ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, _lineY);

        Color SelectedColor;
        if (UseAccent)
        {
            SelectedColor = SkinManager.ColorScheme.AccentColor;
        }
        else
        {
            SelectedColor = SkinManager.ColorScheme.PrimaryColor;
        }

        var SelectedBrush = new SolidBrush(SelectedColor);

        // Create and Draw the arrow
        var pth = new GraphicsPath();
        var TopRight = new PointF(Width - 0.5f - SkinManager.FORM_PADDING, (Height >> 1) - 2.5f);
        var MidBottom = new PointF(Width - 4.5f - SkinManager.FORM_PADDING, (Height >> 1) + 2.5f);
        var TopLeft = new PointF(Width - 8.5f - SkinManager.FORM_PADDING, (Height >> 1) - 2.5f);
        pth.AddLine(TopLeft, TopRight);
        pth.AddLine(TopRight, MidBottom);

        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.FillPath((SolidBrush)(Enabled ? DroppedDown || Focused ?
            SelectedBrush : //DroppedDown or Focused
            SkinManager.TextHighEmphasisBrush : //Not DroppedDown and not Focused
            new SolidBrush(DrawHelper.BlendColor(SkinManager.TextHighEmphasisColor, SkinManager.SwitchOffDisabledThumbColor, 197))  //Disabled
            ), pth);
        g.SmoothingMode = SmoothingMode.None;

        // HintText
        var userTextPresent = SelectedIndex >= 0;
        var hintRect = new Rectangle(SkinManager.FORM_PADDING, ClientRectangle.Y, Width, _lineY);
        var hintTextSize = 16;

        // bottom line base
        g.FillRectangle(SkinManager.DividersAlternativeBrush, 0, _lineY, Width, 1);

        if (!_animationManager.IsAnimating())
        {
            // No animation
            if (_hasHint && UseTallSize && (DroppedDown || Focused || SelectedIndex >= 0))
            {
                // hint text
                hintRect = new Rectangle(SkinManager.FORM_PADDING, _textSmallY, Width, _textSmallSize);
                hintTextSize = 12;
            }

            // bottom line
            if (DroppedDown || Focused)
            {
                g.FillRectangle(SelectedBrush, 0, _lineY, Width, 2);
            }
        }
        else
        {
            // Animate - Focus got/lost
            var animationProgress = _animationManager.GetProgress();

            // hint Animation
            if (_hasHint && UseTallSize)
            {
                hintRect = new Rectangle(
                    SkinManager.FORM_PADDING,
                    userTextPresent && !_animationManager.IsAnimating() ? _textSmallY : ClientRectangle.Y + (int)((_textSmallY - ClientRectangle.Y) * animationProgress),
                    Width,
                    userTextPresent && !_animationManager.IsAnimating() ? _textSmallSize : (int)(_lineY + (_textSmallSize - _lineY) * animationProgress));
                hintTextSize = userTextPresent && !_animationManager.IsAnimating() ? 12 : (int)(16 + (12 - 16) * animationProgress);
            }

            // Line Animation
            var LineAnimationWidth = (int)(Width * animationProgress);
            var LineAnimationX = (Width / 2) - (LineAnimationWidth / 2);
            g.FillRectangle(SelectedBrush, LineAnimationX, _lineY, LineAnimationWidth, 2);
        }

        // Calc text Rect
        var textRect = new Rectangle(
            SkinManager.FORM_PADDING,
            _hasHint && UseTallSize ? hintRect.Y + hintRect.Height - 2 : ClientRectangle.Y,
            ClientRectangle.Width - SkinManager.FORM_PADDING * 3 - 8,
            _hasHint && UseTallSize ? _lineY - (hintRect.Y + hintRect.Height) : _lineY);

        g.Clip = new Region(textRect);

        using (var NativeText = new NativeTextRenderer(g))
        {
            // Draw user text
            NativeText.DrawTransparentText(
                Text,
                SkinManager.GetLogFontByType(FontType.Subtitle1),
                Enabled ? SkinManager.TextHighEmphasisColor : SkinManager.TextDisabledOrHintColor,
                textRect.Location,
                textRect.Size,
                TextAlignFlags.Left | TextAlignFlags.Middle);
        }

        g.ResetClip();

        // Draw hint text
        if (_hasHint && (UseTallSize || string.IsNullOrEmpty(Text)))
        {
            using var NativeText = new NativeTextRenderer(g);
            NativeText.DrawTransparentText(
                Hint,
                SkinManager.GetTextBoxFontBySize(hintTextSize),
                Enabled ? DroppedDown || Focused ?
                SelectedColor : // Focus 
                SkinManager.TextMediumEmphasisColor : // not focused
                SkinManager.TextDisabledOrHintColor, // Disabled
                hintRect.Location,
                hintRect.Size,
                TextAlignFlags.Left | TextAlignFlags.Middle);
        }
    }

    private void CustomMeasureItem(object? sender, MeasureItemEventArgs e) => e.ItemHeight = _height - 7;

    private void CustomDrawItem(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0 || e.Index > Items.Count || !Focused)
            return;

        var g = e.Graphics;

        // Draw the background of the item.
        g.FillRectangle(SkinManager.BackgroundBrush, e.Bounds);

        // Hover
        if (e.State.HasFlag(DrawItemState.Focus)) // Focus == hover
        {
            g.FillRectangle(SkinManager.BackgroundHoverBrush, e.Bounds);
        }

        string? text = null;
        if (!string.IsNullOrWhiteSpace(DisplayMember))
        {
#pragma warning disable IL2075 // 'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.
            if (!Items[e.Index]?.GetType().Equals(typeof(DataRowView)) == true)
            {
                var item = Items[e.Index]?.GetType()?.GetProperty(DisplayMember)?.GetValue(Items[e.Index]);
                text = item?.ToString();
            }
            else
            {
                var table = (Items[e.Index]?.GetType()?.GetProperty("Row")?.GetValue(Items[e.Index]) as DataRow)?.Table;
                text = table?.Rows[e.Index][DisplayMember].ToString();
            }
#pragma warning restore IL2075 // 'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.
        }
        else
        {
            text = Items[e.Index]?.ToString();
        }

        using var NativeText = new NativeTextRenderer(g);
        NativeText.DrawTransparentText(
            text,
            SkinManager.GetFontByType(FontType.Subtitle1),
            SkinManager.TextHighEmphasisNoAlphaColor,
            new Point(e.Bounds.Location.X + SkinManager.FORM_PADDING, e.Bounds.Location.Y),
            new Size(e.Bounds.Size.Width - SkinManager.FORM_PADDING * 2, e.Bounds.Size.Height),
            TextAlignFlags.Left | TextAlignFlags.Middle);
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();
        MouseState = MouseState.OUT;
        MeasureItem += CustomMeasureItem;
        DrawItem += CustomDrawItem;
        DropDownStyle = ComboBoxStyle.DropDownList;
        DrawMode = DrawMode.OwnerDrawVariable;
        RecalculateAutoSize();
        SetHeightVars();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        RecalculateAutoSize();
        SetHeightVars();
    }

    private void SetHeightVars()
    {
        _height = UseTallSize ? 50 : 36;
        Size = new Size(Size.Width, _height);
        _lineY = _height - _bottomPadding;
        ItemHeight = _height - 7;
        DropDownHeight = ItemHeight * MaxDropDownItems + 2;
    }

    public void RecalculateAutoSize()
    {
        if (!AutoResize)
            return;

        var w = DropDownWidth;
        var padding = SkinManager.FORM_PADDING * 3;
        var vertScrollBarWidth = (Items.Count > MaxDropDownItems) ? SystemInformation.VerticalScrollBarWidth : 0;

        var g = CreateGraphics();
        using (var NativeText = new NativeTextRenderer(g))
        {
            var itemsList = Items.Cast<object>().Select(item => item.ToString());
            foreach (var s in itemsList)
            {
                if (s == null)
                    continue;

                var newWidth = NativeText.MeasureLogString(s, SkinManager.GetLogFontByType(FontType.Subtitle1)).Width + vertScrollBarWidth + padding;
                if (w < newWidth)
                {
                    w = newWidth;
                }
            }
        }

        if (Width != w)
        {
            DropDownWidth = w;
            Width = w;
        }
    }
}
