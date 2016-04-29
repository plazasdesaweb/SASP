using Newtonsoft.Json;
using Suma2Lealtad.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Suma2Lealtad.Models.Repositorios
{
    public class OperacionesRepository
    {
        private OrdenRecargaRepository repOrden = new OrdenRecargaRepository();
        private AfiliadoSumaRepository repAfiliado = new AfiliadoSumaRepository();

        private string Transferir(string numdocOrigen, string numdocDestino, string tipoCuenta, string monto)
        {
            int intentos;
            string respuesta = "";
            //Se intenta la operación 3 veces, antes de fallar
            for (intentos = 0; intentos <= 3; intentos++)
            {
                //Se llama al servicio para verificar q este activo
                //SERVICIO WSL.Cards.getClient !
                string clienteCardsJson = WSL.Cards.getClient(numdocOrigen.Substring(2));
                if (WSL.Cards.ExceptionServicioCards(clienteCardsJson))
                {
                    respuesta = "Servicio no responde";
                    intentos++;
                }
                else
                {
                    string montoSinSeparador;
                    if (tipoCuenta == Globals.TIPO_CUENTA_PREPAGO)
                    {
                        montoSinSeparador = Math.Truncate(Convert.ToDecimal(monto) * 100).ToString();
                    }
                    else
                    {
                        montoSinSeparador = monto;
                    }
                    string RespuestaCardsJson = WSL.Cards.addTransfer(numdocOrigen.Substring(2), numdocDestino.Substring(2), montoSinSeparador, tipoCuenta, (string)HttpContext.Current.Session["login"]);
                    if (WSL.Cards.ExceptionServicioCards(RespuestaCardsJson))
                    {
                        ExceptionJSON exceptionJson = (ExceptionJSON)JsonConvert.DeserializeObject<ExceptionJSON>(RespuestaCardsJson);
                        respuesta = exceptionJson.detail + "-" + exceptionJson.source;
                        intentos++;
                    }
                    else
                    {
                        RespuestaCards RespuestaCards = (RespuestaCards)JsonConvert.DeserializeObject<RespuestaCards>(RespuestaCardsJson);
                        if (Convert.ToDecimal(RespuestaCards.excode) < 0)
                        {
                            return RespuestaCards.exdetail;
                        }
                        else
                        {
                            return RespuestaCards.exdetail;
                        }
                    }
                }
            }
            return respuesta;
        }

        private string Anular(string docnumber, string batchid, string Transcode)
        {
            int intentos;
            string respuesta = "";
            //Se intenta la operación 3 veces, antes de fallar
            for (intentos = 0; intentos <= 3; intentos++)
            {
                //Se llama al servicio para verificar q este activo
                //SERVICIO WSL.Cards.getClient !
                string clienteCardsJson = WSL.Cards.getClient(docnumber.Substring(2));
                if (WSL.Cards.ExceptionServicioCards(clienteCardsJson))
                {
                    respuesta = "Servicio no responde";
                    intentos++;
                }
                else
                {
                    string RespuestaCardsJson = WSL.Cards.addBatchAnulacion(docnumber.Substring(2), Transcode, batchid, (string)HttpContext.Current.Session["login"]);
                    if (WSL.Cards.ExceptionServicioCards(RespuestaCardsJson))
                    {
                        ExceptionJSON exceptionJson = (ExceptionJSON)JsonConvert.DeserializeObject<ExceptionJSON>(RespuestaCardsJson);
                        respuesta = exceptionJson.detail + "-" + exceptionJson.source;
                        intentos++;
                    }
                    else
                    {
                        RespuestaCards RespuestaCards = (RespuestaCards)JsonConvert.DeserializeObject<RespuestaCards>(RespuestaCardsJson);
                        if (Convert.ToDecimal(RespuestaCards.excode) < 0)
                        {
                            return RespuestaCards.exdetail;
                        }
                        else
                        {
                            return RespuestaCards.exdetail;
                        }
                    }
                }
            }
            return respuesta;
        }

        public bool ProcesarTransferencia(int id)
        {
            Transferencia transferencia = FindTransferencia(id);
            string respuestaSuma = "";
            string respuestaPrepago = "";
            string mensaje = "";
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                Order orden = db.Orders.Find(transferencia.id);
                List<OrdersDetail> ordersdetails = db.OrdersDetails.Where(x => x.orderid == orden.id).ToList();
                if (orden.comments.Contains("Orden de Transferencia"))
                {
                    //realizar transferencias
                    if (transferencia.ResumenTransferenciaSuma != "0")
                    {
                        respuestaSuma = Transferir(transferencia.docnumberAfiliadoOrigen, transferencia.docnumberAfiliadoDestino, Globals.TIPO_CUENTA_SUMA, transferencia.ResumenTransferenciaSuma);
                        long number1 = 0;
                        bool canConvert = long.TryParse(respuestaSuma, out number1);
                        if (canConvert == false)
                        {
                            mensaje = "Falló transferencia Suma (" + respuestaSuma + "). ";
                            ordersdetails.First().comments = "Transferencia Suma fallida";
                            ordersdetails.First().cardsresponse = respuestaSuma;
                            ordersdetails.First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                            ordersdetails.Skip(2).First().comments = "Transferencia Suma fallida";
                            ordersdetails.Skip(2).First().cardsresponse = respuestaSuma;
                            ordersdetails.Skip(2).First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                        }
                        else
                        {
                            mensaje = "Transferencia Suma efectiva con clave " + respuestaSuma + ". ";
                            ordersdetails.First().comments = "Transferencia Suma efectiva";
                            ordersdetails.First().cardsresponse = respuestaSuma;
                            ordersdetails.First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                            ordersdetails.Skip(2).First().comments = "Transferencia Suma efectiva";
                            ordersdetails.Skip(2).First().cardsresponse = (Convert.ToInt32(respuestaSuma) + 1).ToString();
                            ordersdetails.Skip(2).First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                        }
                    }
                    else
                    {
                        ordersdetails.First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                        ordersdetails.First().comments = "";
                        ordersdetails.First().cardsresponse = "";
                        ordersdetails.Skip(2).First().comments = "";
                        ordersdetails.Skip(2).First().cardsresponse = "";
                        ordersdetails.Skip(2).First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                    }
                    if (transferencia.ResumenTransferenciaPrepago != "0,00")
                    {
                        respuestaPrepago = Transferir(transferencia.docnumberAfiliadoOrigen, transferencia.docnumberAfiliadoDestino, Globals.TIPO_CUENTA_PREPAGO, transferencia.ResumenTransferenciaPrepago);
                        long number1 = 0;
                        bool canConvert = long.TryParse(respuestaPrepago, out number1);
                        if (canConvert == false)
                        {
                            mensaje = mensaje + "Falló transferencia Prepago (" + respuestaPrepago + ").";
                            ordersdetails.Skip(1).First().comments = "Transferencia Prepago fallida";
                            ordersdetails.Skip(1).First().cardsresponse = respuestaPrepago;
                            ordersdetails.Skip(1).First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                            ordersdetails.Skip(3).First().comments = "Transferencia Prepago fallida";
                            ordersdetails.Skip(3).First().cardsresponse = respuestaPrepago;
                            ordersdetails.Skip(3).First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                        }
                        else
                        {
                            mensaje = mensaje + "Transferencia Prepago efectiva con clave " + respuestaPrepago + ".";
                            ordersdetails.Skip(1).First().comments = "Transferencia Prepago efectiva";
                            ordersdetails.Skip(1).First().cardsresponse = respuestaPrepago;
                            ordersdetails.Skip(1).First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                            ordersdetails.Skip(3).First().comments = "Transferencia Prepago efectiva";
                            ordersdetails.Skip(3).First().cardsresponse = (Convert.ToInt32(respuestaPrepago) + 1).ToString();
                            ordersdetails.Skip(3).First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                        }
                    }
                    else
                    {
                        ordersdetails.Skip(1).First().comments = "";
                        ordersdetails.Skip(1).First().cardsresponse = "";
                        ordersdetails.Skip(1).First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                        ordersdetails.Skip(3).First().comments = "";
                        ordersdetails.Skip(3).First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                        ordersdetails.Skip(3).First().cardsresponse = "";
                    }
                    db.SaveChanges();
                }
                else
                {
                    //Anular Transferencias y Actualizar estatus detalleorden
                    if (transferencia.ResumenTransferenciaSuma != "0")
                    {
                        //ANULO DEBITO SUMA                    
                        respuestaSuma = Anular(transferencia.docnumberAfiliadoOrigen, ordersdetails.First().comments, Globals.TRANSCODE_ANULACION);
                        long number1 = 0;
                        bool canConvert = long.TryParse(respuestaSuma, out number1);
                        if (canConvert == false)
                        {
                            mensaje = "Falló Anulación de transferencia Suma (" + respuestaSuma + "). ";
                            ordersdetails.First().comments = "Anulación fallida " + ordersdetails.First().comments;
                            ordersdetails.First().cardsresponse = respuestaSuma;
                            ordersdetails.First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                        }
                        else
                        {
                            mensaje = "Anulación de Transferencia Suma efectiva con clave " + respuestaSuma + ". ";
                            ordersdetails.First().comments = "Anulación efectiva " + ordersdetails.First().comments;
                            ordersdetails.First().cardsresponse = respuestaSuma;
                            ordersdetails.First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;

                        }
                        //ANULO CREDITO SUMA
                        respuestaSuma = Anular(transferencia.docnumberAfiliadoDestino, ordersdetails.Skip(2).First().comments, Globals.TRANSCODE_ANULACION);
                        number1 = 0;
                        canConvert = long.TryParse(respuestaSuma, out number1);
                        if (canConvert == false)
                        {
                            mensaje = "Falló Anulación de transferencia Suma (" + respuestaSuma + "). ";
                            ordersdetails.Skip(2).First().comments = "Anulación fallida " + ordersdetails.Skip(2).First().comments;
                            ordersdetails.Skip(2).First().cardsresponse = respuestaSuma;
                            ordersdetails.Skip(2).First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                        }
                        else
                        {
                            mensaje = "Anulación de Transferencia Suma efectiva con clave " + respuestaSuma + ". ";
                            ordersdetails.Skip(2).First().comments = "Anulación efectiva " + ordersdetails.Skip(2).First().comments;
                            ordersdetails.Skip(2).First().cardsresponse = respuestaSuma;
                            ordersdetails.Skip(2).First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                        }
                    }
                    else
                    {
                        ordersdetails.First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                        ordersdetails.First().comments = "";
                        ordersdetails.First().cardsresponse = "";
                        ordersdetails.Skip(2).First().comments = "";
                        ordersdetails.Skip(2).First().cardsresponse = "";
                        ordersdetails.Skip(2).First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                    }
                    if (transferencia.ResumenTransferenciaPrepago != "0,00")
                    {
                        //ANULO DEBITO PREPAGO
                        respuestaPrepago = Anular(transferencia.docnumberAfiliadoOrigen, ordersdetails.Skip(1).First().comments, Globals.TRANSCODE_ANULACION);
                        long number1 = 0;
                        bool canConvert = long.TryParse(respuestaPrepago, out number1);
                        if (canConvert == false)
                        {
                            mensaje = "Falló Anulación de transferencia Prepago (" + respuestaPrepago + "). ";
                            ordersdetails.Skip(1).First().comments = "Anulación fallida " + ordersdetails.Skip(1).First().comments;
                            ordersdetails.Skip(1).First().cardsresponse = respuestaPrepago;
                            ordersdetails.Skip(1).First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                        }
                        else
                        {
                            mensaje = "Anulación de Transferencia Prepago efectiva con clave " + respuestaPrepago + ". ";
                            ordersdetails.Skip(1).First().comments = "Anulación efectiva " + ordersdetails.Skip(1).First().comments;
                            ordersdetails.Skip(1).First().cardsresponse = respuestaPrepago;
                            ordersdetails.Skip(1).First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                        }
                        //ANULO CREDITO PREPAGO
                        respuestaPrepago = Anular(transferencia.docnumberAfiliadoDestino, ordersdetails.Skip(3).First().comments, Globals.TRANSCODE_ANULACION);
                        number1 = 0;
                        canConvert = long.TryParse(respuestaPrepago, out number1);
                        if (canConvert == false)
                        {
                            mensaje = "Falló Anulación de transferencia Prepago (" + respuestaPrepago + "). ";
                            ordersdetails.Skip(3).First().comments = "Anulación fallida " + ordersdetails.Skip(3).First().comments;
                            ordersdetails.Skip(3).First().cardsresponse = respuestaPrepago;
                            ordersdetails.Skip(3).First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                        }
                        else
                        {
                            mensaje = "Anulación de Transferencia Prepago efectiva con clave " + respuestaPrepago + ". ";
                            ordersdetails.Skip(3).First().comments = "Anulación efectiva " + ordersdetails.Skip(3).First().comments;
                            ordersdetails.Skip(3).First().cardsresponse = respuestaPrepago;
                            ordersdetails.Skip(3).First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                        }
                    }
                    else
                    {
                        ordersdetails.Skip(1).First().comments = "";
                        ordersdetails.Skip(1).First().cardsresponse = "";
                        ordersdetails.Skip(1).First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                        ordersdetails.Skip(3).First().comments = "";
                        ordersdetails.Skip(3).First().sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_PROCESADO) && (s.tablename == "OrdersDetail")).id;
                        ordersdetails.Skip(3).First().cardsresponse = "";
                    }
                    db.SaveChanges();
                }
                //Actualizar estatus de la Orden
                orden.sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_ORDEN_PROCESADA) && (s.tablename == "Orders")).id;
                orden.processdate = DateTime.Now;
                //Entidad OrderHistory
                int idOrderHistory = repOrden.OrdersHistoryId();
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

        public bool AprobarTransferencia(Transferencia transferencia)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                Order orden = db.Orders.Find(transferencia.id);
                List<OrdersDetail> ordersdetails = db.OrdersDetails.Where(x => x.orderid == orden.id).ToList();
                foreach (OrdersDetail od in ordersdetails)
                {
                    OrdersDetail od2 = db.OrdersDetails.Find(od.id, od.orderid);
                    od2.sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_DETALLEORDEN_APROBADO) && (s.tablename == "OrdersDetail")).id;
                }
                //Actualizar estatus y monto de la Orden
                orden.sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_ORDEN_APROBADA) && (s.tablename == "Orders")).id;
                orden.totalamount = 0;
                orden.processdate = DateTime.Now;
                //Entidad OrderHistory
                int idOrderHistory = repOrden.OrdersHistoryId();
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

        public bool RechazarTransferencia(int id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                Order orden = db.Orders.FirstOrDefault(o => o.id.Equals(id));
                orden.sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_ORDEN_RECHAZADA) && (s.tablename == "Orders")).id;
                orden.processdate = DateTime.Now;
                //Entidad OrderHistory
                int idOrderHistory = repOrden.OrdersHistoryId();
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

        public int CrearTransferencia(int idCliente, List<DetalleOrdenRecargaPrepago> detalleOrden)
        {
            int idOrden = 0;
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                //ENTIDAD Order                   
                Order Order = new Order()
                {
                    id = repOrden.OrderId(),
                    prepaidcustomerid = idCliente,
                    totalamount = 0,
                    paymenttype = "",
                    creationdate = DateTime.Now,
                    creationuserid = (int)HttpContext.Current.Session["userid"],
                    processdate = DateTime.Now,
                    comments = detalleOrden.First().tipoOrden,
                    sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_ORDEN_NUEVA) && (s.tablename == "Orders")).id
                };
                db.Orders.Add(Order);
                idOrden = Order.id;
                int idbase = repOrden.OrdersDetailId();
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
                    if (detalleOrden.First().tipoOrden == "Orden de Anulación de Transferencia")
                    {
                        long number1 = 0;
                        bool canConvert = long.TryParse(item.batchid, out number1);
                        //si la transferencia no fue exitosa, creo el renglon de la orden con monto 0
                        if (canConvert == false)
                        {
                            OrderDetail.amount = 0;
                        }
                        else
                        {
                            OrderDetail.amount = item.montoRecarga;
                            OrderDetail.comments = item.batchid;
                        }
                    }
                    db.OrdersDetails.Add(OrderDetail);
                }
                //Entidad OrderHistory
                int idOrderHistory = repOrden.OrdersHistoryId();
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

        public Transferencia FindTransferencia(int id)
        {
            //llenar modelo transferencia desde los datos de la orden
            List<DetalleOrdenRecargaPrepago> detalleOrden = repOrden.FindDetalleOrden(id);
            Transferencia transferencia = new Transferencia();
            AfiliadoSuma afiliadoOrigen = repAfiliado.Find(detalleOrden.First().idAfiliado);
            SaldosMovimientos SaldosMovimientos = repAfiliado.FindSaldosMovimientos(afiliadoOrigen);
            transferencia.docnumberAfiliadoOrigen = afiliadoOrigen.docnumber;
            transferencia.idAfiliadoOrigen = afiliadoOrigen.id;
            transferencia.nameAfiliadoOrigen = afiliadoOrigen.name;
            transferencia.lastname1AfiliadoOrigen = afiliadoOrigen.lastname1;
            transferencia.typeAfiliadoOrigen = afiliadoOrigen.type;
            transferencia.datosCuentaSumaOrigen = SaldosMovimientos.Saldos.First(x => x.accounttype.Equals(Globals.TIPO_CUENTA_SUMA));
            transferencia.DenominacionCuentaOrigenSuma = "Más";
            transferencia.datosCuentaPrepagoOrigen = SaldosMovimientos.Saldos.First(x => x.accounttype.Equals(Globals.TIPO_CUENTA_PREPAGO));
            transferencia.DenominacionCuentaOrigenPrepago = "Bs.";
            transferencia.statusDetalleOrdenOrigenSuma = detalleOrden.First().statusDetalleOrden;
            transferencia.resultadoTransferenciaOrigenSuma = detalleOrden.First().observacionesExclusion + " (" + detalleOrden.First().resultadoRecarga + ")";
            transferencia.statusDetalleOrdenOrigenPrepago = detalleOrden.Skip(1).First().statusDetalleOrden;
            transferencia.resultadoTransferenciaOrigenPrepago = detalleOrden.Skip(1).First().observacionesExclusion + " (" + detalleOrden.Skip(1).First().resultadoRecarga + ")";
            AfiliadoSuma afiliadoDestino = repAfiliado.Find(detalleOrden.SkipWhile(x => x.idAfiliado == afiliadoOrigen.id).First().idAfiliado);
            SaldosMovimientos = repAfiliado.FindSaldosMovimientos(afiliadoDestino);
            transferencia.docnumberAfiliadoDestino = afiliadoDestino.docnumber;
            transferencia.idAfiliadoDestino = afiliadoDestino.id;
            transferencia.nameAfiliadoDestino = afiliadoDestino.name;
            transferencia.lastname1AfiliadoDestino = afiliadoDestino.lastname1;
            transferencia.datosCuentaSumaDestino = SaldosMovimientos.Saldos.First(x => x.accounttype.Equals(Globals.TIPO_CUENTA_SUMA));
            transferencia.datosCuentaPrepagoDestino = SaldosMovimientos.Saldos.First(x => x.accounttype.Equals(Globals.TIPO_CUENTA_PREPAGO));
            transferencia.statusDetalleOrdenDestinoSuma = detalleOrden.Skip(2).First().statusDetalleOrden;
            transferencia.resultadoTransferenciaDestinoSuma = detalleOrden.Skip(2).First().observacionesExclusion + " (" + detalleOrden.Skip(2).First().resultadoRecarga + ")";
            transferencia.statusDetalleOrdenDestinoPrepago = detalleOrden.Skip(3).First().statusDetalleOrden;
            transferencia.resultadoTransferenciaDestinoPrepago = detalleOrden.Skip(3).First().observacionesExclusion + " (" + detalleOrden.Skip(3).First().resultadoRecarga + ")";
            transferencia.ResumenTransferenciaSuma = Math.Truncate(detalleOrden.First().montoRecarga).ToString();
            transferencia.ResumenTransferenciaPrepago = detalleOrden.Skip(1).First().montoRecarga.ToString("N2");
            OrdenRecargaPrepago orden = repOrden.Find(id);
            transferencia.id = orden.id;
            transferencia.creationdateOrden = orden.creationdateOrden;
            transferencia.montoOrden = orden.montoOrden;
            transferencia.statusOrden = orden.statusOrden;
            transferencia.tipoOrden = orden.tipoOrden;
            return transferencia;
        }

        public Transferencia DetalleParaOrdenAnulacionTransferencia(int orden)
        {
            Transferencia transferencia = new Transferencia();
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                //verificar que es orden de transferencia
                Order order = db.Orders.Find(orden);
                //orden no encontrada
                if (order == null)
                {
                    return null;
                }
                //orden no es de transferencia
                if (order.comments != "Orden de Transferencia")
                {
                    return null;
                }
                //verificar que la orden de transferencia este procesada
                if (order.sumastatusid != db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_ORDEN_PROCESADA) && (s.tablename == "Orders")).id)
                {
                    transferencia.tipoOrden = "No ha sido procesada";
                    return transferencia;
                }
                //verificar que esa orden no tenga anulación
                var q = (from od in db.OrdersDetails
                         where od.orderid.Equals(orden)
                         select od).ToList();
                foreach (var o in q)
                {
                    var query = (from od in db.OrdersDetails
                                 join or in db.Orders on od.orderid equals or.id
                                 where (od.comments.Equals(o.cardsresponse) || od.comments.Equals("Anulación efectiva " + o.cardsresponse))
                                 && or.comments == "Orden de Anulación de Transferencia"
                                 select od).ToList();
                    if (query.Count > 0)
                    {
                        transferencia.tipoOrden = "Ya tiene Anulación";
                        return transferencia;
                    }
                }
                //buscar detalle para crear orden de anulación
                {
                    transferencia = FindTransferencia(orden);
                    return transferencia;
                }
            }
        }

    }
}