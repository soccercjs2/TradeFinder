using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using TradeFinder.Data;
using TradeFinder.Models;

namespace TradeFinder.PlayerPool
{
    public class TeamPlayerPool
    {
        public Team Team { get; set; }
        public List<Player> Players { get; set; }

        private TradeFinderContext db = new TradeFinderContext();

        public TeamPlayerPool(int teamId)
        {
            Team = db.Teams.Find(teamId);
            Players = db.Players.Where(p => p.LeagueId == Team.LeagueId && p.SessionId == Team.League.CurrentSessionId && p.TeamId == teamId).ToList();
        }

        public void CalculateDifferentials(Player gainedPlayer, Player lostPlayer, ref decimal starterDifferential, ref decimal benchDifferential)
        {
            decimal originalStarterPoints = 0, originalBenchPoints = 0, newStarterPoints = 0, newBenchPoints = 0;
            List<Player> originalTeam = Players;
            List<Player> newTeam = Players;
            newTeam.Remove(lostPlayer);
            newTeam.Add(gainedPlayer);

            SortPlayersAndCalculatePoints(originalTeam, ref originalStarterPoints, ref originalBenchPoints);
            SortPlayersAndCalculatePoints(newTeam, ref newStarterPoints, ref newBenchPoints);

            starterDifferential = newStarterPoints - originalStarterPoints;
            benchDifferential = newBenchPoints - originalBenchPoints;
        }

        public decimal SortPlayersAndCalculatePoints(List<Player> team, ref decimal starterPoints, ref decimal benchPoints)
        {
            List<Player> starters = new List<Player>();
            List<Player> benchPlayers = team;



            return 0;
        }
    }
}