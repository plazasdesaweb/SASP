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
    [HandleError]
    public class CiudadController : Controller
    {
        private LealtadEntities db = new LealtadEntities();

        //
        // GET: /Ciudad/

        public ActionResult Index()
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            return View(db.CIUDADES.OrderBy(x=> x.DESCRIPC_CIUDAD).ToList());
        }

        [HttpPost]
        public ActionResult Index(CIUDAD ciudad)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            List<CIUDAD> modelo = db.CIUDADES.Where(c => c.DESCRIPC_CIUDAD.Contains(ciudad.DESCRIPC_CIUDAD)).OrderBy(x => x.DESCRIPC_CIUDAD).ToList();
            return View("Index", modelo);
        }

        //
        // GET: /Ciudad/Details/5

        public ActionResult Details(string id = null)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            CIUDAD ciudad = db.CIUDADES.Find(id);
            if (ciudad == null)
            {
                return HttpNotFound();
            }
            return View(ciudad);
        }

        //
        // GET: /Ciudad/Create

        public ActionResult Create()
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            return View();
        }

        //
        // POST: /Ciudad/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CIUDAD ciudad)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            if (ModelState.IsValid)
            {
                if (db.CIUDADES.Count() > 0)
                {
                    List<string> codigosString = (from e in db.CIUDADES
                                                  select e.COD_CIUDAD).ToList();
                    List<int> codigos = new List<int>();
                    foreach (var c in codigosString)
                    {
                        int salida;
                        Int32.TryParse(c, out salida);
                        codigos.Add(salida);
                    }
                    int maximo = codigos.Max();
                    ciudad.COD_CIUDAD = (maximo + 1).ToString();
                }
                else
                {
                    ciudad.COD_CIUDAD = "1";
                }
                db.CIUDADES.Add(ciudad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ciudad);
        }

        //
        // GET: /Ciudad/Edit/5

        public ActionResult Edit(string id = null)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            CIUDAD ciudad = db.CIUDADES.Find(id);
            if (ciudad == null)
            {
                return HttpNotFound();
            }
            return View(ciudad);
        }

        //
        // POST: /Ciudad/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CIUDAD ciudad)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            if (ModelState.IsValid)
            {
                db.Entry(ciudad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ciudad);
        }

        //
        // GET: /Ciudad/Delete/5

        public ActionResult Delete(string id = null)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            CIUDAD ciudad = db.CIUDADES.Find(id);
            if (ciudad == null)
            {
                return HttpNotFound();
            }
            return View(ciudad);
        }

        //
        // POST: /Ciudad/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            CIUDAD ciudad = db.CIUDADES.Find(id);
            db.CIUDADES.Remove(ciudad);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult FilterCiudad()
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}