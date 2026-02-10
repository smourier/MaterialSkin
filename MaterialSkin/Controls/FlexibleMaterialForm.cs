namespace MaterialSkin.Controls;

/// <summary>
/// The form to show the customized message box.
/// </summary>
public partial class FlexibleMaterialForm : MaterialForm, IMaterialControl
{
    //These separators are used for the "copy to clipboard" standard operation, triggered by Ctrl + C (behavior and clipboard format is like in a standard MessageBox)
    private static readonly string _standardMessageBoxSeparatorLines = "---------------------------\n";
    private static readonly string _standardMessageBoxSeparatorSpaces = "   ";

    /// <summary>
    /// Defines the font for all FlexibleMessageBox instances.
    ///
    /// Default is: SystemFonts.MessageBoxFont
    /// </summary>
    private static readonly Font _font = MaterialSkinManager.Instance.GetFontByType(FontType.Body1);

    /// <summary>
    /// Defines the maximum width for all FlexibleMessageBox instances in percent of the working area.
    ///
    /// Allowed values are 0.2 - 1.0 where:
    /// 0.2 means:  The FlexibleMessageBox can be at most half as wide as the working area.
    /// 1.0 means:  The FlexibleMessageBox can be as wide as the working area.
    ///
    /// Default is: 70% of the working area width.
    /// </summary>
    private const double _maxWidthFactor = 0.7;

    /// <summary>
    /// Defines the maximum height for all FlexibleMessageBox instances in percent of the working area.
    ///
    /// Allowed values are 0.2 - 1.0 where:
    /// 0.2 means:  The FlexibleMessageBox can be at most half as high as the working area.
    /// 1.0 means:  The FlexibleMessageBox can be as high as the working area.
    ///
    /// Default is: 90% of the working area height.
    /// </summary>
    private const double _maxHeightFactor = 0.9;

    private readonly MaterialSkinManager _materialSkinManager;
    private MaterialMultiLineTextBox _richTextBoxMessage;
    private MaterialLabel _materialLabel1;
    private MaterialButton _leftButton;
    private MaterialButton _middleButton;
    private MaterialButton _rightButton;
    private Container _components;
    private BindingSource _flexibleMaterialFormBindingSource;
    private Panel _messageContainer;
    private PictureBox _pictureBoxForIcon;
    private MessageBoxDefaultButton _defaultButton;
    private int _visibleButtonsCount;

    private FlexibleMaterialForm()
    {
        InitializeComponent();

        KeyPreview = true;
        KeyUp += FlexibleMaterialForm_KeyUp;

        _materialSkinManager = MaterialSkinManager.Instance;
        _materialSkinManager.AddFormToManage(this);
        _messageContainer.BackColor = BackColor;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _components?.Dispose();
        }
        base.Dispose(disposing);
    }

    [MemberNotNull(
        nameof(_components),
        nameof(_richTextBoxMessage),
        nameof(_materialLabel1),
        nameof(_leftButton),
        nameof(_middleButton),
        nameof(_rightButton),
        nameof(_flexibleMaterialFormBindingSource),
        nameof(_messageContainer),
        nameof(_pictureBoxForIcon))]
    private void InitializeComponent()
    {
        _components = new Container();
        _flexibleMaterialFormBindingSource = new BindingSource(_components);
        _messageContainer = new Panel();
        _materialLabel1 = new MaterialLabel();
        _pictureBoxForIcon = new PictureBox();
        _richTextBoxMessage = new MaterialMultiLineTextBox();
        _leftButton = new MaterialButton();
        _middleButton = new MaterialButton();
        _rightButton = new MaterialButton();
        ((ISupportInitialize)_flexibleMaterialFormBindingSource).BeginInit();
        _messageContainer.SuspendLayout();
        ((ISupportInitialize)_pictureBoxForIcon).BeginInit();
        SuspendLayout();
        // 
        // messageContainer
        // 
        _messageContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom
        | AnchorStyles.Left
        | AnchorStyles.Right;
        _messageContainer.BackColor = Color.White;
        _messageContainer.Controls.Add(_materialLabel1);
        _messageContainer.Controls.Add(_pictureBoxForIcon);
        _messageContainer.Controls.Add(_richTextBoxMessage);
        _messageContainer.Location = new Point(1, 65);
        _messageContainer.Name = "messageContainer";
        _messageContainer.Size = new Size(382, 89);
        _messageContainer.TabIndex = 1;
        // 
        // materialLabel1
        // 
        _materialLabel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom
        | AnchorStyles.Left
        | AnchorStyles.Right;
        _materialLabel1.DataBindings.Add(new Binding("Text", _flexibleMaterialFormBindingSource, "MessageText", true, DataSourceUpdateMode.OnPropertyChanged));
        _materialLabel1.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
        _materialLabel1.Location = new Point(56, 12);
        _materialLabel1.MouseState = MouseState.HOVER;
        _materialLabel1.Name = "materialLabel1";
        _materialLabel1.Size = new Size(314, 65);
        _materialLabel1.TabIndex = 9;
        _materialLabel1.Text = $"<{Functions.LoadDllString("propsys.dll", 39092) ?? "Message"}>";
        _materialLabel1.Visible = false;
        // 
        // pictureBoxForIcon
        // 
        _pictureBoxForIcon.BackColor = Color.Transparent;
        _pictureBoxForIcon.Location = new Point(12, 12);
        _pictureBoxForIcon.Name = "pictureBoxForIcon";
        _pictureBoxForIcon.Size = new Size(32, 32);
        _pictureBoxForIcon.TabIndex = 8;
        _pictureBoxForIcon.TabStop = false;
        // 
        // richTextBoxMessage
        // 
        _richTextBoxMessage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom
        | AnchorStyles.Left
        | AnchorStyles.Right;
        _richTextBoxMessage.BackColor = Color.FromArgb((byte)255, (byte)255, (byte)255);
        _richTextBoxMessage.BorderStyle = BorderStyle.None;
        _richTextBoxMessage.DataBindings.Add(new Binding("Text", _flexibleMaterialFormBindingSource, "MessageText", true, DataSourceUpdateMode.OnPropertyChanged));
        _richTextBoxMessage.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
        _richTextBoxMessage.ForeColor = Color.FromArgb((byte)222, (byte)0, (byte)0, (byte)0);
        _richTextBoxMessage.Location = new Point(56, 12);
        _richTextBoxMessage.Margin = new Padding(0);
        _richTextBoxMessage.MouseState = MouseState.HOVER;
        _richTextBoxMessage.Name = "richTextBoxMessage";
        _richTextBoxMessage.ReadOnly = true;
        _richTextBoxMessage.ScrollBars = RichTextBoxScrollBars.Vertical;
        _richTextBoxMessage.Size = new Size(314, 65);
        _richTextBoxMessage.TabIndex = 0;
        _richTextBoxMessage.TabStop = false;
        _richTextBoxMessage.Text = $"<{Functions.LoadDllString("propsys.dll", 39092) ?? "Message"}>";
        _richTextBoxMessage.LinkClicked += RichTextBoxMessage_LinkClicked;
        // 
        // leftButton
        // 
        _leftButton.Anchor = AnchorStyles.Bottom;
        _leftButton.AutoSize = false;
        _leftButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _leftButton.Density = MaterialButtonDensity.Default;
        _leftButton.DialogResult = DialogResult.OK;
        _leftButton.HighEmphasis = false;
        _leftButton.Icon = null;
        _leftButton.Location = new Point(32, 163);
        _leftButton.Margin = new Padding(4, 6, 4, 6);
        _leftButton.MinimumSize = new Size(0, 24);
        _leftButton.MouseState = MouseState.HOVER;
        _leftButton.Name = "leftButton";
        _leftButton.Size = new Size(108, 36);
        _leftButton.TabIndex = 14;
        _leftButton.Text = GetButtonText(ButtonId.Ok);
        _leftButton.Type = MaterialButtonType.Text;
        _leftButton.UseAccentColor = false;
        _leftButton.UseVisualStyleBackColor = true;
        _leftButton.Visible = false;
        // 
        // middleButton
        // 
        _middleButton.Anchor = AnchorStyles.Bottom;
        _middleButton.AutoSize = false;
        _middleButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _middleButton.Density = MaterialButtonDensity.Default;
        _middleButton.DialogResult = DialogResult.OK;
        _middleButton.HighEmphasis = true;
        _middleButton.Icon = null;
        _middleButton.Location = new Point(148, 163);
        _middleButton.Margin = new Padding(4, 6, 4, 6);
        _middleButton.MinimumSize = new Size(0, 24);
        _middleButton.MouseState = MouseState.HOVER;
        _middleButton.Name = "middleButton";
        _middleButton.Size = new Size(102, 36);
        _middleButton.TabIndex = 15;
        _middleButton.Text = GetButtonText(ButtonId.Ok);
        _middleButton.Type = MaterialButtonType.Text;
        _middleButton.UseAccentColor = false;
        _middleButton.UseVisualStyleBackColor = true;
        _middleButton.Visible = false;
        // 
        // rightButton
        // 
        _rightButton.Anchor = AnchorStyles.Bottom;
        _rightButton.AutoSize = false;
        _rightButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _rightButton.Density = MaterialButtonDensity.Default;
        _rightButton.DialogResult = DialogResult.OK;
        _rightButton.HighEmphasis = true;
        _rightButton.Icon = null;
        _rightButton.Location = new Point(258, 163);
        _rightButton.Margin = new Padding(4, 6, 4, 6);
        _rightButton.MinimumSize = new Size(0, 24);
        _rightButton.MouseState = MouseState.HOVER;
        _rightButton.Name = "rightButton";
        _rightButton.Size = new Size(106, 36);
        _rightButton.TabIndex = 13;
        _rightButton.Text = GetButtonText(ButtonId.Ok);
        _rightButton.Type = MaterialButtonType.Contained;
        _rightButton.UseAccentColor = false;
        _rightButton.UseVisualStyleBackColor = true;
        _rightButton.Visible = false;
        // 
        // FlexibleMaterialForm
        // 
        BackColor = Color.White;
        ClientSize = new Size(384, 208);
        Controls.Add(_leftButton);
        Controls.Add(_middleButton);
        Controls.Add(_rightButton);
        Controls.Add(_messageContainer);
        DataBindings.Add(new Binding("Text", _flexibleMaterialFormBindingSource, "CaptionText", true));
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(276, 140);
        Name = "FlexibleMaterialForm";
        ShowIcon = false;
        SizeGripStyle = SizeGripStyle.Show;
        StartPosition = FormStartPosition.CenterParent;
        Text = $"<{Functions.LoadDllString("propsys.dll", 38662) ?? "Title"}>";
        Shown += FlexibleMaterialForm_Shown;
        ((ISupportInitialize)_flexibleMaterialFormBindingSource).EndInit();
        _messageContainer.ResumeLayout(false);
        ((ISupportInitialize)_pictureBoxForIcon).EndInit();
        ResumeLayout(false);
    }

    /// <summary>
    /// Gets or sets the CaptionText
    /// The text that is been used for the heading.
    /// </summary>
    public string? CaptionText { get; set; }

    /// <summary>
    /// Gets or sets the MessageText
    /// The text that is been used in the FlexibleMaterialForm.
    /// </summary>
    public string? MessageText { get; set; }

    /// <summary>
    /// Gets the string rows.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>The string rows as 1-dimensional array</returns>
    private static string[]? GetStringRows(string message)
    {
        if (string.IsNullOrEmpty(message))
            return null;

        var messageRows = message.Split(['\n'], StringSplitOptions.None);
        return messageRows;
    }

    /// <summary>
    /// Gets the button text for the CurrentUICulture language.
    /// </summary>
    /// <param name="buttonID">The ID of the button.</param>
    /// <returns>The button text</returns>
    public static string GetButtonText(ButtonId buttonID) => Functions.LoadDllString("user32.dll", (uint)buttonID)?.Replace("&", string.Empty) ?? buttonID.ToString();

    /// <summary>
    /// Ensure the given working area factor in the range of  0.2 - 1.0 where:
    ///
    /// 0.2 means:  20 percent of the working area height or width.
    /// 1.0 means:  100 percent of the working area height or width.
    /// </summary>
    /// <param name="workingAreaFactor">The given working area factor.</param>
    /// <returns>The corrected given working area factor.</returns>
    private static double GetCorrectedWorkingAreaFactor(double workingAreaFactor)
    {
        const double minFactor = 0.2;
        const double maxFactor = 1;

        if (workingAreaFactor < minFactor)
            return minFactor;

        if (workingAreaFactor > maxFactor)
            return maxFactor;

        return workingAreaFactor;
    }

    // Set the dialogs start position when given. Otherwise center the dialog on the current screen.
    private static void SetDialogStartPosition(FlexibleMaterialForm FlexibleMaterialForm, IWin32Window? owner)
    {
        //If no owner given: Center on current screen
        if (owner == null)
        {
            var screen = Screen.FromPoint(Cursor.Position);
            FlexibleMaterialForm.StartPosition = FormStartPosition.Manual;
            FlexibleMaterialForm.Left = screen.Bounds.Left + screen.Bounds.Width / 2 - FlexibleMaterialForm.Width / 2;
            FlexibleMaterialForm.Top = screen.Bounds.Top + screen.Bounds.Height / 2 - FlexibleMaterialForm.Height / 2;
        }
    }

    /// <summary>
    /// Calculate the dialogs start size (Try to auto-size width to show longest text row).
    /// Also set the maximum dialog size.
    /// </summary>
    /// <param name="FlexibleMaterialForm">The FlexibleMessageBox dialog.</param>
    /// <param name="text">The text (the longest text row is used to calculate the dialog width).</param>
    /// <param name="caption">The caption<see cref="string"/></param>
    private static void SetDialogSizes(FlexibleMaterialForm FlexibleMaterialForm, string text, string caption)
    {
        //First set the bounds for the maximum dialog size
        FlexibleMaterialForm.MaximumSize = new Size(
            (int)(SystemInformation.WorkingArea.Width * GetCorrectedWorkingAreaFactor(_maxWidthFactor)),
            (int)(SystemInformation.WorkingArea.Height * GetCorrectedWorkingAreaFactor(_maxHeightFactor)));

        //Get rows. Exit if there are no rows to render...
        var stringRows = GetStringRows(text);
        if (stringRows == null)
            return;

        //Calculate whole text height
        var textHeight = Math.Min(TextRenderer.MeasureText(text, _font).Height, 600);

        //Calculate width for longest text line
        const int scrollBarWidthOffset = 15;
        var longestTextRowWidth = stringRows.Max(textForRow => TextRenderer.MeasureText(textForRow, _font).Width);
        var captionWidth = TextRenderer.MeasureText(caption, SystemFonts.CaptionFont).Width;
        var textWidth = Math.Max(longestTextRowWidth + scrollBarWidthOffset, captionWidth);

        //Calculate margins
        var marginWidth = FlexibleMaterialForm.Width - FlexibleMaterialForm._richTextBoxMessage.Width;
        var marginHeight = FlexibleMaterialForm.Height - FlexibleMaterialForm._richTextBoxMessage.Height;

        var minimumHeight = FlexibleMaterialForm._messageContainer.Top + FlexibleMaterialForm._pictureBoxForIcon.Height + 2 * 8 + 54;
        if (marginHeight < minimumHeight) marginHeight = minimumHeight;

        //Set calculated dialog size (if the calculated values exceed the maximums, they were cut by windows forms automatically)
        FlexibleMaterialForm.Size = new Size(textWidth + marginWidth,
                                               textHeight + marginHeight);
    }

    /// <summary>
    /// Set the dialogs icon.
    /// When no icon is used: Correct placement and width of rich text box.
    /// </summary>
    /// <param name="FlexibleMaterialForm">The FlexibleMessageBox dialog.</param>
    /// <param name="icon">The MessageBoxIcon.</param>
    private static void SetDialogIcon(FlexibleMaterialForm FlexibleMaterialForm, MessageBoxIcon icon)
    {
        switch (icon)
        {
            case MessageBoxIcon.Information:
                FlexibleMaterialForm._pictureBoxForIcon.Image = SystemIcons.Information.ToBitmap();
                break;

            case MessageBoxIcon.Warning:
                FlexibleMaterialForm._pictureBoxForIcon.Image = SystemIcons.Warning.ToBitmap();
                break;

            case MessageBoxIcon.Error:
                FlexibleMaterialForm._pictureBoxForIcon.Image = SystemIcons.Error.ToBitmap();
                break;

            case MessageBoxIcon.Question:
                FlexibleMaterialForm._pictureBoxForIcon.Image = SystemIcons.Question.ToBitmap();
                break;

            default:
                //When no icon is used: Correct placement and width of rich text box.
                FlexibleMaterialForm._pictureBoxForIcon.Visible = false;
                FlexibleMaterialForm._richTextBoxMessage.Left -= FlexibleMaterialForm._pictureBoxForIcon.Width;
                FlexibleMaterialForm._richTextBoxMessage.Width += FlexibleMaterialForm._pictureBoxForIcon.Width;
                break;
        }
    }

    /// <summary>
    /// Set dialog buttons visibilities and texts.
    /// Also set a default button.
    /// </summary>
    /// <param name="FlexibleMaterialForm">The FlexibleMessageBox dialog.</param>
    /// <param name="buttons">The buttons.</param>
    /// <param name="defaultButton">The default button.</param>
    private static void SetDialogButtons(FlexibleMaterialForm FlexibleMaterialForm, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, ButtonsPosition buttonsPosition)
    {
        //Set the buttons visibilities and texts
        switch (buttons)
        {
            case MessageBoxButtons.AbortRetryIgnore:
                FlexibleMaterialForm._visibleButtonsCount = 3;

                FlexibleMaterialForm._leftButton.Visible = true;
                FlexibleMaterialForm._leftButton.Text = GetButtonText(ButtonId.Abort);
                FlexibleMaterialForm._leftButton.DialogResult = DialogResult.Abort;

                FlexibleMaterialForm._middleButton.Visible = true;
                FlexibleMaterialForm._middleButton.Text = GetButtonText(ButtonId.Retry);
                FlexibleMaterialForm._middleButton.DialogResult = DialogResult.Retry;

                FlexibleMaterialForm._rightButton.Visible = true;
                FlexibleMaterialForm._rightButton.Text = GetButtonText(ButtonId.Ignore);
                FlexibleMaterialForm._rightButton.DialogResult = DialogResult.Ignore;

                FlexibleMaterialForm.ControlBox = false;
                break;

            case MessageBoxButtons.OKCancel:
                FlexibleMaterialForm._visibleButtonsCount = 2;

                FlexibleMaterialForm._middleButton.Visible = true;
                FlexibleMaterialForm._middleButton.Text = GetButtonText(ButtonId.Cancel);
                FlexibleMaterialForm._middleButton.DialogResult = DialogResult.Cancel;

                FlexibleMaterialForm._rightButton.Visible = true;
                FlexibleMaterialForm._rightButton.Text = GetButtonText(ButtonId.Ok);
                FlexibleMaterialForm._rightButton.DialogResult = DialogResult.OK;

                FlexibleMaterialForm.CancelButton = FlexibleMaterialForm._middleButton;
                break;

            case MessageBoxButtons.RetryCancel:
                FlexibleMaterialForm._visibleButtonsCount = 2;

                FlexibleMaterialForm._middleButton.Visible = true;
                FlexibleMaterialForm._middleButton.Text = GetButtonText(ButtonId.Cancel);
                FlexibleMaterialForm._middleButton.DialogResult = DialogResult.Cancel;

                FlexibleMaterialForm._rightButton.Visible = true;
                FlexibleMaterialForm._rightButton.Text = GetButtonText(ButtonId.Retry);
                FlexibleMaterialForm._rightButton.DialogResult = DialogResult.Retry;

                FlexibleMaterialForm.CancelButton = FlexibleMaterialForm._middleButton;
                break;

            case MessageBoxButtons.YesNo:
                FlexibleMaterialForm._visibleButtonsCount = 2;

                FlexibleMaterialForm._middleButton.Visible = true;
                FlexibleMaterialForm._middleButton.Text = GetButtonText(ButtonId.No);
                FlexibleMaterialForm._middleButton.DialogResult = DialogResult.No;

                FlexibleMaterialForm._rightButton.Visible = true;
                FlexibleMaterialForm._rightButton.Text = GetButtonText(ButtonId.Yes);
                FlexibleMaterialForm._rightButton.DialogResult = DialogResult.Yes;

                //FlexibleMaterialForm.ControlBox = false;
                break;

            case MessageBoxButtons.YesNoCancel:
                FlexibleMaterialForm._visibleButtonsCount = 3;

                FlexibleMaterialForm._rightButton.Visible = true;
                FlexibleMaterialForm._rightButton.Text = GetButtonText(ButtonId.Yes);
                FlexibleMaterialForm._rightButton.DialogResult = DialogResult.Yes;

                FlexibleMaterialForm._middleButton.Visible = true;
                FlexibleMaterialForm._middleButton.Text = GetButtonText(ButtonId.No);
                FlexibleMaterialForm._middleButton.DialogResult = DialogResult.No;

                FlexibleMaterialForm._leftButton.Visible = true;
                FlexibleMaterialForm._leftButton.Text = GetButtonText(ButtonId.Cancel);
                FlexibleMaterialForm._leftButton.DialogResult = DialogResult.Cancel;

                FlexibleMaterialForm.CancelButton = FlexibleMaterialForm._leftButton;
                break;

            case MessageBoxButtons.OK:
            default:
                FlexibleMaterialForm._visibleButtonsCount = 1;
                FlexibleMaterialForm._rightButton.Visible = true;
                FlexibleMaterialForm._rightButton.Text = GetButtonText(ButtonId.Ok);
                FlexibleMaterialForm._rightButton.DialogResult = DialogResult.OK;

                FlexibleMaterialForm.CancelButton = FlexibleMaterialForm._rightButton;
                break;
        }

        //Set default button (used in FlexibleMaterialForm_Shown)
        FlexibleMaterialForm._defaultButton = defaultButton;

        SetButtonsPosition(FlexibleMaterialForm, buttonsPosition);
    }

    private void FlexibleMaterialForm_Shown(object? sender, EventArgs e)
    {
        int buttonIndexToFocus;
        Button buttonToFocus;

        //Set the default button...
        buttonIndexToFocus = _defaultButton switch
        {
            MessageBoxDefaultButton.Button2 => 2,
            MessageBoxDefaultButton.Button3 => 3,
            _ => 1,
        };

        if (buttonIndexToFocus > _visibleButtonsCount)
        {
            buttonIndexToFocus = _visibleButtonsCount;
        }

        if (buttonIndexToFocus == 3)
        {
            buttonToFocus = _rightButton;
        }
        else if (buttonIndexToFocus == 2)
        {
            buttonToFocus = _middleButton;
        }
        else
        {
            buttonToFocus = _leftButton;
        }

        buttonToFocus.Focus();
    }

    private void RichTextBoxMessage_LinkClicked(object? sender, LinkClickedEventArgs e)
    {
        try
        {
            Cursor.Current = Cursors.WaitCursor;
            if (e.LinkText != null)
            {
                Process.Start(e.LinkText);
            }
        }
        catch (Exception)
        {
            //Let the caller of FlexibleMaterialForm decide what to do with this exception...
            throw;
        }
        finally
        {
            Cursor.Current = Cursors.Default;
        }
    }

    internal void FlexibleMaterialForm_KeyUp(object? sender, KeyEventArgs e)
    {
        //Handle standard key strikes for clipboard copy: "Ctrl + C" and "Ctrl + Insert"
        if (e.Control && (e.KeyCode == Keys.C || e.KeyCode == Keys.Insert))
        {
            var buttonsTextLine = (_leftButton.Visible ? _leftButton.Text + _standardMessageBoxSeparatorSpaces : string.Empty)
                                + (_middleButton.Visible ? _middleButton.Text + _standardMessageBoxSeparatorSpaces : string.Empty)
                                + (_rightButton.Visible ? _rightButton.Text + _standardMessageBoxSeparatorSpaces : string.Empty);

            //Build same clipboard text like the standard .Net MessageBox
            var textForClipboard = _standardMessageBoxSeparatorLines
                                 + Text + Environment.NewLine
                                 + _standardMessageBoxSeparatorLines
                                 + _richTextBoxMessage.Text + Environment.NewLine
                                 + _standardMessageBoxSeparatorLines
                                 + buttonsTextLine.Replace("&", string.Empty) + Environment.NewLine
                                 + _standardMessageBoxSeparatorLines;

            //Set text in clipboard
            Clipboard.SetText(textForClipboard);
        }
    }

    /// <summary>
    /// Shows the specified message box.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <param name="text">The text.</param>
    /// <param name="caption">The caption.</param>
    /// <param name="buttons">The buttons.</param>
    /// <param name="icon">The icon.</param>
    /// <param name="defaultButton">The default button.</param>
    /// <returns>The dialog result.</returns>
    public static DialogResult Show(IWin32Window? owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, bool UseRichTextBox = true, ButtonsPosition buttonsPosition = ButtonsPosition.Right)
    {
        //Create a new instance of the FlexibleMessageBox form
        var FlexibleMaterialForm = new FlexibleMaterialForm
        {
            ShowInTaskbar = false,
            Sizable = false,

            //Bind the caption and the message text
            CaptionText = caption,
            MessageText = text
        };
        FlexibleMaterialForm._flexibleMaterialFormBindingSource.DataSource = FlexibleMaterialForm;

        //Set the dialogs icon. When no icon is used: Correct placement and width of rich text box.
        SetDialogIcon(FlexibleMaterialForm, icon);

        //Set the font for all controls
        FlexibleMaterialForm.Font = _font;
        FlexibleMaterialForm._richTextBoxMessage.Font = _font;
        FlexibleMaterialForm._richTextBoxMessage.Visible = UseRichTextBox;
        FlexibleMaterialForm._materialLabel1.Font = _font;
        FlexibleMaterialForm._materialLabel1.Visible = !UseRichTextBox;

        //Calculate the dialogs start size (Try to auto-size width to show longest text row). Also set the maximum dialog size.
        SetDialogSizes(FlexibleMaterialForm, text, caption);

        //Set the dialogs start position when given. Otherwise center the dialog on the current screen.
        SetDialogStartPosition(FlexibleMaterialForm, owner);

        //Set the buttons visibilities and texts. Also set a default button.
        //Moved after SetDialogSizes() because it needs Dialog.Width property set.
        SetDialogButtons(FlexibleMaterialForm, buttons, defaultButton, buttonsPosition);
        //Show the dialog
        return FlexibleMaterialForm.ShowDialog(owner);
    }

    private static void SetButtonsPosition(FlexibleMaterialForm fMF, ButtonsPosition buttonsPosition)
    {
        const int padding = 10;
        var visibleButtonsWidth = 0;
        switch (buttonsPosition)
        {
            case ButtonsPosition.Center:
                switch (fMF._visibleButtonsCount)
                {
                    case 3:
                        fMF._middleButton.Left = fMF.Width / 2 - fMF._middleButton.Width / 2;
                        fMF._leftButton.Left = fMF._middleButton.Left - fMF._leftButton.Width - padding * 2;
                        fMF._rightButton.Left = fMF._middleButton.Right + padding * 2;
                        visibleButtonsWidth = fMF._leftButton.Width + fMF._middleButton.Width + fMF._rightButton.Width + padding * 6;
                        break;
                    case 2:
                        fMF._middleButton.Left = fMF.Width / 2 - fMF._middleButton.Width - padding;
                        fMF._rightButton.Left = fMF.Width / 2 + padding;
                        visibleButtonsWidth = fMF._middleButton.Width + fMF._rightButton.Width + padding * 4;
                        break;
                    case 1:
                        fMF._rightButton.Left = fMF.Width / 2 - fMF._rightButton.Width / 2;
                        visibleButtonsWidth = fMF._rightButton.Width + padding * 2;
                        break;
                }
                break;

            case ButtonsPosition.Left:
                switch (fMF._visibleButtonsCount)
                {
                    case 3:
                        fMF._leftButton.Left = padding;
                        fMF._middleButton.Left = fMF._leftButton.Right + padding * 2;
                        fMF._rightButton.Left = fMF._middleButton.Right + padding * 2;
                        visibleButtonsWidth = fMF._leftButton.Width + fMF._middleButton.Width + fMF._rightButton.Width + padding * 6;
                        break;
                    case 2:
                        fMF._middleButton.Left = padding;
                        fMF._rightButton.Left = fMF._middleButton.Right + padding * 2;
                        visibleButtonsWidth = fMF._middleButton.Width + fMF._rightButton.Width + padding * 4;
                        break;
                    case 1:
                        fMF._rightButton.Left = padding;
                        visibleButtonsWidth = fMF._rightButton.Width + padding * 2;
                        break;
                }
                break;

            case ButtonsPosition.Right:
                // This alignment is simplest, in this alignment doesn't care how many buttons are visible.
                // Always the buttons visibility order is right, right + middle, right + middle + left
                fMF._rightButton.Left = fMF.Width - fMF._rightButton.Width - padding;
                fMF._middleButton.Left = fMF._rightButton.Left - fMF._middleButton.Width - padding * 2;
                fMF._leftButton.Left = fMF._middleButton.Left - fMF._leftButton.Width - padding * 2;
                switch (fMF._visibleButtonsCount)
                {
                    case 3:
                        visibleButtonsWidth = fMF._leftButton.Width + fMF._middleButton.Width + fMF._rightButton.Width + padding * 6;
                        break;
                    case 2:
                        visibleButtonsWidth = fMF._middleButton.Width + fMF._rightButton.Width + padding * 4;
                        break;
                    case 1:
                        visibleButtonsWidth = fMF._rightButton.Width + padding * 2;
                        break;
                }
                break;

            case ButtonsPosition.Fill:
                switch (fMF._visibleButtonsCount)
                {
                    case 3:
                        fMF._leftButton.Left = padding;
                        fMF._middleButton.Left = fMF.Width / 2 - fMF._middleButton.Width / 2;
                        fMF._rightButton.Left = fMF.Width - fMF._rightButton.Width - padding * 2;
                        visibleButtonsWidth = fMF._leftButton.Width + fMF._middleButton.Width + fMF._rightButton.Width + padding * 6;
                        break;
                    case 2:
                        fMF._middleButton.Left = padding;
                        fMF._rightButton.Left = fMF.Width - fMF._rightButton.Width - padding * 2;
                        visibleButtonsWidth = fMF._middleButton.Width + fMF._rightButton.Width + padding * 4;
                        break;
                    case 1:
                        fMF._rightButton.Left = fMF.Width / 2 - fMF._middleButton.Width / 2;
                        visibleButtonsWidth = fMF._rightButton.Width + padding * 2;
                        break;
                }
                break;
        }
        fMF.Width = Math.Max(fMF.Width, visibleButtonsWidth);
    }
}
