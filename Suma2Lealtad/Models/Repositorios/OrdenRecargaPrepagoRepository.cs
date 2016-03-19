using LinqToExcel;
using Newtonsoft.Json;
using Suma2Lealtad.Modules;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Suma2Lealtad.Models
{
    public class OrdenRecargaRepository
    {
        private int OrderId()
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString();
                if (db.Orders.Count() == 0)
                    return 1;
                return (db.Orders.Max(c => c.id) + 1);
            }
        }

        private int OrdersDetailId()
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString();
                if (db.OrdersDetails.Count() == 0)
                    return 1;
                return (db.OrdersDetails.Max(c => c.id));
            }
        }

        private int OrdersHistoryId()
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString();
                if (db.OrdersHistories.Count() == 0)
                    return 1;
                return (db.OrdersHistories.Max(c => c.id) + 1);
            }
        }

        public List<OrdenRecargaPrepago> Find(string fecha, string estadoOrden, string Referencia, string claseOrden, string Observaciones)
        {
            List<OrdenRecargaPrepago> ordenes;
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString();
                if (fecha == "")
                {
                    fecha = null;
                }
                if (estadoOrden == "")
                {
                    estadoOrden = null;
                }
                if (Referencia == "")
                {
                    Referencia = null;
                }
                if (claseOrden == "")
                {
                    claseOrden = null;
                }
                if (Observaciones == "")
                {
                    Observaciones = null;
                }
                //BUSCAR POR estadoOrden 
                if (estadoOrden != null)
                {
                    if (claseOrden != null)
                    {
                        ordenes = (from o in db.Orders
                                   where o.comments.Equals(claseOrden)
                                   join c in db.PrepaidCustomers on o.prepaidcustomerid equals c.id
                                   join s in db.SumaStatuses on o.sumastatusid equals s.id
                                   where s.name == estadoOrden
                                   select new OrdenRecargaPrepago()
                                   {
                                       id = o.id,
                                       statusOrden = s.name,
                                       montoOrden = o.totalamount,
                                       creationdateOrden = o.creationdate,
                                       tipoOrden = o.comments,
                                       documento = o.documento, 
                                       observaciones = o.observaciones,
                                       Cliente = new ClientePrepago()
                                       {
                                           idCliente = c.id,
                                           nameCliente = c.name,
                                           aliasCliente = c.alias,
                                           rifCliente = c.rif,
                                           addressCliente = c.address,
                                           phoneCliente = c.phone,
                                           emailCliente = c.email
                                       }
                                   }).OrderBy(x => x.id).ToList();
                    }
                    else
                    {
                        ordenes = (from o in db.Orders
                                   join c in db.PrepaidCustomers on o.prepaidcustomerid equals c.id
                                   join s in db.SumaStatuses on o.sumastatusid equals s.id
                                   where s.name == estadoOrden
                                   select new OrdenRecargaPrepago()
                                   {
                                       id = o.id,
                                       statusOrden = s.name,
                                       montoOrden = o.totalamount,
                                       creationdateOrden = o.creationdate,
                                       tipoOrden = o.comments,
                                       documento = o.documento,
                                       observaciones = o.observaciones,
                                       Cliente = new ClientePrepago()
                                       {
                                           idCliente = c.id,
                                           nameCliente = c.name,
                                           aliasCliente = c.alias,
                                           rifCliente = c.rif,
                                           addressCliente = c.address,
                                           phoneCliente = c.phone,
                                           emailCliente = c.email
                                       }
                                   }).OrderBy(x => x.id).ToList();
                    }
                }
                //BUSCAR POR fecha
                else if (fecha != null)
                {
                    DateTime f = DateTime.ParseExact(fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    if (claseOrden != null)
                    {
                        ordenes = (from o in db.Orders
                                   where o.comments.Equals(claseOrden)
                                   join c in db.PrepaidCustomers on o.prepaidcustomerid equals c.id
                                   join s in db.SumaStatuses on o.sumastatusid equals s.id
                                   select new OrdenRecargaPrepago()
                                   {
                                       id = o.id,
                                       statusOrden = s.name,
                                       montoOrden = o.totalamount,
                                       creationdateOrden = o.creationdate,
                                       tipoOrden = o.comments,
                                       documento = o.documento,
                                       observaciones = o.observaciones,
                                       Cliente = new ClientePrepago()
                                       {
                                           idCliente = c.id,
                                           nameCliente = c.name,
                                           aliasCliente = c.alias,
                                           rifCliente = c.rif,
                                           addressCliente = c.address,
                                           phoneCliente = c.phone,
                                           emailCliente = c.email
                                       }
                                   }).ToList()
                                   .Where(q => q.creationdateOrden.Date == f.Date)
                                   .OrderBy(x => x.id)
                                   .ToList();
                    }
                    else
                    {
                        ordenes = (from o in db.Orders
                                   join c in db.PrepaidCustomers on o.prepaidcustomerid equals c.id
                                   join s in db.SumaStatuses on o.sumastatusid equals s.id
                                   select new OrdenRecargaPrepago()
                                   {
                                       id = o.id,
                                       statusOrden = s.name,
                                       montoOrden = o.totalamount,
                                       creationdateOrden = o.creationdate,
                                       tipoOrden = o.comments,
                                       documento = o.documento,
                                       observaciones = o.observaciones,
                                       Cliente = new ClientePrepago()
                                       {
                                           idCliente = c.id,
                                           nameCliente = c.name,
                                           aliasCliente = c.alias,
                                           rifCliente = c.rif,
                                           addressCliente = c.address,
                                           phoneCliente = c.phone,
                                           emailCliente = c.email
                                       }
                                   }).ToList()
                                   .Where(q => q.creationdateOrden.Date == f.Date)
                                   .OrderBy(x => x.id)
                                   .ToList();
                    }
                }
                //BUSCAR POR Referencia
                else if (Referencia != null)
                {
                    if (claseOrden != null)
                    {
                        ordenes = (from o in db.Orders
                                   where o.comments.Equals(claseOrden)
                                   join c in db.PrepaidCustomers on o.prepaidcustomerid equals c.id
                                   join s in db.SumaStatuses on o.sumastatusid equals s.id
                                   where o.documento == Referencia
                                   select new OrdenRecargaPrepago()
                                   {
                                       id = o.id,
                                       statusOrden = s.name,
                                       montoOrden = o.totalamount,
                                       creationdateOrden = o.creationdate,
                                       tipoOrden = o.comments,
                                       documento = o.documento,
                                       observaciones = o.observaciones,
                                       Cliente = new ClientePrepago()
                                       {
                                           idCliente = c.id,
                                           nameCliente = c.name,
                                           aliasCliente = c.alias,
                                           rifCliente = c.rif,
                                           addressCliente = c.address,
                                           phoneCliente = c.phone,
                                           emailCliente = c.email
                                       }
                                   }).ToList()
                                   .OrderBy(x => x.id)
                                   .ToList();
                    }
                    else
                    {
                        ordenes = (from o in db.Orders
                                   join c in db.PrepaidCustomers on o.prepaidcustomerid equals c.id
                                   join s in db.SumaStatuses on o.sumastatusid equals s.id
                                   where o.documento == Referencia
                                   select new OrdenRecargaPrepago()
                                   {
                                       id = o.id,
                                       statusOrden = s.name,
                                       montoOrden = o.totalamount,
                                       creationdateOrden = o.creationdate,
                                       tipoOrden = o.comments,
                                       documento = o.documento,
                                       observaciones = o.observaciones,
                                       Cliente = new ClientePrepago()
                                       {
                                           idCliente = c.id,
                                           nameCliente = c.name,
                                           aliasCliente = c.alias,
                                           rifCliente = c.rif,
                                           addressCliente = c.address,
                                           phoneCliente = c.phone,
                                           emailCliente = c.email
                                       }
                                   }).ToList()
                                   .OrderBy(x => x.id)
                                   .ToList();
                    }
                }
                //BUSCAR POR Observaciones
                else if (Observaciones != null)
                {
                    if (claseOrden != null)
                    {
                        ordenes = (from o in db.Orders
                                   where o.comments.Equals(claseOrden)
                                   join c in db.PrepaidCustomers on o.prepaidcustomerid equals c.id
                                   join s in db.SumaStatuses on o.sumastatusid equals s.id
                                   where o.observaciones.Contains(Observaciones)
                                   select new OrdenRecargaPrepago()
                                   {
                                       id = o.id,
                                       statusOrden = s.name,
                                       montoOrden = o.totalamount,
                                       creationdateOrden = o.creationdate,
                                       tipoOrden = o.comments,
                                       documento = o.documento,
                                       observaciones = o.observaciones,
                                       Cliente = new ClientePrepago()
                                       {
                                           idCliente = c.id,
                                           nameCliente = c.name,
                                           aliasCliente = c.alias,
                                           rifCliente = c.rif,
                                           addressCliente = c.address,
                                           phoneCliente = c.phone,
                                           emailCliente = c.email
                                       }
                                   }).ToList()
                                   .OrderBy(x => x.id)
                                   .ToList();
                    }
                    else
                    {
                        ordenes = (from o in db.Orders
                                   join c in db.PrepaidCustomers on o.prepaidcustomerid equals c.id
                                   join s in db.SumaStatuses on o.sumastatusid equals s.id
                                   where o.observaciones.Contains(Observaciones)                                   
                                   select new OrdenRecargaPrepago()
                                   {
                                       id = o.id,
                                       statusOrden = s.name,
                                       montoOrden = o.totalamount,
                                       creationdateOrden = o.creationdate,
                                       tipoOrden = o.comments,
                                       documento = o.documento,
                                       observaciones = o.observaciones,
                                       Cliente = new ClientePrepago()
                                       {
                                           idCliente = c.id,
                                           nameCliente = c.name,
                                           aliasCliente = c.alias,
                                           rifCliente = c.rif,
                                           addressCliente = c.address,
                                           phoneCliente = c.phone,
                                           emailCliente = c.email
                                       }
                                   }).ToList()
                                   .OrderBy(x => x.id)
                                   .ToList();
                    }
                }
                //BUSCAR TODAS                
                else
                {
                    if (claseOrden != null)
                    {
                        ordenes = (from o in db.Orders
                                   where o.comments.Equals(claseOrden)
                                   join c in db.PrepaidCustomers on o.prepaidcustomerid equals c.id
                                   join s in db.SumaStatuses on o.sumastatusid equals s.id
                                   select new OrdenRecargaPrepago()
                                   {
                                       id = o.id,
                                       statusOrden = s.name,
                                       montoOrden = o.totalamount,
                                       creationdateOrden = o.creationdate,
                                       tipoOrden = o.comments,
                                       documento = o.documento,
                                       observaciones = o.observaciones,
                                       Cliente = new ClientePrepago()
                                       {
                                           idCliente = c.id,
                                           nameCliente = c.name,
                                           aliasCliente = c.alias,
                                           rifCliente = c.rif,
                                           addressCliente = c.address,
                                           phoneCliente = c.phone,
                                           emailCliente = c.email
                                       }
                                   }).OrderBy(x => x.id).ToList();
                    }
                    else
                    {
                        ordenes = (from o in db.Orders
                                   join c in db.PrepaidCustomers on o.prepaidcustomerid equals c.id
                                   join s in db.SumaStatuses on o.sumastatusid equals s.id
                                   select new OrdenRecargaPrepago()
                                   {
                                       id = o.id,
                                       statusOrden = s.name,
                                       montoOrden = o.totalamount,
                                       creationdateOrden = o.creationdate,
                                       tipoOrden = o.comments,
                                       documento = o.documento,
                                       observaciones = o.observaciones,
                                       Cliente = new ClientePrepago()
                                       {
                                           idCliente = c.id,
                                           nameCliente = c.name,
                                           aliasCliente = c.alias,
                                           rifCliente = c.rif,
                                           addressCliente = c.address,
                                           phoneCliente = c.phone,
                                           emailCliente = c.email
                                       }
                                   }).OrderBy(x => x.id).ToList();
                    }
                }
            }
            return ordenes;
        }

        public OrdenRecargaPrepago Find(int id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString();
                OrdenRecargaPrepago orden = new OrdenRecargaPrepago();
                orden = (from o in db.Orders
                         join c in db.PrepaidCustomers on o.prepaidcustomerid equals c.id
                         join s in db.SumaStatuses on o.sumastatusid equals s.id
                         where o.id == id
                         select new OrdenRecargaPrepago()
                         {
                             id = o.id,
                             statusOrden = s.name,
                             montoOrden = o.totalamount,
                             creationdateOrden = o.creationdate,
                             tipoOrden = o.comments,
                             documento = o.documento,
                             observaciones = o.observaciones,
                             Cliente = new ClientePrepago()
                             {
                                 idCliente = c.id,
                                 nameCliente = c.name,
                                 aliasCliente = c.alias,
                                 rifCliente = c.rif,
                                 addressCliente = c.address,
                                 phoneCliente = c.phone,
                                 emailCliente = c.email
                             }
                         }).FirstOrDefault();

                return orden;
            }
        }

        public List<DetalleOrdenRecargaPrepago> FindDetalleOrden(int id)
        {
            OrdenRecargaPrepago orden = Find(id);
            List<DetalleOrdenRecargaPrepago> detalleorden = new List<DetalleOrdenRecargaPrepago>();
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString();
                detalleorden = (from od in db.OrdersDetails
                                join a in db.Affiliates on od.customerid equals a.id
                                join c in db.CLIENTES on a.docnumber equals c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO
                                join s in db.SumaStatuses on od.sumastatusid equals s.id
                                join t in db.Types on a.typeid equals t.id
                                where od.orderid == id
                                select new DetalleOrdenRecargaPrepago()
                                {
                                    idCliente = orden.Cliente.idCliente,
                                    nameCliente = orden.Cliente.nameCliente,
                                    rifCliente = orden.Cliente.rifCliente,
                                    phoneCliente = orden.Cliente.phoneCliente,
                                    idOrden = orden.id,
                                    tipoOrden = orden.tipoOrden,
                                    statusOrden = orden.statusOrden,
                                    documentoOrden = orden.documento,
                                    observacionesOrden = orden.observaciones,
                                    idAfiliado = a.id,
                                    docnumberAfiliado = a.docnumber,
                                    nameAfiliado = c.NOMBRE_CLIENTE1,
                                    lastname1Afiliado = c.APELLIDO_CLIENTE1,
                                    montoRecarga = od.amount,
                                    resultadoRecarga = od.cardsresponse,
                                    observacionesExclusion = od.comments,
                                    statusDetalleOrden = s.name,
                                    batchid = od.comments
                                }).OrderBy(x => x.docnumberAfiliado).ToList();
            }
            return detalleorden;
        }

        public bool GuardarOrden(List<DetalleOrdenRecargaPrepago> detalleOrden, decimal MontoTotalRecargas)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString();
                //Actualizar estatus detalleOrden
                foreach (var item in detalleOrden)
                {
                    OrdersDetail ordersdetail = db.OrdersDetails.FirstOrDefault(x => x.orderid == item.idOrden && x.customerid == item.idAfiliado);
                    if (item.statusDetalleOrden == "Excluido")
                    {
                        ordersdetail.sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_EXCLUIDO) && (s.tablename == "OrdersDetail")).id;
                        ordersdetail.comments = item.observacionesExclusion;
                    }
                    else if (item.statusDetalleOrden == "Incluido")
                    {
                        ordersdetail.sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_INCLUIDO) && (s.tablename == "OrdersDetail")).id;
                    }
                }
                //Actualizar estatus y monto de la Orden
                Order orden = db.Orders.Find(detalleOrden.First().idOrden);
                orden.totalamount = MontoTotalRecargas;
                orden.documento = detalleOrden.First().documentoOrden;
                orden.observaciones = detalleOrden.First().observacionesOrden;
                orden.processdate = DateTime.Now;
                //Entidad OrderHistory
                int idOrderHistory = OrdersHistoryId();
                OrdersHistory orderhistory = new OrdersHistory()
                {
                    id = idOrderHistory,
                    orderid = orden.id,
                    estatusid = orden.sumastatusid,
                    userid = (int)HttpContext.Current.Session["userid"],
                    creationdate = orden.processdate,
                    comments = "cambios en orden guardados"
                };
                db.OrdersHistories.Add(orderhistory);
                db.SaveChanges();
                return true;
            }
        }

        public bool AprobarOrden(List<DetalleOrdenRecargaPrepago> detalleOrden, decimal MontoTotalRecargas)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString();
                //Actualizar estatus detalleOrden
                foreach (var item in detalleOrden)
                {
                    OrdersDetail ordersdetail = db.OrdersDetails.FirstOrDefault(x => x.orderid == item.idOrden && x.customerid == item.idAfiliado);
                    if (item.statusDetalleOrden == "Excluido")
                    {
                        ordersdetail.sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_EXCLUIDO) && (s.tablename == "OrdersDetail")).id;
                        ordersdetail.comments = item.observacionesExclusion;
                    }
                    else if (item.statusDetalleOrden == "Incluido")
                    {
                        ordersdetail.sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_APROBADO) && (s.tablename == "OrdersDetail")).id;
                    }
                }
                //Actualizar estatus y monto de la Orden
                Order orden = db.Orders.Find(detalleOrden.First().idOrden);
                orden.sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_ORDEN_APROBADA) && (s.tablename == "Order")).id;
                orden.totalamount = MontoTotalRecargas;
                orden.documento = detalleOrden.First().documentoOrden;
                orden.observaciones = detalleOrden.First().observacionesOrden;
                orden.processdate = DateTime.Now;
                //Entidad OrderHistory
                int idOrderHistory = OrdersHistoryId();
                OrdersHistory orderhistory = new OrdersHistory()
                {
                    id = idOrderHistory,
                    orderid = orden.id,
                    estatusid = orden.sumastatusid,
                    userid = (int)HttpContext.Current.Session["userid"],
                    creationdate = orden.processdate,
                    comments = "orden aprobada"
                };
                db.OrdersHistories.Add(orderhistory);
                db.SaveChanges();
                return true;
            }
        }

        public bool RechazarOrden(int id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString();
                Order orden = db.Orders.FirstOrDefault(o => o.id.Equals(id));
                orden.sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_ORDEN_RECHAZADA) && (s.tablename == "Order")).id;
                orden.processdate = DateTime.Now;
                //Entidad OrderHistory
                int idOrderHistory = OrdersHistoryId();
                OrdersHistory orderhistory = new OrdersHistory()
                {
                    id = idOrderHistory,
                    orderid = orden.id,
                    estatusid = orden.sumastatusid,
                    userid = (int)HttpContext.Current.Session["userid"],
                    creationdate = orden.processdate,
                    comments = "orden rechazada"
                };
                db.OrdersHistories.Add(orderhistory);
                db.SaveChanges();
                return true;
            }
        }

        private bool Recargar(DetalleOrdenRecargaPrepago detalleorden)
        {
            int intentos;
            //Se intenta la operación 3 veces, antes de fallar
            for (intentos = 0; intentos <= 3; intentos++)
            {
                //Se llama al servicio para verificar q este activo
                //SERVICIO WSL.Cards.getClient !
                string clienteCardsJson = WSL.Cards.getClient(detalleorden.docnumberAfiliado.Substring(2));
                if (WSL.Cards.ExceptionServicioCards(clienteCardsJson))
                {
                    intentos++;
                }
                else
                {
                    string montoSinSeparador = Math.Truncate(detalleorden.montoRecarga * 100).ToString();
                    string RespuestaCardsJson = WSL.Cards.addBatch(detalleorden.docnumberAfiliado.Substring(2), montoSinSeparador, Globals.TRANSCODE_RECARGA_PREPAGO, "NULL");
                    if (WSL.Cards.ExceptionServicioCards(RespuestaCardsJson))
                    {
                        ExceptionJSON exceptionJson = (ExceptionJSON)JsonConvert.DeserializeObject<ExceptionJSON>(RespuestaCardsJson);
                        detalleorden.resultadoRecarga = exceptionJson.detail + "-" + exceptionJson.source;
                        intentos++;
                    }
                    else
                    {
                        RespuestaCards RespuestaCards = (RespuestaCards)JsonConvert.DeserializeObject<RespuestaCards>(RespuestaCardsJson);
                        if ((Convert.ToDecimal(RespuestaCards.excode) < 0))
                        {
                            detalleorden.resultadoRecarga = RespuestaCards.exdetail;
                            return false;
                        }
                        else
                        {
                            detalleorden.resultadoRecarga = RespuestaCards.exdetail;
                            return true;
                        }
                    }
                }

            }
            return (intentos < 3);
        }

        private bool Anular(DetalleOrdenRecargaPrepago detalleorden)
        {
            int intentos;
            //Se intenta la operación 3 veces, antes de fallar
            for (intentos = 0; intentos <= 3; intentos++)
            {
                //Se llama al servicio para verificar q este activo
                //SERVICIO WSL.Cards.getClient !
                string clienteCardsJson = WSL.Cards.getClient(detalleorden.docnumberAfiliado.Substring(2));
                if (WSL.Cards.ExceptionServicioCards(clienteCardsJson))
                {
                    intentos++;
                }
                else
                {
                    string RespuestaCardsJson = WSL.Cards.addBatchAnulacion(detalleorden.docnumberAfiliado.Substring(2), Globals.TRANSCODE_ANULACION_PREPAGO, detalleorden.batchid, (string)HttpContext.Current.Session["login"]);
                    if (WSL.Cards.ExceptionServicioCards(RespuestaCardsJson))
                    {
                        ExceptionJSON exceptionJson = (ExceptionJSON)JsonConvert.DeserializeObject<ExceptionJSON>(RespuestaCardsJson);
                        detalleorden.resultadoRecarga = exceptionJson.detail + "-" + exceptionJson.source;
                        intentos++;
                    }
                    else
                    {
                        RespuestaCards RespuestaCards = (RespuestaCards)JsonConvert.DeserializeObject<RespuestaCards>(RespuestaCardsJson);
                        if ((Convert.ToDecimal(RespuestaCards.excode) < 0))
                        {
                            detalleorden.resultadoRecarga = RespuestaCards.exdetail;
                            return false;
                        }
                        else
                        {
                            detalleorden.resultadoRecarga = RespuestaCards.exdetail;
                            return true;
                        }
                    }
                }
            }
            return (intentos < 3);
        }

        public bool ProcesarOrden(int id)
        {
            List<DetalleOrdenRecargaPrepago> detalleOrden = FindDetalleOrden(id);
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString();
                Order orden = db.Orders.Find(id);
                //PROCESAR ORDENES DE RECARGA
                if (orden.comments.Contains("Orden de Recarga"))
                {
                    //Recargar y Actualizar estatus detalleorden
                    foreach (DetalleOrdenRecargaPrepago item in detalleOrden)
                    {
                        OrdersDetail ordersdetail = db.OrdersDetails.FirstOrDefault(x => x.orderid == item.idOrden && x.customerid == item.idAfiliado);
                        if (item.statusDetalleOrden == "Aprobado")
                        {
                            ordersdetail.sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                            if (Recargar(item))
                            {
                                ordersdetail.comments = "Recarga efectiva";
                                ordersdetail.cardsresponse = item.resultadoRecarga;
                            }
                            else
                            {
                                ordersdetail.comments = "Recarga fallida";
                                ordersdetail.cardsresponse = item.resultadoRecarga;
                            }
                        }
                        db.SaveChanges();
                    }                                        
                }
                //PROCESAR ORDENES DE ANULACION
                else
                {
                    //Anular y Actualizar estatus detalleorden
                    foreach (DetalleOrdenRecargaPrepago item in detalleOrden)
                    {
                        OrdersDetail ordersdetail = db.OrdersDetails.FirstOrDefault(x => x.orderid == item.idOrden && x.customerid == item.idAfiliado);
                        if (item.statusDetalleOrden == "Aprobado")
                        {
                            ordersdetail.sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                            if (Anular(item))
                            {
                                ordersdetail.comments = "Anulación efectiva " + ordersdetail.comments;
                                ordersdetail.cardsresponse = item.resultadoRecarga;
                            }
                            else
                            {
                                ordersdetail.comments = "Anulación fallida " + ordersdetail.comments;
                                ordersdetail.cardsresponse = item.resultadoRecarga;
                            }
                        }
                        db.SaveChanges();
                    }
                }
                //Actualizar estatus de la Orden
                orden.sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_ORDEN_PROCESADA) && (s.tablename == "Order")).id;
                orden.documento = detalleOrden.First().documentoOrden;
                orden.processdate = DateTime.Now;
                //Entidad OrderHistory
                int idOrderHistory = OrdersHistoryId();
                OrdersHistory orderhistory = new OrdersHistory()
                {
                    id = idOrderHistory,
                    orderid = orden.id,
                    estatusid = orden.sumastatusid,
                    userid = (int)HttpContext.Current.Session["userid"],
                    creationdate = orden.processdate,
                    comments = "orden procesada"
                };
                db.OrdersHistories.Add(orderhistory);
                db.SaveChanges();
                return true;
            }
        }        

        public List<DetalleOrdenRecargaPrepago> DetalleParaOrden(ClientePrepago cliente, List<BeneficiarioPrepagoIndex> beneficiarios)
        {
            List<DetalleOrdenRecargaPrepago> detalleOrden = new List<DetalleOrdenRecargaPrepago>();
            foreach (BeneficiarioPrepagoIndex item in beneficiarios)
            {
                DetalleOrdenRecargaPrepago detalle = new DetalleOrdenRecargaPrepago()
                {
                    idCliente = cliente.idCliente,
                    nameCliente = cliente.nameCliente,
                    rifCliente = cliente.rifCliente,
                    phoneCliente = cliente.phoneCliente,
                    tipoOrden = "Orden de Recarga",
                    idAfiliado = item.Afiliado.id,
                    docnumberAfiliado = item.Afiliado.docnumber,
                    nameAfiliado = item.Afiliado.name,
                    lastname1Afiliado = item.Afiliado.lastname1,
                    montoRecarga = Convert.ToDecimal(0),
                    statusDetalleOrden = ""
                };
                detalleOrden.Add(detalle);
            }
            return detalleOrden;
        }

        public List<DetalleOrdenRecargaPrepago> DetalleParaOrdenArchivo(ClientePrepago cliente, List<BeneficiarioPrepagoIndex> beneficiarios, List<DetalleOrdenRecargaPrepago> detalleOrdenArchivo)
        {
            List<DetalleOrdenRecargaPrepago> detalleOrdenBase = DetalleParaOrden(cliente, beneficiarios);
            List<DetalleOrdenRecargaPrepago> detalleOrden = new List<DetalleOrdenRecargaPrepago>();
            DetalleOrdenRecargaPrepago detallebeneficiario;
            foreach (DetalleOrdenRecargaPrepago detalleArchivo in detalleOrdenArchivo)
            {
                detallebeneficiario = detalleOrdenBase.Find(x => x.docnumberAfiliado == detalleArchivo.docnumberAfiliado);
                if (detallebeneficiario == null)
                {
                    //la ci del archivo no fue encontrada en la lista de beneficiarios activos del cliente
                    detallebeneficiario = new DetalleOrdenRecargaPrepago()
                    {
                        idCliente = detalleOrdenBase.First().idCliente,
                        nameCliente = detalleOrdenBase.First().nameCliente,
                        rifCliente = detalleOrdenBase.First().rifCliente,
                        phoneCliente = detalleOrdenBase.First().phoneCliente,
                        tipoOrden = "Orden de Recarga desde Archivo",
                        docnumberAfiliado = detalleArchivo.docnumberAfiliado,
                        montoRecarga = 0,
                        statusDetalleOrden = "No encontrado",
                    };
                    detalleOrden.Add(detallebeneficiario);
                }
                else
                {
                    //la ci del archivo fue encontrada en la lista de beneficiarios activos del cliente
                    detallebeneficiario.tipoOrden = "Orden de Recarga desde Archivo";
                    detallebeneficiario.montoRecarga = detalleArchivo.montoRecarga;
                    detallebeneficiario.statusDetalleOrden = "";
                    detalleOrden.Add(detallebeneficiario);
                }
            }
            return detalleOrden;
        }

        public List<DetalleOrdenRecargaPrepago> DetalleParaOrdenAnulacion(string batchid)
        {
            List<DetalleOrdenRecargaPrepago> detalleorden = new List<DetalleOrdenRecargaPrepago>();
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString();
                //verificar que ese batchid no tenga orden de anulación
                var query = (from od in db.OrdersDetails
                             where od.comments.Equals(batchid) || od.comments.Equals("Anulación efectiva " + batchid)
                             select od).ToList();
                if (query.Count > 0)
                {
                    DetalleOrdenRecargaPrepago d = new DetalleOrdenRecargaPrepago()
                    {
                        batchid = "Ya tiene Anulación"
                    };
                    detalleorden.Add(d);
                    return detalleorden;
                }
                //busacr detalle para crear orden de anulación
                else
                {
                    detalleorden = (from od in db.OrdersDetails
                                    where od.cardsresponse.Contains(batchid)
                                    join o in db.Orders on od.orderid equals o.id
                                    join pc in db.PrepaidCustomers on o.prepaidcustomerid equals pc.id
                                    join a in db.Affiliates on od.customerid equals a.id
                                    join c in db.CLIENTES on a.docnumber equals c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO
                                    join s in db.SumaStatuses on od.sumastatusid equals s.id
                                    join t in db.Types on a.typeid equals t.id
                                    select new DetalleOrdenRecargaPrepago()
                                    {
                                        idCliente = pc.id,
                                        nameCliente = pc.name,
                                        rifCliente = pc.rif,
                                        phoneCliente = pc.phone,
                                        tipoOrden = "Orden de Anulación",
                                        batchid = batchid,
                                        statusOrden = "",
                                        idAfiliado = a.id,
                                        docnumberAfiliado = a.docnumber,
                                        nameAfiliado = c.NOMBRE_CLIENTE1,
                                        lastname1Afiliado = c.APELLIDO_CLIENTE1,
                                        montoRecarga = od.amount,
                                        resultadoRecarga = od.cardsresponse,
                                        observacionesExclusion = od.comments,
                                        statusDetalleOrden = s.name
                                    }).ToList();
                    return detalleorden;
                }                
            }
        }

        public int CrearOrden(int idCliente, List<DetalleOrdenRecargaPrepago> detalleOrden, decimal MontoTotalRecargas)
        {
            int idOrden = 0;
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString();
                //ENTIDAD Order                   
                Order Order = new Order()
                {
                    id = OrderId(),
                    prepaidcustomerid = idCliente,
                    totalamount = MontoTotalRecargas,
                    paymenttype = "",
                    creationdate = DateTime.Now,
                    creationuserid = (int)HttpContext.Current.Session["userid"],
                    processdate = DateTime.Now,
                    comments = detalleOrden.First().tipoOrden,
                    documento = detalleOrden.First().documentoOrden,
                    observaciones = detalleOrden.First().observacionesOrden,
                    sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_ORDEN_NUEVA) && (s.tablename == "Order")).id
                };
                db.Orders.Add(Order);
                idOrden = Order.id;
                int idbase = OrdersDetailId();
                detalleOrden = detalleOrden.FindAll(x => x.montoRecarga != 0).ToList();
                foreach (DetalleOrdenRecargaPrepago item in detalleOrden)
                {
                    idbase = idbase + 1;
                    //ENTIDAD OrderDetail    
                    OrdersDetail OrderDetail = new OrdersDetail()
                    {
                        id = idbase,
                        orderid = Order.id,
                        customerid = item.idAfiliado,
                        amount = item.montoRecarga,
                        sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_INCLUIDO) && (s.tablename == "OrdersDetail")).id
                    };
                    if (Order.comments.Contains("Orden de Anulación"))
                    {
                        OrderDetail.comments = item.batchid;
                    }
                    db.OrdersDetails.Add(OrderDetail);
                }
                //Entidad OrderHistory
                int idOrderHistory = OrdersHistoryId();
                OrdersHistory orderhistory = new OrdersHistory()
                {
                    id = idOrderHistory,
                    orderid = idOrden,
                    estatusid = Order.sumastatusid,
                    userid = (int)HttpContext.Current.Session["userid"],
                    creationdate = DateTime.Now,
                    comments = "orden creada"
                };
                db.OrdersHistories.Add(orderhistory);
                db.SaveChanges();
                return idOrden;
            }
        }

        public List<DetalleOrdenRecargaPrepago> GetBeneficiariosArchivo(string fichero)
        {
            List<DetalleOrdenRecargaPrepago> detalleOrdenArchivo = LeerArchivoRecargas(fichero);
            if (detalleOrdenArchivo != null)
            {
                //uso una copia de la lista para iterar en el foreach - que es de sólo lectura
                List<DetalleOrdenRecargaPrepago> Listaparaiterar = detalleOrdenArchivo.ToList();
                foreach (var item in Listaparaiterar)
                {
                    //Validación de filas devueltas en la lectura ciega del archivo con expresiones regulares
                    var RegExPattern = @"^([VvEeJjGg]){1}(-){1}(\d){3,10}$";
                    //var RegExPattern2 = @"^([VvEeJjGg]){1}(\d){3,10}$";
                    var RegExPattern3 = @"^([Pp]){1}(-){1}([A-Za-z0-9]){3,10}$";
                    //var RegExPattern4 = @"^([Pp]){1}([A-Za-z0-9]){3,10}$";
                    var RegExPattern5 = @"^(\d|-)?(\d|,)*\.?\d*$";
                    if (item.docnumberAfiliado == null)
                    {
                        //si no cumple la validación, lo saco de la lista
                        detalleOrdenArchivo.Remove(item);
                    }
                    else if (!Regex.IsMatch(item.docnumberAfiliado, RegExPattern) && !Regex.IsMatch(item.docnumberAfiliado, RegExPattern3))
                    {
                        //si no cumple la validación, lo saco de la lista
                        detalleOrdenArchivo.Remove(item);
                    }
                    else if (!Regex.IsMatch(item.montoRecarga.ToString(), RegExPattern5))
                    {
                        //si no cumple la validación, lo saco de la lista
                        detalleOrdenArchivo.Remove(item);
                    }
                }
                //solo devuelvo aquellos que sean valore positivos en el monto
                return detalleOrdenArchivo.Where(x => x.montoRecarga > 0).ToList();
            }
            else
            {
                return null;
            }
        }

        private List<DetalleOrdenRecargaPrepago> LeerArchivoRecargas(string pathDelArchivoExcel)
        {
            List<DetalleOrdenRecargaPrepago> registros = new List<DetalleOrdenRecargaPrepago>();
            try
            {
                var excel = new ExcelQueryFactory(pathDelArchivoExcel);
                var resultado = (from c in excel.Worksheet("Hoja1")
                                 select c).ToList();
                excel.Dispose();
                //convertir lo leido en DetalleOrdenRecargaPrepago                
                foreach (var item in resultado)
                {
                    string cedula = item[0];
                    string monto = item[1];
                    if (cedula != "" && monto != "")
                    {
                        DetalleOrdenRecargaPrepago registro = new DetalleOrdenRecargaPrepago()
                        {
                            docnumberAfiliado = cedula,
                            montoRecarga = Convert.ToDecimal(monto)
                        };
                        registros.Add(registro);
                    }
                }
                if (registros.Count != 0)
                {
                    return registros.OrderBy(x => x.docnumberAfiliado).ToList();
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public List<PrepaidCustomer> GetClientes()
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString();
                return db.PrepaidCustomers.OrderBy(u => u.name).ToList();
            }
        }

    }
}