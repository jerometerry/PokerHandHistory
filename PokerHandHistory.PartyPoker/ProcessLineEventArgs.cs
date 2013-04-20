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
using System.Text.RegularExpressions;

namespace JeromeTerry.PokerHandHistory.PartyPoker
{
    public class ProcessLineEventArgs : EventArgs
    {
        FileInfo _file;
        string _line;
        long _index;
        PartyPokerParser _parser;
        Match _match;

        public FileInfo File
        {
            get { return _file; }
            set { _file = value; }
        }

        public string Line
        {
            get { return _line; }
            set { _line = value; }
        }

        public long Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public PartyPokerParser Parser
        {
            get { return _parser; }
            set { _parser = value; }
        }

        public Match Match
        {
            get { return _match; }
            set { _match = value; }
        }

        public string GetGroupValue(string groupName)
        {
            if (this.Match == null)
            {
                return null;
            }
            if (Match.Groups == null || Match.Groups.Count <= 1)
            {
                return null;
            }

            Group g = this.Match.Groups[groupName];
            if (g == null)
            {
                return null;
            }

            string val = g.Value;
            if (string.IsNullOrEmpty(val))
            {
                return null;
            }

            return val.Trim();
        }

        public string GetGroupValue(int index)
        {
            if (this.Match == null)
            {
                return null;
            }
            if (Match.Groups == null || Match.Groups.Count <= index)
            {
                return null;
            }

            Group g = this.Match.Groups[index];
            if (g == null)
            {
                return null;
            }

            string val = g.Value;
            if (string.IsNullOrEmpty(val))
            {
                return null;
            }

            return val.Trim();
        }

        public string FirstGroupValue
        {
            get
            {
                return GetGroupValue(1);
            }
        }

        public string SecondGroupValue
        {
            get
            {
                return GetGroupValue(2);
            }
        }

        public string ThirdGroupValue
        {
            get
            {
                return GetGroupValue(3);
            }
        }
    }
}
