﻿using Suma2Lealtad.Filters;
using Suma2Lealtad.Models;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Suma2Lealtad.Controllers
{
    [AuditingFilter]
    [HandleError]
    public class ChannelController : Controller
    {
        private LealtadEntities db = new LealtadEntities();

        //
        // GET: /Channel/

        public ActionResult Index()
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            return View(db.Channels.ToList());
        }

        //
        // GET: /Channel/Details/5

        public ActionResult Details(int id = 0)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            Channel channel = db.Channels.Find(id);
            if (channel == null)
            {
                return HttpNotFound();
            }
            return View(channel);
        }

        //
        // GET: /Channel/Create

        public ActionResult Create()
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            return View();
        }

        //
        // POST: /Channel/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Channel channel)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            if (ModelState.IsValid)
            {

                if (db.Channels.Count() > 0) 
                { 
                    channel.id = db.Channels.Max(c => c.id) + 1; 
                }
                else
                {
                    channel.id = 1;
                }
                db.Channels.Add(channel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(channel);
        }

        //
        // GET: /Channel/Edit/5

        public ActionResult Edit(int id = 0)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            Channel channel = db.Channels.Find(id);
            if (channel == null)
            {
                return HttpNotFound();
            }
            return View(channel);
        }

        //
        // POST: /Channel/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Channel channel)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            if (ModelState.IsValid)
            {
                db.Entry(channel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(channel);
        }

        //
        // GET: /Channel/Delete/5

        public ActionResult Delete(int id = 0)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            Channel channel = db.Channels.Find(id);
            if (channel == null)
            {
                return HttpNotFound();
            }
            return View(channel);
        }

        //
        // POST: /Channel/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            Channel channel = db.Channels.Find(id);
            db.Channels.Remove(channel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}