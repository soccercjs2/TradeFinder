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

namespace TradeFinder.NumberFire
{
    public class NumberFireConnection
    {
        private const string projectionTableName = "projection-data";
        private const string qbProjectionUrl = "https://www.numberfire.com/nfl/fantasy/remaining-projections/qb";
        private const string rbProjectionUrl = "https://www.numberfire.com/nfl/fantasy/remaining-projections/rb";
        private const string wrProjectionUrl = "https://www.numberfire.com/nfl/fantasy/remaining-projections/wr";
        private const string teProjectionUrl = "https://www.numberfire.com/nfl/fantasy/remaining-projections/te";

        public DataTable Quarterbacks { get; set; }
        public DataTable RunningBacks { get; set; }
        public DataTable WideReceivers { get; set; }
        public DataTable TightEnds { get; set; }

        public NumberFireConnection()
        {
            string qbHtml = "", rbHtml = "", wrHtml = "", teHtml = "";

            //get qb data
            Thread qbThread = new Thread(() => qbHtml = ScrapeBrowser(qbProjectionUrl));
            qbThread.SetApartmentState(ApartmentState.STA);
            qbThread.Start();
            qbThread.Join();

            //get qb data
            Thread wrThread = new Thread(() => rbHtml = ScrapeBrowser(rbProjectionUrl));
            wrThread.SetApartmentState(ApartmentState.STA);
            wrThread.Start();
            wrThread.Join();

            //get qb data
            Thread rbThread = new Thread(() => wrHtml = ScrapeBrowser(wrProjectionUrl));
            rbThread.SetApartmentState(ApartmentState.STA);
            rbThread.Start();
            rbThread.Join();

            //get qb data
            Thread teThread = new Thread(() => teHtml = ScrapeBrowser(teProjectionUrl));
            teThread.SetApartmentState(ApartmentState.STA);
            teThread.Start();
            teThread.Join();

            Quarterbacks = ParseQbHtml(qbHtml);
            RunningBacks = ParseRbWrHtml(rbHtml);
            WideReceivers = ParseRbWrHtml(wrHtml);
            TightEnds = ParseTeHtml(teHtml);
        }

        private DataTable ParseQbHtml(string html)
        {
            //create html object and load html into it
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(html);

            //get projection-data table from html and initialize datatable to return
            HtmlNode projectionTable = document.GetElementbyId(projectionTableName);
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Player", typeof(string));
            dataTable.Columns.Add("OverallRank", typeof(int));
            dataTable.Columns.Add("PositionRank", typeof(int));
            dataTable.Columns.Add("Completions/Attempts", typeof(string));
            dataTable.Columns.Add("PassingYards", typeof(decimal));
            dataTable.Columns.Add("PassingTouchdowns", typeof(decimal));
            dataTable.Columns.Add("Interceptions", typeof(decimal));
            dataTable.Columns.Add("RushingAttempts", typeof(decimal));
            dataTable.Columns.Add("RushingYards", typeof(decimal));
            dataTable.Columns.Add("RushingTouchdowns", typeof(decimal));
            dataTable.Columns.Add("ConfidenceInterval", typeof(string));
            dataTable.Columns.Add("FantasyPoints", typeof(decimal));

            //loop through rows in projection-data
            foreach (HtmlNode row in projectionTable.SelectNodes("./tr"))
            {
                //create new datarow
                DataRow dataRow = dataTable.NewRow();

                //set row values
                dataRow["Player"] = row.SelectSingleNode("./td[1]").InnerText;
                dataRow["OverallRank"] = row.SelectSingleNode("./td[2]").InnerText;
                dataRow["PositionRank"] = row.SelectSingleNode("./td[3]").InnerText;
                dataRow["Completions/Attempts"] = row.SelectSingleNode("./td[4]").InnerText;
                dataRow["PassingYards"] = row.SelectSingleNode("./td[5]").InnerText;
                dataRow["PassingTouchdowns"] = row.SelectSingleNode("./td[6]").InnerText;
                dataRow["Interceptions"] = row.SelectSingleNode("./td[7]").InnerText;
                dataRow["RushingAttempts"] = row.SelectSingleNode("./td[8]").InnerText;
                dataRow["RushingYards"] = row.SelectSingleNode("./td[9]").InnerText;
                dataRow["RushingTouchdowns"] = row.SelectSingleNode("./td[10]").InnerText;
                dataRow["ConfidenceInterval"] = row.SelectSingleNode("./td[11]").InnerText;
                dataRow["FantasyPoints"] = row.SelectSingleNode("./td[12]").InnerText;

                //add datarow to datatable
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        private DataTable ParseRbWrHtml(string html)
        {
            //create html object and load html into it
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(html);

            //get projection-data table from html and initialize datatable to return
            HtmlNode projectionTable = document.GetElementbyId(projectionTableName);
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Player", typeof(string));
            dataTable.Columns.Add("OverallRank", typeof(int));
            dataTable.Columns.Add("PositionRank", typeof(int));
            dataTable.Columns.Add("RushingAttempts", typeof(decimal));
            dataTable.Columns.Add("RushingYards", typeof(decimal));
            dataTable.Columns.Add("RushingTouchdowns", typeof(decimal));
            dataTable.Columns.Add("Receptions", typeof(decimal));
            dataTable.Columns.Add("ReceivingYards", typeof(decimal));
            dataTable.Columns.Add("ReceivingTouchdowns", typeof(decimal));
            dataTable.Columns.Add("ConfidenceInterval", typeof(string));
            dataTable.Columns.Add("FantasyPoints", typeof(decimal));

            //loop through rows in projection-data
            foreach (HtmlNode row in projectionTable.SelectNodes("./tr"))
            {
                //create new datarow
                DataRow dataRow = dataTable.NewRow();

                //set row values
                dataRow["Player"] = row.SelectSingleNode("./td[1]").InnerText;
                dataRow["OverallRank"] = row.SelectSingleNode("./td[2]").InnerText;
                dataRow["PositionRank"] = row.SelectSingleNode("./td[3]").InnerText;
                dataRow["RushingAttempts"] = row.SelectSingleNode("./td[4]").InnerText;
                dataRow["RushingYards"] = row.SelectSingleNode("./td[5]").InnerText;
                dataRow["RushingTouchdowns"] = row.SelectSingleNode("./td[6]").InnerText;
                dataRow["Receptions"] = row.SelectSingleNode("./td[7]").InnerText;
                dataRow["ReceivingYards"] = row.SelectSingleNode("./td[8]").InnerText;
                dataRow["ReceivingTouchdowns"] = row.SelectSingleNode("./td[9]").InnerText;
                dataRow["ConfidenceInterval"] = row.SelectSingleNode("./td[10]").InnerText;
                dataRow["FantasyPoints"] = row.SelectSingleNode("./td[11]").InnerText;

                //add datarow to datatable
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        private DataTable ParseTeHtml(string html)
        {
            //create html object and load html into it
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(html);

            //get projection-data table from html and initialize datatable to return
            HtmlNode projectionTable = document.GetElementbyId(projectionTableName);
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Player", typeof(string));
            dataTable.Columns.Add("OverallRank", typeof(int));
            dataTable.Columns.Add("PositionRank", typeof(int));
            dataTable.Columns.Add("Receptions", typeof(decimal));
            dataTable.Columns.Add("ReceivingYards", typeof(decimal));
            dataTable.Columns.Add("ReceivingTouchdowns", typeof(decimal));
            dataTable.Columns.Add("ConfidenceInterval", typeof(string));
            dataTable.Columns.Add("FantasyPoints", typeof(decimal));

            //loop through rows in projection-data
            foreach (HtmlNode row in projectionTable.SelectNodes("./tr"))
            {
                //create new datarow
                DataRow dataRow = dataTable.NewRow();

                //set row values
                dataRow["Player"] = row.SelectSingleNode("./td[1]").InnerText;
                dataRow["OverallRank"] = row.SelectSingleNode("./td[2]").InnerText;
                dataRow["PositionRank"] = row.SelectSingleNode("./td[3]").InnerText;
                dataRow["Receptions"] = row.SelectSingleNode("./td[4]").InnerText;
                dataRow["ReceivingYards"] = row.SelectSingleNode("./td[5]").InnerText;
                dataRow["ReceivingTouchdowns"] = row.SelectSingleNode("./td[6]").InnerText;
                dataRow["ConfidenceInterval"] = row.SelectSingleNode("./td[7]").InnerText;
                dataRow["FantasyPoints"] = row.SelectSingleNode("./td[8]").InnerText;

                //add datarow to datatable
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

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