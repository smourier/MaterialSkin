namespace MaterialSkin;

[Editor(typeof(MaterialItemCollectionEditor), typeof(UITypeEditor))]
public class MaterialItemCollection : Collection<object>
{
    public event EventHandler? ItemUpdated;

    public void AddRange(IEnumerable<object> items)
    {
        foreach (var item in items)
        {
            Add(item);
        }
    }

    public void AddRange(string[] items)
    {
        foreach (var item in items)
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
