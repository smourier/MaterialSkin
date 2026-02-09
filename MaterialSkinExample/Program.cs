namespace MaterialSkinExample;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        // change this if you want to use another culture for the example (e.g. for date formatting)
        //CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo(1036); // fr-FR
        //CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.DefaultThreadCurrentCulture;

        AppDomain.CurrentDomain.UnhandledException += (s, e) => HandleError(e.ExceptionObject as Exception);
        Application.ThreadException += (s, e) => HandleError(e.Exception);

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(true);
        Application.Run(new MainForm());
        MaterialSkinManager.Instance.Dispose();
    }

    private static void HandleError(Exception? ex)
    {
        if (ex == null)
            return;

        MessageBox.Show(ex.Message, "MaterialSkin Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}