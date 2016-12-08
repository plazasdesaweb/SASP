using Suma2Lealtad.Filters;
using Suma2Lealtad.Models;
using Suma2Lealtad.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Suma2Lealtad.Controllers.Prepago
{
    [AuditingFilter]
    [HandleError]
    public class OrdenRecargaPrepagoController : Controller
    {
        private ClientePrepagoRepository repCliente = new ClientePrepagoRepository();
        private BeneficiarioPrepagoRepository repBeneficiario = new BeneficiarioPrepagoRepository();
        private OrdenRecargaRepository repOrden = new OrdenRecargaRepository();

        public ActionResult Filter()
        {
            OrdenRecargaPrepago orden = new OrdenRecargaPrepago()
            {
                Cliente = new ClientePrepago()
                {
                    nameCliente = ""
                },
                ListaClientes = repOrden.GetClientes()
            };
            orden.ListaClientes.Insert(0, new PrepaidCustomer { id = 0, name = "" });
            return View(orden);
        }        

        public ActionResult Create(int idCliente)
        {
            ClientePrepago cliente = repCliente.Find(idCliente);
            List<BeneficiarioPrepagoIndex> beneficiarios = repCliente.FindBeneficiarios(idCliente,"", "", "", "", "").ToList();
            List<DetalleOrdenRecargaPrepago> detalleOrden = repOrden.DetalleParaOrden(cliente, beneficiarios.FindAll(b => b.Afiliado.estatus == "Activa" && b.Afiliado.estatustarjeta == "Activa"));
            if (detalleOrden.Count == 0)
            {
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Prepago / Ordenes de Recarga / Crear Orden";
                viewmodel.Message = "No se puede crear Orden. El cliente no tiene beneficiarios activos con tarjeta activa.";
                viewmodel.ControllerName = "OrdenRecargaPrepago";
                viewmodel.ActionName = "Filter";
                return RedirectToAction("GenericView", viewmodel);
            }
            else
            {
                return View(detalleOrden);
            }
        }

        [HttpPost]
        public ActionResult Create(int idCliente, IList<DetalleOrdenRecargaPrepago> detalleOrden, decimal MontoTotalRecargas)
        {
            int idOrden = repOrden.CrearOrden(idCliente, detalleOrden.ToList(), MontoTotalRecargas);
            if (idOrden != 0)
            {
                //viewmodel.Title = "Prepago / Cliente / Ordenes de Recarga / Detalle de la Orden";
                //viewmodel.Message = "Orden Aprobada.";
                //viewmodel.ControllerName = "ClientePrepago";
                //viewmodel.ActionName = "FilterOrdenes";
                //viewmodel.RouteValues = id.ToString();
                return RedirectToAction("DetalleOrden", new { id = idOrden });
            }
            else
            {
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Prepago / Ordenes de Recarga / Crear Orden";
                viewmodel.Message = "Falló el proceso de creación de la Orden.";
                viewmodel.ControllerName = "OrdenRecargaPrepago";
                viewmodel.ActionName = "Filter";
                return RedirectToAction("GenericView", viewmodel);
            }
        }

        public ActionResult FilterAnulacion()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FilterAnulacion(string batchid)
        {
            List<DetalleOrdenRecargaPrepago> detalleOrden = repOrden.DetalleParaOrdenAnulacion(batchid);
            if (detalleOrden.Count != 1)
            {
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Prepago / Ordenes de Recarga / Crear Orden de Anulación";
                viewmodel.Message = "No se encontró Recarga con la Referencia indicada";
                viewmodel.ControllerName = "OrdenRecargaPrepago";
                viewmodel.ActionName = "FilterAnulacion";
                return RedirectToAction("GenericView", viewmodel);
            }
            else if (detalleOrden.First().batchid == "Ya tiene Anulación")
            {
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Prepago / Ordenes de Recarga / Crear Orden de Anulación";
                viewmodel.Message = "La Referencia indicada ya tiene Orden de Anulación";
                viewmodel.ControllerName = "OrdenRecargaPrepago";
                viewmodel.ActionName = "FilterAnulacion";
                return RedirectToAction("GenericView", viewmodel);
            }            
            else
            {
                return View("CreateAnulacion",detalleOrden);
            }
        }

        [HttpPost]
        public ActionResult CreateAnulacion(int idCliente, IList<DetalleOrdenRecargaPrepago> detalleOrden)
        {
            decimal MontoAnulacion = detalleOrden.First().montoRecarga;
            int idOrden = repOrden.CrearOrden(idCliente, detalleOrden.ToList(), MontoAnulacion);
            if (idOrden != 0)
            {
                //viewmodel.Title = "Prepago / Cliente / Ordenes de Recarga / Detalle de la Orden";
                //viewmodel.Message = "Orden Aprobada.";
                //viewmodel.ControllerName = "ClientePrepago";
                //viewmodel.ActionName = "FilterOrdenes";
                //viewmodel.RouteValues = id.ToString();
                return RedirectToAction("DetalleOrden", new { id = idOrden });
            }
            else
            {
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Prepago / Ordenes de Recarga / Crear Orden de Anulación";
                viewmodel.Message = "Falló el proceso de creación de la Orden.";
                viewmodel.ControllerName = "OrdenRecargaPrepago";
                viewmodel.ActionName = "FilterAnulacion";
                return RedirectToAction("GenericView", viewmodel);
            }
        }

        public ActionResult CargarArchivoRecarga(int idCliente)
        {
            ClientePrepago cliente = repCliente.Find(idCliente);
            List<DetalleOrdenRecargaPrepago> detalleOrden = new List<DetalleOrdenRecargaPrepago>();
            DetalleOrdenRecargaPrepago detalle = new DetalleOrdenRecargaPrepago()
            {
                idCliente = cliente.idCliente,
                nameCliente = cliente.nameCliente,
                rifCliente = cliente.rifCliente,
                phoneCliente = cliente.phoneCliente
            };
            detalleOrden.Add(detalle);
            return View(detalleOrden);
        }

        [HttpPost]
        public ActionResult CreateArchivo(int idCliente, HttpPostedFileBase file)
        {
            ViewModel viewmodel = new ViewModel();
            //subir y guardar archivo
            string path = "";
            string idtemp = System.Web.HttpContext.Current.Session.SessionID + ".xls";
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    path = Server.MapPath("~/App_Data/" + idtemp);
                    file.SaveAs(path);
                }
                catch (Exception ex)
                {
                    viewmodel.Title = "Prepago / Ordenes de Recarga / Crear Orden desde Archivo";
                    viewmodel.Message = "Error de aplicacion: No se pudo cargar archivo (" + ex.Message + ")";
                    viewmodel.ControllerName = "OrdenRecargaPrepago";
                    viewmodel.ActionName = "Filter";
                    return RedirectToAction("GenericView", viewmodel);
                }
                ClientePrepago cliente = repCliente.Find(idCliente);
                List<BeneficiarioPrepagoIndex> beneficiarios = repCliente.FindBeneficiarios(idCliente,"", "", "", "", "").ToList();
                if (beneficiarios.Count == 0)
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    viewmodel.Title = "Prepago / Ordenes de Recarga / Crear Orden";
                    viewmodel.Message = "No se puede crear Orden. El cliente no tiene beneficiarios activos con tarjeta activa.";
                    viewmodel.ControllerName = "OrdenRecargaPrepago";
                    viewmodel.ActionName = "Filter";
                    return RedirectToAction("GenericView", viewmodel);
                }
                else
                {
                    //leer el archivo
                    List<DetalleOrdenRecargaPrepago> detalleOrdenArchivo = repOrden.GetBeneficiariosArchivo(path);
                    //borrar el archivo
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    if (detalleOrdenArchivo == null)
                    {
                        viewmodel.Title = "Prepago / Ordenes de Recarga / Crear Orden desde Archivo";
                        viewmodel.Message = "Error de aplicacion: No se pudo leer el archivo.";
                        viewmodel.ControllerName = "OrdenRecargaPrepago";
                        viewmodel.ActionName = "Filter";
                        return RedirectToAction("GenericView", viewmodel);
                    }
                    else
                    {
                        List<DetalleOrdenRecargaPrepago> detalleOrden = repOrden.DetalleParaOrdenArchivo(cliente, beneficiarios.FindAll(b => b.Afiliado.estatus == "Activa" && b.Afiliado.estatustarjeta == "Activa"), detalleOrdenArchivo);
                        return View("Create", detalleOrden);
                    }
                }
            }
            else
            {
                //borrar el archivo
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                viewmodel.Title = "Prepago / Ordenes de Recarga / Crear Orden desde Archivo";
                viewmodel.Message = "Error de aplicacion: El archivo está vacío";
                viewmodel.ControllerName = "OrdenRecargaPrepago";
                viewmodel.ActionName = "Filter";
                return RedirectToAction("GenericView", viewmodel);
            }
        }

        public ActionResult DetalleOrden(int id)
        {
            List<DetalleOrdenRecargaPrepago> detalleOrden = repOrden.FindDetalleOrden(id);
            return View(detalleOrden);
        }

        [HttpPost]
        public ActionResult AprobarOrden(int id, IList<DetalleOrdenRecargaPrepago> detalleOrden, decimal MontoTotalRecargas, string indicadorGuardar, string DocumentoReferencia, string Observaciones)
        {
            ViewModel viewmodel = new ViewModel();
            detalleOrden.First().documentoOrden = DocumentoReferencia;
            detalleOrden.First().observacionesOrden = Observaciones;
            if (indicadorGuardar == "Aprobar")
            {
                if (repOrden.AprobarOrden(detalleOrden.ToList(), MontoTotalRecargas))
                {
                    viewmodel.Title = "Prepago / Ordenes de Recarga / Detalle de la Orden";
                    viewmodel.Message = "Orden Aprobada.";
                    viewmodel.ControllerName = "OrdenRecargaPrepago";
                    viewmodel.ActionName = "FilterReview";
                    return RedirectToAction("GenericView", viewmodel);
                    //return RedirectToAction("DetalleOrden", new { id = id, idOrden = idOrden });
                }
                else
                {
                    viewmodel.Title = "Prepago / Ordenes de Recarga / Detalle de la Orden";
                    viewmodel.Message = "Falló el proceso de aprobación de la Orden.";
                    viewmodel.ControllerName = "OrdenRecargaPrepago";
                    viewmodel.ActionName = "FilterReview";
                    return RedirectToAction("GenericView", viewmodel);
                }
            }
            else
            {
                if (repOrden.GuardarOrden(detalleOrden.ToList(), MontoTotalRecargas))
                {
                    viewmodel.Title = "Prepago / Ordenes de Recarga / Detalle de la Orden";
                    viewmodel.Message = "Datos de la Orden actualizados.";
                    viewmodel.ControllerName = "OrdenRecargaPrepago";
                    viewmodel.ActionName = "FilterReview";
                    return RedirectToAction("GenericView", viewmodel);
                    //return RedirectToAction("DetalleOrden", new { id = id, idOrden = idOrden });
                }
                else
                {
                    viewmodel.Title = "Prepago / Ordenes de Recarga / Detalle de la Orden";
                    viewmodel.Message = "Falló el proceso de guardado de la Orden.";
                    viewmodel.ControllerName = "OrdenRecargaPrepago";
                    viewmodel.ActionName = "FilterReview";
                    return RedirectToAction("GenericView", viewmodel);
                }
            }
        }

        public ActionResult RechazarOrden(int id)
        {
            ViewModel viewmodel = new ViewModel();
            if (repOrden.RechazarOrden(id))
            {
                viewmodel.Title = "Prepago / Ordenes de Recarga / Detalle de la Orden";
                viewmodel.Message = "Orden Rechazada.";
                viewmodel.ControllerName = "OrdenRecargaPrepago";
                viewmodel.ActionName = "FilterReview";
            }
            else
            {
                viewmodel.Title = "Prepago / Ordenes de Recarga / Detalle de la Orden";
                viewmodel.Message = "Falló el proceso de rechazo de la Orden.";
                viewmodel.ControllerName = "OrdenRecargaPrepago";
                viewmodel.ActionName = "FilterReview";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult ProcesarOrden(int id)
        {
            if (repOrden.ProcesarOrden(id))
            {
                //ViewModel viewmodel = new ViewModel();
                //viewmodel.Title = "Prepago / Cliente / Ordenes de Recarga / Detalle de la Orden / Procesar Orden";
                //viewmodel.Message = "Orden Procesada.";
                //viewmodel.ControllerName = "ClientePrepago";
                //viewmodel.ActionName = "FilterOrdenes";
                //viewmodel.RouteValues = id.ToString();
                //return RedirectToAction("GenericView", viewmodel);
                //List<DetalleOrdenRecargaPrepago> detalleOrden = repOrden.FindDetalleOrden(idOrden);
                //return View("ResultadoOrden", detalleOrden);
                List<DetalleOrdenRecargaPrepago> detalleOrden = repOrden.FindDetalleOrden(id);
                return View("DetalleOrden", detalleOrden);
            }
            else
            {
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Prepago / Ordenes de Recarga / Detalle de la Orden / Procesar Orden";
                viewmodel.Message = "Falló el procesamiento de la Orden.";
                viewmodel.ControllerName = "OrdenRecargaPrepago";
                viewmodel.ActionName = "FilterReview";
                return RedirectToAction("GenericView", viewmodel);
            }
        }

        public FileResult DescargarArchivoFilePath()
        {
            string file = Server.MapPath("~/App_Data/Plantilla.xls");
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(file, contentType, Path.GetFileName(file));
        }

        public ActionResult FilterReview()
        {
            OrdenRecargaPrepago orden = new OrdenRecargaPrepago();
            orden.ClaseDeOrdenOptions = orden.ClaseDeOrdenOptions.Where(x => x.id != "Orden de Acreditación Suma" && x.id != "Orden de Recarga Suma");
            return View(orden);
        }

        [HttpPost]
        public ActionResult FilterReview(string numero, string fecha, string estadoOrden, string Referencia, string claseOrden, string Observaciones)
        {
            List<OrdenRecargaPrepago> ordenes = new List<OrdenRecargaPrepago>();
            OrdenRecargaPrepago orden;
            if (numero != "")
            {
                orden = repOrden.Find(Convert.ToInt32(numero));
                if (orden != null)
                {
                    if (orden.tipoOrden != "Orden de Acreditación Suma" && orden.tipoOrden != "Orden de Recarga Suma")
                    {
                        ordenes.Add(orden);
                    }                    
                }
            }
            else
            {
                ordenes = repOrden.Find(fecha, estadoOrden, Referencia, claseOrden, Observaciones)
                                  .Where(o => o.tipoOrden != "Orden de Acreditación Suma" && o.tipoOrden != "Orden de Recarga Suma")
                                  .OrderBy(x => x.Cliente.nameCliente)
                                  .ThenBy(y => y.id)
                                  .ToList();                
            }
            return View("Index", ordenes);
        }

        public ActionResult GenericView(ViewModel viewmodel)
        {
            return View(viewmodel);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        
    }
}
