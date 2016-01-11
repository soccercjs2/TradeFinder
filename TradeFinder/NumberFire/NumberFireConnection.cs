using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using TradeFinder.Network;
using HtmlAgilityPack;
using System.Net;
using WatiN.Core;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TradeFinder.Data;
using TradeFinder.Models;
using System.IO;
using System.Diagnostics;
using TradeFinder.ViewModels;
using TradeFinder.PlayerPool;

namespace TradeFinder.NumberFire
{
    public class NumberFireConnection
    {
        private TradeFinderContext db = new TradeFinderContext();

        private const string projectionTableName = "projection-data";
        //private const string qbProjectionUrl = "https://www.numberfire.com/nfl/fantasy/remaining-projections/qb";
        //private const string rbProjectionUrl = "https://www.numberfire.com/nfl/fantasy/remaining-projections/rb";
        //private const string wrProjectionUrl = "https://www.numberfire.com/nfl/fantasy/remaining-projections/wr";
        //private const string teProjectionUrl = "https://www.numberfire.com/nfl/fantasy/remaining-projections/te";

        private const string qbProjectionUrl = "A:\\Documents\\Visual Studio 2015\\Projects\\TradeFinder\\TradeFinder\\Projections\\quarterbacks.html";
        private const string rbProjectionUrl = "A:\\Documents\\Visual Studio 2015\\Projects\\TradeFinder\\TradeFinder\\Projections\\runningbacks.html";
        private const string wrProjectionUrl = "A:\\Documents\\Visual Studio 2015\\Projects\\TradeFinder\\TradeFinder\\Projections\\widereceivers.html";
        private const string teProjectionUrl = "A:\\Documents\\Visual Studio 2015\\Projects\\TradeFinder\\TradeFinder\\Projections\\tightends.html";

        private League _league;

        //public DataTable Quarterbacks { get; set; }
        //public DataTable RunningBacks { get; set; }
        //public DataTable WideReceivers { get; set; }
        //public DataTable TightEnds { get; set; }

        public NumberFireConnection(int leagueId, string sessionId)
        {
            _league = db.Leagues.Find(leagueId);
            _league.CurrentSessionId = HttpContext.Current.Session.SessionID;
            db.SaveChanges();
        }

        public void ImportPlayers()
        {
            ImportQbHtml();
            ImportRbHtml();
            ImportWrHtml();
            ImportTeHtml();

            LeaguePlayerPool leaguePlayerPool = new LeaguePlayerPool(_league.LeagueId);
            leaguePlayerPool.SetTeamsPlayers();
            leaguePlayerPool.CalculateTradeValues();

            //LeagueView leageView = new LeagueView(_league.LeagueId);

            ////load teams and set player teams
            //List<Team> teams = db.Teams.Where(t => t.LeagueId == _league.LeagueId).ToList();
            //foreach (Team team in teams)
            //{
            //    AssignPlayerTeams(team, leageView);
            //}

            ////calculate trade values
            //CalculateTradeValues(leageView);
        }

        private void ImportQbHtml()
        {
            //get qb data
            string html = "";
            Thread qbThread = new Thread(() => html = ScrapeBrowser(qbProjectionUrl));
            qbThread.SetApartmentState(ApartmentState.STA);
            qbThread.Start();
            qbThread.Join();

            //create html object and load html into it
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(html);

            //get projection-data table from html
            HtmlNode projectionTable = document.GetElementbyId(projectionTableName);

            //loop through rows in projection-data
            foreach (HtmlNode row in projectionTable.SelectNodes("./tr"))
            {
                //create new datarow
                Quarterback quarterback = new Quarterback();
                NumberFireParser numberFireParser = new NumberFireParser(row.SelectSingleNode("./td[1]").InnerText);

                //set row values
                quarterback.SessionId = _league.CurrentSessionId;
                quarterback.LeagueId = _league.LeagueId;
                quarterback.Name = numberFireParser.Player;
                quarterback.Position = numberFireParser.Position;
                quarterback.NflTeam = numberFireParser.Team;
                quarterback.NflAlternateTeam = numberFireParser.AlternateTeam;
                quarterback.PassingAttempts = decimal.Parse(row.SelectSingleNode("./td[4]").InnerText.Split('/')[0]);
                quarterback.PassingCompletions = decimal.Parse(row.SelectSingleNode("./td[4]").InnerText.Split('/')[1]);
                quarterback.PassingYards = decimal.Parse(row.SelectSingleNode("./td[5]").InnerText);
                quarterback.PassingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[6]").InnerText);
                quarterback.Interceptions = decimal.Parse(row.SelectSingleNode("./td[7]").InnerText);
                quarterback.RushingAttempts = decimal.Parse(row.SelectSingleNode("./td[8]").InnerText);
                quarterback.RushingYards = decimal.Parse(row.SelectSingleNode("./td[9]").InnerText);
                quarterback.RushingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[10]").InnerText);
                quarterback.Points = decimal.Parse(row.SelectSingleNode("./td[12]").InnerText);

                //add datarow to datatable
                db.Players.Add(quarterback);
                db.SaveChanges();
            }
        }

        private void ImportRbHtml()
        {
            //get rb data
            string html = "";
            Thread rbThread = new Thread(() => html = ScrapeBrowser(rbProjectionUrl));
            rbThread.SetApartmentState(ApartmentState.STA);
            rbThread.Start();
            rbThread.Join();

            //create html object and load html into it
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(html);

            //get projection-data table from html and initialize datatable to return
            HtmlNode projectionTable = document.GetElementbyId(projectionTableName);

            //loop through rows in projection-data
            foreach (HtmlNode row in projectionTable.SelectNodes("./tr"))
            {
                //create new datarow
                RunningBack runningBack = new RunningBack();
                NumberFireParser numberFireParser = new NumberFireParser(row.SelectSingleNode("./td[1]").InnerText);

                //set row values
                runningBack.SessionId = _league.CurrentSessionId;
                runningBack.LeagueId = _league.LeagueId;
                runningBack.Name = numberFireParser.Player;
                runningBack.Position = numberFireParser.Position;
                runningBack.NflTeam = numberFireParser.Team;
                runningBack.NflAlternateTeam = numberFireParser.AlternateTeam;
                runningBack.RushingAttempts = decimal.Parse(row.SelectSingleNode("./td[4]").InnerText);
                runningBack.RushingYards = decimal.Parse(row.SelectSingleNode("./td[5]").InnerText);
                runningBack.RushingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[6]").InnerText);
                runningBack.Receptions = decimal.Parse(row.SelectSingleNode("./td[7]").InnerText);
                runningBack.ReceivingYards = decimal.Parse(row.SelectSingleNode("./td[8]").InnerText);
                runningBack.ReceivingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[9]").InnerText);
                runningBack.Points = decimal.Parse(row.SelectSingleNode("./td[11]").InnerText);

                //add datarow to datatable
                db.Players.Add(runningBack);
                db.SaveChanges();
            }
        }

        private void ImportWrHtml()
        {
            //get wr data
            string html = "";
            Thread wrThread = new Thread(() => html = ScrapeBrowser(wrProjectionUrl));
            wrThread.SetApartmentState(ApartmentState.STA);
            wrThread.Start();
            wrThread.Join();

            //create html object and load html into it
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(html);

            //get projection-data table from html and initialize datatable to return
            HtmlNode projectionTable = document.GetElementbyId(projectionTableName);

            //loop through rows in projection-data
            foreach (HtmlNode row in projectionTable.SelectNodes("./tr"))
            {
                //create new datarow
                WideReceiver wideReceiver = new WideReceiver();
                NumberFireParser numberFireParser = new NumberFireParser(row.SelectSingleNode("./td[1]").InnerText);

                //set row values
                wideReceiver.SessionId = _league.CurrentSessionId;
                wideReceiver.LeagueId = _league.LeagueId;
                wideReceiver.Name = numberFireParser.Player;
                wideReceiver.Position = numberFireParser.Position;
                wideReceiver.NflTeam = numberFireParser.Team;
                wideReceiver.NflAlternateTeam = numberFireParser.AlternateTeam;
                wideReceiver.RushingAttempts = decimal.Parse(row.SelectSingleNode("./td[4]").InnerText);
                wideReceiver.RushingYards = decimal.Parse(row.SelectSingleNode("./td[5]").InnerText);
                wideReceiver.RushingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[6]").InnerText);
                wideReceiver.Receptions = decimal.Parse(row.SelectSingleNode("./td[7]").InnerText);
                wideReceiver.ReceivingYards = decimal.Parse(row.SelectSingleNode("./td[8]").InnerText);
                wideReceiver.ReceivingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[9]").InnerText);
                wideReceiver.Points = decimal.Parse(row.SelectSingleNode("./td[11]").InnerText);

                //add datarow to datatable
                db.Players.Add(wideReceiver);
                db.SaveChanges();
            }
        }

        private void ImportTeHtml()
        {
            //get te data
            string html = "";
            Thread teThread = new Thread(() => html = ScrapeBrowser(teProjectionUrl));
            teThread.SetApartmentState(ApartmentState.STA);
            teThread.Start();
            teThread.Join();

            //create html object and load html into it
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(html);

            //get projection-data table from html and initialize datatable to return
            HtmlNode projectionTable = document.GetElementbyId(projectionTableName);

            //loop through rows in projection-data
            foreach (HtmlNode row in projectionTable.SelectNodes("./tr"))
            {
                //create new datarow
                TightEnd tightEnd = new TightEnd();
                NumberFireParser numberFireParser = new NumberFireParser(row.SelectSingleNode("./td[1]").InnerText);

                //set row values
                tightEnd.SessionId = _league.CurrentSessionId;
                tightEnd.LeagueId = _league.LeagueId;
                tightEnd.Name = numberFireParser.Player;
                tightEnd.Position = numberFireParser.Position;
                tightEnd.NflTeam = numberFireParser.Team;
                tightEnd.NflAlternateTeam = numberFireParser.AlternateTeam;
                tightEnd.Receptions = decimal.Parse(row.SelectSingleNode("./td[4]").InnerText);
                tightEnd.ReceivingYards = decimal.Parse(row.SelectSingleNode("./td[5]").InnerText);
                tightEnd.ReceivingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[6]").InnerText);
                tightEnd.Points = decimal.Parse(row.SelectSingleNode("./td[8]").InnerText);

                //add datarow to datatable
                db.Players.Add(tightEnd);
                db.SaveChanges();
            }
        }

        //private void AssignPlayerTeams(Team team, LeagueView leagueView)
        //{
        //    HttpWebRequest webRequest;
        //    StreamReader responseReader;
        //    string responseData;
        //    CookieContainer cookies = new CookieContainer();
        //    StreamWriter requestWriter;

        //    string postData = string.Format(null, team.League.UserName, team.League.Password);

        //    try
        //    {
        //        //get login  page with cookies
        //        webRequest = (HttpWebRequest)WebRequest.Create(team.League.LeagueHost.LoginUrl);
        //        webRequest.CookieContainer = cookies;

        //        //recieve non-authenticated cookie
        //        webRequest.GetResponse().Close();

        //        //post form  data to page
        //        webRequest = (HttpWebRequest)WebRequest.Create(team.League.LeagueHost.LoginUrl);
        //        webRequest.Method = WebRequestMethods.Http.Post;
        //        webRequest.ContentType = "application/x-www-form-urlencoded";
        //        webRequest.CookieContainer = cookies;
        //        webRequest.ContentLength = postData.Length;

        //        requestWriter = new StreamWriter(webRequest.GetRequestStream());
        //        requestWriter.Write(postData);
        //        requestWriter.Close();

        //        //recieve authenticated cookie
        //        webRequest.GetResponse().Close();

        //        //now we get the authenticated page
        //        webRequest = (HttpWebRequest)WebRequest.Create(team.Url);
        //        webRequest.CookieContainer = cookies;
        //        responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
        //        responseData = responseReader.ReadToEnd();
        //        responseReader.Close();

        //        //load html into htmlagilitypack
        //        HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
        //        document.LoadHtml(responseData);

        //        HtmlNode table = document.GetElementbyId(leagueView.LeagueHost.StarterTableName);
        //        foreach (Player player in leagueView.Players)
        //        {
        //            if (table.InnerHtml.ToLower().Contains(player.Name.ToLower()) && table.InnerHtml.ToLower().Contains(player.Position.ToLower()) &&
        //                    (table.InnerHtml.ToLower().Contains(player.NflTeam.ToLower()) || table.InnerHtml.ToLower().Contains(player.NflAlternateTeam.ToLower())))
        //            {
        //                player.TeamId = team.TeamId;
        //                db.SaveChanges();
        //            }
        //        }
        //    }
        //    catch { }
        //}

        //private void CalculateTradeValues(LeagueView leagueView)
        //{
        //    //calculate trade values
        //    Player waiverQb = leagueView.GetBestWaiverQb();
        //    Player waiverRb = leagueView.GetBestWaiverRb();
        //    Player waiverWr = leagueView.GetBestWaiverWr();
        //    Player waiverTe = leagueView.GetBestWaiverTe();

        //    //loop through players and calculate trade vlaues
        //    foreach (Player player in leagueView.Players)
        //    {
        //        switch (player.Position)
        //        {
        //            case "QB":
        //                player.TradeValue = player.Points - waiverQb.Points;
        //                break;
        //            case "RB":
        //                player.TradeValue = player.Points - waiverRb.Points;
        //                break;
        //            case "WR":
        //                player.TradeValue = player.Points - waiverWr.Points;
        //                break;
        //            case "TE":
        //                player.TradeValue = player.Points - waiverTe.Points;
        //                break;
        //        }

        //        db.SaveChanges();
        //    }
        //}

        private string ScrapeBrowser(string url)
        {
            using (WebBrowser browser = new WebBrowser())
            {

                browser.ScriptErrorsSuppressed = true;
                browser.Navigate(url);

                // Wait for control to load page
                while (browser.ReadyState != WebBrowserReadyState.Complete)
                    Application.DoEvents();

                return browser.Document.Body.InnerHtml;
            }
        }
    }
}