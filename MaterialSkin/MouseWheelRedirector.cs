namespace MaterialSkin;

public class MouseWheelRedirector : IMessageFilter
{
    private static MouseWheelRedirector? _instance = null;
#pragma warning disable IDE1006 // Naming Styles
    private const int WM_MOUSEWHEEL = 0x20A;
#pragma warning restore IDE1006 // Naming Styles

    public static bool Active
    {
        set
        {
            if (field != value)
            {
                field = value;
                if (field)
                {
                    _instance ??= new MouseWheelRedirector();
                    Application.AddMessageFilter(_instance);
                }
                else if (_instance != null)
                {
                    Application.RemoveMessageFilter(_instance);
                }
            }
        }

        get;
    } = false;

    public static void Attach(Control control)
    {
        if (!Active)
        {
            Active = true;
        }

        control.MouseEnter += _instance!.ControlMouseEnter;
        control.MouseLeave += _instance.ControlMouseLeaveOrDisposed;
        control.Disposed += _instance.ControlMouseLeaveOrDisposed;
    }

    public static void Detach(Control control)
    {
        if (_instance == null)
            return;

        control.MouseEnter -= _instance.ControlMouseEnter;
        control.MouseLeave -= _instance.ControlMouseLeaveOrDisposed;
        control.Disposed -= _instance.ControlMouseLeaveOrDisposed;

        if (_instance._currentControl == control)
        {
            _instance._currentControl = null;
        }
    }

    private Control? _currentControl;

    public MouseWheelRedirector()
    {
    }

    private void ControlMouseEnter(object? sender, EventArgs e)
    {
        var control = (Control)sender!;
        if (!control.Focused)
        {
            _currentControl = control;
        }
        else
        {
            _currentControl = null;
        }
    }

    private void ControlMouseLeaveOrDisposed(object? sender, EventArgs e)
    {
        if (_currentControl == sender)
        {
            _currentControl = null;
        }
    }

    public bool PreFilterMessage(ref Message m)
    {
        if (_currentControl != null && m.Msg == WM_MOUSEWHEEL)
        {
            SendMessage(_currentControl.Handle, m.Msg, m.WParam, m.LParam);
            return true;
        }
        return false;
    }

    [DllImport("user32.dll", SetLastError = false)]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
}
