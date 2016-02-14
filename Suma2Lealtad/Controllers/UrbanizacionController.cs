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
            return View(db.URBANIZACIONES.OrderBy(x=> x.DESCRIPC_URBANIZACION).ToList());
        }

        [HttpPost]
        public ActionResult Index(URBANIZACION urbanizacion)
        {
            List<URBANIZACION> modelo = db.URBANIZACIONES.Where(c => c.DESCRIPC_URBANIZACION.Contains(urbanizacion.DESCRIPC_URBANIZACION)).OrderBy(x => x.DESCRIPC_URBANIZACION).ToList();

            return View("Index", modelo);
        }

        //
        // GET: /Urbanizacion/Details/5

        public ActionResult Details(string id = null)
        {
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
            return View();
        }

        //
        // POST: /Urbanizacion/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(URBANIZACION urbanizacion)
        {
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
            URBANIZACION urbanizacion = db.URBANIZACIONES.Find(id);
            db.URBANIZACIONES.Remove(urbanizacion);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult FilterUrbanizacion()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}