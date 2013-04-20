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
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

namespace JeromeTerry.PokerHandHistory.PartyPoker
{
    public class LineHandlers
    {
        private static Dictionary<string, Regex> _regex = InitializeRegex();

        private static Dictionary<string, Regex> InitializeRegex()
        {
            Dictionary<string, Regex> regex = new Dictionary<string, Regex>();

            regex.Add("BLINDS", CreateRegex(@"Blinds\((?<blinds>.*)\)"));
            regex.Add("BUTTON", CreateRegex(@".*Seat\s(?<seatNumber>\d+) is the button"));
            regex.Add("DEALING_DOWN_CARDS", CreateRegex(@".*Dealing down cards.*"));
            regex.Add("DEALING_FLOP", CreateRegex(@".*Dealing Flop.*\[(?<flop>.*)\]"));
            regex.Add("DEALING_TURN", CreateRegex(@".*Dealing Turn.*\[(?<turn>.*)\]"));
            regex.Add("DEALING_RIVER", CreateRegex(@".*Dealing River.*\[(?<river>.*)\]"));
            regex.Add("DEALT_TO", CreateRegex(@".*Dealt to\s(?<user>.*)\s\[(?<holecards>.*)\].*"));
            regex.Add("HAND_START", CreateRegex(@".*\sHand History for Game(?<gameNumber>.*)\s.*"));
            regex.Add("PLAYER_CHIP_COUNT", CreateRegex(@".*Seat\s(?<seatNumber>\d+):\s(?<user>.*)\s\((?<chips>.*)\).*"));
            regex.Add("PLAYER_COUNT", CreateRegex(@"Total number of players : (?<playerCount>.*)"));
            regex.Add("TABLE_INFO", CreateRegex(@"Table Summer Million Special (2686100) Table #13 (Real Money)"));
            //regex.Add("TOURNEY_INFO", CreateRegex(@"(?<tourney>.*):\s(?<tourneyNumber>\d+)\sLevel:\s(?<level>\d+).*"));
            regex.Add("CASH_TABLE_HAND", CreateRegex(@"(?<stakes>.*USD)\s(?<gameType>.*)\s-\s(?<dayOfWeek>.*),\s(?<month>.*)\s(?<day>.*),\s(?<hour>.*):(?<minute>.*):(?<second>.*)\s(?<timezone>.*)\s(?<year>\d+.)*"));
            regex.Add("TRNY_HAND", CreateRegex(@"(?<gameType>.*)\sTrny:\s(?<trnyId>.*)\sLevel:\s(?<levelId>.*)\sBlinds.*\((?<blinds>.*)\)\s-\s(?<dayOfWeek>.*),\s(?<month>.*)\s(?<day>.*),\s(?<hour>.*):(?<minute>.*):(?<second>.*)\s(?<timezone>.*)\s(?<year>\d+.)*"));
            regex.Add("USER_BETS", CreateRegex(@"(?<user>.*)\sbets \[(?<bet>.*)\]"));
            regex.Add("USER_CALLS", CreateRegex(@"(?<user>.*)\scalls\s\[(?<call>.*)\].*"));
            regex.Add("USER_CHECKS", CreateRegex(@"(?<user>.*)\schecks"));
            regex.Add("USER_FOLDS", CreateRegex(@"(?<user>.*)\sfolds.*"));
            regex.Add("USER_MUCKS_CARDS", CreateRegex(@"(?<user>.*)does not show cards."));
            regex.Add("USER_RAISES", CreateRegex(@"(?<user>.*)\sraises\s\[(?<raise>.*)\].*"));
            regex.Add("USER_WINS", CreateRegex(@"(?<user>.*)\swins\s(?<won>.*)\schips"));
            regex.Add("WILDCARD", CreateRegex(@".*"));
            regex.Add("CHIP_STACK", CreateRegex(@"$?(?<stack>[0-9]+(\.[0-9][0-9])?).*"));
            regex.Add("SHOW_HAND", CreateRegex(@"(?<user>.*)\sshows\s\[\s(?<hc1>.*),\s(?<hc2>.*)\s\](?<hand>.*)"));

            return regex;
        }

        internal static Regex GetRegex(string key)
        {
            return _regex[key];
        }

        internal static Regex CreateRegex(string regex)
        {
            return new Regex(regex, RegexOptions.Compiled);
        }

        public static Data.ChipStack ConvertStack(string chipCount)
        {
            if (string.IsNullOrEmpty(chipCount))
            {
                return null;
            }

            chipCount = chipCount.Replace(",", "");
            bool isMoney = chipCount.IndexOf("$") >= 0 || chipCount.IndexOf(".") >= 0;

            Regex regex = GetRegex("CHIP_STACK");

            string stack = chipCount;
            System.Text.RegularExpressions.Match match = regex.Match(chipCount);
            if (match.Success)
            {
                stack = match.Groups["stack"].Value;
            }

            Data.StackType stackType = isMoney ? Data.StackType.Money : Data.StackType.Chips;
            return new Data.ChipStack(double.Parse(stack), stackType);
        }

        private static DateTime ParseHandTimeStamp(string dayOfWeek, 
            string month, string day, string hour, string minute, string second, 
            string timeZone, string year)
        {
            DateTime dt = DateTime.Now;
            int y, m, d, hr, min, sec;
            y = dt.Year;
            m = dt.Month;
            d = dt.Day;
            hr = dt.Hour;
            min = dt.Minute;
            sec = dt.Second;

            int.TryParse(year, out y);
            if (year.Length == 2)
            {
                // compensate if the year is stored as 2 digits
                y += 2000;
            }

            m = DateTime.ParseExact(month, "MMMM", CultureInfo.CurrentCulture).Month;
            int.TryParse(day, out d);

            int.TryParse(hour, out hr);
            int.TryParse(minute, out min);
            int.TryParse(second, out sec);

            DateTime handStartDate = new DateTime(y, m, d, hr, min, sec);
            return handStartDate;
        }

        private static LineHandler[] _handlers =
            new LineHandler[] { 
                 new LineHandler(GetRegex("HAND_START"), 
                    new ProcessLineHandler(delegate(object sender, ProcessLineEventArgs e) {
                        string handNumber = e.FirstGroupValue;
                        e.Parser.StartNewHand(handNumber);
                        }))
                ,new LineHandler(GetRegex("TRNY_HAND"), 
                    new ProcessLineHandler(delegate(object sender, ProcessLineEventArgs e) {
                        string gameType = e.GetGroupValue("gameType");
                        string trnyId = e.GetGroupValue("trnyId");
                        string levelId = e.GetGroupValue("levelId");
                        string blinds = e.GetGroupValue("blinds");
                        string dayOfWeek = e.GetGroupValue("dayOfWeek");
                        string month = e.GetGroupValue("month");
                        string day = e.GetGroupValue("day");
                        string hour = e.GetGroupValue("hour");
                        string minute = e.GetGroupValue("minute");
                        string second = e.GetGroupValue("second");
                        string timeZone = e.GetGroupValue("timezone"); // All times should be in Eastern Standard Time
                        string year = e.GetGroupValue("year");

                        DateTime handStartDate = ParseHandTimeStamp(dayOfWeek, month,
                            day, hour, minute, second, timeZone, year);

                        Data.Hand hand = e.Parser.CurrentHand;
                        hand.GameType = Data.GameType.Tournament;
                        hand.Start = handStartDate;
                        
                    }))
                ,new LineHandler(GetRegex("CASH_TABLE_HAND"), 
                    new ProcessLineHandler(delegate(object sender, ProcessLineEventArgs e) {
                        string stakes = e.GetGroupValue("stakes");
                        string gameType = e.GetGroupValue("gameType");

                        string dayOfWeek = e.GetGroupValue("dayOfWeek");
                        string month = e.GetGroupValue("month");
                        string day = e.GetGroupValue("day");
                        string hour = e.GetGroupValue("hour");
                        string minute = e.GetGroupValue("minute");
                        string second = e.GetGroupValue("second");
                        string timeZone = e.GetGroupValue("timezone"); // All times should be in Eastern Standard Time
                        string year = e.GetGroupValue("year");

                        DateTime handStartDate = ParseHandTimeStamp(dayOfWeek, month,
                            day, hour, minute, second, timeZone, year);

                        Data.Hand hand = e.Parser.CurrentHand;
                        hand.GameType = Data.GameType.CashGame;
                        hand.Start = handStartDate;
                    }))
                //,new LineHandler(CreateRegex("TABLE_INFO"), 
                //    new ProcessLineHandler(delegate(object sender, ProcessLineEventArgs e) {
                //    }))
                ,new LineHandler(GetRegex("BUTTON"), 
                    new ProcessLineHandler(delegate(object sender, ProcessLineEventArgs e) {
                        e.Parser.CurrentHand.ButtonSeat = e.FirstGroupValue;
                        }))
                ,new LineHandler(GetRegex("PLAYER_COUNT"), 
                    new ProcessLineHandler(delegate(object sender, ProcessLineEventArgs e) {
                        string count = e.FirstGroupValue;
                        string[] tokens = count.Split('/');
                        string playersAtTable = tokens[0];
                        string numberOfSeats = tokens[1];
                        e.Parser.CurrentHand.PlayerCount = e.FirstGroupValue;
                        }))
                ,new LineHandler(GetRegex("PLAYER_CHIP_COUNT"), 
                    new ProcessLineHandler(delegate(object sender, ProcessLineEventArgs e) {
                        string seat = e.FirstGroupValue;
                        string user = e.SecondGroupValue;
                        string chips = e.ThirdGroupValue;

                        Data.ChipStack stack = ConvertStack(chips);

                        e.Parser.SetPlayerChipCount(seat, user, stack);
                        }))
                //,new LineHandler(CreateRegex("TOURNEY_INFO"), 
                //    new ProcessLineHandler(delegate(object sender, ProcessLineEventArgs e) {
                //        string trny = e.FirstGroupValue;
                //        string number = e.SecondGroupValue;
                //        string level = e.ThirdGroupValue;
                //    }))
                ,new LineHandler(GetRegex("BLINDS"), 
                    new ProcessLineHandler(delegate(object sender, ProcessLineEventArgs e) {
                        e.Parser.CurrentHand.Blinds = e.FirstGroupValue;
                        }))
                ,new LineHandler(GetRegex("DEALING_DOWN_CARDS"), 
                    new ProcessLineHandler(delegate(object sender, ProcessLineEventArgs e) {
                        // TODO - cards being dealt
                    }))
                ,new LineHandler(GetRegex("DEALT_TO"), 
                    new ProcessLineHandler(delegate(object sender, ProcessLineEventArgs e) {
                        string user = e.FirstGroupValue;
                        string holeCards = e.SecondGroupValue;
                        string[] cards = holeCards.Split(' ');
                        string hc1 = cards[0];
                        string hc2 = cards[1];
                        e.Parser.SetHoleCards(user, hc1, hc2);
                        }))
                ,new LineHandler(GetRegex("SHOW_HAND"), 
                    new ProcessLineHandler(delegate(object sender, ProcessLineEventArgs e) {
                        string user = e.GetGroupValue(1);
                        string hc1 = e.GetGroupValue(2);
                        string hc2 = e.GetGroupValue(3);
                        string hand = e.GetGroupValue(4);
                        e.Parser.SetHoleCards(user, hc1, hc2);
                        e.Parser.SetFinalHand(user, hand);
                        }))
                ,new LineHandler(GetRegex("USER_FOLDS"), 
                    new ProcessLineHandler(delegate(object sender, ProcessLineEventArgs e) {
                        string user = e.FirstGroupValue;
                        }))
                ,new LineHandler(GetRegex("USER_RAISES"), 
                    new ProcessLineHandler(delegate(object sender, ProcessLineEventArgs e) {
                        string user = e.FirstGroupValue;
                        string raise = e.SecondGroupValue;
                        }))
                ,new LineHandler(GetRegex("USER_CALLS"), 
                    new ProcessLineHandler(delegate(object sender, ProcessLineEventArgs e) {
                        string user = e.FirstGroupValue;
                        string call = e.SecondGroupValue;
                        }))
                ,new LineHandler(GetRegex("DEALING_FLOP"), 
                    new ProcessLineHandler(delegate(object sender, ProcessLineEventArgs e) {
                        string flop = e.FirstGroupValue;
                        }))
                ,new LineHandler(GetRegex("USER_BETS"), 
                    new ProcessLineHandler(delegate(object sender, ProcessLineEventArgs e) {
                        string user = e.FirstGroupValue;
                        string bet = e.SecondGroupValue;
                        }))
                ,new LineHandler(GetRegex("DEALING_TURN"), 
                    new ProcessLineHandler(delegate(object sender, ProcessLineEventArgs e) {
                        string turn = e.FirstGroupValue;
                        }))
                ,new LineHandler(GetRegex("USER_CHECKS"), 
                    new ProcessLineHandler(delegate(object sender, ProcessLineEventArgs e) {
                        string user = e.FirstGroupValue;
                        }))
                ,new LineHandler(GetRegex("DEALING_RIVER"), 
                    new ProcessLineHandler(delegate(object sender, ProcessLineEventArgs e) {
                        string river = e.FirstGroupValue;
                        }))
                ,new LineHandler(GetRegex("USER_MUCKS_CARDS"), 
                    new ProcessLineHandler(delegate(object sender, ProcessLineEventArgs e) {
                        string user = e.FirstGroupValue;
                        }))
                ,new LineHandler(GetRegex("USER_WINS"), 
                    new ProcessLineHandler(delegate(object sender, ProcessLineEventArgs e) {
                        string user = e.FirstGroupValue;
                        string won = e.SecondGroupValue;
                        }))
                //,new LineHandler(CreateRegex("WILDCARD"), // this catch all regex should be the last line of defence
                //    new ProcessLineHandler(delegate(object sender, ProcessLineEventArgs e) { 
                //        // don't do anything with the line
                //        }))
            };

        public static LineHandler[] Handlers
        {
            get { return LineHandlers._handlers; }
        }
    }
}
