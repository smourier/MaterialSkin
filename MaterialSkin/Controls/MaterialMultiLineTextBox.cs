namespace MaterialSkin.Controls;

public class MaterialMultiLineTextBox : RichTextBox, IMaterialControl
{
    private const int _emSetCueBanner = 0x1501;

    public MaterialMultiLineTextBox()
    {
        base.OnCreateControl();
        Multiline = true;

        BorderStyle = BorderStyle.None;
        Font = SkinManager.GetFontByType(FontType.Body1);
        BackColor = SkinManager.BackgroundColor;
        ForeColor = SkinManager.TextHighEmphasisColor;
        BackColorChanged += (sender, args) => BackColor = SkinManager.BackgroundColor;
        ForeColorChanged += (sender, args) => ForeColor = SkinManager.TextHighEmphasisColor;
    }

    //Properties for managing the material design properties
    [Browsable(false)]
    public int Depth { get; set; }

    [Browsable(false)]
    public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

    [Browsable(false)]
    public MouseState MouseState { get; set; }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

    [Category("Material Skin"), DefaultValue(""), Localizable(true)]
    public string Hint
    {
        get;
        set
        {
            field = value;
            SendMessage(Handle, _emSetCueBanner, (int)0, Hint);
        }
    } = string.Empty;

    [Category("Material Skin"), DefaultValue(false), Description("Select next control which have TabStop property set to True when enter key is pressed. To add enter in text, the user must press CTRL+Enter")]
    public bool LeaveOnEnterKey
    {
        get;
        set
        {
            field = value;
            if (value)
            {
                KeyDown += LeaveOnEnterKey_KeyDown;
            }
            else
            {
                KeyDown -= LeaveOnEnterKey_KeyDown;
            }
            Invalidate();
        }
    }

    public new void Focus() => BeginInvoke(() => base.Focus());
    public new void SelectAll() => BeginInvoke(() =>
    {
        base.Focus();
        base.SelectAll();
    });

    private void LeaveOnEnterKey_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyData == Keys.Enter && !e.Control)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            SendKeys.Send("{TAB}");
        }
    }
}
