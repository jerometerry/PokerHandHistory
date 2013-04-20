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
using System.Linq;
using JeromeTerry.PokerHandHistory.Core;
using Microsoft.Win32;

namespace JeromeTerry.PokerHandHistory.PartyPoker
{
    public sealed class PartyPokerParser : IDisposable
    {
        #region Implementation Data
        string _handHistoryDir;

        List<Data.Hand> _hands = new List<Data.Hand>();
        Data.Hand _currentHand;

        long _siteId;
        Data.PokerSite _site;

        Data.SQLite.DataStore _store;

        public event EventHandler HandImported;

        Statistics _stats = new Statistics();
        #endregion

        #region Constructors
        public PartyPokerParser(Data.SQLite.DataStore store)
        {
            _store = store;
            _handHistoryDir = GetHandHistoryDirectory();

            string partySiteName = "Party Poker";
            _site = this.Store.GetPokerSite(partySiteName);
            if (_site != null)
            {
                _siteId = _site.Id;
            }
            else
            {
                Logger.Error("Poker Site '{0}' not found in database", partySiteName);
            }

            _stats.TotalHands = store.GetHandCount();
            _stats.TotalPlayers = store.GetPlayerCount();
        }
        #endregion

        #region Operations (PartyPokerParser)
        public string HandHistoryDir
        {
            get { return _handHistoryDir; }
        }

        public Data.Hand CurrentHand
        {
            get { return _currentHand; }
            set { _currentHand = value; }
        }

        public void StartNewHand(string siteHandNumber)
        {
            this.OnHandImported();
            
            Data.Hand h = new Data.Hand();
            h.HandNumber = siteHandNumber;
            h.SiteId = this._siteId;
            h.Saved = false;

            this.CurrentHand = h;
            this._hands.Add(h);

            long handsInDb = this.Store.GetHandCount();
            this._stats.TotalHands = _hands.Count + handsInDb;
            this._stats.TotalPlayers = this.Store.GetPlayerCount();
        }

        public void SetPlayerChipCount(string seat, string name, Data.ChipStack stack)
        {
            this.CurrentHand.SetPlayerChipCount(seat, name, stack, this.Store);
        }

        public void SetHoleCards(string player, string holeCard1, string holeCard2)
        {
            this.CurrentHand.SetHoleCards(player, holeCard1, holeCard2);
        }

        public void SetFinalHand(string player, string finalHand)
        {
            this.CurrentHand.SetFinalHand(player, finalHand);
        }
        #endregion

        #region Implementation Operations
        private void OnHandImported()
        {
            if (this.HandImported != null)
            {
                this.HandImported(this, EventArgs.Empty);
            }
        }

        private Data.SQLite.DataStore Store
        {
            get
            {
                return _store;
            }
        }

        public List<FileInfo> DiscoverHandHistoryFiles()
        {
            List<FileInfo> files = new List<FileInfo>();
            DiscoverHandHistoryFiles(this._handHistoryDir, "*.txt", files);
            return files;
        }

        public static int DiscoverHandHistoryFiles(string dir, string searchPattern, List<FileInfo> discovered)
        {
            try
            {
                int count = 0;
                string[] files = Directory.GetFiles(dir, searchPattern, SearchOption.AllDirectories);
                if (files != null && files.Length > 0)
                {
                    foreach (string file in files)
                    {
                        discovered.Add(new FileInfo(file));
                    }
                    
                    count = files.Length;
                }

                return count;
            }
            catch (System.Exception ex)
            {
                Logger.Error("Error discovering hand history files: {0}", ex);
                return 0;
            }
        }

        public long CountHandsInFile(FileInfo file)
        {
            System.Text.RegularExpressions.Regex regex = LineHandlers.GetRegex("HAND_START");

            long count = 0;
            using (StreamReader sr = new StreamReader(File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                string s = sr.ReadToEnd();
                string[] lines = s.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                if (lines == null || lines.Length == 0)
                {
                    Logger.Warn("No lines found in file {0}", file);
                    return 0;
                }

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    System.Text.RegularExpressions.Match match = regex.Match(line);
                    if (match.Success)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public void ParseUserHandHistoryFile(FileInfo file)
        {
            this.CurrentHand = null;

            Logger.Info("Importing Hand History File {0} in directory {1}", file.Name, file.DirectoryName);

            Data.HandHistoryFile currentFile = this.Store.GetHandHistoryFile(file.FullName);
            if (currentFile == null)
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(file.Name);
                string path = file.FullName;

                currentFile = new Data.HandHistoryFile();
                currentFile.Path = path;
                currentFile.Name = name;
                currentFile.SiteId = this._siteId;
                currentFile.DateAdded = UnixTimestamp.UtcNow;
                currentFile.MachineName = Environment.MachineName;
                currentFile.User = Environment.UserName;
                currentFile.DateCreated = new UnixTimestamp(file.CreationTimeUtc);

                using (this.Store.Lock())
                {
                    currentFile.Id = this.Store.InsertHandHistoryFile(_siteId,
                        name, path, 0, currentFile.MachineName,
                        currentFile.User, currentFile.DateCreated.UnixTime);
                }
            }

            using (StreamReader sr = new StreamReader(File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                string s = sr.ReadToEnd();
                string[] lines = s.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                if (lines == null || lines.Length == 0)
                {
                    Logger.Warn("No lines found in file {0}", file);
                    return;
                }

                // last is the last line number parsed;
                // if the file hasn't been parsed yet, last will be zero
                long last = currentFile.LineNumber;

                for (long i = last; i < lines.Length; i++)
                {
                    string line = lines[i];
                    ParseUserHandHistoryLine(file, line, i);
                }
                
                this.Store.UpdateLastLine(currentFile.Id, lines.Length);
            }

            using (this.Store.Lock())
            {
                using (System.Data.SQLite.SQLiteConnection connection = this.Store.CreateConnection())
                {
                    connection.Open();

                    using (System.Data.SQLite.SQLiteTransaction transaction = connection.BeginTransaction())
                    {
                        foreach (Data.Hand hand in this._hands)
                        {
                            bool saved = hand.Save(this.Store, connection);
                        }

                        transaction.Commit();
                    }

                    this._stats.TotalPlayers = this.Store.GetPlayerCount();
                    this._stats.TotalHands = this.Store.GetHandCount(); 
                }
            }

            this.CurrentHand = null;
            _hands.Clear();

            Logger.Info("Importing Hand History File {0} complete", file.Name);
        }

        private LineHandler ParseUserHandHistoryLine(FileInfo file, string line, long index)
        {
            ProcessLineEventArgs e = new ProcessLineEventArgs();
            e.File = file;
            e.Line = line;
            e.Index = index;
            e.Parser = this;

            foreach (LineHandler handler in LineHandlers.Handlers)
            {
                if (handler.Handle(this, e))
                {
                    return handler;
                }
            }

            return null;
        }

        public static string GetHandHistoryDirectory()
        {
            string key = @"HKEY_CURRENT_USER\Software\PartyGaming\PartyPoker";
            string valueName = "AppPath";

            Logger.Info("Loading PartyGaming app path from registry key {0}, value name {1}",
                key, valueName);

            object val = Registry.GetValue(key, valueName, null);
            if (val == null)
            {
                Logger.Error("Registry.GetValue failed");
                return null;
            }

            string exe = System.Convert.ToString(val);
            if (string.IsNullOrEmpty(exe))
            {
                Logger.Error("Registry.GetValue failed");
                return null;
            }
            else
            {
                Logger.Info("Read AppPath value: {0}", exe);
            }

            FileInfo file = new FileInfo(exe);
            if (file.Exists == false)
            {
                Logger.Error("PartyGaming appliication missing: {0}", exe);
                return null;
            }

            DirectoryInfo partyGamingDirectoryInfo = file.Directory;
            if (partyGamingDirectoryInfo == null)
            {
                Logger.Error("Could get determine Party Gaming application directory");
                return null;
            }

            if (partyGamingDirectoryInfo.Exists == false)
            {
                Logger.Error("PartyGaming appliication path does not exist: {0}", partyGamingDirectoryInfo.FullName);
                return null;
            }

            string partyGamingDir = partyGamingDirectoryInfo.FullName;
            string dir = Path.Combine(partyGamingDir, "PartyPoker", "HandHistory");

            DirectoryInfo handHistoryDirectoryInfo = new DirectoryInfo(dir);
            if (handHistoryDirectoryInfo.Exists == false)
            {
                Logger.Error("Hand history path does not exist: {0}", dir);
                return null;
            }
            else
            {
                Logger.Info("Found Party Poker Hand History Directory: {0}", handHistoryDirectoryInfo.FullName);
                return handHistoryDirectoryInfo.FullName;
            }
        }

        public Statistics Stats
        {
            get { return _stats; }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            _store = null;
        }
        #endregion
    }
}
