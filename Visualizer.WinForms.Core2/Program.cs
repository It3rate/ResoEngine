namespace ResoEngine.Visualizer;

static class Program
{
    private static int _reported;

    [STAThread]
    static void Main()
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, args) => ReportUnhandledException("UI thread", args.Exception);
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            ReportUnhandledException(
                "AppDomain",
                args.ExceptionObject as Exception ?? new InvalidOperationException("Unknown unhandled exception."));

        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }

    private static void ReportUnhandledException(string source, Exception exception)
    {
        if (Interlocked.Exchange(ref _reported, 1) != 0)
        {
            return;
        }

        string message =
            $"Unhandled exception ({source}){Environment.NewLine}{Environment.NewLine}" +
            $"{exception}{Environment.NewLine}";

        try
        {
            string logPath = Path.Combine(AppContext.BaseDirectory, "visualizer-exception.log");
            File.AppendAllText(logPath, $"{DateTime.Now:O}{Environment.NewLine}{message}{Environment.NewLine}");
        }
        catch
        {
        }

        try
        {
            MessageBox.Show(
                $"{exception.Message}{Environment.NewLine}{Environment.NewLine}See visualizer-exception.log for details.",
                "Visualizer error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        catch
        {
        }
    }
}
