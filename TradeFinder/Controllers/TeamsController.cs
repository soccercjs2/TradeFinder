using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
            string _login = league.UserName;
            string _password = league.Password;
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

            return raw_result_as_string;
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
