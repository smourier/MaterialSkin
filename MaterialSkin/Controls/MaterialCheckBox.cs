namespace MaterialSkin.Controls;

public class MaterialCheckbox : CheckBox, IMaterialControl
{
    private readonly AnimationManager _checkAM;
    private readonly AnimationManager _rippleAM;
    private readonly AnimationManager _hoverAM;
    private const int _heightRipple = 37;
    private const int _heightNoRipple = 20;
    private const int _textOffset = 26;
    private const int _checkboxSize = 18;
    private const int _checkboxHalfSize = _checkboxSize / 2;

    private int _boxOffset;
    private static readonly Point[] _checkmarkLine = [new Point(3, 8), new Point(7, 12), new Point(14, 5)];
    private bool _hovered;
    private CheckState _oldCheckState;

    public MaterialCheckbox()
    {
        _checkAM = new AnimationManager
        {
            AnimationType = AnimationType.EaseInOut,
            Increment = 0.05
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

        CheckedChanged += (sender, args) =>
        {
            if (Ripple)
            {
                _checkAM.StartNewAnimation(Checked ? AnimationDirection.In : AnimationDirection.Out);
            }
        };

        _checkAM.OnAnimationProgress += (sender, e) => Invalidate();
        _hoverAM.OnAnimationProgress += (sender, e) => Invalidate();
        _rippleAM.OnAnimationProgress += (sender, e) => Invalidate();

        Ripple = true;
        Height = _heightRipple;
        MouseLocation = new Point(-1, -1);
    }

    [Browsable(false)]
    public int Depth { get; set; }

    [Browsable(false)]
    public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

    [Browsable(false)]
    public MouseState MouseState { get; set; }

    [Browsable(false)]
    public Point MouseLocation { get; set; }

    private bool _ripple;

    [Category("Appearance")]
    public bool Ripple
    {
        get => _ripple;
        set
        {
            _ripple = value;
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

    [Browsable(true)]
    public bool ReadOnly { get; set; }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        _boxOffset = _heightRipple / 2 - 9;
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
        Size strSize;
        using (var NativeText = new NativeTextRenderer(CreateGraphics()))
        {
            strSize = NativeText.MeasureLogString(Text, SkinManager.GetLogFontByType(FontType.Body1));
        }

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

        var checkboxCenter = _boxOffset + _checkboxHalfSize - 1;
        var animationSource = new Point(checkboxCenter, checkboxCenter);
        var animationProgress = _checkAM.GetProgress();

        var colorAlpha = Enabled ? (int)(animationProgress * 255.0) : SkinManager.CheckBoxOffDisabledColor.A;
        var backgroundAlpha = Enabled ? (int)(SkinManager.CheckboxOffColor.A * (1.0 - animationProgress)) : SkinManager.CheckBoxOffDisabledColor.A;
        var rippleHeight = (_heightRipple % 2 == 0) ? _heightRipple - 3 : _heightRipple - 2;

        var brush = new SolidBrush(Color.FromArgb(colorAlpha, Enabled ? SkinManager.ColorScheme.AccentColor : SkinManager.CheckBoxOffDisabledColor));
        var pen = new Pen(brush.Color, 2);

        // draw hover animation
        if (Ripple)
        {
            var animationValue = _hoverAM.IsAnimating() ? _hoverAM.GetProgress() : _hovered ? 1 : 0;
            var rippleSize = (int)(rippleHeight * (0.7 + (0.3 * animationValue)));

            using var rippleBrush = new SolidBrush(Color.FromArgb((int)(40 * animationValue),
                !Checked ? (SkinManager.Theme == Themes.LIGHT ? Color.Black : Color.White) : brush.Color)); // no animation
            g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
        }

        // draw ripple animation
        if (Ripple && _rippleAM.IsAnimating())
        {
            for (var i = 0; i < _rippleAM.GetAnimationCount(); i++)
            {
                var animationValue = _rippleAM.GetProgress(i);
                var rippleSize = (_rippleAM.GetDirection(i) == AnimationDirection.InOutIn) ? (int)(rippleHeight * (0.7 + (0.3 * animationValue))) : rippleHeight;

                using var rippleBrush = new SolidBrush(Color.FromArgb((int)(animationValue * 40), !Checked ? (SkinManager.Theme == Themes.LIGHT ? Color.Black : Color.White) : brush.Color));
                g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize, rippleSize));
            }
        }

        var checkMarkLineFill = new Rectangle(_boxOffset, _boxOffset, (int)(_checkboxSize * animationProgress), _checkboxSize);
        using (var checkmarkPath = DrawHelper.CreateRoundRect(_boxOffset - 0.5f, _boxOffset - 0.5f, _checkboxSize, _checkboxSize, 1))
        {
            if (Enabled)
            {
                if (Parent != null)
                {
                    using var pen2 = new Pen(DrawHelper.BlendColor(Parent.BackColor, Enabled ? SkinManager.CheckboxOffColor : SkinManager.CheckBoxOffDisabledColor, backgroundAlpha), 2);
                    g.DrawPath(pen2, checkmarkPath);
                }

                g.DrawPath(pen, checkmarkPath);
                g.FillPath(brush, checkmarkPath);
            }
            else
            {
                if (Checked)
                {
                    g.FillPath(brush, checkmarkPath);
                }
                else
                {
                    g.DrawPath(pen, checkmarkPath);
                }
            }

            g.DrawImageUnscaledAndClipped(DrawCheckMarkBitmap(), checkMarkLineFill);
        }

        // draw checkbox text
        using (var NativeText = new NativeTextRenderer(g))
        {
            var textLocation = new Rectangle(_boxOffset + _textOffset, 0, Width - (_boxOffset + _textOffset), _heightRipple);
            NativeText.DrawTransparentText(Text, SkinManager.GetLogFontByType(FontType.Body1),
                Enabled ? SkinManager.TextHighEmphasisColor : SkinManager.TextDisabledOrHintColor,
                textLocation.Location,
                textLocation.Size,
                TextAlignFlags.Left | TextAlignFlags.Middle);
        }

        // dispose used paint objects
        pen.Dispose();
        brush.Dispose();
    }

    public override bool AutoSize
    {
        get => base.AutoSize;
        set
        {
            base.AutoSize = value;
            if (value)
            {
                Size = new Size(10, 10);
            }
        }
    }

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
            _oldCheckState = CheckState;
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
            if (ReadOnly) CheckState = _oldCheckState;
        };

        KeyDown += (sender, args) =>
        {
            if (Ripple && (args.KeyCode == Keys.Space) && _rippleAM.GetAnimationCount() == 0)
            {
                _rippleAM.SecondaryIncrement = 0;
                _rippleAM.StartNewAnimation(AnimationDirection.InOutIn, [Checked]);
            }
            if (ReadOnly) CheckState = _oldCheckState;
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
            if (ReadOnly) CheckState = _oldCheckState;
        };

        KeyUp += (sender, args) =>
        {
            if (Ripple && (args.KeyCode == Keys.Space))
            {
                MouseState = MouseState.HOVER;
                _rippleAM.SecondaryIncrement = 0.08;
            }
            if (ReadOnly) CheckState = _oldCheckState;
        };

        MouseMove += (sender, args) =>
        {
            MouseLocation = args.Location;
            Cursor = IsMouseInCheckArea() ? Cursors.Hand : Cursors.Default;
        };
    }

    private Bitmap DrawCheckMarkBitmap()
    {
        var checkMark = new Bitmap(_checkboxSize, _checkboxSize);
        var g = Graphics.FromImage(checkMark);

        // clear everything, transparent
        g.Clear(Color.Transparent);

        // draw the checkmark lines
        if (Parent != null)
        {
            using var pen = new Pen(Parent.BackColor, 2);
            g.DrawLines(pen, _checkmarkLine);
        }

        return checkMark;
    }

    private bool IsMouseInCheckArea() => ClientRectangle.Contains(MouseLocation);
}