namespace MaterialSkin.Controls;

public partial class MaterialTabSelector : Control, IMaterialControl
{
    private const int _iconSize = 24;
    private const int _firstTabPadding = 50;
    private const int _tabHeaderPadding = 24;
    private const int _tabWidthMin = 160;
    private const int _tabWidthMax = 264;

    private readonly TextInfo _textInfo = new CultureInfo("en-US", false).TextInfo;
    private readonly AnimationManager _animationManager;
    private int _previousSelectedTabIndex;
    private Point _animationSource;
    private List<Rectangle>? _tabRects;
    private int _tabOverIndex = -1;

    public MaterialTabSelector()
    {
        SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);
        TabIndicatorHeight = 2;
        TabLabel = TabLabelStyle.Text;

        Size = new Size(480, 48);

        _animationManager = new AnimationManager
        {
            AnimationType = AnimationType.EaseOut,
            Increment = 0.04
        };
        _animationManager.OnAnimationProgress += (sender, e) => Invalidate();
    }

    [Browsable(false)]
    public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

    [Browsable(false)]
    public MouseState MouseState { get; set; }

    [Category("Material Skin"), Browsable(true)]
    public MaterialTabControl? BaseTabControl
    {
        get;
        set
        {
            field = value;
            if (field == null)
                return;

            UpdateTabRects();

            _previousSelectedTabIndex = field.SelectedIndex;
            field.Deselected += (sender, args) =>
            {
                _previousSelectedTabIndex = field.SelectedIndex;
            };

            field.SelectedIndexChanged += (sender, args) =>
            {
                _animationManager.SetProgress(0);
                _animationManager.StartNewAnimation(AnimationDirection.In);
            };

            field.ControlAdded += (s, e) => Invalidate();
            field.ControlRemoved += (s, e) => Invalidate();
        }
    }

    [Category("Appearance")]
    public CustomCharacterCasing CharacterCasing
    {
        get;
        set
        {
            field = value;
            BaseTabControl?.Invalidate();
            Invalidate();
        }
    }

    [Category("Material Skin"), Browsable(true), DisplayName("Tab Indicator Height"), DefaultValue(2)]
    public int TabIndicatorHeight
    {
        get;
        set
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Value should be > 0");

            field = value;
            Refresh();
        }
    }

    [Category("Material Skin"), Browsable(true), DisplayName("Tab Label"), DefaultValue(TabLabelStyle.Text)]
    public TabLabelStyle TabLabel
    {
        get;
        set
        {
            field = value;
            if (field == TabLabelStyle.IconAndText)
            {
                Height = 72;
            }
            else
            {
                Height = 48;
            }

            UpdateTabRects();
            Invalidate();
        }
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();
        Font = SkinManager.GetFontByType(FontType.Body1);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        g.Clear(SkinManager.ColorScheme.PrimaryColor);

        if (BaseTabControl == null)
            return;

        if (!_animationManager.IsAnimating() || _tabRects == null || _tabRects.Count != BaseTabControl.TabCount)
        {
            UpdateTabRects();
        }

        var animationProgress = _animationManager.GetProgress();

        //Click feedback
        if (_animationManager.IsAnimating())
        {
            var rippleBrush = new SolidBrush(Color.FromArgb((int)(51 - (animationProgress * 50)), Color.White));
            var rippleSize = (int)(animationProgress * _tabRects[BaseTabControl.SelectedIndex].Width * 1.75);

            g.SetClip(_tabRects[BaseTabControl.SelectedIndex]);
            g.FillEllipse(rippleBrush, new Rectangle(_animationSource.X - rippleSize / 2, _animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
            g.ResetClip();
            rippleBrush.Dispose();
        }

        //Draw tab headers
        if (_tabOverIndex >= 0)
        {
            //Change mouse over tab background color
            g.FillRectangle(SkinManager.BackgroundHoverBrush, _tabRects[_tabOverIndex].X, _tabRects[_tabOverIndex].Y, _tabRects[_tabOverIndex].Width, _tabRects[_tabOverIndex].Height - TabIndicatorHeight);
        }

        foreach (TabPage tabPage in BaseTabControl.TabPages)
        {
            var currentTabIndex = BaseTabControl.TabPages.IndexOf(tabPage);

            if (TabLabel != TabLabelStyle.Icon)
            {
                // Text
                using var NativeText = new NativeTextRenderer(g);
                var textSize = TextRenderer.MeasureText(BaseTabControl.TabPages[currentTabIndex].Text, Font);
                var textLocation = new Rectangle(_tabRects[currentTabIndex].X + (_tabHeaderPadding / 2), _tabRects[currentTabIndex].Y, _tabRects[currentTabIndex].Width - _tabHeaderPadding, _tabRects[currentTabIndex].Height);

                if (TabLabel == TabLabelStyle.IconAndText)
                {
                    textLocation.Y = 46;
                    textLocation.Height = 10;
                }

                if ((_tabHeaderPadding * 2) + textSize.Width < _tabWidthMax)
                {
                    NativeText.DrawTransparentText(
                    CharacterCasing == CustomCharacterCasing.Upper ? tabPage.Text.ToUpper() :
                    CharacterCasing == CustomCharacterCasing.Lower ? tabPage.Text.ToLower() :
                    CharacterCasing == CustomCharacterCasing.Proper ? _textInfo.ToTitleCase(tabPage.Text.ToLower()) : tabPage.Text,
                    Font,
                    Color.FromArgb(CalculateTextAlpha(currentTabIndex, animationProgress), SkinManager.ColorScheme.TextColor),
                    textLocation.Location,
                    textLocation.Size,
                    TextAlignFlags.Center | TextAlignFlags.Middle);
                }
                else
                {
                    if (TabLabel == TabLabelStyle.IconAndText)
                    {
                        textLocation.Y = 40;
                        textLocation.Height = 26;
                    }

                    NativeText.DrawMultilineTransparentText(
                    CharacterCasing == CustomCharacterCasing.Upper ? tabPage.Text.ToUpper() :
                    CharacterCasing == CustomCharacterCasing.Lower ? tabPage.Text.ToLower() :
                    CharacterCasing == CustomCharacterCasing.Proper ? _textInfo.ToTitleCase(tabPage.Text.ToLower()) : tabPage.Text,
                    SkinManager.GetFontByType(FontType.Body2),
                    Color.FromArgb(CalculateTextAlpha(currentTabIndex, animationProgress), SkinManager.ColorScheme.TextColor),
                    textLocation.Location,
                    textLocation.Size,
                    TextAlignFlags.Center | TextAlignFlags.Middle);
                }
            }

            if (TabLabel != TabLabelStyle.Text)
            {
                // Icons
                if (BaseTabControl.ImageList != null && (!string.IsNullOrEmpty(tabPage.ImageKey) | tabPage.ImageIndex > -1))
                {
                    var iconRect = new Rectangle(
                        _tabRects[currentTabIndex].X + (_tabRects[currentTabIndex].Width / 2) - (_iconSize / 2),
                        _tabRects[currentTabIndex].Y + (_tabRects[currentTabIndex].Height / 2) - (_iconSize / 2),
                        _iconSize, _iconSize);
                    if (TabLabel == TabLabelStyle.IconAndText)
                    {
                        iconRect.Y = 12;
                    }

                    var img = string.IsNullOrEmpty(tabPage.ImageKey) ? BaseTabControl.ImageList.Images[tabPage.ImageIndex] : BaseTabControl.ImageList.Images[tabPage.ImageKey];
                    if (img != null)
                    {
                        g.DrawImage(img, iconRect);
                    }
                }
            }
        }

        //Animate tab indicator
        var previousSelectedTabIndexIfHasOne = _previousSelectedTabIndex == -1 ? BaseTabControl.SelectedIndex : _previousSelectedTabIndex;
        var previousActiveTabRect = _tabRects[previousSelectedTabIndexIfHasOne];
        var activeTabPageRect = _tabRects[BaseTabControl.SelectedIndex];

        var y = activeTabPageRect.Bottom - TabIndicatorHeight;
        var x = previousActiveTabRect.X + (int)((activeTabPageRect.X - previousActiveTabRect.X) * animationProgress);
        var width = previousActiveTabRect.Width + (int)((activeTabPageRect.Width - previousActiveTabRect.Width) * animationProgress);

        g.FillRectangle(SkinManager.ColorScheme.AccentBrush, x, y, width, TabIndicatorHeight);
    }

    private int CalculateTextAlpha(int tabIndex, double animationProgress)
    {
        var primaryA = SkinManager.TextHighEmphasisColor.A;
        var secondaryA = SkinManager.TextMediumEmphasisColor.A;

        if (BaseTabControl != null && tabIndex == BaseTabControl.SelectedIndex && !_animationManager.IsAnimating())
            return primaryA;

        if (BaseTabControl != null && tabIndex != _previousSelectedTabIndex && tabIndex != BaseTabControl.SelectedIndex)
            return secondaryA;

        if (tabIndex == _previousSelectedTabIndex)
            return primaryA - (int)((primaryA - secondaryA) * animationProgress);

        return secondaryA + (int)((primaryA - secondaryA) * animationProgress);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (_tabRects == null)
        {
            UpdateTabRects();
        }
        else if (BaseTabControl != null)
        {
            for (var i = 0; i < _tabRects.Count; i++)
            {
                if (_tabRects[i].Contains(e.Location))
                {
                    BaseTabControl.SelectedIndex = i;
                }
            }
        }

        _animationSource = e.Location;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (DesignMode)
            return;

        if (_tabRects == null)
            UpdateTabRects();

        var oldTabOverIndex = _tabOverIndex;
        _tabOverIndex = -1;
        for (var i = 0; i < _tabRects.Count; i++)
        {
            if (_tabRects[i].Contains(e.Location))
            {
                Cursor = Cursors.Hand;
                _tabOverIndex = i;
                break;
            }
        }

        if (_tabOverIndex == -1)
        {
            Cursor = Cursors.Arrow;
        }

        if (oldTabOverIndex != _tabOverIndex)
        {
            Invalidate();
        }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        if (DesignMode)
            return;

        if (_tabRects == null)
            UpdateTabRects();

        Cursor = Cursors.Arrow;
        _tabOverIndex = -1;
        Invalidate();
    }

    [MemberNotNull(nameof(_tabRects))]
    private void UpdateTabRects()
    {
        _tabRects = [];

        //If there isn't a base tab control, the rects shouldn't be calculated
        //If there aren't tab pages in the base tab control, the list should just be empty which has been set already; exit the void
        if (BaseTabControl == null || BaseTabControl.TabCount == 0) return;

        //Calculate the bounds of each tab header specified in the base tab control
        using var b = new Bitmap(1, 1);
        using var g = Graphics.FromImage(b);
        using var NativeText = new NativeTextRenderer(g);
        for (var i = 0; i < BaseTabControl.TabPages.Count; i++)
        {
            var textSize = TextRenderer.MeasureText(BaseTabControl.TabPages[i].Text, Font);
            if (TabLabel == TabLabelStyle.Icon)
            {
                textSize.Width = _iconSize;
            }

            var TabWidth = (_tabHeaderPadding * 2) + textSize.Width;
            if (TabWidth > _tabWidthMax)
            {
                TabWidth = _tabWidthMax;
            }
            else if (TabWidth < _tabWidthMin)
            {
                TabWidth = _tabWidthMin;
            }

            if (i == 0)
            {
                _tabRects.Add(new Rectangle(_firstTabPadding - _tabHeaderPadding, 0, TabWidth, Height));
            }
            else
            {
                _tabRects.Add(new Rectangle(_tabRects[i - 1].Right, 0, TabWidth, Height));
            }
        }
    }
}
