using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace TradeFinder.Network
{
    public class WebScraper
    {
        public string Html { get; set; }

        public WebScraper(string url) : this(url, null, null, null, null)
        {
            
        }

        public WebScraper(string url, string loginUrl, string postData, string login, string password)
        {
            string html = "";
            HttpWebRequest webRequest;
            StreamReader responseReader;
            string responseData;
            CookieContainer cookies = new CookieContainer();
            StreamWriter requestWriter;

            try
            {
                //get login  page with cookies
                if (loginUrl != null)
                {
                    string postDataFormatted = string.Format(postData, login, password);
                    webRequest = (HttpWebRequest)WebRequest.Create(loginUrl);
                    webRequest.CookieContainer = cookies;

                    //recieve non-authenticated cookie
                    webRequest.GetResponse().Close();

                    //post form  data to page
                    webRequest = (HttpWebRequest)WebRequest.Create(loginUrl);
                    webRequest.Method = WebRequestMethods.Http.Post;
                    webRequest.ContentType = "application/x-www-form-urlencoded";
                    webRequest.CookieContainer = cookies;
                    webRequest.ContentLength = postDataFormatted.Length; //login

                    requestWriter = new StreamWriter(webRequest.GetRequestStream());
                    requestWriter.Write(postDataFormatted);
                    requestWriter.Close();

                    //recieve authenticated cookie
                    webRequest.GetResponse().Close();
                }

                //now we get the authenticated page
                //webRequest = (HttpWebRequest)WebRequest.Create(team.Url);
                webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.CookieContainer = cookies;
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
                responseReader.Close();
                html = responseData;
            }
            catch
            {

            }

            Html = html;
        }


    }
}