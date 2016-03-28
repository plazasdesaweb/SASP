using LoginFormApp.Models;
using Suma2Lealtad.Filters;
using Suma2Lealtad.Models;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Suma2Lealtad.Controllers
{
    [AuditingFilter]
    [HandleError]
    public class UserController : Controller
    {
        private LealtadEntities db = new LealtadEntities();

        //
        // GET: /User/

        public ActionResult Index()
        {
            var q = (from u in db.Users
                     join st in db.SumaStatuses on u.sumastatusid equals st.id
                     select new Usuario()
                     {
                         id = u.id,
                         login = u.login,
                         passw = u.passw,
                         firstname = u.firstname,
                         lastname = u.lastname,
                         email = u.email,
                         status = st.name
                     });

            return View(q.ToList());
        }
        //
        // GET: /User/Details/5

        public ActionResult Details(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // GET: /User/CreateRoles/5

        public ActionResult CreateRoles(UserRols UserRoles)
        {
            UserRoles.Update();
            return RedirectToAction("Index");
        }

        //
        // GET: /User/Roles/5

        public ActionResult Roles(int id = 0)
        {
            UserRols UserRoles = new UserRols(id);
            return View(UserRoles);
        }

        //
        // GET: /User/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /User/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                //Agregue esto pq esta dando errores la inserción del valor que viene en null desde la vista
                user.sumastatusid = 1;
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        //
        // GET: /User/Edit/5

        public ActionResult Edit(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        //
        // GET: /User/Delete/5

        public ActionResult Delete(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /User/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
           User user = db.Users.Find(id);

             foreach (var m in db.UserRols.Where(m => m.userid == id))
             {
             db.UserRols.Remove(m);
             }

                db.SaveChanges();
                db.Users.Remove(user);
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