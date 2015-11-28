using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace TradeFinder.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            string loginUrl = "http://www.fleaflicker.com/nfl/login";

            // name / value pairs. field names should match form elements
            string data = "email=soccercjs2%40gmail.com&password=united2";

            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            byte[] response = webClient.UploadData(
                loginUrl, "POST", Encoding.ASCII.GetBytes(data)
            );

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}