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
using TradeFinder.PlayerPool;
using TradeFinder.ViewModels;
//using TradeFinder.ViewModels;


namespace TradeFinder.Controllers
{
    public class TradesController : Controller
    {
        private TradeFinderContext db = new TradeFinderContext();

        // GET: Trades
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            League league = db.Leagues.Find(id);

            //create select lists with teams
            Team myTeam = league.Teams.Where(t => t.MyTeam).FirstOrDefault();
            SelectList myTeamOptions = new SelectList(league.Teams, "TeamId", "Name");
            SelectList otherTeamOptions = new SelectList(league.Teams, "TeamId", "Name");

            ViewBag.MyTeamOptions = myTeamOptions;
            ViewBag.OtherTeamOptions = otherTeamOptions;

            TradeFilterView tradeListView = new TradeFilterView(league.LeagueId);
            tradeListView.LeagueId = league.LeagueId;
            tradeListView.MyTeamId = myTeam.TeamId;

            return View(tradeListView);
        }

        [HttpPost]
        public ActionResult Index(TradeFilterView tradeListView)
        {
            //create select lists with teams
            League league = db.Leagues.Find(tradeListView.LeagueId);
            Team myTeam = league.Teams.Where(t => t.MyTeam).FirstOrDefault();
            SelectList myTeamOptions = new SelectList(league.Teams, "TeamId", "Name", myTeam.TeamId);
            SelectList otherTeamOptions = new SelectList(league.Teams, "TeamId", "Name");

            ViewBag.MyTeamOptions = myTeamOptions;
            ViewBag.OtherTeamOptions = otherTeamOptions;

            return View(tradeListView);
        }

        [HttpPost]
        public JsonResult HasCurrentSessionPlayers(int id)
        {
            string sessionId = HttpContext.Session.SessionID;
            League league = db.Leagues.Find(id);

            return Json(new { HasCurrentSessionPlayers = (sessionId == league.CurrentSessionId) });
        }

        [HttpPost]
        public void ImportQuarterbacks(int id)
        {
            string sessionId = HttpContext.Session.SessionID;
            NumberFireConnection numberFireConnection = new NumberFireConnection(id, sessionId);
            numberFireConnection.ImportQbHtml();
        }

        [HttpPost]
        public void ImportRunningBacks(int id)
        {
            string sessionId = HttpContext.Session.SessionID;
            NumberFireConnection numberFireConnection = new NumberFireConnection(id, sessionId);
            numberFireConnection.ImportRbHtml();
        }

        [HttpPost]
        public void ImportWideReceivers(int id)
        {
            string sessionId = HttpContext.Session.SessionID;
            NumberFireConnection numberFireConnection = new NumberFireConnection(id, sessionId);
            numberFireConnection.ImportWrHtml();
        }

        [HttpPost]
        public void ImportTightEnds(int id)
        {
            string sessionId = HttpContext.Session.SessionID;
            NumberFireConnection numberFireConnection = new NumberFireConnection(id, sessionId);
            numberFireConnection.ImportTeHtml();
        }

        [HttpPost]
        public void SetTeamsPlayers(int id)
        {
            LeaguePlayerPool leaguePlayerPool = new LeaguePlayerPool(id);
            leaguePlayerPool.SetTeamsPlayers();
        }

        [HttpPost]
        public void CalculateTradeValues(int id)
        {
            LeaguePlayerPool leaguePlayerPool = new LeaguePlayerPool(id);
            leaguePlayerPool.CalculateTradeValues();
        }

        [HttpPost]
        public JsonResult GetOtherTeamsIds(int myTeamId, int otherTeamId)
        {
            //find my team and create variable to fill with other teams
            Team myTeam = db.Teams.Find(myTeamId);
            List<Team> otherTeams = new List<Team>();

            //if they selected team to trade with, add that team to list
            if (otherTeamId != -1)
            {
                otherTeams.Add(db.Teams.Find(otherTeamId));
            }
            //else add all other teams that aren't my team to the list
            else
            {
                League league = db.Leagues.Find(myTeam.LeagueId);
                otherTeams = league.Teams.ToList();
                otherTeams.Remove(myTeam);
            }

            //put other team ids into array for ajax
            int[] otherTeamIds = new int[otherTeams.Count];
            for (int i = 0; i < otherTeamIds.Length; i++)
            {
                otherTeamIds[i] = otherTeams[i].TeamId;
            }

            return Json(new { OtherTeamIds = otherTeamIds });
        }

        [HttpPost]
        public JsonResult GetTeamName(int teamId)
        {
            //find my team and create variable to fill with other teams
            Team myTeam = db.Teams.Find(teamId);

            return Json(new { TeamName = myTeam.Name });
        }

        [HttpPost]
        public JsonResult FindOneForOneTrades(int leagueId, int myTeamId, int otherTeamId)
        {
            //create player pools to find trades
            LeaguePlayerPool leaguePlayerPool = new LeaguePlayerPool(leagueId);
            TeamPlayerPool myTeamPlayerPool = new TeamPlayerPool(myTeamId);
            TeamPlayerPool theirTeamPlayerPool = new TeamPlayerPool(otherTeamId);

            //find trades
            List<TradeView> trades = leaguePlayerPool.FindTrades(myTeamPlayerPool, theirTeamPlayerPool, myTeamPlayerPool.OnePlayerTradePool, theirTeamPlayerPool.OnePlayerTradePool);

            //return Json object with found trades
            return Json(new { Trades = trades });
        }

        [HttpPost]
        public JsonResult FindOneForTwoTrades(int leagueId, int myTeamId, int otherTeamId)
        {
            //create player pools to find trades
            LeaguePlayerPool leaguePlayerPool = new LeaguePlayerPool(leagueId);
            TeamPlayerPool myTeamPlayerPool = new TeamPlayerPool(myTeamId);
            TeamPlayerPool theirTeamPlayerPool = new TeamPlayerPool(otherTeamId);

            //find trades
            List<TradeView> trades = leaguePlayerPool.FindTrades(myTeamPlayerPool, theirTeamPlayerPool, myTeamPlayerPool.OnePlayerTradePool, theirTeamPlayerPool.TwoPlayerTradePool);

            //return Json object with found trades
            return Json(new { Trades = trades });
        }

        [HttpPost]
        public JsonResult FindOneForThreeTrades(int leagueId, int myTeamId, int otherTeamId)
        {
            //create player pools to find trades
            LeaguePlayerPool leaguePlayerPool = new LeaguePlayerPool(leagueId);
            TeamPlayerPool myTeamPlayerPool = new TeamPlayerPool(myTeamId);
            TeamPlayerPool theirTeamPlayerPool = new TeamPlayerPool(otherTeamId);

            //find trades
            List<TradeView> trades = leaguePlayerPool.FindTrades(myTeamPlayerPool, theirTeamPlayerPool, myTeamPlayerPool.OnePlayerTradePool, theirTeamPlayerPool.ThreePlayerTradePool);

            //return Json object with found trades
            return Json(new { Trades = trades });
        }

        [HttpPost]
        public JsonResult FindTwoForOneTrades(int leagueId, int myTeamId, int otherTeamId)
        {
            //create player pools to find trades
            LeaguePlayerPool leaguePlayerPool = new LeaguePlayerPool(leagueId);
            TeamPlayerPool myTeamPlayerPool = new TeamPlayerPool(myTeamId);
            TeamPlayerPool theirTeamPlayerPool = new TeamPlayerPool(otherTeamId);

            //find trades
            List<TradeView> trades = leaguePlayerPool.FindTrades(myTeamPlayerPool, theirTeamPlayerPool, myTeamPlayerPool.TwoPlayerTradePool, theirTeamPlayerPool.OnePlayerTradePool);

            //return Json object with found trades
            return Json(new { Trades = trades });
        }

        [HttpPost]
        public JsonResult FindTwoForTwoTrades(int leagueId, int myTeamId, int otherTeamId)
        {
            //create player pools to find trades
            LeaguePlayerPool leaguePlayerPool = new LeaguePlayerPool(leagueId);
            TeamPlayerPool myTeamPlayerPool = new TeamPlayerPool(myTeamId);
            TeamPlayerPool theirTeamPlayerPool = new TeamPlayerPool(otherTeamId);

            //find trades
            List<TradeView> trades = leaguePlayerPool.FindTrades(myTeamPlayerPool, theirTeamPlayerPool, myTeamPlayerPool.TwoPlayerTradePool, theirTeamPlayerPool.TwoPlayerTradePool);

            //return Json object with found trades
            return Json(new { Trades = trades });
        }

        [HttpPost]
        public JsonResult FindTwoForThreeTrades(int leagueId, int myTeamId, int otherTeamId)
        {
            //create player pools to find trades
            LeaguePlayerPool leaguePlayerPool = new LeaguePlayerPool(leagueId);
            TeamPlayerPool myTeamPlayerPool = new TeamPlayerPool(myTeamId);
            TeamPlayerPool theirTeamPlayerPool = new TeamPlayerPool(otherTeamId);

            //find trades
            List<TradeView> trades = leaguePlayerPool.FindTrades(myTeamPlayerPool, theirTeamPlayerPool, myTeamPlayerPool.TwoPlayerTradePool, theirTeamPlayerPool.ThreePlayerTradePool);

            //return Json object with found trades
            return Json(new { Trades = trades });
        }

        [HttpPost]
        public JsonResult FindThreeForOneTrades(int leagueId, int myTeamId, int otherTeamId)
        {
            //create player pools to find trades
            LeaguePlayerPool leaguePlayerPool = new LeaguePlayerPool(leagueId);
            TeamPlayerPool myTeamPlayerPool = new TeamPlayerPool(myTeamId);
            TeamPlayerPool theirTeamPlayerPool = new TeamPlayerPool(otherTeamId);

            //find trades
            List<TradeView> trades = leaguePlayerPool.FindTrades(myTeamPlayerPool, theirTeamPlayerPool, myTeamPlayerPool.ThreePlayerTradePool, theirTeamPlayerPool.OnePlayerTradePool);

            //return Json object with found trades
            return Json(new { Trades = trades });
        }

        [HttpPost]
        public JsonResult FindThreeForTwoTrades(int leagueId, int myTeamId, int otherTeamId)
        {
            //create player pools to find trades
            LeaguePlayerPool leaguePlayerPool = new LeaguePlayerPool(leagueId);
            TeamPlayerPool myTeamPlayerPool = new TeamPlayerPool(myTeamId);
            TeamPlayerPool theirTeamPlayerPool = new TeamPlayerPool(otherTeamId);

            //find trades
            List<TradeView> trades = leaguePlayerPool.FindTrades(myTeamPlayerPool, theirTeamPlayerPool, myTeamPlayerPool.ThreePlayerTradePool, theirTeamPlayerPool.TwoPlayerTradePool);

            //return Json object with found trades
            return Json(new { Trades = trades });
        }

        [HttpPost]
        public JsonResult FindThreeForThreeTrades(int leagueId, int myTeamId, int otherTeamId)
        {
            //create player pools to find trades
            LeaguePlayerPool leaguePlayerPool = new LeaguePlayerPool(leagueId);
            TeamPlayerPool myTeamPlayerPool = new TeamPlayerPool(myTeamId);
            TeamPlayerPool theirTeamPlayerPool = new TeamPlayerPool(otherTeamId);

            //find trades
            List<TradeView> trades = leaguePlayerPool.FindTrades(myTeamPlayerPool, theirTeamPlayerPool, myTeamPlayerPool.ThreePlayerTradePool, theirTeamPlayerPool.ThreePlayerTradePool);

            //return Json object with found trades
            return Json(new { Trades = trades });
        }
    }
}