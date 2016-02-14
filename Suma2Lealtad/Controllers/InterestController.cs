using Suma2Lealtad.Filters;
using Suma2Lealtad.Models;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Suma2Lealtad.Controllers
{
    [AuditingFilter]
    [HandleError]
    public class InterestController : Controller
    {
        private LealtadEntities db = new LealtadEntities();

        //
        // GET: /Interest/

        public ActionResult Index()
        {
            return View(db.Interests.ToList());
        }

        //
        // GET: /Interest/Details/5

        public ActionResult Details(int id = 0)
        {
            Interest interest = db.Interests.Find(id);
            if (interest == null)
            {
                return HttpNotFound();
            }
            return View(interest);
        }

        //
        // GET: /Interest/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Interest/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Interest interest)
        {
            if (ModelState.IsValid)
            {
                db.Interests.Add(interest);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(interest);
        }

        //
        // GET: /Interest/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Interest interest = db.Interests.Find(id);
            if (interest == null)
            {
                return HttpNotFound();
            }
            return View(interest);
        }

        //
        // POST: /Interest/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Interest interest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(interest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(interest);
        }

        //
        // GET: /Interest/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Interest interest = db.Interests.Find(id);
            if (interest == null)
            {
                return HttpNotFound();
            }
            return View(interest);
        }

        //
        // POST: /Interest/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Interest interest = db.Interests.Find(id);
            db.Interests.Remove(interest);
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