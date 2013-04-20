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

namespace JeromeTerry.PokerHandHistory.Core
{
    public class UnixTimestamp
    {
        #region Implementation Data
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0,
                                                      DateTimeKind.Utc);

        long _unixTime;
        DateTime _utcTime;
        #endregion


        #region Constructors
        public UnixTimestamp()
        {
        }

        public UnixTimestamp(DateTime utc)
        {
            _utcTime = utc;
            _unixTime = ToUnixTime(utc);
        }

        public UnixTimestamp(long unixTime)
        {
            _unixTime = unixTime;
            _utcTime = FromUnixTime(unixTime);
        }
        #endregion

        #region Operations (UnixTimestamp)
        public long UnixTime
        {
            get { return _unixTime; }
            set { _unixTime = value; }
        }

        public DateTime UtcTime
        {
            get { return _utcTime; }
            set { _utcTime = value; }
        }
        #endregion

        #region Static Methods
        public static long ToUnixTime(DateTime utc)
        {
            TimeSpan ts = utc.Subtract(Epoch);
            double seconds = ts.TotalSeconds;
            return Convert.ToInt64(seconds);
        }

        public static DateTime FromUnixTime(long unixTime)
        {
            double seconds = Convert.ToDouble(unixTime);
            TimeSpan ts = TimeSpan.FromSeconds(unixTime);
            return Epoch.Add(ts);
        }

        public static UnixTimestamp UtcNow
        {
            get
            {
                return new UnixTimestamp(DateTime.UtcNow);
            }
        }

        public static long CurrentUnixTime
        {
            get
            {
                return UnixTimestamp.UtcNow.UnixTime;
            }
        }
        #endregion
    }
}
