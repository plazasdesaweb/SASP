using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suma2Lealtad.Models;
using System.ComponentModel.DataAnnotations;

namespace Suma2Lealtad.Controllers
{
    [HandleError]
    public class EstadoController : Controller
    {
        private LealtadEntities db = new LealtadEntities();

        //
        // GET: /Estado/

        public ActionResult Index()
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            return View(db.ESTADOS.OrderBy(x=> x.DESCRIPC_ESTADO).ToList());
        }

        //
        // GET: /Estado/Details/5

        public ActionResult Details(string id = null)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            ESTADO estado = db.ESTADOS.Find(id);
            if (estado == null)
            {
                return HttpNotFound();
            }
            return View(estado);
        }

        //
        // GET: /Estado/Create

        public ActionResult Create()
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            return View();
        }

        //
        // POST: /Estado/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ESTADO estado)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            if (ModelState.IsValid)
            {

                if (db.ESTADOS.Count() > 0)
                {
                    List<string> codigosString = (from e in db.ESTADOS
                                                  select e.COD_ESTADO).ToList();
                    List<int> codigos = new List<int>();
                    foreach (var c in codigosString)
                    {
                        int salida;
                        Int32.TryParse(c, out salida);
                        codigos.Add(salida);
                    }
                    int maximo = codigos.Max();
                    estado.COD_ESTADO = (maximo + 1).ToString();
                }
                else
                {
                    estado.COD_ESTADO = "1";
                }
                db.ESTADOS.Add(estado);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(estado);
        }

        //
        // GET: /Estado/Edit/5

        public ActionResult Edit(string id = null)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            ESTADO estado = db.ESTADOS.Find(id);
            if (estado == null)
            {
                return HttpNotFound();
            }
            return View(estado);
        }

        //
        // POST: /Estado/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ESTADO estado)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            if (ModelState.IsValid)
            {
                db.Entry(estado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(estado);
        }

        //
        // GET: /Estado/Delete/5

        public ActionResult Delete(string id = null)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            ESTADO estado = db.ESTADOS.Find(id);
            if (estado == null)
            {
                return HttpNotFound();
            }
            return View(estado);
        }

        //
        // POST: /Estado/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString();
            ESTADO estado = db.ESTADOS.Find(id);
            db.ESTADOS.Remove(estado);
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