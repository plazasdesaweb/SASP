using Suma2Lealtad.Models;
using Suma2Lealtad.Modules;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Suma2Lealtad.Models
{
    public class ReportesRepository
    {
        private ClientePrepagoRepository repCliente = new ClientePrepagoRepository();
        private AfiliadoSumaRepository repAfiliado = new AfiliadoSumaRepository();

        public List<ReportePrepago> ReporteDeComprasConsolidado(string tiporeporte, string fechadesde, string fechahasta, string modotrans, int idCliente)
        {
            string fechasdesdemod = fechadesde.Substring(6, 4) + fechadesde.Substring(3, 2) + fechadesde.Substring(0, 2);
            string fechahastamod = fechahasta.Substring(6, 4) + fechahasta.Substring(3, 2) + fechahasta.Substring(0, 2);
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            EncabezadoReporte encabezado = new EncabezadoReporte();
            ReportePrepago linea = new ReportePrepago();
            List<PLZ_GETREPORT_Result> consulta = new List<PLZ_GETREPORT_Result>();
            decimal TotalCompras;
            #region Por Cliente específico
            if (tiporeporte == "uno")
            {
                List<BeneficiarioPrepagoIndex> beneficiarios = repCliente.FindBeneficiarios(idCliente, "", "", "", "", "").ToList();
                encabezado.nombreReporte = "Reporte de Compras Consolidado";
                encabezado.tipoconsultaReporte = "Por Cliente";
                encabezado.parametrotipoconsultaReporte = beneficiarios.First().Cliente.rifCliente + " " + beneficiarios.First().Cliente.nameCliente;
                encabezado.fechainicioReporte = fechadesde;
                encabezado.fechafinReporte = fechahasta;
                encabezado.modotransaccionReporte = modotrans;
                TotalCompras = 0;
                if (beneficiarios.Count != 0)
                {
                    using (CardsEntities db = new CardsEntities())
                    {
                        foreach (BeneficiarioPrepagoIndex b in beneficiarios)
                        {
                            db.Database.Connection.ConnectionString = AppModule.ConnectionString("Cards");
                            consulta = db.PLZ_GETREPORT(fechasdesdemod, fechahastamod, b.Afiliado.docnumber.Substring(2), "NULL", Globals.TRANSCODE_COMPRA_PREPAGO).ToList();
                            if (modotrans == "Offline")
                            {
                                consulta = consulta.Where(x => x.ISODESCRIPTION.Contains("offline")).ToList();
                            }
                            else if (modotrans == "En Linea")
                            {
                                consulta = consulta.Where(x => !x.ISODESCRIPTION.Contains("offline")).ToList();
                            }
                            foreach (PLZ_GETREPORT_Result fila in consulta)
                            {
                                TotalCompras = TotalCompras + fila.SALDO.Value;
                            }
                        }
                        //NO INCLUYO CLIENTES SIN COMPRAS
                        if (TotalCompras != 0)
                        {
                            linea = new ReportePrepago()
                            {
                                Beneficiario = new BeneficiarioPrepagoIndex()
                                {
                                    Cliente = beneficiarios.First().Cliente
                                },
                                monto = TotalCompras,
                                tipo = "145-Compra",
                                Encabezado = encabezado
                            };
                            reporte.Add(linea);
                        }
                    }
                }
            }
            #endregion
            #region Todos los Clientes
            else if (tiporeporte == "todos")
            {
                List<ClientePrepago> clientes = repCliente.Find("", "").OrderBy(x => x.nameCliente).ToList();
                encabezado.nombreReporte = "Reporte de Compras Consolidado";
                encabezado.tipoconsultaReporte = "Por Cliente";
                encabezado.parametrotipoconsultaReporte = "Todos";
                encabezado.fechainicioReporte = fechadesde;
                encabezado.fechafinReporte = fechahasta;
                encabezado.modotransaccionReporte = modotrans;
                foreach (ClientePrepago c in clientes)
                {
                    TotalCompras = 0;
                    List<BeneficiarioPrepagoIndex> beneficiarios = repCliente.FindBeneficiarios(c.idCliente, "", "", "", "", "").OrderBy(x => x.Afiliado.id).ToList();
                    if (beneficiarios.Count != 0)
                    {
                        using (CardsEntities db = new CardsEntities())
                        {
                            foreach (BeneficiarioPrepagoIndex b in beneficiarios)
                            {
                                db.Database.Connection.ConnectionString = AppModule.ConnectionString("Cards");
                                consulta = db.PLZ_GETREPORT(fechasdesdemod, fechahastamod, b.Afiliado.docnumber.Substring(2), "NULL", Globals.TRANSCODE_COMPRA_PREPAGO).ToList();
                                if (modotrans == "Offline")
                                {
                                    consulta = consulta.Where(x => x.ISODESCRIPTION.Contains("offline")).ToList();
                                }
                                else if (modotrans == "En Linea")
                                {
                                    consulta = consulta.Where(x => !x.ISODESCRIPTION.Contains("offline")).ToList();
                                }
                                foreach (PLZ_GETREPORT_Result fila in consulta)
                                {
                                    TotalCompras = TotalCompras + fila.SALDO.Value;
                                }
                            }
                            //NO INCLUYO CLIENTES SIN COMPRAS
                            if (TotalCompras != 0)
                            {
                                linea = new ReportePrepago()
                                {
                                    Beneficiario = new BeneficiarioPrepagoIndex()
                                    {
                                        Cliente = c
                                    },
                                    monto = TotalCompras,
                                    tipo = "145-Compra",
                                    Encabezado = encabezado
                                };
                                reporte.Add(linea);
                            }
                        }
                    }
                }
            }
            #endregion
            if (reporte.Count == 0)
            {
                linea = new ReportePrepago()
                {
                    Encabezado = encabezado
                };
                reporte.Add(linea);
            }
            return reporte;
        }

        public List<ReportePrepago> ReporteDeComprasDetallado(string tiporeporte, string fechadesde, string fechahasta, string modotrans, int idCliente)
        {
            string fechasdesdemod = fechadesde.Substring(6, 4) + fechadesde.Substring(3, 2) + fechadesde.Substring(0, 2);
            string fechahastamod = fechahasta.Substring(6, 4) + fechahasta.Substring(3, 2) + fechahasta.Substring(0, 2);
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            EncabezadoReporte encabezado = new EncabezadoReporte();
            ReportePrepago linea = new ReportePrepago();
            List<PLZ_GETREPORT_Result> consulta = new List<PLZ_GETREPORT_Result>();
            #region Por Cliente específico, Detallado por Beneficiario
            if (tiporeporte == "detallado")
            {
                List<BeneficiarioPrepagoIndex> beneficiarios = repCliente.FindBeneficiarios(idCliente, "", "", "", "", "").ToList();
                encabezado.nombreReporte = "Reporte de Compras Detallado";
                encabezado.tipodetalleReporte = "Detallado por Beneficiario";
                encabezado.tipoconsultaReporte = "Por Cliente";
                encabezado.parametrotipoconsultaReporte = beneficiarios.First().Cliente.rifCliente + " " + beneficiarios.First().Cliente.nameCliente;
                encabezado.fechainicioReporte = fechadesde;
                encabezado.fechafinReporte = fechahasta;
                encabezado.modotransaccionReporte = modotrans;
                if (beneficiarios.Count != 0)
                {
                    using (CardsEntities db = new CardsEntities())
                    {
                        foreach (BeneficiarioPrepagoIndex b in beneficiarios)
                        {
                            db.Database.Connection.ConnectionString = AppModule.ConnectionString("Cards");
                            consulta = db.PLZ_GETREPORT(fechasdesdemod, fechahastamod, b.Afiliado.docnumber.Substring(2), "NULL", Globals.TRANSCODE_COMPRA_PREPAGO).ToList();
                            if (modotrans == "Fuera de Línea")
                            {
                                consulta = consulta.Where(x => x.ISODESCRIPTION.Contains("offline")).ToList();
                            }
                            else if (modotrans == "En Línea")
                            {
                                consulta = consulta.Where(x => !x.ISODESCRIPTION.Contains("offline")).ToList();
                            }
                            foreach (PLZ_GETREPORT_Result fila in consulta)
                            {
                                linea = new ReportePrepago()
                                {
                                    Beneficiario = b,
                                    fecha = fila.BATCHTIME == null ? DateTime.ParseExact(fila.FECHA.Substring(6, 2) + "/" + fila.FECHA.Substring(4, 2) + "/" + fila.FECHA.Substring(0, 4), "dd/MM/yyyy", CultureInfo.InvariantCulture) : DateTime.ParseExact(fila.FECHA.Substring(6, 2) + "/" + fila.FECHA.Substring(4, 2) + "/" + fila.FECHA.Substring(0, 4) + " " + fila.BATCHTIME.Substring(0, 2) + ":" + fila.BATCHTIME.Substring(2, 2) + ":" + fila.BATCHTIME.Substring(4, 2), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                                    monto = fila.SALDO.Value,
                                    detalle = fila.ISODESCRIPTION,
                                    tipo = "145-Compra",
                                    numerotarjeta = Convert.ToDecimal(fila.PAN),
                                    batchid = fila.BATCHID.ToString(),
                                    Encabezado = encabezado
                                };
                                if (linea.detalle.Contains("offline"))
                                {
                                    //buscar info en FueraDeLinea
                                    using (LealtadEntities db2 = new LealtadEntities())
                                    {
                                        db2.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                                        string operacion = linea.batchid.ToString();
                                        FueraDeLinea f = db2.FueraDeLineas.FirstOrDefault(t => t.batchid.Equals(operacion));
                                        if (f != null)
                                        {
                                            string sucursal = repAfiliado.Find(b.Afiliado.id).StoreOptions.Where(c => c.id != null).FirstOrDefault(x => x.id.Equals(f.store_code)).sucursal;
                                            linea.detalle = linea.detalle + " (origen: " + sucursal + ", motivo: " + f.observaciones + ")";
                                        }
                                    }
                                }
                                reporte.Add(linea);
                            }
                        }
                    }
                }
                reporte = reporte.OrderBy(x => x.fecha).ToList();
            }
            #endregion
            #region Por Cliente específico, Consolidado por Beneficiario
            else if (tiporeporte == "consolidado")
            {
                List<BeneficiarioPrepagoIndex> beneficiarios = repCliente.FindBeneficiarios(idCliente, "", "", "", "", "").OrderBy(x => x.Afiliado.docnumber).ToList();
                encabezado.nombreReporte = "Reporte de Compras Detallado";
                encabezado.tipodetalleReporte = "Consolidado por Beneficiario";
                encabezado.tipoconsultaReporte = "Por Cliente";
                encabezado.parametrotipoconsultaReporte = beneficiarios.First().Cliente.rifCliente + " " + beneficiarios.First().Cliente.nameCliente;
                encabezado.fechainicioReporte = fechadesde;
                encabezado.fechafinReporte = fechahasta;
                encabezado.modotransaccionReporte = modotrans;
                decimal TotalCompras = 0;
                if (beneficiarios.Count != 0)
                {
                    using (CardsEntities db = new CardsEntities())
                    {
                        foreach (BeneficiarioPrepagoIndex b in beneficiarios)
                        {
                            TotalCompras = 0;
                            db.Database.Connection.ConnectionString = AppModule.ConnectionString("Cards");
                            consulta = db.PLZ_GETREPORT(fechasdesdemod, fechahastamod, b.Afiliado.docnumber.Substring(2), "NULL", Globals.TRANSCODE_COMPRA_PREPAGO).ToList();
                            if (modotrans == "Fuera de Línea")
                            {
                                consulta = consulta.Where(x => x.ISODESCRIPTION.Contains("offline")).ToList();
                            }
                            else if (modotrans == "En Línea")
                            {
                                consulta = consulta.Where(x => !x.ISODESCRIPTION.Contains("offline")).ToList();
                            }
                            foreach (PLZ_GETREPORT_Result fila in consulta)
                            {
                                TotalCompras = TotalCompras + fila.SALDO.Value;
                            }
                            // NO INCLUYO BENEFICIARIOS SIN COMPRAS
                            if (TotalCompras != 0)
                            {
                                linea = new ReportePrepago()
                                {
                                    Beneficiario = b,
                                    monto = TotalCompras,
                                    tipo = "145-Compra",
                                    Encabezado = encabezado
                                };
                                reporte.Add(linea);
                            }
                        }
                    }
                }
            }
            #endregion
            if (reporte.Count == 0)
            {
                linea = new ReportePrepago()
                {
                    Encabezado = encabezado
                };
                reporte.Add(linea);
            }
            return reporte;
        }

        public List<ReportePrepago> ReporteRecargasConsolidado(string tiporeporte, string fechadesde, string fechahasta, int idCliente, string referencia, string observaciones)
        {
            string fechasdesdemod = fechadesde.Substring(6, 4) + fechadesde.Substring(3, 2) + fechadesde.Substring(0, 2);
            string fechahastamod = fechahasta.Substring(6, 4) + fechahasta.Substring(3, 2) + fechahasta.Substring(0, 2);
            //para filtrar las fechas
            DateTime desde = DateTime.ParseExact(fechadesde, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime hasta = DateTime.ParseExact(fechahasta, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            EncabezadoReporte encabezado = new EncabezadoReporte();
            ReportePrepago linea = new ReportePrepago();
            decimal TotalRecargas = 0;
            decimal TotalAnulaciones = 0;
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                #region Por Cliente específico
                if (tiporeporte == "uno")
                {
                    ClientePrepago c = repCliente.Find(idCliente);
                    encabezado.nombreReporte = "Reporte de Recargas Consolidado";
                    encabezado.tipoconsultaReporte = "Por Cliente";
                    encabezado.parametrotipoconsultaReporte = c.rifCliente + " " + c.nameCliente;
                    encabezado.fechainicioReporte = fechadesde;
                    encabezado.fechafinReporte = fechahasta;
                    encabezado.documentoreferenciaReporte = referencia;
                    encabezado.observacionesReporte = observaciones;
                    //busco movimientos de recaga y anulación de recarga
                    var queryTotalRecargas = (from o in db.Orders
                                              join od in db.OrdersDetails on o.id equals od.orderid
                                              //join a in db.Affiliates on od.customerid equals a.id
                                              //join c in db.CLIENTES on a.docnumber equals c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO
                                              join s in db.SumaStatuses on o.sumastatusid equals s.id
                                              join p in db.PrepaidCustomers on o.prepaidcustomerid equals p.id
                                              where o.prepaidcustomerid == idCliente
                                                    && (o.comments == "Orden de Anulación de Recarga" || o.comments.Contains("Orden de Recarga"))
                                                    && s.name == "Procesada" && (od.comments == "Recarga efectiva" || od.comments.Contains("Anulación efectiva"))
                                                    && (o.processdate >= desde && o.processdate <= hasta)
                                              group od by new { o.prepaidcustomerid, o.comments, o.processdate, o.documento, o.observaciones } into g
                                              select new
                                              {
                                                  FechaProcesado = g.Key.processdate,
                                                  Cliente = g.Key.prepaidcustomerid,
                                                  ClaseOrden = g.Key.comments,
                                                  Total = g.Sum(x => x.amount),
                                                  Referencia = g.Key.documento,
                                                  Observaciones = g.Key.observaciones
                                              }).ToList();
                    //filtado por referencia u observaciones
                    if (referencia != "")
                    {
                        queryTotalRecargas = queryTotalRecargas.Where(x => x.Referencia == referencia).ToList();
                    }
                    else if (observaciones != "")
                    {
                        queryTotalRecargas = queryTotalRecargas.Where(x => x.Observaciones.Contains(observaciones)).ToList();
                    }
                    //armo lineas del reporte
                    foreach (var q in queryTotalRecargas)
                    {
                        if (q.ClaseOrden == "Orden de Anulación de Recarga")
                        {
                            TotalAnulaciones = TotalAnulaciones + q.Total;
                        }
                        else
                        {
                            TotalRecargas = TotalRecargas + q.Total;
                        }
                    }
                    //NO INCLUYO CLIENTES SIN RECARGAS
                    if (TotalRecargas != 0)
                    {
                        linea = new ReportePrepago()
                        {
                            Beneficiario = new BeneficiarioPrepagoIndex()
                            {
                                Cliente = repCliente.Find(idCliente)
                            },
                            monto = TotalRecargas,
                            tipo = "200-Recarga",
                            Encabezado = encabezado
                        };
                        reporte.Add(linea);
                    }
                    //NO INCLUYO CLIENETS SIN ANULACIONES
                    if (TotalAnulaciones != 0)
                    {
                        linea = new ReportePrepago()
                        {
                            Beneficiario = new BeneficiarioPrepagoIndex()
                            {
                                Cliente = repCliente.Find(idCliente)
                            },
                            monto = TotalAnulaciones * -1,
                            tipo = "161-Anulación",
                            Encabezado = encabezado
                        };
                        reporte.Add(linea);
                    }
                }
                #endregion
                #region Todos los Clientes
                else if (tiporeporte == "todos")
                {
                    List<ClientePrepago> clientes = repCliente.Find("", "").OrderBy(x => x.nameCliente).ToList();
                    encabezado.nombreReporte = "Reporte de Recargas Consolidado";
                    encabezado.tipoconsultaReporte = "Por Cliente";
                    encabezado.parametrotipoconsultaReporte = "Todos";
                    encabezado.fechainicioReporte = fechadesde;
                    encabezado.fechafinReporte = fechahasta;
                    encabezado.documentoreferenciaReporte = referencia;
                    encabezado.observacionesReporte = observaciones;
                    foreach (ClientePrepago c in clientes)
                    {
                        TotalRecargas = 0;
                        TotalAnulaciones = 0;
                        //busco movimientos de recaga y anulación de recarga
                        var queryTotalRecargas = (from o in db.Orders
                                                  join od in db.OrdersDetails on o.id equals od.orderid
                                                  //join a in db.Affiliates on od.customerid equals a.id
                                                  //join c in db.CLIENTES on a.docnumber equals c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO
                                                  join s in db.SumaStatuses on o.sumastatusid equals s.id
                                                  join p in db.PrepaidCustomers on o.prepaidcustomerid equals p.id
                                                  where o.prepaidcustomerid == c.idCliente
                                                        && (o.comments == "Orden de Anulación de Recarga" || o.comments.Contains("Orden de Recarga"))
                                                        && s.name == "Procesada" && (od.comments == "Recarga efectiva" || od.comments.Contains("Anulación efectiva"))
                                                        && (o.processdate >= desde && o.processdate <= hasta)
                                                  group od by new { o.prepaidcustomerid, o.comments, o.processdate, o.documento, o.observaciones } into g
                                                  select new
                                                  {
                                                      FechaProcesado = g.Key.processdate,
                                                      Cliente = g.Key.prepaidcustomerid,
                                                      ClaseOrden = g.Key.comments,
                                                      Total = g.Sum(x => x.amount),
                                                      Referencia = g.Key.documento,
                                                      Observaciones = g.Key.observaciones
                                                  }).ToList();
                        //filtado por referencia u observaciones
                        if (referencia != "")
                        {
                            queryTotalRecargas = queryTotalRecargas.Where(x => x.Referencia == referencia).ToList();
                        }
                        else if (observaciones != "")
                        {
                            queryTotalRecargas = queryTotalRecargas.Where(x => x.Observaciones.Contains(observaciones)).ToList();
                        }
                        //armo lineas del reporte
                        foreach (var q in queryTotalRecargas)
                        {
                            if (q.ClaseOrden == "Orden de Anulación de Recarga")
                            {
                                TotalAnulaciones = TotalAnulaciones + q.Total;
                            }
                            else
                            {
                                TotalRecargas = TotalRecargas + q.Total;
                            }
                        }
                        //NO INCLUYO CLIENTES SIN RECARGAS
                        if (TotalRecargas != 0)
                        {
                            linea = new ReportePrepago()
                            {
                                Beneficiario = new BeneficiarioPrepagoIndex()
                                {
                                    Cliente = c
                                },
                                monto = TotalRecargas,
                                tipo = "200-Recarga",
                                Encabezado = encabezado
                            };
                            reporte.Add(linea);
                        }
                        //NO INCLUYO CLIENTES SIN ANULACIONES
                        if (TotalAnulaciones != 0)
                        {
                            linea = new ReportePrepago()
                            {
                                Beneficiario = new BeneficiarioPrepagoIndex()
                                {
                                    Cliente = c
                                },
                                monto = TotalAnulaciones * -1,
                                tipo = "161-Anulación",
                                Encabezado = encabezado
                            };
                            reporte.Add(linea);
                        }
                    }
                }
                #endregion
            }
            if (reporte.Count == 0)
            {
                linea = new ReportePrepago()
                {
                    Encabezado = encabezado
                };
                reporte.Add(linea);
            }
            return reporte;
        }

        public List<ReportePrepago> ReporteRecargasDetallado(string tiporeporte, string fechadesde, string fechahasta, int idCliente, string referencia, string observaciones)
        {
            string fechasdesdemod = fechadesde.Substring(6, 4) + fechadesde.Substring(3, 2) + fechadesde.Substring(0, 2);
            string fechahastamod = fechahasta.Substring(6, 4) + fechahasta.Substring(3, 2) + fechahasta.Substring(0, 2);
            //para filtrar las fechas
            DateTime desde = DateTime.ParseExact(fechadesde, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime hasta = DateTime.ParseExact(fechahasta, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            EncabezadoReporte encabezado = new EncabezadoReporte();
            ReportePrepago linea = new ReportePrepago();
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                #region Por Cliente específico, Detallado por Beneficiario
                if (tiporeporte == "detallado")
                {
                    ClientePrepago cliente = repCliente.Find(idCliente);
                    encabezado.nombreReporte = "Reporte de Recargas Detallado";
                    encabezado.tipodetalleReporte = "Detallado por Beneficiario";
                    encabezado.tipoconsultaReporte = "Por Cliente";
                    encabezado.parametrotipoconsultaReporte = cliente.rifCliente + " " + cliente.nameCliente; ;
                    encabezado.fechainicioReporte = fechadesde;
                    encabezado.fechafinReporte = fechahasta;
                    encabezado.documentoreferenciaReporte = referencia;
                    encabezado.observacionesReporte = observaciones;
                    var queryreporte = (from o in db.Orders
                                        join od in db.OrdersDetails on o.id equals od.orderid
                                        join a in db.Affiliates on od.customerid equals a.id
                                        join c in db.CLIENTES on a.docnumber equals c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO
                                        join s in db.SumaStatuses on o.sumastatusid equals s.id
                                        join p in db.PrepaidCustomers on o.prepaidcustomerid equals p.id
                                        where o.prepaidcustomerid == idCliente
                                              && (o.comments == "Orden de Anulación de Recarga" || o.comments.Contains("Orden de Recarga"))
                                              && s.name == "Procesada" && (od.comments == "Recarga efectiva" || od.comments.Contains("Anulación efectiva"))
                                              && (o.processdate >= desde && o.processdate <= hasta)
                                        select new ReportePrepago()
                                        {
                                            Beneficiario = new BeneficiarioPrepagoIndex()
                                            {
                                                Afiliado = new AfiliadoSumaIndex()
                                                {
                                                    docnumber = a.docnumber,
                                                    name = c.NOMBRE_CLIENTE1,
                                                    lastname1 = c.APELLIDO_CLIENTE1
                                                }
                                            },
                                            fecha = o.processdate,
                                            monto = od.amount,
                                            detalle = od.comments,
                                            //usare el tipo para guardar el tipo de Orden y determinar el tipo de movimiento
                                            tipo = o.comments,
                                            batchid = od.cardsresponse,
                                            nroordenrecarga = o.id,
                                            referenciarecarga = o.documento,
                                            observacionesrecarga = o.observaciones//,
                                        }).ToList();
                    //filtado por referencia u observaciones
                    if (referencia != "")
                    {
                        queryreporte = queryreporte.Where(x => x.referenciarecarga == referencia).ToList();
                    }
                    else if (observaciones != "")
                    {
                        queryreporte = queryreporte.Where(x => x.observacionesrecarga.Contains(observaciones)).ToList();
                    }
                    //armo lineas del reporte
                    foreach (var q in queryreporte)
                    {
                        if (q.tipo == "Orden de Anulación de Recarga")
                        {
                            linea = new ReportePrepago()
                            {
                                Beneficiario = new BeneficiarioPrepagoIndex()
                                {
                                    Afiliado = q.Beneficiario.Afiliado,
                                    Cliente = repCliente.Find(idCliente)
                                },
                                fecha = q.fecha,
                                monto = q.monto * -1,
                                detalle = q.detalle,
                                tipo = "161-Anulación",
                                numerotarjeta = q.numerotarjeta,
                                batchid = q.batchid,
                                nroordenrecarga = q.nroordenrecarga,
                                referenciarecarga = q.referenciarecarga,
                                observacionesrecarga = q.observacionesrecarga,
                                Encabezado = encabezado
                            };
                            reporte.Add(linea);
                        }
                        else
                        {
                            linea = new ReportePrepago()
                            {
                                Beneficiario = new BeneficiarioPrepagoIndex()
                                {
                                    Afiliado = q.Beneficiario.Afiliado,
                                    Cliente = repCliente.Find(idCliente)
                                },
                                fecha = q.fecha,
                                monto = q.monto,
                                detalle = q.detalle,
                                tipo = "200-Recarga",
                                numerotarjeta = q.numerotarjeta,
                                batchid = q.batchid,
                                nroordenrecarga = q.nroordenrecarga,
                                referenciarecarga = q.referenciarecarga,
                                observacionesrecarga = q.observacionesrecarga,
                                Encabezado = encabezado
                            };
                            reporte.Add(linea);
                        }
                    }
                    reporte = reporte.OrderBy(x => x.fecha).ToList();
                }
                #endregion
                #region Por Cliente específico, Consolidado por Beneficiario
                else if (tiporeporte == "consolidado")
                {
                    ClientePrepago cliente = repCliente.Find(idCliente);
                    encabezado.nombreReporte = "Reporte de Recargas Detallado";
                    encabezado.tipodetalleReporte = "Consolidado por Beneficiario";
                    encabezado.tipoconsultaReporte = "Por Cliente";
                    encabezado.parametrotipoconsultaReporte = cliente.rifCliente + " " + cliente.nameCliente; ;
                    encabezado.fechainicioReporte = fechadesde;
                    encabezado.fechafinReporte = fechahasta;
                    encabezado.documentoreferenciaReporte = referencia;
                    encabezado.observacionesReporte = observaciones;
                    var queryreporte = (from o in db.Orders
                                        join od in db.OrdersDetails on o.id equals od.orderid
                                        join a in db.Affiliates on od.customerid equals a.id
                                        join c in db.CLIENTES on a.docnumber equals c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO
                                        join s in db.SumaStatuses on o.sumastatusid equals s.id
                                        join p in db.PrepaidCustomers on o.prepaidcustomerid equals p.id
                                        where o.prepaidcustomerid == idCliente
                                              && (o.comments == "Orden de Anulación de Recarga" || o.comments.Contains("Orden de Recarga"))
                                              && s.name == "Procesada" && (od.comments == "Recarga efectiva" || od.comments.Contains("Anulación efectiva"))
                                              && (o.processdate >= desde && o.processdate <= hasta)
                                        group od by new { o.prepaidcustomerid, a.docnumber, c.NOMBRE_CLIENTE1, c.APELLIDO_CLIENTE1, o.comments, o.documento, o.observaciones } into g
                                        select new
                                        {
                                            Afiliado = new AfiliadoSumaIndex()
                                            {
                                                docnumber = g.Key.docnumber,
                                                name = g.Key.NOMBRE_CLIENTE1,
                                                lastname1 = g.Key.APELLIDO_CLIENTE1
                                            },
                                            Cliente = g.Key.prepaidcustomerid,
                                            ClaseOrden = g.Key.comments,
                                            Total = g.Sum(x => x.amount),
                                            Referencia = g.Key.documento,
                                            Observaciones = g.Key.observaciones
                                        }).OrderBy(x => x.Afiliado.docnumber).ToList();
                    if (referencia != "")
                    {
                        queryreporte = queryreporte.Where(x => x.Referencia == referencia).ToList();
                    }
                    else if (observaciones != "")
                    {
                        queryreporte = queryreporte.Where(x => x.Observaciones.Contains(observaciones)).ToList();
                    }
                    //armo lineas del reporte
                    foreach (var doc in queryreporte.Select(d => d.Afiliado.docnumber).Distinct())
                    {
                        decimal TotalRecargas = 0;
                        decimal TotalAnulaciones = 0;
                        foreach (var q in queryreporte.Where(s => s.Afiliado.docnumber == doc))
                        {
                            if (q.ClaseOrden == "Orden de Anulación de Recarga")
                            {
                                TotalAnulaciones = TotalAnulaciones + q.Total;
                            }
                            else
                            {
                                TotalRecargas = TotalRecargas + q.Total;
                            }
                        }
                        //NO INCLUYO BENEFICIARIOS SIN RECARGAS
                        if (TotalRecargas != 0)
                        {
                            linea = new ReportePrepago()
                            {
                                Beneficiario = new BeneficiarioPrepagoIndex()
                                {
                                    Afiliado = repAfiliado.Find(doc, "", "", "", "").First(),
                                    Cliente = repCliente.Find(idCliente)
                                },
                                monto = TotalRecargas,
                                tipo = "200-Recarga",
                                Encabezado = encabezado
                            };
                            reporte.Add(linea);
                        }
                        //NO INCLUYO BENEFICIARIOS SIN ANULACIONES
                        if (TotalAnulaciones != 0)
                        {
                            linea = new ReportePrepago()
                            {
                                Beneficiario = new BeneficiarioPrepagoIndex()
                                {
                                    Afiliado = repAfiliado.Find(doc, "", "", "", "").First(),
                                    Cliente = repCliente.Find(idCliente)
                                },
                                monto = TotalAnulaciones * -1,
                                tipo = "161-Anulación",
                                Encabezado = encabezado
                            };
                            reporte.Add(linea);
                        }
                    }
                }
                #endregion
            }
            if (reporte.Count == 0)
            {
                linea = new ReportePrepago()
                {
                    Encabezado = encabezado
                };
                reporte.Add(linea);
            }
            return reporte;
        }

        public List<ReportePrepago> ReporteTransaccionesSuma(string tipotransaccion, string fechadesde, string fechahasta, string docnumber)
        {
            AfiliadoSumaIndex afiliado = repAfiliado.Find(docnumber, "", "", "", "").First();
            string fechasdesdemod = fechadesde.Substring(6, 4) + fechadesde.Substring(3, 2) + fechadesde.Substring(0, 2);
            string fechahastamod = fechahasta.Substring(6, 4) + fechahasta.Substring(3, 2) + fechahasta.Substring(0, 2);
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            EncabezadoReporte encabezado = new EncabezadoReporte();
            ReportePrepago linea = new ReportePrepago();
            List<PLZ_GETREPORT_Result> consulta = new List<PLZ_GETREPORT_Result>();
            encabezado.nombreReporte = "Reporte de Transacciones Suma";
            encabezado.parametrotipoconsultaReporte = docnumber + " " + afiliado.name + " " + afiliado.lastname1;
            encabezado.fechainicioReporte = fechadesde;
            encabezado.fechafinReporte = fechahasta;
            encabezado.tipotransaccionReporte = tipotransaccion;
            using (CardsEntities db = new CardsEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("Cards");
                if (tipotransaccion == "Todas")
                {
                    consulta = db.PLZ_GETREPORT(fechasdesdemod, fechahastamod, docnumber.Substring(2), Globals.TIPO_CUENTA_SUMA, "NULL").ToList();
                }
                foreach (PLZ_GETREPORT_Result fila in consulta.Where(x => x.TRANSCODE != 121))
                {
                    linea = new ReportePrepago()
                    {
                        Beneficiario = new BeneficiarioPrepagoIndex()
                        {
                            Afiliado = afiliado
                        },
                        fecha = fila.BATCHTIME == null ? DateTime.ParseExact(fila.FECHA.Substring(6, 2) + "/" + fila.FECHA.Substring(4, 2) + "/" + fila.FECHA.Substring(0, 4), "dd/MM/yyyy", CultureInfo.InvariantCulture) : DateTime.ParseExact(fila.FECHA.Substring(6, 2) + "/" + fila.FECHA.Substring(4, 2) + "/" + fila.FECHA.Substring(0, 4) + " " + fila.BATCHTIME.Substring(0, 2) + ":" + fila.BATCHTIME.Substring(2, 2) + ":" + fila.BATCHTIME.Substring(4, 2), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                        monto = fila.SALDO.Value,
                        detalle = fila.ISODESCRIPTION,
                        tipo = fila.TRANSCODE + "-" + fila.TRANSNAME,
                        numerotarjeta = Convert.ToDecimal(fila.PAN),
                        batchid = fila.BATCHID.ToString(),
                        Encabezado = encabezado
                    };
                    if (fila.TRANSCODE.Value.ToString() == Globals.TRANSCODE_ANULACION_TRANSFERENCIA_CREDITO_SUMA || fila.TRANSCODE.Value.ToString() == Globals.TRANSCODE_ANULACION_TRANSFERENCIA_DEBITO_SUMA)
                    {
                        linea.detalle  = linea.detalle  + " (" + fila.B037 + ")";
                    }
                    if (fila.TRANSCODE.Value.ToString() == Globals.TRANSCODE_CANJE_SUMA || fila.TRANSCODE.Value.ToString() == Globals.TRANSCODE_TRANSFERENCIA_DEBITO_SUMA || fila.TRANSCODE.Value.ToString() == Globals.TRANSCODE_ANULACION_TRANSFERENCIA_CREDITO_SUMA)
                    {
                        linea.monto = linea.monto * Convert.ToInt32(Globals.FACTOR_CANJE);
                    }
                    reporte.Add(linea);
                }
            }
            reporte = reporte.OrderBy(x => x.fecha).ToList();
            if (reporte.Count == 0)
            {
                linea = new ReportePrepago()
                {
                    Encabezado = encabezado
                };
                reporte.Add(linea);
            }
            return reporte;
        }

        public List<ReportePrepago> ReporteTransaccionesPrepago(string tipotransaccion, string fechadesde, string fechahasta, string docnumber)
        {
            AfiliadoSumaIndex afiliado = repAfiliado.Find(docnumber, "", "", "", "").First();
            string fechasdesdemod = fechadesde.Substring(6, 4) + fechadesde.Substring(3, 2) + fechadesde.Substring(0, 2);
            string fechahastamod = fechahasta.Substring(6, 4) + fechahasta.Substring(3, 2) + fechahasta.Substring(0, 2);
            List<ReportePrepago> reporte = new List<ReportePrepago>();
            EncabezadoReporte encabezado = new EncabezadoReporte();
            ReportePrepago linea = new ReportePrepago();
            List<PLZ_GETREPORT_Result> consulta = new List<PLZ_GETREPORT_Result>();
            encabezado.nombreReporte = "Reporte de Transacciones Prepago";
            encabezado.parametrotipoconsultaReporte = docnumber + " " + afiliado.name + " " + afiliado.lastname1;
            encabezado.fechainicioReporte = fechadesde;
            encabezado.fechafinReporte = fechahasta;
            encabezado.tipotransaccionReporte = tipotransaccion;
            using (CardsEntities db = new CardsEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("Cards");
                if (tipotransaccion == "Todas")
                {
                    consulta = db.PLZ_GETREPORT(fechasdesdemod, fechahastamod, docnumber.Substring(2), Globals.TIPO_CUENTA_PREPAGO, "NULL").ToList();
                }
                foreach (PLZ_GETREPORT_Result fila in consulta.Where(x=>x.TRANSCODE != 121))
                {
                    linea = new ReportePrepago()
                    {
                        Beneficiario = new BeneficiarioPrepagoIndex()
                        {
                            Afiliado = afiliado
                        },
                        fecha = fila.BATCHTIME == null ? DateTime.ParseExact(fila.FECHA.Substring(6, 2) + "/" + fila.FECHA.Substring(4, 2) + "/" + fila.FECHA.Substring(0, 4), "dd/MM/yyyy", CultureInfo.InvariantCulture) : DateTime.ParseExact(fila.FECHA.Substring(6, 2) + "/" + fila.FECHA.Substring(4, 2) + "/" + fila.FECHA.Substring(0, 4) + " " + fila.BATCHTIME.Substring(0, 2) + ":" + fila.BATCHTIME.Substring(2, 2) + ":" + fila.BATCHTIME.Substring(4, 2), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                        monto = fila.SALDO.Value,
                        detalle = fila.ISODESCRIPTION,
                        tipo = fila.TRANSCODE + "-" + fila.TRANSNAME,
                        numerotarjeta = Convert.ToDecimal(fila.PAN),
                        batchid = fila.BATCHID.ToString(),
                        Encabezado = encabezado
                    };
                    if (fila.TRANSCODE.Value.ToString() == Globals.TRANSCODE_ANULACION_TRANSFERENCIA_CREDITO_SUMA || fila.TRANSCODE.Value.ToString() == Globals.TRANSCODE_ANULACION_TRANSFERENCIA_DEBITO_SUMA)
                    {
                        linea.detalle = linea.detalle + " (" + fila.B037 + ")";
                    }
                    if (fila.TRANSCODE.Value.ToString() == Globals.TRANSCODE_CANJE_SUMA || fila.TRANSCODE.Value.ToString() == Globals.TRANSCODE_TRANSFERENCIA_DEBITO_SUMA || fila.TRANSCODE.Value.ToString() == Globals.TRANSCODE_ANULACION_TRANSFERENCIA_CREDITO_SUMA)
                    {
                        linea.monto = linea.monto * Convert.ToInt32(Globals.FACTOR_CANJE);
                    }
                    if (linea.detalle.Contains("offline"))
                    {
                        //buscar info en FueraDeLinea
                        using (LealtadEntities db2 = new LealtadEntities())
                        {
                            db2.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                            string operacion = linea.batchid.ToString();
                            FueraDeLinea f = db2.FueraDeLineas.FirstOrDefault(t => t.batchid.Equals(operacion));
                            if (f != null)
                            {
                                string sucursal = repAfiliado.Find(afiliado.id).StoreOptions.Where(c => c.id != null).FirstOrDefault(x => x.id.Equals(f.store_code)).sucursal;
                                linea.detalle = linea.detalle + " (origen: " + sucursal + ", motivo: " + f.observaciones + ")";
                            }
                        }
                    }
                    reporte.Add(linea);
                }
            }
            reporte = reporte.OrderBy(x => x.fecha).ToList();
            if (reporte.Count == 0)
            {
                linea = new ReportePrepago()
                {
                    Encabezado = encabezado
                };
                reporte.Add(linea);
            }
            return reporte;
        }

        public List<AfiliadoSumaExcel> CargarDatosExportarDatosAfiliadosExcel()
        {
            List<AfiliadoSumaExcel> afiliados = repAfiliado.FindAllExcel();
            return afiliados;
        }

    }
}