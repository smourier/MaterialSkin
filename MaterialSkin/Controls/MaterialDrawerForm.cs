namespace MaterialSkin.Controls;

public class MaterialDrawerForm : Form
{
    public MaterialDrawerForm()
    {
        SetStyle(ControlStyles.Selectable | ControlStyles.OptimizedDoubleBuffer | ControlStyles.EnableNotifyMessage, true);
        MouseWheelRedirector.Attach(this);
    }

    protected override void Dispose(bool disposing)
    {
        MouseWheelRedirector.Detach(this);
        base.Dispose(disposing);
    }
}
