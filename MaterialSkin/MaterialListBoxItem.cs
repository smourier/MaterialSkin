namespace MaterialSkin;

public class MaterialListBoxItem(string text, string? secondarytext = null, object? tag = null)
{
    public MaterialListBoxItem()
        : this("ListBoxItem", string.Empty)
    {
    }

    public string Text { get; set; } = text;
    public string? SecondaryText { get; set; } = secondarytext;
    public object? Tag { get; set; } = tag;
}
