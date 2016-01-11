using HtmlAgilityPack;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using TradeFinder.Data;
using TradeFinder.Models;
using TradeFinder.NumberFire;

namespace TradeFinder.ViewModels
{
    public class TeamView
    {
        public Team Team { get; set; }
        public DataTable TeamTable { get; set; }

        private TradeFinderContext db = new TradeFinderContext();

        public TeamView(int? id, DataTable quarterbacks, DataTable runningBacks, DataTable wideReceivers, DataTable tightEnds)
        {
            Team = db.Teams.Find(id);

            HttpWebRequest webRequest;
            StreamReader responseReader;
            string responseData;
            CookieContainer cookies = new CookieContainer();
            StreamWriter requestWriter;

            string postData = string.Format(null, Team.League.UserName, Team.League.Password);

            DataTable team = new DataTable();
            team.Columns.Add("Player", typeof(string));
            team.Columns.Add("Position", typeof(string));
            team.Columns.Add("Team", typeof(string));

            try
            {
                //get login  page with cookies
                webRequest = (HttpWebRequest)WebRequest.Create(Team.League.LeagueHost.LoginUrl);
                webRequest.CookieContainer = cookies;

                //recieve non-authenticated cookie
                webRequest.GetResponse().Close();

                //post form  data to page
                webRequest = (HttpWebRequest)WebRequest.Create(Team.League.LeagueHost.LoginUrl);
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
                webRequest = (HttpWebRequest)WebRequest.Create(Team.Url);
                webRequest.CookieContainer = cookies;
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
                responseReader.Close();

                //load html into htmlagilitypack
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(responseData);

                //loop through quarterbacks and add team's player to data table
                DataTable teamQuarterbacks = GetPositionPlayers(quarterbacks, document);
                DataTable teamRunningBacks = GetPositionPlayers(runningBacks, document);
                DataTable teamWideReceivers = GetPositionPlayers(wideReceivers, document);
                DataTable teamTightEnds = GetPositionPlayers(tightEnds, document);

                //add players from each position to team datatable
                foreach (DataRow row in teamQuarterbacks.Rows) { team.ImportRow(row); }
                foreach (DataRow row in teamRunningBacks.Rows) { team.ImportRow(row); }
                foreach (DataRow row in teamWideReceivers.Rows) { team.ImportRow(row); }
                foreach (DataRow row in teamTightEnds.Rows) { team.ImportRow(row); }
            }
            catch { }

            TeamTable = team;
        }

        private DataTable GetPositionPlayers(DataTable players, HtmlDocument document)
        {
            //initialize datatable to return
            DataTable teamPlayers = new DataTable();
            teamPlayers.Columns.Add("Player", typeof(string));
            teamPlayers.Columns.Add("Position", typeof(string));
            teamPlayers.Columns.Add("Team", typeof(string));

            foreach (DataRow dataRow in players.Rows)
            {
                NumberFireParser parser = new NumberFireParser(dataRow["Player"].ToString());
                string playerName = parser.Player;
                string playerPosition = parser.Position;
                string playerTeam = parser.Team;
                string playerAlternateTeam = parser.AlternateTeam;

                if (FindPlayerInHtml(document, Team.League.LeagueHost.StarterTableName, playerName, playerPosition, playerTeam, playerAlternateTeam))
                {
                    DataRow row = teamPlayers.NewRow();
                    row["Player"] = playerName;
                    row["Position"] = playerPosition;
                    row["Team"] = playerTeam;
                    teamPlayers.Rows.Add(row);
                }
            }

            return teamPlayers;
        }

        private bool FindPlayerInHtml(HtmlDocument document, string tableName, string name, string position, string team, string altTeam)
        {
            if (tableName != null)
            {
                HtmlNode table = document.GetElementbyId(Team.League.LeagueHost.StarterTableName);
                foreach (HtmlNode htmlRow in table.SelectNodes("./tr"))
                {
                    if (htmlRow.InnerHtml.ToLower().Contains(name.ToLower()) && htmlRow.InnerHtml.ToLower().Contains(position.ToLower()) &&
                        (htmlRow.InnerHtml.ToLower().Contains(team.ToLower()) || htmlRow.InnerHtml.ToLower().Contains(altTeam.ToLower())))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private DataRow FindPlayerInDataRows(List<DataRow> players, string name, string position, string team, string altTeam)
        {
            if (players != null)
            {
                foreach (DataRow player in players)
                {
                    if (player["Player"].ToString().ToLower().Contains(name.ToLower()) && player["Position"].ToString().ToLower().Contains(position.ToLower()) &&
                        (player["Team"].ToString().ToLower().Contains(team.ToLower()) || player["Team"].ToString().ToLower().Contains(altTeam.ToLower())))
                    {
                        return player;
                    }
                }
            }

            return null;
        }

        public decimal OptimalLineUpPoints()
        {
            //return OptimalLineUpPoints(TeamTable.Select());
            return 0;
        }

        public decimal OptimalLineUpPoints(List<DataRow> playersGained, List<DataRow> playersLost)
        {
            //get array of datarows for current team
            List<DataRow> team = new List<DataRow>(TeamTable.AsEnumerable());

            //add gained players to team
            foreach (DataRow player in playersGained)
            {
                team.Add(player);
            }

            //remove lost players from team
            foreach (DataRow player in playersLost)
            {
                //make numberFireParser to compare lost player with players in team table
                NumberFireParser numberFireParser = new NumberFireParser(player["Player"].ToString());

                //find lost player
                DataRow lostPlayer = FindPlayerInDataRows(team, numberFireParser.Player, numberFireParser.Position, numberFireParser.Team, numberFireParser.AlternateTeam);

                //remove lost player
                if (lostPlayer != null)
                {
                    team.Remove(lostPlayer);
                }
            }

            return OptimalLineUpPoints(team);
        }

        private decimal OptimalLineUpPoints(List<DataRow> players)
        {
            return 0;
        }
    }
}