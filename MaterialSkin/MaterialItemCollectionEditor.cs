#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System.Windows.Forms;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class MaterialItemCollectionEditor : CollectionEditor
{
    public MaterialItemCollectionEditor()
        : base(typeof(MaterialItemCollection))
    {

    }

    protected override Type CreateCollectionItemType() => typeof(MaterialListBoxItem);
    protected override Type[] CreateNewItemTypes() => [typeof(MaterialListBoxItem)];
}