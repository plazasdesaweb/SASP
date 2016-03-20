using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suma2Lealtad.Models;

namespace Suma2Lealtad.Controllers
{
    public class EstatusController : Controller
    {
        private LealtadEntities db = new LealtadEntities();

        //
        // GET: /Estatus/

        public ActionResult Index()
        {
            return View(db.SumaStatuses.ToList());
        }

        //
        // GET: /Estatus/Details/5

        public ActionResult Details(int id = 0)
        {
            SumaStatus sumastatus = db.SumaStatuses.Find(id);
            if (sumastatus == null)
            {
                return HttpNotFound();
            }
            return View(sumastatus);
        }

        //
        // GET: /Estatus/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Estatus/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SumaStatus sumastatus)
        {
            if (ModelState.IsValid)
            {
                db.SumaStatuses.Add(sumastatus);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sumastatus);
        }

        //
        // GET: /Estatus/Edit/5

        public ActionResult Edit(int id = 0)
        {
            SumaStatus sumastatus = db.SumaStatuses.Find(id);
            if (sumastatus == null)
            {
                return HttpNotFound();
            }
            return View(sumastatus);
        }

        //
        // POST: /Estatus/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SumaStatus sumastatus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sumastatus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sumastatus);
        }

        //
        // GET: /Estatus/Delete/5

        public ActionResult Delete(int id = 0)
        {
            SumaStatus sumastatus = db.SumaStatuses.Find(id);
            if (sumastatus == null)
            {
                return HttpNotFound();
            }
            return View(sumastatus);
        }

        //
        // POST: /Estatus/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SumaStatus sumastatus = db.SumaStatuses.Find(id);
            db.SumaStatuses.Remove(sumastatus);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}