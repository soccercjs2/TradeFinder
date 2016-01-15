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
        public List<Player> TradablePlayers { get; set; }
        public IEnumerable<IEnumerable<Player>> OnePlayerTradePool { get; set; }
        public IEnumerable<IEnumerable<Player>> TwoPlayerTradePool { get; set; }
        public IEnumerable<IEnumerable<Player>> ThreePlayerTradePool { get; set; }

        private TradeFinderContext db = new TradeFinderContext();

        public TeamPlayerPool(int teamId)
        {
            Team = db.Teams.Find(teamId);
            Players = db.Players.Where(p => p.LeagueId == Team.LeagueId && p.SessionId == Team.League.CurrentSessionId && p.TeamId == teamId).ToList();
            TradablePlayers = Players.Where(p => p.TradeValue > 0).ToList();
            OnePlayerTradePool = GetOnePlayerTradePool();
            TwoPlayerTradePool = GetTwoPlayerTradePool();
            ThreePlayerTradePool = GetThreePlayerTradePool();
        }

        public IEnumerable<Player> OptimalLineUp(LeaguePlayerPool leaguePlayerPool)
        {
            return OptimalLineUp(leaguePlayerPool, null, null);
        }

        public IEnumerable<Player> OptimalLineUp(LeaguePlayerPool leaguePlayerPool, IEnumerable<Player> gainedPlayers, IEnumerable<Player> lostPlayers)
        {
            List<Player> team = new List<Player>(Players);
            if (lostPlayers != null) { foreach (Player lostPlayer in lostPlayers) { team.Remove(lostPlayer); } }
            if (gainedPlayers != null) { foreach (Player gainedPlayer in gainedPlayers) { team.Add(gainedPlayer); } }

            //create list of starters to return
            List<Player> starters = new List<Player>();

            //get position players
            List<Player> quarterbacks = team.Where(p => p.Position == "QB").OrderByDescending(p => p.Points).ToList();
            List<Player> runningBacks = team.Where(p => p.Position == "RB").OrderByDescending(p => p.Points).ToList();
            List<Player> wideReceivers = team.Where(p => p.Position == "WR").OrderByDescending(p => p.Points).ToList();
            List<Player> tightEnds = team.Where(p => p.Position == "TE").OrderByDescending(p => p.Points).ToList();

            //get starting players
            List<Player> startingQbs = quarterbacks.Take(1).ToList();
            List<Player> startingRbs = runningBacks.Take(2).ToList();
            List<Player> startingWrs = wideReceivers.Take(2).ToList();
            List<Player> startingTes = tightEnds.Take(1).ToList();

            //add best waiver to team if missing starter
            if (startingQbs.Count() < 1) { startingQbs.Add(leaguePlayerPool.GetBestWaiverQb()); }
            if (startingRbs.Count() < 2) { startingRbs.Add(leaguePlayerPool.GetBestWaiverRb()); }
            if (startingWrs.Count() < 2) { startingWrs.Add(leaguePlayerPool.GetBestWaiverWr()); }
            if (startingTes.Count() < 1) { startingTes.Add(leaguePlayerPool.GetBestWaiverTe()); }

            //get possible waiver players
            Player flexRb = runningBacks.Skip(2).Take(1).FirstOrDefault();
            Player flexWr = wideReceivers.Skip(2).Take(1).FirstOrDefault();
            Player flexTe = tightEnds.Skip(1).Take(1).FirstOrDefault();

            //get waiver points for easy comparing
            decimal flexRbPoints = (flexRb != null) ? flexRb.Points : 0;
            decimal flexWrPoints = (flexWr != null) ? flexWr.Points : 0;
            decimal flexTePoints = (flexTe != null) ? flexTe.Points : 0;

            //add best waiver player to starters and remove him from the bench
            Player flexPlayer;
            if (flexRbPoints > flexWrPoints && flexRbPoints > flexTePoints) { flexPlayer = flexRb; }
            else if (flexWrPoints > flexRbPoints && flexWrPoints > flexTePoints) { flexPlayer = flexWr; }
            else { flexPlayer = flexTe; }

            foreach (Player player in startingQbs) { starters.Add(player); }
            foreach (Player player in startingRbs) { starters.Add(player); }
            foreach (Player player in startingWrs) { starters.Add(player); }
            foreach (Player player in startingTes) { starters.Add(player); }
            starters.Add(flexPlayer);

            //return optimal starting lineup
            return starters;
        }

        private IEnumerable<IEnumerable<Player>> GetOnePlayerTradePool()
        {
            return   from firstPlayer in TradablePlayers
                     select new List<Player>() { firstPlayer };
        }

        private IEnumerable<IEnumerable<Player>> GetTwoPlayerTradePool()
        {
            return from firstPlayer in TradablePlayers
                   from secondPlayer in TradablePlayers
                   where firstPlayer != secondPlayer
                   select new List<Player>() { firstPlayer, secondPlayer };
        }

        private IEnumerable<IEnumerable<Player>> GetThreePlayerTradePool()
        {
            return from firstPlayer in TradablePlayers
                   from secondPlayer in TradablePlayers
                   from thirdPlayer in TradablePlayers
                   where firstPlayer != secondPlayer && firstPlayer != thirdPlayer && secondPlayer != thirdPlayer
                   select new List<Player>() { firstPlayer, secondPlayer, thirdPlayer };
        }

        //public decimal CalculateDifferentials(List<Player> gainedPlayers, List<Player> lostPlayers, LeaguePlayerPool leaguePlayerPool)
        //{
        //    List<Player> originalTeam = new List<Player>(Players);
        //    List<Player> newTeam = new List<Player>(Players);
        //    foreach (Player lostPlayer in lostPlayers) { newTeam.Remove(lostPlayer); }
        //    foreach (Player gainedPlayer in gainedPlayers) { newTeam.Add(gainedPlayer); }

        //    //sort players and calculate starter/bench points
        //    decimal originalStarterPoints = SortPlayersAndCalculatePoints(originalTeam, leaguePlayerPool);
        //    decimal newStarterPoints = SortPlayersAndCalculatePoints(newTeam, leaguePlayerPool);

        //    //calculate original and new differentials
        //    return newStarterPoints - originalStarterPoints;
        //}

        //private List<Player> SortPlayersAndCalculatePoints(List<Player> team, LeaguePlayerPool leaguePlayerPool)
        //{
        //    //create list of starters to return
        //    List<Player> starters = new List<Player>();

        //    //get position players
        //    List<Player> quarterbacks = team.Where(p => p.Position == "QB").OrderByDescending(p => p.Points).ToList();
        //    List<Player> runningBacks = team.Where(p => p.Position == "RB").OrderByDescending(p => p.Points).ToList();
        //    List<Player> wideReceivers = team.Where(p => p.Position == "WR").OrderByDescending(p => p.Points).ToList();
        //    List<Player> tightEnds = team.Where(p => p.Position == "TE").OrderByDescending(p => p.Points).ToList();

        //    //get starting players
        //    List<Player> startingQbs = quarterbacks.Take(1).ToList();
        //    List<Player> startingRbs = runningBacks.Take(2).ToList();
        //    List<Player> startingWrs = wideReceivers.Take(2).ToList();
        //    List<Player> startingTes = tightEnds.Take(1).ToList();

        //    //add best waiver to team if missing starter
        //    if (startingQbs.Count() < 1) { startingQbs.Add(leaguePlayerPool.GetBestWaiverQb()); }
        //    if (startingRbs.Count() < 2) { startingRbs.Add(leaguePlayerPool.GetBestWaiverRb()); }
        //    if (startingWrs.Count() < 2) { startingWrs.Add(leaguePlayerPool.GetBestWaiverWr()); }
        //    if (startingTes.Count() < 1) { startingTes.Add(leaguePlayerPool.GetBestWaiverTe()); }

        //    //get possible waiver players
        //    Player flexRb = runningBacks.Skip(2).Take(1).FirstOrDefault();
        //    Player flexWr = wideReceivers.Skip(2).Take(1).FirstOrDefault();
        //    Player flexTe = tightEnds.Skip(1).Take(1).FirstOrDefault();

        //    //get waiver points for easy comparing
        //    decimal flexRbPoints = (flexRb != null) ? flexRb.Points : 0;
        //    decimal flexWrPoints = (flexWr != null) ? flexWr.Points : 0;
        //    decimal flexTePoints = (flexTe != null) ? flexTe.Points : 0;

        //    //add best waiver player to starters and remove him from the bench
        //    Player flexPlayer;
        //    if (flexRbPoints > flexWrPoints && flexRbPoints > flexTePoints) { flexPlayer = flexRb; }
        //    else if (flexWrPoints > flexRbPoints && flexWrPoints > flexTePoints) { flexPlayer = flexWr; }
        //    else { flexPlayer = flexTe; }

        //    foreach (Player player in startingQbs) { starters.Add(player); }
        //    foreach (Player player in startingRbs) { starters.Add(player); }
        //    foreach (Player player in startingWrs) { starters.Add(player); }
        //    foreach (Player player in startingTes) { starters.Add(player); }
        //    starters.Add(flexPlayer);

        //    //set starter and bench totals
        //    return starters;
        //}
    }
}