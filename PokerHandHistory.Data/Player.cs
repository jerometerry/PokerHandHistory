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

using System.Data;
using JeromeTerry.PokerHandHistory.Core;

namespace JeromeTerry.PokerHandHistory.Data
{
    public class Player
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long SiteId { get; set; }
        public string Notes { get; set; }
        public long HandPlayerCount { get; set; }
        public UnixTimestamp DateAdded { get; set; }
        public UnixTimestamp DateUpdated { get; set; }

        public Player()
        {
        }

        public Player(DataRow row)
        {
            this.Id = (long)row["Id"];
            this.Name = (string)row["Name"];
            this.SiteId = (long)row["SiteId"];
            if (!row.IsNull("Notes"))
            {
                this.Notes = (string)row["Notes"];
            }
            this.DateAdded = new UnixTimestamp((long)row["DateAdded"]);
            if (!row.IsNull("DateUpdated"))
            {
                this.DateUpdated = new UnixTimestamp((long)row["DateUpdated"]);
            }
        }
    }
}
