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
    public class MunicipioController : Controller
    {
        private LealtadEntities db = new LealtadEntities();

        //
        // GET: /Municipio/

        public ActionResult Index()
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            return View(db.MUNICIPIOS.OrderBy(x=> x.DESCRIPC_MUNICIPIO).ToList());
        }

        [HttpPost]
        public ActionResult Index(MUNICIPIO municipio)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            List<MUNICIPIO> modelo = db.MUNICIPIOS.Where(c => c.DESCRIPC_MUNICIPIO.Contains(municipio.DESCRIPC_MUNICIPIO)).OrderBy(x => x.DESCRIPC_MUNICIPIO).ToList();
            return View("Index", modelo);
        }

        //
        // GET: /Municipio/Details/5

        public ActionResult Details(string id = null)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            MUNICIPIO municipio = db.MUNICIPIOS.Find(id);
            if (municipio == null)
            {
                return HttpNotFound();
            }
            return View(municipio);
        }

        //
        // GET: /Municipio/Create

        public ActionResult Create()
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            return View();
        }

        //
        // POST: /Municipio/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MUNICIPIO municipio)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            if (ModelState.IsValid)
            {
                if (db.MUNICIPIOS.Count() > 0)
                {
                    List<string> codigosString = (from e in db.MUNICIPIOS
                                                  select e.COD_MUNICIPIO).ToList();
                    List<int> codigos = new List<int>();
                    foreach (var c in codigosString)
                    {
                        int salida;
                        Int32.TryParse(c, out salida);
                        codigos.Add(salida);
                    }
                    int maximo = codigos.Max();
                    municipio.COD_MUNICIPIO = (maximo + 1).ToString();
                }
                else
                {
                    municipio.COD_MUNICIPIO = "1";
                }
                db.MUNICIPIOS.Add(municipio);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(municipio);
        }

        //
        // GET: /Municipio/Edit/5

        public ActionResult Edit(string id = null)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            MUNICIPIO municipio = db.MUNICIPIOS.Find(id);
            if (municipio == null)
            {
                return HttpNotFound();
            }
            return View(municipio);
        }

        //
        // POST: /Municipio/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MUNICIPIO municipio)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            if (ModelState.IsValid)
            {
                db.Entry(municipio).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(municipio);
        }

        //
        // GET: /Municipio/Delete/5

        public ActionResult Delete(string id = null)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            MUNICIPIO municipio = db.MUNICIPIOS.Find(id);
            if (municipio == null)
            {
                return HttpNotFound();
            }
            return View(municipio);
        }

        //
        // POST: /Municipio/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            db.Database.Connection.ConnectionString = Suma2Lealtad.Modules.AppModule.ConnectionString("SumaLealtad");
            MUNICIPIO municipio = db.MUNICIPIOS.Find(id);
            db.MUNICIPIOS.Remove(municipio);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult FilterMunicipio()
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