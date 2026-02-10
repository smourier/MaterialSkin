namespace MaterialSkin.Controls;

public class MaterialExpansionPanel : Panel, IMaterialControl
{
    private const int _expansionPanelDefaultPadding = 16;
    private const int _leftrightPadding = 24;
    private const int _buttonPadding = 8;
    private const int _expandcollapsbuttonsize = 24;
    private const int _textHeaderHeight = 24;
    private const int _headerHeightCollapse = 48;
    private const int _headerHeightExpand = 64;
    private const int _footerHeight = 68;
    private const int _footerButtonHeight = 36;
    private const int _minHeight = 200;

    private readonly MaterialButton _validationButton;
    private readonly MaterialButton _cancelButton;
    private int _headerHeight;
    private int _expandHeight;
    private bool _shadowDrawEventSubscribed = false;
    private Rectangle _headerBounds;
    private Rectangle _expandcollapseBounds;
    private Rectangle _savebuttonBounds;
    private Rectangle _cancelbuttonBounds;
    private ButtonState _buttonState = ButtonState.None;
    private Control? _oldParent;

    [Description("Fires when Save button is clicked")]
    public event EventHandler? SaveClick;

    [Category("Action")]
    [Description("Fires when Cancel button is clicked")]
    public event EventHandler? CancelClick;

    [Category("Disposition")]
    [Description("Fires when Panel Collapse")]
    public event EventHandler? PanelCollapse;

    [Category("Disposition")]
    [Description("Fires when Panel Expand")]
    public event EventHandler? PanelExpand;

    public MaterialExpansionPanel()
    {
        ShowValidationButtons = true;
        ValidationButtonEnable = false;
        ValidationButtonText = Functions.LoadDllString("shell32.dll", 38243) ?? "SAVE";
        CancelButtonText = FlexibleMaterialForm.GetButtonText(ButtonId.Cancel);
        ShowCollapseExpand = true;
        Collapse = false;
        Title = Functions.LoadDllString("propsys.dll", 38662) ?? "Title";
        Description = Functions.LoadDllString("propsys.dll", 38948) ?? "Description";
        DrawShadows = true;
        ExpandHeight = 240;
        AutoScroll = false;

        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        BackColor = SkinManager.BackgroundColor;
        ForeColor = SkinManager.TextHighEmphasisColor;

        Padding = new Padding(24, 64, 24, 16);
        Margin = new Padding(3, 16, 3, 16);
        Size = new Size(480, ExpandHeight);

        _validationButton = new MaterialButton
        {
            DrawShadows = false,
            Type = MaterialButtonType.Text,
            UseAccentColor = UseAccentColor,
            Enabled = ValidationButtonEnable,
            Visible = ShowValidationButtons,
            Text = Functions.LoadDllString("shell32.dll", 38243) ?? "SAVE"
        };

        _cancelButton = new MaterialButton
        {
            DrawShadows = false,
            Type = MaterialButtonType.Text,
            UseAccentColor = UseAccentColor,
            Visible = ShowValidationButtons,
            Text = FlexibleMaterialForm.GetButtonText(ButtonId.Cancel)
        };

        if (!Controls.Contains(_validationButton))
        {
            Controls.Add(_validationButton);
        }

        if (!Controls.Contains(_cancelButton))
        {
            Controls.Add(_cancelButton);
        }

        _validationButton.Click += ValidationButton_Click;
        _cancelButton.Click += CancelButton_Click;

        UpdateRects();
    }

    [Browsable(false)]
    public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

    [Browsable(false)]
    public MouseState MouseState { get; set; }

    [Category("Material Skin"), DefaultValue(false), DisplayName("Use Accent Color")]
    public bool UseAccentColor
    {
        get;
        set { field = value; UpdateRects(); Invalidate(); }
    }

    [DefaultValue(false)]
    [Description("Collapses the control when set to true")]
    [Category("Material Skin")]
    public bool Collapse
    {
        get;
        set
        {
            field = value;
            CollapseOrExpand();
            Invalidate();
        }
    }

    [DefaultValue("Title")]
    [Category("Material Skin"), DisplayName("Title")]
    [Description("Title to show in expansion panel's header")]
    public string? Title
    {
        get;
        set
        {
            field = value;
            Invalidate();
        }
    }

    [DefaultValue("Description")]
    [Category("Material Skin"), DisplayName("Description")]
    [Description("Description to show in expansion panel's header")]
    public string? Description
    {
        get;
        set
        {
            field = value;
            Invalidate();
        }
    }

    [DefaultValue(true)]
    [Category("Material Skin"), DisplayName("Draw Shadows")]
    [Description("Draw Shadows around control")]
    public bool DrawShadows
    {
        get;
        set { field = value; Invalidate(); }
    }

    [DefaultValue(240)]
    [Category("Material Skin"), DisplayName("Expand Height")]
    [Description("Define control height when expanded")]
    public int ExpandHeight
    {
        get => _expandHeight;
        set { if (value < _minHeight) value = _minHeight; _expandHeight = value; Invalidate(); }
    }

    [DefaultValue(true)]
    [Category("Material Skin"), DisplayName("Show collapse/expand")]
    [Description("Show collapse/expand indicator")]
    public bool ShowCollapseExpand
    {
        get;
        set { field = value; Invalidate(); }
    }

    [DefaultValue(true)]
    [Category("Material Skin"), DisplayName("Show validation buttons")]
    [Description("Show save/cancel button")]
    public bool ShowValidationButtons
    {
        get;
        set { field = value; UpdateRects(); Invalidate(); }
    }

    [DefaultValue("SAVE")]
    [Category("Material Skin"), DisplayName("Validation button text")]
    [Description("Set Validation button text")]
    public string? ValidationButtonText
    {
        get;
        set { field = value; UpdateRects(); Invalidate(); }
    }

    [DefaultValue("CANCEL")]
    [Category("Material Skin"), DisplayName("Cancel button text")]
    [Description("Set Cancel button text")]
    public string? CancelButtonText
    {
        get;
        set { field = value; UpdateRects(); Invalidate(); }
    }

    [DefaultValue(false)]
    [Category("Material Skin"), DisplayName("Validation button enable")]
    [Description("Enable validation button")]
    public bool ValidationButtonEnable
    {
        get;
        set { field = value; UpdateRects(); Invalidate(); }
    }

    [Category("Action")]
    private void CancelButton_Click(object? sender, EventArgs e)
    {
        CancelClick?.Invoke(this, new EventArgs());
        Collapse = true;
        CollapseOrExpand();
    }

    private void ValidationButton_Click(object? sender, EventArgs e)
    {
        SaveClick?.Invoke(this, new EventArgs());
        Collapse = true;
        CollapseOrExpand();
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();
        Font = SkinManager.GetFontByType(FontType.Body1);
    }

    protected override void InitLayout()
    {
        LocationChanged += (sender, e) => { Parent?.Invalidate(); };
        ForeColor = SkinManager.TextHighEmphasisColor;
    }

    protected override void OnParentChanged(EventArgs e)
    {
        base.OnParentChanged(e);
        if (Parent != null)
        {
            AddShadowPaintEvent(Parent, DawShadowOnParent);
        }

        if (_oldParent != null)
        {
            RemoveShadowPaintEvent(_oldParent, DawShadowOnParent);
        }

        _oldParent = Parent;
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);
        if (Parent == null)
            return;

        if (Visible)
        {
            AddShadowPaintEvent(Parent, DawShadowOnParent);
        }
        else
        {
            RemoveShadowPaintEvent(Parent, DawShadowOnParent);
        }
    }

    private void DawShadowOnParent(object? sender, PaintEventArgs e)
    {
        if (Parent == null)
        {
            RemoveShadowPaintEvent((Control)sender!, DawShadowOnParent);
            return;
        }

        if (!DrawShadows || Parent == null)
            return;

        // paint shadow on parent
        var gp = e.Graphics;
        var rect = new Rectangle(Location, ClientRectangle.Size);
        gp.SmoothingMode = SmoothingMode.AntiAlias;
        DrawHelper.DrawSquareShadow(gp, rect);
    }


    private void AddShadowPaintEvent(Control control, PaintEventHandler shadowPaintEvent)
    {
        if (_shadowDrawEventSubscribed)
            return;

        control.Paint += shadowPaintEvent;
        control.Invalidate();
        _shadowDrawEventSubscribed = true;
    }

    private void RemoveShadowPaintEvent(Control control, PaintEventHandler shadowPaintEvent)
    {
        if (!_shadowDrawEventSubscribed)
            return;

        control.Paint -= shadowPaintEvent;
        control.Invalidate();
        _shadowDrawEventSubscribed = false;
    }

    protected override void OnBackColorChanged(EventArgs e)
    {
        base.OnBackColorChanged(e);
        BackColor = SkinManager.BackgroundColor;
    }

    protected override void OnResize(EventArgs e)
    {
        if (!Collapse)
        {
            if (DesignMode)
            {
                _expandHeight = Height;
            }
            if (Height < _minHeight) Height = _minHeight;
        }
        else
        {
            Height = _headerHeightCollapse;
        }

        base.OnResize(e);

        _headerBounds = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, _headerHeight);
        _expandcollapseBounds = new Rectangle(Width - _leftrightPadding - _expandcollapsbuttonsize, (_headerHeight - _expandcollapsbuttonsize) / 2, _expandcollapsbuttonsize, _expandcollapsbuttonsize);

        UpdateRects();

        if (Parent != null)
        {
            RemoveShadowPaintEvent(Parent, DawShadowOnParent);
            AddShadowPaintEvent(Parent, DawShadowOnParent);
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (DesignMode)
            return;

        var oldState = _buttonState;

        if (_savebuttonBounds.Contains(e.Location))
        {
            _buttonState = ButtonState.SaveOver;
        }
        else if (_cancelbuttonBounds.Contains(e.Location))
        {
            _buttonState = ButtonState.CancelOver;
        }
        else if (_expandcollapseBounds.Contains(e.Location))
        {
            Cursor = Cursors.Hand;
            _buttonState = ButtonState.ColapseExpandOver;
        }
        else if (_headerBounds.Contains(e.Location))
        {
            Cursor = Cursors.Hand;
            _buttonState = ButtonState.HeaderOver;
        }
        else
        {
            Cursor = Cursors.Default;
            _buttonState = ButtonState.None;
        }

        if (oldState != _buttonState)
        {
            Invalidate();
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (Enabled && (_buttonState == ButtonState.HeaderOver | _buttonState == ButtonState.ColapseExpandOver))
        {
            Collapse = !Collapse;
            CollapseOrExpand();
        }
        else
        {
            if (DesignMode)
                return;
        }

        base.OnMouseDown(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        if (DesignMode)
            return;

        Cursor = Cursors.Arrow;
        _buttonState = ButtonState.None;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.TextRenderingHint = TextRenderingHint.AntiAlias;

        if (Parent != null)
        {
            g.Clear(Parent.BackColor);
        }

        // card rectangle path
        var expansionPanelRectF = new RectangleF(ClientRectangle.Location, ClientRectangle.Size);
        expansionPanelRectF.X -= 0.5f;
        expansionPanelRectF.Y -= 0.5f;
        var expansionPanelPath = DrawHelper.CreateRoundRect(expansionPanelRectF, 2);

        // button shadow (blend with form shadow)
        DrawHelper.DrawSquareShadow(g, ClientRectangle);

        // Draw expansion panel
        if (!Enabled)
        {
            if (Parent != null)
            {
                using var disabledBrush = new SolidBrush(DrawHelper.BlendColor(Parent.BackColor, SkinManager.BackgroundDisabledColor, SkinManager.BackgroundDisabledColor.A));
                g.FillPath(disabledBrush, expansionPanelPath);
            }
        }
        // Mormal
        else
        {
            if ((_buttonState == ButtonState.HeaderOver | _buttonState == ButtonState.ColapseExpandOver) && Collapse)
            {
                var expansionPanelBorderRectF = new RectangleF(ClientRectangle.X + 1, ClientRectangle.Y + 1, ClientRectangle.Width - 2, ClientRectangle.Height - 2);
                expansionPanelBorderRectF.X -= 0.5f;
                expansionPanelBorderRectF.Y -= 0.5f;
                var expansionPanelBoarderPath = DrawHelper.CreateRoundRect(expansionPanelBorderRectF, 2);

                g.FillPath(SkinManager.ExpansionPanelFocusBrush, expansionPanelBoarderPath);
            }
            else
            {
                using SolidBrush normalBrush = new(SkinManager.BackgroundColor);
                g.FillPath(normalBrush, expansionPanelPath);
            }
        }

        // Calc text Rect
        var headerRect = new Rectangle(
            _leftrightPadding,
            (_headerHeight - _textHeaderHeight) / 2,
            TextRenderer.MeasureText(Title, Font).Width + _expansionPanelDefaultPadding,
            _textHeaderHeight);

        //Draw  headers
        using (var NativeText = new NativeTextRenderer(g))
        {
            // Draw header text
            NativeText.DrawTransparentText(
                Title,
                SkinManager.GetLogFontByType(FontType.Body1),
                Enabled ? SkinManager.TextHighEmphasisColor : SkinManager.TextDisabledOrHintColor,
                headerRect.Location,
                headerRect.Size,
                TextAlignFlags.Left | TextAlignFlags.Middle);
        }

        if (!string.IsNullOrEmpty(Description))
        {
            //Draw description header text 
            var headerDescriptionRect = new Rectangle(
                headerRect.Right + _expansionPanelDefaultPadding,
                (_headerHeight - _textHeaderHeight) / 2,
                _expandcollapseBounds.Left - (headerRect.Right + _expansionPanelDefaultPadding) - _expansionPanelDefaultPadding,
                _textHeaderHeight);

            using var NativeText = new NativeTextRenderer(g);
            NativeText.DrawTransparentText(
                Description,
                SkinManager.GetLogFontByType(FontType.Body1),
                 SkinManager.TextDisabledOrHintColor,
                headerDescriptionRect.Location,
                headerDescriptionRect.Size,
                TextAlignFlags.Left | TextAlignFlags.Middle);
        }

        if (ShowCollapseExpand)
        {
            using var formButtonsPen = new Pen(UseAccentColor && Enabled ? SkinManager.ColorScheme.AccentColor : SkinManager.TextDisabledOrHintColor, 2);
            if (Collapse)
            {
                //Draw Expand button
                var pth = new GraphicsPath();
                var topLeft = new PointF(_expandcollapseBounds.X + 6, _expandcollapseBounds.Y + 9);
                var midBottom = new PointF(_expandcollapseBounds.X + 12, _expandcollapseBounds.Y + 15);
                var topRight = new PointF(_expandcollapseBounds.X + 18, _expandcollapseBounds.Y + 9);
                pth.AddLine(topLeft, midBottom);
                pth.AddLine(topRight, midBottom);
                g.DrawPath(formButtonsPen, pth);
            }
            else
            {
                // Draw Collapse button
                var pth = new GraphicsPath();
                var bottomLeft = new PointF(_expandcollapseBounds.X + 6, _expandcollapseBounds.Y + 15);
                var midTop = new PointF(_expandcollapseBounds.X + 12, _expandcollapseBounds.Y + 9);
                var bottomRight = new PointF(_expandcollapseBounds.X + 18, _expandcollapseBounds.Y + 15);
                pth.AddLine(bottomLeft, midTop);
                pth.AddLine(bottomRight, midTop);
                g.DrawPath(formButtonsPen, pth);
            }
        }

        if (!Collapse && ShowValidationButtons)
        {
            //Draw divider
            g.DrawLine(new Pen(SkinManager.DividersColor, 1), new Point(0, Height - _footerHeight), new Point(Width, Height - _footerHeight));
        }
    }

    private void CollapseOrExpand()
    {
        if (Collapse)
        {
            _headerHeight = _headerHeightCollapse;
            Height = _headerHeightCollapse;
            Margin = new Padding(16, 1, 16, 0);

            PanelCollapse?.Invoke(this, new EventArgs());
        }
        else
        {
            _headerHeight = _headerHeightExpand;
            Height = _expandHeight;
            Margin = new Padding(16, 16, 16, 16);

            PanelExpand?.Invoke(this, new EventArgs());
        }

        Refresh();
    }

    private void UpdateRects()
    {
        if (!Collapse && ShowValidationButtons)
        {
            var _buttonWidth = TextRenderer.MeasureText(ValidationButtonText, SkinManager.GetFontByType(FontType.Button)).Width + 32;
            _savebuttonBounds = new Rectangle(Width - _buttonPadding - _buttonWidth, Height - _expansionPanelDefaultPadding - _footerButtonHeight, _buttonWidth, _footerButtonHeight);
            _buttonWidth = TextRenderer.MeasureText(CancelButtonText, SkinManager.GetFontByType(FontType.Button)).Width + 32;
            _cancelbuttonBounds = new Rectangle(_savebuttonBounds.Left - _buttonPadding - _buttonWidth, Height - _expansionPanelDefaultPadding - _footerButtonHeight, _buttonWidth, _footerButtonHeight);

            if (_validationButton != null)
            {
                _validationButton.Width = _savebuttonBounds.Width;
                _validationButton.Left = Width - _buttonPadding - _validationButton.Width;  //Button minimum width management
                _validationButton.Top = _savebuttonBounds.Top;
                _validationButton.Height = _savebuttonBounds.Height;
                _validationButton.Text = ValidationButtonText;
                _validationButton.Enabled = ValidationButtonEnable;
                _validationButton.UseAccentColor = UseAccentColor;
            }
            if (_cancelButton != null)
            {
                _cancelButton.Width = _cancelbuttonBounds.Width;
                _cancelButton.Left = _validationButton?.Left ?? 0 - _buttonPadding - _cancelbuttonBounds.Width;  //Button minimum width management
                _cancelButton.Top = _cancelbuttonBounds.Top;
                _cancelButton.Height = _cancelbuttonBounds.Height;
                _cancelButton.Text = CancelButtonText;
                _cancelButton.UseAccentColor = UseAccentColor;
            }
        }

        _validationButton?.Visible = ShowValidationButtons;

        _cancelButton?.Visible = ShowValidationButtons;
    }

    private enum ButtonState
    {
        SaveOver,
        CancelOver,
        ColapseExpandOver,
        HeaderOver,
        None
    }
}
