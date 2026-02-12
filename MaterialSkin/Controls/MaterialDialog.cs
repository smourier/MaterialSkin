namespace MaterialSkin.Controls;

public class MaterialDialog : MaterialForm
{
    private const int _leftRightPadding = 24;
    private const int _buttonPadding = 8;
    private const int _buttonHeight = 36;
    private const int _textTopPadding = 17;
    private const int _textBottomPadding = 28;
    private readonly int _header_Height = 40;

    private readonly MaterialButton _validationButton = new();
    private readonly MaterialButton _cancelButton = new();
    private readonly AnimationManager _animationManager;
    private readonly Form _formOverlay;
    private readonly string _text;
    private readonly string _title;

    public MaterialDialog(Form parentForm, string text)
        : this(parentForm, null, text, null, false, null, false)
    {
    }

    public MaterialDialog(Form parentForm, string? title, string text)
        : this(parentForm, title, text, null, false, null, false)
    {
    }

    public MaterialDialog(Form parentForm, string? title, string text, string validationButtonText)
        : this(parentForm, title, text, validationButtonText, false, null, false)
    {
    }

    public MaterialDialog(Form parentForm, string? title, string text, bool showCancelButton)
        : this(parentForm, title, text, null, showCancelButton, null, false)
    {
    }

    public MaterialDialog(Form parentForm, string? title, string text, bool showCancelButton, string? cancelButtonText)
        : this(parentForm, title, text, null, showCancelButton, cancelButtonText, false)
    {
    }

    public MaterialDialog(Form parentForm, string? title, string text, string? validationButtonText, bool showCancelButton, string? cancelButtonText)
        : this(parentForm, title, text, validationButtonText, showCancelButton, cancelButtonText, false)
    {
    }

    public MaterialDialog(Form parentForm, string? title, string text, string? validationButtonText, bool showCancelButton, string? cancelButtonText, bool useAccentColor)
    {
        ArgumentNullException.ThrowIfNull(parentForm);
        var asm = Assembly.GetEntryAssembly();
        title ??= asm?.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ??
            asm?.GetCustomAttribute<AssemblyProductAttribute>()?.Product ??
            asm?.GetName().Name ??
            string.Empty;

        text ??= string.Empty;
        validationButtonText ??= FlexibleMaterialForm.GetButtonText(ButtonId.Ok);
        cancelButtonText ??= FlexibleMaterialForm.GetButtonText(ButtonId.Cancel);

        _formOverlay = new Form
        {
            BackColor = Color.Black,
            Opacity = 0.5,
            MinimizeBox = false,
            MaximizeBox = false,
            Text = string.Empty,
            ShowIcon = false,
            ControlBox = false,
            FormBorderStyle = FormBorderStyle.None,
            Size = new Size(parentForm.Width, parentForm.Height),
            ShowInTaskbar = false,
            Owner = parentForm,
            Visible = true,
            Location = new Point(parentForm.Location.X, parentForm.Location.Y),
            Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom,
        };

        _title = title;
        if (title.Length == 0)
        {
            _header_Height = 0;
        }
        else
        {
            _header_Height = 40;
        }

        _text = text;
        ShowInTaskbar = false;
        Sizable = false;

        BackColor = MaterialSkinManager.Instance.BackgroundColor;
        FormStyle = FormStyles.StatusAndActionBar_None;

        _animationManager = new AnimationManager
        {
            AnimationType = AnimationType.EaseOut,
            Increment = 0.03
        };

        _validationButton = new MaterialButton
        {
            AutoSize = false,
            DialogResult = DialogResult.OK,
            DrawShadows = false,
            Type = MaterialButtonType.Text,
            UseAccentColor = useAccentColor,
            Text = validationButtonText
        };

        _cancelButton = new MaterialButton
        {
            AutoSize = false,
            DialogResult = DialogResult.Cancel,
            DrawShadows = false,
            Type = MaterialButtonType.Text,
            UseAccentColor = useAccentColor,
            Visible = showCancelButton,
            Text = cancelButtonText
        };

        AcceptButton = _validationButton;
        CancelButton = _cancelButton;

        if (!Controls.Contains(_validationButton))
        {
            Controls.Add(_validationButton);
        }

        if (!Controls.Contains(_cancelButton))
        {
            Controls.Add(_cancelButton);
        }

        Width = 560;
        var TextWidth = TextRenderer.MeasureText(_text, MaterialSkinManager.Instance.GetFontByType(FontType.Body1)).Width;
        var RectWidth = Width - (2 * _leftRightPadding) - _buttonPadding;
        var RectHeight = ((TextWidth / RectWidth) + 1) * 19;
        var textRect = new Rectangle(
            _leftRightPadding,
            _header_Height + _textTopPadding,
            RectWidth,
            RectHeight + 9);

        Height = _header_Height + _textTopPadding + textRect.Height + _textBottomPadding + 52; //560;
        Region = Region.FromHrgn(Functions.CreateRoundRectRgn(0, 0, Width, Height, 6, 6));

        var _buttonWidth = TextRenderer.MeasureText(validationButtonText, MaterialSkinManager.Instance.GetFontByType(FontType.Button)).Width + 32;
        var _validationbuttonBounds = new Rectangle(Width - _buttonPadding - _buttonWidth, Height - _buttonPadding - _buttonHeight, _buttonWidth, _buttonHeight);
        _validationButton.Width = _validationbuttonBounds.Width;
        _validationButton.Height = _validationbuttonBounds.Height;
        _validationButton.Top = _validationbuttonBounds.Top;
        _validationButton.Left = _validationbuttonBounds.Left;  //Button minimum width management
        _validationButton.Visible = true;

        _buttonWidth = TextRenderer.MeasureText(cancelButtonText, MaterialSkinManager.Instance.GetFontByType(FontType.Button)).Width + 32;
        var _cancelbuttonBounds = new Rectangle(_validationbuttonBounds.Left - _buttonPadding - _buttonWidth, Height - _buttonPadding - _buttonHeight, _buttonWidth, _buttonHeight);
        _cancelButton.Width = _cancelbuttonBounds.Width;
        _cancelButton.Height = _cancelbuttonBounds.Height;
        _cancelButton.Top = _cancelbuttonBounds.Top;
        _cancelButton.Left = _cancelbuttonBounds.Left;  //Button minimum width management
    }

    /// <summary>
    /// Sets up the Starting Location and starts the Animation
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (Owner != null)
        {
            Location = new Point(Owner.Location.X + (Owner.Width / 2) - (Width / 2), Owner.Location.Y + (Owner.Height / 2) - (Height / 2));
        }

        _animationManager.StartNewAnimation(AnimationDirection.In);
    }

    /// <summary>
    /// Ovverides the Paint to create the solid colored backcolor
    /// </summary>
    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.Clear(BackColor);

        // Calc title Rect
        var titleRect = new Rectangle(
            _leftRightPadding,
            0,
            Width - (2 * _leftRightPadding),
            _header_Height);

        //Draw title
        using var renderer = new NativeTextRenderer(g);
        // Draw header text
        renderer.DrawTransparentText(
            _title,
            MaterialSkinManager.Instance.GetLogFontByType(FontType.H6),
            MaterialSkinManager.Instance.TextHighEmphasisColor,
            titleRect.Location,
            titleRect.Size,
            TextAlignFlags.Left | TextAlignFlags.Bottom);

        // Calc text Rect

        var TextWidth = TextRenderer.MeasureText(_text, MaterialSkinManager.Instance.GetFontByType(FontType.Body1)).Width;
        var RectWidth = Width - (2 * _leftRightPadding) - _buttonPadding;
        var RectHeight = ((TextWidth / RectWidth) + 1) * 19;

        var textRect = new Rectangle(
            _leftRightPadding,
            _header_Height + 17,
            RectWidth,
            RectHeight + 19);

        //Draw  Text
        // Draw header text
        renderer.DrawMultilineTransparentText(
            _text,
            MaterialSkinManager.Instance.GetLogFontByType(FontType.Body1),
            MaterialSkinManager.Instance.TextHighEmphasisColor,
            textRect.Location,
            textRect.Size,
            TextAlignFlags.Left | TextAlignFlags.Middle);
    }

    // Overrides the Closing Event to Animate the Slide Out
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _formOverlay.Visible = false;
        _formOverlay.Close();
        _formOverlay.Dispose();
        _ = DialogResult;
        base.OnFormClosing(e);
    }

    protected override bool ProcessDialogKey(Keys keyData)
    {
        if (ModifierKeys == Keys.None && keyData == Keys.Escape)
        {
            Close();
            return true;
        }
        return base.ProcessDialogKey(keyData);
    }

    /// <summary>
    /// Prevents the Form from being dragged
    /// </summary>
    protected override void WndProc(ref Message message)
    {
        switch (message.Msg)
        {
            case Constants.WM_SYSCOMMAND:
                var command = message.WParam.ToInt32() & 0xfff0;
                if (command == Constants.SC_MOVE)
                    return;
                break;
        }

        base.WndProc(ref message);
    }
}
