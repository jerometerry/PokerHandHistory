using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using JeromeTerry.PokerHandHistory.Core;
using JeromeTerry.PokerHandHistory.Native;

namespace JeromeTerry.PokerHandHistory.IISExpress.Management
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string exe = Application.ExecutablePath;
            System.IO.FileInfo info = new System.IO.FileInfo(exe);
            string exeName = info.Name;
            string logname = exeName + ".log";

            Logger.Initialize(Data.DataFolder.Path, logname,
                Properties.Settings.Default.log4netCategory);

            string dbPath = Data.SQLite.DataStore.InitializeDatabase();

            long unixTime = UnixTimestamp.UtcNow.UnixTime;
            Logger.Info("Poker Hand History Started: Unix Time {0}", unixTime);

            Core.IIS.IISExpress iisExpress;

            System.IO.DirectoryInfo dirInfo = info.Directory;
            string programDir = dirInfo.FullName;

            string appPath = System.IO.Path.Combine(programDir, "Http");
            int port = IISExpressPort;

            bool sysTray = IISExpressSysTray;
            iisExpress = new Core.IIS.IISExpress(appPath, port, sysTray);

            string httpPath = iisExpress.Path;
            bool httpPathExists = System.IO.Directory.Exists(httpPath);

            if (httpPathExists)
            {
                if (iisExpress.IsRunning)
                {
                    iisExpress.Stop();
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(10));
                }

                iisExpress.Start();

                string url = string.Format("http://localhost:{0}/Home", port);
                NativeMethods.ShellExecute(IntPtr.Zero, "open",
                    url, "", "", NativeMethods.ShowCommands.SW_SHOW);
            }
            else
            {
                Logger.Warn("Hand Analysis ASP.NET Applicaton not found at {0}", httpPath);
            }
        }

        private static int IISExpressPort
        {
            get
            {
                int port = 8080;

                Data.SQLite.DataStore store = Data.SQLite.DataStore.Instance;
                using (store.Lock())
                {
                    Data.Setting setting = store.GetSetting("IISExpressPort");
                    if (setting != null)
                    {
                        if (!int.TryParse(setting.Value, out port))
                        {
                            Logger.Error("Invalid IIS Express Port Number: {0}", setting.Value);
                        }
                    }
                }

                return port;
            }
        }

        private static bool IISExpressSysTray
        {
            get
            {
                bool sysTray = false;

                Data.SQLite.DataStore store = Data.SQLite.DataStore.Instance;
                using (store.Lock())
                {
                    Data.Setting setting = store.GetSetting("IISExpressSysTray");
                    if (setting != null)
                    {
                        if (!bool.TryParse(setting.Value, out sysTray))
                        {
                            Logger.Error("Invalid IIS Express Sys Tray: {0}", setting.Value);
                        }
                    }
                }

                return sysTray;
            }
        }
    }
}
