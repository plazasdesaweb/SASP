using Suma2Lealtad.Filters;
using Suma2Lealtad.Models;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Suma2Lealtad.Controllers
{

    [HandleError]
    [AuditingFilter]
    public class ReasonsController : Controller
    {
        private LealtadEntities db = new LealtadEntities();

        //
        // GET: /Reasons/

        public ActionResult Index()
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            return View(db.Reasons.Where(c => c.type == "Rechazo" && c.id > 1).ToList());
        }

        //
        // GET: /Reasons/Details/5

        public ActionResult Details(int id = 0)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            Reason reason = db.Reasons.Find(id);
            if (reason == null)
            {
                return HttpNotFound();
            }
            return View(reason);
        }

        //
        // GET: /Reasons/Create

        public ActionResult Create()
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            return View();
        }

        //
        // POST: /Reasons/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Reason reason)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            if (ModelState.IsValid)
            {
                db.Reasons.Add(reason);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(reason);
        }

        //
        // GET: /Reasons/Edit/5

        public ActionResult Edit(int id = 0)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            Suma2Lealtad.Models.Reason reason = db.Reasons.Find(id);
            if (reason == null)
            {
                return HttpNotFound();
            }
            return View(reason);
        }

        //
        // POST: /Reasons/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Suma2Lealtad.Models.Reason reason)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            if (ModelState.IsValid)
            {
                reason.type = "Rechazo";
                db.Entry(reason).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(reason);
        }

        //
        // GET: /Reasons/Delete/5

        public ActionResult Delete(int id = 0)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            Reason reason = db.Reasons.Find(id);
            if (reason == null)
            {
                return HttpNotFound();
            }
            return View(reason);
        }

        //
        // POST: /Reasons/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            Reason reason = db.Reasons.Find(id);
            db.Reasons.Remove(reason);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}