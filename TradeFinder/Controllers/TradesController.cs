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
            string sessionId = HttpContext.Session.SessionID;

            if (league.CurrentSessionId != sessionId)
            {
                NumberFireConnection numberFireConnection = new NumberFireConnection(league.LeagueId, sessionId);
                numberFireConnection.ImportPlayers();
            }

            //create select lists with teams
            Team myTeam = league.Teams.Where(t => t.MyTeam).FirstOrDefault();
            SelectList myTeamOptions = new SelectList(league.Teams, "TeamId", "Name");
            SelectList otherTeamOptions = new SelectList(league.Teams, "TeamId", "Name");

            ViewBag.MyTeamOptions = myTeamOptions;
            ViewBag.OtherTeamOptions = otherTeamOptions;

            TradeFilterView tradeListView = new TradeFilterView();
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

            if (ModelState.IsValid)
            {
                tradeListView.FindTrades();

                return View(tradeListView);
            }
            else
            {
                return View(tradeListView);
            }
        }

        public string MakeNumberFirePlayer(string player, string position, string team)
        {
            return string.Format("{0} ({1}, {2})", player, position, team);
        }

        private void CleanQBTable(ref DataTable table)
        {
            table.Columns.Remove("OverallRank");
            table.Columns.Remove("PositionRank");
            table.Columns.Remove("Completions/Attempts");
            table.Columns.Remove("PassingYards");
            table.Columns.Remove("PassingTouchdowns");
            table.Columns.Remove("Interceptions");
            table.Columns.Remove("RushingAttempts");
            table.Columns.Remove("RushingYards");
            table.Columns.Remove("RushingTouchdowns");
            table.Columns.Remove("ConfidenceInterval");
        }

        private void CleanRBWRTable(ref DataTable table)
        {
            table.Columns.Remove("OverallRank");
            table.Columns.Remove("PositionRank");
            table.Columns.Remove("RushingAttempts");
            table.Columns.Remove("RushingYards");
            table.Columns.Remove("RushingTouchdowns");
            table.Columns.Remove("Receptions");
            table.Columns.Remove("ReceivingYards");
            table.Columns.Remove("ReceivingTouchdowns");
            table.Columns.Remove("ConfidenceInterval");
        }

        private void CleanTETable(ref DataTable table)
        {
            table.Columns.Remove("OverallRank");
            table.Columns.Remove("PositionRank");
            table.Columns.Remove("Receptions");
            table.Columns.Remove("ReceivingYards");
            table.Columns.Remove("ReceivingTouchdowns");
            table.Columns.Remove("ConfidenceInterval");
        }

        private void AssignPlayersToTeams(int? leagueId, ref DataTable quarterbacks, ref DataTable runningBacks, ref DataTable wideReceivers, ref DataTable tightEnds, ref int myTeamId)
        {
            //get league to get teams
            League league = db.Leagues.Find(leagueId);
            List<TeamView> teamViews = new List<TeamView>();

            //for each team, create TeamView
            foreach (Team team in league.Teams)
            {
                teamViews.Add(new TeamView(team.TeamId, quarterbacks, runningBacks, wideReceivers, tightEnds));

                //if the team is my team, record TeamId
                if (team.MyTeam) { myTeamId = team.TeamId; }
            }

            //add team column to position tables
            quarterbacks.Columns.Add("Team", typeof(int));
            runningBacks.Columns.Add("Team", typeof(int));
            wideReceivers.Columns.Add("Team", typeof(int));
            tightEnds.Columns.Add("Team", typeof(int));

            //add team column to position tables
            quarterbacks.Columns.Add("TradeValue", typeof(decimal));
            runningBacks.Columns.Add("TradeValue", typeof(decimal));
            wideReceivers.Columns.Add("TradeValue", typeof(decimal));
            tightEnds.Columns.Add("TradeValue", typeof(decimal));

            //assign players to teams
            foreach (TeamView teamView in teamViews)
            {
                foreach (DataRow teamRow in teamView.TeamTable.Rows)
                {
                    //create player with numberfire format
                    string numberfirePlayer = MakeNumberFirePlayer(teamRow["Player"].ToString(), teamRow["Position"].ToString(), teamRow["Team"].ToString());
                    string query = "Player = '" + numberfirePlayer + "'";

                    //try and match player to player in position pool
                    DataRow qbRow = quarterbacks.Select(query).FirstOrDefault();
                    DataRow rbRow = runningBacks.Select(query).FirstOrDefault();
                    DataRow wrRow = wideReceivers.Select(query).FirstOrDefault();
                    DataRow teRow = tightEnds.Select(query).FirstOrDefault();

                    //if match is found, assign team id
                    if (qbRow != null) { qbRow["Team"] = teamView.Team.TeamId; qbRow.AcceptChanges(); }
                    if (rbRow != null) { rbRow["Team"] = teamView.Team.TeamId; rbRow.AcceptChanges(); }
                    if (wrRow != null) { wrRow["Team"] = teamView.Team.TeamId; wrRow.AcceptChanges(); }
                    if (teRow != null) { teRow["Team"] = teamView.Team.TeamId; teRow.AcceptChanges(); }
                }
            }
        }

        private void CalculateTradeValuesAndSort(DataTable positionTable, DataRow waiverRow, int myTeamId, ref DataTable myPlayers, ref DataTable players)
        {
            //calculate trade values for quarterbacks
            foreach (DataRow row in positionTable.Rows)
            {
                row["TradeValue"] = (decimal)row["FantasyPoints"] - (decimal)waiverRow["FantasyPoints"];
                row.AcceptChanges();

                //if quarterback is on a team and has a positive trade value, add quarterback to player table
                if (row["Team"] != DBNull.Value && (decimal)row["TradeValue"] >= 0)
                {
                    //if the player is on my team, add it to my team
                    if ((int)row["Team"] == myTeamId)
                    {
                        myPlayers.ImportRow(row);
                        myPlayers.AcceptChanges();
                    }
                    //if the player is not on my team, add to player pool
                    else
                    {
                        players.ImportRow(row);
                        players.AcceptChanges();
                    }
                }
            }
        }

        private void AddColumnsToPlayerTable(ref DataTable table)
        {
            //create identity column
            DataColumn playerId = new DataColumn();
            playerId.ColumnName = "PlayerId";
            playerId.DataType = typeof(int);
            playerId.AutoIncrement = true;
            playerId.AutoIncrementSeed = 1;
            playerId.AutoIncrementStep = 1;

            //add column to table
            table.Columns.Add(playerId);

            //add other relevant columns
            table.Columns.Add("Player", typeof(string));
            table.Columns.Add("FantasyPoints", typeof(decimal));
            table.Columns.Add("Team", typeof(int));
            table.Columns.Add("TradeValue", typeof(decimal));
        }

        private DataTable FindTrades(DataTable myPlayers, DataTable theirPlayers)
        {
            DataTable trades = new DataTable();
            DataTable oneForOneTrades = OneForOneTrades(myPlayers, theirPlayers);

            return trades;
        }

        private DataTable OneForOneTrades(DataTable myPlayers, DataTable theirPlayers)
        {
            DataTable trades = new DataTable();
            trades.Columns.Add("MyTeamName", typeof(string));
            trades.Columns.Add("TheirTeamName", typeof(string));
            trades.Columns.Add("MyPlayers", typeof(string));
            trades.Columns.Add("TheirPlayers", typeof(string));
            trades.Columns.Add("Fairness", typeof(decimal));
            trades.Columns.Add("MyFantasyPointsDiff", typeof(decimal));
            trades.Columns.Add("TheirFantasyPointsDiff", typeof(decimal));

            foreach (DataRow myPlayer in myPlayers.Rows)
            {
                //get my team for teamname
                int myTeamId = (int)myPlayer["Team"];
                TeamView myTeam = new TeamView(myTeamId, (DataTable)Session["Quarterbacks"], (DataTable)Session["RunningBacks"], (DataTable)Session["WideReceivers"], (DataTable)Session["TightEnds"]);

                //get my team details
                string myTeamName = myTeam.Team.Name;
                string myPlayersList = myPlayer["Player"].ToString();
                decimal myTradeValue = (decimal)myPlayer["TradeValue"];

                //get their tradable characters
                DataRow[] theirTradablePlayers = theirPlayers.Select("TradeValue <=" + (myTradeValue + 5) + " And TradeValue >= " + (myTradeValue - 5), "TradeValue DESC");

                //loop through each tradable character
                foreach (DataRow theirPlayer in theirTradablePlayers)
                {
                    //create datarow lists for point calculations
                    List<DataRow> myPlayersRows = new List<DataRow>();
                    List<DataRow> theirPlayersRows = new List<DataRow>();

                    //add players to rows lists
                    myPlayersRows.Add(myPlayer);
                    theirPlayersRows.Add(theirPlayer);

                    //get their team for teamname
                    int theirTeamId = (int)theirPlayer["Team"];
                    TeamView theirTeam = new TeamView(theirTeamId, (DataTable)Session["Quarterbacks"], (DataTable)Session["RunningBacks"], (DataTable)Session["WideReceivers"], (DataTable)Session["TightEnds"]);

                    //create trade row
                    DataRow trade = trades.NewRow();
                    trade["MyTeamName"] = myTeamName;
                    trade["MyPlayers"] = myPlayersList;
                    trade["TheirTeamName"] = theirTeam.Team.Name;
                    trade["TheirPlayers"] = theirPlayer["Player"];
                    trade["Fairness"] = myTradeValue - (decimal)theirPlayer["TradeValue"];
                    trade["MyFantasyPointsDiff"] = myTeam.OptimalLineUpPoints(theirPlayersRows, myPlayersRows);
                    trade["TheirFantasyPointsDiff"] = theirTeam.OptimalLineUpPoints(myPlayersRows, theirPlayersRows); ;

                    trades.Rows.Add(trade);
                }

            }

            return trades;
        }
    }
}