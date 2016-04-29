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
    public class BeneficiarioPrepagoController : Controller
    {
        private ClientePrepagoRepository repCliente = new ClientePrepagoRepository();
        private BeneficiarioPrepagoRepository repBeneficiario = new BeneficiarioPrepagoRepository();
        private AfiliadoSumaRepository repAfiliado = new AfiliadoSumaRepository();

        public ActionResult Filter()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Filter(string numdoc)
        {
            BeneficiarioPrepago beneficiario;
            BeneficiarioPrepagoIndex beneficiarioIndex = repBeneficiario.Find(numdoc, "", "", "", "").FirstOrDefault();
            //NO ES Beneficiario PrepagoPlazas
            if (beneficiarioIndex == null)
            {
                beneficiario = new BeneficiarioPrepago()
                {
                    Afiliado = repAfiliado.Find(numdoc)
                };
                //CARGO VALOR POR DEFECTO EN LISTA DE ESTADOS
                beneficiario.Afiliado.ListaEstados.Insert(0, new ESTADO { COD_ESTADO = " ", DESCRIPC_ESTADO = "Seleccione un Estado" });
                //SI ES SUMA. VOY A EDIT, SI NO A CREATE
                if (beneficiario.Afiliado.type == "Suma")
                {
                    //MUESTRO MENSAJE DE CAMBIO SUMA A PREPAGO
                    ViewModel viewmodel = new ViewModel();
                    viewmodel.Title = "Prepago / Beneficiario / Crear Beneficiario / Filtro de Búsqueda";
                    viewmodel.Message = "El número de documento ya posee afiliación a Suma Plaza's. Complete la afiliación a Prepago Plaza's.";
                    viewmodel.ControllerName = "BeneficiarioPrepago";
                    viewmodel.ActionName = "CreateBeneficiarioSuma";
                    viewmodel.RouteValues = beneficiario.Afiliado.id.ToString();
                    return RedirectToAction("GenericView", viewmodel);                   
                }
                else
                {
                    beneficiario.Afiliado.typeid = Globals.ID_TYPE_PREPAGO;
                    beneficiario.Afiliado.type = "Prepago";
                    beneficiario.Afiliado.ListaClientes = repBeneficiario.GetClientes();
                    beneficiario.Afiliado.ListaClientes.Insert(0, new PrepaidCustomer { id = 0, name = "" });
                    //verifico si tiene tarjeta en Cards
                    if (beneficiario.Afiliado.pan != "0" && beneficiario.Afiliado.estatustarjeta == "Activa")
                    {
                        ViewModel viewmodel = new ViewModel();
                        viewmodel.Title = "Prepago / Beneficiario / Crear Beneficiario / Filtro de Búsqueda";
                        viewmodel.Message = "El número de documento indicado ya posee una Tarjeta Activa con el número " + beneficiario.Afiliado.pan;
                        viewmodel.ControllerName = "BeneficiarioPrepago";
                        viewmodel.ActionName = "CreateBeneficiarioConTarjeta";
                        viewmodel.RouteValues = "?numdoc=" + numdoc;
                        return RedirectToAction("GenericView", viewmodel);
                    }
                    else
                    {
                        return View("Create", beneficiario.Afiliado);
                    }
                }
            }
            //ES Beneficiario PrepagoPlazas
            else
            {
                beneficiario = repBeneficiario.Find(beneficiarioIndex.Afiliado.id);
                return View("Edit", beneficiario.Afiliado);
            }
        }

        [HttpPost]
        public ActionResult CreateBeneficiario(AfiliadoSuma Afiliado, HttpPostedFileBase fileNoValidado)
        {
            ViewModel viewmodel = new ViewModel();
            if (repAfiliado.Save(Afiliado, fileNoValidado))
            {
                BeneficiarioPrepago beneficiario = new BeneficiarioPrepago()
                {
                    Afiliado = Afiliado,
                    Cliente = repCliente.Find(Afiliado.idClientePrepago)
                };
                if (repBeneficiario.Save(beneficiario))
                {
                    viewmodel.Title = "Prepago / Beneficiario / Crear Afiliación";
                    viewmodel.Message = "Solicitud de afiliación creada exitosamente.";
                    viewmodel.ControllerName = "BeneficiarioPrepago";
                    viewmodel.ActionName = "FilterReview";
                }
            }
            else
            {
                viewmodel.Title = "Prepago / Beneficiario / Crear Afiliación";
                viewmodel.Message = "Error de aplicacion: No se pudo crear solicitud de afiliación.";
                viewmodel.ControllerName = "BeneficiarioPrepago";
                viewmodel.ActionName = "FilterReview";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult CreateBeneficiarioSuma(int id)
        {
            BeneficiarioPrepago beneficiario = new BeneficiarioPrepago()
            {
                Afiliado = repAfiliado.Find(id)
            };
            beneficiario.Afiliado = repAfiliado.ReiniciarAfiliacionSumaAPrepago(beneficiario.Afiliado);
            if (repAfiliado.SaveChanges(beneficiario.Afiliado))
            {
                beneficiario.Afiliado.ListaClientes = repBeneficiario.GetClientes();
                beneficiario.Afiliado.ListaClientes.Insert(0, new PrepaidCustomer { id = 0, name = "" });
                return View("Edit", beneficiario.Afiliado);
            }
            else
            {
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Prepago / Beneficiario / Crear Beneficiario / Filtro de Búsqueda";
                viewmodel.Message = "Error de aplicación: No se pudo guardar afiliacion Prepago";
                viewmodel.ControllerName = "BeneficiarioPrepago";
                viewmodel.ActionName = "FilterReview";
                return RedirectToAction("GenericView", viewmodel);
            }
        }

        public ActionResult CreateBeneficiarioConTarjeta(string numdoc)
        {
            BeneficiarioPrepago beneficiario;
            beneficiario = new BeneficiarioPrepago()
            {
                Afiliado = repAfiliado.Find(numdoc)
            };
            //CARGO VALOR POR DEFECTO EN LISTA DE ESTADOS
            beneficiario.Afiliado.ListaEstados.Insert(0, new ESTADO { COD_ESTADO = " ", DESCRIPC_ESTADO = "Seleccione un Estado" });
            beneficiario.Afiliado.typeid = Globals.ID_TYPE_PREPAGO;
            beneficiario.Afiliado.type = "Prepago";
            beneficiario.Afiliado.ListaClientes = repBeneficiario.GetClientes();
            beneficiario.Afiliado.ListaClientes.Insert(0, new PrepaidCustomer { id = 0, name = "" });
            return View("Create", beneficiario.Afiliado);
        }

        [HttpPost]
        public ActionResult CreateBeneficiarioConTarjeta(AfiliadoSuma Afiliado, HttpPostedFileBase fileNoValidado)
        {
            ViewModel viewmodel = new ViewModel();
            if (repAfiliado.Save(Afiliado, fileNoValidado))
            {
                BeneficiarioPrepago beneficiario = new BeneficiarioPrepago()
                {
                    Afiliado = Afiliado,
                    Cliente = repCliente.Find(Afiliado.idClientePrepago)
                };
                if (repBeneficiario.Save(beneficiario))
                {
                    int idafiliado = repAfiliado.Find(Afiliado.docnumber, "", "", "", "").First().id;
                    beneficiario.Afiliado = repAfiliado.Find(idafiliado);
                    if (repAfiliado.Aprobar(beneficiario.Afiliado))
                    {
                        viewmodel.Title = "Prepago / Beneficiario / Crear Afiliación";
                        viewmodel.Message = "Solicitud de afiliación creada y aprobada exitosamente.";
                        viewmodel.ControllerName = "BeneficiarioPrepago";
                        viewmodel.ActionName = "FilterReview";
                    }
                    else
                    {
                        viewmodel.Title = "Prepago / Beneficiario / Crear Afiliación";
                        viewmodel.Message = "Solicitud de afiliación creada, pero no se pudo aprobar automáticamente.";
                        viewmodel.ControllerName = "BeneficiarioPrepago";
                        viewmodel.ActionName = "FilterReview";
                    }
                }
            }
            else
            {
                viewmodel.Title = "Prepago / Beneficiario / Crear Afiliación";
                viewmodel.Message = "Error de aplicacion: No se pudo crear solicitud de afiliación.";
                viewmodel.ControllerName = "BeneficiarioPrepago";
                viewmodel.ActionName = "FilterReview";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        [HttpPost]
        public ActionResult EditBeneficiarioSuma(AfiliadoSuma Afiliado, HttpPostedFileBase fileNoValidado)
        {
            ViewModel viewmodel = new ViewModel();
            if (repAfiliado.SaveChanges(Afiliado, fileNoValidado))
            {
                BeneficiarioPrepago beneficiario = new BeneficiarioPrepago()
                {
                    Afiliado = Afiliado,
                    Cliente = repCliente.Find(Afiliado.idClientePrepago)
                };
                if (repBeneficiario.Save(beneficiario))
                {
                    viewmodel.Title = "Prepago / Beneficiario / Crear Afiliación";
                    viewmodel.Message = "Solicitud de afiliación creada exitosamente.";
                    viewmodel.ControllerName = "BeneficiarioPrepago";
                    viewmodel.ActionName = "FilterReview";
                }
            }
            else
            {
                viewmodel.Title = "Prepago / Beneficiario / Crear Afiliación";
                viewmodel.Message = "Error de aplicacion: No se pudo crear solicitud de afiliación.";
                viewmodel.ControllerName = "BeneficiarioPrepago";
                viewmodel.ActionName = "FilterReview";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult FilterReview()
        {
            BeneficiarioPrepago beneficiario = new BeneficiarioPrepago();
            return View(beneficiario);
        }

        [HttpPost]
        public ActionResult FilterReview(string numdoc, string name, string email, string estadoAfiliacion, string estadoTarjeta)
        {
            List<BeneficiarioPrepagoIndex> beneficiarios = repBeneficiario.Find(numdoc, name, email, estadoAfiliacion, estadoTarjeta).OrderBy(x => x.Cliente.nameCliente).ThenBy(y => y.Afiliado.docnumber).ToList();
            return View("Index", beneficiarios);
        }

        public ActionResult EditBeneficiario(int id)
        {
            BeneficiarioPrepago beneficiario = repBeneficiario.Find(id);
            return View("Edit", beneficiario.Afiliado);
        }

        [HttpPost]
        //public ActionResult EditBeneficiario(AfiliadoSuma afiliado, ClientePrepago Cliente)
        public ActionResult EditBeneficiario(AfiliadoSuma afiliado, HttpPostedFileBase fileNoValidado)
        {
            ViewModel viewmodel = new ViewModel();
            if (!repAfiliado.SaveChanges(afiliado, fileNoValidado))
            {
                viewmodel.Title = "Prepago / Beneficiario / Revisar Afiliación";
                viewmodel.Message = "Error de aplicacion: No se pudo actualizar afiliación.";
                viewmodel.ControllerName = "BeneficiarioPrepago";
                viewmodel.ActionName = "FilterReview";
            }
            else
            {
                viewmodel.Title = "Prepago / Beneficiario / Revisar Afiliación";
                viewmodel.Message = "La información del beneficiario ha sido actualizada satisfactoriamente.";
                viewmodel.ControllerName = "BeneficiarioPrepago";
                viewmodel.ActionName = "FilterReview";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult CambiarTipoDocumento(int id)
        {
            BeneficiarioPrepago beneficiario = repBeneficiario.Find(id);
            if (beneficiario.Afiliado.docnumber.Substring(0, 1) == "V")
            {
                ViewData["TipoDocumentoOptions"] = new List<SelectListItem>()
                {      
                    new SelectListItem{ Text="Cédula de Extranjero", Value="E"},
                    new SelectListItem{ Text="Pasaporte", Value="P"}
                };
            }
            else if (beneficiario.Afiliado.docnumber.Substring(0, 1) == "E")
            {
                ViewData["TipoDocumentoOptions"] = new List<SelectListItem>()
                {           
                    new SelectListItem{ Text="Cédula de Venezolano", Value="V"},
                    new SelectListItem{ Text="Pasaporte", Value="P"}
                };
            }
            else if (beneficiario.Afiliado.docnumber.Substring(0, 1) == "P")
            {
                ViewData["TipoDocumentoOptions"] = new List<SelectListItem>()
                {           
                    new SelectListItem{ Text="Cédula de Venezolano", Value="V"},
                    new SelectListItem{ Text="Cédula de Extranjero", Value="E"}
                };
            }
            return View(beneficiario.Afiliado);
        }

        [HttpPost]
        public ActionResult CambiarTipoDocumento(int id, string tipoDocumento)
        {
            BeneficiarioPrepago beneficiario = repBeneficiario.Find(id);
            if (repAfiliado.CambiarTipoDocumento(beneficiario.Afiliado, tipoDocumento))
            {
                return RedirectToAction("EditBeneficiario", new { id = id });
            }
            else
            {
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Prepago / Beneficiario / Revisar Afiliación / Cambiar Tipo de Documento";
                viewmodel.Message = "Error de aplicacion: No se pudo cambiar tipo de documento.";
                viewmodel.ControllerName = "BeneficiarioPrepago";
                viewmodel.ActionName = "FilterReview";
                return RedirectToAction("GenericView", viewmodel);
            }
        }

        public ActionResult AprobarBeneficiario(int id)
        {
            ViewModel viewmodel = new ViewModel();
            BeneficiarioPrepago beneficiario = repBeneficiario.Find(id);
            //SI ES AFILIADO SUMA, SE CAMBIA EL TIPO Y YA NO SE BLOQUEA LA TARJETA ACTUAL
            if (beneficiario.Afiliado.type == "Suma")
            {
                if (repAfiliado.Aprobar(beneficiario.Afiliado))
                {
                    beneficiario.Afiliado = repAfiliado.CambiarAPrepago(beneficiario.Afiliado);
                    //if (repAfiliado.BloquearTarjeta(beneficiario.Afiliado) == true)
                    //{
                    //    repAfiliado.SaveChanges(beneficiario.Afiliado);
                    //    viewmodel.Title = "Prepago / Beneficiario / Aprobar Afiliación:";
                    //    viewmodel.Message = "Afiliación Aprobada. Se creó una nueva tarjeta Prepago Plaza's";
                    //    viewmodel.ControllerName = "BeneficiarioPrepago";
                    //    viewmodel.ActionName = "FilterReview";
                    //}
                    //else
                    //{
                    //    repAfiliado.SaveChanges(beneficiario.Afiliado);
                    //    viewmodel.Title = "Prepago / Beneficiario / Eliminar Beneficiario";
                    //    viewmodel.Message = "Afiliación Aprobada. No se pudo crear una nueva tarjeta Prepago Plaza's";
                    //    viewmodel.ControllerName = "BeneficiarioPrepago";
                    //    viewmodel.ActionName = "FilterReview";
                    //}
                    repAfiliado.SaveChanges(beneficiario.Afiliado);
                    viewmodel.Title = "Prepago / Beneficiario / Aprobar Afiliación:";
                    viewmodel.Message = "Afiliación Prepago Aprobada.";
                    viewmodel.ControllerName = "BeneficiarioPrepago";
                    viewmodel.ActionName = "FilterReview";
                }
                else
                {
                    viewmodel.Title = "Prepago / Beneficiario / Aprobar Afiliación:";
                    viewmodel.Message = "Error de aplicacion: No se pudo aprobar afiliación.";
                    viewmodel.ControllerName = "BeneficiarioPrepago";
                    viewmodel.ActionName = "FilterReview";
                }
            }
            else
            {
                if (repAfiliado.Aprobar(beneficiario.Afiliado))
                {
                    viewmodel.Title = "Prepago / Beneficiario / Aprobar Afiliación:";
                    viewmodel.Message = "Afiliación Prepago Aprobada.";
                    viewmodel.ControllerName = "BeneficiarioPrepago";
                    viewmodel.ActionName = "FilterReview";
                }
                else
                {
                    viewmodel.Title = "Prepago / Beneficiario / Aprobar Afiliación:";
                    viewmodel.Message = "Error de aplicacion: No se pudo aprobar afiliación.";
                    viewmodel.ControllerName = "BeneficiarioPrepago";
                    viewmodel.ActionName = "FilterReview";
                }
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult ImprimirTarjeta(int id)
        {
            BeneficiarioPrepago beneficiario = repBeneficiario.Find(id);
            return View("ImpresoraImprimirTarjeta", beneficiario);
        }

        public ActionResult ReImprimirTarjeta(int id, int idCliente)
        {
            ViewModel viewmodel = new ViewModel();
            BeneficiarioPrepago beneficiario = repBeneficiario.Find(id);
            if (repAfiliado.BloquearTarjeta(beneficiario.Afiliado))
            {
                return View("ImpresoraImprimirTarjeta", beneficiario);
            }
            else
            {
                viewmodel.Title = "Prepago / Beneficiario / ReImprimir Tarjeta";
                viewmodel.Message = "Falló el proceso de reimpresión de la Tarjeta";
                viewmodel.ControllerName = "BeneficiarioPrepago";
                viewmodel.ActionName = "FilterReview";
                return RedirectToAction("GenericView", viewmodel);
            }
        }

        [HttpPost]
        public ActionResult ImprimirTarjeta(int id, string mode = "post")
        {
            ViewModel viewmodel = new ViewModel();
            BeneficiarioPrepago beneficiario = repBeneficiario.Find(id);
            if (repAfiliado.ImprimirTarjeta(beneficiario.Afiliado))
            {
                viewmodel.Title = "Prepago / Beneficiario / Operaciones con la Impresora / Imprimir Tarjeta";
                viewmodel.Message = "Tarjeta impresa y activada correctamente";
                viewmodel.ControllerName = "BeneficiarioPrepago";
                viewmodel.ActionName = "FilterReview";
            }
            else
            {
                viewmodel.Title = "Prepago / Beneficiario / Operaciones con la Impresora / Imprimir Tarjeta";
                viewmodel.Message = "Falló el proceso de impresión y activación de la Tarjeta.";
                viewmodel.ControllerName = "BeneficiarioPrepago";
                viewmodel.ActionName = "FilterReview";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult CrearPin(int id)
        {
            BeneficiarioPrepago beneficiario = repBeneficiario.Find(id);
            return View("PinPadCrearPin", beneficiario);
        }

        public ActionResult CambiarPin(int id)
        {
            BeneficiarioPrepago beneficiario = repBeneficiario.Find(id);
            return View("PinPadCambiarPin", beneficiario);
        }

        public ActionResult ReiniciarPin(int id)
        {
            BeneficiarioPrepago beneficiario = repBeneficiario.Find(id);
            return View("PinPadReiniciarPin", beneficiario);
        }

        public ActionResult BloquearTarjeta(int id)
        {
            BeneficiarioPrepago beneficiario = repBeneficiario.Find(id);
            return View(beneficiario);
        }

        [HttpPost]
        public ActionResult BloquearTarjeta(int id, string mode = "post")
        {
            ViewModel viewmodel = new ViewModel();
            AfiliadoSuma afiliado = repAfiliado.Find(id);
            if (repAfiliado.BloquearTarjeta(afiliado))
            {
                viewmodel.Title = "Prepago / Beneficiario / Bloquear Tarjeta";
                viewmodel.Message = "Tarjeta bloqueada correctamente";
                viewmodel.ControllerName = "BeneficiarioPrepago";
                viewmodel.ActionName = "FilterReview";
            }
            else
            {
                viewmodel.Title = "Prepago / Beneficiario / Bloquear Tarjeta";
                viewmodel.Message = "Falló el proceso de bloqueo de la Tarjeta";
                viewmodel.ControllerName = "BeneficiarioPrepago";
                viewmodel.ActionName = "FilterReview";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult SuspenderTarjeta(int id)
        {
            BeneficiarioPrepago beneficiario = repBeneficiario.Find(id);
            return View(beneficiario);
        }

        [HttpPost]
        public ActionResult SuspenderTarjeta(int id, string observaciones = "", string mode = "post")
        {
            ViewModel viewmodel = new ViewModel();
            AfiliadoSuma afiliado = repAfiliado.Find(id);
            if (repAfiliado.SuspenderTarjeta(afiliado, observaciones))
            {
                viewmodel.Title = "Prepago / Beneficiario / Suspender Tarjeta";
                viewmodel.Message = "Tarjeta suspendida correctamente";
                viewmodel.ControllerName = "BeneficiarioPrepago";
                viewmodel.ActionName = "FilterReview";
            }
            else
            {
                viewmodel.Title = "Prepago / Beneficiario / Suspender Tarjeta";
                viewmodel.Message = "Falló el proceso de suspensión de la Tarjeta";
                viewmodel.ControllerName = "BeneficiarioPrepago";
                viewmodel.ActionName = "FilterReview";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult ReactivarTarjeta(int id)
        {
            BeneficiarioPrepago beneficiario = repBeneficiario.Find(id);
            return View(beneficiario);
        }

        [HttpPost]
        public ActionResult ReactivarTarjeta(int id, string mode = "post")
        {
            ViewModel viewmodel = new ViewModel();
            AfiliadoSuma afiliado = repAfiliado.Find(id);
            if (repAfiliado.ReactivarTarjeta(afiliado))
            {
                viewmodel.Title = "Prepago / Beneficiario / Reactivar Tarjeta";
                viewmodel.Message = "Tarjeta reactivada correctamente";
                viewmodel.ControllerName = "BeneficiarioPrepago";
                viewmodel.ActionName = "FilterReview";
            }
            else
            {
                viewmodel.Title = "Prepago / Beneficiario / Reactivar Tarjeta";
                viewmodel.Message = "Falló el proceso de reactivación de la Tarjeta";
                viewmodel.ControllerName = "BeneficiarioPrepago";
                viewmodel.ActionName = "FilterReview";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult SaldosMovimientos(int id)
        {
            BeneficiarioPrepago beneficiario = repBeneficiario.Find(id);
            SaldosMovimientos SaldosMovimientos = repAfiliado.FindSaldosMovimientos(beneficiario.Afiliado);
            if (SaldosMovimientos != null)
            {
                BeneficiarioPrepagoSaldosMovimientos beneficiarioSaldosMovimientos = new BeneficiarioPrepagoSaldosMovimientos()
                {
                    denominacionPrepago = "Bs.",
                    denominacionSuma = "Más",
                    Beneficiario = beneficiario,
                    SaldosMovimientos = SaldosMovimientos,
                };
                return View(beneficiarioSaldosMovimientos);
            }
            else
            {
                //ERROR EN METODO FIND
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Prepago / Beneficiario / Saldos y Movimientos";
                viewmodel.Message = "Ha ocurrido un error de aplicación (FindSaldosMovimientos). Notifique al Administrador.";
                viewmodel.ControllerName = "BeneficiarioPrepago";
                viewmodel.ActionName = "FilterReview";
                return RedirectToAction("GenericView", viewmodel);
            }
        }

        public ActionResult GenericView(ViewModel viewmodel)
        {
            return View(viewmodel);
        }

        public FileContentResult GetImageBeneficiario(int id)
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
