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
using System.Linq;
using System.Text;

namespace JeromeTerry.PokerHandHistory.Data
{
    public static class DataFolder
    {
        private static string _path;

        public static string Path
        {
            get
            {
                return _path;
            }
        }

        private static string _databaseFileName = "pokerhandhistory.db";

        public static string DatabaseFileName
        {
            get
            {
                return _databaseFileName;
            }
        }

        public static string FullDatabasePath
        {
            get
            {
                return System.IO.Path.Combine(DataFolder.Path, DataFolder.DatabaseFileName);
            }
        }

        static DataFolder()
        {
            System.Environment.SpecialFolder folder = 
                Environment.SpecialFolder.LocalApplicationData;
            string dir = System.Environment.GetFolderPath(folder, Environment.SpecialFolderOption.DoNotVerify);
            _path = System.IO.Path.Combine(dir, "Jerome Terry", "Poker Hand History");
        }
    }
}
