using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TradeFinder.Controllers
{
    public class PlayersController : Controller
    {
        // GET: NumberFire
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Quarterbacks()
        {
            return View(Session["Quarterbacks"]);
        }

        public ActionResult RunningBacks()
        {
            return View(Session["RunningBacks"]);
        }

        public ActionResult WideReceivers()
        {
            return View(Session["WideReceivers"]);
        }

        public ActionResult TightEnds()
        {
            return View(Session["TightEnds"]);
        }
    }
}