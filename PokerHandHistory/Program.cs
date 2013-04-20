#region License
// Copyright (c) 2012 Jerome Terry
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using JeromeTerry.PokerHandHistory.Core;

namespace JeromeTerry.PokerHandHistory
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string exe = Application.ExecutablePath;
            System.IO.FileInfo info = new System.IO.FileInfo(exe);
            string exeName = info.Name;
            string logname = exeName + ".log";

            Logger.Initialize(Data.DataFolder.Path, logname, 
                Properties.Settings.Default.log4netCategory);

            long unixTime = UnixTimestamp.UtcNow.UnixTime;
            Logger.Info("Poker Hand History Started: Unix Time {0}", unixTime);

            Application.ThreadException +=
                new ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            string dbPath = null;

            try
            {
                dbPath = Data.SQLite.DataStore.InitializeDatabase();
            }
            catch (Exception ex)
            {
                Logger.Error("Initialize database failed: {0}", ex);
                Logger.Error("The application will now exit.");
                return;
            }

            if (string.IsNullOrEmpty(dbPath))
            {
                Logger.Error("Database not found. The application will now exit.");
                return;
            }

            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

            Application.Run(new MainWindow());
        }

        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            Logger.Info("Poker Hand History Exited.");
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error("{0}", e.ExceptionObject as Exception);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Logger.Error("{0}", e.Exception);
        }
    }
}
