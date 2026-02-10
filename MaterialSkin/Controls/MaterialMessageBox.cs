namespace MaterialSkin.Controls;

///Adapted from http://www.codeproject.com/Articles/601900/FlexibleMessageBox
public class MaterialMessageBox : IMaterialControl
{
    public static DialogResult Show(string text, bool UseRichTextBox = true, ButtonsPosition buttonsPosition = ButtonsPosition.Right)
        => FlexibleMaterialForm.Show(null, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, UseRichTextBox, buttonsPosition);

    public static DialogResult Show(IWin32Window? owner, string text, bool UseRichTextBox = true, ButtonsPosition buttonsPosition = ButtonsPosition.Right)
        => FlexibleMaterialForm.Show(owner, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, UseRichTextBox, buttonsPosition);

    public static DialogResult Show(string text, string caption, bool UseRichTextBox = true, ButtonsPosition buttonsPosition = ButtonsPosition.Right)
        => FlexibleMaterialForm.Show(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, UseRichTextBox, buttonsPosition);

    public static DialogResult Show(IWin32Window? owner, string text, string caption, bool UseRichTextBox = true, ButtonsPosition buttonsPosition = ButtonsPosition.Right)
        => FlexibleMaterialForm.Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, UseRichTextBox, buttonsPosition);

    public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, bool UseRichTextBox = true, ButtonsPosition buttonsPosition = ButtonsPosition.Right)
        => FlexibleMaterialForm.Show(null, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, UseRichTextBox, buttonsPosition);

    public static DialogResult Show(IWin32Window? owner, string text, string caption, MessageBoxButtons buttons, bool UseRichTextBox = true, ButtonsPosition buttonsPosition = ButtonsPosition.Right)
        => FlexibleMaterialForm.Show(owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, UseRichTextBox, buttonsPosition);

    public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, bool UseRichTextBox = true, ButtonsPosition buttonsPosition = ButtonsPosition.Right)
        => FlexibleMaterialForm.Show(null, text, caption, buttons, icon, MessageBoxDefaultButton.Button1, UseRichTextBox, buttonsPosition);

    public static DialogResult Show(IWin32Window? owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, bool UseRichTextBox = true, ButtonsPosition buttonsPosition = ButtonsPosition.Right)
        => FlexibleMaterialForm.Show(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1, UseRichTextBox, buttonsPosition);

    public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, bool UseRichTextBox = true, ButtonsPosition buttonsPosition = ButtonsPosition.Right)
        => FlexibleMaterialForm.Show(null, text, caption, buttons, icon, defaultButton, UseRichTextBox, buttonsPosition);

    public static DialogResult Show(IWin32Window? owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, bool UseRichTextBox = true, ButtonsPosition buttonsPosition = ButtonsPosition.Right)
        => FlexibleMaterialForm.Show(owner, text, caption, buttons, icon, defaultButton, UseRichTextBox, buttonsPosition);

    public static DialogResult Show(string text, string caption, MessageBoxButtons messageBoxButtons, ButtonsPosition buttonsPosition = ButtonsPosition.Right)
        => FlexibleMaterialForm.Show(null, text, caption, messageBoxButtons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, true, buttonsPosition);
}
