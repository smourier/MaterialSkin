namespace MaterialSkin.Controls;

public class MaterialSwitch : CheckBox, IMaterialControl
{
    private const int _thumbSize = 22;
    private const int _thumbSizeHalf = _thumbSize / 2;
    private const int _trackSizeHeight = 14;
    private const int _trackSizeWidth = 36;
    private const int _trackRadius = _trackSizeHeight / 2;
    private const int _textOffset = _thumbSize;
    private const int _rippleDiameter = 37;

    private readonly AnimationManager _checkAM;
    private readonly AnimationManager _hoverAM;
    private readonly AnimationManager _rippleAM;
    private int _trackCenterY;
    private int _traceCenterXBegin;
    private int _traceCenterXEnd;
    private int _traceCenterXDelta;
    private int _trackOffsetY;
    private bool _hovered;

    public MaterialSwitch()
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

        _checkAM.OnAnimationProgress += (sender, e) => Invalidate();
        _rippleAM.OnAnimationProgress += (sender, e) => Invalidate();
        _hoverAM.OnAnimationProgress += (sender, e) => Invalidate();

        CheckedChanged += (sender, args) =>
        {
            if (Ripple)
            {
                _checkAM.StartNewAnimation(Checked ? AnimationDirection.In : AnimationDirection.Out);
            }
        };

        Ripple = true;
        MouseLocation = new Point(-1, -1);
        ReadOnly = false;
    }

    [Browsable(false)]
    public int Depth { get; set; }

    [Browsable(false)]
    public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

    [Browsable(false)]
    public MouseState MouseState { get; set; }

    [Browsable(false)]
    public Point MouseLocation { get; set; }

    [Category("Appearance")]
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

    [Category("Appearance")]
    [Browsable(true), DefaultValue(false), EditorBrowsable(EditorBrowsableState.Always)]
    public bool ReadOnly { get; set; }

    protected override void OnClick(EventArgs e)
    {
        if (!ReadOnly)
        {
            base.OnClick(e);
        }
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);

        _trackOffsetY = Height / 2 - _thumbSizeHalf;
        _trackCenterY = _trackOffsetY + _thumbSizeHalf - 1;
        _traceCenterXBegin = _trackCenterY;
        _traceCenterXEnd = _traceCenterXBegin + _trackSizeWidth - (_trackRadius * 2);
        _traceCenterXDelta = _traceCenterXEnd - _traceCenterXBegin;
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
        using var NativeText = new NativeTextRenderer(CreateGraphics());
        var strSize = NativeText.MeasureLogString(Text, SkinManager.GetLogFontByType(FontType.Body1));
        var w = _trackSizeWidth + _thumbSize + strSize.Width;
        return Ripple ? new Size(w, _rippleDiameter) : new Size(w, _thumbSize);
    }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        var g = pevent.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        if (Parent != null)
        {
            g.Clear(Parent.BackColor);
        }

        var animationProgress = _checkAM.GetProgress();

        // Draw Track
        var thumbColor = DrawHelper.BlendColor(
                    Enabled ? SkinManager.SwitchOffThumbColor : SkinManager.SwitchOffDisabledThumbColor, // Off color
                    Enabled ? SkinManager.ColorScheme.AccentColor : DrawHelper.BlendColor(SkinManager.ColorScheme.AccentColor, SkinManager.SwitchOffDisabledThumbColor, 197), // On color
                    animationProgress * 255); // Blend amount

        using (var path = DrawHelper.CreateRoundRect(new Rectangle(_traceCenterXBegin - _trackRadius, _trackCenterY - _trackSizeHeight / 2, _trackSizeWidth, _trackSizeHeight), _trackRadius))
        {
            using var trackBrush = new SolidBrush(
                Color.FromArgb(Enabled ? SkinManager.SwitchOffTrackColor.A : SkinManager.BackgroundDisabledColor.A, // Track alpha
                DrawHelper.BlendColor( // animate color
                    Enabled ? SkinManager.SwitchOffTrackColor : SkinManager.BackgroundDisabledColor, // Off color
                    SkinManager.ColorScheme.AccentColor, // On color
                    animationProgress * 255) // Blend amount
                    .RemoveAlpha()));
            g.FillPath(trackBrush, path);
        }

        // Calculate animation movement X position
        var OffsetX = (int)(_traceCenterXDelta * animationProgress);

        // Ripple
        var rippleSize = (Height % 2 == 0) ? Height - 2 : Height - 3;

        var rippleColor = Color.FromArgb(40, // color alpha
            Checked ? SkinManager.ColorScheme.AccentColor : // On color
            (SkinManager.Theme == Themes.LIGHT ? Color.Black : Color.White)); // Off color

        if (Ripple && _rippleAM.IsAnimating())
        {
            for (var i = 0; i < _rippleAM.GetAnimationCount(); i++)
            {
                var rippleAnimProgress = _rippleAM.GetProgress(i);
                var rippleAnimatedDiameter = (_rippleAM.GetDirection(i) == AnimationDirection.InOutIn) ? (int)(rippleSize * (0.7 + (0.3 * rippleAnimProgress))) : rippleSize;

                using var rippleBrush = new SolidBrush(Color.FromArgb((int)(40 * rippleAnimProgress), rippleColor.RemoveAlpha()));
                g.FillEllipse(rippleBrush, new Rectangle(_traceCenterXBegin + OffsetX - rippleAnimatedDiameter / 2, _trackCenterY - rippleAnimatedDiameter / 2, rippleAnimatedDiameter, rippleAnimatedDiameter));
            }
        }

        // Hover
        if (Ripple)
        {
            var rippleAnimProgress = _hoverAM.GetProgress();
            var rippleAnimatedDiameter = (int)(rippleSize * (0.7 + (0.3 * rippleAnimProgress)));

            using var rippleBrush = new SolidBrush(Color.FromArgb((int)(40 * rippleAnimProgress), rippleColor.RemoveAlpha()));
            g.FillEllipse(rippleBrush, new Rectangle(_traceCenterXBegin + OffsetX - rippleAnimatedDiameter / 2, _trackCenterY - rippleAnimatedDiameter / 2, rippleAnimatedDiameter, rippleAnimatedDiameter));
        }

        // draw Thumb Shadow
        var thumbBounds = new RectangleF(_traceCenterXBegin + OffsetX - _thumbSizeHalf, _trackCenterY - _thumbSizeHalf, _thumbSize, _thumbSize);
        using (var shadowBrush = new SolidBrush(Color.FromArgb(12, 0, 0, 0)))
        {
            g.FillEllipse(shadowBrush, new RectangleF(thumbBounds.X - 2, thumbBounds.Y - 1, thumbBounds.Width + 4, thumbBounds.Height + 6));
            g.FillEllipse(shadowBrush, new RectangleF(thumbBounds.X - 1, thumbBounds.Y - 1, thumbBounds.Width + 2, thumbBounds.Height + 4));
            g.FillEllipse(shadowBrush, new RectangleF(thumbBounds.X - 0, thumbBounds.Y - 0, thumbBounds.Width + 0, thumbBounds.Height + 2));
            g.FillEllipse(shadowBrush, new RectangleF(thumbBounds.X - 0, thumbBounds.Y + 2, thumbBounds.Width + 0, thumbBounds.Height + 0));
            g.FillEllipse(shadowBrush, new RectangleF(thumbBounds.X - 0, thumbBounds.Y + 1, thumbBounds.Width + 0, thumbBounds.Height + 0));
        }

        // draw Thumb
        using (var thumbBrush = new SolidBrush(thumbColor))
        {
            g.FillEllipse(thumbBrush, thumbBounds);
        }

        // draw text
        using var NativeText = new NativeTextRenderer(g);
        var textLocation = new Rectangle(_textOffset + _trackSizeWidth, 0, Width - (_textOffset + _trackSizeWidth), Height);
        NativeText.DrawTransparentText(
            Text,
            SkinManager.GetLogFontByType(FontType.Body1),
            Enabled ? SkinManager.TextHighEmphasisColor : SkinManager.TextDisabledOrHintColor,
            textLocation.Location,
            textLocation.Size,
            TextAlignFlags.Left | TextAlignFlags.Middle);
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
