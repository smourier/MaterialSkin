namespace MaterialSkin.Controls;

[ToolboxItem(false)]
public class BaseTextBox : TextBox, IMaterialControl
{
    //Properties for managing the material design properties
    public string Hint
    {
        get;
        set
        {
            field = value;
            Invalidate();
        }
    } = string.Empty;

    public new void SelectAll() => BeginInvoke(() =>
    {
        Focus();
        base.SelectAll();
    });

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

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);

        if (m.Msg == Constants.WM_PAINT)
        {
            if (m.Msg == Constants.WM_ENABLE)
            {
                Graphics g = Graphics.FromHwnd(Handle);
                Rectangle bounds = new(0, 0, Width, Height);
                g.FillRectangle(MaterialSkinManager.Instance.BackgroundDisabledBrush, bounds);
            }
        }

        if (m.Msg == Constants.WM_PAINT && string.IsNullOrEmpty(Text) && !Focused)
        {
            using var NativeText = new NativeTextRenderer(Graphics.FromHwnd(m.HWnd));
            NativeText.DrawTransparentText(
                Hint,
                MaterialSkinManager.Instance.GetFontByType(FontType.Subtitle1),
                Enabled ?
                ColorHelper.RemoveAlpha(MaterialSkinManager.Instance.TextMediumEmphasisColor, BackColor) : // not focused
                ColorHelper.RemoveAlpha(MaterialSkinManager.Instance.TextDisabledOrHintColor, BackColor), // Disabled
                ClientRectangle.Location,
                ClientRectangle.Size,
                TextAlignFlags.Left | TextAlignFlags.Top);
        }

        if (m.Msg == Constants.EM_SETBKGNDCOLOR)
        {
            Invalidate();
        }

        if (m.Msg == Constants.WM_KILLFOCUS) //set border back to normal on lost focus
        {
            Invalidate();
        }
    }
}
