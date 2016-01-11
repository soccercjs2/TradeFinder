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
using System.Web.UI;
using TradeFinder.Data;
using TradeFinder.Models;
using TradeFinder.ViewModels;
using TradeFinder.Network;
using TradeFinder.NumberFire;

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

            LeagueView leagueView = new LeagueView(id.GetValueOrDefault());
            string sessionId = HttpContext.Session.SessionID;

            if (leagueView.League.CurrentSessionId == sessionId)
            {
                NumberFireConnection numberFireConnection = new NumberFireConnection(leagueView.League.LeagueId, sessionId);
                numberFireConnection.ImportPlayers();
            }

            return View(leagueView);
        }

        // GET: Teams/Details/TeamId
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeamView team = new TeamView(id, (DataTable)Session["Quarterbacks"], (DataTable)Session["RunningBacks"], (DataTable)Session["WideReceivers"], (DataTable)Session["TightEnds"]);
            if (team == null)
            {
                return HttpNotFound();
            }


            return View(team);
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
