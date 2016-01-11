using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TradeFinder.Data;
using TradeFinder.Models;
using TradeFinder.NumberFire;

namespace TradeFinder.ViewModels
{
    public class LeagueView
    {
        public League League { get; set; }
        public LeagueHost LeagueHost { get; set; }
        public List<Team> Teams { get; set; }

        private TradeFinderContext db = new TradeFinderContext();

        public LeagueView(int leagudId)
        {
            League = db.Leagues.Find(leagudId);
            LeagueHost = db.LeagueHosts.Find(League.LeagueHostId);
            Teams = db.Teams.Where(t => t.LeagueId == leagudId).ToList();
        }
    }
}