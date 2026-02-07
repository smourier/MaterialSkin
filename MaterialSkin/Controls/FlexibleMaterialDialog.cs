namespace MaterialSkin.Controls;

/// <summary>
/// The form to show the customized message box.
/// It is defined as an internal class to keep the public interface of the FlexibleMessageBox clean.
/// </summary>
public partial class FlexibleMaterialForm : MaterialForm, IMaterialControl
{
    private readonly MaterialSkinManager materialSkinManager;

    /// <summary>
    /// Defines the font for all FlexibleMessageBox instances.
    ///
    /// Default is: SystemFonts.MessageBoxFont
    /// </summary>
    public static Font FONT;

    /// <summary>
    /// Defines the maximum width for all FlexibleMessageBox instances in percent of the working area.
    ///
    /// Allowed values are 0.2 - 1.0 where:
    /// 0.2 means:  The FlexibleMessageBox can be at most half as wide as the working area.
    /// 1.0 means:  The FlexibleMessageBox can be as wide as the working area.
    ///
    /// Default is: 70% of the working area width.
    /// </summary>
    public static double MAX_WIDTH_FACTOR = 0.7;

    /// <summary>
    /// Defines the maximum height for all FlexibleMessageBox instances in percent of the working area.
    ///
    /// Allowed values are 0.2 - 1.0 where:
    /// 0.2 means:  The FlexibleMessageBox can be at most half as high as the working area.
    /// 1.0 means:  The FlexibleMessageBox can be as high as the working area.
    ///
    /// Default is: 90% of the working area height.
    /// </summary>
    public static double MAX_HEIGHT_FACTOR = 0.9;

    private MaterialMultiLineTextBox richTextBoxMessage;
    private MaterialLabel materialLabel1;
    private MaterialButton leftButton;
    private MaterialButton middleButton;
    private MaterialButton rightButton;

    public ButtonsPosition ButtonsPositionEnum { get; set; } = ButtonsPosition.Right;

    /// <summary>
    /// Erforderliche Designervariable.
    /// </summary>
    private IContainer components = null;

    /// <summary>
    /// Verwendete Ressourcen bereinigen.
    /// </summary>
    /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    /// <summary>
    /// Erforderliche Methode für die Designerunterstützung.
    /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
    /// </summary>
    private void InitializeComponent()
    {
        components = new Container();
        FlexibleMaterialFormBindingSource = new BindingSource(components);
        messageContainer = new Panel();
        materialLabel1 = new MaterialLabel();
        pictureBoxForIcon = new PictureBox();
        richTextBoxMessage = new MaterialMultiLineTextBox();
        leftButton = new MaterialButton();
        middleButton = new MaterialButton();
        rightButton = new MaterialButton();
        ((ISupportInitialize)(FlexibleMaterialFormBindingSource)).BeginInit();
        messageContainer.SuspendLayout();
        ((ISupportInitialize)(pictureBoxForIcon)).BeginInit();
        SuspendLayout();
        // 
        // messageContainer
        // 
        messageContainer.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
        | AnchorStyles.Left)
        | AnchorStyles.Right);
        messageContainer.BackColor = Color.White;
        messageContainer.Controls.Add(materialLabel1);
        messageContainer.Controls.Add(pictureBoxForIcon);
        messageContainer.Controls.Add(richTextBoxMessage);
        messageContainer.Location = new Point(1, 65);
        messageContainer.Name = "messageContainer";
        messageContainer.Size = new Size(382, 89);
        messageContainer.TabIndex = 1;
        // 
        // materialLabel1
        // 
        materialLabel1.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
        | AnchorStyles.Left)
        | AnchorStyles.Right);
        materialLabel1.DataBindings.Add(new Binding("Text", FlexibleMaterialFormBindingSource, "MessageText", true, DataSourceUpdateMode.OnPropertyChanged));
        materialLabel1.Depth = 0;
        materialLabel1.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
        materialLabel1.Location = new Point(56, 12);
        materialLabel1.MouseState = MouseState.HOVER;
        materialLabel1.Name = "materialLabel1";
        materialLabel1.Size = new Size(314, 65);
        materialLabel1.TabIndex = 9;
        materialLabel1.Text = "<Message>";
        materialLabel1.Visible = false;
        // 
        // pictureBoxForIcon
        // 
        pictureBoxForIcon.BackColor = Color.Transparent;
        pictureBoxForIcon.Location = new Point(12, 12);
        pictureBoxForIcon.Name = "pictureBoxForIcon";
        pictureBoxForIcon.Size = new Size(32, 32);
        pictureBoxForIcon.TabIndex = 8;
        pictureBoxForIcon.TabStop = false;
        // 
        // richTextBoxMessage
        // 
        richTextBoxMessage.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom)
        | AnchorStyles.Left)
        | AnchorStyles.Right);
        richTextBoxMessage.BackColor = Color.FromArgb(((byte)(255)), ((byte)(255)), ((byte)(255)));
        richTextBoxMessage.BorderStyle = BorderStyle.None;
        richTextBoxMessage.DataBindings.Add(new Binding("Text", FlexibleMaterialFormBindingSource, "MessageText", true, DataSourceUpdateMode.OnPropertyChanged));
        richTextBoxMessage.Depth = 0;
        richTextBoxMessage.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
        richTextBoxMessage.ForeColor = Color.FromArgb(((byte)(222)), ((byte)(0)), ((byte)(0)), ((byte)(0)));
        richTextBoxMessage.Location = new Point(56, 12);
        richTextBoxMessage.Margin = new Padding(0);
        richTextBoxMessage.MouseState = MouseState.HOVER;
        richTextBoxMessage.Name = "richTextBoxMessage";
        richTextBoxMessage.ReadOnly = true;
        richTextBoxMessage.ScrollBars = RichTextBoxScrollBars.Vertical;
        richTextBoxMessage.Size = new Size(314, 65);
        richTextBoxMessage.TabIndex = 0;
        richTextBoxMessage.TabStop = false;
        richTextBoxMessage.Text = "<Message>";
        richTextBoxMessage.LinkClicked += new LinkClickedEventHandler(richTextBoxMessage_LinkClicked);
        // 
        // leftButton
        // 
        leftButton.Anchor = AnchorStyles.Bottom;
        leftButton.AutoSize = false;
        leftButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        leftButton.Density = MaterialButtonDensity.Default;
        leftButton.Depth = 0;
        leftButton.DialogResult = DialogResult.OK;
        leftButton.HighEmphasis = false;
        leftButton.Icon = null;
        leftButton.Location = new Point(32, 163);
        leftButton.Margin = new Padding(4, 6, 4, 6);
        leftButton.MinimumSize = new Size(0, 24);
        leftButton.MouseState = MouseState.HOVER;
        leftButton.Name = "leftButton";
        leftButton.Size = new Size(108, 36);
        leftButton.TabIndex = 14;
        leftButton.Text = "OK";
        leftButton.Type = MaterialButtonType.Text;
        leftButton.UseAccentColor = false;
        leftButton.UseVisualStyleBackColor = true;
        leftButton.Visible = false;
        // 
        // middleButton
        // 
        middleButton.Anchor = AnchorStyles.Bottom;
        middleButton.AutoSize = false;
        middleButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        middleButton.Density = MaterialButtonDensity.Default;
        middleButton.Depth = 0;
        middleButton.DialogResult = DialogResult.OK;
        middleButton.HighEmphasis = true;
        middleButton.Icon = null;
        middleButton.Location = new Point(148, 163);
        middleButton.Margin = new Padding(4, 6, 4, 6);
        middleButton.MinimumSize = new Size(0, 24);
        middleButton.MouseState = MouseState.HOVER;
        middleButton.Name = "middleButton";
        middleButton.Size = new Size(102, 36);
        middleButton.TabIndex = 15;
        middleButton.Text = "OK";
        middleButton.Type = MaterialButtonType.Text;
        middleButton.UseAccentColor = false;
        middleButton.UseVisualStyleBackColor = true;
        middleButton.Visible = false;
        // 
        // rightButton
        // 
        rightButton.Anchor = AnchorStyles.Bottom;
        rightButton.AutoSize = false;
        rightButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        rightButton.Density = MaterialButtonDensity.Default;
        rightButton.Depth = 0;
        rightButton.DialogResult = DialogResult.OK;
        rightButton.HighEmphasis = true;
        rightButton.Icon = null;
        rightButton.Location = new Point(258, 163);
        rightButton.Margin = new Padding(4, 6, 4, 6);
        rightButton.MinimumSize = new Size(0, 24);
        rightButton.MouseState = MouseState.HOVER;
        rightButton.Name = "rightButton";
        rightButton.Size = new Size(106, 36);
        rightButton.TabIndex = 13;
        rightButton.Text = "OK";
        rightButton.Type = MaterialButtonType.Contained;
        rightButton.UseAccentColor = false;
        rightButton.UseVisualStyleBackColor = true;
        rightButton.Visible = false;
        // 
        // FlexibleMaterialForm
        // 
        BackColor = Color.White;
        ClientSize = new Size(384, 208);
        Controls.Add(leftButton);
        Controls.Add(middleButton);
        Controls.Add(rightButton);
        Controls.Add(messageContainer);
        DataBindings.Add(new Binding("Text", FlexibleMaterialFormBindingSource, "CaptionText", true));
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(276, 140);
        Name = "FlexibleMaterialForm";
        ShowIcon = false;
        SizeGripStyle = SizeGripStyle.Show;
        StartPosition = FormStartPosition.CenterParent;
        Text = "<Caption>";
        Load += new EventHandler(FlexibleMaterialForm_Load);
        Shown += new EventHandler(FlexibleMaterialForm_Shown);
        ((ISupportInitialize)(FlexibleMaterialFormBindingSource)).EndInit();
        messageContainer.ResumeLayout(false);
        ((ISupportInitialize)(pictureBoxForIcon)).EndInit();
        ResumeLayout(false);

    }

    /// <summary>
    /// Defines the FlexibleMaterialFormBindingSource
    /// </summary>
    private BindingSource FlexibleMaterialFormBindingSource;

    /// <summary>
    /// Defines the panel1
    /// </summary>
    private Panel messageContainer;

    /// <summary>
    /// Defines the pictureBoxForIcon
    /// </summary>
    private PictureBox pictureBoxForIcon;

    //These separators are used for the "copy to clipboard" standard operation, triggered by Ctrl + C (behavior and clipboard format is like in a standard MessageBox)
    /// <summary>
    /// Defines the STANDARD_MESSAGEBOX_SEPARATOR_LINES
    /// </summary>
    private static readonly string STANDARD_MESSAGEBOX_SEPARATOR_LINES = "---------------------------\n";

    /// <summary>
    /// Defines the STANDARD_MESSAGEBOX_SEPARATOR_SPACES
    /// </summary>
    private static readonly string STANDARD_MESSAGEBOX_SEPARATOR_SPACES = "   ";

    //These are the possible buttons (in a standard MessageBox)
    private enum ButtonID
    { /// <summary>
      /// Defines the OK
      /// </summary>
        OK = 0,

        /// <summary>
        /// Defines the CANCEL
        /// </summary>
        CANCEL,

        /// <summary>
        /// Defines the YES
        /// </summary>
        YES,

        /// <summary>
        /// Defines the NO
        /// </summary>
        NO,

        /// <summary>
        /// Defines the ABORT
        /// </summary>
        ABORT,

        /// <summary>
        /// Defines the RETRY
        /// </summary>
        RETRY,

        /// <summary>
        /// Defines the IGNORE
        /// </summary>
        IGNORE
    };

    //These are the buttons texts for different languages.
    //If you want to add a new language, add it here and in the GetButtonText-Function
    private enum TwoLetterISOLanguageID
    { /// <summary>
      /// Defines the en
      /// </summary>
        en,

        /// <summary>
        /// Defines the de
        /// </summary>
        de,

        /// <summary>
        /// Defines the es
        /// </summary>
        es,

        /// <summary>
        /// Defines the it
        /// </summary>
        it,

        /// <summary>
        /// Defines the fr
        /// </summary>
        fr,

        /// <summary>
        /// Defines the ro
        /// </summary>
        ro,

        /// <summary>
        /// Defines the pl
        /// </summary>
        pl
    };

    /// <summary>
    /// Defines the BUTTON_TEXTS_ENGLISH_EN
    /// </summary>
    private static readonly string[] BUTTON_TEXTS_ENGLISH_EN = ["OK", "Cancel", "&Yes", "&No", "&Abort", "&Retry", "&Ignore"];//Note: This is also the fallback language

    /// <summary>
    /// Defines the BUTTON_TEXTS_GERMAN_DE
    /// </summary>
    private static readonly string[] BUTTON_TEXTS_GERMAN_DE = ["OK", "Abbrechen", "&Ja", "&Nein", "&Abbrechen", "&Wiederholen", "&Ignorieren"];

    /// <summary>
    /// Defines the BUTTON_TEXTS_SPANISH_ES
    /// </summary>
    private static readonly string[] BUTTON_TEXTS_SPANISH_ES = ["Aceptar", "Cancelar", "&Sí", "&No", "&Abortar", "&Reintentar", "&Ignorar"];

    /// <summary>
    /// Defines the BUTTON_TEXTS_ITALIAN_IT
    /// </summary>
    private static readonly string[] BUTTON_TEXTS_ITALIAN_IT = ["OK", "Annulla", "&Sì", "&No", "&Interrompi", "&Riprova", "&Ignora"];

    /// <summary>
    /// Defines the BUTTON_TEXTS_FRENCH_FR
    /// </summary>
    private static readonly string[] BUTTON_TEXTS_FRENCH_FR = ["OK", "Annuler", "&Oui", "&Non", "&Interrompre", "&Recommencer", "&Ignorer"];

    /// <summary>
    /// Defines the BUTTON_TEXTS_ROMANIAN_RO
    /// </summary>
    private static readonly string[] BUTTON_TEXTS_ROMANIAN_RO = ["Acceptă", "Anulează", "&Da", "&Nu", "&Întrerupe", "&Reîncearcă", "&Ignoră"];

    /// <summary>
    /// Defines the BUTTON_TEXTS_ROMANIAN_PL
    /// </summary>
    private static readonly string[] BUTTON_TEXTS_POLISH_PL = ["OK", "Anuluj", "Tak", "Nie", "Opuść", "Powtórz", "Ignoruj"];

    /// <summary>
    /// Defines the defaultButton
    /// </summary>
    private MessageBoxDefaultButton defaultButton;

    /// <summary>
    /// Defines the visibleButtonsCount
    /// </summary>
    private int visibleButtonsCount;

    /// <summary>
    /// Defines the languageID
    /// </summary>
    private readonly TwoLetterISOLanguageID languageID = TwoLetterISOLanguageID.en;

    /// <summary>
    /// Prevents a default instance of the <see cref="FlexibleMaterialForm"/> class from being created.
    /// </summary>
    private FlexibleMaterialForm()
    {
        InitializeComponent();

        //Try to evaluate the language. If this fails, the fallback language English will be used
        Enum.TryParse(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, out languageID);

        KeyPreview = true;
        KeyUp += FlexibleMaterialForm_KeyUp;

        materialSkinManager = MaterialSkinManager.Instance;
        materialSkinManager.AddFormToManage(this);
        FONT = materialSkinManager.GetFontByType(FontType.Body1);
        messageContainer.BackColor = BackColor;
    }

    /// <summary>
    /// Gets the string rows.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>The string rows as 1-dimensional array</returns>
    private static string[] GetStringRows(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return null;
        }

        var messageRows = message.Split(['\n'], StringSplitOptions.None);
        return messageRows;
    }

    /// <summary>
    /// Gets the button text for the CurrentUICulture language.
    /// Note: The fallback language is English
    /// </summary>
    /// <param name="buttonID">The ID of the button.</param>
    /// <returns>The button text</returns>
    private string GetButtonText(ButtonID buttonID)
    {
        var buttonTextArrayIndex = Convert.ToInt32(buttonID);

        switch (languageID)
        {
            case TwoLetterISOLanguageID.de: return BUTTON_TEXTS_GERMAN_DE[buttonTextArrayIndex];
            case TwoLetterISOLanguageID.es: return BUTTON_TEXTS_SPANISH_ES[buttonTextArrayIndex];
            case TwoLetterISOLanguageID.it: return BUTTON_TEXTS_ITALIAN_IT[buttonTextArrayIndex];
            case TwoLetterISOLanguageID.fr: return BUTTON_TEXTS_FRENCH_FR[buttonTextArrayIndex];
            case TwoLetterISOLanguageID.ro: return BUTTON_TEXTS_ROMANIAN_RO[buttonTextArrayIndex];
            case TwoLetterISOLanguageID.pl: return BUTTON_TEXTS_POLISH_PL[buttonTextArrayIndex];

            default: return BUTTON_TEXTS_ENGLISH_EN[buttonTextArrayIndex];
        }
    }

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
        const double MIN_FACTOR = 0.2;
        const double MAX_FACTOR = 1.0;

        if (workingAreaFactor < MIN_FACTOR)
        {
            return MIN_FACTOR;
        }

        if (workingAreaFactor > MAX_FACTOR)
        {
            return MAX_FACTOR;
        }

        return workingAreaFactor;
    }

    /// <summary>
    /// Set the dialogs start position when given.
    /// Otherwise center the dialog on the current screen.
    /// </summary>
    /// <param name="FlexibleMaterialForm">The FlexibleMessageBox dialog.</param>
    /// <param name="owner">The owner.</param>
    private static void SetDialogStartPosition(FlexibleMaterialForm FlexibleMaterialForm, IWin32Window owner)
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
        FlexibleMaterialForm.MaximumSize = new Size(Convert.ToInt32(SystemInformation.WorkingArea.Width * GetCorrectedWorkingAreaFactor(MAX_WIDTH_FACTOR)),
                                                      Convert.ToInt32(SystemInformation.WorkingArea.Height * GetCorrectedWorkingAreaFactor(MAX_HEIGHT_FACTOR)));

        //Get rows. Exit if there are no rows to render...
        var stringRows = GetStringRows(text);
        if (stringRows == null)
        {
            return;
        }

        //Calculate whole text height
        var textHeight = Math.Min(TextRenderer.MeasureText(text, FONT).Height, 600);

        //Calculate width for longest text line
        const int SCROLLBAR_WIDTH_OFFSET = 15;
        var longestTextRowWidth = stringRows.Max(textForRow => TextRenderer.MeasureText(textForRow, FONT).Width);
        var captionWidth = TextRenderer.MeasureText(caption, SystemFonts.CaptionFont).Width;
        var textWidth = Math.Max(longestTextRowWidth + SCROLLBAR_WIDTH_OFFSET, captionWidth);

        //Calculate margins
        var marginWidth = FlexibleMaterialForm.Width - FlexibleMaterialForm.richTextBoxMessage.Width;
        var marginHeight = FlexibleMaterialForm.Height - FlexibleMaterialForm.richTextBoxMessage.Height;

        var minimumHeight = FlexibleMaterialForm.messageContainer.Top + (FlexibleMaterialForm.pictureBoxForIcon.Height + 2 * 8) + 54;
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
                FlexibleMaterialForm.pictureBoxForIcon.Image = SystemIcons.Information.ToBitmap();
                break;

            case MessageBoxIcon.Warning:
                FlexibleMaterialForm.pictureBoxForIcon.Image = SystemIcons.Warning.ToBitmap();
                break;

            case MessageBoxIcon.Error:
                FlexibleMaterialForm.pictureBoxForIcon.Image = SystemIcons.Error.ToBitmap();
                break;

            case MessageBoxIcon.Question:
                FlexibleMaterialForm.pictureBoxForIcon.Image = SystemIcons.Question.ToBitmap();
                break;

            default:
                //When no icon is used: Correct placement and width of rich text box.
                FlexibleMaterialForm.pictureBoxForIcon.Visible = false;
                FlexibleMaterialForm.richTextBoxMessage.Left -= FlexibleMaterialForm.pictureBoxForIcon.Width;
                FlexibleMaterialForm.richTextBoxMessage.Width += FlexibleMaterialForm.pictureBoxForIcon.Width;
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
                FlexibleMaterialForm.visibleButtonsCount = 3;

                FlexibleMaterialForm.leftButton.Visible = true;
                FlexibleMaterialForm.leftButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.ABORT);
                FlexibleMaterialForm.leftButton.DialogResult = DialogResult.Abort;

                FlexibleMaterialForm.middleButton.Visible = true;
                FlexibleMaterialForm.middleButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.RETRY);
                FlexibleMaterialForm.middleButton.DialogResult = DialogResult.Retry;

                FlexibleMaterialForm.rightButton.Visible = true;
                FlexibleMaterialForm.rightButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.IGNORE);
                FlexibleMaterialForm.rightButton.DialogResult = DialogResult.Ignore;

                FlexibleMaterialForm.ControlBox = false;
                break;

            case MessageBoxButtons.OKCancel:
                FlexibleMaterialForm.visibleButtonsCount = 2;

                FlexibleMaterialForm.middleButton.Visible = true;
                FlexibleMaterialForm.middleButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.CANCEL);
                FlexibleMaterialForm.middleButton.DialogResult = DialogResult.Cancel;

                FlexibleMaterialForm.rightButton.Visible = true;
                FlexibleMaterialForm.rightButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.OK);
                FlexibleMaterialForm.rightButton.DialogResult = DialogResult.OK;

                FlexibleMaterialForm.CancelButton = FlexibleMaterialForm.middleButton;
                break;

            case MessageBoxButtons.RetryCancel:
                FlexibleMaterialForm.visibleButtonsCount = 2;

                FlexibleMaterialForm.middleButton.Visible = true;
                FlexibleMaterialForm.middleButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.CANCEL);
                FlexibleMaterialForm.middleButton.DialogResult = DialogResult.Cancel;

                FlexibleMaterialForm.rightButton.Visible = true;
                FlexibleMaterialForm.rightButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.RETRY);
                FlexibleMaterialForm.rightButton.DialogResult = DialogResult.Retry;

                FlexibleMaterialForm.CancelButton = FlexibleMaterialForm.middleButton;
                break;

            case MessageBoxButtons.YesNo:
                FlexibleMaterialForm.visibleButtonsCount = 2;

                FlexibleMaterialForm.middleButton.Visible = true;
                FlexibleMaterialForm.middleButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.NO);
                FlexibleMaterialForm.middleButton.DialogResult = DialogResult.No;

                FlexibleMaterialForm.rightButton.Visible = true;
                FlexibleMaterialForm.rightButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.YES);
                FlexibleMaterialForm.rightButton.DialogResult = DialogResult.Yes;

                //FlexibleMaterialForm.ControlBox = false;
                break;

            case MessageBoxButtons.YesNoCancel:
                FlexibleMaterialForm.visibleButtonsCount = 3;

                FlexibleMaterialForm.rightButton.Visible = true;
                FlexibleMaterialForm.rightButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.YES);
                FlexibleMaterialForm.rightButton.DialogResult = DialogResult.Yes;

                FlexibleMaterialForm.middleButton.Visible = true;
                FlexibleMaterialForm.middleButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.NO);
                FlexibleMaterialForm.middleButton.DialogResult = DialogResult.No;

                FlexibleMaterialForm.leftButton.Visible = true;
                FlexibleMaterialForm.leftButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.CANCEL);
                FlexibleMaterialForm.leftButton.DialogResult = DialogResult.Cancel;

                FlexibleMaterialForm.CancelButton = FlexibleMaterialForm.leftButton;
                break;

            case MessageBoxButtons.OK:
            default:
                FlexibleMaterialForm.visibleButtonsCount = 1;
                FlexibleMaterialForm.rightButton.Visible = true;
                FlexibleMaterialForm.rightButton.Text = FlexibleMaterialForm.GetButtonText(ButtonID.OK);
                FlexibleMaterialForm.rightButton.DialogResult = DialogResult.OK;

                FlexibleMaterialForm.CancelButton = FlexibleMaterialForm.rightButton;
                break;
        }

        //Set default button (used in FlexibleMaterialForm_Shown)
        FlexibleMaterialForm.defaultButton = defaultButton;

        SetButtonsPosition(FlexibleMaterialForm, buttonsPosition);
    }

    /// <summary>
    /// Handles the Shown event of the FlexibleMaterialForm control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void FlexibleMaterialForm_Shown(object sender, EventArgs e)
    {
        int buttonIndexToFocus = 1;
        Button buttonToFocus;

        //Set the default button...
        switch (defaultButton)
        {
            case MessageBoxDefaultButton.Button1:
            default:
                buttonIndexToFocus = 1;
                break;

            case MessageBoxDefaultButton.Button2:
                buttonIndexToFocus = 2;
                break;

            case MessageBoxDefaultButton.Button3:
                buttonIndexToFocus = 3;
                break;
        }

        if (buttonIndexToFocus > visibleButtonsCount)
        {
            buttonIndexToFocus = visibleButtonsCount;
        }

        if (buttonIndexToFocus == 3)
        {
            buttonToFocus = rightButton;
        }
        else if (buttonIndexToFocus == 2)
        {
            buttonToFocus = middleButton;
        }
        else
        {
            buttonToFocus = leftButton;
        }

        buttonToFocus.Focus();
    }

    /// <summary>
    /// Handles the LinkClicked event of the richTextBoxMessage control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="LinkClickedEventArgs"/> instance containing the event data.</param>
    private void richTextBoxMessage_LinkClicked(object sender, LinkClickedEventArgs e)
    {
        try
        {
            Cursor.Current = Cursors.WaitCursor;
            Process.Start(e.LinkText);
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

    /// <summary>
    /// Handles the KeyUp event of the richTextBoxMessage control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
    internal void FlexibleMaterialForm_KeyUp(object sender, KeyEventArgs e)
    {
        //Handle standard key strikes for clipboard copy: "Ctrl + C" and "Ctrl + Insert"
        if (e.Control && (e.KeyCode == Keys.C || e.KeyCode == Keys.Insert))
        {
            var buttonsTextLine = (leftButton.Visible ? leftButton.Text + STANDARD_MESSAGEBOX_SEPARATOR_SPACES : string.Empty)
                                + (middleButton.Visible ? middleButton.Text + STANDARD_MESSAGEBOX_SEPARATOR_SPACES : string.Empty)
                                + (rightButton.Visible ? rightButton.Text + STANDARD_MESSAGEBOX_SEPARATOR_SPACES : string.Empty);

            //Build same clipboard text like the standard .Net MessageBox
            var textForClipboard = STANDARD_MESSAGEBOX_SEPARATOR_LINES
                                 + Text + Environment.NewLine
                                 + STANDARD_MESSAGEBOX_SEPARATOR_LINES
                                 + richTextBoxMessage.Text + Environment.NewLine
                                 + STANDARD_MESSAGEBOX_SEPARATOR_LINES
                                 + buttonsTextLine.Replace("&", string.Empty) + Environment.NewLine
                                 + STANDARD_MESSAGEBOX_SEPARATOR_LINES;

            //Set text in clipboard
            Clipboard.SetText(textForClipboard);
        }
    }

    /// <summary>
    /// Gets or sets the CaptionText
    /// The text that is been used for the heading.
    /// </summary>
    public string CaptionText { get; set; }

    /// <summary>
    /// Gets or sets the MessageText
    /// The text that is been used in the FlexibleMaterialForm.
    /// </summary>
    public string MessageText { get; set; }

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
        FlexibleMaterialForm.FlexibleMaterialFormBindingSource.DataSource = FlexibleMaterialForm;


        //Set the dialogs icon. When no icon is used: Correct placement and width of rich text box.
        SetDialogIcon(FlexibleMaterialForm, icon);

        //Set the font for all controls
        FlexibleMaterialForm.Font = FONT;
        FlexibleMaterialForm.richTextBoxMessage.Font = FONT;
        FlexibleMaterialForm.richTextBoxMessage.Visible = UseRichTextBox;
        FlexibleMaterialForm.materialLabel1.Font = FONT;
        FlexibleMaterialForm.materialLabel1.Visible = !UseRichTextBox;

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

    private void FlexibleMaterialForm_Load(object sender, EventArgs e)
    {
    }

    private static void SetButtonsPosition(FlexibleMaterialForm fMF, ButtonsPosition buttonsPosition)
    {
        const int padding = 10;
        int visibleButtonsWidth = 0;
        switch (buttonsPosition)
        {
            case ButtonsPosition.Center:
                switch (fMF.visibleButtonsCount)
                {
                    case 3:
                        fMF.middleButton.Left = fMF.Width / 2 - fMF.middleButton.Width / 2;
                        fMF.leftButton.Left = fMF.middleButton.Left - fMF.leftButton.Width - padding * 2;
                        fMF.rightButton.Left = fMF.middleButton.Right + padding * 2;
                        visibleButtonsWidth = fMF.leftButton.Width + fMF.middleButton.Width + fMF.rightButton.Width + padding * 6;
                        break;
                    case 2:
                        fMF.middleButton.Left = fMF.Width / 2 - fMF.middleButton.Width - padding;
                        fMF.rightButton.Left = fMF.Width / 2 + padding;
                        visibleButtonsWidth = fMF.middleButton.Width + fMF.rightButton.Width + padding * 4;
                        break;
                    case 1:
                        fMF.rightButton.Left = fMF.Width / 2 - fMF.rightButton.Width / 2;
                        visibleButtonsWidth = fMF.rightButton.Width + padding * 2;
                        break;
                    default:
                        break;
                }
                break;
            case ButtonsPosition.Left:
                switch (fMF.visibleButtonsCount)
                {
                    case 3:
                        fMF.leftButton.Left = padding;
                        fMF.middleButton.Left = fMF.leftButton.Right + padding * 2;
                        fMF.rightButton.Left = fMF.middleButton.Right + padding * 2;
                        visibleButtonsWidth = fMF.leftButton.Width + fMF.middleButton.Width + fMF.rightButton.Width + padding * 6;
                        break;
                    case 2:
                        fMF.middleButton.Left = padding;
                        fMF.rightButton.Left = fMF.middleButton.Right + padding * 2;
                        visibleButtonsWidth = fMF.middleButton.Width + fMF.rightButton.Width + padding * 4;
                        break;
                    case 1:
                        fMF.rightButton.Left = padding;
                        visibleButtonsWidth = fMF.rightButton.Width + padding * 2;
                        break;
                    default:
                        break;
                }
                break;
            case ButtonsPosition.Right:
                // This alignment is simplest, in this alignment doesn't care how many buttons are visible.
                // Always the buttons visibility order is right, right + middle, right + middle + left
                fMF.rightButton.Left = fMF.Width - fMF.rightButton.Width - padding;
                fMF.middleButton.Left = fMF.rightButton.Left - fMF.middleButton.Width - padding * 2;
                fMF.leftButton.Left = fMF.middleButton.Left - fMF.leftButton.Width - padding * 2;
                switch (fMF.visibleButtonsCount)
                {
                    case 3:
                        visibleButtonsWidth = fMF.leftButton.Width + fMF.middleButton.Width + fMF.rightButton.Width + padding * 6;
                        break;
                    case 2:
                        visibleButtonsWidth = fMF.middleButton.Width + fMF.rightButton.Width + padding * 4;
                        break;
                    case 1:
                        visibleButtonsWidth = fMF.rightButton.Width + padding * 2;
                        break;
                    default:
                        break;
                }
                break;
            case ButtonsPosition.Fill:
                switch (fMF.visibleButtonsCount)
                {
                    case 3:
                        fMF.leftButton.Left = padding;
                        fMF.middleButton.Left = fMF.Width / 2 - fMF.middleButton.Width / 2;
                        fMF.rightButton.Left = fMF.Width - fMF.rightButton.Width - padding * 2;
                        visibleButtonsWidth = fMF.leftButton.Width + fMF.middleButton.Width + fMF.rightButton.Width + padding * 6;
                        break;
                    case 2:
                        fMF.middleButton.Left = padding;
                        fMF.rightButton.Left = fMF.Width - fMF.rightButton.Width - padding * 2;
                        visibleButtonsWidth = fMF.middleButton.Width + fMF.rightButton.Width + padding * 4;
                        break;
                    case 1:
                        fMF.rightButton.Left = fMF.Width / 2 - fMF.middleButton.Width / 2;
                        visibleButtonsWidth = fMF.rightButton.Width + padding * 2;
                        break;
                    default:
                        break;
                }
                break;
        }
        fMF.Width = Math.Max(fMF.Width, visibleButtonsWidth);
    }
}
