using LinqToExcel;
using Newtonsoft.Json;
using Suma2Lealtad.Modules;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Suma2Lealtad.Models
{
    public class AfiliadoRepository
    {
        //private int INITIAL_INTEGER_VALUE = 1;
        private const int ID_REASONS_INICIAL = 1;
        private const int ID_CORPORACION_PLAZAS = 1;
        private const int ID_ESTATUS_AFILIACION_INICIAL = 0;
        private const int ID_ESTATUS_AFILIACION_ACTIVA = 2;
        private const int ID_TYPE_SUMA = 1;
        private const int ID_COMPANY_PLAZAS = 1;
        private const int ID_AFILIADO_NO_CORRESPONDE = -1;
        //actualmente no hay donde guardar este valor en BD
        private const string WEB_TYPE = "1";
        //private const string ID_ESTATUS_TARJETA_NUEVA = "0";
        //private const string ID_ESTATUS_TARJETA_ACTIVA = "1";
        private const string ID_ESTATUS_TARJETA_SUSPENDIDA = "6";
        //private const string INITIAL_STRING_VALUE = "";

        //determina si hubo excepción en llamada a servicio Cards
        private bool ExceptionServicioCards(string RespuestaServicioCards)
        {
            try
            {
                ExceptionJSON exceptionJson = (ExceptionJSON)JsonConvert.DeserializeObject<ExceptionJSON>(RespuestaServicioCards);
                if (exceptionJson.code == "100")
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

        //determina si hubo excepción en llamada a servicio WebPlazas
        private bool ExceptionServicioWebPlazas(string RespuestaServicioWebPlazas)
        {
            try
            {
                ExceptionJSON exceptionJson = (ExceptionJSON)JsonConvert.DeserializeObject<ExceptionJSON>(RespuestaServicioWebPlazas);
                if (exceptionJson.code == "100")
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

        public class customerInterest
        {
            public int customerID { get; set; }
            public int interestID { get; set; }
        }

        #region Lista_Intereses_Cliente
        private List<Interest> chargeInterestList()
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                return db.Interests.Where(x => x.active == true).ToList();
            }
        }

        private List<Interest> chargeInterestList(int customerID)
        {
            using (LealtadEntities db = new LealtadEntities())
            {

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
                if (db.Affiliates.Count() == 0)
                    return 1;
                return (db.Affiliates.Max(a => a.id) + 1);
            }
        }

        private int AfilliateAudID()
        {
            using (LealtadEntities db = new LealtadEntities())
            {
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
        private List<ESTADO> GetEstados()
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                return db.ESTADOS.OrderBy(u => u.DESCRIPC_ESTADO).ToList();
            }
        }

        // retornar la lista de Ciudades asociadas al campo clave de la entidad Estado.
        public List<CIUDAD> GetCiudades(string id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                var query = db.CIUDADES.Where(a => a.ESTADOS.Select(b => b.COD_ESTADO).Contains(id));

                return query.ToList(); //.ToArray();
            }
        }

        // retornar la lista de Municipios asociadas al campo clave de la entidad Ciudad.
        public List<MUNICIPIO> GetMunicipios(string id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                var query = db.MUNICIPIOS.Where(a => a.CIUDADES.Select(b => b.COD_CIUDAD).Contains(id));

                return query.ToList();
            }
        }

        // retornar la lista de Parroquias asociadas al campo clave de la entidad Municipio.
        public List<PARROQUIA> GetParroquias(string id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                var query = db.PARROQUIAS.Where(a => a.MUNICIPIOS.Select(b => b.COD_MUNICIPIO).Contains(id));

                return query.ToList();
            }
        }

        // retornar la lista de Urbanizaciones asociadas al campo clave de la entidad Parroquia.
        public List<URBANIZACION> GetUrbanizaciones(string id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                var query = db.URBANIZACIONES.Where(a => a.PARROQUIAS.Select(b => b.COD_PARROQUIA).Contains(id));

                return query.ToList();
            }
        }
        #endregion

        //retorna el ojeto Photos_Affiliate a partr del id del afiliado
        private Photos_Affiliate GetPhoto(int idAfiliado)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                Photos_Affiliate photo_affiliate = db.Photos_Affiliates.FirstOrDefault(p => p.Affiliate_id == idAfiliado);
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

        //busca un cliente en la WebPlazas a partir del documento de identificación
        public Afiliado Find(string numdoc, int typeid = ID_TYPE_SUMA, int companyid = ID_COMPANY_PLAZAS)
        {
            //Primero se buscan los datos de CLIENTE en WebPlazas
            //SERVICIO WSL.WebPlazas.getClientByNumDoc 
            Afiliado afiliado = new Afiliado()
            {
                docnumber = numdoc
            };
            string clienteWebPlazasJson = WSL.WebPlazas.getClientByNumDoc(afiliado.docnumber);
            if (ExceptionServicioWebPlazas(clienteWebPlazasJson))
            {
                return null;
            }
            ClienteWebPlazas clienteWebPlazas = (ClienteWebPlazas)JsonConvert.DeserializeObject<ClienteWebPlazas>(clienteWebPlazasJson);
            if (clienteWebPlazas == null)
            {
                //No está en WebPlazas
                afiliado.clientid = 0;
            }
            else
            {
                afiliado.nationality = clienteWebPlazas.nationality.Replace("/","").Replace("\\","");
                afiliado.name = clienteWebPlazas.name.Replace("/", "").Replace("\\", "");
                afiliado.name2 = clienteWebPlazas.name2.Replace("/", "").Replace("\\", "");
                afiliado.lastname1 = clienteWebPlazas.lastname1.Replace("/", "").Replace("\\", "");
                afiliado.lastname2 = clienteWebPlazas.lastname2.Replace("/", "").Replace("\\", "");
                afiliado.birthdate = clienteWebPlazas.birthdate.Value.ToString("dd-MM-yyyy");
                afiliado.gender = clienteWebPlazas.gender.Replace("/", "").Replace("\\", "");
                afiliado.clientid = clienteWebPlazas.id;
                afiliado.maritalstatus = clienteWebPlazas.maritalstatus.Replace("/", "").Replace("\\", "");
                afiliado.occupation = clienteWebPlazas.occupation.Replace("/", "").Replace("\\", "");
                afiliado.phone1 = clienteWebPlazas.phone1.Replace("/", "").Replace("\\", "");
                afiliado.phone2 = clienteWebPlazas.phone2.Replace("/", "").Replace("\\", "");
                afiliado.phone3 = clienteWebPlazas.phone3.Replace("/", "").Replace("\\", "");
                afiliado.email = clienteWebPlazas.email.Replace("/", "").Replace("\\", "");
                afiliado.WebType = clienteWebPlazas.type;
                afiliado.ListaEstados = GetEstados();       //++
            }
            //Segundo se buscan los datos del AFILIADO en SumaPlazas
            using (LealtadEntities db = new LealtadEntities())
            {
                //Entidad Affiliate                
                afiliado.id = (from a in db.Affiliates
                               where a.docnumber.Equals(afiliado.docnumber)
                               select a.id
                               ).SingleOrDefault();
                if (afiliado.id == 0)
                {
                    //No está en SumaPlazas
                    afiliado.Intereses = chargeInterestList();
                    afiliado.typeid = typeid;
                    afiliado.type = db.Types.FirstOrDefault(t => t.id == afiliado.typeid).name;
                    afiliado.companyid = companyid;
                }
                else
                {
                    //Esta en Suma, busco compañia asociada
                    //Entidad CompanyAffiliate
                    afiliado.companyid = (from co in db.CompanyAffiliates
                                          where co.affiliateid.Equals(afiliado.id)
                                          select co.companyid
                                          ).SingleOrDefault();
                    //ENTIDAD TYPE 
                    //afiliado.typeid = TipoAfiliacionUsuario((int)HttpContext.Current.Session["userid"]);
                    afiliado.typeid = (from a in db.Affiliates
                                       where a.id.Equals(afiliado.id)
                                       select a.typeid
                                       ).SingleOrDefault();
                    afiliado.type = db.Types.FirstOrDefault(t => t.id == afiliado.typeid).name;
                    if (afiliado.typeid != typeid)
                    {
                        //EL AFILIADO EXISTE Y NO PERTENECE AL TIPO DE AFILIACION SUMINISTRADO
                        afiliado.id = -1;
                    }
                    else if (afiliado.companyid != companyid)
                    {
                        //EL AFILIADO EXISTE Y NO PERTENECE A LA COMPAÑIA SUMINISTRADA
                        afiliado.id = -2;
                    }
                    else
                    {
                        //AFILIADO EXISTE Y PERTENECE AL TIPO DE AFILIACION Y A LA COMPAÑIA SUMINISTRADOS, se busca lo necesario para llenar el index de editar
                        //afiliado.statusid = (from a in db.Affiliates
                        //                     where a.docnumber.Equals(afiliado.docnumber)
                        //                     select a.statusid
                        //                     ).SingleOrDefault();
                        afiliado.statusid = db.Affiliates.FirstOrDefault(a => a.docnumber == afiliado.docnumber).statusid;
                        //afiliado.estatus = (from s in db.Status
                        //                    where s.id.Equals(afiliado.statusid)
                        //                    select s.name
                        //                    ).SingleOrDefault();
                        afiliado.estatus = db.Statuses.FirstOrDefault(s => s.id == afiliado.statusid).name;
                    }
                }
            }
            return afiliado;
        }

        //busca una lista de afiliados CON UN SUBCONJUNTO DE ATRIBUTOS en SumaPLazas a partir del documento de identifación, el nombre o el email
        public List<Afiliado> Find(string numdoc, string name, string email, int typeid = ID_TYPE_SUMA, int companyid = ID_COMPANY_PLAZAS)
        {
            List<Afiliado> afiliados;
            using (LealtadEntities db = new LealtadEntities())
            {
                if (name == "")
                {
                    name = null;
                }
                if (email == "")
                {
                    email = null;
                }
                if (name == null && email == null)
                {
                    //Primero se buscan los datos de AFILIADO en SumaPlazas
                    //ENTIDAD Affiliado
                    //ENTIDAD CLIENTE
                    //ENTIDAD Status
                    //ENTIDAD CompanyAffiliates
                    afiliados = (from a in db.Affiliates
                                 join c in db.CLIENTES on a.docnumber equals c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO
                                 join s in db.Statuses on a.statusid equals s.id
                                 join t in db.Types on a.typeid equals t.id
                                 join co in db.CompanyAffiliates on a.id equals co.affiliateid
                                 where a.docnumber.Equals(numdoc) && co.companyid.Equals(companyid) && a.typeid.Equals(typeid)
                                 select new Afiliado()
                                 {
                                     //ENTIDAD Affiliate 
                                     id = a.id,
                                     docnumber = a.docnumber,
                                     //ENTIDAD CLIENTE
                                     name = c.NOMBRE_CLIENTE1,
                                     lastname1 = c.APELLIDO_CLIENTE1,
                                     email = c.E_MAIL,
                                     //ENTIDAD Status
                                     estatus = s.name,
                                     //ENTIDAD Type
                                     type = t.name
                                 }).ToList();
                }
                else
                {
                    //Primero se buscan los datos de AFILIADO en SumaPlazas
                    //ENTIDAD Affiliado
                    //ENTIDAD CLIENTE
                    //ENTIDAD Status
                    afiliados = (from a in db.Affiliates
                                 join c in db.CLIENTES on a.docnumber equals c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO
                                 join s in db.Statuses on a.statusid equals s.id
                                 join t in db.Types on a.typeid equals t.id
                                 join co in db.CompanyAffiliates on a.id equals co.affiliateid
                                 where (a.docnumber.Equals(numdoc) || c.E_MAIL == email || c.NOMBRE_CLIENTE1.Contains(name) || c.APELLIDO_CLIENTE1.Contains(name)) && co.companyid.Equals(companyid) && a.typeid.Equals(typeid)
                                 select new Afiliado()
                                 {
                                     //ENTIDAD Affiliate 
                                     id = a.id,
                                     docnumber = a.docnumber,
                                     //ENTIDAD CLIENTE
                                     name = c.NOMBRE_CLIENTE1,
                                     lastname1 = c.APELLIDO_CLIENTE1,
                                     email = c.E_MAIL,
                                     //ENTIDAD Status
                                     estatus = s.name,
                                     //ENTIDAD Type
                                     type = t.name
                                 }).ToList();
                }
                if (afiliados != null)
                {
                    foreach (var afiliado in afiliados)
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
                    }
                }
                else
                {
                    //no se encontro coincidencia
                }
                return afiliados;
            }
        }

        //busca un afiliado CON TODOS SUS ATRIBUTOS en SumaPlazas a partir del id
        public Afiliado Find(int id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                //Primero se buscan los datos de AFILIADO en SumaPlazas
                //ENTIDAD Affiliado
                //ENTIDAD CLIENTE
                //ENTIDAD Status
                //ENTIDAD CustomerInterest
                Afiliado afiliado = (from a in db.Affiliates
                                     join c in db.CLIENTES on a.docnumber equals c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO
                                     join s in db.Statuses on a.statusid equals s.id
                                     join t in db.Types on a.typeid equals t.id
                                     join co in db.CompanyAffiliates on a.id equals co.affiliateid
                                     where a.id.Equals(id)
                                     select new Afiliado()
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
                                         statusid = a.statusid,
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
                                         //ENTIDAD Status
                                         estatus = s.name,
                                         //ENTIDAD Type
                                         type = t.name,
                                         //ENTIDAD CompanyAffiliate
                                         companyid = co.companyid
                                     }).Single();
                DateTime? d = (from c in db.CLIENTES
                               where (c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO).Equals(afiliado.docnumber)
                               select c.FECHA_NACIMIENTO
                               ).SingleOrDefault();
                afiliado.birthdate = d.Value.ToString("dd-MM-yyyy");
                //ENTIDAD CustomerInterest
                afiliado.Intereses = chargeInterestList(afiliado.id);
                //Llenar las listas de Datos Geográficos.
                afiliado.ListaEstados = GetEstados();
                afiliado.ListaCiudades = GetCiudades(afiliado.cod_estado);
                afiliado.ListaMunicipios = GetMunicipios(afiliado.cod_ciudad);
                afiliado.ListaParroquias = GetParroquias(afiliado.cod_municipio);
                afiliado.ListaUrbanizaciones = GetUrbanizaciones(afiliado.cod_parroquia);
                //ENTIDAD TARJETA
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
                //FILEYSTEM ~/Picture/@filename@.jpg
                //afiliado.picture = AppModule.GetPathPicture().Replace("@filename@", afiliado.docnumber);
                //ENTIDAD Photos_Affiliate 
                afiliado.picture = GetPhoto(afiliado.id);
                //POR AHORA NO HAY COLUMNA EN NINGUNA ENTIDAD PARA ALMACENAR ESTE DATO QUE VIENE DE LA WEB
                if (afiliado.WebType == null)
                {
                    afiliado.WebType = WEB_TYPE;
                }
                return afiliado;
            }
        }

        //actualiza los datos en WebPlazas
        private bool SaveWebPlazas(Afiliado afiliado)
        {
            string RespuestaWebPlazasJson = WSL.WebPlazas.UpdateClient(afiliado);
            if (ExceptionServicioWebPlazas(RespuestaWebPlazasJson))
            {
                return false;
            }
            RespuestaWebPlazas RespuestaWebPlazas = (RespuestaWebPlazas)JsonConvert.DeserializeObject<RespuestaWebPlazas>(RespuestaWebPlazasJson);
            return (RespuestaWebPlazas.excode == "0");
        }

        //crea el afiliado en SumaPlazas (solicitud de afiliación)
        public bool Save(Afiliado afiliado, HttpPostedFileBase file)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
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
                    statusid = db.SumaStatuses.FirstOrDefault(s => (s.value == ID_ESTATUS_AFILIACION_INICIAL) && (s.tablename=="Affiliatte")).id,//ID_ESTATUS_AFILIACION_INICIAL,
                    reasonsid = null,
                    twitter_account = afiliado.twitter_account,
                    facebook_account = afiliado.facebook_account,
                    instagram_account = afiliado.instagram_account,
                    comments = afiliado.comments
                };
                db.Affiliates.Add(Affiliate);
                //ENTIDAD CLIENTE
                var CLIENTE = new CLIENTE()
                {
                    TIPO_DOCUMENTO = afiliado.docnumber.Substring(0, 1),
                    NRO_DOCUMENTO = afiliado.docnumber.Substring(2),
                    NACIONALIDAD = afiliado.nationality,
                    NOMBRE_CLIENTE1 = afiliado.name,
                    NOMBRE_CLIENTE2 = afiliado.name2 == null ? "" : afiliado.name2,
                    APELLIDO_CLIENTE1 = afiliado.lastname1,
                    APELLIDO_CLIENTE2 = afiliado.lastname2 == null ? "" : afiliado.lastname2,
                    FECHA_NACIMIENTO = DateTime.ParseExact(afiliado.birthdate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                    SEXO = afiliado.gender,
                    EDO_CIVIL = afiliado.maritalstatus,
                    OCUPACION = afiliado.occupation == null ? "" : afiliado.occupation,
                    TELEFONO_HAB = afiliado.phone1,
                    TELEFONO_OFIC = afiliado.phone2 == null ? "" : afiliado.cod_estado,
                    TELEFONO_CEL = afiliado.phone3 == null ? "" : afiliado.cod_estado,
                    E_MAIL = afiliado.email,
                    COD_SUCURSAL = afiliado.storeid,
                    COD_ESTADO = afiliado.cod_estado,
                    COD_CIUDAD = afiliado.cod_ciudad,
                    COD_MUNICIPIO = afiliado.cod_municipio,
                    COD_PARROQUIA = afiliado.cod_parroquia,
                    COD_URBANIZACION = afiliado.cod_urbanizacion,
                    FECHA_CREACION = DateTime.Now
                };
                db.CLIENTES.Add(CLIENTE);
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
                else
                {
                    return false;
                }
                //ENTIDAD CompanyAffiliate
                var companyaffiliate = new CompanyAffiliate()
                {
                    affiliateid = Affiliate.id,
                    companyid = afiliado.companyid,
                    begindate = DateTime.Now,
                    enddate = new DateTime(),
                    comments = afiliado.comments,
                    active = true
                };
                db.CompanyAffiliates.Add(companyaffiliate);
                //ENTIDAD AffiliateAud
                var affiliateauditoria = new AffiliateAud()
                {
                    id = AfilliateAudID(),
                    affiliateid = Affiliate.id,
                    modifieduserid = (int)HttpContext.Current.Session["userid"],
                    modifieddate = System.DateTime.Now,
                    statusid = Affiliate.statusid,
                    reasonsid = ID_REASONS_INICIAL,
                    comments = afiliado.comments
                };
                db.AffiliateAuds.Add(affiliateauditoria);
                if (SaveWebPlazas(afiliado))
                {
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        //actualiza los datos en SumaPlazasStatuses
        public bool SaveChanges(Afiliado afiliado)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
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
                    affiliate.statusid = afiliado.statusid;
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
                    cliente.NACIONALIDAD = afiliado.nationality;
                    cliente.NOMBRE_CLIENTE1 = afiliado.name;
                    cliente.NOMBRE_CLIENTE2 = afiliado.name2;
                    cliente.APELLIDO_CLIENTE1 = afiliado.lastname1;
                    cliente.APELLIDO_CLIENTE2 = afiliado.lastname2;
                    cliente.FECHA_NACIMIENTO = DateTime.ParseExact(afiliado.birthdate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    cliente.SEXO = afiliado.gender;
                    cliente.EDO_CIVIL = afiliado.maritalstatus;
                    cliente.OCUPACION = afiliado.occupation;
                    cliente.TELEFONO_HAB = afiliado.phone1;
                    cliente.TELEFONO_OFIC = afiliado.phone2;
                    cliente.TELEFONO_CEL = afiliado.phone3;
                    cliente.COD_SUCURSAL = afiliado.storeid;
                    cliente.COD_ESTADO = afiliado.cod_estado;
                    cliente.COD_CIUDAD = afiliado.cod_ciudad;
                    cliente.COD_MUNICIPIO = afiliado.cod_municipio;
                    cliente.COD_PARROQUIA = afiliado.cod_parroquia;
                    cliente.COD_URBANIZACION = afiliado.cod_urbanizacion;
                }
                // Entida: TARJETA
                TARJETA tarjeta = db.TARJETAS.FirstOrDefault(t => t.NRO_AFILIACION.Equals(afiliado.id));
                if (tarjeta != null)
                {
                    tarjeta.ESTATUS_TARJETA = afiliado.estatustarjeta;
                    tarjeta.COD_USUARIO = (int)HttpContext.Current.Session["userid"];
                    tarjeta.FECHA_CREACION = DateTime.Now;
                }
                else if (afiliado.pan != null && afiliado.estatustarjeta != null)
                {
                    tarjeta = new TARJETA()
                    {
                        NRO_TARJETA = Convert.ToDecimal(afiliado.pan),
                        NRO_AFILIACION = afiliado.id,
                        TIPO_DOCUMENTO = afiliado.docnumber.Substring(0, 1),
                        NRO_DOCUMENTO = afiliado.docnumber.Substring(2),
                        ESTATUS_TARJETA = afiliado.estatustarjeta,
                        SALDO_PUNTOS = null,
                        OBSERVACIONES = null,
                        COD_USUARIO = (int)HttpContext.Current.Session["userid"],
                        TRACK1 = null,
                        TRACK2 = null,
                        CVV2 = null,
                        FECHA_CREACION = DateTime.Now
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
                int statusidactual = (from a in db.Affiliates
                                      where a.id.Equals(afiliado.id)
                                      select a.statusid
                                       ).SingleOrDefault();
                //Solo inserto registros cuando hay cambio de estado de Afiliación
                if (statusidactual != afiliado.statusid)
                {
                    var affiliateAuditoria = new AffiliateAud()
                    {
                        id = AfilliateAudID(),
                        affiliateid = afiliado.id,
                        modifieduserid = (int)HttpContext.Current.Session["userid"],
                        modifieddate = System.DateTime.Now,
                        statusid = afiliado.statusid,
                        reasonsid = ID_REASONS_INICIAL,
                        comments = afiliado.comments
                    };
                    db.AffiliateAuds.Add(affiliateAuditoria);
                }
                if (SaveWebPlazas(afiliado))
                {
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        //aprueba una solicitud de afiliación
        public bool Aprobar(Afiliado afiliado)
        {
            string RespuestaCardsJson = WSL.Cards.addClient(afiliado.docnumber.Substring(2), (afiliado.name + " " + afiliado.lastname1).ToUpper(), afiliado.phone1, "Plazas Baruta");
            if (ExceptionServicioCards(RespuestaCardsJson))
            {
                return false;
            }
            RespuestaCards RespuestaCards = (RespuestaCards)JsonConvert.DeserializeObject<RespuestaCards>(RespuestaCardsJson);
            using (LealtadEntities db = new LealtadEntities())
            {
                if (RespuestaCards.excode == "0" || RespuestaCards.excode == "7")
                {
                    //Se buscan los datos de Tarjeta del AFILIADO en Cards
                    //SERVICIO WSL.Cards.getClient !
                    string clienteCardsJson = WSL.Cards.getClient(afiliado.docnumber.Substring(2));
                    if (ExceptionServicioCards(clienteCardsJson))
                    {
                        return false;
                    }
                    ClienteCards clienteCards = (ClienteCards)JsonConvert.DeserializeObject<ClienteCards>(clienteCardsJson);
                    afiliado.pan = clienteCards.pan;
                    afiliado.printed = clienteCards.printed;
                    afiliado.estatustarjeta = clienteCards.tarjeta;
                    afiliado.statusid = db.SumaStatuses.FirstOrDefault(s => (s.value == ID_ESTATUS_AFILIACION_ACTIVA) && (s.tablename == "Affiliatte")).id;//ID_ESTATUS_AFILIACION_ACTIVA;
                    return SaveChanges(afiliado);
                }
                else
                {
                    return false;
                }
            }
        }

        //elimina una tarjeta de SumaPlazas
        private bool BorrarTarjeta(string pan)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
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

        //busca los saldos y movimientos de las cuentas de un afiliado
        public SaldosMovimientos FindSaldosMovimientos(Afiliado afiliado)
        {
            SaldosMovimientos SaldosMovimientos = new SaldosMovimientos();
            SaldosMovimientos.DocId = afiliado.docnumber;
            string saldosJson = WSL.Cards.getBalance(SaldosMovimientos.DocId.Substring(2));
            if (ExceptionServicioCards(saldosJson))
            {
                return null;
            }
            SaldosMovimientos.Saldos = (List<Saldo>)JsonConvert.DeserializeObject<List<Saldo>>(saldosJson);
            string movimientosPrepagoJson = WSL.Cards.getBatch(SaldosMovimientos.Saldos.First().accounttype, SaldosMovimientos.DocId.Substring(2));
            if (ExceptionServicioCards(movimientosPrepagoJson))
            {
                return null;
            }
            SaldosMovimientos.MovimientosPrepago = (List<Movimiento>)JsonConvert.DeserializeObject<List<Movimiento>>(movimientosPrepagoJson);
            List<Movimiento> MovimientosPrepagoOrdenados = SaldosMovimientos.MovimientosPrepago.OrderByDescending(x => x.batchid).ToList();
            SaldosMovimientos.MovimientosPrepago = MovimientosPrepagoOrdenados.Take(3).ToList();
            foreach (Movimiento mov in SaldosMovimientos.MovimientosPrepago)
            {
                mov.fecha = mov.fecha.Substring(6, 2) + "-" + mov.fecha.Substring(4, 2) + "-" + mov.fecha.Substring(0, 4);
            }
            string movimientosLealtadJson = WSL.Cards.getBatch(SaldosMovimientos.Saldos.Skip(1).First().accounttype, SaldosMovimientos.DocId.Substring(2));
            if (ExceptionServicioCards(movimientosLealtadJson))
            {
                return null;
            }
            SaldosMovimientos.MovimientosSuma = (List<Movimiento>)JsonConvert.DeserializeObject<List<Movimiento>>(movimientosLealtadJson);
            List<Movimiento> MovimientosSumaOrdenados = SaldosMovimientos.MovimientosSuma.OrderByDescending(x => x.batchid).ToList();
            SaldosMovimientos.MovimientosSuma = MovimientosSumaOrdenados.Take(3).ToList();
            foreach (Movimiento mov in SaldosMovimientos.MovimientosSuma)
            {
                mov.fecha = mov.fecha.Substring(6, 2) + "-" + mov.fecha.Substring(4, 2) + "-" + mov.fecha.Substring(0, 4);
            }
            return SaldosMovimientos;
        }

        //acredita más en la cuenta del afiliado
        public bool Acreditar(Afiliado afiliado, string monto)
        {
            string RespuestaCardsJson = WSL.Cards.addBatch(afiliado.docnumber.Substring(2), monto, "318", (string)HttpContext.Current.Session["login"]);
            if (ExceptionServicioCards(RespuestaCardsJson))
            {
                return false;
            }
            RespuestaCards RespuestaCards = (RespuestaCards)JsonConvert.DeserializeObject<RespuestaCards>(RespuestaCardsJson);
            if (RespuestaCards.excode == "0")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //bloquea la tarjeta actual y genera una nueva tarjeta para el afiliado
        public bool BloquearTarjeta(Afiliado afiliado)
        {
            string RespuestaCardsJson = WSL.Cards.addCard(afiliado.docnumber.Substring(2));
            if (ExceptionServicioCards(RespuestaCardsJson))
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
                    if (ExceptionServicioCards(clienteCardsJson))
                    {
                        return false;
                    }
                    ClienteCards clienteCards = (ClienteCards)JsonConvert.DeserializeObject<ClienteCards>(clienteCardsJson);
                    afiliado.pan = clienteCards.pan;
                    afiliado.printed = clienteCards.printed;
                    afiliado.estatustarjeta = clienteCards.tarjeta;
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

        //suspende temporalmente la tarjeta del afiliado
        public bool SuspenderTarjeta(Afiliado afiliado)
        {
            string RespuestaCardsJson = WSL.Cards.cardStatus(afiliado.docnumber.Substring(2), ID_ESTATUS_TARJETA_SUSPENDIDA);
            if (ExceptionServicioCards(RespuestaCardsJson))
            {
                return false;
            }
            RespuestaCards RespuestaCards = (RespuestaCards)JsonConvert.DeserializeObject<RespuestaCards>(RespuestaCardsJson);
            if (RespuestaCards.excode == "0")
            {
                //Se buscan los datos de Tarjeta del AFILIADO en Cards
                //SERVICIO WSL.Cards.getClient !
                string clienteCardsJson = WSL.Cards.getClient(afiliado.docnumber.Substring(2));
                if (ExceptionServicioCards(clienteCardsJson))
                {
                    return false;
                }
                ClienteCards clienteCards = (ClienteCards)JsonConvert.DeserializeObject<ClienteCards>(clienteCardsJson);
                afiliado.pan = clienteCards.pan;
                afiliado.printed = clienteCards.printed;
                afiliado.estatustarjeta = clienteCards.tarjeta;
                return SaveChanges(afiliado);
            }
            else
            {
                return false;
            }
        }

        //reactiva la tarjeta del afiliado suspendida temporalmente
        public bool ReactivarTarjeta(Afiliado afiliado)
        {
            string RespuestaCardsJson = WSL.Cards.cardActive(afiliado.docnumber.Substring(2));
            if (ExceptionServicioCards(RespuestaCardsJson))
            {
                return false;
            }
            RespuestaCards RespuestaCards = (RespuestaCards)JsonConvert.DeserializeObject<RespuestaCards>(RespuestaCardsJson);
            if (RespuestaCards.excode == "0")
            {
                //Se buscan los datos de Tarjeta del AFILIADO en Cards
                //SERVICIO WSL.Cards.getClient !
                string clienteCardsJson = WSL.Cards.getClient(afiliado.docnumber.Substring(2));
                if (ExceptionServicioCards(clienteCardsJson))
                {
                    return false;
                }
                ClienteCards clienteCards = (ClienteCards)JsonConvert.DeserializeObject<ClienteCards>(clienteCardsJson);
                afiliado.pan = clienteCards.pan;
                afiliado.printed = clienteCards.printed;
                afiliado.estatustarjeta = clienteCards.tarjeta;
                return SaveChanges(afiliado);
            }
            else
            {
                return false;
            }
        }

        //imprime y activa la tarjeta del afiliado
        public bool ImprimirTarjeta(Afiliado afiliado)
        {
            string RespuestaCardsJson = WSL.Cards.cardActive(afiliado.docnumber.Substring(2));
            if (ExceptionServicioCards(RespuestaCardsJson))
            {
                return false;
            }
            RespuestaCards RespuestaCards = (RespuestaCards)JsonConvert.DeserializeObject<RespuestaCards>(RespuestaCardsJson);
            if (RespuestaCards.excode == "0")
            {
                RespuestaCardsJson = WSL.Cards.cardPrint(afiliado.docnumber.Substring(2));
                if (ExceptionServicioCards(RespuestaCardsJson))
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
                if (ExceptionServicioCards(clienteCardsJson))
                {
                    return false;
                }
                ClienteCards clienteCards = (ClienteCards)JsonConvert.DeserializeObject<ClienteCards>(clienteCardsJson);
                afiliado.pan = clienteCards.pan;
                afiliado.printed = clienteCards.printed;
                afiliado.estatustarjeta = clienteCards.tarjeta;
                return SaveChanges(afiliado);
            }
            else
            {
                return false;
            }
        }

        public List<Afiliado> GetBeneficiarios(string fichero)
        {
            List<Afiliado> resultado = ToListaAfiliado(fichero);

            if (resultado != null)
            {
                foreach (var item in resultado)
                {
                    if (item.docnumber == null || Regex.IsMatch(item.docnumber, @"^[JGVE][-][0-9]{8}[-][0-9]$") || Regex.IsMatch(item.Monto.ToString(), @"^[JGVE][-][0-9]{8}[-][0-9]$"))
                    {
                        return null;
                    }
                }

                return resultado.Where(r => r.Monto > 0).ToList();
            }

            return null;

        }

        private List<Afiliado> ToListaAfiliado(string pathDelFicheroExcel)
        {
            //Filter:("*.xls;*.xlsx)|*.xls;*.xlsx"); //le indicamos el tipo de filtro en este caso que busque solo los archivos excel
            try
            {

                var book = new ExcelQueryFactory(pathDelFicheroExcel);
                var resultado = (from row in book.Worksheet("Hoja1")
                                 let item = new Afiliado
                                 {
                                     docnumber = row["Cedula"].Cast<string>(),
                                     Monto = int.Parse(row["Monto"].Cast<string>())
                                 }
                                 select item).ToList();
                book.Dispose();
                return resultado.ToList();
            }
            catch
            {
                return null;
            }
        }

    }
}
