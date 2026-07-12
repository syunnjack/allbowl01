using System;
using System.Windows.Forms;


namespace allbowl01
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (TryRunCommand(args))
            {
                return;
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }

        private static bool TryRunCommand(string[] args)
        {
            if (args.Length == 0)
            {
                return false;
            }

            if (args[0] == "--export-web")
            {
                var outputDir = args.Length > 1
                    ? args[1]
                    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "web", "src", "data");
                var db = new DatabaseManager(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "prochallenge.db"));
                WebDataExporter.Export(db, outputDir);
                return true;
            }

            return false;
        }
    }
}
