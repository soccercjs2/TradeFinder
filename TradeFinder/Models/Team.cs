using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using HtmlAgilityPack;

namespace TradeFinder.Models
{
    public class Team
    {
        public int TeamId { get; set; }
        public int LeagueId { get; set; }
        public string Name { get; set; }
        public bool MyTeam { get; set; }
        public DateTime AddedOn { get; set; }
        public string Url { get; set; }

        public virtual League League { get; set; }

        public DataTable GetTeam()
        {
            HttpWebRequest webRequest;
            StreamReader responseReader;
            string responseData;
            CookieContainer cookies = new CookieContainer();
            StreamWriter requestWriter;

            string postData = string.Format(null, League.UserName, League.Password);

            try
            {
                //get login  page with cookies
                webRequest = (HttpWebRequest)WebRequest.Create(League.LeagueHost.LoginUrl);
                webRequest.CookieContainer = cookies;

                //recieve non-authenticated cookie
                webRequest.GetResponse().Close();

                //post form  data to page
                webRequest = (HttpWebRequest)WebRequest.Create(League.LeagueHost.LoginUrl);
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
                webRequest = (HttpWebRequest)WebRequest.Create(Url);
                webRequest.CookieContainer = cookies;
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
                responseReader.Close();

                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(responseData);
            }
            catch
            {

            }

            return null;
        }
    }
}