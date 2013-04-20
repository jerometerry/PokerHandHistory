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
using System.Data;
using System.Collections.Generic;
using JeromeTerry.PokerHandHistory.Core;

namespace JeromeTerry.PokerHandHistory.Data
{
    public class Hand
    {
        List<Player> _players = null;
        List<HandPlayer> _handPlayers = null;

        public long Id { get; set; }
        public string HandNumber { get; set; }
        public long SiteId { get; set; }
        public string ButtonSeat { get; set; }
        public string PlayerCount { get; set; }
        public string Blinds { get; set; }
        public DateTime Start { get; set; }
        public long TableStakesId { get; set; }
        public GameType GameType { get; set; }
        public UnixTimestamp DateAdded { get; set; }
        public UnixTimestamp DateUpdated { get; set; }
        public bool Saved { get; set; }

        public List<Player> Players
        {
            get 
            {
                if (_players == null)
                {
                    _players = new List<Player>();
                }
                return _players; 
            }
        }

        public List<HandPlayer> HandPlayers
        {
            get 
            {
                if (_handPlayers == null)
                {
                    _handPlayers = new List<HandPlayer>();
                }
                return _handPlayers; 
            }
        }

        public Hand()
        {
        }

        public Hand(DataRow row)
        {
            this.Id = (long)row["Id"];
            this.HandNumber = (string)row["HandNumber"];
            this.SiteId = (long)row["SiteId"];
            this.DateAdded = new UnixTimestamp((long)row["DateAdded"]);
            if (!row.IsNull("DateUpdated"))
            {
                this.DateUpdated = new UnixTimestamp((long)row["DateUpdated"]);
            }
        }

        public void SetPlayerChipCount(string seat, string name, Data.ChipStack stack, SQLite.DataStore store)
        {
            HandPlayer player = new HandPlayer();
            player.SeatNumber = long.Parse(seat);
            player.PlayerName = name;
            player.Stack = stack.ToInt64();
            player.SiteId = this.SiteId;
            player.Saved = false;

            this.HandPlayers.Add(player);
        }

        public HandPlayer FindHandPlaer(string player)
        {
            foreach (HandPlayer hp in this.HandPlayers)
            {
                if (string.Compare(hp.PlayerName, player, true) == 0)
                {
                    return hp;
                }
            }

            return null;
        }

        public void SetHoleCards(string player, string holeCard1, string holeCard2)
        {
            HandPlayer hp = FindHandPlaer(player);
            if (hp != null)
            {
                hp.HoleCard1 = holeCard1;
                hp.HoleCard2 = holeCard2;
            }
        }

        public void SetFinalHand(string player, string finalHand)
        {
            HandPlayer hp = FindHandPlaer(player);
            if (hp != null)
            {
                hp.FinalHand = finalHand;
            }
        }

        public bool Save(SQLite.DataStore store, System.Data.SQLite.SQLiteConnection connection)
        {
            if (this.Saved == true)
            {
                return false;
            }

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            Hand existing = store.GetHand(this.SiteId, this.HandNumber);
            if (existing == null)
            {
                this.Id = store.InsertHand(this.SiteId, this.HandNumber, connection);

                foreach (HandPlayer player in this.HandPlayers)
                {
                    player.HandId = this.Id;
                    if (player.Saved == false)
                    {
                        player.Save(store, connection);
                    }
                }
            }
            else
            {
                Logger.Error("Hand {0} already imported", this.HandNumber);
            }

            this.Saved = true;
            return true;
        }
    }
}
