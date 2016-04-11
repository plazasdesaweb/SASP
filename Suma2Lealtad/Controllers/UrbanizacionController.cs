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
    public class UrbanizacionController : Controller
    {
        private LealtadEntities db = new LealtadEntities();

        //
        // GET: /Urbanizacion/

        public ActionResult Index()
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            return View(db.URBANIZACIONES.OrderBy(x=> x.DESCRIPC_URBANIZACION).ToList());
        }

        [HttpPost]
        public ActionResult Index(URBANIZACION urbanizacion)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            List<URBANIZACION> modelo = db.URBANIZACIONES.Where(c => c.DESCRIPC_URBANIZACION.Contains(urbanizacion.DESCRIPC_URBANIZACION)).OrderBy(x => x.DESCRIPC_URBANIZACION).ToList();
            return View("Index", modelo);
        }

        //
        // GET: /Urbanizacion/Details/5

        public ActionResult Details(string id = null)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            URBANIZACION urbanizacion = db.URBANIZACIONES.Find(id);
            if (urbanizacion == null)
            {
                return HttpNotFound();
            }
            return View(urbanizacion);
        }

        //
        // GET: /Urbanizacion/Create

        public ActionResult Create()
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            return View();
        }

        //
        // POST: /Urbanizacion/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(URBANIZACION urbanizacion)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            if (ModelState.IsValid)
            {
                if (db.URBANIZACIONES.Count() > 0)
                {
                    List<string> codigosString = (from e in db.URBANIZACIONES
                                                  select e.COD_URBANIZACION).ToList();
                    List<int> codigos = new List<int>();
                    foreach (var c in codigosString)
                    {
                        int salida;
                        Int32.TryParse(c, out salida);
                        codigos.Add(salida);
                    }
                    int maximo = codigos.Max();
                    urbanizacion.COD_URBANIZACION = (maximo + 1).ToString();
                }
                else
                {
                    urbanizacion.COD_URBANIZACION = "1";
                }
                db.URBANIZACIONES.Add(urbanizacion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(urbanizacion);
        }

        //
        // GET: /Urbanizacion/Edit/5

        public ActionResult Edit(string id = null)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            URBANIZACION urbanizacion = db.URBANIZACIONES.Find(id);
            if (urbanizacion == null)
            {
                return HttpNotFound();
            }
            return View(urbanizacion);
        }

        //
        // POST: /Urbanizacion/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(URBANIZACION urbanizacion)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            if (ModelState.IsValid)
            {
                db.Entry(urbanizacion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(urbanizacion);
        }

        //
        // GET: /Urbanizacion/Delete/5

        public ActionResult Delete(string id = null)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            URBANIZACION urbanizacion = db.URBANIZACIONES.Find(id);
            if (urbanizacion == null)
            {
                return HttpNotFound();
            }
            return View(urbanizacion);
        }

        //
        // POST: /Urbanizacion/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            URBANIZACION urbanizacion = db.URBANIZACIONES.Find(id);
            db.URBANIZACIONES.Remove(urbanizacion);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult FilterUrbanizacion()
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}