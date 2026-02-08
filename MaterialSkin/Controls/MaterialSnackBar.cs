namespace MaterialSkin.Controls;

public class MaterialSnackBar : MaterialForm
{
    private const int _topPaddingSingleLine = 6;
    private const int _leftRightPadding = 16;
    private const int _buttonPadding = 8;
    private const int _buttonHeight = 36;

    private readonly MaterialButton _actionButton = new();
    private readonly Timer _duration = new();      // Timer that checks when the drop down is fully visible
    private readonly AnimationManager _animationManager;
    private bool _closingAnimationDone;
    private bool _closeAnimation;

    public MaterialSnackBar()
        : this("SnackBar Text", 3000, false, "OK", false)
    {
    }

    public MaterialSnackBar(string text)
        : this(text, 3000, false, "OK", false)
    {
    }

    public MaterialSnackBar(string text, int duration)
        : this(text, duration, false, "OK", false)
    {
    }

    public MaterialSnackBar(string text, string actionButtonText)
        : this(text, 3000, true, actionButtonText, false)
    {
    }

    public MaterialSnackBar(string text, string actionButtonText, bool useAccentColor)
        : this(text, 3000, true, actionButtonText, useAccentColor)
    {
    }

    public MaterialSnackBar(string text, int duration, string actionButtonText)
        : this(text, duration, true, actionButtonText, false)
    {
    }

    public MaterialSnackBar(string text, int duration, string actionButtonText, bool useAccentColor)
        : this(text, duration, true, actionButtonText, useAccentColor)
    {
    }

    /// <summary>
    /// Constructer Setting up the Layout
    /// </summary>
    public MaterialSnackBar(string text, int duration, bool showActionButton, string actionButtonText, bool useAccentColor)
    {
        Text = text;
        Duration = duration;
        TopMost = true;
        ShowInTaskbar = false;
        Sizable = false;

        BackColor = SkinManager.SnackBarBackgroundColor;
        FormStyle = FormStyles.StatusAndActionBar_None;

        ActionButtonText = actionButtonText;
        UseAccentColor = useAccentColor;
        Height = 48;
        MinimumSize = new Size(344, 48);
        MaximumSize = new Size(568, 48);

        ShowActionButton = showActionButton;

        Region = Region.FromHrgn(Functions.CreateRoundRectRgn(0, 0, Width, Height, 6, 6));

        _animationManager = new AnimationManager
        {
            AnimationType = AnimationType.EaseOut,
            Increment = 0.03
        };
        _animationManager.OnAnimationProgress += AnimationManager_OnAnimationProgress;

        _duration.Tick += Duration_Tick;

        _actionButton = new MaterialButton
        {
            AutoSize = false,
            NoAccentTextColor = SkinManager.SnackBarTextButtonNoAccentTextColor,
            DrawShadows = false,
            Type = MaterialButtonType.Text,
            UseAccentColor = UseAccentColor,
            Visible = ShowActionButton,
            Text = ActionButtonText
        };
        _actionButton.Click += (sender, e) =>
        {
            ActionButtonClick?.Invoke(this, new EventArgs());
            _closingAnimationDone = false;
            Close();
        };

        if (!Controls.Contains(_actionButton))
        {
            Controls.Add(_actionButton);
        }

        UpdateRects();
    }
    [Category("Action")]
    [Description("Fires when Action button is clicked")]
    public event EventHandler? ActionButtonClick;

    [Category("Material Skin"), DefaultValue(false), DisplayName("Use Accent Color")]
    public bool UseAccentColor { get; set { field = value; Invalidate(); } }

    /// <summary>
    /// Get or Set SnackBar show duration in milliseconds
    /// </summary>
    [Category("Material Skin"), DefaultValue(2000)]
    public int Duration { get => _duration.Interval; set => _duration.Interval = value; }

    /// <summary>
    /// The Text which gets displayed as the Content
    /// </summary>
    [Category("Material Skin"), DefaultValue("SnackBar text")]
    public new string Text { get; set { field = value; UpdateRects(); Invalidate(); } }

    [Category("Material Skin"), DefaultValue(false), DisplayName("Show Action Button")]
    public bool ShowActionButton { get; set { field = value; UpdateRects(); Invalidate(); } }

    /// <summary>
    /// The Text which gets displayed as the Content
    /// </summary>
    [Category("Material Skin"), DefaultValue("OK")]
    public string ActionButtonText { get; set { field = value; Invalidate(); } }

    private void UpdateRects()
    {
        if (ShowActionButton == true)
        {
            var _buttonWidth = TextRenderer.MeasureText(ActionButtonText, SkinManager.GetFontByType(FontType.Button)).Width + 32;
            var _actionbuttonBounds = new Rectangle(Width - _buttonPadding - _buttonWidth, _topPaddingSingleLine, _buttonWidth, _buttonHeight);
            _actionButton.Width = _actionbuttonBounds.Width;
            _actionButton.Height = _actionbuttonBounds.Height;
            _actionButton.Text = ActionButtonText;
            _actionButton.Top = _actionbuttonBounds.Top;
            _actionButton.UseAccentColor = UseAccentColor;
        }
        else
        {
            _actionButton.Width = 0;
        }

        _actionButton.Left = Width - _buttonPadding - _actionButton.Width;  //Button minimum width management
        _actionButton.Visible = ShowActionButton;
        Width = TextRenderer.MeasureText(Text, SkinManager.GetFontByType(FontType.Body2)).Width + (2 * _leftRightPadding) + _actionButton.Width + 48;
        Region = Region.FromHrgn(Functions.CreateRoundRectRgn(0, 0, Width, Height, 6, 6));
    }

    private void Duration_Tick(object? sender, EventArgs e)
    {
        _duration.Stop();
        _closingAnimationDone = false;
        Close();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        UpdateRects();
    }

    /// <summary>
    /// Sets up the Starting Location and starts the Animation
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (Owner == null)
            return;

        Location = new Point(Owner.Location.X + (Owner.Width / 2) - (Width / 2), Owner.Location.Y + Owner.Height - 60);
        _animationManager.StartNewAnimation(AnimationDirection.In);
        _duration.Start();
    }

    /// <summary>
    /// Animates the Form slides
    /// </summary>
    private void AnimationManager_OnAnimationProgress(object? sender, EventArgs e)
    {
        if (_closeAnimation)
        {
            Opacity = _animationManager.GetProgress();
        }
    }

    /// <summary>
    /// Ovverides the Paint to create the solid colored backcolor
    /// </summary>
    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.Clear(BackColor);

        // Calc text Rect
        var textRect = new Rectangle(
            _leftRightPadding,
            0,
            Width - (2 * _leftRightPadding) - _actionButton.Width,
            Height);

        //Draw  Text
        using var NativeText = new NativeTextRenderer(g);
        // Draw header text
        NativeText.DrawTransparentText(
            Text,
            SkinManager.GetLogFontByType(FontType.Body2),
            SkinManager.SnackBarTextHighEmphasisColor,
            textRect.Location,
            textRect.Size,
            TextAlignFlags.Left | TextAlignFlags.Middle);
    }

    // Overrides the Closing Event to Animate the Slide Out
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        e.Cancel = !_closingAnimationDone;
        if (!_closingAnimationDone)
        {
            _closeAnimation = true;
            _animationManager.Increment = 0.06;
            _animationManager.OnAnimationFinished += AnimationManager_OnAnimationFinished;
            _animationManager.StartNewAnimation(AnimationDirection.Out);
        }
        base.OnFormClosing(e);
    }

    // Closes the Form after the pull out animation
    private void AnimationManager_OnAnimationFinished(object? sender, EventArgs e)
    {
        _closingAnimationDone = true;
        Close();
    }

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
        _closingAnimationDone = false;
        Close();
    }

    // Prevents the Form from beeing dragged
    protected override void WndProc(ref Message message)
    {
        const int WM_SYSCOMMAND = 0x0112;
        const int SC_MOVE = 0xF010;

        switch (message.Msg)
        {
            case WM_SYSCOMMAND:
                var command = message.WParam.ToInt32() & 0xfff0;
                if (command == SC_MOVE)
                    return;
                break;
        }

        base.WndProc(ref message);
    }

    public new void Show()
    {
        if (Owner == null)
            throw new NotSupportedException();
    }
}
