using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TradeFinder.Data;
using TradeFinder.Models;

namespace TradeFinder.ViewModels
{
    public class TeamIndexData
    {
        public League League { get; set; }
        public List<Team> Teams { get; set; }

        private TradeFinderContext db = new TradeFinderContext();

        public TeamIndexData(int leagudId)
        {
            League = db.Leagues.Find(leagudId);
            Teams = db.Teams.Where(t => t.LeagueId == leagudId).ToList();
        }
    }
}