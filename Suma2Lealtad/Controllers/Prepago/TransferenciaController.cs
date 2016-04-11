using Suma2Lealtad.Filters;
using Suma2Lealtad.Models;
using Suma2Lealtad.Models.Repositorios;
using Suma2Lealtad.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Suma2Lealtad.Controllers.Prepago
{
    [AuditingFilter]
    [HandleError]
    public class TransferenciaController : Controller
    {
        private ClientePrepagoRepository repCliente = new ClientePrepagoRepository();
        AfiliadoSumaRepository repAfiliado = new AfiliadoSumaRepository();
        BeneficiarioPrepagoRepository repBeneficiario = new BeneficiarioPrepagoRepository();
        OperacionesRepository repOperaciones = new OperacionesRepository();
        private OrdenRecargaRepository repOrden = new OrdenRecargaRepository();


        public ActionResult FilterOrigen()
        {
            return View("FilterTransferencia");
        }

        [HttpPost]
        public ActionResult DetalleOrigen(string numdoc, string action = "post")
        {
            AfiliadoSumaIndex a;
            AfiliadoSuma afiliadoOrigen;
            //SI ESTOY EN SESION PREPAGO, SOLO BUSCO BENEFICIARIOS PREPAGO
            if ((string)Session["type"] == "Prepago")
            {
                a = repAfiliado.Find(numdoc, "", "", "", "").FirstOrDefault();
                if (a == null)
                {
                    ViewModel viewmodel = new ViewModel();
                    viewmodel.Title = "Operaciones / Transferencia de Saldo / Filtro de Búsqueda de Origen";
                    viewmodel.Message = "El número de documento origen " + numdoc + " no esta registrado.";
                    viewmodel.ControllerName = "Transferencia";
                    viewmodel.ActionName = "FilterOrigen";
                    return RedirectToAction("GenericView", viewmodel);
                }
                else if (a.type != "Prepago")
                {
                    ViewModel viewmodel = new ViewModel();
                    viewmodel.Title = "Operaciones / Transferencia de Saldo / Filtro de Búsqueda de Origen";
                    viewmodel.Message = "El número de documento origen " + numdoc + " no es beneficiario Prepago Plaza's, no puede hacer transferencias.";
                    viewmodel.ControllerName = "Transferencia";
                    viewmodel.ActionName = "FilterOrigen";
                    return RedirectToAction("GenericView", viewmodel);
                }
                else
                {
                    afiliadoOrigen = repAfiliado.Find(a.id);
                }
            }
            //SI ESTOY EN SESION SUMA, SOLO BUSCO BENEFICIARIOS SUMA
            else
            {
                a = repAfiliado.Find(numdoc, "", "", "", "").FirstOrDefault();
                if (a == null)
                {
                    ViewModel viewmodel = new ViewModel();
                    viewmodel.Title = "Operaciones / Transferencia de Saldo / Filtro de Búsqueda de Origen";
                    viewmodel.Message = "El número de documento origen " + numdoc + " no esta registrado.";
                    viewmodel.ControllerName = "Transferencia";
                    viewmodel.ActionName = "FilterOrigen";
                    return RedirectToAction("GenericView", viewmodel);
                }
                else if (a.type != "Suma")
                {
                    ViewModel viewmodel = new ViewModel();
                    viewmodel.Title = "Operaciones / Transferencia de Saldo / Filtro de Búsqueda de Origen";
                    viewmodel.Message = "El número de documento origen " + numdoc + " es beneficiario Prepago Plaza's, no puede hacer transferencias.";
                    viewmodel.ControllerName = "Transferencia";
                    viewmodel.ActionName = "FilterOrigen";
                    return RedirectToAction("GenericView", viewmodel);
                }
                else
                {
                    afiliadoOrigen = repAfiliado.Find(a.id);
                }
            }
            SaldosMovimientos SaldosMovimientos = repAfiliado.FindSaldosMovimientos(afiliadoOrigen);
            Transferencia transferencia = new Transferencia()
            {
                docnumberAfiliadoOrigen = afiliadoOrigen.docnumber,
                idAfiliadoOrigen = afiliadoOrigen.id,
                nameAfiliadoOrigen = afiliadoOrigen.name,
                lastname1AfiliadoOrigen = afiliadoOrigen.lastname1,
                typeAfiliadoOrigen = afiliadoOrigen.type,
                datosCuentaSumaOrigen = SaldosMovimientos.Saldos.First(x => x.accounttype.Equals(Globals.TIPO_CUENTA_SUMA)),
                DenominacionCuentaOrigenSuma = "Más",
                datosCuentaPrepagoOrigen = SaldosMovimientos.Saldos.First(x => x.accounttype.Equals(Globals.TIPO_CUENTA_PREPAGO)),
                DenominacionCuentaOrigenPrepago = "Bs."
            };
            return View("DetalleTransferencia", transferencia);
        }

        public ActionResult DetalleOrigen(string numdoc)
        {
            AfiliadoSumaIndex a;
            AfiliadoSuma afiliadoOrigen;
            //SI ESTOY EN SESION PREPAGO, SOLO BUSCO BENEFICIARIOS PREPAGO
            if ((string)Session["type"] == "Prepago")
            {
                a = repAfiliado.Find(numdoc, "", "", "", "").FirstOrDefault();
                if (a == null)
                {
                    ViewModel viewmodel = new ViewModel();
                    viewmodel.Title = "Operaciones / Transferencia de Saldo / Filtro de Búsqueda de Origen";
                    viewmodel.Message = "El número de documento origen " + numdoc + " no esta registrado.";
                    viewmodel.ControllerName = "Transferencia";
                    viewmodel.ActionName = "FilterOrigen";
                    return RedirectToAction("GenericView", viewmodel);
                }
                else if (a.type != "Prepago")
                {
                    ViewModel viewmodel = new ViewModel();
                    viewmodel.Title = "Operaciones / Transferencia de Saldo / Filtro de Búsqueda de Origen";
                    viewmodel.Message = "El número de documento origen " + numdoc + " no es beneficiario Prepago Plaza's, no puede hacer transferencias.";
                    viewmodel.ControllerName = "Transferencia";
                    viewmodel.ActionName = "FilterOrigen";
                    return RedirectToAction("GenericView", viewmodel);
                }
                else
                {
                    afiliadoOrigen = repAfiliado.Find(a.id);
                }
            }
            //SI ESTOY EN SESION SUMA, SOLO BUSCO BENEFICIARIOS SUMA
            else
            {
                a = repAfiliado.Find(numdoc, "", "", "", "").FirstOrDefault();
                if (a == null)
                {
                    ViewModel viewmodel = new ViewModel();
                    viewmodel.Title = "Operaciones / Transferencia de Saldo / Filtro de Búsqueda de Origen";
                    viewmodel.Message = "El número de documento " + numdoc + " no esta registrado.";
                    viewmodel.ControllerName = "Transferencia";
                    viewmodel.ActionName = "FilterOrigen";
                    return RedirectToAction("GenericView", viewmodel);
                }
                else if (a.type != "Suma")
                {
                    ViewModel viewmodel = new ViewModel();
                    viewmodel.Title = "Operaciones / Transferencia de Saldo / Filtro de Búsqueda de Origen";
                    viewmodel.Message = "El número de documento " + numdoc + " es beneficiario Prepago Plaza's, no puede hacer transferencias.";
                    viewmodel.ControllerName = "Transferencia";
                    viewmodel.ActionName = "FilterOrigen";
                    return RedirectToAction("GenericView", viewmodel);
                }
                else
                {
                    afiliadoOrigen = repAfiliado.Find(a.id);
                }
            }
            SaldosMovimientos SaldosMovimientos = repAfiliado.FindSaldosMovimientos(afiliadoOrigen);
            Transferencia transferencia = new Transferencia()
            {
                docnumberAfiliadoOrigen = afiliadoOrigen.docnumber,
                idAfiliadoOrigen = afiliadoOrigen.id,
                nameAfiliadoOrigen = afiliadoOrigen.name,
                lastname1AfiliadoOrigen = afiliadoOrigen.lastname1,
                typeAfiliadoOrigen = afiliadoOrigen.type,
                datosCuentaSumaOrigen = SaldosMovimientos.Saldos.First(x => x.accounttype.Equals(Globals.TIPO_CUENTA_SUMA)),
                DenominacionCuentaOrigenSuma = "Más",
                datosCuentaPrepagoOrigen = SaldosMovimientos.Saldos.First(x => x.accounttype.Equals(Globals.TIPO_CUENTA_PREPAGO)),
                DenominacionCuentaOrigenPrepago = "Bs."
            };
            return View("DetalleTransferencia", transferencia);
        }

        [HttpPost]
        public ActionResult DetalleTransferencia(Transferencia transferencia, string numdoc = "")
        {
            AfiliadoSumaIndex a;
            AfiliadoSuma afiliadoDestino;
            //VERIFICO QUE EL NRO DOCUMENTO ORIGEN Y DESTINO SEAN DISTINTOS
            if (numdoc == transferencia.docnumberAfiliadoOrigen)
            {
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Operaciones / Transferencia de Saldo / Filtro de Búsqueda de Destino";
                viewmodel.Message = "El número de documento destino no puede ser igual al número de documento origen.";
                viewmodel.ControllerName = "Transferencia";
                viewmodel.ActionName = "DetalleOrigen";
                viewmodel.RouteValues = "?numdoc=" + transferencia.docnumberAfiliadoOrigen;
                return RedirectToAction("GenericView", viewmodel);
            }
            //SI ESTOY EN SESION PREPAGO, SOLO BUSCO BENEFICIARIOS PREPAGO
            if ((string)Session["type"] == "Prepago")
            {
                a = repAfiliado.Find(numdoc, "", "", "", "").FirstOrDefault();
                if (a == null)
                {
                    ViewModel viewmodel = new ViewModel();
                    viewmodel.Title = "Operaciones / Transferencia de Saldo / Filtro de Búsqueda de Origen";
                    viewmodel.Message = "El número de documento destino " + numdoc + " no esta registrado.";
                    viewmodel.ControllerName = "Transferencia";
                    viewmodel.ActionName = "DetalleOrigen";
                    viewmodel.RouteValues = "?numdoc=" + transferencia.docnumberAfiliadoOrigen;
                    return RedirectToAction("GenericView", viewmodel);
                }
                else if (a.type != "Prepago")
                {
                    ViewModel viewmodel = new ViewModel();
                    viewmodel.Title = "Operaciones / Transferencia de Saldo / Filtro de Búsqueda de Origen";
                    viewmodel.Message = "El número de documento destino " + numdoc + " no es beneficiario Prepago Plaza's, no puede hacer transferencias.";
                    viewmodel.ControllerName = "Transferencia";
                    viewmodel.ActionName = "DetalleOrigen";
                    viewmodel.RouteValues = "?numdoc=" + transferencia.docnumberAfiliadoOrigen;
                    return RedirectToAction("GenericView", viewmodel);
                }
                else
                {
                    afiliadoDestino = repAfiliado.Find(a.id);
                }
            }
            //SI ESTOY EN SESION SUMA, SOLO BUSCO BENEFICIARIOS SUMA
            else
            {
                a = repAfiliado.Find(numdoc, "", "", "", "").FirstOrDefault();
                if (a == null)
                {
                    ViewModel viewmodel = new ViewModel();
                    viewmodel.Title = "Operaciones / Transferencia de Saldo / Filtro de Búsqueda de Origen";
                    viewmodel.Message = "El número de documento destino " + numdoc + " no esta registrado.";
                    viewmodel.ControllerName = "Transferencia";
                    viewmodel.ActionName = "DetalleOrigen";
                    viewmodel.RouteValues = "?numdoc=" + transferencia.docnumberAfiliadoOrigen;
                    return RedirectToAction("GenericView", viewmodel);
                }
                else if (a.type != "Suma")
                {
                    ViewModel viewmodel = new ViewModel();
                    viewmodel.Title = "Operaciones / Transferencia de Saldo / Filtro de Búsqueda de Origen";
                    viewmodel.Message = "El número de documento destino " + numdoc + " es beneficiario Prepago Plaza's, no puede hacer transferencias.";
                    viewmodel.ControllerName = "Transferencia";
                    viewmodel.ActionName = "DetalleOrigen";
                    viewmodel.RouteValues = "?numdoc=" + transferencia.docnumberAfiliadoOrigen;
                    return RedirectToAction("GenericView", viewmodel);
                }
                else
                {
                    afiliadoDestino = repAfiliado.Find(a.id);
                }
            }
            SaldosMovimientos SaldosMovimientos = repAfiliado.FindSaldosMovimientos(afiliadoDestino);
            transferencia.docnumberAfiliadoDestino = afiliadoDestino.docnumber;
            transferencia.idAfiliadoDestino = afiliadoDestino.id;
            transferencia.nameAfiliadoDestino = afiliadoDestino.name;
            transferencia.lastname1AfiliadoDestino = afiliadoDestino.lastname1;
            transferencia.datosCuentaSumaDestino = SaldosMovimientos.Saldos.First(x => x.accounttype.Equals(Globals.TIPO_CUENTA_SUMA));
            transferencia.datosCuentaPrepagoDestino = SaldosMovimientos.Saldos.First(x => x.accounttype.Equals(Globals.TIPO_CUENTA_PREPAGO));
            transferencia.ResumenTransferenciaSuma = "0";
            transferencia.ResumenTransferenciaPrepago = "0,00";
            return View(transferencia);
        }

        [HttpPost]
        public ActionResult CrearOrdenTransferencia(Transferencia transferencia)
        {
            //crear detalle de orden
            int idCliente = 62;
            List<DetalleOrdenRecargaPrepago> detalleOrden = new List<DetalleOrdenRecargaPrepago>();
            DetalleOrdenRecargaPrepago datosorigensuma = new DetalleOrdenRecargaPrepago()
            {
                idAfiliado = transferencia.idAfiliadoOrigen,
                montoRecarga = Convert.ToDecimal(transferencia.ResumenTransferenciaSuma),
                tipoOrden = "Orden de Transferencia"
            };
            detalleOrden.Add(datosorigensuma);
            DetalleOrdenRecargaPrepago datosorigenprepago = new DetalleOrdenRecargaPrepago()
            {
                idAfiliado = transferencia.idAfiliadoOrigen,
                montoRecarga = Convert.ToDecimal(transferencia.ResumenTransferenciaPrepago),
                tipoOrden = "Orden de Transferencia"
            };
            detalleOrden.Add(datosorigenprepago);
            DetalleOrdenRecargaPrepago datosdestinosuma = new DetalleOrdenRecargaPrepago()
            {
                idAfiliado = transferencia.idAfiliadoDestino,
                montoRecarga = Convert.ToDecimal(transferencia.ResumenTransferenciaSuma),
                tipoOrden = "Orden de Transferencia"
            };
            detalleOrden.Add(datosdestinosuma);
            DetalleOrdenRecargaPrepago datosdestinoprepago = new DetalleOrdenRecargaPrepago()
            {
                idAfiliado = transferencia.idAfiliadoDestino,
                montoRecarga = Convert.ToDecimal(transferencia.ResumenTransferenciaPrepago),
                tipoOrden = "Orden de Transferencia"
            };
            detalleOrden.Add(datosdestinoprepago);
            //crear orden            
            int idOrden = repOperaciones.CrearTransferencia(idCliente, detalleOrden);
            if (idOrden != 0)
            {
                //viewmodel.Title = "Prepago / Cliente / Ordenes de Recarga / Detalle de la Orden";
                //viewmodel.Message = "Orden Aprobada.";
                //viewmodel.ControllerName = "ClientePrepago";
                //viewmodel.ActionName = "FilterOrdenes";
                //viewmodel.RouteValues = id.ToString();
                OrdenRecargaPrepago orden = repOrden.Find(idOrden);
                transferencia.id = idOrden;
                transferencia.creationdateOrden = orden.creationdateOrden;
                transferencia.montoOrden = orden.montoOrden;
                transferencia.statusOrden = orden.statusOrden;
                transferencia.tipoOrden = orden.tipoOrden;
                return View("DetalleTransferencia", transferencia);
            }
            else
            {
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Operaciones / Transferencia de Saldo / Crear Orden de Transferencia";
                viewmodel.Message = "Falló el proceso de creación de la Orden.";
                viewmodel.ControllerName = "Transferencia";
                viewmodel.ActionName = "FilterOrigen";
                return RedirectToAction("GenericView", viewmodel);
            }
        }

        public ActionResult DetalleTransferencia(int id)
        {
            Transferencia transferencia = repOperaciones.FindTransferencia(id);
            return View("DetalleTransferencia", transferencia);
        }

        public ActionResult AprobarTransferencia(Transferencia transferencia)
        {
            ViewModel viewmodel = new ViewModel();
            int idOrderHistory = repOrden.OrdersHistoryId();
            if (repOperaciones.AprobarTransferencia(transferencia))
            {
                viewmodel.Title = "Operaciones / Transferencia de Saldo / Aprobar Transferencia";
                viewmodel.Message = "Orden Aprobada.";
                viewmodel.ControllerName = "OrdenRecargaPrepago";
                viewmodel.ActionName = "FilterReview";
                //return RedirectToAction("DetalleOrden", new { id = id, idOrden = idOrden });
            }
            else
            {
                viewmodel.Title = "Operaciones / Transferencia de Saldo / Aprobar Transferencia";
                viewmodel.Message = "Falló el proceso de aprobación de la Orden.";
                viewmodel.ControllerName = "OrdenRecargaPrepago";
                viewmodel.ActionName = "FilterReview";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult RechazarTransferencia(int id)
        {
            ViewModel viewmodel = new ViewModel();
            if (repOperaciones.RechazarTransferencia(id))
            {
                viewmodel.Title = "Operaciones / Transferencia de Saldo / Rechazar Transferencia";
                viewmodel.Message = "Orden Rechazada.";
                viewmodel.ControllerName = "OrdenRecargaPrepago";
                viewmodel.ActionName = "FilterReview";
            }
            else
            {
                viewmodel.Title = "Operaciones / Transferencia de Saldo / Rechazar Transferencia";
                viewmodel.Message = "Falló el proceso de rechazo de la Orden.";
                viewmodel.ControllerName = "OrdenRecargaPrepago";
                viewmodel.ActionName = "FilterReview";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult ProcesarTransferencia(int id)
        {
            if (repOperaciones.ProcesarTransferencia(id))
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
                Transferencia transferencia = repOperaciones.FindTransferencia(id);
                return View("DetalleTransferencia", transferencia);
            }
            else
            {
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Operaciones / Transferencia de Saldo / Procesar Transferencia";
                viewmodel.Message = "Falló el procesamiento de la Orden.";
                viewmodel.ControllerName = "OrdenRecargaPrepago";
                viewmodel.ActionName = "FilterReview";
                return RedirectToAction("GenericView", viewmodel);
            }

        }

        public ActionResult FilterAnulacionTransferencia()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FilterAnulacionTransferencia(int orden)
        {
            Transferencia transferencia = repOperaciones.DetalleParaOrdenAnulacionTransferencia(orden);
            if (transferencia == null)
            {
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Operaciones / Transferencia de Saldo / Crear Orden de Anulación";
                viewmodel.Message = "No se encontró la Orden de Transferencia";
                viewmodel.ControllerName = "Transferencia";
                viewmodel.ActionName = "FilterAnulacionTransferencia";
                return RedirectToAction("GenericView", viewmodel);
            }
            else if (transferencia.tipoOrden == "Ya tiene Anulación")
            {
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Operaciones / Transferencia de Saldo / Crear Orden de Anulación";
                viewmodel.Message = "La Orden indicada ya tiene Orden de Anulación";
                viewmodel.ControllerName = "Transferencia";
                viewmodel.ActionName = "FilterAnulacionTransferencia";
                return RedirectToAction("GenericView", viewmodel);
            }
            else
            {
                return View("CreateAnulacionTransferencia", transferencia);
            }
        }

        [HttpPost]
        public ActionResult CreateAnulacionTransferencia(Transferencia transferencia)
        {
            //crear detalle de orden
            int idCliente = 62;
            List<DetalleOrdenRecargaPrepago> detalleOrdenOrigen = repOrden.FindDetalleOrden(transferencia.id);
            List<DetalleOrdenRecargaPrepago> detalleOrden = new List<DetalleOrdenRecargaPrepago>();
            DetalleOrdenRecargaPrepago datosorigensuma = new DetalleOrdenRecargaPrepago()
            {
                idAfiliado = transferencia.idAfiliadoOrigen,
                montoRecarga = Convert.ToDecimal(transferencia.ResumenTransferenciaSuma),
                tipoOrden = "Orden de Anulación de Transferencia",
                batchid = detalleOrdenOrigen.First().batchid
            };
            detalleOrden.Add(datosorigensuma);
            DetalleOrdenRecargaPrepago datosorigenprepago = new DetalleOrdenRecargaPrepago()
            {
                idAfiliado = transferencia.idAfiliadoOrigen,
                montoRecarga = Convert.ToDecimal(transferencia.ResumenTransferenciaPrepago),
                tipoOrden = "Orden de Anulación de Transferencia",
                batchid = detalleOrdenOrigen.Skip(1).First().batchid            
            };
            detalleOrden.Add(datosorigenprepago);
            DetalleOrdenRecargaPrepago datosdestinosuma = new DetalleOrdenRecargaPrepago()
            {
                idAfiliado = transferencia.idAfiliadoDestino,
                montoRecarga = Convert.ToDecimal(transferencia.ResumenTransferenciaSuma),
                tipoOrden = "Orden de Anulación de Transferencia",
                batchid = detalleOrdenOrigen.Skip(2).First().batchid  
            };
            detalleOrden.Add(datosdestinosuma);
            DetalleOrdenRecargaPrepago datosdestinoprepago = new DetalleOrdenRecargaPrepago()
            {
                idAfiliado = transferencia.idAfiliadoDestino,
                montoRecarga = Convert.ToDecimal(transferencia.ResumenTransferenciaPrepago),
                tipoOrden = "Orden de Anulación de Transferencia",
                batchid = detalleOrdenOrigen.Skip(3).First().batchid
            };
            detalleOrden.Add(datosdestinoprepago);
            //crear orden            
            int idOrden = repOperaciones.CrearTransferencia(idCliente, detalleOrden);
            if (idOrden != 0)
            {
                //viewmodel.Title = "Prepago / Cliente / Ordenes de Recarga / Detalle de la Orden";
                //viewmodel.Message = "Orden Aprobada.";
                //viewmodel.ControllerName = "ClientePrepago";
                //viewmodel.ActionName = "FilterOrdenes";
                //viewmodel.RouteValues = id.ToString();
                transferencia = repOperaciones.FindTransferencia(idOrden);
                return View("DetalleTransferencia", transferencia);
            }
            else
            {
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Operaciones / Transferencia de Saldo / Crear Orden de Anulación";
                viewmodel.Message = "Falló el proceso de creación de la Orden.";
                viewmodel.ControllerName = "Transferencia";
                viewmodel.ActionName = "FilterOrigen";
                return RedirectToAction("GenericView", viewmodel);
            }
        }

        public ActionResult GenericView(ViewModel viewmodel)
        {
            return View(viewmodel);
        }

    }
}
