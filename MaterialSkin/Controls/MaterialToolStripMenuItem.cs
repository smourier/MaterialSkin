namespace MaterialSkin.Controls;

public class MaterialToolStripMenuItem : ToolStripMenuItem
{
    public MaterialToolStripMenuItem()
    {
        AutoSize = false;
        Size = new Size(128, 32);
    }

    protected override ToolStripDropDown CreateDefaultDropDown()
    {
        var baseDropDown = base.CreateDefaultDropDown();
        if (DesignMode)
            return baseDropDown;

        var defaultDropDown = new MaterialContextMenuStrip();
        defaultDropDown.Items.AddRange(baseDropDown.Items);
        return defaultDropDown;
    }
}
