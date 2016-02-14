using Suma2Lealtad.Filters;
using Suma2Lealtad.Models;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Suma2Lealtad.Controllers
{
    [AuditingFilter]
    [HandleError]
    public class StatusController : Controller
    {
        private LealtadEntities db = new LealtadEntities();

        //
        // GET: /Status/

        public ActionResult Index()
        {
            return View(db.Statuses.ToList());
        }

        //
        // GET: /Status/Details/5

        public ActionResult Details(int id = 0)
        {
            Status status = db.Statuses.Find(id);
            if (status == null)
            {
                return HttpNotFound();
            }
            return View(status);
        }

        //
        // GET: /Status/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Status/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SumaStatus status)
        {
            if (ModelState.IsValid)
            {
                if (db.SumaStatuses.Count() > 0)
                {
                    status.id = db.SumaStatuses.Max(c => c.id) + 1;
                }
                else
                {
                    status.id = 1;
                }
                db.SumaStatuses.Add(status);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(status);
        }

        //
        // GET: /Status/Edit/5

        public ActionResult Edit(int id = 0)
        {
            SumaStatus status = db.SumaStatuses.Find(id);
            if (status == null)
            {
                return HttpNotFound();
            }
            return View(status);
        }

        //
        // POST: /Status/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SumaStatus status)
        {
            if (ModelState.IsValid)
            {
                db.Entry(status).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(status);
        }

        //
        // GET: /Status/Delete/5

        public ActionResult Delete(int id = 0)
        {
            SumaStatus status = db.SumaStatuses.Find(id);
            if (status == null)
            {
                return HttpNotFound();
            }
            return View(status);
        }

        //
        // POST: /Status/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SumaStatus status = db.SumaStatuses.Find(id);
            db.SumaStatuses.Remove(status);
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