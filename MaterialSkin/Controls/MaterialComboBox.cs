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

        Font = MaterialSkinManager.Instance.GetFontByType(FontType.Subtitle2);
        BackColor = MaterialSkinManager.Instance.BackgroundColor;
        ForeColor = MaterialSkinManager.Instance.TextHighEmphasisColor;
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
            MouseState = MouseState.Out;
            if (SelectedIndex < 0 && !Focused) _animationManager.StartNewAnimation(AnimationDirection.Out);
        };

        LostFocus += (sender, args) =>
        {
            MouseState = MouseState.Out;
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
            MouseState = MouseState.Hover;
            Invalidate();
        };

        MouseLeave += (sender, args) =>
        {
            MouseState = MouseState.Out;
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
    public MouseState MouseState { get; private set; }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        var g = pevent.Graphics;

        if (Parent != null)
        {
            g.Clear(Parent.BackColor);
        }

        g.FillRectangle(Enabled ? Focused ?
            MaterialSkinManager.Instance.BackgroundFocusBrush : // Focused
            MouseState == MouseState.Hover ?
            MaterialSkinManager.Instance.BackgroundHoverBrush : // Hover
            MaterialSkinManager.Instance.BackgroundAlternativeBrush : // normal
            MaterialSkinManager.Instance.BackgroundDisabledBrush // Disabled
            , ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, _lineY);

        Color selectedColor;
        if (UseAccent)
        {
            selectedColor = MaterialSkinManager.Instance.ColorScheme.AccentColor;
        }
        else
        {
            selectedColor = MaterialSkinManager.Instance.ColorScheme.PrimaryColor;
        }

        using var selectedBrush = new SolidBrush(selectedColor);

        // Create and Draw the arrow
        var pth = new GraphicsPath();
        var TopRight = new PointF(Width - 0.5f - MaterialSkinManager.Instance.FormPadding, (Height >> 1) - 2.5f);
        var MidBottom = new PointF(Width - 4.5f - MaterialSkinManager.Instance.FormPadding, (Height >> 1) + 2.5f);
        var TopLeft = new PointF(Width - 8.5f - MaterialSkinManager.Instance.FormPadding, (Height >> 1) - 2.5f);
        pth.AddLine(TopLeft, TopRight);
        pth.AddLine(TopRight, MidBottom);

        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pathBrush = new SolidBrush(DrawHelper.BlendColor(MaterialSkinManager.Instance.TextHighEmphasisColor, MaterialSkinManager.Instance.SwitchOffDisabledThumbColor, 197));
        g.FillPath((SolidBrush)(Enabled ? DroppedDown || Focused ?
            selectedBrush : //DroppedDown or Focused
            MaterialSkinManager.Instance.TextHighEmphasisBrush : //Not DroppedDown and not Focused
            pathBrush  //Disabled
            ), pth);
        g.SmoothingMode = SmoothingMode.None;

        // HintText
        var userTextPresent = SelectedIndex >= 0;
        var hintRect = new Rectangle(MaterialSkinManager.Instance.FormPadding, ClientRectangle.Y, Width, _lineY);
        var hintTextSize = 16;

        // bottom line base
        g.FillRectangle(MaterialSkinManager.Instance.DividersAlternativeBrush, 0, _lineY, Width, 1);

        if (!_animationManager.IsAnimating())
        {
            // No animation
            if (_hasHint && UseTallSize && (DroppedDown || Focused || SelectedIndex >= 0))
            {
                // hint text
                hintRect = new Rectangle(MaterialSkinManager.Instance.FormPadding, _textSmallY, Width, _textSmallSize);
                hintTextSize = 12;
            }

            // bottom line
            if (DroppedDown || Focused)
            {
                g.FillRectangle(selectedBrush, 0, _lineY, Width, 2);
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
                    MaterialSkinManager.Instance.FormPadding,
                    userTextPresent && !_animationManager.IsAnimating() ? _textSmallY : ClientRectangle.Y + (int)((_textSmallY - ClientRectangle.Y) * animationProgress),
                    Width,
                    userTextPresent && !_animationManager.IsAnimating() ? _textSmallSize : (int)(_lineY + (_textSmallSize - _lineY) * animationProgress));
                hintTextSize = userTextPresent && !_animationManager.IsAnimating() ? 12 : (int)(16 + (12 - 16) * animationProgress);
            }

            // Line Animation
            var LineAnimationWidth = (int)(Width * animationProgress);
            var LineAnimationX = (Width / 2) - (LineAnimationWidth / 2);
            g.FillRectangle(selectedBrush, LineAnimationX, _lineY, LineAnimationWidth, 2);
        }

        // Calc text Rect
        var textRect = new Rectangle(
            MaterialSkinManager.Instance.FormPadding,
            _hasHint && UseTallSize ? hintRect.Y + hintRect.Height - 2 : ClientRectangle.Y,
            ClientRectangle.Width - MaterialSkinManager.Instance.FormPadding * 3 - 8,
            _hasHint && UseTallSize ? _lineY - (hintRect.Y + hintRect.Height) : _lineY);

        g.Clip = new Region(textRect);

        using (var NativeText = new NativeTextRenderer(g))
        {
            // Draw user text
            NativeText.DrawTransparentText(
                Text,
                MaterialSkinManager.Instance.GetLogFontByType(FontType.Subtitle1),
                Enabled ? MaterialSkinManager.Instance.TextHighEmphasisColor : MaterialSkinManager.Instance.TextDisabledOrHintColor,
                textRect.Location,
                textRect.Size,
                TextAlignFlags.Left | TextAlignFlags.Middle);
        }

        g.ResetClip();

        // Draw hint text
        if (_hasHint && (UseTallSize || string.IsNullOrEmpty(Text)))
        {
            using var renderer = new NativeTextRenderer(g);
            renderer.DrawTransparentText(
                Hint,
                MaterialSkinManager.Instance.GetTextBoxFontBySize(hintTextSize),
                Enabled ? DroppedDown || Focused ?
                selectedColor : // Focus 
                MaterialSkinManager.Instance.TextMediumEmphasisColor : // not focused
                MaterialSkinManager.Instance.TextDisabledOrHintColor, // Disabled
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
        g.FillRectangle(MaterialSkinManager.Instance.BackgroundBrush, e.Bounds);

        // Hover
        if (e.State.HasFlag(DrawItemState.Focus)) // Focus == hover
        {
            g.FillRectangle(MaterialSkinManager.Instance.BackgroundHoverBrush, e.Bounds);
        }

        string? text;
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

        using var renderer = new NativeTextRenderer(g);
        renderer.DrawTransparentText(
            text,
            MaterialSkinManager.Instance.GetFontByType(FontType.Subtitle1),
            MaterialSkinManager.Instance.TextHighEmphasisNoAlphaColor,
            new Point(e.Bounds.Location.X + MaterialSkinManager.Instance.FormPadding, e.Bounds.Location.Y),
            new Size(e.Bounds.Size.Width - MaterialSkinManager.Instance.FormPadding * 2, e.Bounds.Size.Height),
            TextAlignFlags.Left | TextAlignFlags.Middle);
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();
        MouseState = MouseState.Out;
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
        var padding = MaterialSkinManager.Instance.FormPadding * 3;
        var vertScrollBarWidth = (Items.Count > MaxDropDownItems) ? SystemInformation.VerticalScrollBarWidth : 0;

        var g = CreateGraphics();
        using var renderer = new NativeTextRenderer(g);
        var itemsList = Items.Cast<object>().Select(item => item.ToString());
        foreach (var s in itemsList)
        {
            if (s == null)
                continue;

            var newWidth = renderer.MeasureLogString(s, MaterialSkinManager.Instance.GetLogFontByType(FontType.Subtitle1)).Width + vertScrollBarWidth + padding;
            if (w < newWidth)
            {
                w = newWidth;
            }
        }

        if (Width != w)
        {
            DropDownWidth = w;
            Width = w;
        }
    }
}
