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

            byte[] data = new ASCIIEncoding().GetBytes("email=soccercjs2%40gmail.com&password=united2");
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(loginUrl);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.ContentLength = data.Length;
            Stream myStream = httpWebRequest.GetRequestStream();
            myStream.Write(data, 0, data.Length);
            myStream.Close();

            //Build up your post string
            string postData = "email=soccercjs2%40gmail.com&password=united2";

            //Create a POST WebRequest
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(loginUrl);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            //Write your post string to the body of the POST WebRequest
            var sw = new StreamWriter(request.GetRequestStream());
            sw.Write(postData.ToString());
            sw.Close();

            //Get the response and read it
            var response = request.GetResponse();
            var raw_result_as_string = (new StreamReader(response.GetResponseStream())).ReadToEnd();

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}