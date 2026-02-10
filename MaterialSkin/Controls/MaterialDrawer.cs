namespace MaterialSkin.Controls;

public class MaterialDrawer : Control, IMaterialControl
{
    private const int _tabHeaderPadding = 24;
    private const int _borderWidth = 7;
    private bool _isOpen;
    private Dictionary<string, TextureBrush>? _iconsBrushes;
    private Dictionary<string, TextureBrush>? _iconsSelectedBrushes;
    private Dictionary<string, Rectangle>? _iconsSize;
    private int _prevLocation;
    private int _rippleSize = 0;
    private int _previousSelectedTabIndex;
    private Point _animationSource;
    private readonly AnimationManager _clickAnimManager;
    private readonly AnimationManager _showHideAnimManager;
    private List<Rectangle>? _drawerItemRects;
    private List<GraphicsPath>? _drawerItemPaths;
    private int _drawerItemHeight;
    private int _lastMouseY;
    private int _lastLocationY;

    public event EventHandler? DrawerStateChanged;
    public event EventHandler? DrawerBeginOpen;
    public event EventHandler? DrawerEndOpen;
    public event EventHandler? DrawerBeginClose;
    public event EventHandler? DrawerEndClose;
    public event EventHandler? DrawerShowIconsWhenHiddenChanged;
    public event EventHandler<Cursor>? CursorUpdate;

    public MaterialDrawer()
    {
        SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        Height = 120;
        Width = 250;
        IndicatorWidth = 0;
        _isOpen = false;
        ShowIconsWhenHidden = false;
        AutoHide = false;
        AutoShow = false;
        HighlightWithAccent = true;
        BackgroundWithAccent = false;

        _showHideAnimManager = new AnimationManager
        {
            AnimationType = AnimationType.EaseInOut,
            Increment = 0.04
        };

        _showHideAnimManager.OnAnimationProgress += (sender, e) =>
        {
            Invalidate();
            ShowHideAnimation();
        };

        _showHideAnimManager.OnAnimationFinished += (sender, e) =>
        {
            if (BaseTabControl != null && _drawerItemRects?.Count > 0)
            {
                _rippleSize = _drawerItemRects[BaseTabControl.SelectedIndex].Width;
            }

            if (_isOpen)
            {
                DrawerEndOpen?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                DrawerEndClose?.Invoke(this, EventArgs.Empty);
            }
        };

        SkinManager.ColorSchemeChanged += (sender, e) =>
        {
            PreProcessIcons();
        };

        SkinManager.ThemeChanged += (sender, e) =>
        {
            PreProcessIcons();
        };

        _clickAnimManager = new AnimationManager
        {
            AnimationType = AnimationType.EaseOut,
            Increment = 0.04
        };

        _clickAnimManager.OnAnimationProgress += (sender, e) => Invalidate();

        MouseWheel += MaterialDrawer_MouseWheel;
    }

    public int MinWidth { get; private set; }

    [Category("Drawer")]
    public bool ShowIconsWhenHidden
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                UpdateTabRects();
                PreProcessIcons();
                ShowHideAnimation();
                Paint(new PaintEventArgs(CreateGraphics(), ClientRectangle));
                DrawerShowIconsWhenHiddenChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    [Category("Drawer")]
    public bool IsOpen
    {
        get => _isOpen;
        set
        {
            _isOpen = value;
            if (value)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
    }

    [Category("Drawer")]
    public bool AutoHide { get; set; }

    [Category("Drawer")]
    public bool AutoShow { get; set; }

    [Category("Drawer")]
    public bool UseColors
    {
        get;
        set
        {
            field = value;
            PreProcessIcons();
            Invalidate();
        }
    }

    [Category("Drawer")]
    public bool HighlightWithAccent
    {
        get;
        set
        {
            field = value;
            PreProcessIcons();
            Invalidate();
        }
    }

    [Category("Drawer")]
    public bool BackgroundWithAccent
    {
        get;
        set
        {
            field = value;
            Invalidate();
        }
    }

    [Category("Drawer")]
    public int IndicatorWidth { get; set; }

    [Browsable(false)]
    public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

    [Browsable(false)]
    public MouseState MouseState { get; set; }

    [Category("Behavior")]
    public MaterialTabControl? BaseTabControl
    {
        get;
        set
        {
            field = value;
            if (field == null)
                return;

            UpdateTabRects();
            PreProcessIcons();

            // Other helpers

            _previousSelectedTabIndex = field.SelectedIndex;
            field.Deselected += (sender, args) =>
            {
                _previousSelectedTabIndex = field.SelectedIndex;
            };

            field.SelectedIndexChanged += (sender, args) =>
            {
                _clickAnimManager.SetProgress(0);
                _clickAnimManager.StartNewAnimation(AnimationDirection.In);
            };

            field.ControlAdded += (s, e) => Invalidate();
            field.ControlRemoved += (s, e) => Invalidate();
        }
    }

    private void PreProcessIcons()
    {
        // pre-process and pre-allocate texture brushes (icons)
        if (BaseTabControl == null || BaseTabControl.TabCount == 0 || BaseTabControl.ImageList == null || _drawerItemRects == null || _drawerItemRects.Count == 0)
            return;

        // Calculate lightness and color
        var l = UseColors ? SkinManager.ColorScheme.TextColor.R / 255 : SkinManager.Theme == Themes.LIGHT ? 0f : 1f;
        var r = (HighlightWithAccent ? SkinManager.ColorScheme.AccentColor.R : SkinManager.ColorScheme.PrimaryColor.R) / 255f;
        var g = (HighlightWithAccent ? SkinManager.ColorScheme.AccentColor.G : SkinManager.ColorScheme.PrimaryColor.G) / 255f;
        var b = (HighlightWithAccent ? SkinManager.ColorScheme.AccentColor.B : SkinManager.ColorScheme.PrimaryColor.B) / 255f;

        // Create matrices
        float[][] matrixGray = [
                [0,   0,   0,   0,  0], // Red scale factor
                [0,   0,   0,   0,  0], // Green scale factor
                [0,   0,   0,   0,  0], // Blue scale factor
                [0,   0,   0, .7f,  0], // alpha scale factor
                [l,   l,   l,   0,  1]];// offset

        float[][] matrixColor = [
                [0,   0,   0,   0,  0], // Red scale factor
                [0,   0,   0,   0,  0], // Green scale factor
                [0,   0,   0,   0,  0], // Blue scale factor
                [0,   0,   0,   1,  0], // alpha scale factor
                [r,   g,   b,   0,  1]];// offset

        var colorMatrixGray = new ColorMatrix(matrixGray);
        var colorMatrixColor = new ColorMatrix(matrixColor);

        var grayImageAttributes = new ImageAttributes();
        var colorImageAttributes = new ImageAttributes();

        // Set color matrices
        grayImageAttributes.SetColorMatrix(colorMatrixGray, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        colorImageAttributes.SetColorMatrix(colorMatrixColor, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

        // Create brushes
        _iconsBrushes = new Dictionary<string, TextureBrush>(BaseTabControl.TabPages.Count);
        _iconsSelectedBrushes = new Dictionary<string, TextureBrush>(BaseTabControl.TabPages.Count);
        _iconsSize = new Dictionary<string, Rectangle>(BaseTabControl.TabPages.Count);

        foreach (TabPage tabPage in BaseTabControl.TabPages)
        {
            // skip items without image
            if (string.IsNullOrEmpty(tabPage.ImageKey) || _drawerItemRects == null)
                continue;

            if (BaseTabControl.ImageList == null)
                continue;

            var img = BaseTabControl.ImageList.Images[tabPage.ImageKey];
            if (img == null)
                continue;

            // Image Rect
            var destRect = new Rectangle(0, 0, img.Width, img.Height);

            // Create a pre-processed copy of the image (GRAY)
            var bgray = new Bitmap(destRect.Width, destRect.Height);
            using (var gGray = Graphics.FromImage(bgray))
            {
                gGray.DrawImage(img,
                    [
                            new Point(0, 0),
                            new Point(destRect.Width, 0),
                            new Point(0, destRect.Height),
                    ],
                    destRect, GraphicsUnit.Pixel, grayImageAttributes);
            }

            // Create a pre-processed copy of the image (PRIMARY COLOR)
            var bcolor = new Bitmap(destRect.Width, destRect.Height);
            using (var gColor = Graphics.FromImage(bcolor))
            {
                gColor.DrawImage(img,
                    [
                            new Point(0, 0),
                            new Point(destRect.Width, 0),
                            new Point(0, destRect.Height),
                    ],
                    destRect, GraphicsUnit.Pixel, colorImageAttributes);
            }

            // added processed image to brush for drawing
            var textureBrushGray = new TextureBrush(bgray);
            var textureBrushColor = new TextureBrush(bcolor);

            textureBrushGray.WrapMode = WrapMode.Clamp;
            textureBrushColor.WrapMode = WrapMode.Clamp;

            // Translate the brushes to the correct positions
            var currentTabIndex = BaseTabControl.TabPages.IndexOf(tabPage);

            var iconRect = new Rectangle(
               _drawerItemRects[currentTabIndex].X + (_drawerItemHeight / 2) - (img.Width / 2),
               _drawerItemRects[currentTabIndex].Y + (_drawerItemHeight / 2) - (img.Height / 2),
               img.Width, img.Height);

            textureBrushGray.TranslateTransform(iconRect.X + iconRect.Width / 2 - img.Width / 2,
                                                iconRect.Y + iconRect.Height / 2 - img.Height / 2);
            textureBrushColor.TranslateTransform(iconRect.X + iconRect.Width / 2 - img.Width / 2,
                                                 iconRect.Y + iconRect.Height / 2 - img.Height / 2);

            // add to dictionary
            var ik = string.Concat(tabPage.ImageKey, "_", tabPage.Name);
            _iconsBrushes.Add(ik, textureBrushGray);
            _iconsSelectedBrushes.Add(ik, textureBrushColor);
            _iconsSize.Add(ik, new Rectangle(0, 0, iconRect.Width, iconRect.Height));
        }
    }

    private void MaterialDrawer_MouseWheel(object? sender, MouseEventArgs e)
    {
        var step = 20;
        if (e.Delta > 0)
        {
            if (Location.Y < 0)
            {
                Location = new Point(Location.X, Location.Y + step > 0 ? 0 : Location.Y + step);
                Height = Location.Y + step > 0 ? Parent?.Height ?? 0 : Height - step;
            }
        }
        else
        {
            if (Height < (8 + _drawerItemHeight) * (_drawerItemRects?.Count ?? 0))
            {
                Location = new Point(Location.X, Location.Y - step);
                Height += step;
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void InitLayout()
    {
        _drawerItemHeight = _tabHeaderPadding * 2 - SkinManager.FORM_PADDING / 2;
        MinWidth = (int)(SkinManager.FORM_PADDING * 1.5 + _drawerItemHeight);
        _showHideAnimManager.SetProgress(_isOpen ? 0 : 1);
        ShowHideAnimation();
        Invalidate();
        base.InitLayout();
    }

    private void ShowHideAnimation()
    {
        var showHideAnimProgress = _showHideAnimManager.GetProgress();
        if (_showHideAnimManager.IsAnimating())
        {
            if (ShowIconsWhenHidden)
            {
                Location = new Point((int)((-Width + MinWidth) * showHideAnimProgress), Location.Y);
            }
            else
            {
                Location = new Point((int)(-Width * showHideAnimProgress), Location.Y);
            }
        }
        else
        {
            if (_isOpen)
            {
                Location = new Point(0, Location.Y);
            }
            else
            {
                if (ShowIconsWhenHidden)
                {
                    Location = new Point(-Width + MinWidth, Location.Y);
                }
                else
                {
                    Location = new Point(-Width, Location.Y);
                }
            }
        }
        UpdateTabRects();
    }

    protected override void OnPaint(PaintEventArgs e) => Paint(e);
    private new void Paint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        // redraw stuff
        g.Clear(UseColors ? SkinManager.ColorScheme.PrimaryColor : SkinManager.BackdropColor);

        if (BaseTabControl == null)
            return;

        if (!_clickAnimManager.IsAnimating() || _drawerItemRects == null || _drawerItemRects.Count != BaseTabControl.TabCount)
            UpdateTabRects();

        if (_drawerItemRects == null || _drawerItemRects.Count != BaseTabControl.TabCount)
            return;

        // Click Animation
        var clickAnimProgress = _clickAnimManager.GetProgress();
        // Show/Hide Drawer Animation
        var showHideAnimProgress = _showHideAnimManager.GetProgress();
        var rSize = (int)(clickAnimProgress * _rippleSize * 1.75);

        var dx = _prevLocation - Location.X;
        _prevLocation = Location.X;

        // Ripple
        if (_clickAnimManager.IsAnimating())
        {
            var rippleBrush = new SolidBrush(Color.FromArgb((int)(70 - (clickAnimProgress * 70)),
                UseColors ? SkinManager.ColorScheme.AccentColor : // Using colors
                SkinManager.Theme == Themes.LIGHT ? SkinManager.ColorScheme.PrimaryColor : // light theme
                SkinManager.ColorScheme.LightPrimaryColor)); // dark theme

            if (_drawerItemPaths != null && BaseTabControl.SelectedIndex < _drawerItemPaths.Count)
            {
                g.SetClip(_drawerItemPaths[BaseTabControl.SelectedIndex]);
            }

            g.FillEllipse(rippleBrush, new Rectangle(_animationSource.X + dx - (rSize / 2), _animationSource.Y - rSize / 2, rSize, rSize));
            g.ResetClip();
            rippleBrush.Dispose();
        }

        // Draw menu items
        foreach (TabPage tabPage in BaseTabControl.TabPages)
        {
            var currentTabIndex = BaseTabControl.TabPages.IndexOf(tabPage);

            // Background
            using var bgBrush = new SolidBrush(Color.FromArgb(CalculateAlpha(60, 0, currentTabIndex, clickAnimProgress),
                UseColors ? BackgroundWithAccent ? SkinManager.ColorScheme.AccentColor : SkinManager.ColorScheme.LightPrimaryColor : // using colors
                BackgroundWithAccent ? SkinManager.ColorScheme.AccentColor : // defaul accent
                SkinManager.Theme == Themes.LIGHT ? SkinManager.ColorScheme.PrimaryColor : // default light
                SkinManager.ColorScheme.LightPrimaryColor)); // default dark

            if (_drawerItemPaths != null && currentTabIndex < _drawerItemPaths.Count)
            {
                g.FillPath(bgBrush, _drawerItemPaths[currentTabIndex]);
            }

            // Text
            var textColor = Color.FromArgb(CalculateAlphaZeroWhenClosed(SkinManager.TextHighEmphasisColor.A, UseColors ? SkinManager.TextMediumEmphasisColor.A : 255, currentTabIndex, clickAnimProgress, 1 - showHideAnimProgress), // alpha
                UseColors ? (currentTabIndex == BaseTabControl.SelectedIndex ? (HighlightWithAccent ? SkinManager.ColorScheme.AccentColor : SkinManager.ColorScheme.PrimaryColor) // Use colors - selected
                : SkinManager.ColorScheme.TextColor) :  // Use colors - not selected
                (currentTabIndex == BaseTabControl.SelectedIndex ? (HighlightWithAccent ? SkinManager.ColorScheme.AccentColor : SkinManager.ColorScheme.PrimaryColor) : // selected
                SkinManager.TextHighEmphasisColor));

            var textFont = SkinManager.GetLogFontByType(FontType.Subtitle2);

            var textRect = _drawerItemRects[currentTabIndex];
            textRect.X += BaseTabControl.ImageList != null ? _drawerItemHeight : (int)(SkinManager.FORM_PADDING * 0.75);
            textRect.Width -= SkinManager.FORM_PADDING << 2;

            using (var NativeText = new NativeTextRenderer(g))
            {
                NativeText.DrawTransparentText(tabPage.Text, textFont, textColor, textRect.Location, textRect.Size, TextAlignFlags.Left | TextAlignFlags.Middle);
            }

            // Icons
            if (BaseTabControl.ImageList != null && !string.IsNullOrEmpty(tabPage.ImageKey) &&
                _iconsBrushes != null && _iconsSelectedBrushes != null && _iconsSize != null)
            {
                var ik = string.Concat(tabPage.ImageKey, "_", tabPage.Name);
                var iconRect = new Rectangle(
                    _drawerItemRects[currentTabIndex].X + (_drawerItemHeight >> 1) - (_iconsSize[ik].Width >> 1),
                    _drawerItemRects[currentTabIndex].Y + (_drawerItemHeight >> 1) - (_iconsSize[ik].Height >> 1),
                    _iconsSize[ik].Width, _iconsSize[ik].Height);

                if (ShowIconsWhenHidden)
                {
                    _iconsBrushes[ik].TranslateTransform(dx, 0);
                    _iconsSelectedBrushes[ik].TranslateTransform(dx, 0);
                }

                g.FillRectangle(currentTabIndex == BaseTabControl.SelectedIndex ? _iconsSelectedBrushes[ik] : _iconsBrushes[ik], iconRect);
            }
        }

        // Draw divider if not using colors
        if (!UseColors)
        {
            using Pen dividerPen = new(SkinManager.DividersColor, 1);
            g.DrawLine(dividerPen, Width - 1, 0, Width - 1, Height);
        }

        // Animate tab indicator
        var previousSelectedTabIndexIfHasOne = _previousSelectedTabIndex == -1 ? BaseTabControl.SelectedIndex : _previousSelectedTabIndex;
        if (_drawerItemRects.Count == 0 || previousSelectedTabIndexIfHasOne >= _drawerItemRects.Count || BaseTabControl.SelectedIndex >= _drawerItemRects.Count)
            return;

        var previousActiveTabRect = _drawerItemRects[previousSelectedTabIndexIfHasOne];
        var activeTabPageRect = _drawerItemRects[BaseTabControl.SelectedIndex];

        var y = previousActiveTabRect.Y + (int)((activeTabPageRect.Y - previousActiveTabRect.Y) * clickAnimProgress);
        var x = ShowIconsWhenHidden ? -Location.X : 0;
        var height = _drawerItemHeight;

        g.FillRectangle(SkinManager.ColorScheme.AccentBrush, x, y, IndicatorWidth, height);
    }

    public new void Show()
    {
        _isOpen = true;
        DrawerStateChanged?.Invoke(this, EventArgs.Empty);
        DrawerBeginOpen?.Invoke(this, EventArgs.Empty);
        _showHideAnimManager.StartNewAnimation(AnimationDirection.Out);
    }

    public new void Hide()
    {
        _isOpen = false;
        DrawerStateChanged?.Invoke(this, EventArgs.Empty);
        DrawerBeginClose?.Invoke(this, EventArgs.Empty);
        _showHideAnimManager.StartNewAnimation(AnimationDirection.In);
    }

    public void Toggle()
    {
        if (_isOpen)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private int CalculateAlphaZeroWhenClosed(int primaryA, int secondaryA, int tabIndex, double clickAnimProgress, double showHideAnimProgress)
    {
        // Drawer is closed
        if (!_isOpen && !_showHideAnimManager.IsAnimating())
            return 0;

        // Active menu (no change)
        if (BaseTabControl != null && tabIndex == BaseTabControl.SelectedIndex && (!_clickAnimManager.IsAnimating() || _showHideAnimManager.IsAnimating()))
            return (int)(primaryA * showHideAnimProgress);

        // Previous menu (changing)
        if (tabIndex == _previousSelectedTabIndex && !_showHideAnimManager.IsAnimating())
            return primaryA - (int)((primaryA - secondaryA) * clickAnimProgress);

        // Inactive menu (no change)
        if (BaseTabControl != null && tabIndex != BaseTabControl.SelectedIndex)
            return (int)(secondaryA * showHideAnimProgress);

        // Active menu (changing)
        return secondaryA + (int)((primaryA - secondaryA) * clickAnimProgress);
    }

    private int CalculateAlpha(int primaryA, int secondaryA, int tabIndex, double clickAnimProgress)
    {
        if (BaseTabControl != null && tabIndex == BaseTabControl.SelectedIndex && !_clickAnimManager.IsAnimating())
            return primaryA;

        if (BaseTabControl != null && tabIndex != _previousSelectedTabIndex && tabIndex != BaseTabControl.SelectedIndex)
            return secondaryA;

        if (tabIndex == _previousSelectedTabIndex)
            return primaryA - (int)((primaryA - secondaryA) * clickAnimProgress);

        return secondaryA + (int)((primaryA - secondaryA) * clickAnimProgress);
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);

        if (_drawerItemRects == null)
        {
            UpdateTabRects();
            return;
        }

        if (BaseTabControl != null)
        {
            for (var i = 0; i < _drawerItemRects.Count; i++)
            {
                if (_drawerItemRects[i].Contains(e.Location) && _lastLocationY == Location.Y)
                {
                    BaseTabControl.SelectedIndex = i;
                    if (AutoHide && !AutoShow)
                        Hide();
                }
            }
        }

        _animationSource = e.Location;
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        _lastMouseY = e.Y;
        _lastLocationY = Location.Y; // memorize Y location of drawer
        base.OnMouseDown(e);
        if (DesignMode)
            return;

        MouseState = MouseState.DOWN;
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (DesignMode)
            return;

        MouseState = MouseState.OUT;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (DesignMode)
            return;

        if (e.Button == MouseButtons.Left && e.Y != _lastMouseY && (Location.Y < 0 || Height < (8 + _drawerItemHeight) * (_drawerItemRects?.Count ?? 0)))
        {
            var diff = e.Y - _lastMouseY;
            if (diff > 0)
            {
                if (Location.Y < 0)
                {
                    Location = new Point(Location.X, Location.Y + diff > 0 ? 0 : Location.Y + diff);
                    Height = Parent?.Height ?? 0 + Math.Abs(Location.Y);
                }
            }
            else
            {
                if (Height < (8 + _drawerItemHeight) * (_drawerItemRects?.Count ?? 0))
                {
                    Location = new Point(Location.X, Location.Y + diff);
                    Height = Parent?.Height ?? 0 + Math.Abs(Location.Y);
                }
            }
        }

        base.OnMouseMove(e);

        if (_drawerItemRects == null)
            UpdateTabRects();

        var previousCursor = Cursor;

        if (e.Location.X + Location.X < _borderWidth)
        {
            if (e.Location.Y > Height - _borderWidth)
            {
                Cursor = Cursors.SizeNESW;                  //Bottom Left
            }
            else
            {
                Cursor = Cursors.SizeWE;                    //Left
            }
        }
        else if (e.Location.Y > Height - _borderWidth)
        {
            Cursor = Cursors.SizeNS;                        //Bottom
        }
        else
        {
            if (_drawerItemRects != null && e.Location.Y < _drawerItemRects[^1].Bottom && (e.Location.X + Location.X) >= _borderWidth)
            {
                Cursor = Cursors.Hand;
            }
            else
            {
                Cursor = Cursors.Default;
            }
        }

        if (previousCursor != Cursor)
        {
            CursorUpdate?.Invoke(this, Cursor);
        }
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        if (AutoShow && _isOpen == false)
        {
            Show();
        }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);

        if (MouseState != MouseState.DOWN)
        {
            Cursor = Cursors.Default;
            CursorUpdate?.Invoke(this, Cursor);
        }

        if (AutoShow)
        {
            Hide();
        }
    }

    private void UpdateTabRects()
    {
        //If there isn't a base tab control, the rects shouldn't be calculated
        //or if there aren't tab pages in the base tab control, the list should just be empty
        if (BaseTabControl == null || BaseTabControl.TabCount == 0 || SkinManager == null || _drawerItemRects == null)
        {
            _drawerItemRects = [];
            _drawerItemPaths = [];
            return;
        }

        if (_drawerItemRects.Count != BaseTabControl.TabCount)
        {
            _drawerItemRects = new List<Rectangle>(BaseTabControl.TabCount);
            _drawerItemPaths = new List<GraphicsPath>(BaseTabControl.TabCount);

            for (var i = 0; i < BaseTabControl.TabCount; i++)
            {
                _drawerItemRects.Add(new Rectangle());
                _drawerItemPaths.Add(new GraphicsPath());
            }
        }

        //Calculate the bounds of each tab header specified in the base tab control
        for (var i = 0; i < BaseTabControl.TabPages.Count; i++)
        {
            _drawerItemRects[i] = new Rectangle(
                (int)(SkinManager.FORM_PADDING * 0.75) - (ShowIconsWhenHidden ? Location.X : 0),
                _tabHeaderPadding * 2 * i + (SkinManager.FORM_PADDING >> 1),
                Width + (ShowIconsWhenHidden ? Location.X : 0) - (int)(SkinManager.FORM_PADDING * 1.5) - 1,
                _drawerItemHeight);

            _drawerItemPaths?[i] = DrawHelper.CreateRoundRect(new RectangleF(_drawerItemRects[i].X - 0.5f, _drawerItemRects[i].Y - 0.5f, _drawerItemRects[i].Width, _drawerItemRects[i].Height), 4);
        }
    }
}
