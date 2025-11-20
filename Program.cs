namespace iPhoneTool;

static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// Sets up global exception handlers to prevent crashes
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Set up unhandled exception handlers
        // These catch any exceptions that would otherwise crash the app
        Application.ThreadException += Application_ThreadException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }
    
    /// <summary>
    /// Handles exceptions on the UI thread
    /// </summary>
    private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
    {
        MessageBox.Show(
            $"An error occurred:\n\n{e.Exception.Message}\n\nStack Trace:\n{e.Exception.StackTrace}",
            "Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
    
    /// <summary>
    /// Handles exceptions on background threads
    /// </summary>
    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            MessageBox.Show(
                $"A critical error occurred:\n\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                "Critical Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}