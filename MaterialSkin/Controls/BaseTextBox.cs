namespace MaterialSkin.Controls;

[ToolboxItem(false)]
public class BaseTextBox : TextBox, IMaterialControl
{
    //Properties for managing the material design properties
    [Browsable(false)]
    public int Depth { get; set; }

    [Browsable(false)]
    public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

    [Browsable(false)]
    public MouseState MouseState { get; set; }

    private string _hint = string.Empty;
    public string Hint
    {
        get { return _hint; }
        set
        {
            _hint = value;
            Invalidate();
        }
    }

    public new void SelectAll()
    {
        BeginInvoke((MethodInvoker)delegate ()
        {
            Focus();
            base.SelectAll();
        });
    }

    public BaseTextBox()
    {
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        Invalidate();
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
        Invalidate();
    }

    private const int WM_ENABLE = 0x0A;
    private const int WM_PAINT = 0xF;
    private const uint WM_USER = 0x0400;
    private const uint EM_SETBKGNDCOLOR = (WM_USER + 67);
    private const uint WM_KILLFOCUS = 0x0008;
    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);

        if (m.Msg == WM_PAINT)
        {
            if (m.Msg == WM_ENABLE)
            {
                Graphics g = Graphics.FromHwnd(Handle);
                Rectangle bounds = new(0, 0, Width, Height);
                g.FillRectangle(SkinManager.BackgroundDisabledBrush, bounds);
            }
        }

        if (m.Msg == WM_PAINT && string.IsNullOrEmpty(Text) && !Focused)
        {
            using NativeTextRenderer NativeText = new(Graphics.FromHwnd(m.HWnd));
            NativeText.DrawTransparentText(
            Hint,
            SkinManager.GetFontByType(FontType.Subtitle1),
            Enabled ?
            ColorHelper.RemoveAlpha(SkinManager.TextMediumEmphasisColor, BackColor) : // not focused
            ColorHelper.RemoveAlpha(SkinManager.TextDisabledOrHintColor, BackColor), // Disabled
            ClientRectangle.Location,
            ClientRectangle.Size,
            TextAlignFlags.Left | TextAlignFlags.Top);
        }

        if (m.Msg == EM_SETBKGNDCOLOR)
        {
            Invalidate();
        }

        if (m.Msg == WM_KILLFOCUS) //set border back to normal on lost focus
        {
            Invalidate();
        }
    }
}
