using Suma2Lealtad.Filters;
using Suma2Lealtad.Models;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Suma2Lealtad.Controllers
{
    [AuditingFilter]
    [HandleError]
    public class MenuController : Controller
    {
        private LealtadEntities db = new LealtadEntities();

        //
        // GET: /Menu/

        public ActionResult Index()
        {
            return View(db.Menus.ToList());
        }

        //
        // GET: /Menu/Details/5

        public ActionResult Details(int id = 0)
        {
            Menu menu = db.Menus.Find(id);
            if (menu == null)
            {
                return HttpNotFound();
            }
            return View(menu);
        }

        //
        // GET: /Menu/Menus/5

        public ActionResult Menus(int id = 0)
        {
            RoleMenus RoleMenus = new RoleMenus(id);
            return View(RoleMenus);
        }


        //
        // GET: /Menu/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Menu/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Menu menu)
        {
            if (ModelState.IsValid)
            {
                menu.id = db.Channels.Max(c => c.id) + 1;
                db.Menus.Add(menu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(menu);
        }

        //
        // GET: /Menu/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Menu menu = db.Menus.Find(id);
            if (menu == null)
            {
                return HttpNotFound();
            }
            return View(menu);
        }

        //
        // POST: /Menu/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Menu menu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(menu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(menu);
        }

        //
        // GET: /Menu/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Menu menu = db.Menus.Find(id);
            if (menu == null)
            {
                return HttpNotFound();
            }
            return View(menu);
        }

        //
        // POST: /Menu/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Menu menu = db.Menus.Find(id);
            db.Menus.Remove(menu);
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