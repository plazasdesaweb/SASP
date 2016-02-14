using Suma2Lealtad.Models;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Suma2Lealtad.Controllers
{
    [HandleError]
    public class AuditingController : Controller
    {
        private LealtadEntities db = new LealtadEntities();

        //
        // GET: /Auditing/

        public ActionResult Index()
        {
            return View(db.Auditings.ToList());
        }

        //
        // GET: /Auditing/Details/5

        public ActionResult Details(int id = 0)
        {
            Auditing auditing = db.Auditings.Find(id);
            if (auditing == null)
            {
                return HttpNotFound();
            }
            return View(auditing);
        }

        //
        // GET: /Auditing/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Auditing/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Auditing auditing)
        {
            if (ModelState.IsValid)
            {
                db.Auditings.Add(auditing);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(auditing);
        }

        //
        // GET: /Auditing/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Auditing auditing = db.Auditings.Find(id);
            if (auditing == null)
            {
                return HttpNotFound();
            }
            return View(auditing);
        }

        //
        // POST: /Auditing/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Auditing auditing)
        {
            if (ModelState.IsValid)
            {
                db.Entry(auditing).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(auditing);
        }

        //
        // GET: /Auditing/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Auditing auditing = db.Auditings.Find(id);
            if (auditing == null)
            {
                return HttpNotFound();
            }
            return View(auditing);
        }

        //
        // POST: /Auditing/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Auditing auditing = db.Auditings.Find(id);
            db.Auditings.Remove(auditing);
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