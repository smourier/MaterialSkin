namespace MaterialSkin.Controls;

public class MaterialDrawerForm : Form
{
    public MouseWheelRedirector MouseWheelRedirector;

    public MaterialDrawerForm()
    {
        MouseWheelRedirector = new MouseWheelRedirector();
        SetStyle(ControlStyles.Selectable | ControlStyles.OptimizedDoubleBuffer | ControlStyles.EnableNotifyMessage, true);
    }

    public void Attach(Control control)
    {
        MouseWheelRedirector.Attach(control);
    }

    public void Detach(Control control)
    {
        MouseWheelRedirector.Detach(control);
    }
}
