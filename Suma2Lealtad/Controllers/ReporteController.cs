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

        public ActionResult FilterReporteRecargas()
        {
            ReportePrepago reporte = new ReportePrepago();
            reporte.ListaClientes = repBeneficiario.GetClientes();
            reporte.ListaClientes.Insert(0, new PrepaidCustomer { id = 0, name = "" });
            return View(reporte);
        }

        public ActionResult FilterReporteCompras()
        {
            ReportePrepago reporte = new ReportePrepago();
            reporte.ListaClientes = repBeneficiario.GetClientes();
            reporte.ListaClientes.Insert(0, new PrepaidCustomer { id = 0, name = "" });
            return View(reporte);
        }

        public ActionResult FilterReporteTarjetas()
        {
            ReportePrepago reporte = new ReportePrepago();
            reporte.ListaClientes = repBeneficiario.GetClientes();
            reporte.ListaClientes.Insert(0, new PrepaidCustomer { id = 0, name = "" });
            return View(reporte);
        }

        public ActionResult FilterReporteTransacciones()
        {
            ReporteSuma reporte = new ReporteSuma();
            return View(reporte);
        }

        [HttpPost]
        public ActionResult ReporteCompras(string TipoConsulta, string ModoTransaccion, string fechadesde, string fechahasta, int idCliente = 0, string numdoc = "")
        {
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            if (TipoConsulta == "Cliente")
            {
                if (idCliente == 0)
                {
                    reporte = repBeneficiario.ReporteComprasxCliente("todos", fechadesde, fechahasta, ModoTransaccion);
                }
                else
                {
                    reporte = repBeneficiario.ReporteComprasxCliente("uno", fechadesde, fechahasta, ModoTransaccion, idCliente);
                }
            }
            else if (TipoConsulta == "Beneficiario")
            {
                if (numdoc == "")
                {
                    reporte = repBeneficiario.ReporteComprasxBeneficiario("todos", fechadesde, fechahasta, ModoTransaccion);
                }
                else
                {
                    reporte = repBeneficiario.ReporteComprasxBeneficiario("uno", fechadesde, fechahasta, ModoTransaccion, numdoc);
                }
            }
            ParametrosReporte p = new ParametrosReporte()
            {
                TipoConsulta = TipoConsulta,
                ModoTransaccion = ModoTransaccion,
                fechadesde = fechadesde,
                fechahasta = fechahasta,
                idCliente = idCliente,
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

        public ActionResult GenerateReporteComprasPDF(string TipoConsulta, string ModoTransaccion, string fechadesde, string fechahasta, int idCliente = 0, string numdoc = "")
        {
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            if (TipoConsulta == "Cliente")
            {
                if (idCliente == 0)
                {
                    reporte = repBeneficiario.ReporteComprasxCliente("todos", fechadesde, fechahasta, ModoTransaccion);
                }
                else
                {
                    reporte = repBeneficiario.ReporteComprasxCliente("uno", fechadesde, fechahasta, ModoTransaccion, idCliente);
                }
            }
            else if (TipoConsulta == "Beneficiario")
            {
                if (numdoc == "")
                {
                    reporte = repBeneficiario.ReporteComprasxBeneficiario("todos", fechadesde, fechahasta, ModoTransaccion);
                }
                else
                {
                    reporte = repBeneficiario.ReporteComprasxBeneficiario("uno", fechadesde, fechahasta, ModoTransaccion, numdoc);
                }
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";
            return new Rotativa.ViewAsPdf("ReporteComprasPDF", reporte)
            {
                FileName = "Reporte de Compras.pdf",
                CustomSwitches = footer
            };
        }

        [HttpPost]
        public ActionResult ReporteRecargas(string TipoConsulta, string fechadesde, string fechahasta, int idCliente = 0, string numdoc = "", string Referencia = "")
        {
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            if (TipoConsulta == "Cliente")
            {
                if (idCliente == 0)
                {
                    reporte = repBeneficiario.ReporteRecargasxCliente("todos", fechadesde, fechahasta, 0, Referencia);
                }
                else
                {
                    reporte = repBeneficiario.ReporteRecargasxCliente("uno", fechadesde, fechahasta, idCliente, Referencia);
                }
            }
            else if (TipoConsulta == "Beneficiario")
            {
                if (numdoc == "")
                {
                    reporte = repBeneficiario.ReporteRecargasxBeneficiario("todos", fechadesde, fechahasta, "", Referencia);
                }
                else
                {
                    reporte = repBeneficiario.ReporteRecargasxBeneficiario("uno", fechadesde, fechahasta, numdoc, Referencia);
                }
            }
            ParametrosReporte p = new ParametrosReporte()
            {
                TipoConsulta = TipoConsulta,
                fechadesde = fechadesde,
                fechahasta = fechahasta,
                idCliente = idCliente,
                numdoc = numdoc,
                referencia = Referencia
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

        public ActionResult GenerateReporteRecargasPDF(string TipoConsulta, string fechadesde, string fechahasta, int idCliente = 0, string numdoc = "", string Referencia = "")
        {
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            if (TipoConsulta == "Cliente")
            {
                if (idCliente == 0)
                {
                    reporte = repBeneficiario.ReporteRecargasxCliente("todos", fechadesde, fechahasta, 0, Referencia);
                }
                else
                {
                    reporte = repBeneficiario.ReporteRecargasxCliente("uno", fechadesde, fechahasta, idCliente, Referencia);
                }
            }
            else if (TipoConsulta == "Beneficiario")
            {
                if (numdoc == "")
                {
                    reporte = repBeneficiario.ReporteRecargasxBeneficiario("todos", fechadesde, fechahasta, "", Referencia);
                }
                else
                {
                    reporte = repBeneficiario.ReporteRecargasxBeneficiario("uno", fechadesde, fechahasta, numdoc, Referencia);
                }
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";
            return new Rotativa.ViewAsPdf("ReporteRecargasPDF", reporte)
            {
                FileName = "Reporte de Recargas.pdf",
                CustomSwitches = footer
            };
        }

        [HttpPost]
        public ActionResult ReporteTarjetas(string TipoConsulta, string fechadesde, string fechahasta, int idCliente = 0, string numdoc = "", string estadoTarjeta = "")
        {
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            if (TipoConsulta == "Cliente")
            {
                if (idCliente == 0)
                {
                    reporte = repBeneficiario.ReporteTarjetasxCliente("todos", fechadesde, fechahasta, 0, estadoTarjeta);
                }
                else
                {
                    reporte = repBeneficiario.ReporteTarjetasxCliente("uno", fechadesde, fechahasta, idCliente, estadoTarjeta);
                }
            }
            else if (TipoConsulta == "Beneficiario")
            {
                if (numdoc == "")
                {
                    reporte = repBeneficiario.ReporteTarjetasxBeneficiario("todos", fechadesde, fechahasta, "", estadoTarjeta);
                }
                else
                {
                    reporte = repBeneficiario.ReporteTarjetasxBeneficiario("uno", fechadesde, fechahasta, numdoc, estadoTarjeta);
                }
            }
            ParametrosReporte p = new ParametrosReporte()
            {
                TipoConsulta = TipoConsulta,
                fechadesde = fechadesde,
                fechahasta = fechahasta,
                idCliente = idCliente,
                numdoc = numdoc,
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

        public ActionResult GenerateReporteTarjetasPDF(string TipoConsulta, string fechadesde, string fechahasta, int idCliente = 0, string numdoc = "", string estadoTarjeta = "")
        {
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            if (TipoConsulta == "Cliente")
            {
                if (idCliente == 0)
                {
                    reporte = repBeneficiario.ReporteTarjetasxCliente("todos", fechadesde, fechahasta, 0, estadoTarjeta);
                }
                else
                {
                    reporte = repBeneficiario.ReporteTarjetasxCliente("uno", fechadesde, fechahasta, idCliente, estadoTarjeta);
                }
            }
            else if (TipoConsulta == "Beneficiario")
            {
                if (numdoc == "")
                {
                    reporte = repBeneficiario.ReporteTarjetasxBeneficiario("todos", fechadesde, fechahasta, "", estadoTarjeta);
                }
                else
                {
                    reporte = repBeneficiario.ReporteTarjetasxBeneficiario("uno", fechadesde, fechahasta, numdoc, estadoTarjeta);
                }
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";
            return new Rotativa.ViewAsPdf("ReporteTarjetasPDF", reporte)
            {
                FileName = "Reporte de Tarjetas.pdf",
                CustomSwitches = footer
            };
        }

        [HttpPost]
        public ActionResult ReporteTransacciones(string TipoTransaccion, string fechadesde, string fechahasta, string numdoc = "")
        {
            List<ReporteSuma> reporte = new List<ReporteSuma>();
            if (TipoTransaccion == "Todas")
            {
                reporte = repAfiliado.ReporteTransacciones(fechadesde, fechahasta, TipoTransaccion, numdoc);
            }
            else if (TipoTransaccion == "Acreditacion")
            {
                reporte = repAfiliado.ReporteTransacciones(fechadesde, fechahasta, Globals.TRANSCODE_ACREDITACION_SUMA, numdoc);
            }
            else if (TipoTransaccion == "Canje")
            {
                reporte = repAfiliado.ReporteTransacciones(fechadesde, fechahasta, Globals.TRANSCODE_CANJE_SUMA, numdoc);
            }
            ParametrosReporteSuma p = new ParametrosReporteSuma()
            {
                tipotransaccion = TipoTransaccion,
                fechadesde = fechadesde,
                fechahasta = fechahasta,
                numdoc = numdoc
            };
            if (reporte.Count == 0)
            {
                ReporteSuma r = new ReporteSuma()
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

        public ActionResult GenerateReporteTransaccionesPDF(string TipoTransaccion, string fechadesde, string fechahasta, string numdoc = "")
        {
            List<ReporteSuma> reporte = new List<ReporteSuma>();
            if (TipoTransaccion == "Todas")
            {
                reporte = repAfiliado.ReporteTransacciones(fechadesde, fechahasta, TipoTransaccion, numdoc);
            }
            else if (TipoTransaccion == "Acreditacion")
            {
                reporte = repAfiliado.ReporteTransacciones(fechadesde, fechahasta, Globals.TRANSCODE_ACREDITACION_SUMA, numdoc);
            }
            else if (TipoTransaccion == "Canje")
            {
                reporte = repAfiliado.ReporteTransacciones(fechadesde, fechahasta, Globals.TRANSCODE_CANJE_SUMA, numdoc);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";
            return new Rotativa.ViewAsPdf("ReporteTransaccionesPDF", reporte)
            {
                FileName = "Reporte de Transacciones.pdf",
                CustomSwitches = footer
            };
        }

    }

}
