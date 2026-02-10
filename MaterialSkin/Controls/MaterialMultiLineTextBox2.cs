namespace MaterialSkin.Controls;

public class MaterialMultiLineTextBox2 : Control, IMaterialControl
{
    private const int _lineBottomPadding = 3;
    private const int _topPadding = 10;
    private const int _bottomPadding = 10;
    private const int _leftPadding = 16;
    private const int _rightPadding = 12;

    private readonly MaterialContextMenuStrip _cms = new BaseTextBoxContextMenuStrip();
    private readonly AnimationManager _animationManager;
    private readonly BaseTextBox _baseTextBox;
    private ContextMenuStrip _lastContextMenuStrip = new();
    public bool _isFocused;
    private int _lineY;

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

    public MaterialMultiLineTextBox2()
    {
        AllowScroll = true;
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

        _baseTextBox = new BaseTextBox
        {
            BorderStyle = BorderStyle.None,
            Font = MaterialSkinManager.Instance.GetFontByType(FontType.Subtitle1),
            ForeColor = MaterialSkinManager.Instance.TextHighEmphasisColor,
            Multiline = true
        };

        Cursor = Cursors.IBeam;
        Enabled = true;
        ReadOnly = false;
        ScrollBars = ScrollBars.None;
        Size = new Size(250, 100);

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
        };

        _baseTextBox.LostFocus += (sender, args) =>
        {
            _isFocused = false;
            _animationManager.StartNewAnimation(AnimationDirection.Out);
        };

        _baseTextBox.TextChanged += Redraw;
        _baseTextBox.BackColorChanged += Redraw;

        _baseTextBox.TabStop = true;
        TabStop = false;

        _cms.Opening += ContextMenuStripOnOpening;
        _cms.OnItemClickStart += ContextMenuStripOnItemClickStart;
        ContextMenuStrip = _cms;
        MouseWheel += OnMouseWheel;
    }

    //Properties for managing the material design properties

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

    [Category("Material Skin"), DefaultValue(""), Localizable(true)]
    public string Hint
    {
        get => _baseTextBox.Hint;
        set
        {
            _baseTextBox.Hint = value;
            Invalidate();
        }
    }

    [Category("Material Skin"), DefaultValue(true)]
    public bool UseAccent { get; set; }

    [Browsable(true)]
    [Category("Material Skin"), DefaultValue(true), Description("Defines whether MaterialMultiLineTextBox allows scrolling of text. This property is independent of the ScrollBars property")]
    public bool AllowScroll { get; set; }

    public override ContextMenuStrip? ContextMenuStrip
    {
        get => _baseTextBox.ContextMenuStrip;
        set
        {
            if (value != null)
            {
                _baseTextBox.ContextMenuStrip = value;
                base.ContextMenuStrip = value;
            }
            else
            {
                _baseTextBox.ContextMenuStrip = _cms;
                base.ContextMenuStrip = _cms;
            }
            _lastContextMenuStrip = base.ContextMenuStrip;
        }
    }

    [Browsable(false)]
    public override Color BackColor => Parent == null ? MaterialSkinManager.Instance.BackgroundColor : Parent.BackColor;

    [AllowNull]
    public override string Text { get => _baseTextBox.Text; set => _baseTextBox.Text = value; }

    [Category("Appearance")]
    public HorizontalAlignment TextAlign { get => _baseTextBox.TextAlign; set => _baseTextBox.TextAlign = value; }

    [Category("Appearance")]
    public ScrollBars ScrollBars { get => _baseTextBox.ScrollBars; set => _baseTextBox.ScrollBars = value; }

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
        get;
        set
        {
            field = value;
            if (Enabled == true)
            {
                _baseTextBox.ReadOnly = field;
            }
            Invalidate();
        }
    }

    [Category("Material Skin")]
    [Browsable(true)]
    public bool AnimateReadOnly
    {
        get;
        set
        {
            field = value;
            Invalidate();
        }
    }

    [Category("Material Skin"), DefaultValue(false), Description("Select next control which have TabStop property set to True when enter key is pressed. To add enter in text, the user must press CTRL+Enter")]
    public bool LeaveOnEnterKey
    {
        get;
        set
        {
            field = value;
            if (value)
            {
                _baseTextBox.KeyDown += LeaveOnEnterKey_KeyDown;
            }
            else
            {
                _baseTextBox.KeyDown -= LeaveOnEnterKey_KeyDown;
            }
            Invalidate();
        }
    }

    public void SelectAll() => _baseTextBox.SelectAll();
    public void Clear() => _baseTextBox.Clear();
    public void Copy() => _baseTextBox.Copy();
    public void Cut() => _baseTextBox.Cut();
    public void Undo() => _baseTextBox.Undo();
    public void Paste() => _baseTextBox.Paste();

    private void Redraw(object? sender, EventArgs e)
    {
        SuspendLayout();
        Invalidate();
        ResumeLayout(false);
    }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        var g = pevent.Graphics;
        g.TextRenderingHint = TextRenderingHint.AntiAlias;

        var parentBackColor = Color.White;
        if (Parent != null)
        {
            parentBackColor = Parent.BackColor;
        }

        g.Clear(parentBackColor);
        var backBrush = new SolidBrush(DrawHelper.BlendColor(parentBackColor, MaterialSkinManager.Instance.BackgroundAlternativeColor, MaterialSkinManager.Instance.BackgroundAlternativeColor.A));

        //backColor
        g.FillRectangle(
            !Enabled ? MaterialSkinManager.Instance.BackgroundDisabledBrush : // Disabled
            _isFocused ? MaterialSkinManager.Instance.BackgroundFocusBrush :  // Focused
            MouseState == MouseState.HOVER && (!ReadOnly || (ReadOnly && !AnimateReadOnly)) ? MaterialSkinManager.Instance.BackgroundHoverBrush : // Hover
            backBrush, // Normal
            ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, _lineY);

        _baseTextBox.BackColor = !Enabled ? ColorHelper.RemoveAlpha(MaterialSkinManager.Instance.BackgroundDisabledColor, BackColor) : //Disabled
            _isFocused ? DrawHelper.BlendColor(BackColor, MaterialSkinManager.Instance.BackgroundFocusColor, MaterialSkinManager.Instance.BackgroundFocusColor.A) : //Focused
            MouseState == MouseState.HOVER && (!ReadOnly || (ReadOnly && !AnimateReadOnly)) ? DrawHelper.BlendColor(BackColor, MaterialSkinManager.Instance.BackgroundHoverColor, MaterialSkinManager.Instance.BackgroundHoverColor.A) : // Hover
            DrawHelper.BlendColor(BackColor, MaterialSkinManager.Instance.BackgroundAlternativeColor, MaterialSkinManager.Instance.BackgroundAlternativeColor.A); // Normal

        // bottom line base
        g.FillRectangle(MaterialSkinManager.Instance.DividersAlternativeBrush, 0, _lineY, Width, 1);

        if (!ReadOnly || (ReadOnly && AnimateReadOnly))
        {
            if (!_animationManager.IsAnimating())
            {
                // bottom line
                if (_isFocused)
                {
                    //No animation
                    g.FillRectangle(_isFocused ? UseAccent ? MaterialSkinManager.Instance.ColorScheme.AccentBrush : MaterialSkinManager.Instance.ColorScheme.PrimaryBrush : MaterialSkinManager.Instance.DividersBrush, 0, _lineY, Width, _isFocused ? 2 : 1);
                }
            }
            else
            {
                // Animate - Focus got/lost
                var animationProgress = _animationManager.GetProgress();

                // Line Animation
                var LineAnimationWidth = (int)(Width * animationProgress);
                var LineAnimationX = (Width / 2) - (LineAnimationWidth / 2);
                g.FillRectangle(UseAccent ? MaterialSkinManager.Instance.ColorScheme.AccentBrush : MaterialSkinManager.Instance.ColorScheme.PrimaryBrush, LineAnimationX, _lineY, LineAnimationWidth, 2);
            }
        }
    }

    protected void OnMouseWheel(object? sender, MouseEventArgs e)
    {
        if (AllowScroll)
        {
            if (DesignMode)
                return;

            //Calculate number of notches mouse wheel moved
            var v = e.Delta / 120;
            //Down Movement
            if (v < 0)
            {
                var ptrWparam = new IntPtr(Constants.SB_LINEDOWN);
                Functions.SendMessageW(_baseTextBox.Handle, Constants.WM_VSCROLL, ptrWparam, 0);
            }
            //Up Movement
            else if (v > 0)
            {
                var ptrWparam = new IntPtr(Constants.SB_LINEUP);
                Functions.SendMessageW(_baseTextBox.Handle, Constants.WM_VSCROLL, ptrWparam, 0);
            }

            _baseTextBox?.Focus();
            base.OnMouseDown(e);
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (DesignMode)
            return;

        base.OnMouseMove(e);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (DesignMode)
            return;

        _baseTextBox?.Focus();
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

        base.OnMouseLeave(e);
        MouseState = MouseState.OUT;
        Invalidate();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        _baseTextBox.Location = new Point(_leftPadding, _topPadding);
        _baseTextBox.Width = Width - (_leftPadding + _rightPadding);
        _baseTextBox.Height = Height - (_topPadding + _bottomPadding);
        _lineY = Height - _lineBottomPadding;
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();

        // events
        MouseState = MouseState.OUT;
    }

    private void ContextMenuStripOnItemClickStart(object? sender, ToolStripItemClickedEventArgs toolStripItemClickedEventArgs)
    {
        if (toolStripItemClickedEventArgs.ClickedItem == null)
            return;

        if (toolStripItemClickedEventArgs.ClickedItem.Text == BaseTextBoxContextMenuStrip.UndoText)
        {
            Undo();
            return;
        }

        if (toolStripItemClickedEventArgs.ClickedItem.Text == BaseTextBoxContextMenuStrip.CutText)
        {
            Cut();
            return;
        }

        if (toolStripItemClickedEventArgs.ClickedItem.Text == BaseTextBoxContextMenuStrip.CopyText)
        {
            Copy();
            return;
        }

        if (toolStripItemClickedEventArgs.ClickedItem.Text == BaseTextBoxContextMenuStrip.PasteText)
        {
            Paste();
            return;
        }

        if (toolStripItemClickedEventArgs.ClickedItem.Text == BaseTextBoxContextMenuStrip.DeleteText)
        {
            SelectedText = string.Empty;
            return;
        }

        if (toolStripItemClickedEventArgs.ClickedItem.Text == BaseTextBoxContextMenuStrip.SelectAllText)
        {
            SelectAll();
            return;
        }
    }

    private void ContextMenuStripOnOpening(object? sender, CancelEventArgs cancelEventArgs)
    {
        if (sender is BaseTextBoxContextMenuStrip strip)
        {
            strip.Undo.Enabled = _baseTextBox.CanUndo && !ReadOnly;
            strip.Cut.Enabled = !string.IsNullOrEmpty(SelectedText) && !ReadOnly;
            strip.Copy.Enabled = !string.IsNullOrEmpty(SelectedText);
            strip.Paste.Enabled = Clipboard.ContainsText() && !ReadOnly;
            strip.Delete.Enabled = !string.IsNullOrEmpty(SelectedText) && !ReadOnly;
            strip.SelectAll.Enabled = !string.IsNullOrEmpty(Text);
        }
    }

    private void LeaveOnEnterKey_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyData == Keys.Enter && e.Control == false)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            SendKeys.Send("{TAB}");
        }
    }
}
