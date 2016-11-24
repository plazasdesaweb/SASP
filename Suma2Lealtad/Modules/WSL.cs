using Newtonsoft.Json;
using Suma2Lealtad.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;

namespace Suma2Lealtad.Modules
{
    /* Capa de Acceso a Servicios Web */
    public class WSL
    {
        //servicios Cards
        private const string WSL_CARDS_ADDCLIENT = "addclient/{numdoc}/{name}/{phone}/{address}";
        private const string WSL_CARDS_ADDBATCH = "addbatch/{numdoc}/{transcode}/{monto}/{factoracred}/{factorcanje}/{usuario}";
        private const string WSL_CARDS_ADDBATCH_ANULACION = "addbatch/{numdoc}/{transcode}/{batchid}/{factoracred}/{factorcanje}/{usuario}";
        private const string WSL_CARDS_ADDTRANSFER = "addtransfer/{numdocOrigen}/{numdocDestino}/{monto}/{accounttype}/{usuario}";
        private const string WSL_CARDS_GETCLIENT = "getclient/{numdoc}";
        private const string WSL_CARDS_GETBALANCE = "getbalance/{numdoc}";
        private const string WSL_CARDS_GETBATCH = "getbatch/{accounttype}/{numdoc}";
        private const string WSL_CARDS_GETREPORT = "getreport/{fechadesde}/{fechahasta}/{numdoc}/{accounttype}/{transcode}";
        private const string WSL_CARDS_ADDCARD = "addcard/{numdoc}";
        private const string WSL_CARDS_CARD_PRINT = "card/print/{numdoc}";
        private const string WSL_CARDS_CARD_ACTIVE = "card/active/{numdoc}";
        private const string WSL_CARDS_CARD_INACTIVE = "card/inactive/{numdoc}";
        private const string WSL_CARDS_CARD_STATUS = "card/status/{numdoc}/{status}";
        //servicios WebPlazas
        private const string WSL_WEBPLAZAS_GETCLIENTBYNUMDOC = "getclientbynumdoc/{docnumber}";
        private const string WSL_WEBPLAZAS_UPDCLIENT = "updclient/{id}/{docnumber}/{nationality}/{name}/{name2}/{lastname1}/{lastname2}/{birthdate}/{gender}/{maritalstatus}/{occupation}/{phone1}/{phone2}/{phone3}/{email}/{type}";

        // retornar la respuesta en JSON.
        private static string GetResponseJSON(string uri)
        {
            HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Default);
            HttpWebRequest.DefaultCachePolicy = policy;
            WebRequest request = WebRequest.Create(uri);
            HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            request.CachePolicy = noCachePolicy;
            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }

        // retornar excepcion JSON.
        private static string GetExcepcionJSON(string ExcepcionMessage, string Source)
        {
            return "{\"code\":\"100\",\"detail\":\" @Message@\", \"source\": \" @Source@\" }".Replace("@Message@", ExcepcionMessage).Replace("@Source@", Source);
        }

        //implementa los servicios cards
        public class Cards
        {
            // consumir los servicios asociados a Cards.
            private static string ConsumirServicioCards(string nameService)
            {
                string str = AppModule.CardsServerRoute() + nameService;                                                        
                try
                {
                    return GetResponseJSON(str.Trim());
                }
                catch (Exception ex)
                {
                    return GetExcepcionJSON(ex.Message, str);
                }
            }

            //determina si hubo excepción en llamada a servicio Cards
            public static bool ExceptionServicioCards(string RespuestaServicioCards)
            {
                try
                {
                    ExceptionJSON exceptionJson = (ExceptionJSON)JsonConvert.DeserializeObject<ExceptionJSON>(RespuestaServicioCards);
                    if (exceptionJson.excode == "100" || exceptionJson.excode == "-1000")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }

            public static string addClient(string numdoc, string name, string phone, string address)
            {
                string req = WSL_CARDS_ADDCLIENT;
                req = req.Replace("{numdoc}", numdoc); 
                req = req.Replace("{name}", name);
                req = req.Replace("{phone}", phone);
                req = req.Replace("{address}", address);
                return ConsumirServicioCards(req);
            }

            public static string addBatch(string numdoc, string monto, string transcode, string usuario, string factoracred = "1", string factorcanje = "100")
            {
                string req = WSL_CARDS_ADDBATCH;
                req = req.Replace("{numdoc}", numdoc);
                req = req.Replace("{transcode}", transcode);
                req = req.Replace("{monto}", monto);
                req = req.Replace("{factoracred}", factoracred);
                req = req.Replace("{factorcanje}", factorcanje);
                req = req.Replace("{usuario}", usuario);
                return ConsumirServicioCards(req);
            }

            public static string addBatchAnulacion(string numdoc, string transcode, string batchid, string usuario, string factoracred = "1", string factorcanje = "100")
            {
                string req = WSL_CARDS_ADDBATCH_ANULACION;
                req = req.Replace("{numdoc}", numdoc);
                req = req.Replace("{transcode}", transcode);
                req = req.Replace("{batchid}", batchid);
                req = req.Replace("{factoracred}", factoracred);
                req = req.Replace("{factorcanje}", factorcanje);
                req = req.Replace("{usuario}", usuario);
                return ConsumirServicioCards(req);
            }

            public static string addTransfer(string numdocOrigen, string numdocDestino, string monto, string accounttype, string usuario)
            {
                string req = WSL_CARDS_ADDTRANSFER;
                req = req.Replace("{numdocOrigen}", numdocOrigen);
                req = req.Replace("{numdocDestino}", numdocDestino);
                req = req.Replace("{monto}", monto);
                req = req.Replace("{accounttype}", accounttype);
                req = req.Replace("{usuario}", usuario);
                return ConsumirServicioCards(req);
            }

            public static string getClient(string numdoc)
            {
                string req = WSL_CARDS_GETCLIENT;
                req = req.Replace("{numdoc}", numdoc);
                return ConsumirServicioCards(req);
            }

            public static string getBalance(string numdoc)
            {
                string req = WSL_CARDS_GETBALANCE;
                req = req.Replace("{numdoc}", numdoc);
                return ConsumirServicioCards(req);
            }

            public static string getBatch(string accounttype, string numdoc)
            {
                string req = WSL_CARDS_GETBATCH;
                req = req.Replace("{accounttype}", accounttype);
                req = req.Replace("{numdoc}", numdoc);
                return ConsumirServicioCards(req);
            }

            public static string getReport(string fechadesde, string fechahasta, string numdoc, string accounttype, string transcode)
            {
                string req = WSL_CARDS_GETREPORT;
                req = req.Replace("{fechadesde}", fechadesde);
                req = req.Replace("{fechahasta}", fechahasta);
                req = req.Replace("{numdoc}", numdoc);
                req = req.Replace("{accounttype}", accounttype);
                req = req.Replace("{transcode}", transcode);
                return ConsumirServicioCards(req);
            }

            public static string addCard(string numdoc)
            {
                string req = WSL_CARDS_ADDCARD;
                req = req.Replace("{numdoc}", numdoc);
                return ConsumirServicioCards(req);
            }

            public static string cardPrint(string numdoc)
            {
                string req = WSL_CARDS_CARD_PRINT;
                req = req.Replace("{numdoc}", numdoc);
                return ConsumirServicioCards(req);
            }

            public static string cardActive(string numdoc)
            {
                string req = WSL_CARDS_CARD_ACTIVE;
                req = req.Replace("{numdoc}", numdoc);
                return ConsumirServicioCards(req);
            }

            public static string cardInactive(string numdoc)
            {
                string req = WSL_CARDS_CARD_INACTIVE;
                req = req.Replace("{numdoc}", numdoc);
                return ConsumirServicioCards(req);
            }

            public static string cardStatus(string numdoc, string status)
            {
                string req = WSL_CARDS_CARD_STATUS;
                req = req.Replace("{numdoc}", numdoc);
                req = req.Replace("{status}", status);
                return ConsumirServicioCards(req);
            }

        }

        //implementa los servicios web
        public class WebPlazas
        {
            // consumir los servicios asociados a PlazasWeb.
            private static string ConsumirServicioPlazasWeb(string nameService)
            {
                string str = AppModule.WebServerRoute() + nameService;
                try
                {
                    return GetResponseJSON(str.Trim());
                }
                catch (Exception ex)
                {
                    return GetExcepcionJSON(ex.Message, str);
                }
            }

            //determina si hubo excepción en llamada a servicio WebPlazas
            public static bool ExceptionServicioWebPlazas(string RespuestaServicioWebPlazas)
            {
                try
                {
                    ExceptionJSON exceptionJson = (ExceptionJSON)JsonConvert.DeserializeObject<ExceptionJSON>(RespuestaServicioWebPlazas);
                    if (exceptionJson.excode == "100" || exceptionJson.excode == "-1000")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }

            public static string getClientByNumDoc(string docnumber)
            {
                string req = WSL_WEBPLAZAS_GETCLIENTBYNUMDOC;
                req = req.Replace("{docnumber}", docnumber);
                return ConsumirServicioPlazasWeb(req);
            }

            public static string UpdateClient(AfiliadoSuma record)
            {
                string req = WSL_WEBPLAZAS_UPDCLIENT;
                req = req.Replace("{id}", record.clientid.ToString());
                req = req.Replace("{docnumber}", record.docnumber);
                req = req.Replace("{nationality}", (record.nationality == null || record.nationality == 0) ? "NULO" : record.nationality.ToString());// + "");
                req = req.Replace("{name}", record.name);
                req = req.Replace("{name2}", (record.name2 == null || record.name2 == "") ? "NULO" : record.name2);
                req = req.Replace("{lastname1}", record.lastname1);
                req = req.Replace("{lastname2}", (record.lastname2 == null || record.lastname2 == "") ? "NULO" : record.lastname2);
                req = req.Replace("{birthdate}", record.birthdate.Substring(6, 4) + record.birthdate.Substring(3, 2) + record.birthdate.Substring(0, 2));// == null ? "19000101" : record.birthdate); 'yyyyMMdd'
                req = req.Replace("{gender}", record.gender.ToString());// + "");
                req = req.Replace("{maritalstatus}", record.maritalstatus.ToString());// + "");
                req = req.Replace("{occupation}", (record.occupation == null || record.occupation == "") ? "NULO" : record.occupation);// + "");
                req = req.Replace("{phone1}", record.phone1);
                req = req.Replace("{phone2}", (record.phone2 == null || record.phone2 == "") ? "NULO" : record.phone2);
                req = req.Replace("{phone3}", (record.phone3 == null || record.phone3 == "") ? "NULO" : record.phone3);
                req = req.Replace("{email}", record.email);
                req = req.Replace("{type}", record.WebType);// + "");
                return ConsumirServicioPlazasWeb(req);
            }

        }

    }
}
