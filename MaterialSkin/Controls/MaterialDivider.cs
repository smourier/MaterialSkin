namespace MaterialSkin.Controls;

public sealed class MaterialDivider : Control, IMaterialControl
{
    public MaterialDivider()
    {
        SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        Height = 1;
        BackColor = MaterialSkinManager.Instance.DividersColor;
    }
}