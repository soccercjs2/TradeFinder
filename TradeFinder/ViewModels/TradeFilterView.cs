using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeFinder.Models;
using TradeFinder.PlayerPool;

namespace TradeFinder.ViewModels
{
    public class TradeFilterView
    {
        public int MyTeamId { get; set; }
        public int? OtherTeamId { get; set; }
        public int LeagueId { get; set; }
        public List<Trade> Trades { get; set; }

        public TradeFilterView() { }

        public void FindTrades()
        {
            LeaguePlayerPool leaguePlayerPool = new LeaguePlayerPool(LeagueId);
            Trades = leaguePlayerPool.FindAllTrades(MyTeamId, OtherTeamId);
        }
    }
}