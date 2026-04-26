namespace OutlastTrayTool
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        private static Mutex mutex;
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            bool createdNew;
            mutex = new Mutex(true, "Lathe_SingleInstance", out createdNew);
            if (!createdNew)
            {
                MessageBox.Show("Lathe is already running.");
                return;
            }
            ApplicationConfiguration.Initialize();
            try
            {
                Application.Run(new Form1());
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
    }
}