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
    public class ParroquiaController : Controller
    {
        private LealtadEntities db = new LealtadEntities();

        //
        // GET: /Parroquia/

        public ActionResult Index()
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            return View(db.PARROQUIAS.OrderBy(x=> x.DESCRIPC_PARROQUIA).ToList());
        }

        [HttpPost]
        public ActionResult Index(PARROQUIA parroquia)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            List<PARROQUIA> modelo = db.PARROQUIAS.Where(c => c.DESCRIPC_PARROQUIA.Contains(parroquia.DESCRIPC_PARROQUIA)).OrderBy(x => x.DESCRIPC_PARROQUIA).ToList();

            return View("Index", modelo);
        }

        //
        // GET: /Parroquia/Details/5

        public ActionResult Details(string id = null)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            PARROQUIA parroquia = db.PARROQUIAS.Find(id);
            if (parroquia == null)
            {
                return HttpNotFound();
            }
            return View(parroquia);
        }

        //
        // GET: /Parroquia/Create

        public ActionResult Create()
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            return View();
        }

        //
        // POST: /Parroquia/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PARROQUIA parroquia)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            if (ModelState.IsValid)
            {
                if (db.PARROQUIAS.Count() > 0)
                {
                    List<string> codigosString = (from e in db.PARROQUIAS
                                                  select e.COD_PARROQUIA).ToList();
                    List<int> codigos = new List<int>();
                    foreach (var c in codigosString)
                    {
                        int salida;
                        Int32.TryParse(c, out salida);
                        codigos.Add(salida);
                    }
                    int maximo = codigos.Max();
                    parroquia.COD_PARROQUIA = (maximo + 1).ToString();
                }
                else
                {
                    parroquia.COD_PARROQUIA = "1";
                }
                db.PARROQUIAS.Add(parroquia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(parroquia);
        }

        //
        // GET: /Parroquia/Edit/5

        public ActionResult Edit(string id = null)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            PARROQUIA parroquia = db.PARROQUIAS.Find(id);
            if (parroquia == null)
            {
                return HttpNotFound();
            }
            return View(parroquia);
        }

        //
        // POST: /Parroquia/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PARROQUIA parroquia)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            if (ModelState.IsValid)
            {
                db.Entry(parroquia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(parroquia);
        }

        //
        // GET: /Parroquia/Delete/5

        public ActionResult Delete(string id = null)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            PARROQUIA parroquia = db.PARROQUIAS.Find(id);
            if (parroquia == null)
            {
                return HttpNotFound();
            }
            return View(parroquia);
        }

        //
        // POST: /Parroquia/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            PARROQUIA parroquia = db.PARROQUIAS.Find(id);
            db.PARROQUIAS.Remove(parroquia);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult FilterParroquia()
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