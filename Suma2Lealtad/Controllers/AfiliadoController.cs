using Suma2Lealtad.Filters;
using Suma2Lealtad.Models;
using Suma2Lealtad.Modules;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Suma2Lealtad.Controllers
{
    [AuditingFilter]
    public class AfiliadoController : Controller
    {
        private AfiliadoRepository repAfiliado = new AfiliadoRepository();

        public ActionResult Filter()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Find(string numdoc)
        {
            //        //Los estados actuales para una persona son:
            //        //NOCLIENTE            (no registrado en WEBPLAZAS)
            //        //NOAFILIADO           (no afiliado en SUMAPLAZAS)
            //        //CLIENTE              (registrado en WEBPLAZAS)
            //        //AFILIADO SUMA        (afiliado en SUMAPLAZAS)
            //        //BENEFCIARIO PREPAGO  (afiliado en SUMAPLAZAS)
            //        //El estado deseado es:
            //        //CLIENTE/AFILIADO SUMA     (registrado en WEBPLAZAS y afiliado en SUMAPLAZAS TIPO SUMA) 
            //        //Existen los siguientes resultados posibles para esta búsqueda
            //        //NOCLIENTE/NOAFILIADO            -> por definir acción para crear registro de CLIENTE y crear afiliación de AFILIADO => Redireccionar a GenericView con mensaje descriptivo
            //        //NOCLIENTE/AFILIADO              -> por definir acción para crear registro de CLIENTE => Redireccionar a GenericView con mensaje descriptivo
            //        //CLIENTE/NOAFILIADO              -> acción: editar registro de CLIENTE y crear afiliación de AFILIADO => CREAR AFILIACION SUMA (retornar vista Create)
            //        //CLIENTE/AFILIADO SUMA           -> acción: editar registro de CLIENTE y editar afiliación de AFILIADO => REVISAR AFILIACION (Redirecciónar a acción Index ó Edit)
            //        //CLIENTE/BENEFCIARIO PREPAGO     -> acción: error cliente prepago => Redireccionar a GenericView con mensaje descriptivo
            
            Afiliado afiliado = repAfiliado.Find(numdoc);
            if (afiliado == null)
            {
                //ERROR EN METODO FIND
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Afiliado / Crear Afiliación / Buscar Cliente";
                viewmodel.Message = "Ha ocurrido un error de aplicación (Find). Notifique al Administrador.";
                viewmodel.ControllerName = "Afiliado";
                viewmodel.ActionName = "Filter";
                return RedirectToAction("GenericView", viewmodel);
            }
            //OJO
            if (afiliado.id == -1)
            {
                //ERROR EN METODO FIND
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Afiliado / Crear Afiliación / Buscar Cliente";
                viewmodel.Message = "Error: El Afiliado ya es beneficiario Prepago.";
                viewmodel.ControllerName = "Afiliado";
                viewmodel.ActionName = "Filter";
                return RedirectToAction("GenericView", viewmodel);
            }
           if (afiliado.clientid == 0 && afiliado.id == 0)
            {
                //NOCLIENTE/NOAFILIADO
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Afiliado / Crear Afiliación / Buscar Cliente";
                viewmodel.Message = "El Cliente no esta registrado en WebPlazas. (Escenario NOCLIENTE/NOAFILIADO)";
                viewmodel.ControllerName = "Afiliado";
                viewmodel.ActionName = "Filter";
                return RedirectToAction("GenericView", viewmodel);
            }
            else if (afiliado.clientid == 0 && afiliado.id != 0)
            {
                //NOCLIENTE/AFILIADO
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Afiliado / Crear Afiliación / Buscar Cliente";
                viewmodel.Message = "El Cliente no esta registrado en WebPlazas. (Escenario NOCLIENTE/AFILIADO)";
                viewmodel.ControllerName = "Afiliado";
                viewmodel.ActionName = "Filter";
                return RedirectToAction("GenericView", viewmodel);
            }
            else if (afiliado.clientid != 0 && afiliado.id == 0)
            {
                //CLIENTE/NOAFILIADO
                return View("Create", afiliado);
            }
            else
            {
                //CLIENTE/AFILIADO
                List<Afiliado> afiliados = repAfiliado.Find(numdoc, "", "");
                return View("Index", afiliados);
            }
        }

        public ActionResult Create(string numdoc, int typeid, int companyid)
        {
            Afiliado afiliado = repAfiliado.Find(numdoc, typeid, companyid);
            return View("Create", afiliado);
        }

        [HttpPost]
        public ActionResult Create(Afiliado afiliado, HttpPostedFileBase file)
        {
            ViewModel viewmodel = new ViewModel();
            if (repAfiliado.Save(afiliado, file))
            {
                #region validacion de carga de archivo 
                //if (file != null && file.ContentLength > 0)
                //    try
                //    {
                //        string path = Server.MapPath(AppModule.GetPathPicture().Replace("@filename@", afiliado.docnumber));
                //        file.SaveAs(path);
                //    }
                //    catch (Exception ex)
                //    {
                //        viewmodel.Title = "Afiliado / Crear Afiliación";
                //        viewmodel.Message = "Error de aplicacion: No se pudo cargar archivo (" + ex.Message + ")";
                //        viewmodel.ControllerName = "Afiliado";
                //        viewmodel.ActionName = "Filter";
                //        return RedirectToAction("GenericView", viewmodel);
                //    }
                //else
                //{
                //    viewmodel.Title = "Afiliado / Crear Afiliación";
                //    viewmodel.Message = "Error de aplicacion: No se pudo cargar archivo (archivo inválido)";
                //    viewmodel.ControllerName = "Afiliado";
                //    viewmodel.ActionName = "Filter";
                //    return RedirectToAction("GenericView", viewmodel);
                //}
                #endregion
                viewmodel.Title = "Afiliado / Crear Afiliación";
                viewmodel.Message = "Solicitud de afiliación creada exitosamente.";
                viewmodel.ControllerName = "Afiliado";
                viewmodel.ActionName = "Index";
                viewmodel.RouteValues = afiliado.docnumber;
            }
            else
            {
                viewmodel.Title = "Afiliado / Crear Afiliación";
                viewmodel.Message = "Error de aplicacion: No se pudo crear solicitud de afiliación.";
                viewmodel.ControllerName = "Afiliado";
                viewmodel.ActionName = "Filter";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult FilterReview()
        {
            return View();
        }

        public ActionResult Index(string numdoc)
        {
            List<Afiliado> afiliados = repAfiliado.Find(numdoc, "", "");
            return View(afiliados);
        }

        [HttpPost]
        public ActionResult Index(string numdoc, string name, string email)
        {
            List<Afiliado> afiliados = repAfiliado.Find(numdoc, name, email);
            if (afiliados.Count == 0)
            {
                //NOCLIENTE/NOAFILIADO
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Afiliado / Revisar Afiliación / Buscar Afiliado";
                viewmodel.Message = "No se ha(n) encontrado el(los) Afiliado(s) con los datos suministrados";
                viewmodel.ControllerName = "Afiliado";
                viewmodel.ActionName = "FilterReview";
                return RedirectToAction("GenericView", viewmodel);
            }
            return View(afiliados);
        }

        public ActionResult Edit(int id)
        {
            Afiliado afiliado = repAfiliado.Find(id);
            return View(afiliado);
        }

        [HttpPost]
        public ActionResult Edit(Afiliado afiliado)
        {
            ViewModel viewmodel = new ViewModel();
            if (!repAfiliado.SaveChanges(afiliado))
            {
                viewmodel.Title = "Afiliado / Revisar Afiliación";
                viewmodel.Message = "Existen campos que son requeridos para procesar el formulario.";
                viewmodel.ControllerName = "Afiliado";
                viewmodel.ActionName = "Index";
                viewmodel.RouteValues = afiliado.docnumber;
            }
            else
            {
                viewmodel.Title = "Afiliado / Revisar Afiliación";
                viewmodel.Message = "La información del afiliado ha sido actualizada satisfactoriamente.";
                viewmodel.ControllerName = "Afiliado";
                viewmodel.ActionName = "Index";
                viewmodel.RouteValues = afiliado.docnumber;
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public FileContentResult GetImage(int id)
        {
            Photos_Affiliate Photo = repAfiliado.Find(id).picture;
            if (Photo.photo != null)
            {
                return File(Photo.photo, Photo.photo_type);
            }
            else
            {
                return null;
            }
        }

        public ActionResult Aprobar(int id)
        {
            ViewModel viewmodel = new ViewModel();
            Afiliado afiliado = repAfiliado.Find(id);
            if (repAfiliado.Aprobar(afiliado))
            {
                viewmodel.Title = "Afiliado / Aprobar Afiliación:";
                viewmodel.Message = "Afiliación Aprobada.";
                viewmodel.ControllerName = "Afiliado";
                viewmodel.ActionName = "Index";
                viewmodel.RouteValues = afiliado.docnumber;
            }
            else
            {
                viewmodel.Title = "Afiliado / Aprobar Afiliación:";
                viewmodel.Message = "Aprobación Fallida.";
                viewmodel.ControllerName = "Afiliado";
                viewmodel.ActionName = "Index";
                viewmodel.RouteValues = afiliado.docnumber;
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult CrearPin(int id)
        {
            Afiliado afiliado = repAfiliado.Find(id);
            return View("PinPadCrearPin", afiliado);
        }

        public ActionResult CambiarPin(int id)
        {
            Afiliado afiliado = repAfiliado.Find(id);
            return View("PinPadCambiarPin", afiliado);
        }

        public ActionResult ReiniciarPin(int id)
        {
            Afiliado afiliado = repAfiliado.Find(id);
            return View("PinPadReiniciarPin", afiliado);
        }

        public ActionResult ImprimirTarjeta(int id)
        {
            Afiliado afiliado = repAfiliado.Find(id);
            return View("ImpresoraImprimirTarjeta", afiliado);
        }

        [HttpPost]
        public ActionResult ImprimirTarjeta(int id, string mode = "post")
        {
            ViewModel viewmodel = new ViewModel();
            Afiliado afiliado = repAfiliado.Find(id);
            afiliado.trackI = Tarjeta.ConstruirTrackI(afiliado.pan);
            afiliado.trackII = Tarjeta.ConstruirTrackII(afiliado.pan);
            if (repAfiliado.ImprimirTarjeta(afiliado))
            {
                viewmodel.Title = "Afiliado / Operaciones con la Impresora / Imprimir Tarjeta";
                viewmodel.Message = "Tarjeta impresa y activada correctamente";
                viewmodel.ControllerName = "Afiliado";
                viewmodel.ActionName = "Index";
                viewmodel.RouteValues = afiliado.docnumber;
            }
            else
            {
                viewmodel.Title = "Afiliado / Operaciones con la Impresora / Imprimir Tarjeta";
                viewmodel.Message = "Falló el proceso de impresión y activación de la Tarjeta";
                viewmodel.ControllerName = "Afiliado";
                viewmodel.ActionName = "Index";
                viewmodel.RouteValues = afiliado.docnumber;
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult BloquearTarjeta(int id)
        {
            Afiliado afiliado = repAfiliado.Find(id);
            return View(afiliado);
        }

        [HttpPost]
        public ActionResult BloquearTarjeta(int id, string mode = "post")
        {
            ViewModel viewmodel = new ViewModel();
            Afiliado afiliado = repAfiliado.Find(id);
            if (repAfiliado.BloquearTarjeta(afiliado))
            {
                viewmodel.Title = "Afiliado / Bloquear Tarjeta";
                viewmodel.Message = "Tarjeta bloqueada correctamente";
                viewmodel.ControllerName = "Afiliado";
                viewmodel.ActionName = "Index";
                viewmodel.RouteValues = afiliado.docnumber;
            }
            else
            {
                viewmodel.Title = "Afiliado / Bloquear Tarjeta";
                viewmodel.Message = "Falló el proceso de bloqueo de la Tarjeta";
                viewmodel.ControllerName = "Afiliado";
                viewmodel.ActionName = "Index";
                viewmodel.RouteValues = afiliado.docnumber;
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult SuspenderTarjeta(int id)
        {
            Afiliado afiliado = repAfiliado.Find(id);
            return View(afiliado);
        }

        [HttpPost]
        public ActionResult SuspenderTarjeta(int id, string mode = "post")
        {
            ViewModel viewmodel = new ViewModel();
            Afiliado afiliado = repAfiliado.Find(id);
            if (repAfiliado.SuspenderTarjeta(afiliado))
            {
                viewmodel.Title = "Afiliado / Suspender Tarjeta";
                viewmodel.Message = "Tarjeta suspendida correctamente";
                viewmodel.ControllerName = "Afiliado";
                viewmodel.ActionName = "Index";
                viewmodel.RouteValues = afiliado.docnumber;
            }
            else
            {
                viewmodel.Title = "Afiliado / Suspender Tarjeta";
                viewmodel.Message = "Falló el proceso de suspensión de la Tarjeta";
                viewmodel.ControllerName = "Afiliado";
                viewmodel.ActionName = "Index";
                viewmodel.RouteValues = afiliado.docnumber;
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult ReactivarTarjeta(int id)
        {
            Afiliado afiliado = repAfiliado.Find(id);
            return View(afiliado);
        }

        [HttpPost]
        public ActionResult ReactivarTarjeta(int id, string mode = "post")
        {
            ViewModel viewmodel = new ViewModel();
            Afiliado afiliado = repAfiliado.Find(id);
            if (repAfiliado.ReactivarTarjeta(afiliado))
            {
                viewmodel.Title = "Afiliado / Reactivar Tarjeta";
                viewmodel.Message = "Tarjeta reactivada correctamente";
                viewmodel.ControllerName = "Afiliado";
                viewmodel.ActionName = "Index";
                viewmodel.RouteValues = afiliado.docnumber;
            }
            else
            {
                viewmodel.Title = "Afiliado / Reactivar Tarjeta";
                viewmodel.Message = "Falló el proceso de reactivación de la Tarjeta";
                viewmodel.ControllerName = "Afiliado";
                viewmodel.ActionName = "Index";
                viewmodel.RouteValues = afiliado.docnumber;
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult SaldosMovimientos(int id)
        {
            Afiliado afiliado = repAfiliado.Find(id);
            SaldosMovimientos SaldosMovimientos = repAfiliado.FindSaldosMovimientos(afiliado);
            if (SaldosMovimientos == null)
            {
                //ERROR EN METODO FIND
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Afiliado / Saldos y Movimientos";
                viewmodel.Message = "Ha ocurrido un error de aplicación (FindSaldosMovimientos). Notifique al Administrador.";
                viewmodel.ControllerName = "Afiliado";
                viewmodel.ActionName = "Filter";
                return RedirectToAction("GenericView", viewmodel);
            }
            AfiliadoSaldosMovimientos AfiliadoSaldosMovimientos = new AfiliadoSaldosMovimientos()
            {
                denominacionPrepago = "Bs.",
                denominacionSuma = "Más.",
                Afiliado = afiliado,
                SaldosMovimientos = SaldosMovimientos
            };
            return View(AfiliadoSaldosMovimientos);
        }

        public ActionResult Acreditar(int id)
        {
            Afiliado afiliado = repAfiliado.Find(id);
            return View(afiliado);
        }

        [HttpPost]
        public ActionResult Acreditar(int id, string monto)
        {
            ViewModel viewmodel = new ViewModel();
            Afiliado afiliado = repAfiliado.Find(id);
            if (repAfiliado.Acreditar(afiliado, monto))
            {
                viewmodel.Title = "Afiliado / Acreditar";
                viewmodel.Message = "Acreditación exitosa";
                viewmodel.ControllerName = "Afiliado";
                viewmodel.ActionName = "Index";
                viewmodel.RouteValues = afiliado.docnumber;
            }
            else
            {
                viewmodel.Title = "Afiliado / Acreditar ";
                viewmodel.Message = "Falló el proceso de acreditación";
                viewmodel.ControllerName = "Afiliado";
                viewmodel.ActionName = "Index";
                viewmodel.RouteValues = afiliado.docnumber;
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult GenericView(ViewModel viewmodel)
        {
            return View(viewmodel);
        }

        public JsonResult CiudadList(string id)
        {
            List<CIUDAD> ciudades = repAfiliado.GetCiudades(id);

            return Json(new SelectList(ciudades, "COD_CIUDAD", "DESCRIPC_CIUDAD"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult MunicipioList(string id)
        {
            List<MUNICIPIO> municipios = repAfiliado.GetMunicipios(id);

            return Json(new SelectList(municipios, "COD_MUNICIPIO", "DESCRIPC_MUNICIPIO"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ParroquiaList(string id)
        {
            List<PARROQUIA> parroquias = repAfiliado.GetParroquias(id);

            return Json(new SelectList(parroquias, "COD_PARROQUIA", "DESCRIPC_PARROQUIA"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult UrbanizacionList(string id)
        {
            List<URBANIZACION> urb = repAfiliado.GetUrbanizaciones(id);

            return Json(new SelectList(urb, "COD_URBANIZACION", "DESCRIPC_URBANIZACION"), JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public class Galaxy
        {
            public string Name { get; set; }
            public int MegaLightYears { get; set; }
        }

    }
}