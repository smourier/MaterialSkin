namespace MaterialSkin.Controls;

public class MaterialProgressBar : ProgressBar, IMaterialControl
{
    public MaterialProgressBar()
    {
        SetStyle(ControlStyles.UserPaint, true);
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
    }

    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) => base.SetBoundsCore(x, y, width, 5, specified);
    protected override void OnPaint(PaintEventArgs e)
    {
        var doneProgress = (int)(Width * ((double)Value / Maximum));
        e.Graphics.FillRectangle(Enabled ?
            MaterialSkinManager.Instance.ColorScheme.PrimaryBrush :
            new SolidBrush(DrawHelper.BlendColor(MaterialSkinManager.Instance.ColorScheme.PrimaryColor, MaterialSkinManager.Instance.SwitchOffDisabledThumbColor, 197)),
            0, 0, doneProgress, Height);

        e.Graphics.FillRectangle(MaterialSkinManager.Instance.BackgroundFocusBrush, doneProgress, 0, Width - doneProgress, Height);
    }
}
