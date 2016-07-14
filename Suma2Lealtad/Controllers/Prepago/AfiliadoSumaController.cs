using Suma2Lealtad.Filters;
using Suma2Lealtad.Models;
using Suma2Lealtad.Modules;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Suma2Lealtad.Controllers.Prepago
{
    [AuditingFilter]
    [HandleError]
    public class AfiliadoSumaController : Controller
    {
        private AfiliadoSumaRepository repAfiliado = new AfiliadoSumaRepository();

        public ActionResult Filter()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Filter(string numdoc)
        {
            AfiliadoSuma afiliado = repAfiliado.Find(numdoc);
            if (afiliado.id == 0)
            {
                //NO ESTA EN SUMA

                //VERIFICO QUE EL NUMERO DE DOCUMENTO NO EXISTA PARA PODER REGISTRARLO
                string validacion = repAfiliado.VerificarNumeroDeDocumentoCrear(numdoc.Substring(2));
                if (validacion != null)
                {
                    ViewModel viewmodel = new ViewModel();
                    viewmodel.Title = "Afiliado / Crear Afiliación";
                    viewmodel.Message = "El número de documento indicado (" + numdoc + ") ya está registrado como otro tipo de identificación (" + validacion + "), no se puede afiliar.";
                    viewmodel.ControllerName = "AfiliadoSuma";
                    viewmodel.ActionName = "Filter";
                    return RedirectToAction("GenericView", viewmodel);
                }

                afiliado.typeid = Globals.ID_TYPE_SUMA;
                afiliado.type = "Suma";
                //CARGO VALOR POR DEFECTO EN LISTA DE ESTADOS
                afiliado.ListaEstados.Insert(0, new ESTADO { COD_ESTADO = " ", DESCRIPC_ESTADO = "Seleccione un Estado" });
                //verifico si tiene tarjeta en Cards
                if (afiliado.pan != "0" && afiliado.estatustarjeta == "Activa")
                {
                    ViewModel viewmodel = new ViewModel();
                    viewmodel.Title = "Afiliado / Crear Afiliación";
                    viewmodel.Message = "El número de documento indicado ya posee una Tarjeta Activa con el número " + afiliado.pan;
                    viewmodel.ControllerName = "AfiliadoSuma";
                    viewmodel.ActionName = "CreateConTarjeta";
                    viewmodel.RouteValues = "?numdoc=" + numdoc;
                    return RedirectToAction("GenericView", viewmodel);
                }
                else
                {
                    return View("Create", afiliado);
                }
            }
            else
            {
                //ESTA EN SUMA
                afiliado = repAfiliado.Find(afiliado.id);
                //SI ESTA ELIMINADO, NO SE PUEDE HACER OPERACIONES, SE MUESTRA MENSAJE
                if (afiliado.estatus == "Eliminada")
                {
                    ViewModel viewmodel = new ViewModel();
                    viewmodel.Title = "Afiliado / Crear Afiliación";
                    viewmodel.Message = "El número de documento indicado posee un Afiliación Eliminada. No puede realizar operaciones.";
                    viewmodel.ControllerName = "AfiliadoSuma";
                    viewmodel.ActionName = "Filter";
                    return RedirectToAction("GenericView", viewmodel);
                }
                else
                {
                    return View("Edit", afiliado);
                }
            }
        }

        [HttpPost]
        public ActionResult Create(AfiliadoSuma afiliado, HttpPostedFileBase file)
        {
            ViewModel viewmodel = new ViewModel();
            if (repAfiliado.Save(afiliado, file))
            {
                viewmodel.Title = "Afiliado / Crear Afiliación";
                viewmodel.Message = "Solicitud de afiliación creada exitosamente.";
                viewmodel.ControllerName = "AfiliadoSuma";
                viewmodel.ActionName = "FilterReview";
            }
            else
            {
                viewmodel.Title = "Afiliado / Crear Afiliación";
                viewmodel.Message = "Error de aplicacion: No se pudo crear solicitud de afiliación.";
                viewmodel.ControllerName = "AfiliadoSuma";
                viewmodel.ActionName = "FilterReview";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult CreateConTarjeta(string numdoc)
        {
            AfiliadoSuma afiliado = repAfiliado.Find(numdoc);
            //NO ESTA EN SUMA
            afiliado.typeid = Globals.ID_TYPE_SUMA;
            afiliado.type = "Suma";
            //CARGO VALOR POR DEFECTO EN LISTA DE ESTADOS
            afiliado.ListaEstados.Insert(0, new ESTADO { COD_ESTADO = " ", DESCRIPC_ESTADO = "Seleccione un Estado" });
            //verifico si tiene tarjeta en Cards
            return View("Create", afiliado);
        }

        [HttpPost]
        public ActionResult CreateConTarjeta(AfiliadoSuma afiliado, HttpPostedFileBase file)
        {
            ViewModel viewmodel = new ViewModel();
            if (repAfiliado.Save(afiliado, file))
            {
                int idafiliado = repAfiliado.Find(afiliado.docnumber, "", "", "", "").First().id;
                afiliado = repAfiliado.Find(idafiliado);
                if (repAfiliado.Aprobar(afiliado))
                {
                    viewmodel.Title = "Afiliado / Crear Afiliación";
                    viewmodel.Message = "Solicitud de afiliación creada y aprobada exitosamente.";
                    viewmodel.ControllerName = "AfiliadoSuma";
                    viewmodel.ActionName = "FilterReview";
                }
                else
                {
                    viewmodel.Title = "Afiliado / Crear Afiliación";
                    viewmodel.Message = "Solicitud de afiliación creada, pero no se pudo aprobar automáticamente.";
                    viewmodel.ControllerName = "AfiliadoSuma";
                    viewmodel.ActionName = "FilterReview";
                }
            }
            else
            {
                viewmodel.Title = "Afiliado / Crear Afiliación";
                viewmodel.Message = "Error de aplicacion: No se pudo crear solicitud de afiliación.";
                viewmodel.ControllerName = "AfiliadoSuma";
                viewmodel.ActionName = "FilterReview";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult FilterReview()
        {
            AfiliadoSuma afiliado = new AfiliadoSuma();
            return View(afiliado);
        }

        [HttpPost]
        public ActionResult FilterReview(string numdoc, string name, string email, string estadoAfiliacion, string estadoTarjeta)
        {
            List<AfiliadoSumaIndex> afiliados = repAfiliado.Find(numdoc, name, email, estadoAfiliacion, estadoTarjeta);
            return View("Index", afiliados);
        }

        public ActionResult Edit(int id)
        {
            AfiliadoSuma afiliado = repAfiliado.Find(id);
            ////SI ESTA ELIMINADO, NO SE PUEDE HACER OPERACIONES, SE MUESTRA MENSAJE
            //if (afiliado.estatus == "Eliminada")
            //{
            //    ViewModel viewmodel = new ViewModel();
            //    viewmodel.Title = "Afiliado / Revisar Afiliación";
            //    viewmodel.Message = "El número de documento indicado posee un Afiliación Eliminada. No puede realizar operaciones.";
            //    viewmodel.ControllerName = "AfiliadoSuma";
            //    viewmodel.ActionName = "FilterReview";
            //    return RedirectToAction("GenericView", viewmodel);
            //}
            //else
            //{
                return View(afiliado);
            //}
        }

        [HttpPost]
        public ActionResult Edit(AfiliadoSuma afiliado, HttpPostedFileBase fileNoValidado)
        {
            ViewModel viewmodel = new ViewModel();
            if (!repAfiliado.SaveChanges(afiliado, fileNoValidado))
            {
                viewmodel.Title = "Afiliado / Revisar Afiliación";
                viewmodel.Message = "Error de aplicacion: No se pudo actualizar afiliación.";
                viewmodel.ControllerName = "AfiliadoSuma";
                viewmodel.ActionName = "FilterReview";
            }
            else
            {
                viewmodel.Title = "Afiliado / Revisar Afiliación";
                viewmodel.Message = "La información del afiliado ha sido actualizada satisfactoriamente.";
                viewmodel.ControllerName = "AfiliadoSuma";
                viewmodel.ActionName = "FilterReview";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public FileContentResult GetImage(int id)
        {
            Photos_Affiliate Photo = repAfiliado.Find(id).picture;
            if (Photo != null)
            {
                return File(Photo.photo, Photo.photo_type);
            }
            else
            {
                return null;
            }
        }

        public ActionResult CambiarTipoDocumento(int id)
        {
            AfiliadoSuma afiliado = repAfiliado.Find(id);
            if (afiliado.docnumber.Substring(0, 1) == "V")
            {
                ViewData["TipoDocumentoOptions"] = new List<SelectListItem>()
                {      
                    new SelectListItem{ Text="Cédula de Extranjero", Value="E"},
                    new SelectListItem{ Text="Pasaporte", Value="P"}
                };
            }
            else if (afiliado.docnumber.Substring(0, 1) == "E")
            {
                ViewData["TipoDocumentoOptions"] = new List<SelectListItem>()
                {           
                    new SelectListItem{ Text="Cédula de Venezolano", Value="V"},
                    new SelectListItem{ Text="Pasaporte", Value="P"}
                };
            }
            else if (afiliado.docnumber.Substring(0, 1) == "P")
            {
                ViewData["TipoDocumentoOptions"] = new List<SelectListItem>()
                {           
                    new SelectListItem{ Text="Cédula de Venezolano", Value="V"},
                    new SelectListItem{ Text="Cédula de Extranjero", Value="E"}
                };
            }
            return View(afiliado);
        }

        [HttpPost]
        public ActionResult CambiarTipoDocumento(int id, string tipoDocumento)
        {
            AfiliadoSuma afiliado = repAfiliado.Find(id);

            //VERIFICO QUE EL NUMERO DE DOCUMENTO NO EXISTA PARA PODER REGISTRARLO
            string validacion = repAfiliado.VerificarNumeroDeDocumentoCambiarTipo(tipoDocumento + afiliado.docnumber.Substring(1));
            if (validacion != null)
            {
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Afiliado / Revisar Afiliación / Cambiar Tipo de Documento";
                viewmodel.Message = "La combinación Tipo de Documento-Número de Documento (" + tipoDocumento + afiliado.docnumber.Substring(1) + ") ya está registrada, no se puede realizar el cambio de Tipo de Documento.";
                viewmodel.ControllerName = "AfiliadoSuma";
                viewmodel.ActionName = "FilterReview";
                return RedirectToAction("GenericView", viewmodel);
            }
            
            if (repAfiliado.CambiarTipoDocumento(afiliado, tipoDocumento))
            {
                return RedirectToAction("Edit", new { id = id });
            }
            else
            {
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Afiliado / Revisar Afiliación / Cambiar Tipo de Documento";
                viewmodel.Message = "Error de aplicacion: No se pudo cambiar tipo de documento.";
                viewmodel.ControllerName = "AfiliadoSuma";
                viewmodel.ActionName = "FilterReview";
                return RedirectToAction("GenericView", viewmodel);
            }
        }

        public ActionResult Aprobar(int id)
        {
            ViewModel viewmodel = new ViewModel();
            AfiliadoSuma afiliado = repAfiliado.Find(id);
            if (repAfiliado.Aprobar(afiliado))
            {
                viewmodel.Title = "Afiliado / Aprobar Afiliación:";
                viewmodel.Message = "Afiliación Aprobada.";
                viewmodel.ControllerName = "AfiliadoSuma";
                viewmodel.ActionName = "FilterReview";
            }
            else
            {
                viewmodel.Title = "Afiliado / Aprobar Afiliación:";
                viewmodel.Message = "Error de aplicacion: No se pudo aprobar afiliación.";
                viewmodel.ControllerName = "AfiliadoSuma";
                viewmodel.ActionName = "FilterReview";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult CrearPin(int id)
        {
            AfiliadoSuma afiliado = repAfiliado.Find(id);
            return View("PinPadCrearPin", afiliado);
        }

        public ActionResult CambiarPin(int id)
        {
            AfiliadoSuma afiliado = repAfiliado.Find(id);
            return View("PinPadCambiarPin", afiliado);
        }

        public ActionResult ReiniciarPin(int id)
        {
            AfiliadoSuma afiliado = repAfiliado.Find(id);
            return View("PinPadReiniciarPin", afiliado);
        }

        public ActionResult ImprimirTarjeta(int id)
        {
            AfiliadoSuma afiliado = repAfiliado.Find(id);
            return View("ImpresoraImprimirTarjeta", afiliado);
        }

        public ActionResult ReImprimirTarjeta(int id)
        {
            ViewModel viewmodel = new ViewModel();
            AfiliadoSuma afiliado = repAfiliado.Find(id);
            if (repAfiliado.BloquearTarjeta(afiliado))
            {
                return View("ImpresoraImprimirTarjeta", afiliado);
            }
            else
            {
                viewmodel.Title = "Afiliado / ReImprimir Tarjeta";
                viewmodel.Message = "Falló el proceso de reimpresión de la Tarjeta";
                viewmodel.ControllerName = "AfiliadoSuma";
                viewmodel.ActionName = "FilterReview";
                return RedirectToAction("GenericView", viewmodel);
            }
        }

        [HttpPost]
        public ActionResult ImprimirTarjeta(int id, string mode = "post")
        {
            ViewModel viewmodel = new ViewModel();
            AfiliadoSuma afiliado = repAfiliado.Find(id);
            if (repAfiliado.ImprimirTarjeta(afiliado))
            {
                viewmodel.Title = "Afiliado / Operaciones con la Impresora / Imprimir Tarjeta";
                viewmodel.Message = "Tarjeta impresa y activada correctamente";
                viewmodel.ControllerName = "AfiliadoSuma";
                viewmodel.ActionName = "FilterReview";
            }
            else
            {
                viewmodel.Title = "Afiliado / Operaciones con la Impresora / Imprimir Tarjeta";
                viewmodel.Message = "Falló el proceso de impresión y activación de la Tarjeta";
                viewmodel.ControllerName = "AfiliadoSuma";
                viewmodel.ActionName = "FilterReview";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult BloquearTarjeta(int id)
        {
            AfiliadoSuma afiliado = repAfiliado.Find(id);
            return View(afiliado);
        }

        [HttpPost]
        public ActionResult BloquearTarjeta(int id, string mode = "post")
        {
            ViewModel viewmodel = new ViewModel();
            AfiliadoSuma afiliado = repAfiliado.Find(id);
            if (repAfiliado.BloquearTarjeta(afiliado))
            {
                viewmodel.Title = "Afiliado / Bloquear Tarjeta";
                viewmodel.Message = "Tarjeta bloqueada correctamente";
                viewmodel.ControllerName = "AfiliadoSuma";
                viewmodel.ActionName = "FilterReview";
            }
            else
            {
                viewmodel.Title = "Afiliado / Bloquear Tarjeta";
                viewmodel.Message = "Falló el proceso de bloqueo de la Tarjeta";
                viewmodel.ControllerName = "AfiliadoSuma";
                viewmodel.ActionName = "FilterReview";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult SuspenderTarjeta(int id)
        {
            AfiliadoSuma afiliado = repAfiliado.Find(id);
            return View(afiliado);
        }

        [HttpPost]
        public ActionResult SuspenderTarjeta(int id, string observaciones, string mode = "post")
        {
            ViewModel viewmodel = new ViewModel();
            AfiliadoSuma afiliado = repAfiliado.Find(id);
            if (repAfiliado.SuspenderTarjeta(afiliado, observaciones))
            {
                viewmodel.Title = "Afiliado / Suspender Tarjeta";
                viewmodel.Message = "Tarjeta suspendida correctamente";
                viewmodel.ControllerName = "AfiliadoSuma";
                viewmodel.ActionName = "FilterReview";
            }
            else
            {
                viewmodel.Title = "Afiliado / Suspender Tarjeta";
                viewmodel.Message = "Falló el proceso de suspensión de la Tarjeta";
                viewmodel.ControllerName = "AfiliadoSuma";
                viewmodel.ActionName = "FilterReview";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult ReactivarTarjeta(int id)
        {
            AfiliadoSuma afiliado = repAfiliado.Find(id);
            return View(afiliado);
        }

        [HttpPost]
        public ActionResult ReactivarTarjeta(int id, string mode = "post")
        {
            ViewModel viewmodel = new ViewModel();
            AfiliadoSuma afiliado = repAfiliado.Find(id);
            if (repAfiliado.ReactivarTarjeta(afiliado))
            {
                viewmodel.Title = "Afiliado / Reactivar Tarjeta";
                viewmodel.Message = "Tarjeta reactivada correctamente";
                viewmodel.ControllerName = "AfiliadoSuma";
                viewmodel.ActionName = "FilterReview";
            }
            else
            {
                viewmodel.Title = "Afiliado / Reactivar Tarjeta";
                viewmodel.Message = "Falló el proceso de reactivación de la Tarjeta";
                viewmodel.ControllerName = "AfiliadoSuma";
                viewmodel.ActionName = "FilterReview";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult SaldosMovimientos(int id)
        {
            AfiliadoSuma afiliado = repAfiliado.Find(id);
            SaldosMovimientos SaldosMovimientos = repAfiliado.FindSaldosMovimientos(afiliado);
            if (SaldosMovimientos != null)
            {

                AfiliadoSumaSaldosMovimientos AfiliadoSaldosMovimientos = new AfiliadoSumaSaldosMovimientos()
                {
                    denominacionPrepago = "Bs.",
                    denominacionSuma = "Más",
                    Afiliado = afiliado,
                    SaldosMovimientos = SaldosMovimientos
                };
                return View(AfiliadoSaldosMovimientos);
            }
            else
            {
                //ERROR EN METODO FIND
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Afiliado / Saldos y Movimientos";
                viewmodel.Message = "Ha ocurrido un error de aplicación (FindSaldosMovimientos). Notifique al Administrador.";
                viewmodel.ControllerName = "AfiliadoSuma";
                viewmodel.ActionName = "FilterReview";
                return RedirectToAction("GenericView", viewmodel);
            }
        }

        public ActionResult Acreditar(int id)
        {
            AfiliadoSuma afiliado = repAfiliado.Find(id);
            return View(afiliado);
        }

        [HttpPost]
        public ActionResult Acreditar(int id, string monto)
        {
            ViewModel viewmodel = new ViewModel();
            AfiliadoSuma afiliado = repAfiliado.Find(id);
            string respuesta = repAfiliado.Acreditar(afiliado, monto);
            if (respuesta != null)
            {
                viewmodel.Title = "Afiliado / Acreditar";
                viewmodel.Message = "Acreditación exitosa. Clave de aprobación: " + respuesta;
                viewmodel.ControllerName = "AfiliadoSuma";
                viewmodel.ActionName = "FilterReview";
            }
            else
            {
                viewmodel.Title = "Afiliado / Acreditar ";
                viewmodel.Message = "Falló el proceso de acreditación";
                viewmodel.ControllerName = "AfiliadoSuma";
                viewmodel.ActionName = "FilterReview";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult EliminarAfiliacion(int id)
        {
            AfiliadoSuma afiliado = repAfiliado.Find(id);
            return View(afiliado);
        }

        [HttpPost]
        public ActionResult EliminarAfiliacion(int id, string estatustarjeta, string observaciones)
        {
            ViewModel viewmodel = new ViewModel();
            AfiliadoSuma afiliado = repAfiliado.Find(id);
            if (repAfiliado.EliminarAfiliacion(afiliado, observaciones))
            {
                if (estatustarjeta == "Activa")
                {
                    if (repAfiliado.SuspenderTarjeta(afiliado, observaciones))
                    {
                        viewmodel.Title = "Afiliado / Eliminar Afiliación";
                        viewmodel.Message = "La Afiliación fué eliminada con éxito";
                        viewmodel.ControllerName = "AfiliadoSuma";
                        viewmodel.ActionName = "FilterReview";
                    }
                    else
                    {
                        viewmodel.Title = "Afiliado / Eliminar Afiliación";
                        viewmodel.Message = "Falló el proceso de suspender Tarjeta";
                        viewmodel.ControllerName = "AfiliadoSuma";
                        viewmodel.ActionName = "FilterReview";
                    }
                }
                else
                {
                    viewmodel.Title = "Afiliado / Eliminar Afiliación";
                    viewmodel.Message = "La Afiliación fué eliminada con éxito";
                    viewmodel.ControllerName = "AfiliadoSuma";
                    viewmodel.ActionName = "FilterReview";
                }
            }
            else
            {
                viewmodel.Title = "Afiliado / Acreditar ";
                viewmodel.Message = "Falló el proceso de eliminar Afiliación";
                viewmodel.ControllerName = "AfiliadoSuma";
                viewmodel.ActionName = "FilterReview";
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

    }
}
