namespace MaterialSkin.Controls;

public class MaterialRadioButton : RadioButton, IMaterialControl
{
    // size constants
    private const int _heightRipple = 37;
    private const int _heightNoRipple = 20;
    private const int _radioButtonSize = 18;
    private const int _radioButtonSizeHalf = _radioButtonSize / 2;
    private const int _textOffset = 26;

    // animation managers
    private readonly AnimationManager _checkAM;
    private readonly AnimationManager _rippleAM;
    private readonly AnimationManager _hoverAM;
    // size related variables which should be recalculated onsizechanged
    private int _boxOffset;
    private bool _hovered;

    [Browsable(false)]
    public MouseState MouseState { get; set; }

    [Browsable(false)]
    public Point MouseLocation { get; set; }

    [Category("Behavior")]
    public bool Ripple
    {
        get;
        set
        {
            field = value;
#pragma warning disable CA2245 // Do not assign a property to itself
            AutoSize = AutoSize; //Make AutoSize directly set the bounds.
#pragma warning restore CA2245 // Do not assign a property to itself

            if (value)
            {
                Margin = new Padding(0);
            }

            Invalidate();
        }
    }

    public MaterialRadioButton()
    {
        SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);

        _checkAM = new AnimationManager
        {
            AnimationType = AnimationType.EaseInOut,
            Increment = 0.06
        };

        _hoverAM = new AnimationManager(true)
        {
            AnimationType = AnimationType.Linear,
            Increment = 0.10
        };

        _rippleAM = new AnimationManager(false)
        {
            AnimationType = AnimationType.Linear,
            Increment = 0.10,
            SecondaryIncrement = 0.08
        };

        _checkAM.OnAnimationProgress += (sender, e) => Invalidate();
        _hoverAM.OnAnimationProgress += (sender, e) => Invalidate();
        _rippleAM.OnAnimationProgress += (sender, e) => Invalidate();

        TabStopChanged += (sender, e) => TabStop = true;

        CheckedChanged += (sender, args) =>
        {
            if (Ripple)
                _checkAM.StartNewAnimation(Checked ? AnimationDirection.In : AnimationDirection.Out);
        };

        SizeChanged += OnSizeChanged;

        Ripple = true;
        MouseLocation = new Point(-1, -1);
    }

    private void OnSizeChanged(object? sender, EventArgs eventArgs) => _boxOffset = Height / 2 - _radioButtonSize / 2;

    public override Size GetPreferredSize(Size proposedSize)
    {
        using var NativeText = new NativeTextRenderer(CreateGraphics());
        var strSize = NativeText.MeasureLogString(Text, MaterialSkinManager.Instance.GetLogFontByType(FontType.Body1));

        var w = _boxOffset + _textOffset + strSize.Width;
        return Ripple ? new Size(w, _heightRipple) : new Size(w, _heightNoRipple);
    }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        var g = pevent.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        // clear the control
        if (Parent != null)
        {
            g.Clear(Parent.BackColor);
        }

        var center = _boxOffset + _radioButtonSizeHalf;
        var animationSource = new Point(center, center);

        var animationProgress = _checkAM.GetProgress();

        var colorAlpha = Enabled ? (int)(animationProgress * 255.0) : MaterialSkinManager.Instance.CheckBoxOffDisabledColor.A;
        var backgroundAlpha = Enabled ? (int)(MaterialSkinManager.Instance.CheckboxOffColor.A * (1.0 - animationProgress)) : MaterialSkinManager.Instance.CheckBoxOffDisabledColor.A;
        var animationSize = (float)(animationProgress * 9f);
        var animationSizeHalf = animationSize / 2;
        var rippleHeight = (_heightRipple % 2 == 0) ? _heightRipple - 3 : _heightRipple - 2;

        var RadioColor = Color.FromArgb(colorAlpha, Enabled ? MaterialSkinManager.Instance.ColorScheme.AccentColor : MaterialSkinManager.Instance.CheckBoxOffDisabledColor);

        // draw hover animation
        if (Ripple)
        {
            var animationValue = _hoverAM.GetProgress();
            var rippleSize = (int)(rippleHeight * (0.7 + (0.3 * animationValue)));

            using var rippleBrush = new SolidBrush(Color.FromArgb((int)(40 * animationValue), !Checked ? (MaterialSkinManager.Instance.Theme == Themes.LIGHT ? Color.Black : Color.White) : RadioColor));
            g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize - 1, rippleSize - 1));
        }

        // draw ripple animation
        if (Ripple && _rippleAM.IsAnimating())
        {
            for (var i = 0; i < _rippleAM.GetAnimationCount(); i++)
            {
                var animationValue = _rippleAM.GetProgress(i);
                var rippleSize = (_rippleAM.GetDirection(i) == AnimationDirection.InOutIn) ? (int)(rippleHeight * (0.7 + (0.3 * animationValue))) : rippleHeight;

                using var rippleBrush = new SolidBrush(Color.FromArgb((int)(animationValue * 40), !Checked ? (MaterialSkinManager.Instance.Theme == Themes.LIGHT ? Color.Black : Color.White) : RadioColor));
                g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize - 1, rippleSize - 1));
            }
        }

        // draw radiobutton circle
        if (Parent != null)
        {
            using var pen = new Pen(DrawHelper.BlendColor(Parent.BackColor, Enabled ? MaterialSkinManager.Instance.CheckboxOffColor : MaterialSkinManager.Instance.CheckBoxOffDisabledColor, backgroundAlpha), 2);
            g.DrawEllipse(pen, new Rectangle(_boxOffset, _boxOffset, _radioButtonSize, _radioButtonSize));
        }

        if (Enabled)
        {
            using var pen = new Pen(RadioColor, 2);
            g.DrawEllipse(pen, new Rectangle(_boxOffset, _boxOffset, _radioButtonSize, _radioButtonSize));
        }

        if (Checked)
        {
            using var brush = new SolidBrush(RadioColor);
            g.FillEllipse(brush, new RectangleF(center - animationSizeHalf, center - animationSizeHalf, animationSize, animationSize));
        }

        // Text
        using var NativeText = new NativeTextRenderer(g);
        var textLocation = new Rectangle(_boxOffset + _textOffset, 0, Width, Height);
        NativeText.DrawTransparentText(Text, MaterialSkinManager.Instance.GetLogFontByType(FontType.Body1),
            Enabled ? MaterialSkinManager.Instance.TextHighEmphasisColor : MaterialSkinManager.Instance.TextDisabledOrHintColor,
            textLocation.Location,
            textLocation.Size,
            TextAlignFlags.Left | TextAlignFlags.Middle);
    }

    private bool IsMouseInCheckArea() => ClientRectangle.Contains(MouseLocation);
    protected override void OnCreateControl()
    {
        base.OnCreateControl();

        if (DesignMode)
            return;

        MouseState = MouseState.OUT;

        GotFocus += (sender, AddingNewEventArgs) =>
        {
            if (Ripple && !_hovered)
            {
                _hoverAM.StartNewAnimation(AnimationDirection.In, [Checked]);
                _hovered = true;
            }
        };

        LostFocus += (sender, args) =>
        {
            if (Ripple && _hovered)
            {
                _hoverAM.StartNewAnimation(AnimationDirection.Out, [Checked]);
                _hovered = false;
            }
        };

        MouseEnter += (sender, args) =>
        {
            MouseState = MouseState.HOVER;
        };

        MouseLeave += (sender, args) =>
        {
            MouseLocation = new Point(-1, -1);
            MouseState = MouseState.OUT;
        };

        MouseDown += (sender, args) =>
        {
            MouseState = MouseState.DOWN;
            if (Ripple)
            {
                _rippleAM.SecondaryIncrement = 0;
                _rippleAM.StartNewAnimation(AnimationDirection.InOutIn, [Checked]);
            }
        };

        KeyDown += (sender, args) =>
        {
            if (Ripple && (args.KeyCode == Keys.Space) && _rippleAM.GetAnimationCount() == 0)
            {
                _rippleAM.SecondaryIncrement = 0;
                _rippleAM.StartNewAnimation(AnimationDirection.InOutIn, [Checked]);
            }
        };

        MouseUp += (sender, args) =>
        {
            if (Ripple)
            {
                MouseState = MouseState.HOVER;
                _rippleAM.SecondaryIncrement = 0.08;
                _hoverAM.StartNewAnimation(AnimationDirection.Out, [Checked]);
                _hovered = false;
            }
        };

        KeyUp += (sender, args) =>
        {
            if (Ripple && (args.KeyCode == Keys.Space))
            {
                MouseState = MouseState.HOVER;
                _rippleAM.SecondaryIncrement = 0.08;
            }
        };

        MouseMove += (sender, args) =>
        {
            MouseLocation = args.Location;
            Cursor = IsMouseInCheckArea() ? Cursors.Hand : Cursors.Default;
        };
    }
}