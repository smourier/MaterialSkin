namespace MaterialSkinExample;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        AppDomain.CurrentDomain.UnhandledException += (s, e) => HandleError(e.ExceptionObject as Exception);
        Application.ThreadException += (s, e) => HandleError(e.Exception);

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(true);
        Application.Run(new MainForm());
    }

    private static void HandleError(Exception? ex)
    {
        if (ex == null)
            return;

        MessageBox.Show(ex.Message, "MaterialSkin Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}