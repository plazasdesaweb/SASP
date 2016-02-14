using Suma2Lealtad.Models;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Suma2Lealtad.Controllers
{
    [HandleError]
    public class SecurityLevelController : Controller
    {
        private LealtadEntities db = new LealtadEntities();

        //
        // GET: /SecurityLevel/

        public ActionResult Index()
        {
            return View(db.SecurityLevels.ToList());
        }

        //
        // GET: /SecurityLevel/Details/5

        public ActionResult Details(int id = 0)
        {
            SecurityLevel securitylevel = db.SecurityLevels.Find(id);
            if (securitylevel == null)
            {
                return HttpNotFound();
            }
            return View(securitylevel);
        }

        //
        // GET: /SecurityLevel/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /SecurityLevel/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SecurityLevel securitylevel)
        {
            if (ModelState.IsValid)
            {
                db.SecurityLevels.Add(securitylevel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(securitylevel);
        }

        //
        // GET: /SecurityLevel/Edit/5

        public ActionResult Edit(int id = 0)
        {
            SecurityLevel securitylevel = db.SecurityLevels.Find(id);
            if (securitylevel == null)
            {
                return HttpNotFound();
            }
            return View(securitylevel);
        }

        //
        // POST: /SecurityLevel/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SecurityLevel securitylevel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(securitylevel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(securitylevel);
        }

        //
        // GET: /SecurityLevel/Delete/5

        public ActionResult Delete(int id = 0)
        {
            SecurityLevel securitylevel = db.SecurityLevels.Find(id);
            if (securitylevel == null)
            {
                return HttpNotFound();
            }
            return View(securitylevel);
        }

        //
        // POST: /SecurityLevel/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SecurityLevel securitylevel = db.SecurityLevels.Find(id);
            db.SecurityLevels.Remove(securitylevel);
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