using Suma2Lealtad.Filters;
using Suma2Lealtad.Models;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Suma2Lealtad.Controllers
{
    [AuditingFilter]
    [HandleError]
    public class RoleController : Controller
    {
        private LealtadEntities db = new LealtadEntities();

        public ActionResult CreateRoles(RoleMenus RoleMenus)
        {
            RoleMenus.Update();
            return RedirectToAction("Index");
        }


        //
        // GET: /Role/

        public ActionResult Index()
        {
            return View(db.Roles.ToList());
        }

        //
        // GET: /Role/Details/5

        public ActionResult Details(int id = 0)
        {
            Role role = db.Roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        //
        // GET: /User/Roles/5

        public ActionResult Menus(int id = 0)
        {
            RoleMenus RoleMenus = new RoleMenus(id);
            return View(RoleMenus);
        }

        //
        // GET: /Role/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Role/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Role role)
        {
            if (ModelState.IsValid)
            {
                db.Roles.Add(role);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(role);
        }

        //
        // GET: /Role/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Role role = db.Roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        //
        // POST: /Role/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Role role)
        {
            if (ModelState.IsValid)
            {
                db.Entry(role).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(role);
        }

        //
        // GET: /Role/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Role role = db.Roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        //
        // POST: /Role/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserRol userrol = db.UserRols.Find(id);

            if (userrol == null)
            {
                return RedirectToAction("Index");
            }

            Role role = db.Roles.Find(id);
            db.Roles.Remove(role);
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