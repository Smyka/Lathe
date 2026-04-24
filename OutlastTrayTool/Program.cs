using System.Threading;

namespace OutlastTrayTool
{
    internal static class Program
    {
        private static Mutex? mutex;

        [STAThread]
        static void Main()
        {
            bool createdNew;
            mutex = new Mutex(true, "Lathe_SingleInstance", out createdNew);

            if (!createdNew)
            {
                MessageBox.Show("Lathe is already running.");
                mutex.Dispose();
                return;
            }

            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                try { mutex?.ReleaseMutex(); } catch { }
                try { mutex?.Dispose(); } catch { }
            };

            ApplicationConfiguration.Initialize();

            // Run the splash on the main STA thread using ApplicationContext so the
            // WinForms message pump never stops. Blocking the main thread with Sleep()
            // prevents SynchronizationContext.Post() from dispatching, causing NAudio deadlocks.
            Form1? mainForm = null;
            var splash = new SplashScreen();
            splash.FormClosed += (s, e) =>
            {
                mainForm = new Form1();
                mainForm.Show();
            };
            var appCtx = new ApplicationContext(splash);
            splash.FormClosed += (s, e) => { if (mainForm != null) appCtx.MainForm = mainForm; };
            Application.Run(appCtx);

            try { mutex.ReleaseMutex(); } catch { }
            try { mutex.Dispose(); } catch { }
        }
    }
}
