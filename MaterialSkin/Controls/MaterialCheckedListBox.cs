namespace MaterialSkin.Controls;

public class MaterialCheckedListBox : Panel, IMaterialControl
{
    [Browsable(false)]
    public int Depth { get; set; }

    [Browsable(false)]
    public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

    [Browsable(false)]
    public MouseState MouseState { get; set; }

    public bool Striped { get; set; }

    public Color StripeDarkColor { get; set; }

    public ItemsList Items { get; set; }

    public MaterialCheckedListBox() : base()
    {
        DoubleBuffered = true;
        Items = new ItemsList(this);
        AutoScroll = true;
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();
        if (DesignMode)
        {
            BackColorChanged += (sender, args) => BackColor = Parent.BackColor;
            BackColor = Parent.BackColor;
        }
        else
        {
            BackColorChanged += (sender, args) => BackColor = DrawHelper.BlendColor(Parent.BackColor, SkinManager.BackgroundAlternativeColor, SkinManager.BackgroundAlternativeColor.A);
            BackColor = DrawHelper.BlendColor(Parent.BackColor, SkinManager.BackgroundAlternativeColor, SkinManager.BackgroundAlternativeColor.A);
        }
    }

    public CheckState GetItemCheckState(int Index)
    {
        return Items[Index].CheckState;
    }

    public class ItemsList(Panel parent) : List<MaterialCheckbox>
    {
        private readonly Panel _parent = parent;

        public delegate void SelectedIndexChangedEventHandler(int Index);

        public void Add(string text)
        {
            Add(text, false);
        }

        public void Add(string text, bool defaultValue)
        {
            MaterialCheckbox cb = new();
            Add(cb);
            cb.Checked = defaultValue;
            cb.Text = text;
        }

        public new void Add(MaterialCheckbox value)
        {
            base.Add(value);
            _parent.Controls.Add(value);
            value.Dock = DockStyle.Top;
        }

        public new void Remove(MaterialCheckbox value)
        {
            base.Remove(value);
            _parent.Controls.Remove(value);
        }
    }
}