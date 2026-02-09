namespace MaterialSkin.Controls;

[ToolboxItem(false)]
public class BaseTextBoxContextMenuStrip : MaterialContextMenuStrip
{
    public static string UndoText { get; } = Functions.LoadDllString("shell32.dll", 31261) ?? "Undo";
    public static string CutText { get; } = Functions.LoadDllString("shell32.dll", 31244) ?? "Cut";
    public static string CopyText { get; } = Functions.LoadDllString("shell32.dll", 4146) ?? "Copy";
    public static string PasteText { get; } = Functions.LoadDllString("shell32.dll", 31380) ?? "Paste";
    public static string DeleteText { get; } = Functions.LoadDllString("shell32.dll", 31252) ?? "Delete";
    public static string SelectAllText { get; } = Functions.LoadDllString("shell32.dll", 31276) ?? "Select All";

    public ToolStripItem Undo { get; } = new MaterialToolStripMenuItem { Text = UndoText };
    public ToolStripItem Seperator1 { get; } = new ToolStripSeparator();
    public ToolStripItem Cut { get; } = new MaterialToolStripMenuItem { Text = CutText };
    public ToolStripItem Copy { get; } = new MaterialToolStripMenuItem { Text = CopyText };
    public ToolStripItem Paste { get; } = new MaterialToolStripMenuItem { Text = PasteText };
    public ToolStripItem Delete { get; } = new MaterialToolStripMenuItem { Text = DeleteText };
    public ToolStripItem Seperator2 { get; } = new ToolStripSeparator();
    public ToolStripItem SelectAll { get; } = new MaterialToolStripMenuItem { Text = SelectAllText };

    public BaseTextBoxContextMenuStrip() => Items.AddRange(
    [
        Undo,
        Seperator1,
        Cut,
        Copy,
        Paste,
        Delete,
        Seperator2,
        SelectAll
    ]);
}
