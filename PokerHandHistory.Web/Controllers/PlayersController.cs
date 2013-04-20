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

using System.Collections.Generic;
using System.Web.Mvc;
using JeromeTerry.PokerHandHistory.Core;

namespace JeromeTerry.PokerHandHistory.Web.Controllers
{
    public class PlayersController : Controller
    {
        //
        // GET: /Players/

        public ActionResult Index()
        {
            List<Models.Player> players = new List<Models.Player>();

            Data.SQLite.DataStore ds = Data.SQLite.DataStore.Instance;
            using (ds.Lock())
            {
                Data.Setting setting = ds.GetSetting("Version");
                if (setting != null)
                {
                    string version = setting.Value;
                    Logger.Info("Version: {0}", version);
                }

                List<Data.Player> results = ds.GetPlayers();
                foreach (Data.Player p in results)
                {
                    Models.Player player = new Models.Player();
                    player.Id = p.Id;
                    player.SiteId = p.SiteId;
                    player.Name = p.Name;
                    player.Notes = p.Notes;
                    player.HandPlayerCount = ds.GetHandPlayerCount(p.Id);

                    player.DateAdded = p.DateAdded.UtcTime.ToLocalTime().ToString();
                    if (p.DateUpdated != null)
                    {
                        player.DateUpdated = p.DateUpdated.UtcTime.ToLocalTime().ToString();
                    }
                    players.Add(player);
                }
            }
                
            return View(players);
        }

    }
}
