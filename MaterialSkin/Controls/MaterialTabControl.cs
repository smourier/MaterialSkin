namespace MaterialSkin.Controls;

public class MaterialTabControl : TabControl, IMaterialControl
{
    public MaterialTabControl()
    {
        Multiline = true;
    }

    [Browsable(false)]
    public MouseState MouseState { get; set; }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x1328 && !DesignMode)
        {
            m.Result = 1;
        }
        else
        {
            base.WndProc(ref m);
        }
    }

    protected override void OnControlAdded(ControlEventArgs e)
    {
        base.OnControlAdded(e);
        e.Control?.BackColor = Color.White;
    }
}
