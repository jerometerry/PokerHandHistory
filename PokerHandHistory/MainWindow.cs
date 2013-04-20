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
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using JeromeTerry.PokerHandHistory.Core;
using JeromeTerry.PokerHandHistory.Core.IIS;
using JeromeTerry.PokerHandHistory.Native;

namespace JeromeTerry.PokerHandHistory
{
    public partial class MainWindow : Form
    {
        IISExpress _iisExpress;
        Thread _importThread;
        PartyPoker.PartyPokerParser _parser;
        PartyPoker.HandHistoryMonitor _monitor;
        Data.SQLite.DataStore _store;
        Queue<FileInfo> _filesToImport = new Queue<FileInfo>();
        object _fileLock = new object();
        AutoResetEvent _fileWait = new AutoResetEvent(false);
        bool _running = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private int IISExpressPort
        {
            get
            {
                int port = 8080;

                using (_store.Lock())
                {
                    Data.Setting setting = _store.GetSetting("IISExpressPort");
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

        private bool IISExpressSysTray
        {
            get
            {
                bool sysTray = false;

                using (_store.Lock())
                {
                    Data.Setting setting = _store.GetSetting("IISExpressSysTray");
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

        private void MainWindow_Load(object sender, EventArgs e)
        {
            _store = Data.SQLite.DataStore.Instance;
            _parser = new PartyPoker.PartyPokerParser(_store);
            _parser.HandImported += new EventHandler(parser_HandImported);

            string exe = Application.ExecutablePath;
            System.IO.FileInfo exeInfo = new System.IO.FileInfo(exe);
            System.IO.DirectoryInfo dirInfo = exeInfo.Directory;
            string programDir = dirInfo.FullName;

            string appPath = System.IO.Path.Combine(programDir, "Http");
            int port = this.IISExpressPort;
            bool sysTray = this.IISExpressSysTray;
            _iisExpress = new IISExpress(appPath, port, sysTray);

            string httpPath = _iisExpress.Path;
            bool httpPathExists = System.IO.Directory.Exists(httpPath);

            if (httpPathExists)
            {
                if (_iisExpress.IsRunning == false)
                {
                    this.StartIIS();
                }
            }
            else
            {
                Logger.Warn("Hand Analysis ASP.NET Applicaton not found at {0}", httpPath);
            }

            // Parse existing files
            this.DiscoverExistingFiles();

            this.UpdateStats();

            this.StartImport();

            _monitor = new PartyPoker.HandHistoryMonitor(_parser.HandHistoryDir);
            _monitor.FileChanged += new FileSystemEventHandler(this._monitor_FileChanged);
            _monitor.Start();

            _statsRefreshTimer.Start();

            this.RefreshPlayers();
        }

        private void RefreshPlayers()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(this.RefreshPlayers));
            }
            else
            {
                _playersDataSet.Player.Clear();

                Data.SQLite.DataStore ds = Data.SQLite.DataStore.Instance;
                using (ds.Lock())
                {
                    List<Data.Player> results = ds.GetPlayers();
                    _playersDataSet.Player.BeginLoadData();
                    foreach (Data.Player p in results)
                    {
                        Players.PlayerRow row = _playersDataSet.Player.NewPlayerRow();
                        row.Id = p.Id;
                        row.Name = p.Name;
                        row.HandCount = ds.GetHandPlayerCount(p.Id);
                        row.DateAdded = p.DateAdded.UtcTime.ToLocalTime();
                        _playersDataSet.Player.AddPlayerRow(row);
                    }
                    _playersDataSet.Player.EndLoadData();
                }
            }
        }

        private void DiscoverExistingFiles()
        {
            Logger.Info("Discovering Party Poker Hand Histories Started");

            long totalHandsFound = 0;
            List<FileInfo> files = _parser.DiscoverHandHistoryFiles();
            string dir = _parser.HandHistoryDir;

            int count = files.Count;
            for (int i = 0; i < files.Count; i++)
            {
                FileInfo file = files[i];
                long hands = _parser.CountHandsInFile(file);
                totalHandsFound += hands;
            }

            Logger.Info("Found {0} hand history files ({1} hands) in folder {2}", count, totalHandsFound, dir);

            if (files.Count > 0)
            {
                lock (_fileLock)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        FileInfo file = files[i];
                        _filesToImport.Enqueue(file);
                    }
                }
                _fileWait.Set();
            }
        }

        void _monitor_FileChanged(object sender, System.IO.FileSystemEventArgs e)
        {
            FileInfo file = new FileInfo(e.FullPath);
            lock (_fileLock)
            {
                _filesToImport.Enqueue(file);
            }
            _fileWait.Set();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            _statsRefreshTimer.Stop();

            this._running = false;
            _fileWait.Set();

            _monitor.Stop();

            _parser.HandImported -= new EventHandler(parser_HandImported);
            _parser.Dispose();
        }

        private bool ImportRunning
        {
            get
            {
                return _importThread != null &&
                       _importThread.ThreadState == ThreadState.Running;
            }
        }

        private void StartImport()
        {
            if (this.ImportRunning)
            {
                return;
            }

            _running = true;
            _importThread = new Thread(new ThreadStart(this.ImportHands));
            _importThread.Name = "Hand History Process Thread";
            _importThread.Start();
        }

        private void ImportHands()
        {
            try
            {
                while (_running)
                {
                    _fileWait.WaitOne();

                    if (!_running)
                    {
                        return;
                    }

                    bool ok = true;
                    while (ok)
                    {
                        FileInfo file = null;
                        lock (_fileLock)
                        {
                            if (_filesToImport.Count > 0)
                            {
                                file = _filesToImport.Dequeue();
                            }
                            else
                            {
                                ok = false;
                            }
                        }
                        if (file != null)
                        {
                            _parser.ParseUserHandHistoryFile(file);
                        }
                    }
                }
            }
            finally
            {
                Logger.Info("Import thread exited");
            }
        }

        void parser_HandImported(object sender, EventArgs e)
        {
        }

        private void UpdateStats()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(this.UpdateStats));
            }
            else
            {
                long totalHands = this._parser.Stats.TotalHands;
                _lblTotalHands.Text = totalHands.ToString();
                long totalPlayers = this._parser.Stats.TotalPlayers;
                _lblTotalPlayers.Text = totalPlayers.ToString();

                this.RefreshPlayers();
            } 
        }

        private bool StartIIS()
        {
            return _iisExpress.Start();
        }

        private bool IISRunning
        {
            get
            {
                return _iisExpress != null && _iisExpress.IsRunning;
            }
        }

        private void StopIIS()
        {
            _iisExpress.Stop();
        }

        private void _miExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void _miFile_DropDownOpening(object sender, EventArgs e)
        {
            _miExit.Enabled = !this.ImportRunning;
        }

        private void _miViewLog_Click(object sender, EventArgs e)
        {
            string logFile = Logger.LogFile;
            string exe = "notepad.exe";
            NativeMethods.ShellExecute(IntPtr.Zero, "open",
                exe, logFile, "", NativeMethods.ShowCommands.SW_SHOW);
        }

        private void _miDataDir_Click(object sender, EventArgs e)
        {
            string path = Data.DataFolder.Path;
            NativeMethods.ShellExecute(IntPtr.Zero, "open",
                path, "", "", NativeMethods.ShowCommands.SW_SHOW);
        }

        private void _miAbout_Click(object sender, EventArgs e)
        {
            AboutPokerHandHistory dlg = new AboutPokerHandHistory();
            dlg.ShowDialog();
        }

        private void _btnHandAnalysis_Click(object sender, EventArgs e)
        {
            int port = this.IISExpressPort;
            string url = string.Format("http://localhost:{0}/Home", port);
            NativeMethods.ShellExecute(IntPtr.Zero, "open",
                url, "", "", NativeMethods.ShowCommands.SW_SHOW);
        }

        private void _miHandAnalysis_Click(object sender, EventArgs e)
        {
            string exe = Application.ExecutablePath;
            System.IO.FileInfo exeInfo = new System.IO.FileInfo(exe);
            System.IO.DirectoryInfo dirInfo = exeInfo.Directory;
            string mgmtExe = "JeromeTerry.PokerHandHistory.IISExpress.Management.exe";
            string path = System.IO.Path.Combine(dirInfo.FullName, mgmtExe);
            NativeMethods.ShellExecute(IntPtr.Zero, "open",
                path, "", "", NativeMethods.ShowCommands.SW_HIDE);
        }

        private void _statsRefreshTimer_Tick(object sender, EventArgs e)
        {
            this.UpdateStats();
        }

        private void _fileWatchTimer_Tick(object sender, EventArgs e)
        {

        }
    }
}
