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
    public class HandPlayer
    {
        public long Id { get; set; }
        public string PlayerName { get; set; }
        public long HandId { get; set; }
        public long SiteId { get; set; }
        public long SeatNumber { get; set; }
        public long PlayerId { get; set; }
        public long Stack { get; set; }
        public string HoleCard1 { get; set; }
        public string HoleCard2 { get; set; }
        public string FinalHand { get; set; }
        public bool Saved { get; set; }

        public void Save(SQLite.DataStore store, System.Data.SQLite.SQLiteConnection connection)
        {
            if (this.Saved == true)
            {
                return;
            }

            if (string.IsNullOrEmpty(this.PlayerName))
            {
                return;
            }

            this.PlayerId = -1;
            if (store.PlayerIdCache.ContainsKey(this.PlayerName))
            {
                this.PlayerId = store.PlayerIdCache[this.PlayerName];
            }
            else
            {
                Data.Player player = store.GetPlayer(this.SiteId, this.PlayerName, connection);
                if (player == null)
                {
                    this.PlayerId = store.InsertPlayer(this.SiteId, this.PlayerName, connection);
                }
                else
                {
                    this.PlayerId = player.Id;
                }

                store.PlayerIdCache[this.PlayerName] = this.PlayerId;
            }

            long handId = this.HandId;
            long offButton = -1; // TODO

            this.Id = store.InsertHandPlayer(handId, this.PlayerId, this.Stack, 
                this.SeatNumber, offButton, this.HoleCard1, this.HoleCard2,
                this.FinalHand, connection);

            this.Saved = true;
        }
    }
}
