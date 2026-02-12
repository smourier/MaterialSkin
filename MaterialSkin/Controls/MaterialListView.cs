namespace MaterialSkin.Controls;

public class MaterialListView : ListView, IMaterialControl
{
    private const int _pad = 16;
    private ListViewItem? _hoveredItem;
    private Point _mouseLocation;

    public MaterialListView()
    {
        GridLines = false;
        FullRowSelect = true;
        View = View.Details;
        OwnerDraw = true;
        ResizeRedraw = true;
        BorderStyle = BorderStyle.None;
        MinimumSize = new Size(200, 100);

        SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);
        BackColor = MaterialSkinManager.Instance.BackgroundColor;

        // Fix for hovers, by default it doesn't redraw
        _mouseLocation = new Point(-1, -1);
        MouseState = MouseState.Out;
        MouseEnter += (s, e) =>
        {
            MouseState = MouseState.Hover;
        };

        MouseLeave += (s, e) =>
        {
            MouseState = MouseState.Out;
            _mouseLocation = new Point(-1, -1);
            _hoveredItem = null;
            Invalidate();
        };

        MouseDown += (s, e) =>
        {
            MouseState = MouseState.Down;
        };

        MouseUp += (s, e) =>
        {
            MouseState = MouseState.Hover;
        };

        MouseMove += (s, e) =>
        {
            _mouseLocation = e.Location;
            var currentHoveredItem = GetItemAt(_mouseLocation.X, _mouseLocation.Y);
            if (_hoveredItem != currentHoveredItem)
            {
                _hoveredItem = currentHoveredItem;
                Invalidate();
            }
        };
    }

    [Browsable(false)]
    public MouseState MouseState { get; private set; }

    [Category("Appearance"), Browsable(true)]
    public bool AutoSizeTable
    {
        get;
        set
        {
            field = value;
            Scrollable = !value;
        }
    }

    protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var backBrush = new SolidBrush(BackColor);
        g.FillRectangle(backBrush, e.Bounds);

        // Draw Text
        using var renderer = new NativeTextRenderer(g);
        renderer.DrawTransparentText(
            e.Header?.Text,
            MaterialSkinManager.Instance.GetLogFontByType(FontType.Subtitle2),
            Enabled ? MaterialSkinManager.Instance.TextHighEmphasisNoAlphaColor : MaterialSkinManager.Instance.TextDisabledOrHintColor,
            new Point(e.Bounds.Location.X + _pad, e.Bounds.Location.Y),
            new Size(e.Bounds.Size.Width - _pad * 2, e.Bounds.Size.Height),
            TextAlignFlags.Left | TextAlignFlags.Middle);
    }

    protected override void OnDrawItem(DrawListViewItemEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Always draw default background
        g.FillRectangle(MaterialSkinManager.Instance.BackgroundBrush, e.Bounds);

        if (e.Item.Selected)
        {
            // Selected background
            g.FillRectangle(MaterialSkinManager.Instance.BackgroundFocusBrush, e.Bounds);
        }
        else if (e.Bounds.Contains(_mouseLocation) && MouseState == MouseState.Hover)
        {
            // Hover background
            g.FillRectangle(MaterialSkinManager.Instance.BackgroundHoverBrush, e.Bounds);
        }

        // Draw separator line
        g.DrawLine(new Pen(MaterialSkinManager.Instance.DividersColor), e.Bounds.Left, e.Bounds.Y, e.Bounds.Right, e.Bounds.Y);

        foreach (ListViewItem.ListViewSubItem subItem in e.Item.SubItems)
        {
            // Draw Text
            using var renderer = new NativeTextRenderer(g);
            renderer.DrawTransparentText(
                subItem.Text,
                MaterialSkinManager.Instance.GetLogFontByType(FontType.Body2),
                Enabled ? MaterialSkinManager.Instance.TextHighEmphasisNoAlphaColor : MaterialSkinManager.Instance.TextDisabledOrHintColor,
                new Point(subItem.Bounds.X + _pad, subItem.Bounds.Y),
                new Size(subItem.Bounds.Width - _pad * 2, subItem.Bounds.Height),
                TextAlignFlags.Left | TextAlignFlags.Middle);
        }
    }

    // Resize
    protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
    {
        base.OnColumnWidthChanging(e);
        AutoResize();
    }

    protected override void OnColumnWidthChanged(ColumnWidthChangedEventArgs e)
    {
        base.OnColumnWidthChanged(e);
        AutoResize();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        AutoResize();
    }

    private void AutoResize()
    {
        if (!AutoSizeTable)
            return;

        // Width
        var w = 0;
        foreach (ColumnHeader col in Columns)
        {
            w += col.Width;
        }

        // Height
        var h = 50; //Header size
        if (Items.Count > 0)
        {
            h = TopItem?.Bounds.Top ?? 0;
        }

        foreach (ListViewItem item in Items)
        {
            h += item.Bounds.Height;
        }

        Size = new Size(w, h);
    }

    protected override void InitLayout()
    {
        base.InitLayout();

        // enforce settings
        GridLines = false;
        FullRowSelect = true;
        View = View.Details;
        OwnerDraw = true;
        ResizeRedraw = true;
        BorderStyle = BorderStyle.None;
    }

    protected override void OnBackColorChanged(EventArgs e)
    {
        base.OnBackColorChanged(e);
        BackColor = MaterialSkinManager.Instance.BackgroundColor;
    }
}
