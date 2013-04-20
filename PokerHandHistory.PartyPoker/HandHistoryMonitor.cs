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

namespace JeromeTerry.PokerHandHistory.PartyPoker
{
    public sealed class HandHistoryMonitor : IDisposable
    {
        public event FileSystemEventHandler FileChanged;

        public string WatchDirectory { get; set; }
        Timer _fileWatchTimer;
        public TimeSpan Interval { get; set; }
        public DateTime LastCheck { get; set; }
        private Dictionary<string, long> _fileSizes = new Dictionary<string, long>();


        public HandHistoryMonitor(string path)
        {
            this.WatchDirectory = path;
            _fileWatchTimer = new Timer(new TimerCallback(FileWatchTimer_Tick), this, Timeout.Infinite, Timeout.Infinite);
            this.LastCheck = DateTime.Now;
            this.Interval = TimeSpan.FromSeconds(10);
        }

        private void FileWatchTimer_Tick(object state)
        {
            this.Stop();

            string pattern = "*.txt";

            List<FileInfo> files = new List<FileInfo>();
            PartyPokerParser.DiscoverHandHistoryFiles(this.WatchDirectory, pattern, files);

            List<FileInfo> changes = new List<FileInfo>();

            foreach (FileInfo file in files)
            {
                bool parseFile = false;

                DateTime create = file.CreationTime;
                DateTime write = file.LastWriteTime;
                long size = file.Length;

                if (_fileSizes.ContainsKey(file.FullName))
                {
                    parseFile = size > _fileSizes[file.FullName];
                }
                else
                {
                    parseFile = create >= this.LastCheck;
                }

                _fileSizes[file.FullName] = file.Length;
                if (parseFile)
                {
                    changes.Add(file);
                }
            }

            if (changes.Count > 0)
            {
                foreach (FileInfo file in changes)
                {
                    FileSystemEventArgs e = new FileSystemEventArgs(WatcherChangeTypes.Changed, 
                        file.DirectoryName, file.Name);
                    OnFileChanged(this, e);
                }
            }

            this.LastCheck = DateTime.Now;

            this.Start();
        }

        public void Start()
        {
            _fileWatchTimer.Change(this.Interval, this.Interval);
        }

        public void Stop()
        {
            _fileWatchTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (this.FileChanged != null)
            {
                this.FileChanged(sender, e);
            }
        }

        public void Dispose()
        {
            this.Stop();
        }
    }
}
