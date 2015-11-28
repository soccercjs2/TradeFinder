using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TradeFinder.Data;
using TradeFinder.Models;
using TradeFinder.ViewModels;

namespace TradeFinder.Controllers
{
    [Authorize]
    public class TeamsController : Controller
    {
        private TradeFinderContext db = new TradeFinderContext();

        // GET: Teams/LeagueId
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TeamIndexData viewModel = new TeamIndexData(id.GetValueOrDefault());

            return View(viewModel);
        }

        // GET: Teams/Details/TeamId
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            League league = db.Leagues.Find(team.LeagueId);
            if (league == null)
            {
                return HttpNotFound();
            }

            ViewBag.Html = GetTeamHtml(league, team);

            return View(team);
        }

        private string GetTeamHtml(League league, Team team)
        {
            CookieContainer _yahooContainer;
            string _login = league.UserName;
            string _password = league.Password;
            string loginUrl = "https://login.yahoo.com/config/login";

            //string strPostData = String.Format("login={0}&passwd={1}", _login, _password);
            string strPostData = String.Format("countrycode={0}&username={1}&passwd={2}",
                1, league.UserName, league.Password);

            // Setup the http request.
            HttpWebRequest wrWebRequest = WebRequest.Create(loginUrl) as HttpWebRequest;
            wrWebRequest.Method = "POST";
            wrWebRequest.ContentLength = strPostData.Length;
            wrWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            _yahooContainer = new CookieContainer();
            wrWebRequest.CookieContainer = _yahooContainer;

            // Post to the login form.
            using (StreamWriter swRequestWriter = new StreamWriter(wrWebRequest.GetRequestStream()))
            {
                swRequestWriter.Write(strPostData);
                swRequestWriter.Close();
            }

            // Get the response.
            HttpWebResponse hwrWebResponse = (HttpWebResponse)wrWebRequest.GetResponse();

            if (hwrWebResponse.ResponseUri.AbsoluteUri.Contains("my.yahoo.com"))
            {
                // you authenticated properly
            }

            // Now use the cookies to create more requests.
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(team.Url);
            req.CookieContainer = _yahooContainer;
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            StreamReader sr = new StreamReader(resp.GetResponseStream());
            string html = sr.ReadToEnd();

            return html;
        }

        // GET: Teams/Create/LeagueId
        public ActionResult Create(int id)
        {
            Team team = new Team();
            team.LeagueId = id;

            return View(team);
        }

        // POST: Teams/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TeamId,LeagueId,Name,MyTeam,AddedOn,Url")] Team team)
        {
            if (ModelState.IsValid)
            {
                team.AddedOn = DateTime.Now;
                db.Teams.Add(team);
                db.SaveChanges();
                return RedirectToAction("Index", new { Id = team.LeagueId });
            }

            return View(team);
        }

        // GET: Teams/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TeamId,LeagueId,Name,MyTeam,AddedOn,Url")] Team team)
        {
            if (ModelState.IsValid)
            {
                db.Entry(team).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { Id = team.LeagueId });
            }
            return View(team);
        }

        // GET: Teams/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Team team = db.Teams.Find(id);
            db.Teams.Remove(team);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
