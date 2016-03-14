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
        AfiliadoSumaRepository repAfiliado = new AfiliadoSumaRepository();
        BeneficiarioPrepagoRepository repBeneficiario = new BeneficiarioPrepagoRepository();
        OperacionesRepository repOperaciones = new OperacionesRepository();

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
            transferencia.nameAfiliadoDestino = afiliadoDestino.name;
            transferencia.lastname1AfiliadoDestino = afiliadoDestino.lastname1;
            transferencia.datosCuentaSumaDestino = SaldosMovimientos.Saldos.First(x => x.accounttype.Equals(Globals.TIPO_CUENTA_SUMA));
            transferencia.datosCuentaPrepagoDestino = SaldosMovimientos.Saldos.First(x => x.accounttype.Equals(Globals.TIPO_CUENTA_PREPAGO));
            return View(transferencia);
        }

        [HttpPost]
        public ActionResult ProcesarTransferencia(Transferencia transferencia)
        {
            string respuestaSuma = "";
            string respuestaPrepago = "";
            string mensaje = "";
            //realizar transferencias
            if (transferencia.ResumenTransferenciaSuma != "0")
            {
                respuestaSuma = repOperaciones.Transferir(transferencia.docnumberAfiliadoOrigen, transferencia.docnumberAfiliadoDestino, Globals.TIPO_CUENTA_SUMA, transferencia.ResumenTransferenciaSuma);
                long number1 = 0;
                bool canConvert = long.TryParse(respuestaSuma, out number1);
                if (canConvert == false)
                {
                    mensaje = "Falló transferencia Suma (" + respuestaSuma + "). ";
                }
                else
                {
                    mensaje = "Transferencia Suma exitosa con clave " + respuestaSuma + ". ";
                }
            }
            if (transferencia.ResumenTransferenciaPrepago != "0,00")
            {
                respuestaPrepago = repOperaciones.Transferir(transferencia.docnumberAfiliadoOrigen, transferencia.docnumberAfiliadoDestino, Globals.TIPO_CUENTA_PREPAGO, transferencia.ResumenTransferenciaPrepago);
                long number1 = 0;
                bool canConvert = long.TryParse(respuestaPrepago, out number1);
                if (canConvert == false)
                {
                    mensaje = mensaje + "Falló transferencia Prepago (" + respuestaPrepago + ").";
                }
                {
                    mensaje = mensaje + "Transferencia Prepago exitosa con clave " + respuestaPrepago + ".";
                }
            }
            //mensaje de confirmacion  
            ViewModel viewmodel = new ViewModel();
            viewmodel.Title = "Operaciones / Transferencia de Saldo / Procesar Transferencia";
            viewmodel.Message = mensaje;
            viewmodel.ControllerName = "Transferencia";
            viewmodel.ActionName = "FilterOrigen";
            return RedirectToAction("GenericView", viewmodel);

        }

        public ActionResult GenericView(ViewModel viewmodel)
        {
            return View(viewmodel);
        }

    }
}
