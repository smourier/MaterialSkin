namespace MaterialSkin.Controls;

public sealed class MaterialDivider : Control, IMaterialControl
{
    [Browsable(false)]
    public MouseState MouseState { get; set; }

    public MaterialDivider()
    {
        SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        Height = 1;
        BackColor = MaterialSkinManager.Instance.DividersColor;
    }
}