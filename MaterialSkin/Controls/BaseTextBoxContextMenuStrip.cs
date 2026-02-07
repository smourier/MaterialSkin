namespace MaterialSkin.Controls;

[ToolboxItem(false)]
public class BaseTextBoxContextMenuStrip : MaterialContextMenuStrip
{
    public readonly ToolStripItem undo = new MaterialToolStripMenuItem { Text = "Undo" };
    public readonly ToolStripItem seperator1 = new ToolStripSeparator();
    public readonly ToolStripItem cut = new MaterialToolStripMenuItem { Text = "Cut" };
    public readonly ToolStripItem copy = new MaterialToolStripMenuItem { Text = "Copy" };
    public readonly ToolStripItem paste = new MaterialToolStripMenuItem { Text = "Paste" };
    public readonly ToolStripItem delete = new MaterialToolStripMenuItem { Text = "Delete" };
    public readonly ToolStripItem seperator2 = new ToolStripSeparator();
    public readonly ToolStripItem selectAll = new MaterialToolStripMenuItem { Text = "Select All" };

    public BaseTextBoxContextMenuStrip() => Items.AddRange(
    [
        undo,
        seperator1,
        cut,
        copy,
        paste,
        delete,
        seperator2,
        selectAll
    ]);
}
