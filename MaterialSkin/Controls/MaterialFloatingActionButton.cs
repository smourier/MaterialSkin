namespace MaterialSkin.Controls;

public class MaterialFloatingActionButton : Button, IMaterialControl
{
    private const int _fabSize = 56;
    private const int _fabMiniSize = 40;

    private readonly AnimationManager _animationManager;
    private readonly AnimationManager _showAnimationManager;
    private bool _isHiding;
    private bool _mouseHover;
    private bool _mini;
    private bool _shadowDrawEventSubscribed;
    private Rectangle _fabBounds;

    public MaterialFloatingActionButton()
    {
        AnimateShowHideButton = false;
        Mini = false;
        DrawShadows = true;
        SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

        Size = new Size(_fabSize, _fabSize);
        _animationManager = new AnimationManager(false)
        {
            Increment = 0.03,
            AnimationType = AnimationType.EaseOut
        };
        _animationManager.OnAnimationProgress += (sender, e) => Invalidate();

        _showAnimationManager = new AnimationManager(true)
        {
            Increment = 0.1,
            AnimationType = AnimationType.EaseOut
        };
        _showAnimationManager.OnAnimationProgress += (sender, e) => Invalidate();
        _showAnimationManager.OnAnimationFinished += ShowAnimationManager_OnAnimationFinished;
    }

    [Browsable(false)]
    public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

    [Browsable(false)]
    public MouseState MouseState { get; set; }

    [DefaultValue(true)]
    [Category("Material Skin"), DisplayName("Draw Shadows")]
    [Description("Draw Shadows around control")]
    public bool DrawShadows { get; set; }

    [DefaultValue(false)]
    [Category("Material Skin"), DisplayName("Size Mini")]
    [Description("Set control size to default or mini")]
    public bool Mini
    {
        get => _mini;
        set
        {
            Parent?.Invalidate();
            SetSize(value);
        }
    }

    [DefaultValue(false)]
    [Category("Material Skin"), DisplayName("Animate Show HideButton")]
    public bool AnimateShowHideButton
    {
        get;
        set { field = value; Refresh(); }
    }


    [DefaultValue(false)]
    [Category("Material Skin")]
    [Description("Define icon to display")]
    public Image? Icon
    {
        get;
        set { field = value; Refresh(); }
    }

    protected override void InitLayout() => LocationChanged += (sender, e) => { if (DrawShadows) Parent?.Invalidate(); };
    protected override void OnParentChanged(EventArgs e)
    {
        base.OnParentChanged(e);
        if (DrawShadows && Parent != null)
        {
            AddShadowPaintEvent(Parent, DrawShadowOnParent);
        }

        if (_oldParent != null)
        {
            RemoveShadowPaintEvent(_oldParent, DrawShadowOnParent);
        }
        _oldParent = Parent;
    }

    private Control? _oldParent;

    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);
        if (Parent == null)
            return;

        if (Visible)
        {
            AddShadowPaintEvent(Parent, DrawShadowOnParent);
        }
        else
        {
            RemoveShadowPaintEvent(Parent, DrawShadowOnParent);
        }
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

    private void SetSize(bool mini)
    {
        _mini = mini;
        Size = _mini ? new Size(_fabMiniSize, _fabMiniSize) : new Size(_fabSize, _fabSize);
        _fabBounds = _mini ? new Rectangle(0, 0, _fabMiniSize, _fabMiniSize) : new Rectangle(0, 0, _fabSize, _fabSize);
        _fabBounds.Width -= 1;
        _fabBounds.Height -= 1;
    }

    private void ShowAnimationManager_OnAnimationFinished(object? sender, EventArgs e)
    {
        if (_isHiding)
        {
            Visible = false;
            _isHiding = false;
        }
    }

    private void DrawShadowOnParent(object? sender, PaintEventArgs e)
    {
        if (Parent == null)
        {
            RemoveShadowPaintEvent((Control)sender!, DrawShadowOnParent);
            return;
        }

        // paint shadow on parent
        var gp = e.Graphics;
        var rect = new Rectangle(Location, _fabBounds.Size);
        gp.SmoothingMode = SmoothingMode.AntiAlias;
        DrawHelper.DrawRoundShadow(gp, rect);
    }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        var g = pevent.Graphics;

        if (Parent != null)
        {
            g.Clear(Parent.BackColor);
        }

        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Paint shadow on element to blend with the parent shadow
        DrawHelper.DrawRoundShadow(g, _fabBounds);

        // draw fab
        g.FillEllipse(Enabled ? _mouseHover ?
            new SolidBrush(SkinManager.ColorScheme.AccentColor.Lighten(0.25f)) :
            SkinManager.ColorScheme.AccentBrush :
            new SolidBrush(DrawHelper.BlendColor(SkinManager.ColorScheme.AccentColor, SkinManager.SwitchOffDisabledThumbColor, 197)),
            _fabBounds);

        if (_animationManager.IsAnimating())
        {
            var regionPath = new GraphicsPath();
            regionPath.AddEllipse(new Rectangle(_fabBounds.X - 1, _fabBounds.Y - 1, _fabBounds.Width + 3, _fabBounds.Height + 2));
            var fabRegion = new Region(regionPath);

            var gcont = g.BeginContainer();
            g.SetClip(fabRegion, CombineMode.Replace);

            for (int i = 0; i < _animationManager.GetAnimationCount(); i++)
            {
                var animationValue = _animationManager.GetProgress(i);
                var animationSource = _animationManager.GetSource(i);
                var rippleBrush = new SolidBrush(Color.FromArgb((int)(51 - (animationValue * 50)), Color.White));
                var rippleSize = (int)(animationValue * Width * 2);
                g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
            }

            g.EndContainer(gcont);
        }

        if (Icon != null)
        {
            g.DrawImage(Icon, new Rectangle(_fabBounds.Width / 2 - 11, _fabBounds.Height / 2 - 11, 24, 24));
        }

        if (_showAnimationManager.IsAnimating())
        {
            var target = (int)((_mini ? _fabMiniSize : _fabSize) * _showAnimationManager.GetProgress());
            _fabBounds.Width = target == 0 ? 1 : target;
            _fabBounds.Height = target == 0 ? 1 : target;
            _fabBounds.X = (int)(((_mini ? _fabMiniSize : _fabSize) / 2) - ((_mini ? _fabMiniSize : _fabSize) / 2 * _showAnimationManager.GetProgress()));
            _fabBounds.Y = (int)(((_mini ? _fabMiniSize : _fabSize) / 2) - ((_mini ? _fabMiniSize : _fabSize) / 2 * _showAnimationManager.GetProgress()));
        }

        // Clip to a round shape with a 1px padding
        var clipPath = new GraphicsPath();
        clipPath.AddEllipse(new Rectangle(_fabBounds.X - 1, _fabBounds.Y - 1, _fabBounds.Width + 3, _fabBounds.Height + 3));
        Region = new Region(clipPath);
    }

    protected override void OnMouseClick(MouseEventArgs mevent)
    {
        base.OnMouseClick(mevent);
        _animationManager.StartNewAnimation(AnimationDirection.In, mevent.Location);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (DesignMode)
            return;

        _mouseHover = ClientRectangle.Contains(e.Location);
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        if (DesignMode)
            return;

        _mouseHover = false;
        Invalidate();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        if (DrawShadows && Parent != null)
        {
            RemoveShadowPaintEvent(Parent, DrawShadowOnParent);
            AddShadowPaintEvent(Parent, DrawShadowOnParent);
        }
    }

    public new void Hide()
    {
        if (Visible)
        {
            _isHiding = true;
            _showAnimationManager.StartNewAnimation(AnimationDirection.Out);
        }
    }

    public new void Show()
    {
        if (!Visible)
        {
            _showAnimationManager.StartNewAnimation(AnimationDirection.In);
            Visible = true;
        }
    }
}
