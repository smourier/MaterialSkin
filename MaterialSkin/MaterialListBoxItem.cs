namespace MaterialSkin;

public class MaterialListBoxItem
{
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

    public string Text { get; set; }
    public string? SecondaryText { get; set; }
    public object? Tag { get; set; }
}
