using Newtonsoft.Json;
using Suma2Lealtad.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Suma2Lealtad.Models
{
    public class CompanyRepository
    {
        private const int ID_TYPE_PREPAGO = 2;
        private const int ESTATUS_ORDEN_RECARGA_NUEVA = 1;
        private const int ESTATUS_ORDEN_RECARGA_APROBADA = 2;
        private const int ESTATUS_ORDEN_RECARGA_RECHAZADA = 3;
        private const int ESTATUS_ORDEN_RECARGA_EFECTIVA = 4;
        private const int ESTATUS_DETALLEORDEN_RECARGA_NUEVA = 1;
        private const int ESTATUS_DETALLEORDEN_RECARGA_APROBADA = 2;
        private const int ESTATUS_DETALLEORDEN_RECARGA_EFECTIVA = 3;
        private const int ESTATUS_DETALLEORDEN_RECARGA_FALLIDA = 4;
        private const string TRANS_CODE_RECARGA = "200";

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

        #region SequenceID
        private int CompanyID()
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                if (db.Companies.Count() == 0)
                    return 1;
                return (db.Companies.Max(c => c.id) + 1);
            }
        }

        private int OrderID()
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                if (db.Orders.Count() == 0)
                    return 1;
                return (db.Orders.Max(c => c.id) + 1);
            }
        }

        private int OrderDetailID()
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                if (db.OrdersDetails.Count() == 0)
                    return 0;
                return (db.OrdersDetails.Max(c => c.id));
            }
        }
        #endregion

        //busca una lista de compañias en SumaPLazas a partir del documento de identificación o el nombre
        public List<PrepagoCompanyAffiliattes> Find(string numdoc, string name = "")
        {
            List<PrepagoCompanyAffiliattes> compañias;
            using (LealtadEntities db = new LealtadEntities())
            {
                if (name == "")
                {
                    name = null;
                }
                compañias = (from c in db.Companies
                             where c.rif.Equals(numdoc) || c.name.Contains(name)
                             select new PrepagoCompanyAffiliattes()
                             {
                                 companyid = c.id,
                                 namecompañia = c.name,
                                 alias = c.ALIAS,
                                 rif = c.rif,
                                 address = c.address,
                                 phone = c.phone,
                                 email = c.email
                             }).ToList();
            }
            return compañias;
        }

        //busca sólo los datos de la compañia a partir de su id
        public Company FindCompany(int id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                return db.Companies.Find(id);
            }
        }

        //busca una compañia CON TODOS SUS BENEFICIARIOS en SumaPlazas a partir del id
        public PrepagoCompanyAffiliattes Find(int companyid)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                PrepagoCompanyAffiliattes compañiaBeneficiarios = (from c in db.Companies
                                                                   where c.id.Equals(companyid)
                                                                   select new PrepagoCompanyAffiliattes()
                                                                   {
                                                                       companyid = c.id,
                                                                       namecompañia = c.name,
                                                                       alias = c.ALIAS,
                                                                       rif = c.rif,
                                                                       address = c.address,
                                                                       phone = c.phone,
                                                                       email = c.email
                                                                   }).Single();
                compañiaBeneficiarios.Beneficiarios = (from a in db.Affiliates
                                                       join c in db.CLIENTES on a.docnumber equals c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO
                                                       join s in db.SumaStatuses on a.statusid equals s.id
                                                       join t in db.Types on a.typeid equals t.id
                                                       join b in db.CompanyAffiliates on a.id equals b.affiliateid
                                                       where b.companyid.Equals(companyid) && a.typeid.Equals(ID_TYPE_PREPAGO)
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
                if (compañiaBeneficiarios.Beneficiarios != null)
                {
                    foreach (var beneficiario in compañiaBeneficiarios.Beneficiarios)
                    {
                        Decimal p = (from t in db.TARJETAS
                                     where t.NRO_AFILIACION.Equals(beneficiario.id)
                                     select t.NRO_TARJETA
                                     ).SingleOrDefault();
                        if (p != 0)
                        {
                            beneficiario.pan = p.ToString();
                        }
                        else
                        {
                            beneficiario.pan = "";
                        }
                        string e = (from t in db.TARJETAS
                                    where t.NRO_AFILIACION.Equals(beneficiario.id)
                                    select t.ESTATUS_TARJETA
                                    ).SingleOrDefault();
                        if (e != null)
                        {
                            beneficiario.estatustarjeta = e.ToString();
                        }
                        else
                        {
                            beneficiario.estatustarjeta = "";
                        }
                    }
                }
                return compañiaBeneficiarios;
            }
        }

        //busca los beneficiarios por cedula o nombre para una compañia con id 
        public PrepagoCompanyAffiliattes FindBeneficiarios(int companyid, string numdoc, string name, string email)
        {
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
                PrepagoCompanyAffiliattes compañiaBeneficiarios = (from c in db.Companies
                                                                   where c.id.Equals(companyid)
                                                                   select new PrepagoCompanyAffiliattes()
                                                                   {
                                                                       companyid = c.id,
                                                                       namecompañia = c.name,
                                                                       alias = c.ALIAS,
                                                                       rif = c.rif,
                                                                       address = c.address,
                                                                       phone = c.phone,
                                                                       email = c.email
                                                                   }).Single();
                if (name == null && email == null)
                {
                    compañiaBeneficiarios.Beneficiarios = (from a in db.Affiliates
                                                           join c in db.CLIENTES on a.docnumber equals c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO
                                                           join s in db.SumaStatuses on a.statusid equals s.id
                                                           join t in db.Types on a.typeid equals t.id
                                                           join b in db.CompanyAffiliates on a.id equals b.affiliateid
                                                           where b.companyid.Equals(companyid) && a.typeid.Equals(ID_TYPE_PREPAGO) && a.docnumber.Equals(numdoc)
                                                           select new Afiliado()
                                                           {
                                                               //ENTIDAD Affiliate 
                                                               id = a.id,
                                                               docnumber = a.docnumber,
                                                               typeid = a.typeid,
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
                    compañiaBeneficiarios.Beneficiarios = (from a in db.Affiliates
                                                           join c in db.CLIENTES on a.docnumber equals c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO
                                                           join s in db.SumaStatuses on a.statusid equals s.id
                                                           join t in db.Types on a.typeid equals t.id
                                                           join b in db.CompanyAffiliates on a.id equals b.affiliateid
                                                           where b.companyid.Equals(companyid) && a.typeid.Equals(ID_TYPE_PREPAGO) && (c.E_MAIL == email || c.NOMBRE_CLIENTE1.Contains(name) || c.APELLIDO_CLIENTE1.Contains(name))
                                                           select new Afiliado()
                                                           {
                                                               //ENTIDAD Affiliate 
                                                               id = a.id,
                                                               docnumber = a.docnumber,
                                                               typeid = a.typeid,
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
                if (compañiaBeneficiarios.Beneficiarios != null)
                {
                    foreach (var beneficiario in compañiaBeneficiarios.Beneficiarios)
                    {
                        Decimal p = (from t in db.TARJETAS
                                     where t.NRO_AFILIACION.Equals(beneficiario.id)
                                     select t.NRO_TARJETA
                                     ).SingleOrDefault();
                        if (p != 0)
                        {
                            beneficiario.pan = p.ToString();
                        }
                        else
                        {
                            beneficiario.pan = "";
                        }
                        string e = (from t in db.TARJETAS
                                    where t.NRO_AFILIACION.Equals(beneficiario.id)
                                    select t.ESTATUS_TARJETA
                                    ).SingleOrDefault();
                        if (e != null)
                        {
                            beneficiario.estatustarjeta = e.ToString();
                        }
                        else
                        {
                            beneficiario.estatustarjeta = "";
                        }
                    }
                }
                return compañiaBeneficiarios;
            }
        }

        public List<CompanyAfiliadoRecarga> FindRecarga(int companyid)
        {
            List<CompanyAfiliadoRecarga> compañiaafiliados = new List<CompanyAfiliadoRecarga>();
            PrepagoCompanyAffiliattes ca = new PrepagoCompanyAffiliattes();
            ca = Find(companyid);
            foreach (Afiliado afiliado in ca.Beneficiarios)
            {
                CompanyAfiliadoRecarga temp = new CompanyAfiliadoRecarga();
                temp.companyid = ca.companyid;
                temp.namecompañia = ca.namecompañia;
                temp.rif = ca.rif;
                temp.phone = ca.phone;
                temp.address = ca.phone;
                temp.email = ca.email;
                temp.Afiliadoid = afiliado.id;
                temp.docnumber = afiliado.docnumber;
                temp.name = afiliado.name;
                temp.lastname1 = afiliado.lastname1;
                temp.typeid = afiliado.typeid;
                temp.type = afiliado.type;
                temp.statusid = afiliado.statusid;
                temp.estatus = afiliado.estatus;
                compañiaafiliados.Add(temp);
                temp = null;
            }
            return compañiaafiliados;
        }

        public List<CompanyAfiliadoRecarga> FindRecarga(int companyid, string numdoc, string name, string email)
        {
            if (name == "")
            {
                name = null;
            }
            if (email == "")
            {
                email = null;
            }
            List<CompanyAfiliadoRecarga> compañiaafiliados = new List<CompanyAfiliadoRecarga>();
            PrepagoCompanyAffiliattes ca = new PrepagoCompanyAffiliattes();
            ca = Find(companyid);
            if (name == null && email == null)
            {
                foreach (Afiliado afiliado in ca.Beneficiarios)
                {
                    if (afiliado.docnumber == numdoc)
                    {
                        CompanyAfiliadoRecarga temp = new CompanyAfiliadoRecarga();
                        temp.companyid = ca.companyid;
                        temp.namecompañia = ca.namecompañia;
                        temp.rif = ca.rif;
                        temp.phone = ca.phone;
                        temp.address = ca.phone;
                        temp.email = ca.email;
                        temp.Afiliadoid = afiliado.id;
                        temp.docnumber = afiliado.docnumber;
                        temp.name = afiliado.name;
                        temp.lastname1 = afiliado.lastname1;
                        temp.typeid = afiliado.typeid;
                        temp.type = afiliado.type;
                        temp.statusid = afiliado.statusid;
                        temp.estatus = afiliado.estatus;
                        compañiaafiliados.Add(temp);
                        temp = null;
                    }
                }
            }
            else
            {
                foreach (Afiliado afiliado in ca.Beneficiarios)
                {
                    if (afiliado.name.ToLower().Contains(name.ToLower()) || afiliado.lastname1.ToLower().Contains(name.ToLower()) || afiliado.email == email)
                    {
                        CompanyAfiliadoRecarga temp = new CompanyAfiliadoRecarga();
                        temp.companyid = ca.companyid;
                        temp.namecompañia = ca.namecompañia;
                        temp.rif = ca.rif;
                        temp.phone = ca.phone;
                        temp.address = ca.phone;
                        temp.email = ca.email;
                        temp.Afiliadoid = afiliado.id;
                        temp.docnumber = afiliado.docnumber;
                        temp.name = afiliado.name;
                        temp.lastname1 = afiliado.lastname1;
                        temp.typeid = afiliado.typeid;
                        temp.type = afiliado.type;
                        temp.statusid = afiliado.statusid;
                        temp.estatus = afiliado.estatus;
                        compañiaafiliados.Add(temp);
                        temp = null;
                    }
                }
            }
            if (compañiaafiliados.Count == 0)
            {
                CompanyAfiliadoRecarga temp = new CompanyAfiliadoRecarga();
                temp.companyid = ca.companyid;
                temp.namecompañia = ca.namecompañia;
                temp.rif = ca.rif;
                temp.phone = ca.phone;
                temp.address = ca.phone;
                temp.email = ca.email;
                temp.estatus = "Activa";
                compañiaafiliados.Add(temp);
            }
            return compañiaafiliados;
        }

        //busca el detalle de una orden para procesar
        public List<CompanyAfiliadoRecarga> FindRecargaDetalle(int companyid, int id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                List<CompanyAfiliadoRecarga> compañiaafiliados = new List<CompanyAfiliadoRecarga>();
                PrepagoCompanyAffiliattes ca = new PrepagoCompanyAffiliattes();
                ca = Find(companyid);
                decimal montorecarga;
                foreach (Afiliado afiliado in ca.Beneficiarios)
                {
                    CompanyAfiliadoRecarga temp = new CompanyAfiliadoRecarga();
                    montorecarga = (from o in db.OrdersDetails
                                    where o.customerid.Equals(afiliado.id) && o.orderid.Equals(id)
                                    select o.amount
                                    ).FirstOrDefault();
                    if (montorecarga != 0)
                    {
                        temp.companyid = ca.companyid;
                        temp.namecompañia = ca.namecompañia;
                        temp.rif = ca.rif;
                        temp.phone = ca.phone;
                        temp.address = ca.phone;
                        temp.email = ca.email;
                        temp.Afiliadoid = afiliado.id;
                        temp.docnumber = afiliado.docnumber;
                        temp.name = afiliado.name;
                        temp.lastname1 = afiliado.lastname1;
                        temp.typeid = afiliado.typeid;
                        temp.type = afiliado.type;
                        temp.statusid = afiliado.statusid;
                        temp.estatus = afiliado.estatus;
                        temp.Orderid = id;
                        //temp.Orderstatus = db.Orders.Find(id).status;
                        temp.MontoRecarga = Math.Truncate(montorecarga);
                        compañiaafiliados.Add(temp);
                        temp = null;
                    }
                }
                return compañiaafiliados;
            }
        }

        //Busca el resultado de una orden de recarga
        public List<CompanyAfiliadoRecarga> FindRecargaProcesada(int companyid, int id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                List<CompanyAfiliadoRecarga> compañiaafiliados = new List<CompanyAfiliadoRecarga>();
                PrepagoCompanyAffiliattes ca = new PrepagoCompanyAffiliattes();
                ca = Find(companyid);
                decimal montorecarga;
                string resultadorecarga;
                foreach (Afiliado afiliado in ca.Beneficiarios)
                {
                    CompanyAfiliadoRecarga temp = new CompanyAfiliadoRecarga();
                    montorecarga = (from o in db.OrdersDetails
                                    where o.customerid.Equals(afiliado.id) && o.orderid.Equals(id)
                                    select o.amount
                                    ).FirstOrDefault();
                    resultadorecarga = (from o in db.OrdersDetails
                                        where o.customerid.Equals(afiliado.id) && o.orderid.Equals(id)
                                        select o.comments
                                        ).FirstOrDefault();
                    if (montorecarga != 0 && resultadorecarga != null)
                    {
                        temp.companyid = ca.companyid;
                        temp.namecompañia = ca.namecompañia;
                        temp.rif = ca.rif;
                        temp.phone = ca.phone;
                        temp.address = ca.phone;
                        temp.email = ca.email;
                        temp.Afiliadoid = afiliado.id;
                        temp.docnumber = afiliado.docnumber;
                        temp.name = afiliado.name;
                        temp.lastname1 = afiliado.lastname1;
                        temp.typeid = afiliado.typeid;
                        temp.type = afiliado.type;
                        temp.statusid = afiliado.statusid;
                        temp.estatus = afiliado.estatus;                   
                        temp.MontoRecarga = montorecarga;
                        temp.ResultadoRecarga = resultadorecarga;
                        compañiaafiliados.Add(temp);
                        temp = null;
                    }
                }
                return compañiaafiliados;
            }
        }

        public bool CrearOrden(int companyid, decimal MontoTotalRecargas, List<CompanyAfiliadoRecarga> recargas)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                //ENTIDAD Order                   
                var Order = new Order()
                {
                    id = OrderID(),
                    prepaidcustomerid = companyid,
                    totalamount = MontoTotalRecargas,
                    paymenttype = "",
                    creationdate = DateTime.Now,
                    creationuserid = (int)HttpContext.Current.Session["userid"],
                    processdate = DateTime.Now,
                    sumastatusid = ESTATUS_ORDEN_RECARGA_NUEVA 
                };
                db.Orders.Add(Order);
                var OrderDetail = new OrdersDetail();
                int idbase = OrderDetailID();
                foreach (CompanyAfiliadoRecarga b in recargas)
                {
                    idbase = idbase + 1;
                    //ENTIDAD OrderDetail    
                    OrderDetail = new OrdersDetail()
                    {
                        id = idbase,
                        orderid = Order.id,
                        customerid = b.Afiliadoid,
                        amount = b.MontoRecarga,
                        sumastatusid = ESTATUS_ORDEN_RECARGA_NUEVA
                    };
                    db.OrdersDetails.Add(OrderDetail);
                    OrderDetail = null;
                }
                db.SaveChanges();
                return true;
            }
        }

        public List<Orden> BuscarOrdenes(int companyid)
        {
            List<Orden> ordenes;
            using (LealtadEntities db = new LealtadEntities())
            {
                ordenes = (from o in db.Orders
                           where o.prepaidcustomerid.Equals(companyid)
                           select new Orden()
                           {
                               id = o.id,
                               prepaidcustomerid = companyid,
                               statusid = o.sumastatusid,
                               totalamount = o.totalamount,
                               creationdate = o.creationdate
                           }).ToList();
                return ordenes;
            }
        }

        //busca el detalle de una orden a partir del id
        private List<DetalleOrden> BuscarDetalleOrden(int orderid)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                List<DetalleOrden> detalleorden;
                detalleorden = (from o in db.OrdersDetails
                                where o.orderid.Equals(orderid)
                                select new DetalleOrden()
                                {
                                    id = o.id,
                                    customerid = o.customerid,
                                    amount = o.amount
                                }).ToList();
                return detalleorden;
            }
        }

        public bool AprobarOrden(int orderid)
        {
            using (LealtadEntities db = new LealtadEntities())
            {             
                Order orden = db.Orders.FirstOrDefault(o => o.id.Equals(orderid));
                orden.sumastatusid = ESTATUS_ORDEN_RECARGA_APROBADA; 
                orden.processdate = DateTime.Now;
                db.SaveChanges();
                return true;
            }
        }

        public bool RechazarOrden(int orderid)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                Order orden = db.Orders.FirstOrDefault(o => o.id.Equals(orderid));
                orden.sumastatusid = ESTATUS_ORDEN_RECARGA_RECHAZADA;
                orden.processdate = DateTime.Now;
                db.SaveChanges();
                return true;
            }
        }

        //recarga bs en la cuenta del afiliado
        private bool Recargar(DetalleOrden detalleorden, string docnumber, decimal monto)
        {
            string RespuestaCardsJson = WSL.Cards.addBatch(docnumber, Math.Truncate(monto).ToString(), "200", (string)HttpContext.Current.Session["login"]);
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
                detalleorden.comments = RespuestaCards.exdetail;
                return false;
            }
        }
        
        public bool ProcesarOrden(int orderid)
        {
            List<DetalleOrden> detalleorden = BuscarDetalleOrden(orderid);
            string docnumber;
            decimal amount;
            //bool resultado = true;
            using (LealtadEntities db = new LealtadEntities())
            {
                foreach (DetalleOrden o in detalleorden)
                {
                    docnumber = db.Affiliates.FirstOrDefault(a => a.id.Equals(o.customerid)).docnumber.Substring(2);
                    amount = o.amount;
                    if (Recargar(o, docnumber, amount))
                    {
                        o.statusid = ESTATUS_DETALLEORDEN_RECARGA_EFECTIVA;
                        o.comments = "Recarga efectiva";
                    }
                    else
                    {
                        o.statusid = ESTATUS_DETALLEORDEN_RECARGA_FALLIDA;
                        //resultado = false;
                    }
                    OrdersDetail d = db.OrdersDetails.FirstOrDefault(x => x.id.Equals(o.id));
                    d.sumastatusid = o.statusid;
                    d.comments = o.comments;
                    db.SaveChanges();
                }
                Order orden = db.Orders.FirstOrDefault(o => o.id.Equals(orderid));
                orden.sumastatusid = ESTATUS_ORDEN_RECARGA_EFECTIVA;
                orden.processdate = DateTime.Now;
                db.SaveChanges();
                //return resultado;
                return true;
            }
        }

        public bool Save(Company company)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                var Company = new Company()
                {
                    id = CompanyID(),
                    name = company.name,
                    phone = company.phone,
                    rif = company.rif,
                    ALIAS = company.ALIAS,
                    address = company.address,
                    email = company.email,
                    creationdate = DateTime.Now,
                    userid = (int)HttpContext.Current.Session["userid"]
                };
                db.Companies.Add(Company);
                db.SaveChanges();
                return true;
            }
        }

        public bool SaveChanges(Company company)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                Company Company = db.Companies.FirstOrDefault(c => c.id == company.id);
                if (Company != null)
                {
                    Company.name = company.name;
                    Company.phone = company.phone;
                    Company.rif = company.rif;
                    Company.ALIAS = company.ALIAS;
                    Company.address = company.address;
                    Company.email = company.email;
                }
                db.SaveChanges();
                return true;
            }
        }

        public bool BorrarCompañia(int id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                Company company = db.Companies.Find(id);
                if (company != null)
                {
                    db.Companies.Remove(company);
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

    }
}