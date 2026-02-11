namespace MaterialSkin.Controls;

public partial class MaterialForm : Form, IMaterialControl
{
    private const int _borderWidth = 7;
    private const int _statusBarButtonWidth = 24;
    private const int _statusBarHeightDefault = 24;
    private const int _iconSize = 24;
    private const int _paddingMinimum = 3;
    private const int _titleLeftPadding = 72;
    private const int _actionBarPadding = 16;
    private const int _actionBarHeightDefault = 40;

    private readonly Cursor[] _resizeCursors = [Cursors.SizeNESW, Cursors.SizeWE, Cursors.SizeNWSE, Cursors.SizeWE, Cursors.SizeNS];
    private readonly MaterialDrawer _drawerControl = new();
    private readonly AnimationManager _clickAnimManager;
    private readonly Form _drawerOverlay = new();
    private readonly MaterialDrawerForm _drawerForm = new();

    private ResizeDirection _resizeDir;
    private ButtonState _buttonState = ButtonState.None;
    private Rectangle _drawerIconRect;
    private Point _animationSource;
    private Padding _originalPadding;

    // Drawer overlay and speed improvements
    private AnimationManager? _drawerShowHideAnimManager;
    private int _statusBarHeight = 24;
    private int _actionBarHeight = 40;

    public MaterialForm()
    {
        DrawerWidth = 200;
        DrawerIsOpen = false;
        DrawerShowIconsWhenHidden = false;
        DrawerAutoHide = true;
        DrawerAutoShow = false;
        DrawerIndicatorWidth = 0;
        DrawerHighlightWithAccent = true;
        DrawerBackgroundWithAccent = false;

        FormBorderStyle = FormBorderStyle.None;
        Sizable = true;
        DoubleBuffered = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        FormStyle = FormStyles.ActionBar_40;

        Padding = new Padding(_paddingMinimum, _statusBarHeight + _actionBarHeight, _paddingMinimum, _paddingMinimum);      //Keep space for resize by mouse

        _clickAnimManager = new AnimationManager()
        {
            AnimationType = AnimationType.EaseOut,
            Increment = 0.04
        };

        _clickAnimManager.OnAnimationProgress += (sender, e) => Invalidate();

        // Drawer
        Shown += (sender, e) =>
        {
            if (DesignMode || IsDisposed)
                return;

            AddDrawerOverlayForm();
        };
    }

    private Rectangle MinButtonBounds => new(ClientSize.Width - 3 * _statusBarButtonWidth, ClientRectangle.Y, _statusBarButtonWidth, _statusBarHeight);
    private Rectangle MaxButtonBounds => new(ClientSize.Width - 2 * _statusBarButtonWidth, ClientRectangle.Y, _statusBarButtonWidth, _statusBarHeight);
    private Rectangle XButtonBounds => new(ClientSize.Width - _statusBarButtonWidth, ClientRectangle.Y, _statusBarButtonWidth, _statusBarHeight);
    private Rectangle ActionBarBounds => new(ClientRectangle.X, ClientRectangle.Y + _statusBarHeight, ClientSize.Width, _actionBarHeight);
    private Rectangle DrawerButtonBounds => new(ClientRectangle.X + (MaterialSkinManager.Instance.FormPadding / 2) + 3, _statusBarHeight + (_actionBarHeight / 2) - (_actionBarHeightDefault / 2), _actionBarHeightDefault, _actionBarHeightDefault);
    private Rectangle StatusBarBounds => new(ClientRectangle.X, ClientRectangle.Y, ClientSize.Width, _statusBarHeight);

    private bool Maximized
    {
        get => WindowState == FormWindowState.Maximized;
        set
        {
            if (!MaximizeBox || !ControlBox)
                return;

            if (value)
            {
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                WindowState = FormWindowState.Normal;
            }
        }
    }

    [Browsable(false)]
    public Rectangle UserArea => new(ClientRectangle.X, ClientRectangle.Y + _statusBarHeight + _actionBarHeight, ClientSize.Width, ClientSize.Height - (_statusBarHeight + _actionBarHeight));

    [Browsable(false)]
    public Form DrawerOverlay => _drawerOverlay;

    [Browsable(false)]
    public MaterialDrawerForm DrawerForm => _drawerForm;

    [Category("Layout")]
    public bool Sizable { get; set; }

    [Category("Material Skin"), Browsable(true), DisplayName("Form Style"), DefaultValue(FormStyles.ActionBar_40)]
    public FormStyles FormStyle
    {
        get;
        set
        {
            if (field == value)
                return;

            field = value;
            RecalculateFormBoundaries();
        }
    }

    [Category("Drawer")]
    public bool DrawerShowIconsWhenHidden
    {
        get;
        set
        {
            if (field == value) return;

            field = value;
            if (_drawerControl == null)
                return;

            _drawerControl.ShowIconsWhenHidden = field;
            _drawerControl.Refresh();
        }
    }

    [Category("Drawer")]
    public int DrawerWidth { get; set; }

    [Category("Drawer")]
    public bool DrawerAutoHide { get; set => _drawerControl.AutoHide = field = value; }

    [Category("Drawer")]
    public bool DrawerAutoShow { get; set => _drawerControl.AutoShow = field = value; }

    [Category("Drawer")]
    public int DrawerIndicatorWidth { get; set; }

    [Category("Drawer")]
    public bool DrawerIsOpen
    {
        get;
        set
        {
            if (field == value)
                return;

            field = value;

            if (value)
            {
                _drawerControl?.Show();
            }
            else
            {
                _drawerControl?.Hide();
            }
        }
    }

    [Category("Drawer")]
    public bool DrawerUseColors
    {
        get;
        set
        {
            if (field == value)
                return;

            field = value;

            if (_drawerControl == null)
                return;

            _drawerControl.UseColors = value;
            _drawerControl.Refresh();
        }
    }

    [Category("Drawer")]
    public bool DrawerHighlightWithAccent
    {
        get;
        set
        {
            if (field == value)
                return;

            field = value;

            if (_drawerControl == null)
                return;

            _drawerControl.HighlightWithAccent = value;
            _drawerControl.Refresh();
        }
    }

    [Category("Drawer")]
    public bool DrawerBackgroundWithAccent
    {
        get;
        set
        {
            if (field == value)
                return;

            field = value;

            if (_drawerControl == null)
                return;

            _drawerControl.BackgroundWithAccent = value;
            _drawerControl.Refresh();
        }
    }

    [Category("Drawer")]
    public MaterialTabControl? DrawerTabControl { get; set; }

    [AllowNull]
    public override string Text { get => base.Text; set { base.Text = value; Invalidate(); } }

    protected void AddDrawerOverlayForm()
    {
        if (DrawerTabControl == null)
            return;

        // Form opacity fade animation;
        _drawerShowHideAnimManager = new AnimationManager
        {
            AnimationType = AnimationType.EaseInOut,
            Increment = 0.04
        };

        _drawerShowHideAnimManager.OnAnimationProgress += (sender, e) =>
        {
            _drawerOverlay.Opacity = (float)(_drawerShowHideAnimManager.GetProgress() * 0.55f);
        };

        var h = ClientSize.Height - StatusBarBounds.Height - ActionBarBounds.Height;
        var y = PointToScreen(Point.Empty).Y + StatusBarBounds.Height + ActionBarBounds.Height;

        // Overlay Form definitions
        _drawerOverlay.BackColor = Color.Black;
        _drawerOverlay.Opacity = 0;
        _drawerOverlay.MinimizeBox = false;
        _drawerOverlay.MaximizeBox = false;
        _drawerOverlay.Text = string.Empty;
        _drawerOverlay.ShowIcon = false;
        _drawerOverlay.ControlBox = false;
        _drawerOverlay.FormBorderStyle = FormBorderStyle.None;
        _drawerOverlay.Visible = true;
        _drawerOverlay.Size = new Size(ClientSize.Width, h);
        _drawerOverlay.Location = new Point(PointToScreen(Point.Empty).X, y);
        _drawerOverlay.ShowInTaskbar = false;
        _drawerOverlay.Owner = this;
        _drawerOverlay.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

        // Drawer Form definitions
        _drawerForm.BackColor = Color.LimeGreen;
        _drawerForm.TransparencyKey = Color.LimeGreen;
        _drawerForm.MinimizeBox = false;
        _drawerForm.MaximizeBox = false;
        _drawerForm.Text = string.Empty;
        _drawerForm.ShowIcon = false;
        _drawerForm.ControlBox = false;
        _drawerForm.FormBorderStyle = FormBorderStyle.None;
        _drawerForm.Visible = true;
        _drawerForm.Size = new Size(DrawerWidth, h);
        _drawerForm.Location = new Point(PointToScreen(Point.Empty).X, y);
        _drawerForm.ShowInTaskbar = false;
        _drawerForm.Owner = _drawerOverlay;
        _drawerForm.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

        // Add drawer to overlay form
        _drawerForm.Controls.Add(_drawerControl);
        _drawerControl.Location = new Point(0, 0);
        _drawerControl.Size = new Size(DrawerWidth, h);
        _drawerControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
        _drawerControl.BaseTabControl = DrawerTabControl;
        _drawerControl.ShowIconsWhenHidden = true;

        // Init Options
        _drawerControl.IsOpen = DrawerIsOpen;
        _drawerControl.ShowIconsWhenHidden = DrawerShowIconsWhenHidden;
        _drawerControl.AutoHide = DrawerAutoHide;
        _drawerControl.AutoShow = DrawerAutoShow;
        _drawerControl.IndicatorWidth = DrawerIndicatorWidth;
        _drawerControl.HighlightWithAccent = DrawerHighlightWithAccent;
        _drawerControl.BackgroundWithAccent = DrawerBackgroundWithAccent;

        // Changing colors or theme
        MaterialSkinManager.Instance.ThemeChanged += (sender, e) =>
        {
            _drawerForm.Refresh();
        };

        MaterialSkinManager.Instance.ColorSchemeChanged += (sender, e) =>
        {
            _drawerForm.Refresh();
        };

        // Visible, Resize and move events
        VisibleChanged += (sender, e) =>
        {
            _drawerForm.Visible = Visible;
            _drawerOverlay.Visible = Visible;
        };

        Resize += (sender, e) =>
        {
            h = ClientSize.Height - StatusBarBounds.Height - ActionBarBounds.Height;
            _drawerForm.Size = new Size(DrawerWidth, h);
            _drawerOverlay.Size = new Size(ClientSize.Width, h);
        };

        Move += (sender, e) =>
        {
            Point pos = new(PointToScreen(Point.Empty).X, PointToScreen(Point.Empty).Y + StatusBarBounds.Height + ActionBarBounds.Height);
            _drawerForm.Location = pos;
            _drawerOverlay.Location = pos;
        };

        // Close when click outside menu
        _drawerOverlay.Click += (sender, e) =>
        {
            _drawerControl.Hide();
        };

        //Resize form when mouse over drawer
        _drawerControl.MouseDown += (sender, e) =>
        {
            ResizeForm(_resizeDir);
        };

        // Animation and visibility
        _drawerControl.DrawerBeginOpen += (sender, e) =>
        {
            _drawerShowHideAnimManager.StartNewAnimation(AnimationDirection.In);
        };

        _drawerControl.DrawerBeginClose += (sender, e) =>
        {
            _drawerShowHideAnimManager.StartNewAnimation(AnimationDirection.Out);
        };

        _drawerControl.CursorUpdate += (sender, drawerCursor) =>
        {
            if (Sizable && !Maximized)
            {
                if (drawerCursor == Cursors.SizeNESW)
                {
                    _resizeDir = ResizeDirection.BottomLeft;
                }
                else if (drawerCursor == Cursors.SizeWE)
                {
                    _resizeDir = ResizeDirection.Left;
                }
                else if (drawerCursor == Cursors.SizeNS)
                {
                    _resizeDir = ResizeDirection.Bottom;
                }
                else
                {
                    _resizeDir = ResizeDirection.None;
                }
            }
            else
            {
                _resizeDir = ResizeDirection.None;
            }

            Cursor = drawerCursor;
        };

        // Form Padding corrections

        if (Padding.Top < (StatusBarBounds.Height + ActionBarBounds.Height))
        {
            Padding = new Padding(Padding.Left, StatusBarBounds.Height + ActionBarBounds.Height, Padding.Right, Padding.Bottom);
        }

        _originalPadding = Padding;

        _drawerControl.DrawerShowIconsWhenHiddenChanged += FixFormPadding;
        FixFormPadding(this, EventArgs.Empty);
    }

    private void FixFormPadding(object? sender, EventArgs e)
    {
        if (_drawerControl.ShowIconsWhenHidden)
        {
            Padding = new Padding(Padding.Left < _drawerControl.MinWidth ? _drawerControl.MinWidth : Padding.Left, _originalPadding.Top, _originalPadding.Right, _originalPadding.Bottom);
        }
        else
        {
            Padding = new Padding(_paddingMinimum, _originalPadding.Top, _originalPadding.Right, _originalPadding.Bottom);
        }
    }

    private void UpdateButtons(MouseButtons button, Point location, bool up = false)
    {
        if (DesignMode)
            return;

        var oldState = _buttonState;
        var showMin = MinimizeBox && ControlBox;
        var showMax = MaximizeBox && ControlBox;

        if (button == MouseButtons.Left && !up)
        {
            if (showMin && !showMax && MaxButtonBounds.Contains(location))
            {
                _buttonState = ButtonState.MinDown;
            }
            else if (showMin && showMax && MinButtonBounds.Contains(location))
            {
                _buttonState = ButtonState.MinDown;
            }
            else if (showMax && MaxButtonBounds.Contains(location))
            {
                _buttonState = ButtonState.MaxDown;
            }
            else if (ControlBox && XButtonBounds.Contains(location))
            {
                _buttonState = ButtonState.XDown;
            }
            else if (DrawerButtonBounds.Contains(location))
            {
                _buttonState = ButtonState.DrawerDown;
            }
            else
            {
                _buttonState = ButtonState.None;
            }
        }
        else
        {
            if (showMin && !showMax && MaxButtonBounds.Contains(location))
            {
                _buttonState = ButtonState.MinOver;

                if (oldState == ButtonState.MinDown && up)
                {
                    WindowState = FormWindowState.Minimized;
                }
            }
            else if (showMin && showMax && MinButtonBounds.Contains(location))
            {
                _buttonState = ButtonState.MinOver;

                if (oldState == ButtonState.MinDown && up)
                {
                    WindowState = FormWindowState.Minimized;
                }
            }
            else if (showMax && MaxButtonBounds.Contains(location))
            {
                _buttonState = ButtonState.MaxOver;

                if (oldState == ButtonState.MaxDown && up)
                {
                    Maximized = !Maximized;
                }
            }
            else if (ControlBox && XButtonBounds.Contains(location))
            {
                _buttonState = ButtonState.XOver;

                if (oldState == ButtonState.XDown && up)
                {
                    Close();
                }
            }
            else if (DrawerButtonBounds.Contains(location))
            {
                _buttonState = ButtonState.DrawerOver;
            }
            else
            {
                _buttonState = ButtonState.None;
            }
        }

        if (oldState != _buttonState)
        {
            Invalidate();
        }
    }

    private void ResizeForm(ResizeDirection direction)
    {
        if (DesignMode)
            return;

        var dir = -1;
        switch (direction)
        {
            case ResizeDirection.BottomLeft:
                dir = (int)HT.BottomLeft;
                Cursor = Cursors.SizeNESW;
                break;

            case ResizeDirection.Left:
                dir = (int)HT.Left;
                Cursor = Cursors.SizeWE;
                break;

            case ResizeDirection.Right:
                dir = (int)HT.Right;
                break;

            case ResizeDirection.BottomRight:
                dir = (int)HT.BottomRight;
                break;

            case ResizeDirection.Bottom:
                dir = (int)HT.Bottom;
                break;

            case ResizeDirection.Top:
                dir = (int)HT.Top;
                break;

            case ResizeDirection.TopLeft:
                dir = (int)HT.TopLeft;
                break;

            case ResizeDirection.TopRight:
                dir = (int)HT.TopRight;
                break;
        }

        Functions.ReleaseCapture();
        if (dir != -1)
        {
            Functions.SendMessageW(Handle, (int)WM.NonClientLeftButtonDown, dir, 0);
        }
    }

    private void RecalculateFormBoundaries()
    {
        switch (FormStyle)
        {
            case FormStyles.StatusAndActionBar_None:
                _actionBarHeight = 0;
                _statusBarHeight = 0;
                break;

            case FormStyles.ActionBar_None:
                _actionBarHeight = 0;
                _statusBarHeight = _statusBarHeightDefault;
                break;

            case FormStyles.ActionBar_40:
                _actionBarHeight = _actionBarHeightDefault;
                _statusBarHeight = _statusBarHeightDefault;
                break;

            case FormStyles.ActionBar_48:
                _actionBarHeight = 48;
                _statusBarHeight = _statusBarHeightDefault;
                break;

            case FormStyles.ActionBar_56:
                _actionBarHeight = 56;
                _statusBarHeight = _statusBarHeightDefault;
                break;

            case FormStyles.ActionBar_64:
                _actionBarHeight = 64;
                _statusBarHeight = _statusBarHeightDefault;
                break;

            default:
                _actionBarHeight = _actionBarHeightDefault;
                _statusBarHeight = _statusBarHeightDefault;
                break;
        }

        Padding = new Padding(DrawerShowIconsWhenHidden ? _drawerControl.MinWidth : _paddingMinimum, _statusBarHeight + _actionBarHeight, Padding.Right, Padding.Bottom);
        _originalPadding = Padding;

        if (DrawerTabControl != null)
        {
            var height = ClientSize.Height - (_statusBarHeight + _actionBarHeight);
            var location = Point.Add(Location, new Size(0, _statusBarHeight + _actionBarHeight));
            _drawerOverlay.Size = new Size(ClientSize.Width, height);
            _drawerOverlay.Location = location;
            _drawerForm.Size = new Size(DrawerWidth, height);
            _drawerForm.Location = location;
        }

        Invalidate();
    }

    protected override CreateParams CreateParams
    {
        get
        {
            var par = base.CreateParams;
            par.Style |= (int)WS.MinimizeBox | (int)WS.SysMenu;
            return par;
        }
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();

        // Sets the Window Style for having a Size Frame after the form is created
        // This prevents unexpected sizing while still allowing for Aero Snapping
        var flags = GetWindowLongPtr(Handle, WINDOW_LONG_PTR_INDEX.GWL_STYLE).ToInt64();
        SetWindowLongPtr(Handle, WINDOW_LONG_PTR_INDEX.GWL_STYLE, (nint)(flags | (int)WS.SizeFrame));
    }

    protected override void WndProc(ref Message m)
    {
        var message = (WM)m.Msg;

        // Prevent the base class from receiving the message
        if (message == WM.NonClientCalcSize)
            return;

        // https://docs.microsoft.com/en-us/windows/win32/winmsg/wm-ncactivate?redirectedfrom=MSDN#parameters
        // "If this parameter is set to -1, DefWindowProc does not repaint the nonclient area to reflect the state change."
        if (message == WM.NonClientActivate)
        {
            m.Result = new nint(-1);
            return;
        }

        base.WndProc(ref m);
        if (DesignMode || IsDisposed)
            return;

        var cursorPos = PointToClient(Cursor.Position);
        var isOverCaption = (StatusBarBounds.Contains(cursorPos) || ActionBarBounds.Contains(cursorPos)) &&
            !(MinButtonBounds.Contains(cursorPos) || MaxButtonBounds.Contains(cursorPos) || XButtonBounds.Contains(cursorPos));

        // Drawer
        if (DrawerTabControl != null && (message == WM.LeftButtonDown || message == WM.LeftButtonDoubleClick) && _drawerIconRect.Contains(cursorPos))
        {
            _drawerControl.Toggle();
            _clickAnimManager.SetProgress(0);
            _clickAnimManager.StartNewAnimation(AnimationDirection.In);
            _animationSource = cursorPos;
        }
        // Double click to maximize
        else if (message == WM.LeftButtonDoubleClick && isOverCaption)
        {
            Maximized = !Maximized;
        }
        // Treat the Caption as if it was Non-Client
        else if (message == WM.LeftButtonDown && isOverCaption)
        {
            Functions.ReleaseCapture();
            Functions.SendMessageW(Handle, (int)WM.NonClientLeftButtonDown, (int)HT.Caption, 0);
        }
        // Default context menu
        else if (message == WM.RightButtonDown)
        {
            if (StatusBarBounds.Contains(cursorPos) && !MinButtonBounds.Contains(cursorPos) &&
                !MaxButtonBounds.Contains(cursorPos) && !XButtonBounds.Contains(cursorPos))
            {
                // Temporary disable user defined ContextMenuStrip
                var user_cms = base.ContextMenuStrip;
                base.ContextMenuStrip = null;

                // Show default system menu when right clicking titlebar
                var id = Functions.TrackPopupMenuEx(Functions.GetSystemMenu(Handle, false), (int)TPM.LeftAlign | (int)TPM.ReturnCommand, Cursor.Position.X, Cursor.Position.Y, Handle, 0);

                // Pass the command as a WM_SYSCOMMAND message
                Functions.SendMessageW(Handle, (int)WM.SystemCommand, id, 0);

                // restore user defined ContextMenuStrip
                base.ContextMenuStrip = user_cms;
            }
        }
    }

    protected override void OnMove(EventArgs e)
    {
        // Empty Point ensures the screen maximizes to the top left of the current screen
        MaximizedBounds = new Rectangle(Point.Empty, Screen.GetWorkingArea(Location).Size);
        base.OnMove(e);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (DesignMode)
            return;

        UpdateButtons(e.Button, e.Location);
        if (e.Button == MouseButtons.Left && !Maximized && _resizeCursors.Contains(Cursor))
        {
            ResizeForm(_resizeDir);
        }

        base.OnMouseDown(e);
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        Cursor = Cursors.Default;
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        if (DesignMode)
            return;

        _buttonState = ButtonState.None;
        _resizeDir = ResizeDirection.None;
        //Only reset the cursor when needed
        if (_resizeCursors.Contains(Cursor))
        {
            Cursor = Cursors.Default;
        }

        Invalidate();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (DesignMode)
            return;

        var coords = e.Location;
        UpdateButtons(e.Button, coords);

        if (!Sizable)
            return;

        //True if the mouse is hovering over a child control
        var isChildUnderMouse = GetChildAtPoint(coords) != null;

        if (!isChildUnderMouse && !Maximized && coords.Y < _borderWidth && coords.X > _borderWidth && coords.X < ClientSize.Width - _borderWidth)
        {
            _resizeDir = ResizeDirection.Top;
            Cursor = Cursors.SizeNS;
        }
        else if (!isChildUnderMouse && !Maximized && coords.X <= _borderWidth && coords.Y < _borderWidth)
        {
            _resizeDir = ResizeDirection.TopLeft;
            Cursor = Cursors.SizeNWSE;
        }
        else if (!isChildUnderMouse && !Maximized && coords.X >= ClientSize.Width - _borderWidth && coords.Y < _borderWidth)
        {
            _resizeDir = ResizeDirection.TopRight;
            Cursor = Cursors.SizeNESW;
        }
        else if (!isChildUnderMouse && !Maximized && coords.X <= _borderWidth && coords.Y >= ClientSize.Height - _borderWidth)
        {
            _resizeDir = ResizeDirection.BottomLeft;
            Cursor = Cursors.SizeNESW;
        }
        else if ((!isChildUnderMouse || DrawerTabControl != null) && !Maximized && coords.X <= _borderWidth)
        {
            _resizeDir = ResizeDirection.Left;
            Cursor = Cursors.SizeWE;
        }
        else if (!isChildUnderMouse && !Maximized && coords.X >= ClientSize.Width - _borderWidth && coords.Y >= ClientSize.Height - _borderWidth)
        {
            _resizeDir = ResizeDirection.BottomRight;
            Cursor = Cursors.SizeNWSE;
        }
        else if (!isChildUnderMouse && !Maximized && coords.X >= ClientSize.Width - _borderWidth)
        {
            _resizeDir = ResizeDirection.Right;
            Cursor = Cursors.SizeWE;
        }
        else if (!isChildUnderMouse && !Maximized && coords.Y >= ClientSize.Height - _borderWidth)
        {
            _resizeDir = ResizeDirection.Bottom;
            Cursor = Cursors.SizeNS;
        }
        else
        {
            _resizeDir = ResizeDirection.None;

            //Only reset the cursor when needed, this prevents it from flickering when a child control changes the cursor to its own needs
            if (_resizeCursors.Contains(Cursor))
                Cursor = Cursors.Default;
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        if (DesignMode)
            return;

        UpdateButtons(e.Button, e.Location, true);
        base.OnMouseUp(e);
        Functions.ReleaseCapture();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var hoverBrush = MaterialSkinManager.Instance.BackgroundHoverBrush;
        var downBrush = MaterialSkinManager.Instance.BackgroundFocusBrush;
        var g = e.Graphics;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        g.Clear(MaterialSkinManager.Instance.BackdropColor);

        //Draw border
        using (var borderPen = new Pen(MaterialSkinManager.Instance.DividersColor, 1))
        {
            g.DrawLine(borderPen, new Point(0, ActionBarBounds.Bottom), new Point(0, ClientSize.Height - 2));
            g.DrawLine(borderPen, new Point(ClientSize.Width - 1, ActionBarBounds.Bottom), new Point(ClientSize.Width - 1, ClientSize.Height - 2));
            g.DrawLine(borderPen, new Point(0, ClientSize.Height - 1), new Point(ClientSize.Width - 1, ClientSize.Height - 1));
        }

        if (FormStyle != FormStyles.StatusAndActionBar_None)
        {
            if (ControlBox)
            {
                g.FillRectangle(MaterialSkinManager.Instance.ColorScheme.DarkPrimaryBrush, StatusBarBounds);
                g.FillRectangle(MaterialSkinManager.Instance.ColorScheme.PrimaryBrush, ActionBarBounds);
            }

            // Determine whether or not we even should be drawing the buttons.
            var showMin = MinimizeBox && ControlBox;
            var showMax = MaximizeBox && ControlBox;

            // When MaximizeButton == false, the minimize button will be painted in its place
            if (_buttonState == ButtonState.MinOver && showMin)
            {
                g.FillRectangle(hoverBrush, showMax ? MinButtonBounds : MaxButtonBounds);
            }

            if (_buttonState == ButtonState.MinDown && showMin)
            {
                g.FillRectangle(downBrush, showMax ? MinButtonBounds : MaxButtonBounds);
            }

            if (_buttonState == ButtonState.MaxOver && showMax)
            {
                g.FillRectangle(hoverBrush, MaxButtonBounds);
            }

            if (_buttonState == ButtonState.MaxDown && showMax)
            {
                g.FillRectangle(downBrush, MaxButtonBounds);
            }

            if (_buttonState == ButtonState.XOver && ControlBox)
            {
                g.FillRectangle(MaterialSkinManager.Instance.BackgroundHoverRedBrush, XButtonBounds);
            }

            if (_buttonState == ButtonState.XDown && ControlBox)
            {
                g.FillRectangle(MaterialSkinManager.Instance.BackgroundDownRedBrush, XButtonBounds);
            }

            using var formButtonsPen = new Pen(MaterialSkinManager.Instance.ColorScheme.TextColor, 2);
            // Minimize button.
            if (showMin)
            {
                var x = showMax ? MinButtonBounds.X : MaxButtonBounds.X;
                var y = showMax ? MinButtonBounds.Y : MaxButtonBounds.Y;

                g.DrawLine(
                    formButtonsPen,
                    x + (int)(MinButtonBounds.Width * 0.33),
                    y + (int)(MinButtonBounds.Height * 0.66),
                    x + (int)(MinButtonBounds.Width * 0.66),
                    y + (int)(MinButtonBounds.Height * 0.66)
               );
            }

            // Maximize button
            if (showMax)
            {
                if (WindowState != FormWindowState.Maximized)
                {
                    g.DrawRectangle(
                        formButtonsPen,
                        MaxButtonBounds.X + (int)(MaxButtonBounds.Width * 0.33),
                        MaxButtonBounds.Y + (int)(MaxButtonBounds.Height * 0.36),
                        (int)(MaxButtonBounds.Width * 0.39),
                        (int)(MaxButtonBounds.Height * 0.31)
                    );
                }
                else
                {
                    // Change position of square
                    g.DrawRectangle(
                        formButtonsPen,
                        MaxButtonBounds.X + (int)(MaxButtonBounds.Width * 0.30),
                        MaxButtonBounds.Y + (int)(MaxButtonBounds.Height * 0.42),
                        (int)(MaxButtonBounds.Width * 0.40),
                        (int)(MaxButtonBounds.Height * 0.32)
                    );

                    // Draw lines for background square
                    g.DrawLine(formButtonsPen,
                        MaxButtonBounds.X + (int)(MaxButtonBounds.Width * 0.42),
                        MaxButtonBounds.Y + (int)(MaxButtonBounds.Height * 0.30),
                        MaxButtonBounds.X + (int)(MaxButtonBounds.Width * 0.42),
                        MaxButtonBounds.Y + (int)(MaxButtonBounds.Height * 0.38)
                    );

                    g.DrawLine(formButtonsPen,
                        MaxButtonBounds.X + (int)(MaxButtonBounds.Width * 0.40),
                        MaxButtonBounds.Y + (int)(MaxButtonBounds.Height * 0.30),
                        MaxButtonBounds.X + (int)(MaxButtonBounds.Width * 0.86),
                        MaxButtonBounds.Y + (int)(MaxButtonBounds.Width * 0.30)
                    );

                    g.DrawLine(formButtonsPen,
                        MaxButtonBounds.X + (int)(MaxButtonBounds.Width * 0.82),
                        MaxButtonBounds.Y + (int)(MaxButtonBounds.Height * 0.28),
                        MaxButtonBounds.X + (int)(MaxButtonBounds.Width * 0.82),
                        MaxButtonBounds.Y + (int)(MaxButtonBounds.Width * 0.64)
                    );

                    g.DrawLine(formButtonsPen,
                        MaxButtonBounds.X + (int)(MaxButtonBounds.Width * 0.70),
                        MaxButtonBounds.Y + (int)(MaxButtonBounds.Height * 0.62),
                        MaxButtonBounds.X + (int)(MaxButtonBounds.Width * 0.84),
                        MaxButtonBounds.Y + (int)(MaxButtonBounds.Width * 0.62)
                    );
                }
            }

            // Close button
            if (ControlBox)
            {
                g.DrawLine(
                    formButtonsPen,
                    XButtonBounds.X + (int)(XButtonBounds.Width * 0.33),
                    XButtonBounds.Y + (int)(XButtonBounds.Height * 0.33),
                    XButtonBounds.X + (int)(XButtonBounds.Width * 0.66),
                    XButtonBounds.Y + (int)(XButtonBounds.Height * 0.66)
               );

                g.DrawLine(
                    formButtonsPen,
                    XButtonBounds.X + (int)(XButtonBounds.Width * 0.66),
                    XButtonBounds.Y + (int)(XButtonBounds.Height * 0.33),
                    XButtonBounds.X + (int)(XButtonBounds.Width * 0.33),
                    XButtonBounds.Y + (int)(XButtonBounds.Height * 0.66));
            }
        }

        // Drawer Icon
        if (DrawerTabControl != null && FormStyle != FormStyles.ActionBar_None && FormStyle != FormStyles.StatusAndActionBar_None)
        {
            if (_buttonState == ButtonState.DrawerOver)
            {
                g.FillRectangle(hoverBrush, DrawerButtonBounds);
            }

            if (_buttonState == ButtonState.DrawerDown)
            {
                g.FillRectangle(downBrush, DrawerButtonBounds);
            }

            _drawerIconRect = new Rectangle(MaterialSkinManager.Instance.FormPadding / 2, _statusBarHeight, _actionBarHeightDefault, _actionBarHeight);
            // Ripple
            if (_clickAnimManager.IsAnimating())
            {
                var clickAnimProgress = _clickAnimManager.GetProgress();
                using var rippleBrush = new SolidBrush(Color.FromArgb((int)(51 - (clickAnimProgress * 50)), Color.White));
                var rippleSize = (int)(clickAnimProgress * _drawerIconRect.Width * 1.75);

                g.SetClip(_drawerIconRect);
                g.FillEllipse(rippleBrush, new Rectangle(_animationSource.X - rippleSize / 2, _animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
                g.ResetClip();
            }

            using var formButtonsPen = new Pen(MaterialSkinManager.Instance.ColorScheme.TextColor, 2);
            // Middle line
            g.DrawLine(
               formButtonsPen,
               _drawerIconRect.X + MaterialSkinManager.Instance.FormPadding,
               _drawerIconRect.Y + _actionBarHeight / 2,
               _drawerIconRect.X + MaterialSkinManager.Instance.FormPadding + 18,
               _drawerIconRect.Y + _actionBarHeight / 2);

            // Bottom line
            g.DrawLine(
               formButtonsPen,
               _drawerIconRect.X + MaterialSkinManager.Instance.FormPadding,
               _drawerIconRect.Y + _actionBarHeight / 2 - 6,
               _drawerIconRect.X + MaterialSkinManager.Instance.FormPadding + 18,
               _drawerIconRect.Y + _actionBarHeight / 2 - 6);

            // Top line
            g.DrawLine(
               formButtonsPen,
               _drawerIconRect.X + MaterialSkinManager.Instance.FormPadding,
               _drawerIconRect.Y + _actionBarHeight / 2 + 6,
               _drawerIconRect.X + MaterialSkinManager.Instance.FormPadding + 18,
               _drawerIconRect.Y + _actionBarHeight / 2 + 6);
        }

        if (ControlBox && FormStyle != FormStyles.ActionBar_None && FormStyle != FormStyles.StatusAndActionBar_None)
        {
            //Form title
            using var NativeText = new NativeTextRenderer(g);
            var textLocation = new Rectangle(DrawerTabControl != null ? _titleLeftPadding : _titleLeftPadding - (_iconSize + (_actionBarPadding * 2)), _statusBarHeight, ClientSize.Width, _actionBarHeight);
            NativeText.DrawTransparentText(Text, MaterialSkinManager.Instance.GetLogFontByType(FontType.H6),
                MaterialSkinManager.Instance.ColorScheme.TextColor,
                textLocation.Location,
                textLocation.Size,
                TextAlignFlags.Left | TextAlignFlags.Middle);
        }
    }

    private static nint GetWindowLongPtr(nint hWnd, WINDOW_LONG_PTR_INDEX nIndex)
    {
        if (nint.Size == 8)
            return Functions.GetWindowLongPtrW(hWnd, nIndex);

        return Functions.GetWindowLongW(hWnd, nIndex);
    }

    private static nint SetWindowLongPtr(nint hWnd, WINDOW_LONG_PTR_INDEX nIndex, nint dwNewLong)
    {
        if (nint.Size == 8)
            return Functions.SetWindowLongPtrW(hWnd, nIndex, dwNewLong);

        return Functions.SetWindowLongW(hWnd, nIndex, dwNewLong.ToInt32());
    }

    /// <summary>
    /// Various directions the form can be resized in
    /// </summary>
    private enum ResizeDirection
    {
        BottomLeft,
        Left,
        Right,
        BottomRight,
        Bottom,
        Top,
        TopLeft,
        TopRight,
        None
    }

    /// <summary>
    /// The states a button can be in
    /// </summary>
    private enum ButtonState
    {
        XOver,
        MaxOver,
        MinOver,
        DrawerOver,
        XDown,
        MaxDown,
        MinDown,
        DrawerDown,
        None
    }

    /// <summary>
    /// Window Messages
    /// <see href="https://docs.microsoft.com/en-us/windows/win32/winmsg/about-messages-and-message-queues"/>
    /// </summary>
    private enum WM
    {
        /// <summary>
        /// WM_NCCALCSIZE
        /// </summary>
        NonClientCalcSize = 0x0083,
        /// <summary>
        /// WM_NCACTIVATE
        /// </summary>
        NonClientActivate = 0x0086,
        /// <summary>
        /// WM_NCLBUTTONDOWN
        /// </summary>
        NonClientLeftButtonDown = 0x00A1,
        /// <summary>
        /// WM_SYSCOMMAND
        /// </summary>
        SystemCommand = 0x0112,
        /// <summary>
        /// WM_MOUSEMOVE
        /// </summary>
        MouseMove = 0x0200,
        /// <summary>
        /// WM_LBUTTONDOWN
        /// </summary>
        LeftButtonDown = 0x0201,
        /// <summary>
        /// WM_LBUTTONUP
        /// </summary>
        LeftButtonUp = 0x0202,
        /// <summary>
        /// WM_LBUTTONDBLCLK
        /// </summary>
        LeftButtonDoubleClick = 0x0203,
        /// <summary>
        /// WM_RBUTTONDOWN
        /// </summary>
        RightButtonDown = 0x0204,
    }

    /// <summary>
    /// Hit Test Results
    /// <see href="https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-nchittest"/>
    /// </summary>
    private enum HT
    {
        /// <summary>
        /// HTNOWHERE - Nothing under cursor
        /// </summary>
        None = 0,
        /// <summary>
        /// HTCAPTION - Titlebar
        /// </summary>
        Caption = 2,
        /// <summary>
        /// HTLEFT - Left border
        /// </summary>
        Left = 10,
        /// <summary>
        /// HTRIGHT - Right border
        /// </summary>
        Right = 11,
        /// <summary>
        /// HTTOP - Top border
        /// </summary>
        Top = 12,
        /// <summary>
        /// HTTOPLEFT - Top left corner
        /// </summary>
        TopLeft = 13,
        /// <summary>
        /// HTTOPRIGHT - Top right corner
        /// </summary>
        TopRight = 14,
        /// <summary>
        /// HTBOTTOM - Bottom border
        /// </summary>
        Bottom = 15,
        /// <summary>
        /// HTBOTTOMLEFT - Bottom left corner
        /// </summary>
        BottomLeft = 16,
        /// <summary>
        /// HTBOTTOMRIGHT - Bottom right corner
        /// </summary>
        BottomRight = 17,
    }

    /// <summary>
    /// Window Styles
    /// <see href="https://docs.microsoft.com/en-us/windows/win32/winmsg/window-styles"/>
    /// </summary>
    private enum WS
    {
        /// <summary>
        /// WS_MINIMIZEBOX - Allow minimizing from taskbar
        /// </summary>
        MinimizeBox = 0x20000,
        /// <summary>
        /// WS_SIZEFRAME - Required for Aero Snapping
        /// </summary>
        SizeFrame = 0x40000,
        /// <summary>
        /// WS_SYSMENU - Trigger the creation of the system menu
        /// </summary>
        SysMenu = 0x80000,
    }

    /// <summary>
    /// Track Popup Menu Flags
    /// <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-trackpopupmenu"/>
    /// </summary>
    private enum TPM
    {
        /// <summary>
        /// TPM_LEFTALIGN
        /// </summary>
        LeftAlign = 0x0000,
        /// <summary>
        /// TPM_RETURNCMD
        /// </summary>
        ReturnCommand = 0x0100,
    }
}
