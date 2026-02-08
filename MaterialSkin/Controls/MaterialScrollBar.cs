namespace MaterialSkin.Controls;

[DefaultEvent("Scroll")]
[DefaultProperty("Value")]
public class MaterialScrollBar : Control, IMaterialControl
{
    internal const int _scrollBarDefaultSize = 10;

    public const int WM_SETREDRAW = 0xb;

    private readonly Timer _progressTimer = new();
    private Timer? _autoHoverTimer;
    private bool _isHovered;
    private bool _isFirstScrollEventVertical = true;
    private bool _isFirstScrollEventHorizontal = true;
    private bool _inUpdate;
    private Rectangle _clickedBarRectangle;
    private Rectangle _thumbRectangle;
    private bool _topBarClicked;
    private bool _bottomBarClicked;
    private bool _thumbClicked;
    private int _thumbWidth = 6;
    private int _thumbHeight;
    private int _thumbBottomLimitBottom;
    private int _thumbBottomLimitTop;
    private int _thumbTopLimit;
    private int _thumbPosition;
    private int _trackPosition;
    private bool _dontUpdateColor;
    private ScrollOrientation _scrollOrientation = ScrollOrientation.VerticalScroll;
    private int _largeChange = 10;
    private int _curValue = 0;

    public event ScrollEventHandler? Scroll;

    public delegate void ScrollValueChangedDelegate(object sender, int newValue);
    public event ScrollValueChangedDelegate? ValueChanged;

    public MaterialScrollBar(MaterialScrollOrientation orientation)
        : this()
    {
        Orientation = orientation;
    }

    public MaterialScrollBar(MaterialScrollOrientation orientation, int width)
        : this(orientation)
    {
        Width = width;
    }

    public MaterialScrollBar()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.Selectable |
                 ControlStyles.SupportsTransparentBackColor |
                 ControlStyles.UserPaint, true);

        Width = _scrollBarDefaultSize;
        Height = 200;

        SetupScrollBar();

        _progressTimer.Interval = 20;
        _progressTimer.Tick += ProgressTimerTick;
    }

    [Browsable(false)]
    public int Depth { get; set; }

    [Browsable(false)]
    public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

    [Browsable(false)]
    public MouseState MouseState { get; set; }

    [Category("Material Skin"), DefaultValue(false), DisplayName("Use Accent Color")]
    public bool UseAccentColor { get; set { field = value; Invalidate(); } }

    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    private void OnScroll(ScrollEventType type, int oldValue, int newValue, ScrollOrientation orientation)
    {
        if (oldValue != newValue)
        {
            ValueChanged?.Invoke(this, _curValue);
        }

        if (Scroll == null)
        {
            return;
        }

        if (orientation == ScrollOrientation.HorizontalScroll)
        {
            if (type != ScrollEventType.EndScroll && _isFirstScrollEventHorizontal)
            {
                type = ScrollEventType.First;
            }
            else if (!_isFirstScrollEventHorizontal && type == ScrollEventType.EndScroll)
            {
                _isFirstScrollEventHorizontal = true;
            }
        }
        else
        {
            if (type != ScrollEventType.EndScroll && _isFirstScrollEventVertical)
            {
                type = ScrollEventType.First;
            }
            else if (!_isFirstScrollEventHorizontal && type == ScrollEventType.EndScroll)
            {
                _isFirstScrollEventVertical = true;
            }
        }

        Scroll(this, new ScrollEventArgs(type, oldValue, newValue, orientation));
    }

    [DefaultValue(10)]
    public int MouseWheelBarPartitions
    {
        get;
        set
        {
            if (value > 0)
            {
                field = value;
            }
            else
                throw new ArgumentOutOfRangeException(nameof(value), "MouseWheelBarPartitions has to be greather than zero");
        }
    } = 10;

    [DefaultValue(false)]
    public bool UseBarColor { get; set; }

    [DefaultValue(_scrollBarDefaultSize)]
    public int ScrollbarSize
    {
        get => Orientation == MaterialScrollOrientation.Vertical ? Width : Height;
        set
        {
            if (Orientation == MaterialScrollOrientation.Vertical)
            {
                Width = value;
            }
            else
            {
                Height = value;
            }
        }
    }

    [DefaultValue(false)]
    public bool HighlightOnWheel { get; set; } = false;

    public MaterialScrollOrientation Orientation
    {
        get; set
        {
            if (value == field) return;
            field = value;
            _scrollOrientation = value == MaterialScrollOrientation.Vertical ? ScrollOrientation.VerticalScroll : ScrollOrientation.HorizontalScroll;
            Size = new Size(Height, Width);
            SetupScrollBar();
        }
    } = MaterialScrollOrientation.Vertical;

    [DefaultValue(0)]
    public int Minimum
    {
        get; set
        {
            if (field == value || value < 0 || value >= Maximum)
            {
                return;
            }

            field = value;
            if (_curValue < value)
            {
                _curValue = value;
            }

            if (_largeChange > (Maximum - field))
            {
                _largeChange = Maximum - field;
            }

            SetupScrollBar();

            if (_curValue < value)
            {
                _dontUpdateColor = true;
                Value = value;
            }
            else
            {
                ChangeThumbPosition(GetThumbPosition());
                Refresh();
            }
        }
    } = 0;

    [DefaultValue(100)]
    public int Maximum
    {
        get; set
        {
            if (value == field || value < 1 || value <= Minimum)
            {
                return;
            }

            field = value;
            if (_largeChange > (field - Minimum))
            {
                _largeChange = field - Minimum;
            }

            SetupScrollBar();

            if (_curValue > value)
            {
                _dontUpdateColor = true;
                Value = field;
            }
            else
            {
                ChangeThumbPosition(GetThumbPosition());
                Refresh();
            }
        }
    } = 100;

    [DefaultValue(1)]
    public int SmallChange
    {
        get; set
        {
            if (value == field || value < 1 || value >= _largeChange)
            {
                return;
            }

            field = value;
            SetupScrollBar();
        }
    } = 1;

    [DefaultValue(10)]
    public int LargeChange
    {
        get => _largeChange;
        set
        {
            if (value == _largeChange || value < SmallChange || value < 2)
            {
                return;
            }

            if (value > (Maximum - Minimum))
            {
                _largeChange = Maximum - Minimum;
            }
            else
            {
                _largeChange = value;
            }

            SetupScrollBar();
        }
    }

    [DefaultValue(0)]
    [Browsable(false)]
    public int Value
    {
        get => _curValue;

        set
        {
            if (_curValue == value || value < Minimum || value > Maximum)
            {
                return;
            }

            _curValue = value;

            ChangeThumbPosition(GetThumbPosition());

            OnScroll(ScrollEventType.ThumbPosition, -1, value, _scrollOrientation);

            if (!_dontUpdateColor && HighlightOnWheel)
            {
                if (!_isHovered)
                    _isHovered = true;

                if (_autoHoverTimer == null)
                {
                    _autoHoverTimer = new Timer
                    {
                        Interval = 1000
                    };

                    _autoHoverTimer.Tick += AutoHoverTimer_Tick;
                    _autoHoverTimer.Start();
                }
                else
                {
                    _autoHoverTimer.Stop();
                    _autoHoverTimer.Start();
                }
            }
            else
            {
                _dontUpdateColor = false;
            }

            Refresh();
        }
    }

    private void AutoHoverTimer_Tick(object? sender, EventArgs e)
    {
        _isHovered = false;
        Invalidate();
        _autoHoverTimer?.Stop();
    }

    public bool HitTest(Point point) => _thumbRectangle.Contains(point);
    public void BeginUpdate()
    {
        SendMessage(Handle, WM_SETREDRAW, 0, 0);
        _inUpdate = true;
    }

    public void EndUpdate()
    {
        SendMessage(Handle, WM_SETREDRAW, 1, 0);
        _inUpdate = false;
        SetupScrollBar();
        Refresh();
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        if (Parent == null)
        {
            base.OnPaintBackground(e);
            return;
        }

        e.Graphics.Clear(Parent.BackColor);
    }

    protected override void OnPaint(PaintEventArgs e) => DrawScrollBar(e.Graphics, MaterialSkinManager.Instance.CardsColor, SkinManager.SwitchOffTrackColor, UseAccentColor ? MaterialSkinManager.Instance.ColorScheme.AccentColor : MaterialSkinManager.Instance.ColorScheme.PrimaryColor);
    private void DrawScrollBar(Graphics g, Color backColor, Color thumbColor, Color barColor)
    {
        if (UseBarColor)
        {
            using var b0 = new SolidBrush(barColor);
            g.FillRectangle(b0, ClientRectangle);
        }

        using var b1 = new SolidBrush(backColor);
        var thumbRect = new Rectangle(_thumbRectangle.X - 1, _thumbRectangle.Y - 1, _thumbRectangle.Width + 2, _thumbRectangle.Height + 2);
        g.FillRectangle(b1, thumbRect);

        using var b2 = new SolidBrush(_isHovered ? barColor : thumbColor);
        g.FillRectangle(b2, _thumbRectangle);
    }

    protected override void OnGotFocus(EventArgs e)
    {
        Invalidate();
        base.OnGotFocus(e);
    }

    protected override void OnLostFocus(EventArgs e)
    {
        _isHovered = false;
        Invalidate();
        base.OnLostFocus(e);
    }

    protected override void OnEnter(EventArgs e)
    {
        Invalidate();
        base.OnEnter(e);
    }

    protected override void OnLeave(EventArgs e)
    {
        _isHovered = false;
        Invalidate();
        base.OnLeave(e);
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);

        var v = e.Delta / 120 * (Maximum - Minimum) / MouseWheelBarPartitions;
        if (Orientation == MaterialScrollOrientation.Vertical)
        {
            Value -= v;
        }
        else
        {
            Value += v;
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            //isPressed = true;
            Invalidate();
        }

        base.OnMouseDown(e);

        Focus();

        if (e.Button == MouseButtons.Left)
        {
            var mouseLocation = e.Location;
            if (_thumbRectangle.Contains(mouseLocation))
            {
                _thumbClicked = true;
                _thumbPosition = Orientation == MaterialScrollOrientation.Vertical ? mouseLocation.Y - _thumbRectangle.Y : mouseLocation.X - _thumbRectangle.X;
                Invalidate(_thumbRectangle);
            }
            else
            {
                _trackPosition = Orientation == MaterialScrollOrientation.Vertical ? mouseLocation.Y : mouseLocation.X;
                if (_trackPosition < (Orientation == MaterialScrollOrientation.Vertical ? _thumbRectangle.Y : _thumbRectangle.X))
                {
                    _topBarClicked = true;
                }
                else
                {
                    _bottomBarClicked = true;
                }

                ProgressThumb(true);
            }
        }
        else if (e.Button == MouseButtons.Right)
        {
            _trackPosition = Orientation == MaterialScrollOrientation.Vertical ? e.Y : e.X;
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (e.Button == MouseButtons.Left)
        {
            if (_thumbClicked)
            {
                _thumbClicked = false;
                OnScroll(ScrollEventType.EndScroll, -1, _curValue, _scrollOrientation);
            }
            else if (_topBarClicked)
            {
                _topBarClicked = false;
                StopTimer();
            }
            else if (_bottomBarClicked)
            {
                _bottomBarClicked = false;
                StopTimer();
            }

            Invalidate();
        }
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        _isHovered = true;
        Invalidate();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        _isHovered = false;
        Invalidate();
        base.OnMouseLeave(e);
        ResetScrollStatus();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (e.Button == MouseButtons.Left)
        {
            if (_thumbClicked)
            {
                var oldScrollValue = _curValue;

                var pos = Orientation == MaterialScrollOrientation.Vertical ? e.Location.Y : e.Location.X;
                var thumbSize = Orientation == MaterialScrollOrientation.Vertical ? pos / Height / _thumbHeight : pos / Width / _thumbWidth;

                if (pos <= (_thumbTopLimit + _thumbPosition))
                {
                    ChangeThumbPosition(_thumbTopLimit);
                    _curValue = Minimum;
                    Invalidate();
                }
                else if (pos >= (_thumbBottomLimitTop + _thumbPosition))
                {
                    ChangeThumbPosition(_thumbBottomLimitTop);
                    _curValue = Maximum;
                    Invalidate();
                }
                else
                {
                    ChangeThumbPosition(pos - _thumbPosition);

                    int pixelRange, thumbPos;

                    if (Orientation == MaterialScrollOrientation.Vertical)
                    {
                        pixelRange = Height - thumbSize;
                        thumbPos = _thumbRectangle.Y;
                    }
                    else
                    {
                        pixelRange = Width - thumbSize;
                        thumbPos = _thumbRectangle.X;
                    }

                    var perc = 0f;

                    if (pixelRange != 0)
                    {
                        perc = thumbPos / (float)pixelRange;
                    }

                    _curValue = Convert.ToInt32((perc * (Maximum - Minimum)) + Minimum);
                }

                if (oldScrollValue != _curValue)
                {
                    OnScroll(ScrollEventType.ThumbTrack, oldScrollValue, _curValue, _scrollOrientation);
                    Refresh();
                }
            }
        }
        else if (!ClientRectangle.Contains(e.Location))
        {
            ResetScrollStatus();
        }
        else if (e.Button == MouseButtons.None)
        {
            if (_thumbRectangle.Contains(e.Location))
            {
                Invalidate(_thumbRectangle);
            }
            else if (ClientRectangle.Contains(e.Location))
            {
                Invalidate();
            }
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        _isHovered = true;
        Invalidate();
        base.OnKeyDown(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        _isHovered = false;
        Invalidate();
        base.OnKeyUp(e);
    }

    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
        base.SetBoundsCore(x, y, width, height, specified);

        if (DesignMode)
        {
            SetupScrollBar();
        }
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        SetupScrollBar();
    }

    protected override bool ProcessDialogKey(Keys keyData)
    {
        var keyUp = Keys.Up;
        var keyDown = Keys.Down;

        if (Orientation == MaterialScrollOrientation.Horizontal)
        {
            keyUp = Keys.Left;
            keyDown = Keys.Right;
        }

        if (keyData == keyUp)
        {
            Value -= SmallChange;
            return true;
        }

        if (keyData == keyDown)
        {
            Value += SmallChange;
            return true;
        }

        if (keyData == Keys.PageUp)
        {
            Value = GetValue(false, true);
            return true;
        }

        if (keyData == Keys.PageDown)
        {
            if (_curValue + _largeChange > Maximum)
            {
                Value = Maximum;
            }
            else
            {
                Value += _largeChange;
            }
            return true;
        }

        if (keyData == Keys.Home)
        {
            Value = Minimum;
            return true;
        }

        if (keyData == Keys.End)
        {
            Value = Maximum;
            return true;
        }

        return base.ProcessDialogKey(keyData);
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);
        Invalidate();
    }

    private void SetupScrollBar()
    {
        if (_inUpdate) return;

        if (Orientation == MaterialScrollOrientation.Vertical)
        {
            _thumbWidth = Width > 0 ? Width : 10;
            _thumbHeight = GetThumbSize();

            _clickedBarRectangle = ClientRectangle;
            _clickedBarRectangle.Inflate(-1, -1);

            _thumbRectangle = new Rectangle(ClientRectangle.X, ClientRectangle.Y, _thumbWidth, _thumbHeight);

            _thumbPosition = _thumbRectangle.Height / 2;
            _thumbBottomLimitBottom = ClientRectangle.Bottom;
            _thumbBottomLimitTop = _thumbBottomLimitBottom - _thumbRectangle.Height;
            _thumbTopLimit = ClientRectangle.Y;
        }
        else
        {
            _thumbHeight = Height > 0 ? Height : 10;
            _thumbWidth = GetThumbSize();

            _clickedBarRectangle = ClientRectangle;
            _clickedBarRectangle.Inflate(-1, -1);

            _thumbRectangle = new Rectangle(ClientRectangle.X, ClientRectangle.Y, _thumbWidth, _thumbHeight);

            _thumbPosition = _thumbRectangle.Width / 2;
            _thumbBottomLimitBottom = ClientRectangle.Right;
            _thumbBottomLimitTop = _thumbBottomLimitBottom - _thumbRectangle.Width;
            _thumbTopLimit = ClientRectangle.X;
        }

        ChangeThumbPosition(GetThumbPosition());
        Refresh();
    }

    private void ResetScrollStatus()
    {
        _bottomBarClicked = _topBarClicked = false;
        StopTimer();
        Refresh();
    }

    private void ProgressTimerTick(object? sender, EventArgs e) => ProgressThumb(true);
    private int GetValue(bool smallIncrement, bool up)
    {
        int newValue;
        if (up)
        {
            newValue = _curValue - (smallIncrement ? SmallChange : _largeChange);

            if (newValue < Minimum)
            {
                newValue = Minimum;
            }
        }
        else
        {
            newValue = _curValue + (smallIncrement ? SmallChange : _largeChange);

            if (newValue > Maximum)
            {
                newValue = Maximum;
            }
        }

        return newValue;
    }

    private int GetThumbPosition()
    {
        if (_thumbHeight == 0 || _thumbWidth == 0)
            return 0;

        var thumbSize = Orientation == MaterialScrollOrientation.Vertical ? _thumbPosition / Height / _thumbHeight : _thumbPosition / Width / _thumbWidth;
        int pixelRange;
        if (Orientation == MaterialScrollOrientation.Vertical)
        {
            pixelRange = Height - thumbSize;
        }
        else
        {
            pixelRange = Width - thumbSize;
        }

        var realRange = Maximum - Minimum;
        var perc = 0f;
        if (realRange != 0)
        {
            perc = (_curValue - (float)Minimum) / realRange;
        }

        return Math.Max(_thumbTopLimit, Math.Min(_thumbBottomLimitTop, Convert.ToInt32(perc * pixelRange)));
    }

    private int GetThumbSize()
    {
        var trackSize = Orientation == MaterialScrollOrientation.Vertical ? Height : Width;
        if (Maximum == 0 || _largeChange == 0)
            return trackSize;

        var newThumbSize = _largeChange * (float)trackSize / Maximum;
        return (int)Math.Min(trackSize, Math.Max(newThumbSize, 10f));
    }

    private void EnableTimer()
    {
        if (!_progressTimer.Enabled)
        {
            _progressTimer.Interval = 600;
            _progressTimer.Start();
        }
        else
        {
            _progressTimer.Interval = 10;
        }
    }

    private void StopTimer() => _progressTimer.Stop();
    private void ChangeThumbPosition(int position)
    {
        if (Orientation == MaterialScrollOrientation.Vertical)
        {
            _thumbRectangle.Y = position;
        }
        else
        {
            _thumbRectangle.X = position;
        }
    }

    private void ProgressThumb(bool enableTimer)
    {
        var scrollOldValue = _curValue;
        var type = ScrollEventType.First;
        int thumbSize, thumbPos;

        if (Orientation == MaterialScrollOrientation.Vertical)
        {
            thumbPos = _thumbRectangle.Y;
            thumbSize = _thumbRectangle.Height;
        }
        else
        {
            thumbPos = _thumbRectangle.X;
            thumbSize = _thumbRectangle.Width;
        }

        if (_bottomBarClicked && (thumbPos + thumbSize) < _trackPosition)
        {
            type = ScrollEventType.LargeIncrement;
            _curValue = GetValue(false, false);
            if (_curValue == Maximum)
            {
                ChangeThumbPosition(_thumbBottomLimitTop);
                type = ScrollEventType.Last;
            }
            else
            {
                ChangeThumbPosition(Math.Min(_thumbBottomLimitTop, GetThumbPosition()));
            }
        }
        else if (_topBarClicked && thumbPos > _trackPosition)
        {
            type = ScrollEventType.LargeDecrement;
            _curValue = GetValue(false, true);
            if (_curValue == Minimum)
            {
                ChangeThumbPosition(_thumbTopLimit);
                type = ScrollEventType.First;
            }
            else
            {
                ChangeThumbPosition(Math.Max(_thumbTopLimit, GetThumbPosition()));
            }
        }

        if (scrollOldValue != _curValue)
        {
            OnScroll(type, scrollOldValue, _curValue, _scrollOrientation);
            Invalidate();

            if (enableTimer)
            {
                EnableTimer();
            }
        }
    }
}
