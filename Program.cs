namespace runescape_discord_rpc_server;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Instead of creating the form directly, use Application.Run with a new instance of your form
        Application.Run(new MainForm());
    }
}