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
using System.Data;
using System.Data.SQLite;
using System.IO;
using JeromeTerry.PokerHandHistory.Core;

namespace JeromeTerry.PokerHandHistory.Data.SQLite
{
    public class DataStore
    {
        private string _databasefile;

        public long Inserts { get; set; }
        public long Selects { get; set; }

        private string ConnectionString
        {
            get
            {
                string cs = string.Format("Data Source={0}", this.DatabaseFile);
                return cs;
            }
        }

        private string DatabaseFile
        {
            get
            {
                return _databasefile;
            }
        }

        Dictionary<string, long> _playerIdCache =
            new Dictionary<string, long>();

        public Dictionary<string, long> PlayerIdCache
        {
            get { return _playerIdCache; }
        }

        private static object _instanceLock = new object();
        private static DataStore _instance;

        internal object _lockObject = new object();
        internal int _lockCount = 0;

        public static DataStore Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new DataStore(DataFolder.FullDatabasePath);
                    }
                    return _instance;
                }
            }
        }

        public DbLock Lock()
        {
            return new DbLock(this);
        }

        public DataStore(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentException("File cannot be null", file);
            }

            if (System.IO.File.Exists(file) == false)
            {
                throw new System.IO.FileNotFoundException("Database file doesn't exist", _databasefile);
            }

            this._databasefile = file;
        }

        public static string InitializeDatabase()
        {
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            FileInfo exeInfo = new FileInfo(exePath);
            string filename = Data.DataFolder.DatabaseFileName;
            string defaultDatabasePath = Path.Combine(exeInfo.Directory.FullName, filename);

            string defaultDatabaseVersion = null;
            string currentDatabaseVersion = null;

            bool defaultDatabaseExists = File.Exists(defaultDatabasePath);
            bool currentDatabaseExists = File.Exists(Data.DataFolder.FullDatabasePath);
            bool databaseVersionsDifferent = false;

            if (!defaultDatabaseExists && !currentDatabaseExists)
            {
                Logger.Error("Database could not be initialized because the default database {0} is missing",
                    defaultDatabasePath);
                return null;
            }

            if (defaultDatabaseExists)
            {
                DataStore ds = new DataStore(defaultDatabasePath);
                Setting setting = ds.GetSetting("Version");

                if (setting != null)
                {
                    UnixTimestamp ts = setting.DateAdded;
                    long unixTime = ts.UnixTime;
                    DateTime utc = ts.UtcTime;
                    DateTime local = utc.ToLocalTime();
                    defaultDatabaseVersion = setting.Value;
                    Logger.Info("Default database: {0} version: {1} ",
                        defaultDatabasePath, defaultDatabaseVersion);
                }
            }

            if (currentDatabaseExists)
            {
                DataStore ds = Data.SQLite.DataStore.Instance;
                Setting setting = ds.GetSetting("Version");

                if (setting != null)
                {
                    UnixTimestamp ts = setting.DateAdded;
                    long unixTime = ts.UnixTime;
                    DateTime utc = ts.UtcTime;
                    DateTime local = utc.ToLocalTime();
                    currentDatabaseVersion = setting.Value;
                    Logger.Info("Current database: {0} version: {1} ",
                        Data.DataFolder.FullDatabasePath, currentDatabaseVersion);
                }
            }
            else
            {
                Logger.Warn("Database is missing.");
            }

            if (defaultDatabaseExists && currentDatabaseExists)
            {
                databaseVersionsDifferent =
                    string.Compare(currentDatabaseVersion,
                        defaultDatabaseVersion, true) != 0;
            }

            if (databaseVersionsDifferent)
            {
                Logger.Warn("Database version mismatch.");
            }

            if (!currentDatabaseExists || databaseVersionsDifferent)
            {
                string src = defaultDatabasePath;
                Logger.Warn("Trying to copy the default database from '{0}'", src);

                FileInfo source = new FileInfo(src);
                if (source.Exists == true)
                {
                    source.CopyTo(Data.DataFolder.FullDatabasePath, true);
                    Logger.Info("Default database copied to data directory");
                    return Data.DataFolder.FullDatabasePath;
                }
                else
                {
                    Logger.Warn("Default database not found:  '{0}'", src);
                    return null;
                }
            }
            else
            {
                return Data.DataFolder.FullDatabasePath;
            }
        }

        public SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection(this.ConnectionString);
        }

        public DataTable GetData(string sql, SQLiteConnection connection)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            try
            {
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    DataTable dt = new DataTable();

                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }

                    return dt;
                }
            }
            finally
            {
                sw.Stop();
                double ms = sw.ElapsedMilliseconds;
                Selects++;
            }
        }

        public void ResetStats()
        {
            Inserts = Selects = 0;
        }

        public long GetCount(string sql, SQLiteConnection connection)
        {
            try
            {
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    SQLiteDataReader reader = cmd.ExecuteReader();
                    long count = -1;
                    if (reader.Read())
                    {
                        count = reader.GetInt64(0);
                    }

                    return count;
                }
            }
            finally
            {
                this.Selects++;
            }
        }

        public Setting GetSetting(string name)
        {
            using (SQLiteConnection connection = this.CreateConnection())
            {
                return GetSetting(name, connection);
            }
        }

        public Setting GetSetting(string name, SQLiteConnection connection)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            try
            {
                string sql = string.Format("SELECT * FROM Settings WHERE Name = '{0}'", name);
                DataTable dt = GetData(sql, connection);
                int rowCount = dt.Rows.Count;
                if (rowCount > 0)
                {
                    DataRow row = dt.Rows[0];
                    Setting setting = new Setting(row);
                    return setting;
                }
            }
            finally
            {
                sw.Stop();
                double ms = sw.ElapsedMilliseconds;
            }

            return null;
        }

        public List<Player> GetPlayers()
        {
            using (SQLiteConnection connection = this.CreateConnection())
            {
                return GetPlayers(connection);
            }
        }

        public List<Player> GetPlayers(SQLiteConnection connection)
        {
            string sql = string.Format("SELECT * FROM Players");
            DataTable dt = GetData(sql, connection);
            List<Player> players = new List<Player>();
            foreach (DataRow row in dt.Rows)
            {
                Player player = new Player(row);
                players.Add(player);
            }
            return players;
        }

        public long GetPlayerCount()
        {
            using (SQLiteConnection connection = this.CreateConnection())
            {
                return GetPlayerCount(connection);
            }
        }

        public long GetPlayerCount(SQLiteConnection connection)
        {
            string sql = "SELECT COUNT(*) FROM Players";
            long count = GetCount(sql, connection);
            return count;
        }

        public HandHistoryFile GetHandHistoryFile(string path)
        {
            using (SQLiteConnection connection = this.CreateConnection())
            {
                return GetHandHistoryFile(path, connection);
            }
        }

        public HandHistoryFile GetHandHistoryFile(string path, SQLiteConnection connection)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            string sql = string.Format("SELECT * FROM HandHistoryFiles WHERE Path = '{0}'", path);
            DataTable dt = GetData(sql, connection);
            int rowCount = dt.Rows.Count;
            if (rowCount > 0)
            {
                DataRow row = dt.Rows[0];
                HandHistoryFile file = new HandHistoryFile(row);
                return file;
            }

            return null;
        }

        public long GetHandCount()
        {
            using (SQLiteConnection connection = this.CreateConnection())
            {
                return GetHandCount(connection);
            }
        }

        public long GetHandCount(SQLiteConnection connection)
        {
            string sql = "SELECT COUNT(*) FROM Hands";
            return GetCount(sql, connection);
        }

        public long GetHandPlayerCount()
        {
            using (SQLiteConnection connection = this.CreateConnection())
            {
                return GetHandPlayerCount(connection);
            }
        }

        public long GetHandPlayerCount(SQLiteConnection connection)
        {
            string sql = "SELECT COUNT(*) FROM HandPlayers";
            return GetCount(sql, connection);
        }

        public long GetHandPlayerCount(long playerId)
        {
            using (SQLiteConnection connection = this.CreateConnection())
            {
                return GetHandPlayerCount(playerId, connection);
            }
        }

        public long GetHandPlayerCount(long playerId, SQLiteConnection connection)
        {
            string sql = string.Format("SELECT COUNT(*) FROM HandPlayers WHERE PlayerId = {0}", playerId);
            return GetCount(sql, connection);
        }

        public Hand GetHand(long siteId, string handNumber)
        {
            using (SQLiteConnection connection = this.CreateConnection())
            {
                return GetHand(siteId, handNumber, connection);
            }
        }

        public Hand GetHand(long siteId, string handNumber, SQLiteConnection connection)
        {
            if (string.IsNullOrEmpty(handNumber))
            {
                return null;
            }
            
            string sql = string.Format("SELECT * FROM Hands WHERE SiteId = {0} AND HandNumber = '{1}'", siteId, handNumber);
            DataTable dt = GetData(sql, connection);
            int rowCount = dt.Rows.Count;
            if (rowCount > 0)
            {
                DataRow row = dt.Rows[0];
                Hand hand = new Hand(row);
                return hand;
            }

            return null;
        }

        public PokerSite GetPokerSite(string name)
        {
            using (SQLiteConnection connection = this.CreateConnection())
            {
                return GetPokerSite(name, connection);
            }
        }

        public PokerSite GetPokerSite(string name, SQLiteConnection connection)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            
            string sql = string.Format("SELECT * FROM PokerSites WHERE Name = '{0}'", name);
            DataTable dt = GetData(sql, connection);
            int rowCount = dt.Rows.Count;
            if (rowCount > 0)
            {
                DataRow row = dt.Rows[0];
                PokerSite site = new PokerSite(row);
                return site;
            }

            return null;
        }

        public Player GetPlayer(long siteId, string name)
        {
            using (SQLiteConnection connection = this.CreateConnection())
            {
                return GetPlayer(siteId, name, connection);
            }
        }

        public Player GetPlayer(long siteId, string name, SQLiteConnection connection)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            try
            {
                string sql = string.Format("SELECT * FROM Players WHERE Name = '{0}' AND SiteId = {1}", name, siteId);
                DataTable dt = GetData(sql, connection);
                int rowCount = dt.Rows.Count;
                if (rowCount > 0)
                {
                    DataRow row = dt.Rows[0];
                    Player player = new Player(row);
                    return player;
                }
            }
            finally
            {
                sw.Stop();
                double ms = sw.ElapsedMilliseconds;
            }

            return null;
        }

        public long InsertPlayer(long siteId, string name)
        {
            using (SQLiteConnection connection = this.CreateConnection())
            {
                return InsertPlayer(siteId, name, connection);
            }
        }

        public long InsertPlayer(long siteId, string name, SQLiteConnection connection)
        {
            long unixTime = UnixTimestamp.CurrentUnixTime;

            using (SQLiteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "INSERT INTO Players (SiteId, Name, DateAdded) VALUES (@siteId, @name, @ts); SELECT last_insert_rowid() AS [Id]";
                
                SQLiteParameter siteIdParam = cmd.CreateParameter();
                siteIdParam.ParameterName = "@siteId";
                siteIdParam.Value = siteId;
                cmd.Parameters.Add(siteIdParam);
                
                SQLiteParameter nameParam = cmd.CreateParameter();
                nameParam.ParameterName = "@name";
                nameParam.Value = name;
                cmd.Parameters.Add(nameParam);

                SQLiteParameter tsParam = cmd.CreateParameter();
                tsParam.ParameterName = "@ts";
                tsParam.Value = unixTime;
                cmd.Parameters.Add(tsParam);

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                SQLiteDataReader reader = cmd.ExecuteReader();
                long playerId = -1;
                if (reader.Read())
                {
                    playerId = reader.GetInt64(0);
                }

                this.Inserts++;
                return playerId;
            }
        }

        public int UpdateLastLine(long fileId, long lineNumber)
        {
            using (SQLiteConnection connection = this.CreateConnection())
            {
                return UpdateLastLine(fileId, lineNumber, connection);
            }
        }

        public int UpdateLastLine(long fileId, long lineNumber, SQLiteConnection connection)
        {
            long unixTime = UnixTimestamp.CurrentUnixTime;

            using (SQLiteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "UPDATE HandHistoryFiles SET LineNumber = @line, DateUpdated = @ts WHERE Id = @id";

                SQLiteParameter idParam = cmd.CreateParameter();
                idParam.ParameterName = "@id";
                idParam.Value = fileId;
                cmd.Parameters.Add(idParam);

                SQLiteParameter lineParam = cmd.CreateParameter();
                lineParam.ParameterName = "@line";
                lineParam.Value = lineNumber;
                cmd.Parameters.Add(lineParam);

                SQLiteParameter tsParam = cmd.CreateParameter();
                tsParam.ParameterName = "@ts";
                tsParam.Value = unixTime;
                cmd.Parameters.Add(tsParam);

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                return cmd.ExecuteNonQuery();
            }
        }

        public long InsertHandHistoryFile(long siteId, string name, string path,
            long lineNumber, string machineName, string user, long dateCreated)
        {
            using (SQLiteConnection connection = this.CreateConnection())
            {
                return InsertHandHistoryFile(siteId, name, path, lineNumber, 
                    machineName, user, dateCreated, connection);
            }
        }

        public long InsertHandHistoryFile(long siteId, string name, string path, 
            long lineNumber, string machineName, string user, long dateCreated, 
            SQLiteConnection connection)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(path))
            {
                return -1;
            }

            long unixTime = UnixTimestamp.CurrentUnixTime;

            using (SQLiteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "INSERT INTO HandHistoryFiles (SiteId, Name, Path, LineNumber, MachineName, User, DateCreated, DateAdded) VALUES (@siteId, @name, @path, @line, @machine, @user, @dc, @ts); SELECT last_insert_rowid() AS [Id]";

                SQLiteParameter siteIdParam = cmd.CreateParameter();
                siteIdParam.ParameterName = "@siteId";
                siteIdParam.Value = siteId;
                cmd.Parameters.Add(siteIdParam);

                SQLiteParameter nameParam = cmd.CreateParameter();
                nameParam.ParameterName = "@name";
                nameParam.Value = name;
                cmd.Parameters.Add(nameParam);

                SQLiteParameter pathParam = cmd.CreateParameter();
                pathParam.ParameterName = "@path";
                pathParam.Value = path;
                cmd.Parameters.Add(pathParam);

                SQLiteParameter lineParam = cmd.CreateParameter();
                lineParam.ParameterName = "@line";
                lineParam.Value = lineNumber;
                cmd.Parameters.Add(lineParam);

                SQLiteParameter machineNameParam = cmd.CreateParameter();
                machineNameParam.ParameterName = "@machine";
                machineNameParam.Value = machineName;
                cmd.Parameters.Add(machineNameParam);

                SQLiteParameter userParam = cmd.CreateParameter();
                userParam.ParameterName = "@user";
                userParam.Value = user;
                cmd.Parameters.Add(userParam);

                SQLiteParameter dcParam = cmd.CreateParameter();
                dcParam.ParameterName = "@dc";
                dcParam.Value = dateCreated;
                cmd.Parameters.Add(dcParam);

                SQLiteParameter tsParam = cmd.CreateParameter();
                tsParam.ParameterName = "@ts";
                tsParam.Value = unixTime;
                cmd.Parameters.Add(tsParam);

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                SQLiteDataReader reader = cmd.ExecuteReader();
                long fileId = -1;
                if (reader.Read())
                {
                    fileId = reader.GetInt64(0);
                }

                this.Inserts++;
                return fileId;
            }
        }

        public long InsertHand(long siteId, string handNumber)
        {
            using (SQLiteConnection connection = this.CreateConnection())
            {
                return InsertHand(siteId, handNumber, connection);
            }
        }

        public long InsertHand(long siteId, string handNumber, SQLiteConnection connection)
        {
            if (string.IsNullOrEmpty(handNumber))
            {
                return -1;
            }

            long unixTime = UnixTimestamp.CurrentUnixTime;

            using (SQLiteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "INSERT INTO Hands (SiteId, HandNumber, DateAdded) VALUES (@siteId, @handNumber, @ts); SELECT last_insert_rowid() AS [Id]";
                
                SQLiteParameter siteIdParam = cmd.CreateParameter();
                siteIdParam.ParameterName = "@siteId";
                siteIdParam.Value = siteId;
                cmd.Parameters.Add(siteIdParam);
                
                SQLiteParameter handNumberParam = cmd.CreateParameter();
                handNumberParam.ParameterName = "@handNumber";
                handNumberParam.Value = handNumber;
                cmd.Parameters.Add(handNumberParam);

                SQLiteParameter tsParam = cmd.CreateParameter();
                tsParam.ParameterName = "@ts";
                tsParam.Value = unixTime;
                cmd.Parameters.Add(tsParam);

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                SQLiteDataReader reader = cmd.ExecuteReader();
                long playerId = -1;
                if (reader.Read())
                {
                    playerId = reader.GetInt64(0);
                }

                this.Inserts++;
                return playerId;
            }
        }

        public long InsertHandPlayer(long handId, long playerId, long stack, 
            long seatNumber, long offButton, string holeCard1, string holeCard2, string finalHand)
        {
            using (SQLiteConnection connection = this.CreateConnection())
            {
                return InsertHandPlayer(handId, playerId, stack, seatNumber, 
                    offButton, holeCard1, holeCard2, finalHand, connection);
            }
        }

        public long InsertHandPlayer(long handId, long playerId, long stack, 
            long seatNumber, long offButton, string holeCard1, string holeCard2,
            string finalHand, SQLiteConnection connection)
        {
            long unixTime = UnixTimestamp.CurrentUnixTime;

            using (SQLiteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "INSERT INTO HandPlayers (HandId, PlayerId, StartStack, SeatNumber, OffButton, HoleCard1, HoleCard2, FinalHand, DateAdded) VALUES (@handId, @playerId, @stack, @seat, @offBtn, @hc1, @hc2, @final, @ts); SELECT last_insert_rowid() AS [Id]";
                
                SQLiteParameter handIdParam = cmd.CreateParameter();
                handIdParam.ParameterName = "@handId";
                handIdParam.Value = handId;
                cmd.Parameters.Add(handIdParam);

                SQLiteParameter playerIdParam = cmd.CreateParameter();
                playerIdParam.ParameterName = "@playerId";
                playerIdParam.Value = playerId;
                cmd.Parameters.Add(playerIdParam);

                SQLiteParameter stackParam = cmd.CreateParameter();
                stackParam.ParameterName = "@stack";
                stackParam.Value = stack;
                cmd.Parameters.Add(stackParam);

                SQLiteParameter seatParam = cmd.CreateParameter();
                seatParam.ParameterName = "@seat";
                seatParam.Value = seatNumber;
                cmd.Parameters.Add(seatParam);

                SQLiteParameter offBtnParam = cmd.CreateParameter();
                offBtnParam.ParameterName = "@offBtn";
                offBtnParam.Value = offButton;
                cmd.Parameters.Add(offBtnParam);

                SQLiteParameter hc1Param = cmd.CreateParameter();
                hc1Param.ParameterName = "@hc1";
                hc1Param.Value = holeCard1;
                cmd.Parameters.Add(hc1Param);

                SQLiteParameter hc2Param = cmd.CreateParameter();
                hc2Param.ParameterName = "@hc2";
                hc2Param.Value = holeCard2;
                cmd.Parameters.Add(hc2Param);

                SQLiteParameter finalParam = cmd.CreateParameter();
                finalParam.ParameterName = "@final";
                finalParam.Value = finalHand;
                cmd.Parameters.Add(finalParam);

                SQLiteParameter tsParam = cmd.CreateParameter();
                tsParam.ParameterName = "@ts";
                tsParam.Value = unixTime;
                cmd.Parameters.Add(tsParam);

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                SQLiteDataReader reader = cmd.ExecuteReader();
                long handPlayerId = -1;
                if (reader.Read())
                {
                    handPlayerId = reader.GetInt64(0);
                }

                this.Inserts++;
                return handPlayerId;
            }
        }
    }
}
