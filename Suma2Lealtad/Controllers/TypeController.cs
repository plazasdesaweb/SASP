using Suma2Lealtad.Filters;
using Suma2Lealtad.Models;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Suma2Lealtad.Controllers
{
    [AuditingFilter]
    [HandleError]
    public class TypeController : Controller
    {
        private LealtadEntities db = new LealtadEntities();

        //
        // GET: /Type/

        public ActionResult Index()
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            return View(db.Types.ToList());
        }

        //
        // GET: /Type/Details/5

        public ActionResult Details(int id = 0)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            Suma2Lealtad.Models.Type type = db.Types.Find(id);
            if (type == null)
            {
                return HttpNotFound();
            }
            return View(type);
        }

        //
        // GET: /Type/Create

        public ActionResult Create()
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            return View();
        }

        //
        // POST: /Type/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Suma2Lealtad.Models.Type type)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            if (ModelState.IsValid)
            {
                if (db.Types.Count() > 0)
                {
                    type.id = db.Types.Max(c => c.id) + 1;
                }
                else
                {
                    type.id = 1;
                }
                db.Types.Add(type);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(type);
        }

        //
        // GET: /Type/Edit/5

        public ActionResult Edit(int id = 0)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            Suma2Lealtad.Models.Type type = db.Types.Find(id);
            if (type == null)
            {
                return HttpNotFound();
            }
            return View(type);
        }

        //
        // POST: /Type/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Suma2Lealtad.Models.Type type)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            if (ModelState.IsValid)
            {
                db.Entry(type).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(type);
        }

        //
        // GET: /Type/Delete/5

        public ActionResult Delete(int id = 0)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            Suma2Lealtad.Models.Type type = db.Types.Find(id);
            if (type == null)
            {
                return HttpNotFound();
            }
            return View(type);
        }

        //
        // POST: /Type/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            Suma2Lealtad.Models.Type type = db.Types.Find(id);
            db.Types.Remove(type);
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