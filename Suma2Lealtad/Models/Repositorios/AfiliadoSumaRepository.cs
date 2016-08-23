using Newtonsoft.Json;
using Suma2Lealtad.Modules;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Suma2Lealtad.Models
{
    public class AfiliadoSumaRepository
    {
        //retorna el ojeto Photos_Affiliate a partr del id del afiliado
        private Photos_Affiliate GetPhoto(int id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                Photos_Affiliate photo_affiliate = db.Photos_Affiliates.FirstOrDefault(p => p.Affiliate_id == id);
                if (photo_affiliate != null)
                {
                    return photo_affiliate;
                }
                else
                {
                    return null;
                }
            }
        }

        public AfiliadoSuma Find(string numdoc)
        {
            AfiliadoSuma afiliado = new AfiliadoSuma()
            {
                docnumber = numdoc
            };
            //SE CALCULA LA SUCURSAL DE AFILIACION EN FUNCION A LA DIRECCION IP DEL CLIENTE
            afiliado.storeid = DeterminarSucursalAfiliacion();
            //INCLUSION: Se determina si el número de documento ya tiene tarjeta en Cards
            //Se buscan los datos de Tarjeta del AFILIADO en Cards
            //SERVICIO WSL.Cards.getClient !
            string clienteCardsJson = WSL.Cards.getClient(numdoc.Substring(2));
            ClienteCards clienteCards;
            if (WSL.Cards.ExceptionServicioCards(clienteCardsJson))
            {
                //return null;
                clienteCards = null;
            }
            else
            {
                clienteCards = (ClienteCards)JsonConvert.DeserializeObject<ClienteCards>(clienteCardsJson);
            }
            if (clienteCards == null)
            {
                //No está en Cards o no se pudo leer desde Cards
                afiliado.pan = "0";
            }
            else
            {
                afiliado.pan = clienteCards.pan;
                afiliado.estatustarjeta = clienteCards.tarjeta;
            }
            //Primero se buscan los datos de CLIENTE en WebPlazas
            //SERVICIO WSL.WebPlazas.getClientByNumDoc 
            string clienteWebPlazasJson = WSL.WebPlazas.getClientByNumDoc(afiliado.docnumber);
            ClienteWebPlazas clienteWebPlazas;
            if (WSL.WebPlazas.ExceptionServicioWebPlazas(clienteWebPlazasJson))
            {
                //return null;
                clienteWebPlazas = null;
            }
            else
            {
                if (clienteWebPlazasJson.Contains("\"id\":null"))
                {
                    //no se pudo leer desde la web
                    clienteWebPlazas = null;
                }
                else
                {
                    clienteWebPlazas = (ClienteWebPlazas)JsonConvert.DeserializeObject<ClienteWebPlazas>(clienteWebPlazasJson);
                }
            }
            if (clienteWebPlazas == null)
            {
                //No está en WebPlazas ó no se pudo leer desde la web
                afiliado.clientid = 0;
                afiliado.ListaEstados = GetEstados();
            }
            else
            {
                //Si está en la WebPlazas
                //afiliado.nationality = clienteWebPlazas.nationality.Replace("/", "").Replace("\\", "");
                if (afiliado.docnumber.Substring(0, 1).ToUpper() == "V")
                {
                    afiliado.nationality = "1";
                }
                else if (afiliado.docnumber.Substring(0, 1).ToUpper() == "E" || afiliado.docnumber.Substring(0, 1).ToUpper() == "P")
                {
                    afiliado.nationality = "2";
                }
                else
                {
                    afiliado.nationality = "0";
                }
                afiliado.name = clienteWebPlazas.name.Replace("/", "").Replace("\\", "");
                afiliado.name2 = clienteWebPlazas.name2.Replace("/", "").Replace("\\", "");
                afiliado.lastname1 = clienteWebPlazas.lastname1.Replace("/", "").Replace("\\", "");
                afiliado.lastname2 = clienteWebPlazas.lastname2.Replace("/", "").Replace("\\", "");
                afiliado.birthdate = clienteWebPlazas.birthdate.Value.ToString("dd/MM/yyyy");
                afiliado.gender = clienteWebPlazas.gender.Replace("/", "").Replace("\\", "");
                afiliado.clientid = clienteWebPlazas.id;
                afiliado.maritalstatus = clienteWebPlazas.maritalstatus.Replace("/", "").Replace("\\", "");
                afiliado.occupation = clienteWebPlazas.occupation.Replace("/", "").Replace("\\", "").Length > 30 ? clienteWebPlazas.occupation.Replace("/", "").Replace("\\", "").Substring(0, 30) : clienteWebPlazas.occupation.Replace("/", "").Replace("\\", "");
                afiliado.phone1 = clienteWebPlazas.phone1.Replace("/", "").Replace("\\", "");
                afiliado.phone2 = clienteWebPlazas.phone2.Replace("/", "").Replace("\\", "");
                afiliado.phone3 = clienteWebPlazas.phone3.Replace("/", "").Replace("\\", "");
                afiliado.email = clienteWebPlazas.email.Replace("/", "").Replace("\\", "");
                afiliado.WebType = clienteWebPlazas.type;
                afiliado.ListaEstados = GetEstados();
            }
            //Segundo se buscan los datos del AFILIADO en SumaPlazas
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                //Entidad Affiliate                
                afiliado.id = (from a in db.Affiliates
                               where a.docnumber.Equals(afiliado.docnumber)
                               select a.id
                               ).SingleOrDefault();
                if (afiliado.id == 0)
                {
                    //No está en SumaPlazas
                    afiliado.Intereses = chargeInterestList();
                }
                else
                {
                    //Esta en SumaPlazas
                    //ENTIDAD TYPE 
                    afiliado.typeid = db.Affiliates.FirstOrDefault(a => a.id == afiliado.id).typeid;
                    afiliado.type = db.Types.FirstOrDefault(t => t.id == afiliado.typeid).name;
                    afiliado.sumastatusid = db.Affiliates.FirstOrDefault(a => a.docnumber == afiliado.docnumber).sumastatusid.Value;
                    afiliado.estatus = db.SumaStatuses.FirstOrDefault(s => s.id == afiliado.sumastatusid).name;
                    afiliado.Intereses = chargeInterestList(afiliado.id);
                }
            }
            return afiliado;
        }

        public List<AfiliadoSumaIndex> Find(string numdoc, string name, string email, string estadoAfiliacion, string estadoTarjeta)
        {
            List<AfiliadoSumaIndex> afiliados = new List<AfiliadoSumaIndex>();
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                if (numdoc == "")
                {
                    numdoc = null;
                }
                if (name == "")
                {
                    name = null;
                }
                if (email == "")
                {
                    email = null;
                }
                if (estadoAfiliacion == "")
                {
                    estadoAfiliacion = null;
                }
                if (estadoTarjeta == "")
                {
                    estadoTarjeta = null;
                }
                //BUSCAR POR ESTADO DE TARJETA
                if (estadoTarjeta != null)
                {
                    var query = (from t in db.TARJETAS
                                 where t.ESTATUS_TARJETA.Equals(estadoTarjeta)
                                 join a in db.Affiliates on t.NRO_AFILIACION equals a.id
                                 join c in db.CLIENTES on a.docnumber equals c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO
                                 select new
                                 {
                                     pan = t.NRO_TARJETA,
                                     estatustarjeta = t.ESTATUS_TARJETA,
                                     id = t.NRO_AFILIACION,
                                     docnumber = a.docnumber,
                                     typeid = a.typeid,
                                     sumastatusid = a.sumastatusid,
                                     name = c.NOMBRE_CLIENTE1,
                                     lastname1 = c.APELLIDO_CLIENTE1,
                                     email = c.E_MAIL
                                 }).OrderBy(d => d.docnumber);
                    afiliados = (from q in query.AsEnumerable()
                                 join s in db.SumaStatuses on q.sumastatusid equals s.id
                                 join ty in db.Types on q.typeid equals ty.id
                                 select new AfiliadoSumaIndex()
                                 {
                                     pan = q.pan.ToString(),
                                     estatustarjeta = q.estatustarjeta,
                                     id = q.id,
                                     docnumber = q.docnumber,
                                     typeid = q.typeid,
                                     sumastatusid = q.sumastatusid.Value,
                                     name = q.name,
                                     lastname1 = q.lastname1,
                                     email = q.email,
                                     estatus = s.name,
                                     type = ty.name
                                 }).ToList();
                }
                //BUSCAR POR ESTADO DE AFILIACION
                else if (estadoAfiliacion != null)
                {
                    var query = (from a in db.Affiliates
                                 where a.SumaStatu.name.Equals(estadoAfiliacion)
                                 join c in db.CLIENTES on a.docnumber equals c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO
                                 join t in db.TARJETAS on a.id equals t.NRO_AFILIACION into PRUEBA
                                 from prue in PRUEBA.DefaultIfEmpty()
                                 select new
                                 {
                                     pan = prue == null ? new decimal() : prue.NRO_TARJETA,
                                     estatustarjeta = prue == null ? "" : prue.ESTATUS_TARJETA,
                                     id = a.id,
                                     docnumber = a.docnumber,
                                     typeid = a.typeid,
                                     sumastatusid = a.sumastatusid,
                                     name = c.NOMBRE_CLIENTE1,
                                     lastname1 = c.APELLIDO_CLIENTE1,
                                     email = c.E_MAIL
                                 }).OrderBy(d => d.docnumber);
                    afiliados = (from q in query.AsEnumerable()
                                 join s in db.SumaStatuses on q.sumastatusid equals s.id
                                 join ty in db.Types on q.typeid equals ty.id
                                 select new AfiliadoSumaIndex()
                                 {
                                     pan = q.pan == 0 ? "" : q.pan.ToString(),
                                     estatustarjeta = q.estatustarjeta,
                                     id = q.id,
                                     docnumber = q.docnumber,
                                     typeid = q.typeid,
                                     sumastatusid = q.sumastatusid.Value,
                                     name = q.name,
                                     lastname1 = q.lastname1,
                                     email = q.email,
                                     estatus = s.name,
                                     type = ty.name
                                 }).ToList();
                }
                //BUSCAR POR NUMERO DE DOCUMENTO
                else if (numdoc != null)
                {
                    var query = (from a in db.Affiliates
                                 where a.docnumber.Equals(numdoc)
                                 join c in db.CLIENTES on a.docnumber equals c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO
                                 join t in db.TARJETAS on a.id equals t.NRO_AFILIACION into PRUEBA
                                 from prue in PRUEBA.DefaultIfEmpty()
                                 select new
                                 {
                                     pan = prue == null ? new decimal() : prue.NRO_TARJETA,
                                     estatustarjeta = prue == null ? "" : prue.ESTATUS_TARJETA,
                                     id = a.id,
                                     docnumber = a.docnumber,
                                     typeid = a.typeid,
                                     sumastatusid = a.sumastatusid,
                                     name = c.NOMBRE_CLIENTE1,
                                     lastname1 = c.APELLIDO_CLIENTE1,
                                     email = c.E_MAIL
                                 }).OrderBy(d => d.docnumber);
                    afiliados = (from q in query.AsEnumerable()
                                 join s in db.SumaStatuses on q.sumastatusid equals s.id
                                 join ty in db.Types on q.typeid equals ty.id
                                 select new AfiliadoSumaIndex()
                                 {
                                     pan = q.pan == 0 ? "" : q.pan.ToString(),
                                     estatustarjeta = q.estatustarjeta,
                                     id = q.id,
                                     docnumber = q.docnumber,
                                     typeid = q.typeid,
                                     sumastatusid = q.sumastatusid.Value,
                                     name = q.name,
                                     lastname1 = q.lastname1,
                                     email = q.email,
                                     estatus = s.name,
                                     type = ty.name
                                 }).ToList();
                }
                //BUSCAR POR NOMBRE O CORREO
                else if (name != null || email != null)
                {
                    var query = (from a in db.Affiliates
                                 join c in db.CLIENTES on a.docnumber equals c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO
                                 where (c.NOMBRE_CLIENTE1.Contains(name) || c.APELLIDO_CLIENTE1.Contains(name) || c.E_MAIL.Equals(email))
                                 join t in db.TARJETAS on a.id equals t.NRO_AFILIACION into PRUEBA
                                 from prue in PRUEBA.DefaultIfEmpty()
                                 select new
                                 {
                                     pan = prue == null ? new decimal() : prue.NRO_TARJETA,
                                     estatustarjeta = prue == null ? "" : prue.ESTATUS_TARJETA,
                                     id = a.id,
                                     docnumber = a.docnumber,
                                     typeid = a.typeid,
                                     sumastatusid = a.sumastatusid,
                                     name = c.NOMBRE_CLIENTE1,
                                     lastname1 = c.APELLIDO_CLIENTE1,
                                     email = c.E_MAIL
                                 }).OrderBy(d => d.docnumber);
                    afiliados = (from q in query.AsEnumerable()
                                 join s in db.SumaStatuses on q.sumastatusid equals s.id
                                 join ty in db.Types on q.typeid equals ty.id
                                 select new AfiliadoSumaIndex()
                                 {
                                     pan = q.pan == 0 ? "" : q.pan.ToString(),
                                     estatustarjeta = q.estatustarjeta,
                                     id = q.id,
                                     docnumber = q.docnumber,
                                     typeid = q.typeid,
                                     sumastatusid = q.sumastatusid.Value,
                                     name = q.name,
                                     lastname1 = q.lastname1,
                                     email = q.email,
                                     estatus = s.name,
                                     type = ty.name
                                 }).ToList();
                }
                //BUSCAR TODOS
                else if (numdoc == null && name == null && email == null && estadoAfiliacion == null && estadoTarjeta == null)
                {
                    var query = (from a in db.Affiliates
                                 join c in db.CLIENTES on a.docnumber equals c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO
                                 join t in db.TARJETAS on a.id equals t.NRO_AFILIACION into PRUEBA
                                 from prue in PRUEBA.DefaultIfEmpty()
                                 select new
                                 {
                                     pan = prue == null ? new decimal() : prue.NRO_TARJETA,
                                     estatustarjeta = prue == null ? "" : prue.ESTATUS_TARJETA,
                                     id = a.id,
                                     docnumber = a.docnumber,
                                     typeid = a.typeid,
                                     sumastatusid = a.sumastatusid,
                                     name = c.NOMBRE_CLIENTE1,
                                     lastname1 = c.APELLIDO_CLIENTE1,
                                     email = c.E_MAIL
                                 }).OrderBy(d => d.docnumber);
                    afiliados = (from q in query.AsEnumerable()
                                 join s in db.SumaStatuses on q.sumastatusid equals s.id
                                 join ty in db.Types on q.typeid equals ty.id
                                 select new AfiliadoSumaIndex()
                                 {
                                     pan = q.pan == 0 ? "" : q.pan.ToString(),
                                     estatustarjeta = q.estatustarjeta,
                                     id = q.id,
                                     docnumber = q.docnumber,
                                     typeid = q.typeid,
                                     sumastatusid = q.sumastatusid.Value,
                                     name = q.name,
                                     lastname1 = q.lastname1,
                                     email = q.email,
                                     estatus = s.name,
                                     type = ty.name
                                 }).ToList();
                }
            }
            return afiliados;
        }

        public AfiliadoSuma Find(int id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                AfiliadoSuma afiliado = (from a in db.Affiliates
                                         join c in db.CLIENTES on a.docnumber equals c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO
                                         join s in db.SumaStatuses on a.sumastatusid equals s.id
                                         join t in db.Types on a.typeid equals t.id
                                         where a.id.Equals(id)
                                         select new AfiliadoSuma()
                                         {
                                             //ENTIDAD Affiliate 
                                             id = a.id,
                                             customerid = a.customerid,
                                             docnumber = a.docnumber,
                                             clientid = a.clientid,
                                             storeid = a.storeid,
                                             channelid = a.channelid,
                                             typeid = a.typeid,
                                             typedelivery = a.typedelivery,
                                             storeiddelivery = a.storeiddelivery,
                                             sumastatusid = a.sumastatusid.Value,
                                             reasonsid = a.reasonsid,
                                             twitter_account = a.twitter_account,
                                             facebook_account = a.facebook_account,
                                             instagram_account = a.instagram_account,
                                             comments = a.comments,
                                             //ENTIDAD CLIENTE
                                             nationality = c.NACIONALIDAD,
                                             name = c.NOMBRE_CLIENTE1,
                                             name2 = c.NOMBRE_CLIENTE2,
                                             lastname1 = c.APELLIDO_CLIENTE1,
                                             lastname2 = c.APELLIDO_CLIENTE2,
                                             gender = c.SEXO,
                                             maritalstatus = c.EDO_CIVIL,
                                             occupation = c.OCUPACION,
                                             phone1 = c.TELEFONO_HAB,
                                             phone2 = c.TELEFONO_OFIC,
                                             phone3 = c.TELEFONO_CEL,
                                             email = c.E_MAIL,
                                             cod_estado = c.COD_ESTADO,
                                             cod_ciudad = c.COD_CIUDAD,
                                             cod_municipio = c.COD_MUNICIPIO,
                                             cod_parroquia = c.COD_PARROQUIA,
                                             cod_urbanizacion = c.COD_URBANIZACION,
                                             //ENTIDAD SumaStatuses
                                             estatus = s.name,
                                             //ENTIDAD Type
                                             type = t.name,
                                         }).SingleOrDefault();
                if (afiliado != null)
                {
                    DateTime? d = (from c in db.CLIENTES
                                   where (c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO).Equals(afiliado.docnumber)
                                   select c.FECHA_NACIMIENTO
                                   ).SingleOrDefault();
                    if (d == null)
                    {
                        afiliado.birthdate = null;
                    }
                    else
                    {
                        afiliado.birthdate = d.Value.ToString("dd/MM/yyyy");
                    }
                    //ENTIDAD CustomerInterest
                    afiliado.Intereses = chargeInterestList(afiliado.id);
                    //Llenar las listas de Datos Geográficos.
                    afiliado.ListaEstados = GetEstados();
                    afiliado.ListaCiudades = GetCiudades(afiliado.cod_estado);
                    afiliado.ListaMunicipios = GetMunicipios(afiliado.cod_ciudad);
                    afiliado.ListaParroquias = GetParroquias(afiliado.cod_municipio);
                    afiliado.ListaUrbanizaciones = GetUrbanizaciones(afiliado.cod_parroquia);
                    //ENTIDAD TARJETA
                    if (afiliado.estatus != "Nueva")
                    {
                        Decimal p = (from t in db.TARJETAS
                                     where t.NRO_AFILIACION.Equals(afiliado.id)
                                     select t.NRO_TARJETA
                                     ).SingleOrDefault();
                        if (p != 0)
                        {
                            afiliado.pan = p.ToString();
                        }
                        else
                        {
                            afiliado.pan = "";
                        }
                        string e = (from t in db.TARJETAS
                                    where t.NRO_AFILIACION.Equals(afiliado.id)
                                    select t.ESTATUS_TARJETA
                                    ).SingleOrDefault();
                        if (e != null)
                        {
                            afiliado.estatustarjeta = e.ToString();
                        }
                        else
                        {
                            afiliado.estatustarjeta = "";
                        }
                        string v = (from t in db.TARJETAS
                                    where t.NRO_AFILIACION.Equals(afiliado.id)
                                    select t.CVV2
                                    ).SingleOrDefault();
                        if (v != null)
                        {
                            afiliado.cvv2 = v;
                        }
                        else
                        {
                            afiliado.cvv2 = "";
                        }
                        afiliado.trackI = Tarjeta.ConstruirTrackI(afiliado.pan);
                        afiliado.trackII = (from t in db.TARJETAS
                                            where t.NRO_AFILIACION.Equals(afiliado.id)
                                            select t.TRACK2
                                            ).SingleOrDefault();
                    }
                    //ENTIDAD Photos_Affiliate 
                    afiliado.picture = GetPhoto(afiliado.id);
                    //POR AHORA NO HAY COLUMNA EN NINGUNA ENTIDAD PARA ALMACENAR ESTE DATO QUE VIENE DE LA WEB
                    if (afiliado.WebType == null)
                    {
                        afiliado.WebType = Globals.WEB_TYPE;
                    }
                    //TEMPORAL CARGAR FECHA Y USUARIO DE AFILIACION
                    afiliado.fechaAfiliacion = db.Affiliates.FirstOrDefault(x => x.id == afiliado.id).creationdate;
                    afiliado.usuarioAfiliacion = (from a in db.Affiliates
                                                  join u in db.Users on a.creationuserid equals u.id
                                                  where a.id == afiliado.id
                                                  select u.firstname + " " + u.lastname + " (" + u.login + ")"
                                                  ).SingleOrDefault();
                }
                return afiliado;
            }
        }

        public List<AfiliadoSumaExcel> FindAllExcel()
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                AfiliadoSuma datos = new AfiliadoSuma();
                List<AfiliadoSumaExcel> afiliados = new List<AfiliadoSumaExcel>();
                var query = (from a in db.Affiliates
                             join c in db.CLIENTES on a.docnumber equals c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO
                             join t in db.TARJETAS on a.id equals t.NRO_AFILIACION into PRUEBA
                             from prue in PRUEBA.DefaultIfEmpty()
                             select new
                             {
                                 //tarjeta
                                 pan = prue == null ? new decimal() : prue.NRO_TARJETA,
                                 estatustarjeta = prue == null ? "" : prue.ESTATUS_TARJETA,
                                 //affiliate
                                 id = a.id,
                                 docnumber = a.docnumber,
                                 typeid = a.typeid,
                                 sumastatusid = a.sumastatusid,
                                 customerid = a.customerid,
                                 clientid = a.clientid,
                                 storeid = a.storeid,
                                 channelid = a.channelid,
                                 typedelivery = a.typedelivery,
                                 storeiddelivery = a.storeiddelivery,
                                 reasonsid = a.reasonsid,
                                 twitter_account = a.twitter_account,
                                 facebook_account = a.facebook_account,
                                 instagram_account = a.instagram_account,
                                 comments = a.comments,
                                 //cliente
                                 name = c.NOMBRE_CLIENTE1,
                                 name2 = c.NOMBRE_CLIENTE2,
                                 lastname1 = c.APELLIDO_CLIENTE1,
                                 lastname2 = c.APELLIDO_CLIENTE2,
                                 fecha_nacimiento = c.FECHA_NACIMIENTO,
                                 email = c.E_MAIL,
                                 nationality = c.NACIONALIDAD,
                                 gender = c.SEXO,
                                 maritalstatus = c.EDO_CIVIL,
                                 occupation = c.OCUPACION,
                                 phone1 = c.TELEFONO_HAB,
                                 phone2 = c.TELEFONO_OFIC,
                                 phone3 = c.TELEFONO_CEL,
                                 cod_estado = c.COD_ESTADO,
                                 cod_ciudad = c.COD_CIUDAD,
                                 cod_municipio = c.COD_MUNICIPIO,
                                 cod_parroquia = c.COD_PARROQUIA,
                                 cod_urbanizacion = c.COD_URBANIZACION,
                                 fechaAfiliacion = a.creationdate,
                                 usuarioAfiliacion = a.creationuserid
                             }).OrderBy(d => d.docnumber);
                var query2 = (from q in query.AsEnumerable()
                              join s in db.SumaStatuses on q.sumastatusid equals s.id
                              join ty in db.Types on q.typeid equals ty.id
                              select new //AfiliadoSumaExcel()
                              {
                                  id = q.id,
                                  clientid = q.clientid,
                                  estatus = s.name,
                                  type = ty.name,
                                  docnumber = q.docnumber,
                                  nationality = datos.NacionalidadOptions.FirstOrDefault(x => x.id.ToString() == q.nationality).nacionalidad,
                                  storeid = datos.StoreOptions.First(x => x.id == q.storeid.ToString()).sucursal,
                                  channelid = datos.ChannelOptions.First(x => x.id == q.channelid).channel,
                                  typedelivery = q.typedelivery == null ? "" : datos.TypeDeliveryOptions.First(x => x.id.ToString() == q.typedelivery).delivery,
                                  storeiddelivery = q.storeiddelivery == null ? "" : datos.StoreOptions.FirstOrDefault(x => x.id == q.storeiddelivery.Value.ToString()).sucursal,
                                  name = q.name,
                                  name2 = q.name2,
                                  lastname1 = q.lastname1,
                                  lastname2 = q.lastname2,
                                  birthdate = q.fecha_nacimiento == null ? "" : q.fecha_nacimiento.Value.ToString(),
                                  gender = datos.SexoOptions.FirstOrDefault(x => x.id.ToString() == q.gender).sexo,
                                  edad = q.fecha_nacimiento == null ? 0 : ((DateTime.Now.Subtract(q.fecha_nacimiento.Value)).Days) / 365,
                                  maritalstatus = datos.MaritalStatusOptions.FirstOrDefault(x => x.id.ToString() == q.maritalstatus).maritalstatus,
                                  occupation = q.occupation,
                                  email = q.email,
                                  phone1 = q.phone1,
                                  phone2 = q.phone2,
                                  phone3 = q.phone3,
                                  twitter_account = q.twitter_account == null ? "" : q.twitter_account,
                                  facebook_account = q.facebook_account == null ? "" : q.facebook_account,
                                  instagram_account = q.instagram_account == null ? "" : q.instagram_account,
                                  cod_estado = q.cod_estado,
                                  cod_ciudad = q.cod_ciudad,
                                  cod_municipio = q.cod_municipio,
                                  cod_parroquia = q.cod_parroquia,
                                  cod_urbanizacion = q.cod_urbanizacion,
                                  pan = q.pan == 0 ? "" : q.pan.ToString(),
                                  estatustarjeta = q.estatustarjeta,
                                  fechaAfiliacion = q.fechaAfiliacion.ToString(),
                                  usuarioAfiliacion = q.usuarioAfiliacion
                              }).ToList();
                var query3 = (from q in query2.AsEnumerable()
                              join u in db.Users on q.usuarioAfiliacion equals u.id
                              join e in db.ESTADOS on q.cod_estado equals e.COD_ESTADO into estados
                              from estado in estados.DefaultIfEmpty()
                              select new
                              {
                                  id = q.id,
                                  clientid = q.clientid,
                                  estatus = q.estatus,
                                  type = q.type,
                                  docnumber = q.docnumber,
                                  nationality = q.nationality,
                                  storeid = q.storeid,
                                  channelid = q.channelid,
                                  typedelivery = q.typedelivery,
                                  storeiddelivery = q.storeiddelivery,
                                  name = q.name,
                                  name2 = q.name2,
                                  lastname1 = q.lastname1,
                                  lastname2 = q.lastname2,
                                  birthdate = q.birthdate,
                                  gender = q.gender,
                                  edad = q.edad,
                                  maritalstatus = q.maritalstatus,
                                  occupation = q.occupation,
                                  email = q.email,
                                  phone1 = q.phone1,
                                  phone2 = q.phone2,
                                  phone3 = q.phone3,
                                  twitter_account = q.twitter_account,
                                  facebook_account = q.facebook_account,
                                  instagram_account = q.instagram_account,
                                  cod_estado = estado == null ? "" : estado.DESCRIPC_ESTADO,
                                  cod_ciudad = q.cod_ciudad,
                                  cod_municipio = q.cod_municipio,
                                  cod_parroquia = q.cod_parroquia,
                                  cod_urbanizacion = q.cod_urbanizacion,
                                  pan = q.pan,
                                  estatustarjeta = q.estatustarjeta,
                                  fechaAfiliacion = q.fechaAfiliacion,
                                  usuarioAfiliacion = u.login
                              }).ToList();
                var query4 = (from q in query3.AsEnumerable()
                              join c in db.CIUDADES on q.cod_ciudad equals c.COD_CIUDAD into ciudades
                              from ciudad in ciudades.DefaultIfEmpty()
                              select new
                              {
                                  id = q.id,
                                  clientid = q.clientid,
                                  estatus = q.estatus,
                                  type = q.type,
                                  docnumber = q.docnumber,
                                  nationality = q.nationality,
                                  storeid = q.storeid,
                                  channelid = q.channelid,
                                  typedelivery = q.typedelivery,
                                  storeiddelivery = q.storeiddelivery,
                                  name = q.name,
                                  name2 = q.name2,
                                  lastname1 = q.lastname1,
                                  lastname2 = q.lastname2,
                                  birthdate = q.birthdate,
                                  gender = q.gender,
                                  edad = q.edad,
                                  maritalstatus = q.maritalstatus,
                                  occupation = q.occupation,
                                  email = q.email,
                                  phone1 = q.phone1,
                                  phone2 = q.phone2,
                                  phone3 = q.phone3,
                                  twitter_account = q.twitter_account,
                                  facebook_account = q.facebook_account,
                                  instagram_account = q.instagram_account,
                                  cod_estado = q.cod_estado,
                                  cod_ciudad = ciudad == null ? "" : ciudad.DESCRIPC_CIUDAD,
                                  cod_municipio = q.cod_municipio,
                                  cod_parroquia = q.cod_parroquia,
                                  cod_urbanizacion = q.cod_urbanizacion,
                                  pan = q.pan,
                                  estatustarjeta = q.estatustarjeta,
                                  fechaAfiliacion = q.fechaAfiliacion,
                                  usuarioAfiliacion = q.usuarioAfiliacion
                              }).ToList();
                var query5 = (from q in query4.AsEnumerable()
                              join m in db.MUNICIPIOS on q.cod_municipio equals m.COD_MUNICIPIO into municipios
                              from municipio in municipios.DefaultIfEmpty()
                              select new
                              {
                                  id = q.id,
                                  clientid = q.clientid,
                                  estatus = q.estatus,
                                  type = q.type,
                                  docnumber = q.docnumber,
                                  nationality = q.nationality,
                                  storeid = q.storeid,
                                  channelid = q.channelid,
                                  typedelivery = q.typedelivery,
                                  storeiddelivery = q.storeiddelivery,
                                  name = q.name,
                                  name2 = q.name2,
                                  lastname1 = q.lastname1,
                                  lastname2 = q.lastname2,
                                  birthdate = q.birthdate,
                                  gender = q.gender,
                                  edad = q.edad,
                                  maritalstatus = q.maritalstatus,
                                  occupation = q.occupation,
                                  email = q.email,
                                  phone1 = q.phone1,
                                  phone2 = q.phone2,
                                  phone3 = q.phone3,
                                  twitter_account = q.twitter_account,
                                  facebook_account = q.facebook_account,
                                  instagram_account = q.instagram_account,
                                  cod_estado = q.cod_estado,
                                  cod_ciudad = q.cod_ciudad,
                                  cod_municipio = municipio == null ? "" : municipio.DESCRIPC_MUNICIPIO,
                                  cod_parroquia = q.cod_parroquia,
                                  cod_urbanizacion = q.cod_urbanizacion,
                                  pan = q.pan,
                                  estatustarjeta = q.estatustarjeta,
                                  fechaAfiliacion = q.fechaAfiliacion,
                                  usuarioAfiliacion = q.usuarioAfiliacion
                              }).ToList();
                var query6 = (from q in query5.AsEnumerable()
                              join p in db.PARROQUIAS on q.cod_parroquia equals p.COD_PARROQUIA into parroquias
                              from parroquia in parroquias.DefaultIfEmpty()
                              select new
                              {
                                  id = q.id,
                                  clientid = q.clientid,
                                  estatus = q.estatus,
                                  type = q.type,
                                  docnumber = q.docnumber,
                                  nationality = q.nationality,
                                  storeid = q.storeid,
                                  channelid = q.channelid,
                                  typedelivery = q.typedelivery,
                                  storeiddelivery = q.storeiddelivery,
                                  name = q.name,
                                  name2 = q.name2,
                                  lastname1 = q.lastname1,
                                  lastname2 = q.lastname2,
                                  birthdate = q.birthdate,
                                  gender = q.gender,
                                  edad = q.edad,
                                  maritalstatus = q.maritalstatus,
                                  occupation = q.occupation,
                                  email = q.email,
                                  phone1 = q.phone1,
                                  phone2 = q.phone2,
                                  phone3 = q.phone3,
                                  twitter_account = q.twitter_account,
                                  facebook_account = q.facebook_account,
                                  instagram_account = q.instagram_account,
                                  cod_estado = q.cod_estado,
                                  cod_ciudad = q.cod_ciudad,
                                  cod_municipio = q.cod_municipio,
                                  cod_parroquia = parroquia == null ? "" : parroquia.DESCRIPC_PARROQUIA,
                                  cod_urbanizacion = q.cod_urbanizacion,
                                  pan = q.pan,
                                  estatustarjeta = q.estatustarjeta,
                                  fechaAfiliacion = q.fechaAfiliacion,
                                  usuarioAfiliacion = q.usuarioAfiliacion
                              }).ToList();
                afiliados = (from q in query6.AsEnumerable()
                             join u in db.URBANIZACIONES on q.cod_urbanizacion equals u.COD_URBANIZACION into urbanizaciones
                             from urbanizacion in urbanizaciones.DefaultIfEmpty()
                             select new AfiliadoSumaExcel
                             {
                                 id = q.id,
                                 clientid = q.clientid,
                                 estatus = q.estatus,
                                 type = q.type,
                                 docnumber = q.docnumber,
                                 nationality = q.nationality,
                                 storeid = q.storeid,
                                 channelid = q.channelid,
                                 typedelivery = q.typedelivery,
                                 storeiddelivery = q.storeiddelivery,
                                 name = q.name,
                                 name2 = q.name2,
                                 lastname1 = q.lastname1,
                                 lastname2 = q.lastname2,
                                 birthdate = q.birthdate,
                                 gender = q.gender,
                                 edad = q.edad,
                                 maritalstatus = q.maritalstatus,
                                 occupation = q.occupation,
                                 email = q.email,
                                 phone1 = q.phone1,
                                 phone2 = q.phone2,
                                 phone3 = q.phone3,
                                 twitter_account = q.twitter_account,
                                 facebook_account = q.facebook_account,
                                 instagram_account = q.instagram_account,
                                 cod_estado = q.cod_estado,
                                 cod_ciudad = q.cod_ciudad,
                                 cod_municipio = q.cod_municipio,
                                 cod_parroquia = q.cod_parroquia,
                                 cod_urbanizacion = urbanizacion == null ? "" : urbanizacion.DESCRIPC_URBANIZACION,
                                 pan = q.pan,
                                 estatustarjeta = q.estatustarjeta,
                                 fechaAfiliacion = q.fechaAfiliacion,
                                 usuarioAfiliacion = q.usuarioAfiliacion
                             }).ToList();
                return afiliados;
            }
        }

        //YA NO SE ENVIARÁ INFORMACIÓN A LA WEB
        //private bool SaveWebPlazas(AfiliadoSuma afiliado)
        //{
        //    string RespuestaWebPlazasJson = WSL.WebPlazas.UpdateClient(afiliado);
        //    if (ExceptionServicioWebPlazas(RespuestaWebPlazasJson))
        //    {
        //        return false;
        //    }
        //    RespuestaWebPlazas RespuestaWebPlazas = (RespuestaWebPlazas)JsonConvert.DeserializeObject<RespuestaWebPlazas>(RespuestaWebPlazasJson);
        //    return (RespuestaWebPlazas.excode == "0");
        //}

        public bool Save(AfiliadoSuma afiliado, HttpPostedFileBase file)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                //ENTIDAD Affiliatte                   
                var Affiliate = new Affiliate()
                {
                    id = AfilliatesID(),
                    customerid = afiliado.customerid,
                    docnumber = afiliado.docnumber,
                    clientid = afiliado.clientid,
                    storeid = afiliado.storeid,
                    channelid = afiliado.channelid,
                    typeid = afiliado.typeid,
                    affiliatedate = System.DateTime.Now,
                    typedelivery = afiliado.typedelivery,
                    storeiddelivery = afiliado.storeiddelivery,
                    estimateddatedelivery = new DateTime(),
                    creationdate = DateTime.Now,
                    creationuserid = (int)HttpContext.Current.Session["userid"],
                    modifieddate = DateTime.Now,
                    modifieduserid = (int)HttpContext.Current.Session["userid"],
                    sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_AFILIACION_INICIAL) && (s.tablename == "Affiliatte")).id,
                    reasonsid = null,
                    twitter_account = afiliado.twitter_account,
                    facebook_account = afiliado.facebook_account,
                    instagram_account = afiliado.instagram_account,
                    comments = afiliado.comments
                };
                db.Affiliates.Add(Affiliate);
                //ENTIDAD CLIENTE
                CLIENTE cliente = db.CLIENTES.FirstOrDefault(c => c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO == afiliado.docnumber);
                if (cliente == null)
                {
                    var CLIENTE = new CLIENTE()
                    {
                        TIPO_DOCUMENTO = afiliado.docnumber.Substring(0, 1),
                        NRO_DOCUMENTO = afiliado.docnumber.Substring(2),
                        E_MAIL = afiliado.email,
                        NACIONALIDAD = afiliado.nationality == null ? "" : afiliado.nationality,
                        NOMBRE_CLIENTE1 = afiliado.name,
                        NOMBRE_CLIENTE2 = afiliado.name2 == null ? "" : afiliado.name2,
                        APELLIDO_CLIENTE1 = afiliado.lastname1 == null ? "" : afiliado.lastname1,
                        APELLIDO_CLIENTE2 = afiliado.lastname2 == null ? "" : afiliado.lastname2,
                        FECHA_NACIMIENTO = afiliado.birthdate == null ? new DateTime?() : DateTime.ParseExact(afiliado.birthdate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        SEXO = afiliado.gender == null ? "" : afiliado.gender,
                        EDO_CIVIL = afiliado.maritalstatus == null ? "" : afiliado.maritalstatus,
                        //OCUPACION = afiliado.occupation == null ? "" : afiliado.occupation.Substring(0, 30),
                        TELEFONO_HAB = afiliado.phone1,
                        TELEFONO_OFIC = afiliado.phone2 == null ? "" : afiliado.phone2,
                        TELEFONO_CEL = afiliado.phone3 == null ? "" : afiliado.phone3,
                        COD_SUCURSAL = afiliado.storeid,
                        COD_ESTADO = afiliado.cod_estado,
                        COD_CIUDAD = afiliado.cod_ciudad,
                        COD_MUNICIPIO = afiliado.cod_municipio,
                        COD_PARROQUIA = afiliado.cod_parroquia,
                        COD_URBANIZACION = afiliado.cod_urbanizacion,
                        FECHA_CREACION = DateTime.Now
                    };
                    if (afiliado.occupation == null)
                    {
                        CLIENTE.OCUPACION = afiliado.occupation;
                    }
                    else if (afiliado.occupation.Length > 30)
                    {
                        CLIENTE.OCUPACION = afiliado.occupation.Substring(0, 30);
                    }
                    else
                    {
                        CLIENTE.OCUPACION = afiliado.occupation;
                    }
                    db.CLIENTES.Add(CLIENTE);
                }
                else
                {
                    cliente.E_MAIL = afiliado.email;
                    cliente.NACIONALIDAD = afiliado.nationality == null ? "" : afiliado.nationality;
                    cliente.NOMBRE_CLIENTE1 = afiliado.name;
                    cliente.NOMBRE_CLIENTE2 = afiliado.name2 == null ? "" : afiliado.name2;
                    cliente.APELLIDO_CLIENTE1 = afiliado.lastname1 == null ? "" : afiliado.lastname1;
                    cliente.APELLIDO_CLIENTE2 = afiliado.lastname2 == null ? "" : afiliado.lastname2;
                    cliente.FECHA_NACIMIENTO = afiliado.birthdate == null ? new DateTime?() : DateTime.ParseExact(afiliado.birthdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    cliente.SEXO = afiliado.gender == null ? "" : afiliado.gender;
                    cliente.EDO_CIVIL = afiliado.maritalstatus == null ? "" : afiliado.maritalstatus;
                    //cliente.OCUPACION = afiliado.occupation == null ? "" : afiliado.occupation;
                    cliente.TELEFONO_HAB = afiliado.phone1;
                    cliente.TELEFONO_OFIC = afiliado.phone2 == null ? "" : afiliado.phone2;
                    cliente.TELEFONO_CEL = afiliado.phone3 == null ? "" : afiliado.phone3;
                    cliente.COD_SUCURSAL = afiliado.storeid;
                    cliente.COD_ESTADO = afiliado.cod_estado;
                    cliente.COD_CIUDAD = afiliado.cod_ciudad;
                    cliente.COD_MUNICIPIO = afiliado.cod_municipio;
                    cliente.COD_PARROQUIA = afiliado.cod_parroquia;
                    cliente.COD_URBANIZACION = afiliado.cod_urbanizacion;
                    if (afiliado.occupation == null)
                    {
                        cliente.OCUPACION = afiliado.occupation;
                    }
                    else if (afiliado.occupation.Length > 30)
                    {
                        cliente.OCUPACION = afiliado.occupation.Substring(0, 30);
                    }
                    else
                    {
                        cliente.OCUPACION = afiliado.occupation;
                    }
                }
                //ENTIDAD CustomerInterest
                foreach (var interes in afiliado.Intereses.Where(x => x.Checked == true))
                {
                    CustomerInterest customerInterest = new CustomerInterest()
                    {
                        customerid = Affiliate.id,
                        interestid = interes.id,
                        comments = ""
                    };
                    db.CustomerInterests.Add(customerInterest);
                }
                //ENTIDAD Photos_Affiliate
                if (file != null)
                {
                    try
                    {
                        int length = file.ContentLength;
                        byte[] buffer = new byte[length];
                        file.InputStream.Read(buffer, 0, length);
                        var Photos_Affiliate = new Photos_Affiliate()
                        {
                            photo = buffer,
                            photo_type = file.ContentType,
                            Affiliate_id = Affiliate.id
                        };
                        db.Photos_Affiliates.Add(Photos_Affiliate);
                    }
                    catch
                    {
                        return false;
                    }
                }
                //PARA QUE LA IMAGEN DEL DOCUMENTO SEA OPCIONAL
                //else
                //{
                //    return false;
                //}
                //ENTIDAD AffiliateAud
                var affiliateauditoria = new AffiliateAud()
                {
                    id = AfilliateAudID(),
                    affiliateid = Affiliate.id,
                    modifieduserid = (int)HttpContext.Current.Session["userid"],
                    modifieddate = System.DateTime.Now,
                    statusid = Affiliate.sumastatusid.Value,
                    reasonsid = Globals.ID_REASONS_INICIAL,
                    comments = afiliado.comments
                };
                db.AffiliateAuds.Add(affiliateauditoria);
                //YA NO SE ENVIARÁ INFORMACIÓN A LA WEB
                //if (SaveWebPlazas(afiliado))
                //{
                db.SaveChanges();
                return true;
                //}
                //else
                //{
                //    return false;
                //}
            }
        }

        public bool SaveChanges(AfiliadoSuma afiliado, HttpPostedFileBase fileNoValidado = null)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                // Entidad: Affiliate
                Affiliate affiliate = db.Affiliates.FirstOrDefault(a => a.id == afiliado.id);
                if (affiliate != null)
                {
                    affiliate.storeid = afiliado.storeid;
                    affiliate.channelid = afiliado.channelid;
                    affiliate.typeid = afiliado.typeid;
                    affiliate.typedelivery = afiliado.typedelivery;
                    affiliate.storeiddelivery = afiliado.storeiddelivery;
                    affiliate.modifieduserid = (int)HttpContext.Current.Session["userid"];
                    affiliate.modifieddate = System.DateTime.Now;
                    affiliate.sumastatusid = afiliado.sumastatusid;
                    affiliate.reasonsid = afiliado.reasonsid;
                    affiliate.twitter_account = afiliado.twitter_account;
                    affiliate.facebook_account = afiliado.facebook_account;
                    affiliate.instagram_account = afiliado.instagram_account;
                    affiliate.comments = afiliado.comments;
                }
                // Entidad: CLIENTE 
                CLIENTE cliente = db.CLIENTES.FirstOrDefault(c => c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO == afiliado.docnumber);
                if (cliente != null)
                {
                    cliente.E_MAIL = afiliado.email;
                    cliente.NACIONALIDAD = afiliado.nationality;
                    cliente.NOMBRE_CLIENTE1 = afiliado.name;
                    cliente.NOMBRE_CLIENTE2 = afiliado.name2;
                    cliente.APELLIDO_CLIENTE1 = afiliado.lastname1;
                    cliente.APELLIDO_CLIENTE2 = afiliado.lastname2;
                    cliente.FECHA_NACIMIENTO = afiliado.birthdate == null ? new DateTime?() : DateTime.ParseExact(afiliado.birthdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    cliente.SEXO = afiliado.gender;
                    cliente.EDO_CIVIL = afiliado.maritalstatus;
                    cliente.TELEFONO_HAB = afiliado.phone1;
                    cliente.TELEFONO_OFIC = afiliado.phone2;
                    cliente.TELEFONO_CEL = afiliado.phone3;
                    cliente.COD_SUCURSAL = afiliado.storeid;
                    cliente.COD_ESTADO = afiliado.cod_estado;
                    cliente.COD_CIUDAD = afiliado.cod_ciudad;
                    cliente.COD_MUNICIPIO = afiliado.cod_municipio;
                    cliente.COD_PARROQUIA = afiliado.cod_parroquia;
                    cliente.COD_URBANIZACION = afiliado.cod_urbanizacion;
                    if (afiliado.occupation == null)
                    {
                        cliente.OCUPACION = afiliado.occupation;
                    }
                    else if (afiliado.occupation.Length > 30)
                    {
                        cliente.OCUPACION = afiliado.occupation.Substring(0, 30);
                    }
                    else
                    {
                        cliente.OCUPACION = afiliado.occupation;
                    }
                }                
                // Entida: TARJETA
                //TARJETA tarjeta = db.TARJETAS.FirstOrDefault(t => t.NRO_AFILIACION.Equals(afiliado.id));
                Decimal pan = Convert.ToDecimal(afiliado.pan);
                TARJETA tarjeta = db.TARJETAS.FirstOrDefault(t => t.NRO_TARJETA.Equals(pan));
                if (tarjeta != null)
                {
                    tarjeta.NRO_AFILIACION = afiliado.id;
                    tarjeta.ESTATUS_TARJETA = afiliado.estatustarjeta;
                    tarjeta.COD_USUARIO = (int)HttpContext.Current.Session["userid"];
                    tarjeta.TRACK2 = afiliado.trackII;
                    tarjeta.CVV2 = afiliado.cvv2;
                    tarjeta.FECHA_CREACION = afiliado.printed == null ? new DateTime?() : DateTime.ParseExact(afiliado.printed, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                else if (afiliado.pan != null && afiliado.estatustarjeta != null)
                {
                    tarjeta = new TARJETA()
                    {
                        NRO_TARJETA = pan,
                        NRO_AFILIACION = afiliado.id,
                        TIPO_DOCUMENTO = afiliado.docnumber.Substring(0, 1),
                        NRO_DOCUMENTO = afiliado.docnumber.Substring(2),
                        ESTATUS_TARJETA = afiliado.estatustarjeta,
                        SALDO_PUNTOS = null,
                        OBSERVACIONES = null,
                        COD_USUARIO = (int)HttpContext.Current.Session["userid"],
                        TRACK1 = null,
                        TRACK2 = afiliado.trackII,
                        CVV2 = afiliado.cvv2,
                        FECHA_CREACION = afiliado.printed == null ? new DateTime?() : DateTime.ParseExact(afiliado.printed, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                    };
                    db.TARJETAS.Add(tarjeta);
                }

                // Entidad: CustomerInterest
                foreach (var m in db.CustomerInterests.Where(f => f.customerid == afiliado.id))
                {
                    db.CustomerInterests.Remove(m);
                }
                foreach (var interes in afiliado.Intereses.Where(x => x.Checked == true))
                {
                    CustomerInterest customerInterest = new CustomerInterest()
                    {
                        customerid = afiliado.id,
                        interestid = interes.id,
                        comments = ""
                    };
                    db.CustomerInterests.Add(customerInterest);
                }
                //Entidad: AffiliateAud 
                int sumastatusidactual = (from a in db.Affiliates
                                          where a.id.Equals(afiliado.id)
                                          select a.sumastatusid
                                         ).SingleOrDefault().Value;
                //Solo inserto registros cuando hay cambio de estado de Afiliación
                if (sumastatusidactual != afiliado.sumastatusid)
                {
                    var affiliateAuditoria = new AffiliateAud()
                    {
                        id = AfilliateAudID(),
                        affiliateid = afiliado.id,
                        modifieduserid = (int)HttpContext.Current.Session["userid"],
                        modifieddate = DateTime.Now,
                        statusid = afiliado.sumastatusid,
                        reasonsid = Globals.ID_REASONS_INICIAL,
                        comments = afiliado.comments
                    };
                    db.AffiliateAuds.Add(affiliateAuditoria);
                }
                //YA NO SE ENVIARÁ INFORMACIÓN A LA WEB
                //if (SaveWebPlazas(afiliado))
                //{
                //ENTIDAD Photos_Affiliate
                if (fileNoValidado != null)
                {
                    try
                    {
                        int length = fileNoValidado.ContentLength;
                        byte[] buffer = new byte[length];
                        fileNoValidado.InputStream.Read(buffer, 0, length);
                        Photos_Affiliate photos_affiliate = db.Photos_Affiliates.FirstOrDefault(x => x.Affiliate_id == afiliado.id);
                        if (photos_affiliate == null)
                        {
                            photos_affiliate = new Photos_Affiliate()
                        {
                            photo = buffer,
                            photo_type = fileNoValidado.ContentType,
                            Affiliate_id = afiliado.id
                        };
                            db.Photos_Affiliates.Add(photos_affiliate);
                        }
                        else
                        {
                            photos_affiliate.photo = buffer;
                            photos_affiliate.photo_type = fileNoValidado.ContentType;
                        }
                    }
                    catch
                    {
                    }
                }
                db.SaveChanges();
                return true;
                //}
                //else
                //{
                //    return false;
                //}
            }
        }

        public bool CambiarTipoDocumento(AfiliadoSuma afiliado, string tipoDocumento)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                // Entidad: Affiliate
                Affiliate affiliate = db.Affiliates.FirstOrDefault(a => a.id == afiliado.id);
                if (affiliate != null)
                {
                    affiliate.docnumber = tipoDocumento + afiliado.docnumber.Substring(1);
                    affiliate.modifieduserid = (int)HttpContext.Current.Session["userid"];
                    affiliate.modifieddate = System.DateTime.Now;
                }
                // Entidad: CLIENTE 
                CLIENTE cliente = db.CLIENTES.FirstOrDefault(c => c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO == afiliado.docnumber);
                if (cliente != null)
                {
                    CLIENTE clienteNuevo = new CLIENTE()
                    {
                        TIPO_DOCUMENTO = tipoDocumento,
                        NRO_DOCUMENTO = cliente.NRO_DOCUMENTO,
                        E_MAIL = cliente.E_MAIL,
                        NOMBRE_CLIENTE1 = cliente.NOMBRE_CLIENTE1,
                        NOMBRE_CLIENTE2 = cliente.NOMBRE_CLIENTE2,
                        APELLIDO_CLIENTE1 = cliente.APELLIDO_CLIENTE1,
                        APELLIDO_CLIENTE2 = cliente.APELLIDO_CLIENTE2,
                        FECHA_NACIMIENTO = cliente.FECHA_NACIMIENTO,
                        SEXO = cliente.SEXO,
                        EDO_CIVIL = cliente.EDO_CIVIL,
                        OCUPACION = cliente.OCUPACION,
                        TELEFONO_HAB = cliente.TELEFONO_HAB,
                        TELEFONO_OFIC = cliente.TELEFONO_OFIC,
                        TELEFONO_CEL = cliente.TELEFONO_CEL,
                        COD_SUCURSAL = cliente.COD_SUCURSAL,
                        COD_ESTADO = cliente.COD_ESTADO,
                        COD_CIUDAD = cliente.COD_CIUDAD,
                        COD_MUNICIPIO = cliente.COD_MUNICIPIO,
                        COD_PARROQUIA = cliente.COD_PARROQUIA,
                        COD_URBANIZACION = cliente.COD_URBANIZACION,
                        FECHA_CREACION = cliente.FECHA_CREACION
                    };
                    clienteNuevo.TIPO_DOCUMENTO = tipoDocumento;
                    //NACIONALIDAD => NINGUNA = "0", VENEZOLANO = "1", EXTRANJERO = "2"
                    if (tipoDocumento == "V")
                    {
                        clienteNuevo.NACIONALIDAD = "1";
                    }
                    else if (tipoDocumento == "E" || tipoDocumento == "P")
                    {
                        clienteNuevo.NACIONALIDAD = "2";
                    }
                    else
                    {
                        clienteNuevo.NACIONALIDAD = "0";
                    }
                    db.CLIENTES.Add(clienteNuevo);
                    db.CLIENTES.Remove(cliente);
                }
                // Entida: TARJETA
                //TARJETA tarjeta = db.TARJETAS.FirstOrDefault(t => t.NRO_AFILIACION.Equals(afiliado.id));
                Decimal pan = Convert.ToDecimal(afiliado.pan);
                TARJETA tarjeta = db.TARJETAS.FirstOrDefault(t => t.NRO_TARJETA.Equals(pan));
                if (tarjeta != null)
                {
                    tarjeta.TIPO_DOCUMENTO = tipoDocumento;
                    tarjeta.COD_USUARIO = (int)HttpContext.Current.Session["userid"];
                }
                //Entidad: AffiliateAud 
                //inserto registro por cambio de tipo de documento
                var affiliateAuditoria = new AffiliateAud()
                {
                    id = AfilliateAudID(),
                    affiliateid = afiliado.id,
                    modifieduserid = (int)HttpContext.Current.Session["userid"],
                    modifieddate = DateTime.Now,
                    statusid = afiliado.sumastatusid,
                    reasonsid = Globals.ID_REASONS_INICIAL,
                    comments = afiliado.comments
                };
                db.AffiliateAuds.Add(affiliateAuditoria);
                db.SaveChanges();
                return true;
            }
        }

        public bool Aprobar(AfiliadoSuma afiliado)
        {
            string RespuestaCardsJson = WSL.Cards.addClient(afiliado.docnumber.Substring(2), (afiliado.name + " " + afiliado.lastname1).ToUpper(), afiliado.phone1, "Plazas Baruta");
            if (WSL.Cards.ExceptionServicioCards(RespuestaCardsJson))
            {
                return false;
            }
            RespuestaCards RespuestaCards = (RespuestaCards)JsonConvert.DeserializeObject<RespuestaCards>(RespuestaCardsJson);
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                if (RespuestaCards.excode == "0" || RespuestaCards.excode == "7")
                {
                    //Se buscan los datos de Tarjeta del AFILIADO en Cards
                    //SERVICIO WSL.Cards.getClient !
                    string clienteCardsJson = WSL.Cards.getClient(afiliado.docnumber.Substring(2));
                    if (WSL.Cards.ExceptionServicioCards(clienteCardsJson))
                    {
                        return false;
                    }
                    ClienteCards clienteCards = (ClienteCards)JsonConvert.DeserializeObject<ClienteCards>(clienteCardsJson);
                    afiliado.pan = clienteCards.pan;
                    afiliado.printed = clienteCards.printed == null ? null : clienteCards.printed.Substring(6, 2) + "/" + clienteCards.printed.Substring(4, 2) + "/" + clienteCards.printed.Substring(0, 4);
                    afiliado.estatustarjeta = clienteCards.tarjeta;
                    afiliado.trackII = Tarjeta.ConstruirTrackII(afiliado.pan);
                    afiliado.cvv2 = "123";
                    afiliado.sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_AFILIACION_ACTIVA) && (s.tablename == "Affiliatte")).id;
                    return SaveChanges(afiliado);
                }
                else
                {
                    return false;
                }
            }
        }

        public bool ImprimirTarjeta(AfiliadoSuma afiliado)
        {
            string RespuestaCardsJson = WSL.Cards.cardActive(afiliado.docnumber.Substring(2));
            if (WSL.Cards.ExceptionServicioCards(RespuestaCardsJson))
            {
                return false;
            }
            RespuestaCards RespuestaCards = (RespuestaCards)JsonConvert.DeserializeObject<RespuestaCards>(RespuestaCardsJson);
            if (RespuestaCards.excode == "0")
            {
                RespuestaCardsJson = WSL.Cards.cardPrint(afiliado.docnumber.Substring(2));
                if (WSL.Cards.ExceptionServicioCards(RespuestaCardsJson))
                {
                    return false;
                }
                RespuestaCards = (RespuestaCards)JsonConvert.DeserializeObject<RespuestaCards>(RespuestaCardsJson);
            }
            else
            {
                return false;
            }
            if (RespuestaCards.excode == "0")
            {
                //Se buscan los datos de Tarjeta del AFILIADO en Cards
                //SERVICIO WSL.Cards.getClient !
                string clienteCardsJson = WSL.Cards.getClient(afiliado.docnumber.Substring(2));
                if (WSL.Cards.ExceptionServicioCards(clienteCardsJson))
                {
                    return false;
                }
                ClienteCards clienteCards = (ClienteCards)JsonConvert.DeserializeObject<ClienteCards>(clienteCardsJson);
                afiliado.pan = clienteCards.pan;
                afiliado.printed = clienteCards.printed == null ? null : clienteCards.printed.Substring(6, 2) + "/" + clienteCards.printed.Substring(4, 2) + "/" + clienteCards.printed.Substring(0, 4);
                afiliado.estatustarjeta = clienteCards.tarjeta;
                afiliado.trackII = Tarjeta.ConstruirTrackII(afiliado.pan);
                afiliado.cvv2 = "123";
                return SaveChanges(afiliado);
            }
            else
            {
                return false;
            }
        }

        private bool BorrarTarjeta(string pan)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                // Entidad TARJETA
                var dpan = Convert.ToDecimal(pan);
                TARJETA tarjeta = db.TARJETAS.FirstOrDefault(t => t.NRO_TARJETA.Equals(dpan));
                if (tarjeta != null)
                {
                    db.TARJETAS.Remove(tarjeta);
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool BloquearTarjeta(AfiliadoSuma afiliado)
        {
            string RespuestaCardsJson = WSL.Cards.addCard(afiliado.docnumber.Substring(2));
            if (WSL.Cards.ExceptionServicioCards(RespuestaCardsJson))
            {
                return false;
            }
            RespuestaCards RespuestaCards = (RespuestaCards)JsonConvert.DeserializeObject<RespuestaCards>(RespuestaCardsJson);
            if (RespuestaCards.excode == "0")
            {
                if (BorrarTarjeta(afiliado.pan))
                {
                    //Se buscan los datos de la nueva Tarjeta del AFILIADO en Cards
                    //SERVICIO WSL.Cards.getClient !
                    string clienteCardsJson = WSL.Cards.getClient(afiliado.docnumber.Substring(2));
                    if (WSL.Cards.ExceptionServicioCards(clienteCardsJson))
                    {
                        return false;
                    }
                    ClienteCards clienteCards = (ClienteCards)JsonConvert.DeserializeObject<ClienteCards>(clienteCardsJson);
                    afiliado.pan = clienteCards.pan;
                    afiliado.printed = clienteCards.printed == null ? null : clienteCards.printed.Substring(6, 2) + "/" + clienteCards.printed.Substring(4, 2) + "/" + clienteCards.printed.Substring(0, 4);
                    afiliado.estatustarjeta = clienteCards.tarjeta;
                    afiliado.trackII = Tarjeta.ConstruirTrackII(afiliado.pan);
                    afiliado.cvv2 = "123";
                    return SaveChanges(afiliado);
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }

        public bool SuspenderTarjeta(AfiliadoSuma afiliado, string observaciones)
        {
            string RespuestaCardsJson = WSL.Cards.cardStatus(afiliado.docnumber.Substring(2), Globals.ID_ESTATUS_TARJETA_SUSPENDIDA);
            if (WSL.Cards.ExceptionServicioCards(RespuestaCardsJson))
            {
                return false;
            }
            RespuestaCards RespuestaCards = (RespuestaCards)JsonConvert.DeserializeObject<RespuestaCards>(RespuestaCardsJson);
            if (RespuestaCards.excode == "0")
            {
                //Se buscan los datos de Tarjeta del AFILIADO en Cards
                //SERVICIO WSL.Cards.getClient !
                string clienteCardsJson = WSL.Cards.getClient(afiliado.docnumber.Substring(2));
                if (WSL.Cards.ExceptionServicioCards(clienteCardsJson))
                {
                    return false;
                }
                ClienteCards clienteCards = (ClienteCards)JsonConvert.DeserializeObject<ClienteCards>(clienteCardsJson);
                afiliado.pan = clienteCards.pan;
                afiliado.printed = clienteCards.printed == null ? null : clienteCards.printed.Substring(6, 2) + "/" + clienteCards.printed.Substring(4, 2) + "/" + clienteCards.printed.Substring(0, 4);
                afiliado.estatustarjeta = clienteCards.tarjeta;
                afiliado.trackII = Tarjeta.ConstruirTrackII(afiliado.pan);
                afiliado.cvv2 = "123";
                afiliado.comments = "Tarjeta suspendida, fecha: " + DateTime.Now + ", observaciones: " + observaciones;
                return SaveChanges(afiliado);
            }
            else
            {
                return false;
            }
        }

        public bool ReactivarTarjeta(AfiliadoSuma afiliado)
        {
            string RespuestaCardsJson = WSL.Cards.cardActive(afiliado.docnumber.Substring(2));
            if (WSL.Cards.ExceptionServicioCards(RespuestaCardsJson))
            {
                return false;
            }
            RespuestaCards RespuestaCards = (RespuestaCards)JsonConvert.DeserializeObject<RespuestaCards>(RespuestaCardsJson);
            if (RespuestaCards.excode == "0")
            {
                //Se buscan los datos de Tarjeta del AFILIADO en Cards
                //SERVICIO WSL.Cards.getClient !
                string clienteCardsJson = WSL.Cards.getClient(afiliado.docnumber.Substring(2));
                if (WSL.Cards.ExceptionServicioCards(clienteCardsJson))
                {
                    return false;
                }
                ClienteCards clienteCards = (ClienteCards)JsonConvert.DeserializeObject<ClienteCards>(clienteCardsJson);
                afiliado.pan = clienteCards.pan;
                afiliado.printed = clienteCards.printed == null ? null : clienteCards.printed.Substring(6, 2) + "/" + clienteCards.printed.Substring(4, 2) + "/" + clienteCards.printed.Substring(0, 4);
                afiliado.estatustarjeta = clienteCards.tarjeta;
                afiliado.trackII = Tarjeta.ConstruirTrackII(afiliado.pan);
                afiliado.cvv2 = "123";
                afiliado.comments = "";
                return SaveChanges(afiliado);
            }
            else
            {
                return false;
            }
        }

        public SaldosMovimientos FindSaldosMovimientos(AfiliadoSuma afiliado)
        {
            SaldosMovimientos SaldosMovimientos = new SaldosMovimientos();
            SaldosMovimientos.DocId = afiliado.docnumber;
            string saldosJson = WSL.Cards.getBalance(SaldosMovimientos.DocId.Substring(2));
            if (WSL.Cards.ExceptionServicioCards(saldosJson))
            {
                return null;
            }
            SaldosMovimientos.Saldos = (List<Saldo>)JsonConvert.DeserializeObject<List<Saldo>>(saldosJson);
            string movimientosPrepagoJson = WSL.Cards.getBatch(Globals.TIPO_CUENTA_PREPAGO, SaldosMovimientos.DocId.Substring(2));
            if (WSL.Cards.ExceptionServicioCards(movimientosPrepagoJson))
            {
                return null;
            }
            SaldosMovimientos.MovimientosPrepago = (List<Movimiento>)JsonConvert.DeserializeObject<List<Movimiento>>(movimientosPrepagoJson);
            List<Movimiento> MovimientosPrepagoOrdenados = SaldosMovimientos.MovimientosPrepago.OrderByDescending(x => x.batchid).ToList();
            SaldosMovimientos.MovimientosPrepago = MovimientosPrepagoOrdenados.Take(20).ToList();
            foreach (Movimiento mov in SaldosMovimientos.MovimientosPrepago)
            {
                mov.fecha = mov.fecha.Substring(6, 2) + "/" + mov.fecha.Substring(4, 2) + "/" + mov.fecha.Substring(0, 4);
                mov.batchtime = mov.batchtime.Substring(0, 2) + ":" + mov.batchtime.Substring(2, 2) + ":" + mov.batchtime.Substring(4, 2);
                mov.fechahora = DateTime.ParseExact(mov.fecha + " " + mov.batchtime, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                if (mov.transcode == Globals.TRANSCODE_ANULACION_RECARGA_PREPAGO || mov.transcode == Globals.TRANSCODE_ANULACION_TRANSFERENCIA_CREDITO_PREPAGO || mov.transcode == Globals.TRANSCODE_ANULACION_TRANSFERENCIA_DEBITO_PREPAGO)
                {
                    mov.isodescription = mov.isodescription + " (" + mov.b037 + ")";
                }
                if (mov.isodescription.Contains("offline"))
                {
                    //buscar info en FueraDeLinea
                    using (LealtadEntities db = new LealtadEntities())
                    {
                        db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                        string operacion = mov.batchid.ToString();
                        FueraDeLinea f = db.FueraDeLineas.FirstOrDefault(t => t.batchid.Equals(operacion));
                        if (f != null)
                        {
                            string sucursal = afiliado.StoreOptions.Where(c => c.id != null).FirstOrDefault(x => x.id.Equals(f.store_code)).sucursal;
                            mov.isodescription = mov.isodescription + " (origen: " + sucursal + ", motivo: " + f.observaciones + ")";
                        }
                    }
                }
            }
            string movimientosLealtadJson = WSL.Cards.getBatch(Globals.TIPO_CUENTA_SUMA, SaldosMovimientos.DocId.Substring(2));
            if (WSL.Cards.ExceptionServicioCards(movimientosLealtadJson))
            {
                return null;
            }
            SaldosMovimientos.MovimientosSuma = (List<Movimiento>)JsonConvert.DeserializeObject<List<Movimiento>>(movimientosLealtadJson);
            List<Movimiento> MovimientosSumaOrdenados = SaldosMovimientos.MovimientosSuma.OrderByDescending(x => x.batchid).ToList();
            SaldosMovimientos.MovimientosSuma = MovimientosSumaOrdenados.Take(20).ToList();
            foreach (Movimiento mov in SaldosMovimientos.MovimientosSuma)
            {
                mov.fecha = mov.fecha.Substring(6, 2) + "/" + mov.fecha.Substring(4, 2) + "/" + mov.fecha.Substring(0, 4);
                mov.batchtime = mov.batchtime.Substring(0, 2) + ":" + mov.batchtime.Substring(2, 2) + ":" + mov.batchtime.Substring(4, 2);
                mov.fechahora = DateTime.ParseExact(mov.fecha + " " + mov.batchtime, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                if (mov.transcode == Globals.TRANSCODE_ANULACION_TRANSFERENCIA_CREDITO_SUMA || mov.transcode == Globals.TRANSCODE_ANULACION_TRANSFERENCIA_DEBITO_SUMA)
                {
                    mov.isodescription = mov.isodescription + " (" + mov.b037 + ")";
                }
                if (mov.transcode == Globals.TRANSCODE_CANJE_SUMA || mov.transcode == Globals.TRANSCODE_TRANSFERENCIA_DEBITO_SUMA || mov.transcode == Globals.TRANSCODE_ANULACION_TRANSFERENCIA_CREDITO_SUMA)
                {
                    mov.saldo = mov.saldo * Convert.ToInt32(Globals.FACTOR_CANJE);
                }
            }
            return SaldosMovimientos;
        }

        public string Acreditar(AfiliadoSuma afiliado, string monto)
        {
            int intentos;
            string respuesta = "";
            //Se llama al servicio para verificar q este activo. Se intenta la operación 3 veces, antes de fallar
            for (intentos = 0; intentos <= 3; intentos++)
            {
                //SERVICIO WSL.Cards.getClient !
                string clienteCardsJson = WSL.Cards.getClient(afiliado.docnumber.Substring(2));
                if (WSL.Cards.ExceptionServicioCards(clienteCardsJson))
                {
                    respuesta = "Servicio no responde";
                    intentos++;
                }
                else
                {
                    break;
                }
            }
            if (intentos > 3)
            {
                return respuesta;
            }
            //Se intenta la operación 6 veces, antes de fallar
            for (intentos = 0; intentos <= 6; intentos++)
            {
                string RespuestaCardsJson = WSL.Cards.addBatch(afiliado.docnumber.Substring(2), monto, Globals.TRANSCODE_ACREDITACION_SUMA, "NULL");
                if (WSL.Cards.ExceptionServicioCards(RespuestaCardsJson))
                {
                    ExceptionJSON exceptionJson = (ExceptionJSON)JsonConvert.DeserializeObject<ExceptionJSON>(RespuestaCardsJson);
                    respuesta = exceptionJson.exdetail + "-" + exceptionJson.exsource;
                    intentos++;
                }
                else
                {
                    RespuestaCards RespuestaCards = (RespuestaCards)JsonConvert.DeserializeObject<RespuestaCards>(RespuestaCardsJson);
                    if (Convert.ToDecimal(RespuestaCards.excode) < 0)
                    {
                        return null;
                    }
                    else
                    {
                        return RespuestaCards.exdetail;
                    }
                }
            }
            return respuesta;
        }

        public AfiliadoSuma CambiarASuma(AfiliadoSuma afiliado)
        {
            afiliado.type = "Suma";
            afiliado.typeid = Globals.ID_TYPE_SUMA;
            return afiliado;
        }

        public AfiliadoSuma CambiarAPrepago(AfiliadoSuma afiliado)
        {
            afiliado.type = "Prepago";
            afiliado.typeid = Globals.ID_TYPE_PREPAGO;
            return afiliado;
        }

        public AfiliadoSuma ReiniciarAfiliacionSumaAPrepago(AfiliadoSuma afiliado)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                Affiliate afiliate = db.Affiliates.FirstOrDefault(a => a.docnumber == afiliado.docnumber);
                afiliate.sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_AFILIACION_INICIAL) && (s.tablename == "Affiliatte")).id;
                db.SaveChanges();
                afiliado.estatus = "Nueva";
                afiliado.sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_AFILIACION_INICIAL) && (s.tablename == "Affiliatte")).id;
                return afiliado;
            }
        }

        public bool EliminarAfiliacion(AfiliadoSuma afiliado, string observaciones)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                afiliado.estatus = "Eliminada"; 
                afiliado.sumastatusid = db.SumaStatuses.FirstOrDefault(s => (s.value == Globals.ID_ESTATUS_AFILIACION_ELIMINADA) && (s.tablename == "Affiliatte")).id;
                afiliado.comments = observaciones;
                return SaveChanges(afiliado);                
            }
        }

        //public List<ReporteSuma> ReporteTransacciones(string fechadesde, string fechahasta, string tipotrans, string numdoc = "")
        //{
        //    string fechasdesdemod = fechadesde.Substring(6, 4) + fechadesde.Substring(3, 2) + fechadesde.Substring(0, 2);
        //    string fechahastamod = fechahasta.Substring(6, 4) + fechahasta.Substring(3, 2) + fechahasta.Substring(0, 2);
        //    List<ReporteSuma> reporte = new List<ReporteSuma>();
        //    EncabezadoReporteSuma encabezado = new EncabezadoReporteSuma();
        //    #region Por Afiliado específico
        //    if (numdoc != "")
        //    {
        //        List<AfiliadoSumaIndex> afiliados = Find(numdoc, "", "", "", "").ToList();
        //        encabezado.nombreReporte = "Reporte de Transacciones";
        //        encabezado.tipoconsultaReporte = "Por Afiliado";
        //        if (afiliados.Count == 0)
        //        {
        //            encabezado.parametrotipoconsultaReporte = numdoc;
        //        }
        //        else
        //        {
        //            encabezado.parametrotipoconsultaReporte = afiliados.First().docnumber + " " + afiliados.First().name + " " + afiliados.First().lastname1;
        //        }
        //        encabezado.fechainicioReporte = fechadesde;
        //        encabezado.fechafinReporte = fechahasta;
        //        encabezado.tipotransaccionReporte = tipotrans;
        //        foreach (AfiliadoSumaIndex a in afiliados)
        //        {
        //            //determino el tipo de llamada a getreport
        //            //string movimientosLealtadJson;
        //            List<Movimiento> movimientosSuma = new List<Movimiento>();                    
        //            if (tipotrans == "Acreditacion")
        //            {
        //            //    movimientosLealtadJson = WSL.Cards.getReport(fechasdesdemod, fechahastamod, a.docnumber.Substring(2), "NULL", Globals.TRANSCODE_ACREDITACION_SUMA);
        //                using (CardsEntities db = new CardsEntities())
        //                {
        //                    db.Database.Connection.ConnectionString = AppModule.ConnectionString("Cards");
        //                    foreach (PLZ_GETREPORT_Result fila in db.PLZ_GETREPORT(fechasdesdemod, fechahastamod, a.docnumber.Substring(2), "NULL", Globals.TRANSCODE_ACREDITACION_SUMA))
        //                    {
        //                        if (fila.TRANSCODE != Globals.TRANSCODE_CONSULTA_DE_SALDO)
        //                        {
        //                            Movimiento mov = new Movimiento()
        //                            {
        //                                fecha = fila.FECHA,
        //                                batchtime = fila.BATCHTIME,
        //                                saldo = fila.SALDO.Value,
        //                                isodescription = fila.ISODESCRIPTION,
        //                                transcode = fila.TRANSCODE.ToString(),
        //                                transname = fila.TRANSNAME,
        //                                pan = fila.PAN,
        //                                batchid = fila.BATCHID
        //                            };
        //                            movimientosSuma.Add(mov);
        //                        }
        //                    }
        //                }
        //            }
        //            else if (tipotrans == "Canje")
        //            {
        //            //    movimientosLealtadJson = WSL.Cards.getReport(fechasdesdemod, fechahastamod, a.docnumber.Substring(2), "NULL", Globals.TRANSCODE_CANJE_SUMA);
        //                using (CardsEntities db = new CardsEntities())
        //                {
        //                    db.Database.Connection.ConnectionString = AppModule.ConnectionString("Cards");
        //                    foreach (PLZ_GETREPORT_Result fila in db.PLZ_GETREPORT(fechasdesdemod, fechahastamod, a.docnumber.Substring(2), "NULL", Globals.TRANSCODE_CANJE_SUMA))
        //                    {
        //                        if (fila.TRANSCODE != Globals.TRANSCODE_CONSULTA_DE_SALDO)
        //                        {
        //                            Movimiento mov = new Movimiento()
        //                            {
        //                                fecha = fila.FECHA,
        //                                batchtime = fila.BATCHTIME,
        //                                saldo = fila.SALDO.Value,
        //                                isodescription = fila.ISODESCRIPTION,
        //                                transcode = fila.TRANSCODE.ToString(),
        //                                transname = fila.TRANSNAME,
        //                                pan = fila.PAN,
        //                                batchid = fila.BATCHID
        //                            };
        //                            movimientosSuma.Add(mov);
        //                        }
        //                    }
        //                }
        //            }
        //            else if (tipotrans == "Transferencias Credito")
        //            {
        //            //    movimientosLealtadJson = WSL.Cards.getReport(fechasdesdemod, fechahastamod, a.docnumber.Substring(2), "NULL", Globals.TRANSCODE_TRANSFERENCIA_CREDITO_SUMA);
        //                using (CardsEntities db = new CardsEntities())
        //                {
        //                    db.Database.Connection.ConnectionString = AppModule.ConnectionString("Cards");
        //                    foreach (PLZ_GETREPORT_Result fila in db.PLZ_GETREPORT(fechasdesdemod, fechahastamod, a.docnumber.Substring(2), "NULL", Globals.TRANSCODE_TRANSFERENCIA_CREDITO_SUMA))
        //                    {
        //                        if (fila.TRANSCODE != Globals.TRANSCODE_CONSULTA_DE_SALDO)
        //                        {
        //                            Movimiento mov = new Movimiento()
        //                            {
        //                                fecha = fila.FECHA,
        //                                batchtime = fila.BATCHTIME,
        //                                saldo = fila.SALDO.Value,
        //                                isodescription = fila.ISODESCRIPTION,
        //                                transcode = fila.TRANSCODE.ToString(),
        //                                transname = fila.TRANSNAME,
        //                                pan = fila.PAN,
        //                                batchid = fila.BATCHID
        //                            };
        //                            movimientosSuma.Add(mov);
        //                        }
        //                    }
        //                }
        //            }
        //            else if (tipotrans == "Transferencias Debito")
        //            {
        //            //    movimientosLealtadJson = WSL.Cards.getReport(fechasdesdemod, fechahastamod, a.docnumber.Substring(2), "NULL", Globals.TRANSCODE_TRANSFERENCIA_DEBITO_SUMA);
        //                using (CardsEntities db = new CardsEntities())
        //                {
        //                    db.Database.Connection.ConnectionString = AppModule.ConnectionString("Cards");
        //                    foreach (PLZ_GETREPORT_Result fila in db.PLZ_GETREPORT(fechasdesdemod, fechahastamod, a.docnumber.Substring(2), "NULL", Globals.TRANSCODE_TRANSFERENCIA_DEBITO_SUMA))
        //                    {
        //                        if (fila.TRANSCODE != Globals.TRANSCODE_CONSULTA_DE_SALDO)
        //                        {
        //                            Movimiento mov = new Movimiento()
        //                            {
        //                                fecha = fila.FECHA,
        //                                batchtime = fila.BATCHTIME,
        //                                saldo = fila.SALDO.Value,
        //                                isodescription = fila.ISODESCRIPTION,
        //                                transcode = fila.TRANSCODE.ToString(),
        //                                transname = fila.TRANSNAME,
        //                                pan = fila.PAN,
        //                                batchid = fila.BATCHID
        //                            };
        //                            movimientosSuma.Add(mov);
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //            //    movimientosLealtadJson = WSL.Cards.getReport(fechasdesdemod, fechahastamod, a.docnumber.Substring(2), Globals.TIPO_CUENTA_SUMA, "NULL");
        //                using (CardsEntities db = new CardsEntities())
        //                {
        //                    db.Database.Connection.ConnectionString = AppModule.ConnectionString("Cards");
        //                    foreach (PLZ_GETREPORT_Result fila in db.PLZ_GETREPORT(fechasdesdemod, fechahastamod, a.docnumber.Substring(2), Globals.TIPO_CUENTA_SUMA, "NULL"))
        //                    {
        //                        if (fila.TRANSCODE != Globals.TRANSCODE_CONSULTA_DE_SALDO)
        //                        {
        //                            Movimiento mov = new Movimiento()
        //                            {
        //                                fecha = fila.FECHA,
        //                                batchtime = fila.BATCHTIME,
        //                                saldo = fila.SALDO.Value,
        //                                isodescription = fila.ISODESCRIPTION,
        //                                transcode = fila.TRANSCODE.ToString(),
        //                                transname = fila.TRANSNAME,
        //                                pan = fila.PAN,
        //                                batchid = fila.BATCHID
        //                            };
        //                            movimientosSuma.Add(mov);
        //                        }
        //                    }
        //                }
        //            }
        //            //if (WSL.Cards.ExceptionServicioCards(movimientosLealtadJson))
        //            //{
        //            //    return null;
        //            //}
        //            //List<Movimiento> movimientosSuma = (List<Movimiento>)JsonConvert.DeserializeObject<List<Movimiento>>(movimientosLealtadJson).OrderBy(x => x.fecha).ToList();                    
        //            foreach (Movimiento m in movimientosSuma)
        //            {
        //                ReporteSuma linea = new ReporteSuma()
        //                {
        //                    Afiliado = a,
        //                    fecha = m.batchtime == null ? DateTime.ParseExact(m.fecha.Substring(6, 2) + "/" + m.fecha.Substring(4, 2) + "/" + m.fecha.Substring(0, 4), "dd/MM/yyyy", CultureInfo.InvariantCulture) : DateTime.ParseExact(m.fecha.Substring(6, 2) + "/" + m.fecha.Substring(4, 2) + "/" + m.fecha.Substring(0, 4) + " " + m.batchtime.Substring(0, 2) + ":" + m.batchtime.Substring(2, 2) + ":" + m.batchtime.Substring(4, 2), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
        //                    monto = Convert.ToInt32(m.saldo),
        //                    detalle = m.isodescription,
        //                    tipo = m.transcode + "-" + m.transname,
        //                    numerotarjeta = Convert.ToDecimal(m.pan),
        //                    batchid = m.batchid.ToString(),
        //                    Encabezado = encabezado
        //                };
        //                reporte.Add(linea);
        //            }
        //        }
        //    }
        //    #endregion
        //    #region Todos los Afiliados
        //    else if (numdoc == "")
        //    {
        //        List<AfiliadoSumaIndex> afiliados = Find("", "", "", "", "").ToList();
        //        encabezado.nombreReporte = "Reporte de Transacciones";
        //        encabezado.tipoconsultaReporte = "Por Afiliado";
        //        encabezado.parametrotipoconsultaReporte = "Todos";
        //        encabezado.fechainicioReporte = fechadesde;
        //        encabezado.fechafinReporte = fechahasta;
        //        encabezado.tipotransaccionReporte = tipotrans;
        //        foreach (AfiliadoSumaIndex a in afiliados)
        //        {
        //            //determino el tipo de llamada a getreport
        //            //string movimientosLealtadJson;
        //            List<Movimiento> movimientosSuma = new List<Movimiento>();
        //            if (tipotrans == "Acreditacion")
        //            {
        //                //    movimientosLealtadJson = WSL.Cards.getReport(fechasdesdemod, fechahastamod, a.docnumber.Substring(2), "NULL", Globals.TRANSCODE_ACREDITACION_SUMA);
        //                using (CardsEntities db = new CardsEntities())
        //                {
        //                    db.Database.Connection.ConnectionString = AppModule.ConnectionString("Cards");
        //                    foreach (PLZ_GETREPORT_Result fila in db.PLZ_GETREPORT(fechasdesdemod, fechahastamod, a.docnumber.Substring(2), "NULL", Globals.TRANSCODE_ACREDITACION_SUMA))
        //                    {
        //                        if (fila.TRANSCODE != Globals.TRANSCODE_CONSULTA_DE_SALDO)
        //                        {
        //                            Movimiento mov = new Movimiento()
        //                            {
        //                                fecha = fila.FECHA,
        //                                batchtime = fila.BATCHTIME,
        //                                saldo = fila.SALDO.Value,
        //                                isodescription = fila.ISODESCRIPTION,
        //                                transcode = fila.TRANSCODE.ToString(),
        //                                transname = fila.TRANSNAME,
        //                                pan = fila.PAN,
        //                                batchid = fila.BATCHID
        //                            };
        //                            movimientosSuma.Add(mov);
        //                        }
        //                    }
        //                }
        //            }
        //            else if (tipotrans == "Canje")
        //            {
        //                //    movimientosLealtadJson = WSL.Cards.getReport(fechasdesdemod, fechahastamod, a.docnumber.Substring(2), "NULL", Globals.TRANSCODE_CANJE_SUMA);
        //                using (CardsEntities db = new CardsEntities())
        //                {
        //                    db.Database.Connection.ConnectionString = AppModule.ConnectionString("Cards");
        //                    foreach (PLZ_GETREPORT_Result fila in db.PLZ_GETREPORT(fechasdesdemod, fechahastamod, a.docnumber.Substring(2), "NULL", Globals.TRANSCODE_CANJE_SUMA))
        //                    {
        //                        if (fila.TRANSCODE != Globals.TRANSCODE_CONSULTA_DE_SALDO)
        //                        {
        //                            Movimiento mov = new Movimiento()
        //                            {
        //                                fecha = fila.FECHA,
        //                                batchtime = fila.BATCHTIME,
        //                                saldo = fila.SALDO.Value,
        //                                isodescription = fila.ISODESCRIPTION,
        //                                transcode = fila.TRANSCODE.ToString(),
        //                                transname = fila.TRANSNAME,
        //                                pan = fila.PAN,
        //                                batchid = fila.BATCHID
        //                            };
        //                            movimientosSuma.Add(mov);
        //                        }
        //                    }
        //                }
        //            }
        //            else if (tipotrans == "Transferencias Credito")
        //            {
        //                //    movimientosLealtadJson = WSL.Cards.getReport(fechasdesdemod, fechahastamod, a.docnumber.Substring(2), "NULL", Globals.TRANSCODE_TRANSFERENCIA_CREDITO_SUMA);
        //                using (CardsEntities db = new CardsEntities())
        //                {
        //                    db.Database.Connection.ConnectionString = AppModule.ConnectionString("Cards");
        //                    foreach (PLZ_GETREPORT_Result fila in db.PLZ_GETREPORT(fechasdesdemod, fechahastamod, a.docnumber.Substring(2), "NULL", Globals.TRANSCODE_TRANSFERENCIA_CREDITO_SUMA))
        //                    {
        //                        if (fila.TRANSCODE != Globals.TRANSCODE_CONSULTA_DE_SALDO)
        //                        {
        //                            Movimiento mov = new Movimiento()
        //                            {
        //                                fecha = fila.FECHA,
        //                                batchtime = fila.BATCHTIME,
        //                                saldo = fila.SALDO.Value,
        //                                isodescription = fila.ISODESCRIPTION,
        //                                transcode = fila.TRANSCODE.ToString(),
        //                                transname = fila.TRANSNAME,
        //                                pan = fila.PAN,
        //                                batchid = fila.BATCHID
        //                            };
        //                            movimientosSuma.Add(mov);
        //                        }
        //                    }
        //                }
        //            }
        //            else if (tipotrans == "Transferencias Debito")
        //            {
        //                //    movimientosLealtadJson = WSL.Cards.getReport(fechasdesdemod, fechahastamod, a.docnumber.Substring(2), "NULL", Globals.TRANSCODE_TRANSFERENCIA_DEBITO_SUMA);
        //                using (CardsEntities db = new CardsEntities())
        //                {
        //                    db.Database.Connection.ConnectionString = AppModule.ConnectionString("Cards");
        //                    foreach (PLZ_GETREPORT_Result fila in db.PLZ_GETREPORT(fechasdesdemod, fechahastamod, a.docnumber.Substring(2), "NULL", Globals.TRANSCODE_TRANSFERENCIA_DEBITO_SUMA))
        //                    {
        //                        if (fila.TRANSCODE != Globals.TRANSCODE_CONSULTA_DE_SALDO)
        //                        {
        //                            Movimiento mov = new Movimiento()
        //                            {
        //                                fecha = fila.FECHA,
        //                                batchtime = fila.BATCHTIME,
        //                                saldo = fila.SALDO.Value,
        //                                isodescription = fila.ISODESCRIPTION,
        //                                transcode = fila.TRANSCODE.ToString(),
        //                                transname = fila.TRANSNAME,
        //                                pan = fila.PAN,
        //                                batchid = fila.BATCHID
        //                            };
        //                            movimientosSuma.Add(mov);
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                //    movimientosLealtadJson = WSL.Cards.getReport(fechasdesdemod, fechahastamod, a.docnumber.Substring(2), Globals.TIPO_CUENTA_SUMA, "NULL");
        //                using (CardsEntities db = new CardsEntities())
        //                {
        //                    db.Database.Connection.ConnectionString = AppModule.ConnectionString("Cards");
        //                    foreach (PLZ_GETREPORT_Result fila in db.PLZ_GETREPORT(fechasdesdemod, fechahastamod, a.docnumber.Substring(2), Globals.TIPO_CUENTA_SUMA, "NULL"))
        //                    {
        //                        if (fila.TRANSCODE != Globals.TRANSCODE_CONSULTA_DE_SALDO)
        //                        {
        //                            Movimiento mov = new Movimiento()
        //                            {
        //                                fecha = fila.FECHA,
        //                                batchtime = fila.BATCHTIME,
        //                                saldo = fila.SALDO.Value,
        //                                isodescription = fila.ISODESCRIPTION,
        //                                transcode = fila.TRANSCODE.ToString(),
        //                                transname = fila.TRANSNAME,
        //                                pan = fila.PAN,
        //                                batchid = fila.BATCHID
        //                            };
        //                            movimientosSuma.Add(mov);
        //                        }
        //                    }
        //                }
        //            }
        //            //if (WSL.Cards.ExceptionServicioCards(movimientosLealtadJson))
        //            //{
        //            //    return null;
        //            //}
        //            //List<Movimiento> movimientosSuma = (List<Movimiento>)JsonConvert.DeserializeObject<List<Movimiento>>(movimientosLealtadJson).OrderBy(x => x.fecha).ToList();                    
        //            foreach (Movimiento m in movimientosSuma)
        //            {
        //                ReporteSuma linea = new ReporteSuma()
        //                {
        //                    Afiliado = a,
        //                    fecha = m.batchtime == null ? DateTime.ParseExact(m.fecha.Substring(6, 2) + "/" + m.fecha.Substring(4, 2) + "/" + m.fecha.Substring(0, 4), "dd/MM/yyyy", CultureInfo.InvariantCulture) : DateTime.ParseExact(m.fecha.Substring(6, 2) + "/" + m.fecha.Substring(4, 2) + "/" + m.fecha.Substring(0, 4) + " " + m.batchtime.Substring(0, 2) + ":" + m.batchtime.Substring(2, 2) + ":" + m.batchtime.Substring(4, 2), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
        //                    monto = Convert.ToInt32(m.saldo),
        //                    detalle = m.isodescription,
        //                    tipo = m.transcode + "-" + m.transname,
        //                    numerotarjeta = Convert.ToDecimal(m.pan),
        //                    batchid = m.batchid.ToString(),
        //                    Encabezado = encabezado
        //                };
        //                reporte.Add(linea);
        //            }
        //        }
        //    }
        //    #endregion
        //    if (reporte.Count == 0)
        //    {
        //        ReporteSuma r = new ReporteSuma()
        //        {
        //            Encabezado = encabezado
        //        };
        //        reporte.Add(r);
        //    }
        //    return reporte.OrderBy(x => x.fecha).ToList();
        //}

        private int DeterminarSucursalAfiliacion()
        {
            string ip;
            ip = HttpContext.Current.Request.UserHostAddress;
            //ip = ip + HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            //ip = ip + HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];        
            if (ip.Contains("192.168.2"))
                return 1003;
            if (ip.Contains("192.168.3"))
                return 1006;
            if (ip.Contains("192.168.4"))
                return 1002;
            if (ip.Contains("192.168.5"))
                return 1005;
            if (ip.Contains("192.168.6"))
                return 1007;
            if (ip.Contains("192.168.8"))
                return 1008;
            if (ip.Contains("192.168.9"))
                return 1009;
            if (ip.Contains("192.168.10"))
                return 1010;
            if (ip.Contains("192.168.11"))
                return 1011;
            if (ip.Contains("192.168.12"))
                return 1012;
            if (ip.Contains("192.168.13"))
                return 1013;
            if (ip.Contains("192.168.14"))
                return 1014;
            if (ip.Contains("192.168.16"))
                return 1016;
            if (ip.Contains("192.168.17"))
                return 1017;
            if (ip.Contains("192.168.18"))
                return 1018;
            if (ip.Contains("192.168.19"))
                return 1019;
            if (ip.Contains("192.168.21"))
                return 1021;
            else
                return 1001;
        }

        public string VerificarNumeroDeDocumentoCrear(string numerodedocumento)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                var query = (from a in db.Affiliates
                             where a.docnumber.Substring(2) == numerodedocumento
                             select a.docnumber
                             ).ToList();
                if (query.Count > 0)
                {
                    return query.First();
                }
                var query2 = (from c in db.CLIENTES
                             where c.NRO_DOCUMENTO == numerodedocumento
                             select c.TIPO_DOCUMENTO+"-"+c.NRO_DOCUMENTO
                            ).ToList();
                if (query2.Count > 0)
                {
                    return query2.First();
                }
                return null;
            }           
        }

        public string VerificarNumeroDeDocumentoCambiarTipo(string numerodedocumento)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                var query = (from a in db.Affiliates
                             where a.docnumber == numerodedocumento
                             select a.docnumber
                             ).ToList();
                if (query.Count > 0)
                {
                    return query.First();
                }
                var query2 = (from c in db.CLIENTES
                              where c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO == numerodedocumento
                              select c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO
                            ).ToList();
                if (query2.Count > 0)
                {
                    return query2.First();
                }
                return null;
            }
        }

        public class customerInterest
        {
            public int customerID { get; set; }
            public int interestID { get; set; }
        }

        #region Lista_Intereses_Cliente
        public List<Interest> chargeInterestList()
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                return db.Interests.Where(x => x.active == true).ToList();
            }
        }

        public List<Interest> chargeInterestList(int customerID)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                List<customerInterest> records = (from i in db.CustomerInterests
                                                  where i.customerid == customerID
                                                  select new customerInterest()
                                                  {
                                                      customerID = i.customerid,
                                                      interestID = i.interestid
                                                  }).ToList();

                var lista = new List<Interest>();

                foreach (var items in db.Interests.Where(x => x.active == true))
                {

                    foreach (var customerItem in records)
                    {
                        if (items.id.Equals(customerItem.interestID))
                        {
                            items.Checked = true;
                            break;
                        }

                    }

                    lista.Add(items);
                }

                return lista;
            }
        }
        #endregion

        #region Funciones_Valores_Consecutivos
        private int AfilliatesID()
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                if (db.Affiliates.Count() == 0)
                    return 1;
                return (db.Affiliates.Max(a => a.id) + 1);
            }
        }

        private int AfilliateAudID()
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                if (db.AffiliateAuds.Count() == 0)
                    return 1;
                return (db.AffiliateAuds.Max(a => a.id) + 1);
            }
        }
        #endregion

        // Métodos DDL para Datos Geográficos.
        // Abstenerse a las consecuencias físicas, la persona que modifique éste código.

        #region Lista_de_Datos_Geograficos
        // retornar la lista de Estados.
        public List<ESTADO> GetEstados()
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                return db.ESTADOS.OrderBy(u => u.DESCRIPC_ESTADO).ToList();
            }
        }

        private string GetNombreEstado(string id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                if (db.ESTADOS.Find(id) == null)
                {
                    return "";
                }
                else
                {
                    return db.ESTADOS.Find(id).DESCRIPC_ESTADO;
                }
            }
        }

        // retornar la lista de Ciudades asociadas al campo clave de la entidad Estado.
        public List<CIUDAD> GetCiudades(string id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                var query = db.CIUDADES.Where(a => a.ESTADOS.Select(b => b.COD_ESTADO).Contains(id));
                return query.ToList(); //.ToArray();
            }
        }

        private string GetNombreCiudad(string id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                if (db.CIUDADES.Find(id) == null)
                {
                    return "";
                }
                else
                {
                    return db.CIUDADES.Find(id).DESCRIPC_CIUDAD;
                }
            }
        }

        // retornar la lista de Municipios asociadas al campo clave de la entidad Ciudad.
        public List<MUNICIPIO> GetMunicipios(string id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                var query = db.MUNICIPIOS.Where(a => a.CIUDADES.Select(b => b.COD_CIUDAD).Contains(id));
                return query.ToList();
            }
        }

        private string GetNombreMunicipio(string id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                if (db.MUNICIPIOS.Find(id) == null)
                {
                    return "";
                }
                else
                {
                    return db.MUNICIPIOS.Find(id).DESCRIPC_MUNICIPIO;
                }
            }
        }

        // retornar la lista de Parroquias asociadas al campo clave de la entidad Municipio.
        public List<PARROQUIA> GetParroquias(string id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                var query = db.PARROQUIAS.Where(a => a.MUNICIPIOS.Select(b => b.COD_MUNICIPIO).Contains(id));
                return query.ToList();
            }
        }

        private string GetNombreParroquia(string id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                if (db.PARROQUIAS.Find(id) == null)
                {
                    return "";
                }
                else
                {
                    return db.PARROQUIAS.Find(id).DESCRIPC_PARROQUIA;
                }
            }
        }

        // retornar la lista de Urbanizaciones asociadas al campo clave de la entidad Parroquia.
        public List<URBANIZACION> GetUrbanizaciones(string id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                var query = db.URBANIZACIONES.Where(a => a.PARROQUIAS.Select(b => b.COD_PARROQUIA).Contains(id));
                return query.ToList();
            }
        }

        private string GetNombreUrbanizacion(string id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                if (db.URBANIZACIONES.Find(id) == null)
                {
                    return "";
                }
                else
                {
                    return db.URBANIZACIONES.Find(id).DESCRIPC_URBANIZACION;
                }
            }
        }
        #endregion


    }
}