namespace MaterialSkin.Controls;

public partial class MaterialTextBox : Control, IMaterialControl
{
    private const int _prefixSuffixPadding = 4;
    private const int _iconSize = 24;
    private const int _hintTextSmallSize = 18;
    private const int _hintTextSmallY = 4;
    private const int _leftPadding = 16;
    private const int _rightPadding = 12;
    private const int _activationIndicatorHeight = 2;
    private const int _helperTextHeightDefault = 16;
    private const int _fontHeight = 20;

    private readonly MaterialContextMenuStrip _cms = new();
    private readonly AnimationManager _animationManager;
    private ContextMenuStrip _lastContextMenuStrip = new();
    private bool _useTallSize;
    private bool _readonly;
    private bool _showAssistiveText;
    private string? _helperText;
    private string? _errorMessage;
    private Image? _leadingIcon;
    private Image? _trailingIcon;
    private PrefixSuffixTypes _prefixsuffix;
    private string? _prefixsuffixText;
    private bool _animateReadOnly;
    private bool _leaveOnEnterKey;
    public bool _isFocused;
    private int _height = 48;
    private int _lineY;
    private bool _hasHint;
    private readonly BaseTextBox _baseTextBox;
    private bool _errorState;
    private int _left_padding;
    private int _right_padding;
    private int _prefix_padding;
    private int _suffix_padding;
    private int _helperTextHeight;
    private Rectangle _leadingIconBounds;
    private Rectangle _trailingIconBounds;
    private Dictionary<string, TextureBrush>? _iconsBrushes;
    private Dictionary<string, TextureBrush>? _iconsErrorBrushes;

    [Category("Action")]
    [Description("Fires when Leading Icon is clicked")]
    public event EventHandler? LeadingIconClick;

    [Category("Action")]
    [Description("Fires when Trailing Icon is clicked")]
    public event EventHandler? TrailingIconClick;

    public event EventHandler AcceptsTabChanged { add => _baseTextBox.AcceptsTabChanged += value; remove => _baseTextBox.AcceptsTabChanged -= value; }
    public new event EventHandler AutoSizeChanged { add => _baseTextBox.AutoSizeChanged += value; remove => _baseTextBox.AutoSizeChanged -= value; }
    public new event EventHandler BackgroundImageChanged { add => _baseTextBox.BackgroundImageChanged += value; remove => _baseTextBox.BackgroundImageChanged -= value; }
    public new event EventHandler BackgroundImageLayoutChanged { add => _baseTextBox.BackgroundImageLayoutChanged += value; remove => _baseTextBox.BackgroundImageLayoutChanged -= value; }
    public new event EventHandler BindingContextChanged { add => _baseTextBox.BindingContextChanged += value; remove => _baseTextBox.BindingContextChanged -= value; }
    public event EventHandler BorderStyleChanged { add => _baseTextBox.BorderStyleChanged += value; remove => _baseTextBox.BorderStyleChanged -= value; }
    public new event EventHandler CausesValidationChanged { add => _baseTextBox.CausesValidationChanged += value; remove => _baseTextBox.CausesValidationChanged -= value; }
    public new event UICuesEventHandler ChangeUICues { add => _baseTextBox.ChangeUICues += value; remove => _baseTextBox.ChangeUICues -= value; }
    public new event EventHandler Click { add => _baseTextBox.Click += value; remove => _baseTextBox.Click -= value; }
    public new event EventHandler ClientSizeChanged { add => _baseTextBox.ClientSizeChanged += value; remove => _baseTextBox.ClientSizeChanged -= value; }
    public new event EventHandler ContextMenuStripChanged { add => _baseTextBox.ContextMenuStripChanged += value; remove => _baseTextBox.ContextMenuStripChanged -= value; }
    public new event ControlEventHandler ControlAdded { add => _baseTextBox.ControlAdded += value; remove => _baseTextBox.ControlAdded -= value; }
    public new event ControlEventHandler ControlRemoved { add => _baseTextBox.ControlRemoved += value; remove => _baseTextBox.ControlRemoved -= value; }
    public new event EventHandler CursorChanged { add => _baseTextBox.CursorChanged += value; remove => _baseTextBox.CursorChanged -= value; }
    public new event EventHandler Disposed { add => _baseTextBox.Disposed += value; remove => _baseTextBox.Disposed -= value; }
    public new event EventHandler DockChanged { add => _baseTextBox.DockChanged += value; remove => _baseTextBox.DockChanged -= value; }
    public new event EventHandler DoubleClick { add => _baseTextBox.DoubleClick += value; remove => _baseTextBox.DoubleClick -= value; }
    public new event DragEventHandler DragDrop { add => _baseTextBox.DragDrop += value; remove => _baseTextBox.DragDrop -= value; }
    public new event DragEventHandler DragEnter { add => _baseTextBox.DragEnter += value; remove => _baseTextBox.DragEnter -= value; }
    public new event EventHandler DragLeave { add => _baseTextBox.DragLeave += value; remove => _baseTextBox.DragLeave -= value; }
    public new event DragEventHandler DragOver { add => _baseTextBox.DragOver += value; remove => _baseTextBox.DragOver -= value; }
    public new event EventHandler EnabledChanged { add => _baseTextBox.EnabledChanged += value; remove => _baseTextBox.EnabledChanged -= value; }
    public new event EventHandler Enter { add => _baseTextBox.Enter += value; remove => _baseTextBox.Enter -= value; }
    public new event EventHandler FontChanged { add => _baseTextBox.FontChanged += value; remove => _baseTextBox.FontChanged -= value; }
    public new event EventHandler ForeColorChanged { add => _baseTextBox.ForeColorChanged += value; remove => _baseTextBox.ForeColorChanged -= value; }
    public new event GiveFeedbackEventHandler GiveFeedback { add => _baseTextBox.GiveFeedback += value; remove => _baseTextBox.GiveFeedback -= value; }
    public new event EventHandler GotFocus { add => _baseTextBox.GotFocus += value; remove => _baseTextBox.GotFocus -= value; }
    public new event EventHandler HandleCreated { add => _baseTextBox.HandleCreated += value; remove => _baseTextBox.HandleCreated -= value; }
    public new event EventHandler HandleDestroyed { add => _baseTextBox.HandleDestroyed += value; remove => _baseTextBox.HandleDestroyed -= value; }
    public new event HelpEventHandler HelpRequested { add => _baseTextBox.HelpRequested += value; remove => _baseTextBox.HelpRequested -= value; }
    public event EventHandler HideSelectionChanged { add => _baseTextBox.HideSelectionChanged += value; remove => _baseTextBox.HideSelectionChanged -= value; }
    public new event EventHandler ImeModeChanged { add => _baseTextBox.ImeModeChanged += value; remove => _baseTextBox.ImeModeChanged -= value; }
    public new event InvalidateEventHandler Invalidated { add => _baseTextBox.Invalidated += value; remove => _baseTextBox.Invalidated -= value; }
    public new event KeyEventHandler KeyDown { add => _baseTextBox.KeyDown += value; remove => _baseTextBox.KeyDown -= value; }
    public new event KeyPressEventHandler KeyPress { add => _baseTextBox.KeyPress += value; remove => _baseTextBox.KeyPress -= value; }
    public new event KeyEventHandler KeyUp { add => _baseTextBox.KeyUp += value; remove => _baseTextBox.KeyUp -= value; }
    public new event LayoutEventHandler Layout { add => _baseTextBox.Layout += value; remove => _baseTextBox.Layout -= value; }
    public new event EventHandler Leave { add => _baseTextBox.Leave += value; remove => _baseTextBox.Leave -= value; }
    public new event EventHandler LocationChanged { add => _baseTextBox.LocationChanged += value; remove => _baseTextBox.LocationChanged -= value; }
    public new event EventHandler LostFocus { add => _baseTextBox.LostFocus += value; remove => _baseTextBox.LostFocus -= value; }
    public new event EventHandler MarginChanged { add => _baseTextBox.MarginChanged += value; remove => _baseTextBox.MarginChanged -= value; }
    public event EventHandler ModifiedChanged { add => _baseTextBox.ModifiedChanged += value; remove => _baseTextBox.ModifiedChanged -= value; }
    public new event EventHandler MouseCaptureChanged { add => _baseTextBox.MouseCaptureChanged += value; remove => _baseTextBox.MouseCaptureChanged -= value; }
    public new event MouseEventHandler MouseClick { add => _baseTextBox.MouseClick += value; remove => _baseTextBox.MouseClick -= value; }
    public new event MouseEventHandler MouseDoubleClick { add => _baseTextBox.MouseDoubleClick += value; remove => _baseTextBox.MouseDoubleClick -= value; }
    public new event MouseEventHandler MouseDown { add => _baseTextBox.MouseDown += value; remove => _baseTextBox.MouseDown -= value; }
    public new event EventHandler MouseEnter { add => _baseTextBox.MouseEnter += value; remove => _baseTextBox.MouseEnter -= value; }
    public new event EventHandler MouseHover { add => _baseTextBox.MouseHover += value; remove => _baseTextBox.MouseHover -= value; }
    public new event EventHandler MouseLeave { add => _baseTextBox.MouseLeave += value; remove => _baseTextBox.MouseLeave -= value; }
    public new event MouseEventHandler MouseMove { add => _baseTextBox.MouseMove += value; remove => _baseTextBox.MouseMove -= value; }
    public new event MouseEventHandler MouseUp { add => _baseTextBox.MouseUp += value; remove => _baseTextBox.MouseUp -= value; }
    public new event MouseEventHandler MouseWheel { add => _baseTextBox.MouseWheel += value; remove => _baseTextBox.MouseWheel -= value; }
    public new event EventHandler Move { add => _baseTextBox.Move += value; remove => _baseTextBox.Move -= value; }
    public event EventHandler MultilineChanged { add => _baseTextBox.MultilineChanged += value; remove => _baseTextBox.MultilineChanged -= value; }
    public new event EventHandler PaddingChanged { add => _baseTextBox.PaddingChanged += value; remove => _baseTextBox.PaddingChanged -= value; }
    public new event PaintEventHandler Paint { add => _baseTextBox.Paint += value; remove => _baseTextBox.Paint -= value; }
    public new event EventHandler ParentChanged { add => _baseTextBox.ParentChanged += value; remove => _baseTextBox.ParentChanged -= value; }
    public new event PreviewKeyDownEventHandler PreviewKeyDown { add => _baseTextBox.PreviewKeyDown += value; remove => _baseTextBox.PreviewKeyDown -= value; }
    public new event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp { add => _baseTextBox.QueryAccessibilityHelp += value; remove => _baseTextBox.QueryAccessibilityHelp -= value; }
    public new event QueryContinueDragEventHandler QueryContinueDrag { add => _baseTextBox.QueryContinueDrag += value; remove => _baseTextBox.QueryContinueDrag -= value; }
    public event EventHandler ReadOnlyChanged { add => _baseTextBox.ReadOnlyChanged += value; remove => _baseTextBox.ReadOnlyChanged -= value; }
    public new event EventHandler RegionChanged { add => _baseTextBox.RegionChanged += value; remove => _baseTextBox.RegionChanged -= value; }
    public new event EventHandler Resize { add => _baseTextBox.Resize += value; remove => _baseTextBox.Resize -= value; }
    public new event EventHandler RightToLeftChanged { add => _baseTextBox.RightToLeftChanged += value; remove => _baseTextBox.RightToLeftChanged -= value; }
    public new event EventHandler SizeChanged { add => _baseTextBox.SizeChanged += value; remove => _baseTextBox.SizeChanged -= value; }
    public new event EventHandler StyleChanged { add => _baseTextBox.StyleChanged += value; remove => _baseTextBox.StyleChanged -= value; }
    public new event EventHandler SystemColorsChanged { add => _baseTextBox.SystemColorsChanged += value; remove => _baseTextBox.SystemColorsChanged -= value; }
    public new event EventHandler TabIndexChanged { add => _baseTextBox.TabIndexChanged += value; remove => _baseTextBox.TabIndexChanged -= value; }
    public new event EventHandler TabStopChanged { add => _baseTextBox.TabStopChanged += value; remove => _baseTextBox.TabStopChanged -= value; }
    public event EventHandler TextAlignChanged { add => _baseTextBox.TextAlignChanged += value; remove => _baseTextBox.TextAlignChanged -= value; }
    public new event EventHandler TextChanged { add => _baseTextBox.TextChanged += value; remove => _baseTextBox.TextChanged -= value; }
    public new event EventHandler Validated { add => _baseTextBox.Validated += value; remove => _baseTextBox.Validated -= value; }
    public new event CancelEventHandler Validating { add => _baseTextBox.Validating += value; remove => _baseTextBox.Validating -= value; }
    public new event EventHandler VisibleChanged { add => _baseTextBox.VisibleChanged += value; remove => _baseTextBox.VisibleChanged -= value; }

    public MaterialTextBox()
    {
        // Material Properties
        UseAccent = true;
        MouseState = MouseState.OUT;

        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer, true);

        // Animations
        _animationManager = new AnimationManager
        {
            Increment = 0.06,
            AnimationType = AnimationType.EaseInOut,
            InterruptAnimation = false
        };
        _animationManager.OnAnimationProgress += (sender, e) => Invalidate();

        SkinManager.ColorSchemeChanged += (sender, e) =>
        {
            PreProcessIcons();
        };

        SkinManager.ThemeChanged += (sender, e) =>
        {
            PreProcessIcons();
        };

        Font = SkinManager.GetFontByType(FontType.Subtitle1);

        _baseTextBox = new BaseTextBox
        {
            BorderStyle = BorderStyle.None,
            Font = base.Font,
            ForeColor = SkinManager.TextHighEmphasisColor,
            Multiline = false,
            Location = new Point(_leftPadding, _height / 2 - _fontHeight / 2),
            Width = Width - (_leftPadding + _rightPadding),
            Height = _fontHeight
        };

        Enabled = true;
        ReadOnly = false;
        Size = new Size(250, _height);

        UseTallSize = true;
        PrefixSuffix = PrefixSuffixTypes.None;
        ShowAssistiveText = false;
        HelperText = string.Empty;
        ErrorMessage = string.Empty;

        if (!Controls.Contains(_baseTextBox) && !DesignMode)
        {
            Controls.Add(_baseTextBox);
        }

        _baseTextBox.GotFocus += (sender, args) =>
        {
            if (Enabled)
            {
                _isFocused = true;
                _animationManager.StartNewAnimation(AnimationDirection.In);
            }
            else
                Focus();
            UpdateRects();
        };
        _baseTextBox.LostFocus += (sender, args) =>
        {
            _isFocused = false;
            _animationManager.StartNewAnimation(AnimationDirection.Out);
            UpdateRects();
        };

        _baseTextBox.TextChanged += new EventHandler(Redraw);
        _baseTextBox.BackColorChanged += new EventHandler(Redraw);

        _baseTextBox.TabStop = true;
        TabStop = false;

        _cms.Opening += ContextMenuStripOnOpening;
        _cms.OnItemClickStart += ContextMenuStripOnItemClickStart;
        ContextMenuStrip = _cms;
    }

    //Properties for managing the material design properties
    [Browsable(false)]
    public int Depth { get; set; }

    [Browsable(false)]
    public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

    [Browsable(false)]
    public MouseState MouseState { get; set; }

    //Unused properties
    [Browsable(false)]
    [AllowNull]
    public override Image BackgroundImage { get; set; }

    [Browsable(false)]
    public override ImageLayout BackgroundImageLayout { get; set; }

    [Browsable(false)]
    public string SelectedText { get => _baseTextBox.SelectedText; set => _baseTextBox.SelectedText = value; }

    [Browsable(false)]
    public int SelectionStart { get => _baseTextBox.SelectionStart; set => _baseTextBox.SelectionStart = value; }

    [Browsable(false)]
    public int SelectionLength { get => _baseTextBox.SelectionLength; set => _baseTextBox.SelectionLength = value; }

    [Browsable(false)]
    public int TextLength => _baseTextBox.TextLength;

    [Browsable(false)]
    public override Color ForeColor { get; set; }

    [Category("Material Skin"), DefaultValue(true), Description("Using a larger size enables the hint to always be visible")]
    public bool UseTallSize
    {
        get => _useTallSize;
        set
        {
            _useTallSize = value;
            UpdateHeight();
            UpdateRects();
            Invalidate();
        }
    }

    [Category("Material Skin"), DefaultValue(false), Description("Assistive elements provide additional detail about text entered into text fields. Could be Helper text or Error message.")]
    public bool ShowAssistiveText
    {
        get => _showAssistiveText;
        set
        {
            _showAssistiveText = value;
            if (_showAssistiveText)
                _helperTextHeight = _helperTextHeightDefault;
            else
                _helperTextHeight = 0;
            UpdateHeight();
            //UpdateRects();
            Invalidate();
        }
    }

    [Category("Material Skin"), DefaultValue(""), Localizable(true), Description("Helper text conveys additional guidance about the input field, such as how it will be used.")]
    public string? HelperText
    {
        get => _helperText;
        set
        {
            _helperText = value;
            Invalidate();
        }
    }

    [Category("Material Skin"), DefaultValue(""), Localizable(true), Description("When text input isn't accepted, an error message can display instructions on how to fix it. Error messages are displayed below the input line, replacing helper text until fixed.")]
    public string? ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            Invalidate();
        }
    }

    [Category("Material Skin"), DefaultValue(""), Localizable(true)]
    public string Hint
    {
        get => _baseTextBox.Hint;
        set
        {
            _baseTextBox.Hint = value;
            _hasHint = !string.IsNullOrEmpty(_baseTextBox.Hint);
            UpdateRects();
            Invalidate();
        }
    }

    [Category("Material Skin"), DefaultValue(true)]
    public bool UseAccent { get; set; }

    [Category("Material Skin"), Browsable(true), Localizable(false)]
    /// <summary>
    /// Gets or sets the leading Icon
    /// </summary>
    public Image? LeadingIcon
    {
        get => _leadingIcon;
        set
        {
            _leadingIcon = value;
            UpdateRects();
            PreProcessIcons();
            Invalidate();
        }
    }

    [Category("Material Skin"), Browsable(true), Localizable(false)]
    /// <summary>
    /// Gets or sets the trailing Icon
    /// </summary>
    public Image? TrailingIcon
    {
        get => _trailingIcon;
        set
        {
            _trailingIcon = value;
            UpdateRects();
            PreProcessIcons();
            Invalidate();
        }
    }

    [Category("Material Skin"), DefaultValue(PrefixSuffixTypes.None), Description("Set Prefix/Suffix/None")]
    public PrefixSuffixTypes PrefixSuffix
    {
        get => _prefixsuffix;
        set
        {
            _prefixsuffix = value;
            UpdateRects();            //Génére une nullref exception
            if (_prefixsuffix == PrefixSuffixTypes.Suffix)
            {
                RightToLeft = RightToLeft.Yes;
            }
            else
            {
                RightToLeft = RightToLeft.No;
            }
            Invalidate();
        }
    }

    [Category("Material Skin"), DefaultValue(""), Localizable(true), Description("Set Prefix or Suffix text")]
    public string? PrefixSuffixText
    {
        get => _prefixsuffixText;
        set
        {
            _prefixsuffixText = value;
            UpdateRects();
            Invalidate();
        }
    }

    public override ContextMenuStrip? ContextMenuStrip
    {
        get => _baseTextBox.ContextMenuStrip;
        set
        {
            if (value != null)
            {
                //ContextMenuStrip = value;
                _baseTextBox.ContextMenuStrip = value;
                base.ContextMenuStrip = value;
            }
            else
            {
                //ContextMenuStrip = cms;
                _baseTextBox.ContextMenuStrip = _cms;
                base.ContextMenuStrip = _cms;
            }
            _lastContextMenuStrip = base.ContextMenuStrip;
        }
    }

    [Browsable(false)]
    public override Color BackColor => Parent == null ? SkinManager.BackgroundColor : Parent.BackColor;

    [AllowNull]
    public override string Text { get => _baseTextBox.Text; set { _baseTextBox.Text = value; UpdateRects(); } }

    [Category("Appearance")]
    public HorizontalAlignment TextAlign { get => _baseTextBox.TextAlign; set => _baseTextBox.TextAlign = value; }

    [Category("Behavior")]
    public CharacterCasing CharacterCasing { get => _baseTextBox.CharacterCasing; set => _baseTextBox.CharacterCasing = value; }

    [Category("Behavior")]
    public bool HideSelection { get => _baseTextBox.HideSelection; set => _baseTextBox.HideSelection = value; }

    [Category("Behavior")]
    public int MaxLength { get => _baseTextBox.MaxLength; set => _baseTextBox.MaxLength = value; }

    [Category("Behavior")]
    public char PasswordChar { get => _baseTextBox.PasswordChar; set => _baseTextBox.PasswordChar = value; }

    [Category("Behavior")]
    public bool ShortcutsEnabled
    {
        get => _baseTextBox.ShortcutsEnabled;
        set
        {
            _baseTextBox.ShortcutsEnabled = value;
            if (value == false)
            {
                _baseTextBox.ContextMenuStrip = null;
                base.ContextMenuStrip = null;
            }
            else
            {
                _baseTextBox.ContextMenuStrip = _lastContextMenuStrip;
                base.ContextMenuStrip = _lastContextMenuStrip;
            }
        }
    }

    [Category("Behavior")]
    public bool UseSystemPasswordChar { get => _baseTextBox.UseSystemPasswordChar; set => _baseTextBox.UseSystemPasswordChar = value; }
    public new object? Tag { get => _baseTextBox.Tag; set => _baseTextBox.Tag = value; }

    [Category("Behavior")]
    public bool ReadOnly
    {
        get => _readonly;
        set
        {
            _readonly = value;
            if (Enabled == true)
            {
                _baseTextBox.ReadOnly = _readonly;
            }
            Invalidate();
        }
    }

    [Category("Material Skin")]
    [Browsable(true)]
    public bool AnimateReadOnly
    {
        get => _animateReadOnly;
        set
        {
            _animateReadOnly = value;
            Invalidate();
        }
    }

    [Category("Material Skin"), DefaultValue(false), Description("Select next control which have TabStop property set to True when enter key is pressed.")]
    public bool LeaveOnEnterKey
    {
        get => _leaveOnEnterKey;
        set
        {
            _leaveOnEnterKey = value;
            if (value)
            {
                _baseTextBox.KeyDown += new KeyEventHandler(LeaveOnEnterKey_KeyDown);
            }
            else
            {
                _baseTextBox.KeyDown -= LeaveOnEnterKey_KeyDown;
            }
            Invalidate();
        }
    }

    public AutoCompleteStringCollection AutoCompleteCustomSource { get => _baseTextBox.AutoCompleteCustomSource; set => _baseTextBox.AutoCompleteCustomSource = value; }
    public AutoCompleteSource AutoCompleteSource { get => _baseTextBox.AutoCompleteSource; set => _baseTextBox.AutoCompleteSource = value; }
    public AutoCompleteMode AutoCompleteMode { get => _baseTextBox.AutoCompleteMode; set => _baseTextBox.AutoCompleteMode = value; }

    public void SelectAll() { _baseTextBox.SelectAll(); }
    public void Clear() { _baseTextBox.Clear(); }
    public void Copy() { _baseTextBox.Copy(); }
    public void Cut() { _baseTextBox.Cut(); }
    public void Undo() { _baseTextBox.Undo(); }
    public void Paste() { _baseTextBox.Paste(); }

    private void Redraw(object? sender, EventArgs e)
    {
        SuspendLayout();
        Invalidate();
        ResumeLayout(false);
    }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        if (Parent == null)
        {
            base.OnPaint(pevent);
            return;
        }

        var g = pevent.Graphics;
        g.TextRenderingHint = TextRenderingHint.AntiAlias;
        g.Clear(Parent.BackColor);
        SolidBrush backBrush = new(DrawHelper.BlendColor(Parent.BackColor, SkinManager.BackgroundAlternativeColor, SkinManager.BackgroundAlternativeColor.A));

        //backColor
        g.FillRectangle(
            !Enabled ? SkinManager.BackgroundDisabledBrush : // Disabled
            _isFocused ? SkinManager.BackgroundFocusBrush :  // Focused
            MouseState == MouseState.HOVER && (!ReadOnly || (ReadOnly && !AnimateReadOnly)) ? SkinManager.BackgroundHoverBrush : // Hover
            backBrush, // Normal
            ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, _lineY);

        _baseTextBox.BackColor = !Enabled ? ColorHelper.RemoveAlpha(SkinManager.BackgroundDisabledColor, BackColor) : //Disabled
            _isFocused ? DrawHelper.BlendColor(BackColor, SkinManager.BackgroundFocusColor, SkinManager.BackgroundFocusColor.A) : //Focused
            MouseState == MouseState.HOVER && (!ReadOnly || (ReadOnly && !AnimateReadOnly)) ? DrawHelper.BlendColor(BackColor, SkinManager.BackgroundHoverColor, SkinManager.BackgroundHoverColor.A) : // Hover
            DrawHelper.BlendColor(BackColor, SkinManager.BackgroundAlternativeColor, SkinManager.BackgroundAlternativeColor.A); // Normal

        //Leading Icon
        if (LeadingIcon != null)
        {
            if (_errorState)
            {
                g.FillRectangle(_iconsErrorBrushes["_leadingIcon"], _leadingIconBounds);
            }
            else
            {
                g.FillRectangle(_iconsBrushes["_leadingIcon"], _leadingIconBounds);
            }
        }

        //Trailing Icon
        if (TrailingIcon != null)
        {
            if (_errorState)
            {
                g.FillRectangle(_iconsErrorBrushes["_trailingIcon"], _trailingIconBounds);
            }
            else
            {
                g.FillRectangle(_iconsBrushes["_trailingIcon"], _trailingIconBounds);
            }
        }

        // HintText
        var userTextPresent = !string.IsNullOrEmpty(Text);
        var helperTextRect = new Rectangle(_leftPadding - _prefix_padding, _lineY + _activationIndicatorHeight, Width - (_leftPadding - _prefix_padding) - _right_padding, _helperTextHeightDefault);
        var hintRect = new Rectangle(_left_padding - _prefix_padding, _hintTextSmallY, Width - (_left_padding - _prefix_padding) - _right_padding, _hintTextSmallSize);
        var hintTextSize = 12;

        // bottom line base
        g.FillRectangle(SkinManager.DividersAlternativeBrush, 0, _lineY, Width, 1);

        if (ReadOnly == false || (ReadOnly && AnimateReadOnly))
        {
            if (!_animationManager.IsAnimating())
            {
                // No animation

                // bottom line
                if (_isFocused)
                {
                    g.FillRectangle(_errorState ? SkinManager.BackgroundHoverRedBrush : _isFocused ? UseAccent ? SkinManager.ColorScheme.AccentBrush : SkinManager.ColorScheme.PrimaryBrush : SkinManager.DividersBrush, 0, _lineY, Width, _isFocused ? 2 : 1);
                }
            }
            else
            {
                // Animate - Focus got/lost
                double animationProgress = _animationManager.GetProgress();

                // Line Animation
                int LineAnimationWidth = (int)(Width * animationProgress);
                int LineAnimationX = (Width / 2) - (LineAnimationWidth / 2);
                g.FillRectangle(UseAccent ? SkinManager.ColorScheme.AccentBrush : SkinManager.ColorScheme.PrimaryBrush, LineAnimationX, _lineY, LineAnimationWidth, 2);
            }
        }

        // Prefix:
        if (_prefixsuffix == PrefixSuffixTypes.Prefix && _prefixsuffixText != null && _prefixsuffixText.Length > 0 && (_isFocused || userTextPresent || !_hasHint))
        {
            using NativeTextRenderer NativeText = new(g);
            Rectangle prefixRect = new(
                _left_padding - _prefix_padding,
                _hasHint && UseTallSize ? hintRect.Y + hintRect.Height - 2 : ClientRectangle.Y,
                //                        NativeText.MeasureLogString(_prefixsuffixText, SkinManager.getLogFontByType(FontType.Subtitle1)).Width,
                _prefix_padding,
                _hasHint && UseTallSize ? _lineY - (hintRect.Y + hintRect.Height) : _lineY);

            // Draw Prefix text 
            NativeText.DrawTransparentText(
            _prefixsuffixText,
            SkinManager.GetLogFontByType(FontType.Subtitle1),
            Enabled ? SkinManager.TextMediumEmphasisColor : SkinManager.TextDisabledOrHintColor,
            prefixRect.Location,
            prefixRect.Size,
            TextAlignFlags.Left | TextAlignFlags.Middle);
        }

        // Suffix:
        if (_prefixsuffix == PrefixSuffixTypes.Suffix && _prefixsuffixText != null && _prefixsuffixText.Length > 0 && (_isFocused || userTextPresent || !_hasHint))
        {
            using var NativeText = new NativeTextRenderer(g);
            var suffixRect = new Rectangle(
                Width - _right_padding,
                _hasHint && UseTallSize ? hintRect.Y + hintRect.Height - 2 : ClientRectangle.Y,
                //NativeText.MeasureLogString(_prefixsuffixText, SkinManager.getLogFontByType(FontType.Subtitle1)).Width + PREFIX_SUFFIX_PADDING,
                _suffix_padding,
                _hasHint && UseTallSize ? _lineY - (hintRect.Y + hintRect.Height) : _lineY);

            // Draw Suffix text 
            NativeText.DrawTransparentText(
                _prefixsuffixText,
                SkinManager.GetLogFontByType(FontType.Subtitle1),
                Enabled ? SkinManager.TextMediumEmphasisColor : SkinManager.TextDisabledOrHintColor,
                suffixRect.Location,
                suffixRect.Size,
                TextAlignFlags.Right | TextAlignFlags.Middle);
        }

        // Draw hint text
        if (_hasHint && UseTallSize && (_isFocused || userTextPresent))
        {
            using var NativeText = new NativeTextRenderer(g);
            NativeText.DrawTransparentText(
                Hint,
                SkinManager.GetTextBoxFontBySize(hintTextSize),
                Enabled ? !_errorState || (!userTextPresent && !_isFocused) ? _isFocused ? UseAccent ?
                SkinManager.ColorScheme.AccentColor : // Focus Accent
                SkinManager.ColorScheme.PrimaryColor : // Focus Primary
                SkinManager.TextMediumEmphasisColor : // not focused
                SkinManager.BackgroundHoverRedColor : // error state
                SkinManager.TextDisabledOrHintColor, // Disabled
                hintRect.Location,
                hintRect.Size,
                TextAlignFlags.Left | TextAlignFlags.Middle);
        }

        // Draw helper text
        if (_showAssistiveText && _isFocused && !_errorState)
        {
            using var NativeText = new NativeTextRenderer(g);
            NativeText.DrawTransparentText(
                HelperText,
                SkinManager.GetTextBoxFontBySize(hintTextSize),
                Enabled ? !_errorState || (!userTextPresent && !_isFocused) ? _isFocused ? UseAccent ?
                SkinManager.ColorScheme.AccentColor : // Focus Accent
                SkinManager.ColorScheme.PrimaryColor : // Focus Primary
                SkinManager.TextMediumEmphasisColor : // not focused
                SkinManager.BackgroundHoverRedColor : // error state
                SkinManager.TextDisabledOrHintColor, // Disabled
                helperTextRect.Location,
                helperTextRect.Size,
                TextAlignFlags.Left | TextAlignFlags.Middle);
        }

        // Draw error message
        if (_showAssistiveText && _errorState && ErrorMessage != null)
        {
            using var NativeText = new NativeTextRenderer(g);
            NativeText.DrawTransparentText(
                ErrorMessage,
                SkinManager.GetTextBoxFontBySize(hintTextSize),
                Enabled ?
                SkinManager.BackgroundHoverRedColor : // error state
                SkinManager.TextDisabledOrHintColor, // Disabled
                helperTextRect.Location,
                helperTextRect.Size,
                TextAlignFlags.Left | TextAlignFlags.Middle);
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (DesignMode)
            return;

        if (LeadingIcon != null && _leadingIconBounds.Contains(e.Location) && LeadingIconClick != null)
        {
            Cursor = Cursors.Hand;
        }
        else if (TrailingIcon != null && _trailingIconBounds.Contains(e.Location) && TrailingIconClick != null)
        {
            Cursor = Cursors.Hand;
        }
        else
        {
            Cursor = Cursors.IBeam;
        }

    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (DesignMode)
            return;

        if (LeadingIcon != null && _leadingIconBounds.Contains(e.Location))
        {
            LeadingIconClick?.Invoke(this, new EventArgs());
        }
        else if (TrailingIcon != null && _trailingIconBounds.Contains(e.Location))
        {
            TrailingIconClick?.Invoke(this, new EventArgs());
        }
        else
        {
            _baseTextBox?.Focus();
        }
        base.OnMouseDown(e);

    }
    protected override void OnMouseEnter(EventArgs e)
    {
        if (DesignMode)
            return;

        base.OnMouseEnter(e);
        MouseState = MouseState.HOVER;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        if (DesignMode)
            return;

        if (ClientRectangle.Contains(PointToClient(MousePosition)))
            return;
        else
        {
            base.OnMouseLeave(e);
            MouseState = MouseState.OUT;
            Invalidate();
        }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        UpdateRects();
        PreProcessIcons();

        Size = new Size(Width, _height);
        _lineY = _height - _activationIndicatorHeight - _helperTextHeight;

    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();

        // events
        MouseState = MouseState.OUT;

    }

    private static Size ResizeIcon(Image Icon)
    {
        int newWidth, newHeight;
        //Resize icon if greater than ICON_SIZE
        if (Icon.Width > _iconSize || Icon.Height > _iconSize)
        {
            //calculate aspect ratio
            float aspect = Icon.Width / (float)Icon.Height;

            //calculate new dimensions based on aspect ratio
            newWidth = (int)(_iconSize * aspect);
            newHeight = (int)(newWidth / aspect);

            //if one of the two dimensions exceed the box dimensions
            if (newWidth > _iconSize || newHeight > _iconSize)
            {
                //depending on which of the two exceeds the box dimensions set it as the box dimension and calculate the other one based on the aspect ratio
                if (newWidth > newHeight)
                {
                    newWidth = _iconSize;
                    newHeight = (int)(newWidth / aspect);
                }
                else
                {
                    newHeight = _iconSize;
                    newWidth = (int)(newHeight * aspect);
                }
            }
        }
        else
        {
            newWidth = Icon.Width;
            newHeight = Icon.Height;
        }

        return new Size()
        {
            Height = newHeight,
            Width = newWidth
        };
    }

    private void PreProcessIcons()
    {
        if (_trailingIcon == null && _leadingIcon == null)
            return;

        // Calculate lightness and color
        var l = (SkinManager.Theme == Themes.LIGHT) ? 0f : 1f;

        // Create matrices
        float[][] matrixGray = [
                [0,   0,   0,   0,  0], // Red scale factor
                [0,   0,   0,   0,  0], // Green scale factor
                [0,   0,   0,   0,  0], // Blue scale factor
                [0,   0,   0, Enabled ? .7f : .3f,  0], // alpha scale factor
                [l,   l,   l,   0,  1]];// offset

        float[][] matrixRed = [
                [0,   0,   0,   0,  0], // Red scale factor
                [0,   0,   0,   0,  0], // Green scale factor
                [0,   0,   0,   0,  0], // Blue scale factor
                [0,   0,   0,   1,  0], // alpha scale factor
                [1,   0,   0,   0,  1]];// offset

        var colorMatrixGray = new ColorMatrix(matrixGray);
        var colorMatrixRed = new ColorMatrix(matrixRed);

        var grayImageAttributes = new ImageAttributes();
        var redImageAttributes = new ImageAttributes();

        // Set color matrices
        grayImageAttributes.SetColorMatrix(colorMatrixGray, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        redImageAttributes.SetColorMatrix(colorMatrixRed, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

        // Create brushes
        _iconsBrushes = new Dictionary<string, TextureBrush>(2);
        _iconsErrorBrushes = new Dictionary<string, TextureBrush>(2);

        // Image Rect
        var destRect = new Rectangle(0, 0, _iconSize, _iconSize);

        if (_leadingIcon != null)
        {
            //Resize icon if greater than ICON_SIZE
            var newSize_leadingIcon = ResizeIcon(_leadingIcon);
            var _leadingIconIconResized = new Bitmap(_leadingIcon, newSize_leadingIcon.Width, newSize_leadingIcon.Height);

            // Create a pre-processed copy of the image (GRAY)
            var bgray = new Bitmap(destRect.Width, destRect.Height);
            using (var gGray = Graphics.FromImage(bgray))
            {
                gGray.DrawImage(_leadingIconIconResized,
                    [
                        new Point(0, 0),
                        new Point(destRect.Width, 0),
                        new Point(0, destRect.Height),
                    ],
                    destRect, GraphicsUnit.Pixel, grayImageAttributes);
            }

            //Create a pre - processed copy of the image(RED)
            var bred = new Bitmap(destRect.Width, destRect.Height);
            using (var gred = Graphics.FromImage(bred))
            {
                gred.DrawImage(_leadingIconIconResized,
                    [
                        new Point(0, 0),
                        new Point(destRect.Width, 0),
                        new Point(0, destRect.Height),
                    ],
                    destRect, GraphicsUnit.Pixel, redImageAttributes);
            }

            // added processed image to brush for drawing
            var textureBrushGray = new TextureBrush(bgray);
            var textureBrushRed = new TextureBrush(bred);

            textureBrushGray.WrapMode = WrapMode.Clamp;
            textureBrushRed.WrapMode = WrapMode.Clamp;

            var iconRect = _leadingIconBounds;

            textureBrushGray.TranslateTransform(iconRect.X + iconRect.Width / 2 - _leadingIconIconResized.Width / 2,
                                                iconRect.Y + iconRect.Height / 2 - _leadingIconIconResized.Height / 2);
            textureBrushRed.TranslateTransform(iconRect.X + iconRect.Width / 2 - _leadingIconIconResized.Width / 2,
                                                 iconRect.Y + iconRect.Height / 2 - _leadingIconIconResized.Height / 2);

            // add to dictionary
            _iconsBrushes.Add("_leadingIcon", textureBrushGray);
            _iconsErrorBrushes.Add("_leadingIcon", textureBrushRed);
        }

        if (_trailingIcon != null)
        {
            //Resize icon if greater than ICON_SIZE
            var newSize_trailingIcon = ResizeIcon(_trailingIcon);
            var _trailingIconResized = new Bitmap(_trailingIcon, newSize_trailingIcon.Width, newSize_trailingIcon.Height);

            // Create a pre-processed copy of the image (GRAY)
            var bgray = new Bitmap(destRect.Width, destRect.Height);
            using (var gGray = Graphics.FromImage(bgray))
            {
                gGray.DrawImage(_trailingIconResized,
                    [
                        new Point(0, 0),
                        new Point(destRect.Width, 0),
                        new Point(0, destRect.Height),
                    ],
                    destRect, GraphicsUnit.Pixel, grayImageAttributes);
            }

            //Create a pre - processed copy of the image(RED)
            var bred = new Bitmap(destRect.Width, destRect.Height);
            using (var gred = Graphics.FromImage(bred))
            {
                gred.DrawImage(_trailingIconResized,
                    [
                        new Point(0, 0),
                        new Point(destRect.Width, 0),
                        new Point(0, destRect.Height),
                    ],
                    destRect, GraphicsUnit.Pixel, redImageAttributes);
            }


            // added processed image to brush for drawing
            var textureBrushGray = new TextureBrush(bgray);
            var textureBrushRed = new TextureBrush(bred);

            textureBrushGray.WrapMode = WrapMode.Clamp;
            textureBrushRed.WrapMode = WrapMode.Clamp;

            var iconRect = _trailingIconBounds;

            textureBrushGray.TranslateTransform(iconRect.X + iconRect.Width / 2 - _trailingIconResized.Width / 2,
                                                iconRect.Y + iconRect.Height / 2 - _trailingIconResized.Height / 2);
            textureBrushRed.TranslateTransform(iconRect.X + iconRect.Width / 2 - _trailingIconResized.Width / 2,
                                                 iconRect.Y + iconRect.Height / 2 - _trailingIconResized.Height / 2);

            // add to dictionary
            _iconsBrushes.Add("_trailingIcon", textureBrushGray);
            _iconsErrorBrushes.Add("_trailingIcon", textureBrushRed);
        }
    }

    private void UpdateHeight()
    {
        _height = _useTallSize ? 48 : 36;
        _height += _helperTextHeight;
        Size = new Size(Size.Width, _height);
    }

    private void UpdateRects()
    {
        if (LeadingIcon != null)
        {
            _left_padding = _leftPadding + _iconSize;
        }
        else
        {
            _left_padding = _leftPadding;
        }

        if (_trailingIcon != null)
        {
            _right_padding = _rightPadding + _iconSize;
        }
        else
        {
            _right_padding = _rightPadding;
        }

        if (_prefixsuffix == PrefixSuffixTypes.Prefix && _prefixsuffixText != null && _prefixsuffixText.Length > 0)
        {
            using NativeTextRenderer NativeText = new(CreateGraphics());
            _prefix_padding = NativeText.MeasureLogString(_prefixsuffixText, SkinManager.GetLogFontByType(FontType.Subtitle1)).Width + _prefixSuffixPadding;
            _left_padding += _prefix_padding;
        }
        else
        {
            _prefix_padding = 0;
        }

        if (_prefixsuffix == PrefixSuffixTypes.Suffix && _prefixsuffixText != null && _prefixsuffixText.Length > 0)
        {
            using NativeTextRenderer NativeText = new(CreateGraphics());
            _suffix_padding = NativeText.MeasureLogString(_prefixsuffixText, SkinManager.GetLogFontByType(FontType.Subtitle1)).Width + _prefixSuffixPadding;
            _right_padding += _suffix_padding;
        }
        else
        {
            _suffix_padding = 0;
        }

        if (_hasHint && UseTallSize && (_isFocused || !string.IsNullOrEmpty(Text)))
        {
            _baseTextBox.Location = new Point(_left_padding, 22);
            _baseTextBox.Width = Width - (_left_padding + _right_padding);
            _baseTextBox.Height = _fontHeight;
        }
        else
        {
            _baseTextBox.Location = new Point(_left_padding, (_lineY + _activationIndicatorHeight) / 2 - _fontHeight / 2);
            _baseTextBox.Width = Width - (_left_padding + _right_padding);
            _baseTextBox.Height = _fontHeight;
        }

        _leadingIconBounds = new Rectangle(8, ((_lineY + _activationIndicatorHeight) / 2) - (_iconSize / 2), _iconSize, _iconSize);
        _trailingIconBounds = new Rectangle(Width - (_iconSize + 8), ((_lineY + _activationIndicatorHeight) / 2) - (_iconSize / 2), _iconSize, _iconSize);
    }

    public void SetErrorState(bool ErrorState)
    {
        _errorState = ErrorState;
        if (_errorState)
        {
            _baseTextBox.ForeColor = SkinManager.BackgroundHoverRedColor;
        }
        else
        {
            _baseTextBox.ForeColor = SkinManager.TextHighEmphasisColor;
        }

        _baseTextBox.Invalidate();
        Invalidate();
    }

    public bool GetErrorState() => _errorState;
    private void ContextMenuStripOnItemClickStart(object sender, ToolStripItemClickedEventArgs toolStripItemClickedEventArgs)
    {
        switch (toolStripItemClickedEventArgs.ClickedItem?.Text)
        {
            case "Undo":
                Undo();
                break;

            case "Cut":
                Cut();
                break;

            case "Copy":
                Copy();
                break;

            case "Paste":
                Paste();
                break;

            case "Delete":
                SelectedText = string.Empty;
                break;

            case "Select All":
                SelectAll();
                break;
        }
    }

    private void ContextMenuStripOnOpening(object? sender, CancelEventArgs cancelEventArgs)
    {
        if (sender is BaseTextBoxContextMenuStrip strip)
        {
            strip.undo.Enabled = _baseTextBox.CanUndo && !ReadOnly;
            strip.cut.Enabled = !string.IsNullOrEmpty(SelectedText) && !ReadOnly;
            strip.copy.Enabled = !string.IsNullOrEmpty(SelectedText);
            strip.paste.Enabled = Clipboard.ContainsText() && !ReadOnly;
            strip.delete.Enabled = !string.IsNullOrEmpty(SelectedText) && !ReadOnly;
            strip.selectAll.Enabled = !string.IsNullOrEmpty(Text);
        }
    }

    private void LeaveOnEnterKey_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            SendKeys.Send("{TAB}");
        }
    }
}
