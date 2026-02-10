namespace MaterialSkin;

public class MaterialItemCollectionEditor : CollectionEditor
{
    public MaterialItemCollectionEditor()
        : base(typeof(MaterialItemCollection))
    {

    }

    protected override Type CreateCollectionItemType() => typeof(MaterialListBoxItem);
    protected override Type[] CreateNewItemTypes() => [typeof(MaterialListBoxItem)];
}