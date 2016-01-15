using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using TradeFinder.Data;
using TradeFinder.Models;

namespace TradeFinder.PlayerPool
{
    public class LeaguePlayerPool
    {
        public League League { get; set; }
        public List<Player> Players { get; set; }

        private TradeFinderContext db = new TradeFinderContext();

        public LeaguePlayerPool(int leagueId)
        {
            League = db.Leagues.Find(leagueId);
            Players = db.Players.Where(p => p.LeagueId == League.LeagueId && p.SessionId == League.CurrentSessionId).ToList();
        }

        public void SetTeamsPlayers()
        {
            foreach (Team team in League.Teams)
            {
                SetTeamPlayers(team);
            }
        }

        private void SetTeamPlayers(Team team)
        {
            HttpWebRequest webRequest;
            StreamReader responseReader;
            string responseData;
            CookieContainer cookies = new CookieContainer();
            StreamWriter requestWriter;

            string postData = string.Format(null, team.League.UserName, team.League.Password);

            try
            {
                //get login  page with cookies
                webRequest = (HttpWebRequest)WebRequest.Create(team.League.LeagueHost.LoginUrl);
                webRequest.CookieContainer = cookies;

                //recieve non-authenticated cookie
                webRequest.GetResponse().Close();

                //post form  data to page
                webRequest = (HttpWebRequest)WebRequest.Create(team.League.LeagueHost.LoginUrl);
                webRequest.Method = WebRequestMethods.Http.Post;
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.CookieContainer = cookies;
                webRequest.ContentLength = postData.Length;

                requestWriter = new StreamWriter(webRequest.GetRequestStream());
                requestWriter.Write(postData);
                requestWriter.Close();

                //recieve authenticated cookie
                webRequest.GetResponse().Close();

                //now we get the authenticated page
                webRequest = (HttpWebRequest)WebRequest.Create(team.Url);
                webRequest.CookieContainer = cookies;
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
                responseReader.Close();

                //load html into htmlagilitypack
                HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
                document.LoadHtml(responseData);

                HtmlNode table = document.GetElementbyId(League.LeagueHost.StarterTableName);
                foreach (Player player in Players)
                {
                    if (table.InnerHtml.ToLower().Contains(player.Name.ToLower()) && table.InnerHtml.ToLower().Contains(player.Position.ToLower()) &&
                            (table.InnerHtml.ToLower().Contains(player.NflTeam.ToLower()) || table.InnerHtml.ToLower().Contains(player.NflAlternateTeam.ToLower())))
                    {
                        try
                        {
                            player.TeamId = team.TeamId;
                            db.SaveChanges();
                        }
                        catch (Exception e) { }
                    }
                }
            }
            catch { }
        }

        public Player GetBestWaiverQb()
        {
            return GetBestWaiver("QB");
        }

        public Player GetBestWaiverRb()
        {
            return GetBestWaiver("RB");
        }

        public Player GetBestWaiverWr()
        {
            return GetBestWaiver("WR");
        }

        public Player GetBestWaiverTe()
        {
            return GetBestWaiver("TE");
        }

        public Player GetBestWaiverFlex()
        {
            Player bestWaiverRb = GetBestWaiverRb();
            Player bestWaiverWr = GetBestWaiverWr();

            if (bestWaiverRb.Points > bestWaiverWr.Points) { return bestWaiverRb; }
            else { return bestWaiverWr; }
        }

        private Player GetBestWaiver(string position)
        {
            return (from player in Players
                    where player.TeamId == null && player.Position == position
                    orderby player.Points descending
                    select player).FirstOrDefault();
        }

        public void CalculateTradeValues()
        {
            //calculate trade values
            Player waiverQb = GetBestWaiverQb();
            Player waiverRb = GetBestWaiverRb();
            Player waiverWr = GetBestWaiverWr();
            Player waiverTe = GetBestWaiverTe();

            //loop through players and calculate trade vlaues
            foreach (Player player in Players)
            {
                switch (player.Position)
                {
                    case "QB":
                        player.TradeValue = player.Points - waiverQb.Points;
                        break;
                    case "RB":
                        player.TradeValue = player.Points - waiverRb.Points;
                        break;
                    case "WR":
                        player.TradeValue = player.Points - waiverWr.Points;
                        break;
                    case "TE":
                        player.TradeValue = player.Points - waiverTe.Points;
                        break;
                }

                db.SaveChanges();
            }
        }

        public List<Trade> FindAllTrades(int myTeamId, int? otherTeamId)
        {
            //create table list to store each trade 
            List<Trade> trades = new List<Trade>();

            //get my team and create team player pool
            TeamPlayerPool myTeamPlayerPool = new TeamPlayerPool(myTeamId);

            //create other team list
            List<Team> otherTeams = new List<Team>();
            if (otherTeamId != null && otherTeamId > 0)
            {
                otherTeams.Add(League.Teams.Where(t => t.TeamId == otherTeamId).FirstOrDefault());
            }
            else
            {
                otherTeams = League.Teams.ToList();
                otherTeams.Remove(League.Teams.Where(t => t.TeamId == otherTeamId).FirstOrDefault());
            }

            //for each other team, find trades
            foreach (Team otherTeam in otherTeams)
            {
                //load team player pool for this team
                TeamPlayerPool otherTeamPlayerPool = new TeamPlayerPool(otherTeam.TeamId);

                //find trades with this team
                FindTrades(ref trades, myTeamPlayerPool, otherTeamPlayerPool, myTeamPlayerPool.OnePlayerTradePool, otherTeamPlayerPool.OnePlayerTradePool); //1 for 1
                FindTrades(ref trades, myTeamPlayerPool, otherTeamPlayerPool, myTeamPlayerPool.OnePlayerTradePool, otherTeamPlayerPool.TwoPlayerTradePool); //1 for 2
                FindTrades(ref trades, myTeamPlayerPool, otherTeamPlayerPool, myTeamPlayerPool.OnePlayerTradePool, otherTeamPlayerPool.ThreePlayerTradePool); //1 for 3
                FindTrades(ref trades, myTeamPlayerPool, otherTeamPlayerPool, myTeamPlayerPool.TwoPlayerTradePool, otherTeamPlayerPool.OnePlayerTradePool); //2 for 1
                FindTrades(ref trades, myTeamPlayerPool, otherTeamPlayerPool, myTeamPlayerPool.TwoPlayerTradePool, otherTeamPlayerPool.TwoPlayerTradePool); //2 for 2
                FindTrades(ref trades, myTeamPlayerPool, otherTeamPlayerPool, myTeamPlayerPool.TwoPlayerTradePool, otherTeamPlayerPool.ThreePlayerTradePool); //2 for 3
                FindTrades(ref trades, myTeamPlayerPool, otherTeamPlayerPool, myTeamPlayerPool.ThreePlayerTradePool, otherTeamPlayerPool.OnePlayerTradePool); //3 for 1
                FindTrades(ref trades, myTeamPlayerPool, otherTeamPlayerPool, myTeamPlayerPool.ThreePlayerTradePool, otherTeamPlayerPool.TwoPlayerTradePool); //3 for 2
                FindTrades(ref trades, myTeamPlayerPool, otherTeamPlayerPool, myTeamPlayerPool.ThreePlayerTradePool, otherTeamPlayerPool.ThreePlayerTradePool); //3 for 3
            }

            return trades.OrderByDescending(t => t.CompositeDifferential).Distinct().ToList();
        }

        //private void FindOneForOneTrades(ref List<Trade> allTrades, TeamPlayerPool myTeamPlayerPool, TeamPlayerPool theirTeamPlayerPool)
        //{
        //    //get team trade pools
        //    IEnumerable<IEnumerable<Player>> myTradePool = myTeamPlayerPool.OnePlayerTradePool();
        //    IEnumerable<IEnumerable<Player>> theirTradePool = theirTeamPlayerPool.OnePlayerTradePool();

        //    //find trades with trade pools
        //    FindTrades(ref allTrades, myTradePool, theirTradePool, myTeamPlayerPool, theirTeamPlayerPool);
        //}

        private void FindTrades(ref List<Trade> allTrades,
                                TeamPlayerPool myTeamPlayerPool, TeamPlayerPool theirTeamPlayerPool,
                                IEnumerable<IEnumerable<Player>> myTradePool, IEnumerable<IEnumerable<Player>> theirTradePool)
        {
            IEnumerable<Trade> trades = from mySideOfTrade in myTradePool
                                        from theirSideOfTrade in theirTradePool
                                        select (new Trade(mySideOfTrade, theirSideOfTrade));

            foreach (Trade trade in trades)
            {
                if (Math.Abs(trade.Fairness) <= 5)
                {
                    trade.CalculateDifferentials(this, myTeamPlayerPool, theirTeamPlayerPool);
                    if (trade.MyDifferential > 0) { allTrades.Add(trade); }
                }
            }
        }
    }
}