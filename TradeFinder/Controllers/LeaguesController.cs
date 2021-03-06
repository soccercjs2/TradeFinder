﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TradeFinder.Data;
using TradeFinder.Models;
using Microsoft.AspNet.Identity;
using TradeFinder.ViewModels;
using TradeFinder.NumberFire;

namespace TradeFinder.Controllers
{
    [Authorize]
    public class LeaguesController : Controller
    {
        private TradeFinderContext db = new TradeFinderContext();

        // GET: Leagues
        public ActionResult Index()
        {
            string currentUser = User.Identity.GetUserId();
            List<LeagueView> leagueViews = new List<LeagueView>();
            List<League> leagues = db.Leagues.Where(l => l.UserId == currentUser).ToList();

            foreach (League league in leagues)
            {
                leagueViews.Add(new LeagueView(league.LeagueId));
            }

            return View(leagueViews);
        }

        // GET: Leagues/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            League league = db.Leagues.Find(id);
            if (league == null)
            {
                return HttpNotFound();
            }
            return View(league);
        }

        // GET: Leagues/Create
        public ActionResult Create()
        {
            List<LeagueHost> leagueHosts = db.LeagueHosts.ToList<LeagueHost>();
            ViewBag.LeagueHosts = new SelectList(leagueHosts, "LeagueHostId", "Name");

            return View();
        }

        // POST: Leagues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LeagueId,LeagueHostId,Name,UserName,Password")] League league)
        {
            if (ModelState.IsValid)
            {
                league.UserId = User.Identity.GetUserId();
                league.AddedOn = DateTime.Now;
                db.Leagues.Add(league);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(league);
        }

        // GET: Leagues/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            League league = db.Leagues.Find(id);
            List<LeagueHost> leagueHosts = db.LeagueHosts.ToList<LeagueHost>();
            ViewBag.LeagueHosts = new SelectList(leagueHosts, "LeagueHostId", "Name");

            if (league == null)
            {
                return HttpNotFound();
            }
            return View(league);
        }

        // POST: Leagues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LeagueId,Name,LeagueHostId,UserId,AddedOn,UserName,Password")] League league)
        {
            if (ModelState.IsValid)
            {
                db.Entry(league).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(league);
        }

        // GET: Leagues/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            League league = db.Leagues.Find(id);
            if (league == null)
            {
                return HttpNotFound();
            }
            return View(league);
        }

        // POST: Leagues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            League league = db.Leagues.Find(id);
            db.Leagues.Remove(league);
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
