namespace MaterialSkin.Controls;

[DefaultProperty("Items")]
[DefaultEvent("SelectedIndexChanged")]
[ComVisible(true)]
public partial class MaterialListBox : Control, IMaterialControl
{
    private const int _leftrightPadding = 16;
    private List<object>? _indicates;
    private int _selectedIndex;
    private MaterialListBoxItem? _selectedItem;
    private bool _showScrollBar;
    private bool _multiKeyDown;
    private int _hoveredItem;
    private MaterialScrollBar? _scrollBar;
    private bool _updating = false;
    private int _itemHeight;
    private Font? _primaryFont;
    private Font _secondaryFont;
    private int _primaryTextBottomPadding = 0;
    private int _secondaryTextTopPadding = 0;
    private int _secondaryTextBottomPadding = 0;

    [Category("Behavior")]
    [Description("Occurs when selected index change.")]
    public event SelectedIndexChangedEventHandler? SelectedIndexChanged;

    public delegate void SelectedIndexChangedEventHandler(object sender, MaterialListBoxItem selectedItem);

    [Category("Behavior")]
    [Description("Occurs when selected value change.")]
    public event SelectedValueEventHandler? SelectedValueChanged;

    public delegate void SelectedValueEventHandler(object sender, MaterialListBoxItem selectedItem);

    [Category("Behavior")]
    [Description("Occurs when item is added or removed.")]
    public event EventHandler? ItemsCountChanged;

    public MaterialListBox()
    {
        SetStyle
        (
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.Selectable |
            ControlStyles.ResizeRedraw |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.SupportsTransparentBackColor,
            true
        );

        UpdateStyles();
        base.BackColor = Color.Transparent;
        base.Font = MaterialSkinManager.Instance.GetFontByType(FontType.Subtitle1);
        _secondaryFont = MaterialSkinManager.Instance.GetFontByType(FontType.Body1);
        SetDefaults();
        ShowBorder = true;
        ShowScrollBar = false;
        MultiSelect = false;
        UseAccentColor = false;
        ForeColor = MaterialSkinManager.Instance.TextHighEmphasisColor; // Color.Black;
        BackColor = Color.White;
        BorderColor = Color.LightGray;
        UpdateProperties();
    }

    //Properties for managing the material design properties
    [Browsable(false)]
    public MouseState MouseState { get; set; }

    [Category("Material Skin"), DefaultValue(false), DisplayName("Use Accent Color")]
    public bool UseAccentColor { get; set { field = value; _scrollBar?.UseAccentColor = value; Invalidate(); } }

    [TypeConverter(typeof(CollectionConverter))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Editor(typeof(MaterialItemCollectionEditor), typeof(UITypeEditor))]
    [Category("Material Skin"), Description("Gets the items of the ListBox.")]
    public ObservableCollection<MaterialListBoxItem> Items { get; } = [];

    [Browsable(false)]
    [Category("Material Skin"), Description("Gets a collection containing the currently selected items in the ListBox.")]
    public List<object>? SelectedItems { get; private set; }

    [Browsable(false), Category("Material Skin"), Description("Gets or sets the currently selected item in the ListBox.")]
    public MaterialListBoxItem? SelectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
            _selectedIndex = _selectedItem != null ? Items.IndexOf(_selectedItem) : -1;
            UpdateSelection();
            Invalidate();
        }
    }

    [Browsable(false), Category("Material Skin"), Description("Gets the currently selected Text in the ListBox.")]
    public string? SelectedText { get; private set; }

    [Browsable(false), Category("Material Skin"), Description("Gets or sets the zero-based index of the currently selected item in a ListBox.")]
    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            _selectedIndex = value;
            UpdateSelection();
            Invalidate();
        }
    }

    [Browsable(true), Category("Material Skin"), Description("Gets the value of the member property specified by the ValueMember property.")]
    public object? SelectedValue { get; private set; }

    [Category("Material Skin"), DefaultValue(false), Description("Gets or sets a value indicating whether the ListBox supports multiple rows.")]
    public bool MultiSelect
    {
        get; set
        {
            field = value;
            if (SelectedItems?.Count > 1)
            {
                SelectedItems.RemoveRange(1, SelectedItems.Count - 1);
            }

            Invalidate();
        }
    }

    [Browsable(false)]
    public int Count => Items.Count;

    [Category("Material Skin"), DefaultValue(false), Description("Gets or sets a value indicating whether the vertical scroll bar be shown or not.")]
    public bool ShowScrollBar
    {
        get => _showScrollBar;
        set
        {
            _showScrollBar = value;
            _scrollBar?.Visible = value;
            Invalidate();
        }
    }

    [Category("Material Skin"), DefaultValue(true), Description("Gets or sets a value indicating whether the border shown or not.")]
    public bool ShowBorder { get; set { field = value; Refresh(); } }

    [Category("Material Skin"), Description("Gets or sets backcolor used by the control.")]
    public override Color BackColor { get; set; }

    [Category("Material Skin"), Description("Gets or sets forecolor used by the control.")]
    public override Color ForeColor { get; set; }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AllowNull]
    public override string Text { get => base.Text; set => base.Text = value; }

    [Category("Material Skin"), Description("Gets or sets border color used by the control.")]
    public Color BorderColor { get; set { field = value; Refresh(); } }

    [Category("Material Skin"), DefaultValue(ListBoxStyle.SingleLine)]
    [Description("Gets or sets the control style.")]
    public ListBoxStyle Style
    {
        get; set
        {
            field = value;
            UpdateItemSpecs();
            InvalidateScroll(this, EventArgs.Empty);
            Refresh();
        }
    } = ListBoxStyle.SingleLine;

    [Category("Material Skin"), DefaultValue(MaterialItemDensity.Dense)]
    [Description("Gets or sets list density")]
    public MaterialItemDensity Density { get; set { field = value; UpdateItemSpecs(); Invalidate(); } }

    private void SetDefaults()
    {
        SelectedIndex = -1;
        _hoveredItem = -1;
        _showScrollBar = false;
        Items.CollectionChanged += InvalidateScroll;
        SelectedItems = [];
        _indicates = [];
        _multiKeyDown = false;
        _scrollBar = new MaterialScrollBar()
        {
            Orientation = MaterialScrollOrientation.Vertical,
            Size = new Size(12, Height),
            Maximum = Items.Count * _itemHeight,
            SmallChange = _itemHeight,
            LargeChange = _itemHeight
        };
        _scrollBar.Scroll += HandleScroll;
        _scrollBar.MouseDown += HandleMouseDown;
        _scrollBar.BackColor = Color.Transparent;
        if (!Controls.Contains(_scrollBar))
        {
            Controls.Add(_scrollBar);
        }

        Style = ListBoxStyle.SingleLine;
        Density = MaterialItemDensity.Dense;
    }

    private void UpdateProperties() => Invalidate();
    private void UpdateItemSpecs()
    {
        if (Style == ListBoxStyle.TwoLine)
        {
            _secondaryTextTopPadding = 4;
            if (Density == MaterialItemDensity.Dense)
            {
                _itemHeight = 60;
                _secondaryTextBottomPadding = 10;
                _primaryTextBottomPadding = 2;
                _primaryFont = MaterialSkinManager.Instance.GetFontByType(FontType.Body1);
                _secondaryFont = MaterialSkinManager.Instance.GetFontByType(FontType.Body2);
            }
            else
            {
                _itemHeight = 72;
                _secondaryTextBottomPadding = 16;
                _primaryTextBottomPadding = 4;
                _primaryFont = MaterialSkinManager.Instance.GetFontByType(FontType.Subtitle1);
                _secondaryFont = MaterialSkinManager.Instance.GetFontByType(FontType.Body1);
            }
        }
        else if (Style == ListBoxStyle.ThreeLine)
        {
            _primaryTextBottomPadding = 4;
            _secondaryTextTopPadding = 4;
            if (Density == MaterialItemDensity.Dense)
            {
                _itemHeight = 76;
                _secondaryTextBottomPadding = 16;
                _primaryFont = MaterialSkinManager.Instance.GetFontByType(FontType.Body1);
                _secondaryFont = MaterialSkinManager.Instance.GetFontByType(FontType.Body2);
            }
            else
            {
                _itemHeight = 88;
                _secondaryTextBottomPadding = 12;
                _primaryFont = MaterialSkinManager.Instance.GetFontByType(FontType.Subtitle1);
                _secondaryFont = MaterialSkinManager.Instance.GetFontByType(FontType.Body1);
            }
        }
        else
        {
            //SingleLine
            if (Density == MaterialItemDensity.Dense)
            {
                _itemHeight = 40;
            }
            else
            {
                _itemHeight = 48;
            }
            _primaryFont = MaterialSkinManager.Instance.GetFontByType(FontType.Subtitle1);
            _secondaryFont = MaterialSkinManager.Instance.GetFontByType(FontType.Body1);
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (_updating == true)
            return;

        var g = e.Graphics;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        var mainRect = new Rectangle(0, 0, Width - (ShowBorder ? 1 : 0), Height - (ShowBorder ? 1 : 0));

        var sbv = _scrollBar?.Value ?? 0;
        var lastItem = (sbv / _itemHeight) + (Height / _itemHeight) + 1 > Items.Count ? Items.Count : (sbv / _itemHeight) + (Height / _itemHeight) + 1;
        var firstItem = sbv / _itemHeight < 0 ? 0 : (sbv / _itemHeight);

        g.FillRectangle(Enabled ? MaterialSkinManager.Instance.BackgroundBrush : MaterialSkinManager.Instance.BackgroundDisabledBrush, mainRect);

        //Set TextAlignFlags
        TextAlignFlags primaryTextAlignFlags;
        var secondaryTextAlignFlags = TextAlignFlags.Left | TextAlignFlags.Top;
        if (Style == ListBoxStyle.TwoLine || Style == ListBoxStyle.ThreeLine)
        {
            primaryTextAlignFlags = TextAlignFlags.Left | TextAlignFlags.Bottom;
        }
        else
        {
            //SingleLine
            primaryTextAlignFlags = TextAlignFlags.Left | TextAlignFlags.Middle;
        }

        //Set color and brush
        Color selectedColor;
        if (UseAccentColor)
        {
            selectedColor = MaterialSkinManager.Instance.ColorScheme.AccentColor;
        }
        else
        {
            selectedColor = MaterialSkinManager.Instance.ColorScheme.PrimaryColor;
        }

        var SelectedBrush = new SolidBrush(selectedColor);

        //Draw items
        for (var i = firstItem; i < lastItem; i++)
        {
            var itemText = Items[i].Text;
            var itemSecondaryText = Items[i].SecondaryText;
            var itemRect = new Rectangle(0, (i - firstItem) * _itemHeight, Width - (_showScrollBar && _scrollBar?.Visible == true ? _scrollBar.Width : 0), _itemHeight);

            if (_indicates != null && MultiSelect && _indicates.Count != 0)
            {
                if (i == _hoveredItem && !_indicates.Contains(i))
                {
                    g.FillRectangle(MaterialSkinManager.Instance.BackgroundHoverBrush, itemRect);
                }
                else if (_indicates.Contains(i))
                {
                    g.FillRectangle(Enabled ?
                        SelectedBrush :
                        new SolidBrush(DrawHelper.BlendColor(selectedColor, MaterialSkinManager.Instance.SwitchOffDisabledThumbColor, 197)),
                        itemRect);
                }
            }
            else
            {
                if (i == _hoveredItem && i != SelectedIndex)
                {
                    g.FillRectangle(MaterialSkinManager.Instance.BackgroundHoverBrush, itemRect);
                }
                else if (i == SelectedIndex)
                {
                    g.FillRectangle(Enabled ?
                        SelectedBrush :
                        new SolidBrush(DrawHelper.BlendColor(selectedColor, MaterialSkinManager.Instance.SwitchOffDisabledThumbColor, 197)),
                        itemRect);
                }
            }

            //Define primary & secondary Text Rect
            var primaryTextRect = new Rectangle(itemRect.X + _leftrightPadding, itemRect.Y, itemRect.Width - (2 * _leftrightPadding), itemRect.Height);

            if (Style == ListBoxStyle.TwoLine)
            {
                primaryTextRect.Height = (primaryTextRect.Height / 2) - _primaryTextBottomPadding;
            }
            else if (Style == ListBoxStyle.ThreeLine)
            {
                if (Density == MaterialItemDensity.Default)
                {
                    primaryTextRect.Height = 36 - _primaryTextBottomPadding;
                }
                else
                {
                    primaryTextRect.Height = 30 - _primaryTextBottomPadding;
                }
            }

            var secondaryTextRect = new Rectangle(primaryTextRect.X, primaryTextRect.Y + primaryTextRect.Height + _primaryTextBottomPadding + _secondaryTextTopPadding, primaryTextRect.Width, _itemHeight - _secondaryTextBottomPadding - primaryTextRect.Height - (_primaryTextBottomPadding + _secondaryTextTopPadding));

            using var NativeText = new NativeTextRenderer(g);
            NativeText.DrawTransparentText(
                itemText,
                _primaryFont,
                Enabled ? (i != SelectedIndex || UseAccentColor) ?
                MaterialSkinManager.Instance.TextHighEmphasisColor :
                MaterialSkinManager.Instance.ColorScheme.TextColor :
                MaterialSkinManager.Instance.TextDisabledOrHintColor, // Disabled
                primaryTextRect.Location,
                primaryTextRect.Size,
                primaryTextAlignFlags);
            if (Style == ListBoxStyle.TwoLine)
            {
                NativeText.DrawTransparentText(
                    itemSecondaryText,
                    _secondaryFont,
                    Enabled ? (i != SelectedIndex || UseAccentColor) ?
                    MaterialSkinManager.Instance.TextDisabledOrHintColor :
                    MaterialSkinManager.Instance.ColorScheme.TextColor.Darken(0.25f) :
                    MaterialSkinManager.Instance.TextDisabledOrHintColor, // Disabled
                    secondaryTextRect.Location,
                    secondaryTextRect.Size,
                    secondaryTextAlignFlags);
            }
            else if (Style == ListBoxStyle.ThreeLine)
            {
                NativeText.DrawMultilineTransparentText(
                    itemSecondaryText,
                    _secondaryFont,
                    Enabled ? (i != SelectedIndex || UseAccentColor) ?
                    MaterialSkinManager.Instance.TextDisabledOrHintColor :
                    MaterialSkinManager.Instance.ColorScheme.TextColor.Darken(0.25f) :
                    MaterialSkinManager.Instance.TextDisabledOrHintColor, // Disabled
                    secondaryTextRect.Location,
                    secondaryTextRect.Size,
                    secondaryTextAlignFlags);
            }
        }

        if (ShowBorder)
        {
            g.DrawRectangle(Pens.LightGray, mainRect);
        }
    }

    public void AddItem(MaterialListBoxItem newItem)
    {
        Items.Add(newItem);
        InvalidateScroll(this, EventArgs.Empty);
        ItemsCountChanged?.Invoke(this, new EventArgs());
    }

    public void AddItem(string newItem)
    {
        var newitemMLBI = new MaterialListBoxItem(newItem);
        Items.Add(newitemMLBI);
        InvalidateScroll(this, EventArgs.Empty);
        ItemsCountChanged?.Invoke(this, new EventArgs());
    }

    public void AddItems(MaterialListBoxItem[] newItems)
    {
        _updating = true;
        foreach (MaterialListBoxItem str in newItems)
        {
            AddItem(str);
        }
        _updating = false;

        InvalidateScroll(this, EventArgs.Empty);
        ItemsCountChanged?.Invoke(this, new EventArgs());
    }

    public void AddItems(string[] newItems)
    {
        _updating = true;
        foreach (string str in newItems)
        {
            AddItem(str);
        }
        _updating = false;

        InvalidateScroll(this, EventArgs.Empty);
        ItemsCountChanged?.Invoke(this, new EventArgs());
    }

    public void RemoveItemAt(int index)
    {
        if (index <= _selectedIndex)
        {
            _selectedIndex -= 1;
            UpdateSelection();
        }

        Items.RemoveAt(index);
        InvalidateScroll(this, EventArgs.Empty);
        ItemsCountChanged?.Invoke(this, new EventArgs());
    }

    public void RemoveItem(MaterialListBoxItem item)
    {
        if (Items.IndexOf(item) <= _selectedIndex)
        {
            _selectedIndex -= 1;
            UpdateSelection();
        }

        Items.Remove(item);
        InvalidateScroll(this, EventArgs.Empty);
        ItemsCountChanged?.Invoke(this, new EventArgs());
    }

    public int IndexOf(MaterialListBoxItem value) => Items.IndexOf(value);
    public void RemoveItems(MaterialListBoxItem[] itemsToRemove)
    {
        _updating = true;
        foreach (var item in itemsToRemove)
        {
            if (Items.IndexOf(item) <= _selectedIndex)
            {
                _selectedIndex -= 1;
                UpdateSelection();
            }
            Items.Remove(item);
        }
        _updating = false;

        InvalidateScroll(this, EventArgs.Empty);
        ItemsCountChanged?.Invoke(this, new EventArgs());
    }

    private void UpdateSelection()
    {
        if (_selectedIndex >= 0)
        {
            _selectedItem = Items[_selectedIndex];
            SelectedValue = Items[_selectedIndex];
            SelectedText = Items[_selectedIndex].ToString();
        }
        else
        {
            _selectedItem = null;
            SelectedValue = null;
            SelectedText = null;
        }
    }

    public void Clear()
    {
        _updating = true;
        for (var i = Items.Count - 1; i >= 0; i += -1)
        {
            Items.RemoveAt(i);
        }
        _updating = false;

        _selectedIndex = -1;
        UpdateSelection();
        InvalidateScroll(this, EventArgs.Empty);
        ItemsCountChanged?.Invoke(this, new EventArgs());
    }

    public void BeginUpdate() => _updating = true;
    public void EndUpdate() => _updating = false;

    protected override void OnSizeChanged(EventArgs e)
    {
        InvalidateScroll(this, e);
        InvalidateLayout();
        base.OnSizeChanged(e);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        Focus();
        if (e.Button == MouseButtons.Left && _scrollBar != null)
        {
            var index = _scrollBar.Value / _itemHeight + e.Location.Y / _itemHeight;
            if (index >= 0 && index < Items.Count)
            {
                if (MultiSelect && _multiKeyDown)
                {
                    _indicates?.Add(index);
                    SelectedItems?.Add(Items[index]);
                }
                else
                {
                    _indicates?.Clear();
                    SelectedItems?.Clear();
                    _selectedItem = Items[index];
                    _selectedIndex = index;
                    SelectedValue = Items[index];
                    SelectedText = Items[index].ToString();
                    SelectedIndexChanged?.Invoke(this, _selectedItem);
                    SelectedValueChanged?.Invoke(this, _selectedItem);
                }
            }
            Invalidate();
        }
        base.OnMouseDown(e);
    }

    private void HandleScroll(object? sender, ScrollEventArgs e)
    {
        if (_scrollBar != null && _scrollBar.Maximum < _scrollBar.Value + Height)
        {
            _scrollBar.Value = _scrollBar.Maximum - Height;
        }

        Invalidate();
    }

    private void InvalidateScroll(object? sender, EventArgs e)
    {
        if (_scrollBar != null)
        {
            _scrollBar.Maximum = Items.Count * _itemHeight;
            _scrollBar.SmallChange = _itemHeight;
            _scrollBar.LargeChange = Height;
            _scrollBar.Visible = (Items.Count * _itemHeight) > Height;
            if (Items.Count == 0)
            {
                _scrollBar.Value = 0;
            }
        }

        Invalidate();
    }

    private void HandleMouseDown(object? sender, MouseEventArgs e) => Focus();
    private void InvalidateLayout()
    {
        _scrollBar?.Size = new Size(12, Height - (ShowBorder ? 2 : 0));
        _scrollBar?.Location = new Point(Width - (_scrollBar.Width + (ShowBorder ? 1 : 0)), ShowBorder ? 1 : 0);
        Invalidate();
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        if (_scrollBar?.Visible == true)
        {
            if (_scrollBar.Minimum > _scrollBar.Value - e.Delta / 2)
            {
                _scrollBar.Value = _scrollBar.Minimum;
            }
            else if (_scrollBar.Maximum < _scrollBar.Value + Height)
            {
                if (e.Delta > 0)
                {
                    _scrollBar.Value -= e.Delta / 2;
                }
                // else Do nothing, maximum reached
            }
            else
            {
                _scrollBar.Value -= e.Delta / 2;
            }

            UpdateHoveredItem(e);
            Invalidate();
            base.OnMouseWheel(e);
            ((HandledMouseEventArgs)e).Handled = true;
        }
    }

    protected override bool IsInputKey(Keys keyData)
    {
        if (SelectedItems != null)
        {
            switch (keyData)
            {
                case Keys.Down:
                    try
                    {
                        SelectedItems.Remove(Items[SelectedIndex]);
                        SelectedIndex += 1;
                        SelectedItems.Add(Items[SelectedIndex]);
                    }
                    catch
                    {
                        //
                    }
                    break;

                case Keys.Up:
                    try
                    {
                        SelectedItems.Remove(Items[SelectedIndex]);
                        SelectedIndex -= 1;
                        SelectedItems.Add(Items[SelectedIndex]);
                    }
                    catch
                    {
                        //
                    }
                    break;
            }
        }

        Invalidate();
        return base.IsInputKey(keyData);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        Cursor = Cursors.Hand;
        UpdateHoveredItem(e);
        Invalidate();
    }

    private void UpdateHoveredItem(MouseEventArgs e)
    {
        var index = (_scrollBar?.Value ?? 0) / _itemHeight + e.Location.Y / _itemHeight;
        if (index >= Items.Count)
        {
            index = -1;
        }

        if (index >= 0 && index < Items.Count)
        {
            _hoveredItem = index;
        }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        _hoveredItem = -1;
        Cursor = Cursors.Default;
        Invalidate();
        base.OnMouseLeave(e);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (_scrollBar != null)
        {
            _scrollBar.Size = new Size(12, Height - (ShowBorder ? 2 : 0));
            _scrollBar.Location = new Point(Width - (_scrollBar.Width + (ShowBorder ? 1 : 0)), ShowBorder ? 1 : 0);
        }

        InvalidateScroll(this, e);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == Constants.WM_SETCURSOR)
        {
            Functions.SetCursor(Functions.LoadCursorW(0, new PWSTR { Value = Constants.IDC_HAND }));
            m.Result = 0;
            return;
        }
        base.WndProc(ref m);
    }
}
