using Suma2Lealtad.Filters;
using Suma2Lealtad.Models;
using Suma2Lealtad.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Suma2Lealtad.Controllers
{
    [AuditingFilter]
    [HandleError]
    public class ReporteController : Controller
    {
        private BeneficiarioPrepagoRepository repBeneficiario = new BeneficiarioPrepagoRepository();
        private AfiliadoSumaRepository repAfiliado = new AfiliadoSumaRepository();
        private ReportesRepository repReportes = new ReportesRepository();

        public ActionResult FilterReporteComprasConsolidado()
        {
            ReportePrepago reporte = new ReportePrepago();
            reporte.ListaClientes = repBeneficiario.GetClientes();
            reporte.ListaClientes.Insert(0, new PrepaidCustomer { id = 0, name = "" });
            return View(reporte);
        }

        public ActionResult FilterReporteComprasDetallado()
        {
            ReportePrepago reporte = new ReportePrepago();
            reporte.ListaClientes = repBeneficiario.GetClientes();
            reporte.ListaClientes.Insert(0, new PrepaidCustomer { id = 0, name = "" });
            return View(reporte);
        }

        public ActionResult FilterReporteRecargasConsolidado()
        {
            ReportePrepago reporte = new ReportePrepago();
            reporte.ListaClientes = repBeneficiario.GetClientes();
            reporte.ListaClientes.Insert(0, new PrepaidCustomer { id = 0, name = "" });
            return View(reporte);
        }

        public ActionResult FilterReporteRecargasDetallado()
        {
            ReportePrepago reporte = new ReportePrepago();
            reporte.ListaClientes = repBeneficiario.GetClientes();
            reporte.ListaClientes.Insert(0, new PrepaidCustomer { id = 0, name = "" });
            return View(reporte);
        }

        public ActionResult FilterReporteTransaccionesSuma()
        {
            ReportePrepago reporte = new ReportePrepago();
            return View(reporte);
        }

        public ActionResult FilterReporteTransaccionesPrepago()
        {
            ReportePrepago reporte = new ReportePrepago();
            return View(reporte);
        }

        public ActionResult FilterReporteTarjetas()
        {
            ReportePrepago reporte = new ReportePrepago();
            reporte.ListaClientes = repBeneficiario.GetClientes();
            reporte.ListaClientes.Insert(0, new PrepaidCustomer { id = 0, name = "" });
            return View(reporte);
        }

        [HttpPost]
        public ActionResult ReporteComprasConsolidado(string ModoTransaccion, string fechadesde, string fechahasta, int idCliente = 0)
        {
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            if (idCliente == 0)
            {
                reporte = repReportes.ReporteDeComprasConsolidado("todos", fechadesde, fechahasta, ModoTransaccion, idCliente);
            }
            else
            {
                reporte = repReportes.ReporteDeComprasConsolidado("uno", fechadesde, fechahasta, ModoTransaccion, idCliente);
            }
            ParametrosReporte p = new ParametrosReporte()
            {
                ModoTransaccion = ModoTransaccion,
                fechadesde = fechadesde,
                fechahasta = fechahasta,
                idCliente = idCliente,
            };
            if (reporte.Count == 0)
            {
                ReportePrepago r = new ReportePrepago()
                {
                    Parametros = p
                };
                reporte.Add(r);
            }
            else
            {
                reporte.First().Parametros = p;
            }
            return View(reporte);
        }

        public ActionResult GenerateReporteComprasConsolidadoPDF(string ModoTransaccion, string fechadesde, string fechahasta, int idCliente = 0)
        {
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            if (idCliente == 0)
            {
                reporte = repReportes.ReporteDeComprasConsolidado("todos", fechadesde, fechahasta, ModoTransaccion, idCliente);
            }
            else
            {
                reporte = repReportes.ReporteDeComprasConsolidado("uno", fechadesde, fechahasta, ModoTransaccion, idCliente);
            }
            string footer = "--footer-right \"Fecha: [date] [time]\" " + "--footer-center \"Página: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";
            return new Rotativa.ViewAsPdf("ReporteComprasConsolidadoPDF", reporte)
            {
                FileName = "Reporte de Compras Consolidado.pdf",
                CustomSwitches = footer
            };
        }

        [HttpPost]
        public ActionResult ReporteComprasDetallado(string TipoDetalle, string ModoTransaccion, string fechadesde, string fechahasta, int idCliente)
        {
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            if (TipoDetalle == "Consolidado por Beneficiario")
            {
                reporte = repReportes.ReporteDeComprasDetallado("consolidado", fechadesde, fechahasta, ModoTransaccion, idCliente);
            }
            else
            {
                reporte = repReportes.ReporteDeComprasDetallado("detallado", fechadesde, fechahasta, ModoTransaccion, idCliente);
            }
            ParametrosReporte p = new ParametrosReporte()
            {
                TipoDetalle = TipoDetalle,
                ModoTransaccion = ModoTransaccion,
                fechadesde = fechadesde,
                fechahasta = fechahasta,
                idCliente = idCliente,
            };
            if (reporte.Count == 0)
            {
                ReportePrepago r = new ReportePrepago()
                {
                    Parametros = p
                };
                reporte.Add(r);
            }
            else
            {
                reporte.First().Parametros = p;
            }
            return View(reporte);
        }
        
        public ActionResult GenerateReporteComprasDetalladoPDF(string TipoDetalle, string ModoTransaccion, string fechadesde, string fechahasta, int idCliente)
        {
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            if (TipoDetalle == "Consolidado por Beneficiario")
            {
                reporte = repReportes.ReporteDeComprasDetallado("consolidado", fechadesde, fechahasta, ModoTransaccion, idCliente);
            }
            else
            {
                reporte = repReportes.ReporteDeComprasDetallado("detallado", fechadesde, fechahasta, ModoTransaccion, idCliente);
            }            
            string footer = "--footer-right \"Fecha: [date] [time]\" " + "--footer-center \"Página: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";
            return new Rotativa.ViewAsPdf("ReporteComprasDetalladoPDF", reporte)
            {
                FileName = "Reporte de Compras Detallado.pdf",
                CustomSwitches = footer
            };
        }

        [HttpPost]
        public ActionResult ReporteRecargasConsolidado(string fechadesde, string fechahasta, int idCliente = 0, string Referencia = "", string Observaciones = "")
        {
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            if (idCliente == 0)
            {
                reporte = repReportes.ReporteRecargasConsolidado("todos", fechadesde, fechahasta, idCliente, Referencia, Observaciones);
            }
            else
            {
                reporte = repReportes.ReporteRecargasConsolidado("uno", fechadesde, fechahasta, idCliente, Referencia, Observaciones);
            }
            ParametrosReporte p = new ParametrosReporte()
            {
                fechadesde = fechadesde,
                fechahasta = fechahasta,
                idCliente = idCliente,
                referencia = Referencia,
                observaciones = Observaciones            
            };
            if (reporte.Count == 0)
            {
                ReportePrepago r = new ReportePrepago()
                {
                    Parametros = p
                };
                reporte.Add(r);
            }
            else
            {
                reporte.First().Parametros = p;
            }
            return View(reporte);
        }

        public ActionResult GenerateReporteRecargasConsolidadoPDF(string fechadesde, string fechahasta, int idCliente = 0, string Referencia = "", string Observaciones = "")
        {
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            if (idCliente == 0)
            {
                reporte = repReportes.ReporteRecargasConsolidado("todos", fechadesde, fechahasta, idCliente, Referencia, Observaciones);
            }
            else
            {
                reporte = repReportes.ReporteRecargasConsolidado("uno", fechadesde, fechahasta, idCliente, Referencia, Observaciones);
            }
            string footer = "--footer-right \"Fecha: [date] [time]\" " + "--footer-center \"Página: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";
            return new Rotativa.ViewAsPdf("ReporteRecargasConsolidadoPDF", reporte)
            {
                FileName = "Reporte de Recargas Consolidado.pdf",
                CustomSwitches = footer
            };
        }

        [HttpPost]
        public ActionResult ReporteRecargasDetallado(string TipoDetalle, string fechadesde, string fechahasta, int idCliente, string Referencia = "", string Observaciones = "")
        {
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            if (TipoDetalle == "Consolidado por Beneficiario")
            {
                reporte = repReportes.ReporteRecargasDetallado("consolidado", fechadesde, fechahasta, idCliente, Referencia, Observaciones);
            }
            else
            {
                reporte = repReportes.ReporteRecargasDetallado("detallado", fechadesde, fechahasta, idCliente, Referencia, Observaciones);
            }
            ParametrosReporte p = new ParametrosReporte()
            {
                TipoDetalle = TipoDetalle,
                fechadesde = fechadesde,
                fechahasta = fechahasta,
                idCliente = idCliente,
                referencia = Referencia,
                observaciones = Observaciones
            };
            if (reporte.Count == 0)
            {
                ReportePrepago r = new ReportePrepago()
                {
                    Parametros = p
                };
                reporte.Add(r);
            }
            else
            {
                reporte.First().Parametros = p;
            }
            return View(reporte);
        }

        public ActionResult GenerateReporteRecargasDetalladoPDF(string TipoDetalle, string fechadesde, string fechahasta, int idCliente, string Referencia = "", string Observaciones = "")
        {
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            if (TipoDetalle == "Consolidado por Beneficiario")
            {
                reporte = repReportes.ReporteRecargasDetallado("consolidado", fechadesde, fechahasta, idCliente, Referencia, Observaciones);
            }
            else
            {
                reporte = repReportes.ReporteRecargasDetallado("detallado", fechadesde, fechahasta, idCliente, Referencia, Observaciones);
            }
            string footer = "--footer-right \"Fecha: [date] [time]\" " + "--footer-center \"Página: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";
            return new Rotativa.ViewAsPdf("ReporteRecargasDetalladoPDF", reporte)
            {
                FileName = "Reporte de Recargas Detallado.pdf",
                CustomSwitches = footer
            };
        }

        [HttpPost]
        public ActionResult ReporteTransaccionesSuma(string TipoTransaccion, string fechadesde, string fechahasta, string numdoc)
        {
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            //reporte = repReportes.ReporteTransaccionesSuma(TipoTransaccion,fechadesde,fechahasta,numdoc)
            ParametrosReporte p = new ParametrosReporte()
            {
                TipoTransaccion = TipoTransaccion,
                fechadesde = fechadesde,
                fechahasta = fechahasta,
                numdoc = numdoc
            };
            if (reporte.Count == 0)
            {
                ReportePrepago r = new ReportePrepago()
                {
                    Parametros = p
                };
                reporte.Add(r);
            }
            else
            {
                reporte.First().Parametros = p;
            }
            return View(reporte);
        }

        public ActionResult GenerateReporteTransaccionesSumaPDF(string TipoTransaccion, string fechadesde, string fechahasta, string numdoc)
        {
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            //reporte = repReportes.ReporteTransaccionesSuma(TipoTransaccion,fechadesde,fechahasta,numdoc)
            string footer = "--footer-right \"Fecha: [date] [time]\" " + "--footer-center \"Página: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";
            return new Rotativa.ViewAsPdf("ReporteTransaccionesSumaPDF", reporte)
            {
                FileName = "Reporte de Transacciones Suma.pdf",
                CustomSwitches = footer
            };
        }

        [HttpPost]
        public ActionResult ReporteTransaccionesPrepago(string TipoTransaccion, string fechadesde, string fechahasta, string numdoc)
        {
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            //reporte = repReportes.ReporteTransaccionesPrepago(TipoTransaccion,fechadesde,fechahasta,numdoc)
            ParametrosReporte p = new ParametrosReporte()
            {
                TipoTransaccion = TipoTransaccion,
                fechadesde = fechadesde,
                fechahasta = fechahasta,
                numdoc = numdoc
            };
            if (reporte.Count == 0)
            {
                ReportePrepago r = new ReportePrepago()
                {
                    Parametros = p
                };
                reporte.Add(r);
            }
            else
            {
                reporte.First().Parametros = p;
            }
            return View(reporte);
        }

        public ActionResult GenerateReporteTransaccionesPrepagoPDF(string TipoTransaccion, string fechadesde, string fechahasta, string numdoc)
        {
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            //reporte = repReportes.ReporteTransaccionesPrepago(TipoTransaccion,fechadesde,fechahasta,numdoc)
            string footer = "--footer-right \"Fecha: [date] [time]\" " + "--footer-center \"Página: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";
            return new Rotativa.ViewAsPdf("ReporteTransaccionesSumaPDF", reporte)
            {
                FileName = "Reporte de Transacciones Prepago.pdf",
                CustomSwitches = footer
            };
        }

        [HttpPost]
        public ActionResult ReporteTarjetas(string fechadesde, string fechahasta, int idCliente = 0, string estadoTarjeta = "")
        {
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            if (idCliente == 0)
            {
                //reporte = repReportes.Tarjetas("todos", fechadesde, fechahasta, idCliente, estadoTarjeta);
            }
            else
            {
                //reporte = repReportes.Tarjetas("uno", fechadesde, fechahasta, idCliente, estadoTarjeta);
            }
            ParametrosReporte p = new ParametrosReporte()
            {
                fechadesde = fechadesde,
                fechahasta = fechahasta,
                idCliente = idCliente,
                estatusTarjeta = estadoTarjeta
            };
            if (reporte.Count == 0)
            {
                ReportePrepago r = new ReportePrepago()
                {
                    Parametros = p
                };
                reporte.Add(r);
            }
            else
            {
                reporte.First().Parametros = p;
            }
            return View(reporte);
        }

        public ActionResult GenerateReporteTarjetasPDF(string fechadesde, string fechahasta, int idCliente = 0, string estadoTarjeta = "")
        {
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            if (idCliente == 0)
            {
                //reporte = repReportes.Tarjetas("todos", fechadesde, fechahasta, idCliente, estadoTarjeta);
            }
            else
            {
                //reporte = repReportes.Tarjetas("uno", fechadesde, fechahasta, idCliente, estadoTarjeta);
            }
            string footer = "--footer-right \"Fecha: [date] [time]\" " + "--footer-center \"Página: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";
            return new Rotativa.ViewAsPdf("ReporteTarjetasPDF", reporte)
            {
                FileName = "Reporte de Tarjetas.pdf",
                CustomSwitches = footer
            };
        }

    }

}
