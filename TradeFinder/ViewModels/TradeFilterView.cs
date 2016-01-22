using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeFinder.Data;
using TradeFinder.Models;
using TradeFinder.NumberFire;
using TradeFinder.PlayerPool;

namespace TradeFinder.ViewModels
{
    public class TradeFilterView
    {
        public int MyTeamId { get; set; }
        public int? OtherTeamId { get; set; }
        public int LeagueId { get; set; }
        public List<Trade> Trades { get; set; }
        public bool HasCurrentSessionPlayers { get; set; }

        private TradeFinderContext db = new TradeFinderContext();

        public TradeFilterView() { }

        public TradeFilterView(int leagueId)
        {
            LeagueId = leagueId;
            League league = db.Leagues.Find(leagueId);

            //if players haven't been loaded for the current session, import players into database
            string sessionId = HttpContext.Current.Session.SessionID;

            //determine whether numberfire players have been loaded this session
            if (league.CurrentSessionId != sessionId) { HasCurrentSessionPlayers = false; } else { HasCurrentSessionPlayers = true; }
        }

        //public void FindTrades()
        //{
        //    League league = db.Leagues.Find(LeagueId);
            
        //    //if players haven't been loaded for the current session, import players into database
        //    string sessionId = HttpContext.Current.Session.SessionID;
        //    if (league.CurrentSessionId != sessionId)
        //    {
        //        NumberFireConnection numberFireConnection = new NumberFireConnection(LeagueId, sessionId);
        //        numberFireConnection.ImportPlayers();
        //    }

        //    LeaguePlayerPool leaguePlayerPool = new LeaguePlayerPool(LeagueId);
        //    Trades = leaguePlayerPool.FindAllTrades(MyTeamId, OtherTeamId);
        //}
    }
}