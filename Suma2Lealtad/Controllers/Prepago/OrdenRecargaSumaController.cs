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
    public class OrdenRecargaSumaController : Controller
    {
        private ClientePrepagoRepository repCliente = new ClientePrepagoRepository();
        private BeneficiarioPrepagoRepository repBeneficiario = new BeneficiarioPrepagoRepository();        
        private AfiliadoSumaRepository repAfiliado = new AfiliadoSumaRepository();
        private OrdenRecargaRepository repOrden = new OrdenRecargaRepository();

        //public ActionResult Filter()
        //{
        //    OrdenRecargaPrepago orden = new OrdenRecargaPrepago()
        //    {
        //        Cliente = new ClientePrepago()
        //        {
        //            nameCliente = ""
        //        },
        //        ListaClientes = repOrden.GetClientes()
        //    };
        //    orden.ListaClientes.Insert(0, new PrepaidCustomer { id = 0, name = "" });
        //    return View(orden);
        //}

        public JsonResult FindAfiliado(string docnumber)
        {
            List<AfiliadoSumaIndex> afiliados = repAfiliado.Find(docnumber, "", "", "", "").FindAll(b => b.estatus == "Activa" && b.estatustarjeta == "Activa");
            return Json(afiliados, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateAcreditacion()
        {
            List<DetalleOrdenRecargaPrepago> detalleOrden = new List<DetalleOrdenRecargaPrepago>();
            DetalleOrdenRecargaPrepago dorden = new DetalleOrdenRecargaPrepago()
            {
                tipoOrden = "Orden de Acreditación Suma"
            };
            detalleOrden.Add(dorden);
            return View("Create", detalleOrden);
        }

        public ActionResult CreateRecarga()
        {
            List<DetalleOrdenRecargaPrepago> detalleOrden = new List<DetalleOrdenRecargaPrepago>();
            DetalleOrdenRecargaPrepago dorden = new DetalleOrdenRecargaPrepago()
            {
                tipoOrden = "Orden de Recarga Suma"
            };
            detalleOrden.Add(dorden);
            return View("Create", detalleOrden);
        }

        [HttpPost]
        public ActionResult Create(IList<DetalleOrdenRecargaPrepago> detalleOrden, decimal MontoTotalRecargas)
        {
            int idOrden = repOrden.CrearOrden(62, detalleOrden.ToList(), MontoTotalRecargas);
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
                viewmodel.Title = "Suma / Ordenes de Recarga / Crear Orden";
                viewmodel.Message = "Falló el proceso de creación de la Orden.";
                viewmodel.ControllerName = "OrdenRecargaSuma";
                viewmodel.ActionName = "FilterReview";
                return RedirectToAction("GenericView", viewmodel);
            }
        }

        public ActionResult FilterAnulacionRecarga()
        {
            DetalleOrdenRecargaPrepago dorden = new DetalleOrdenRecargaPrepago()
            {
                tipoOrden = "Orden de Anulación de Recarga Suma"
            };
            return View("FilterAnulacion", dorden);
        }

        public ActionResult FilterAnulacionAcreditacion()
        {
            DetalleOrdenRecargaPrepago dorden = new DetalleOrdenRecargaPrepago()
            {
                tipoOrden = "Orden de Anulación de Acreditación Suma"
            };
            return View("FilterAnulacion", dorden);
        }


        [HttpPost]
        public ActionResult FilterAnulacion(string batchid, string tipoorden)
        {
            string destino;
            if (tipoorden == "Orden de Anulación de Recarga Suma")
            {
                destino = "FilterAnulacionRecarga";
            }
            else
            {
                destino = "FilterAnulacionAcreditacion";
            }
            List<DetalleOrdenRecargaPrepago> detalleOrden = repOrden.DetalleParaOrdenAnulacion(batchid, tipoorden);
            if (detalleOrden.Count == 0)
            {
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Suma / Ordenes de Recarga / Crear Orden de Anulación";
                viewmodel.Message = "No se encontró Recarga con la Referencia indicada";
                viewmodel.ControllerName = "OrdenRecargaSuma";
                viewmodel.ActionName = destino;
                return RedirectToAction("GenericView", viewmodel);
            }
            else if (detalleOrden.First().batchid == "Ya tiene Anulación")
            {
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Suma / Ordenes de Recarga / Crear Orden de Anulación";
                viewmodel.Message = "La Referencia indicada ya tiene Orden de Anulación";
                viewmodel.ControllerName = "OrdenRecargaSuma";
                viewmodel.ActionName = destino;
                return RedirectToAction("GenericView", viewmodel);
            }
            else if (detalleOrden.First().batchid == "Órdenes no corresponden")
            {
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Suma / Ordenes de Recarga / Crear Orden de Anulación";
                viewmodel.Message = "La Referencia indicada corresponde a otro tipo de orden";
                viewmodel.ControllerName = "OrdenRecargaSuma";
                viewmodel.ActionName = destino;
                return RedirectToAction("GenericView", viewmodel);
            }
            else
            {
                return View("CreateAnulacion", detalleOrden);
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
                if (repOrden.GuardarOrden(detalleOrden.ToList(), MontoTotalRecargas))
                {
                    if (repOrden.AprobarOrden(repOrden.FindDetalleOrden(detalleOrden.First().idOrden), MontoTotalRecargas))
                    {
                        
                        viewmodel.Title = "Suma / Ordenes de Recarga / Detalle de la Orden";
                        viewmodel.Message = "Orden Aprobada.";
                        viewmodel.ControllerName = "OrdenRecargaSuma";
                        viewmodel.ActionName = "FilterReview";
                        return RedirectToAction("GenericView", viewmodel);
                        //return RedirectToAction("DetalleOrden", new { id = id, idOrden = idOrden });
                    }
                    else
                    {
                        viewmodel.Title = "Suma / Ordenes de Recarga / Detalle de la Orden";
                        viewmodel.Message = "Falló el proceso de aprobación de la Orden.";
                        viewmodel.ControllerName = "OrdenRecargaSuma";
                        viewmodel.ActionName = "FilterReview";
                        return RedirectToAction("GenericView", viewmodel);
                    }
                }
                else
                {
                    viewmodel.Title = "Suma / Ordenes de Recarga / Detalle de la Orden";
                    viewmodel.Message = "Falló el proceso de guardado para aprobación de la Orden.";
                    viewmodel.ControllerName = "OrdenRecargaSuma";
                    viewmodel.ActionName = "FilterReview";
                    return RedirectToAction("GenericView", viewmodel);
                }
            }
            else
            {
                if (repOrden.GuardarOrden(detalleOrden.ToList(), MontoTotalRecargas))
                {
                    viewmodel.Title = "Suma / Ordenes de Recarga / Detalle de la Orden";
                    viewmodel.Message = "Datos de la Orden actualizados.";
                    viewmodel.ControllerName = "OrdenRecargaSuma";
                    viewmodel.ActionName = "FilterReview";
                    return RedirectToAction("GenericView", viewmodel);
                    //return RedirectToAction("DetalleOrden", new { id = id, idOrden = idOrden });
                }
                else
                {
                    viewmodel.Title = "Suma / Ordenes de Recarga / Detalle de la Orden";
                    viewmodel.Message = "Falló el proceso de guardado de la Orden.";
                    viewmodel.ControllerName = "OrdenRecargaSuma";
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
                viewmodel.Title = "Suma / Ordenes de Recarga / Detalle de la Orden";
                viewmodel.Message = "Orden Rechazada.";
                viewmodel.ControllerName = "OrdenRecargaSuma";
                viewmodel.ActionName = "FilterReview";
            }
            else
            {
                viewmodel.Title = "Suma / Ordenes de Recarga / Detalle de la Orden";
                viewmodel.Message = "Falló el proceso de rechazo de la Orden.";
                viewmodel.ControllerName = "OrdenRecargaSuma";
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
                viewmodel.Title = "Suma / Ordenes de Recarga / Detalle de la Orden / Procesar Orden";
                viewmodel.Message = "Falló el procesamiento de la Orden.";
                viewmodel.ControllerName = "OrdenRecargaSuma";
                viewmodel.ActionName = "FilterReview";
                return RedirectToAction("GenericView", viewmodel);
            }
        }        

        public ActionResult FilterReview()
        {
            OrdenRecargaPrepago orden = new OrdenRecargaPrepago();
            orden.ClaseDeOrdenOptions = orden.ClaseDeOrdenOptions.Where(x => x.id == "" || x.id == "Orden de Acreditación Suma" || x.id == "Orden de Recarga Suma" || x.id == "Orden de Anulación de Acreditación Suma" || x.id == "Orden de Anulación de Recarga Suma");
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
                    ordenes.Add(orden);
                }
            }
            else
            {
                ordenes = repOrden.Find(fecha, estadoOrden, Referencia, claseOrden, Observaciones)
                                  .Where(o => o.tipoOrden == "Orden de Acreditación Suma" || o.tipoOrden == "Orden de Recarga Suma" || o.tipoOrden == "Orden de Anulación de Acreditación Suma" || o.tipoOrden == "Orden de Anulación de Recarga Suma")
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
