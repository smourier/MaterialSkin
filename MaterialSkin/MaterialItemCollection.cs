namespace MaterialSkin;

[Editor(typeof(MaterialItemCollectionEditor), typeof(UITypeEditor))]
public class MaterialItemCollection : Collection<object>
{
    public event EventHandler ItemUpdated;

    public delegate void EventHandler(object sender, EventArgs e);

    public void AddRange(IEnumerable<object> items)
    {
        foreach (object item in items)
        {
            Add(item);
        }
    }

    public void AddRange(string[] items)
    {
        foreach (object item in items)
        {
            Add(item);
        }
    }

    protected new void Add(object item)
    {
        base.Add(item);
        ItemUpdated?.Invoke(this, null);
    }

    protected override void InsertItem(int index, object item)
    {
        base.InsertItem(index, item);
        ItemUpdated?.Invoke(this, null);
    }

    protected override void RemoveItem(int value)
    {
        base.RemoveItem(value);
        ItemUpdated?.Invoke(this, null);
    }

    protected new void Clear()
    {
        base.Clear();
        ItemUpdated?.Invoke(this, null);
    }

    protected override void ClearItems()
    {
        base.ClearItems();
        ItemUpdated?.Invoke(this, null);
    }
}

public class MaterialListBoxItem
{
    public string Text { get; set; }
    public string SecondaryText { get; set; }
    public object Tag { get; set; }

    public MaterialListBoxItem()
    {
        Text = "ListBoxItem";
        SecondaryText = "";
    }

    public MaterialListBoxItem(string text)
    {
        Text = text;
    }

    public MaterialListBoxItem(string text, string secondarytext)
    {
        Text = text;
        SecondaryText = secondarytext;
    }

    public MaterialListBoxItem(string text, string secondarytext, object tag)
    {
        Text = text;
        SecondaryText = secondarytext;
        Tag = tag;
    }
}
