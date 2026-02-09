namespace MaterialSkin;

public static class MouseWheelRedirector
{
    private static readonly Filter _filter = new();
    private static int _controls;
    private static Control? _currentControl;

    public static void Attach(Control control)
    {
        if (Interlocked.Increment(ref _controls) == 1)
        {
            //Application.AddMessageFilter(_filter);
        }

        control.MouseEnter += ControlMouseEnter;
        control.MouseLeave += ControlMouseLeaveOrDisposed;
        control.Disposed += ControlMouseLeaveOrDisposed;
    }

    public static void Detach(Control control)
    {
        control.MouseEnter -= ControlMouseEnter;
        control.MouseLeave -= ControlMouseLeaveOrDisposed;
        control.Disposed -= ControlMouseLeaveOrDisposed;

        if (_currentControl == control)
        {
            _currentControl = null;
        }

        if (Interlocked.Decrement(ref _controls) == 0)
        {
            Application.RemoveMessageFilter(_filter);
        }
    }

    private static void ControlMouseEnter(object? sender, EventArgs e)
    {
        if (sender is not Control control)
            return;

        if (!control.Focused)
        {
            _currentControl = control;
        }
        else
        {
            _currentControl = null;
        }
    }

    private static void ControlMouseLeaveOrDisposed(object? sender, EventArgs e)
    {
        if (_currentControl == sender)
        {
            _currentControl = null;
        }
    }

    private sealed class Filter : IMessageFilter
    {
        public bool PreFilterMessage(ref Message m)
        {
            if (_currentControl != null &&
                !_currentControl.IsDisposed &&
                _currentControl.Handle != 0 &&
                m.Msg == Constants.WM_MOUSEWHEEL)
            {
                Functions.SendMessageW(_currentControl.Handle, m.Msg, m.WParam, m.LParam);
                return true;
            }
            return false;
        }
    }
}
