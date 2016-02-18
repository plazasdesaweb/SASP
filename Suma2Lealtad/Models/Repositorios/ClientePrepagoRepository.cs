using Suma2Lealtad.Modules;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;


namespace Suma2Lealtad.Models
{
    public class ClientePrepagoRepository
    {
        private int ClientePrepagoID()
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString();
                if (db.PrepaidCustomers.Count() == 0)
                    return 1;
                return (db.PrepaidCustomers.Max(c => c.id) + 1);
            }
        }

        public ClientePrepago Find(string rif)
        {
            ClientePrepago cliente;
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString();
                cliente = (from c in db.PrepaidCustomers
                           where c.rif.Equals(rif)
                           select new ClientePrepago()
                           {
                               idCliente = c.id,
                               nameCliente = c.name,
                               aliasCliente = c.alias,
                               rifCliente = c.rif,
                               phoneCliente = c.phone,
                               emailCliente = c.email
                           }).FirstOrDefault();
            }
            return cliente;
        }

        public List<ClientePrepago> Find(string name, string rif)
        {
            List<ClientePrepago> clientes;
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString();
                if (name == "")
                {
                    name = null;
                }
                if (rif == "")
                {
                    rif = null;
                }
                if (name == null && rif == null)
                {
                    clientes = (from c in db.PrepaidCustomers
                                select new ClientePrepago()
                                {
                                    idCliente = c.id,
                                    nameCliente = c.name,
                                    aliasCliente = c.alias,
                                    rifCliente = c.rif,
                                    phoneCliente = c.phone,
                                    emailCliente = c.email
                                }).OrderBy(x => x.nameCliente).ToList();
                }
                else
                {
                    clientes = (from c in db.PrepaidCustomers
                                where c.rif.Equals(rif) || c.name.Contains(name)
                                select new ClientePrepago()
                                {
                                    idCliente = c.id,
                                    nameCliente = c.name,
                                    aliasCliente = c.alias,
                                    rifCliente = c.rif,
                                    phoneCliente = c.phone,
                                    emailCliente = c.email
                                }).OrderBy(x => x.nameCliente).ToList();
                }
            }
            return clientes;
        }

        public ClientePrepago Find(int id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString();
                ClientePrepago cliente;
                cliente = (from c in db.PrepaidCustomers
                           where c.id.Equals(id)
                           select new ClientePrepago()
                           {
                               idCliente = c.id,
                               nameCliente = c.name,
                               aliasCliente = c.alias,
                               rifCliente = c.rif,
                               addressCliente = c.address,
                               phoneCliente = c.phone,
                               emailCliente = c.email
                           }).FirstOrDefault();
                return cliente;
            }
        }

        public ClientePrepago FindXidAfiliado(int idAfiliado)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString();
                ClientePrepago cliente;
                cliente = (from c in db.PrepaidCustomers
                           join b in db.PrepaidBeneficiaries on c.id equals b.prepaidcustomerid
                           where b.affiliateid.Equals(idAfiliado)
                           select new ClientePrepago()
                           {
                               idCliente = c.id,
                               nameCliente = c.name,
                               aliasCliente = c.alias,
                               rifCliente = c.rif,
                               addressCliente = c.address,
                               phoneCliente = c.phone,
                               emailCliente = c.email
                           }).FirstOrDefault();
                return cliente;
            }
        }

        public List<BeneficiarioPrepagoIndex> FindBeneficiarios(int id, string numdoc, string name, string email, string estadoAfiliacion, string estadoTarjeta)
        {
            List<BeneficiarioPrepagoIndex> beneficiarios = new List<BeneficiarioPrepagoIndex>();
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString();
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
                                 join b in db.PrepaidBeneficiaries on a.id equals b.affiliateid
                                 where b.prepaidcustomerid.Equals(id)
                                 join c in db.CLIENTES on a.docnumber equals c.TIPO_DOCUMENTO + "-" + c.NRO_DOCUMENTO
                                 select new
                                 {
                                     pan = t.NRO_TARJETA,
                                     estatustarjeta = t.ESTATUS_TARJETA,
                                     id = t.NRO_AFILIACION,
                                     docnumber = a.docnumber,
                                     typeid = a.typeid,
                                     sumastatusid = a.sumastatusid,
                                     idCliente = b.prepaidcustomerid,
                                     name = c.NOMBRE_CLIENTE1,
                                     lastname1 = c.APELLIDO_CLIENTE1,
                                     email = c.E_MAIL
                                 }).OrderBy(d => d.docnumber);
                    beneficiarios = (from q in query.AsEnumerable()
                                     join p in db.PrepaidCustomers on q.idCliente equals p.id
                                     join s in db.SumaStatuses on q.sumastatusid equals s.id
                                     join ty in db.Types on q.typeid equals ty.id
                                     select new BeneficiarioPrepagoIndex
                                     {
                                         Afiliado = new AfiliadoSumaIndex
                                         {
                                             //ENTIDAD Affiliate 
                                             id = q.id,
                                             docnumber = q.docnumber,
                                             typeid = q.typeid,
                                             //ENTIDAD CLIENTE
                                             name = q.name,
                                             lastname1 = q.lastname1,
                                             email = q.email,
                                             //ENTIDAD SumaStatuses
                                             estatus = s.name,
                                             //ENTIDAD Type
                                             type = ty.name,
                                             //ENTIDAD TARJETA
                                             pan = q.pan.ToString(),
                                             estatustarjeta = q.estatustarjeta
                                         },
                                         Cliente = new ClientePrepago
                                         {
                                             idCliente = p.id,
                                             nameCliente = p.name,
                                             aliasCliente = p.alias,
                                             rifCliente = p.rif,
                                             addressCliente = p.address,
                                             phoneCliente = p.phone,
                                             emailCliente = p.email
                                         }
                                     }).OrderBy(n => n.Cliente.nameCliente).OrderBy(x => x.Afiliado.docnumber).ToList();
                }
                //BUSCAR POR ESTADO DE AFILIACION
                else if (estadoAfiliacion != null)
                {
                    var query = (from a in db.Affiliates
                                 where a.SumaStatu.name.Equals(estadoAfiliacion)
                                 join b in db.PrepaidBeneficiaries on a.id equals b.affiliateid
                                 where b.prepaidcustomerid.Equals(id)
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
                                     idCliente = b.prepaidcustomerid,
                                     name = c.NOMBRE_CLIENTE1,
                                     lastname1 = c.APELLIDO_CLIENTE1,
                                     email = c.E_MAIL
                                 }).OrderBy(d => d.docnumber);
                    beneficiarios = (from q in query.AsEnumerable()
                                     join p in db.PrepaidCustomers on q.idCliente equals p.id
                                     join s in db.SumaStatuses on q.sumastatusid equals s.id
                                     join ty in db.Types on q.typeid equals ty.id
                                     select new BeneficiarioPrepagoIndex
                                     {
                                         Afiliado = new AfiliadoSumaIndex
                                         {
                                             //ENTIDAD Affiliate 
                                             id = q.id,
                                             docnumber = q.docnumber,
                                             typeid = q.typeid,
                                             //ENTIDAD CLIENTE
                                             name = q.name,
                                             lastname1 = q.lastname1,
                                             email = q.email,
                                             //ENTIDAD SumaStatuses
                                             estatus = s.name,
                                             //ENTIDAD Type
                                             type = ty.name,
                                             //ENTIDAD TARJETA
                                             pan = q.pan == 0 ? "" : q.pan.ToString(),
                                             estatustarjeta = q.estatustarjeta
                                         },
                                         Cliente = new ClientePrepago
                                         {
                                             idCliente = p.id,
                                             nameCliente = p.name,
                                             aliasCliente = p.alias,
                                             rifCliente = p.rif,
                                             addressCliente = p.address,
                                             phoneCliente = p.phone,
                                             emailCliente = p.email
                                         }
                                     }).OrderBy(n => n.Cliente.nameCliente).OrderBy(x => x.Afiliado.docnumber).ToList();

                }
                //BUSCAR POR NUMERO DE DOCUMENTO
                else if (numdoc != null)
                {
                    var query = (from a in db.Affiliates
                                 where a.docnumber.Equals(numdoc)
                                 join b in db.PrepaidBeneficiaries on a.id equals b.affiliateid
                                 where b.prepaidcustomerid.Equals(id)
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
                                     idCliente = b.prepaidcustomerid,
                                     name = c.NOMBRE_CLIENTE1,
                                     lastname1 = c.APELLIDO_CLIENTE1,
                                     email = c.E_MAIL
                                 }).OrderBy(d => d.docnumber);
                    beneficiarios = (from q in query.AsEnumerable()
                                     join p in db.PrepaidCustomers on q.idCliente equals p.id
                                     join s in db.SumaStatuses on q.sumastatusid equals s.id
                                     join ty in db.Types on q.typeid equals ty.id
                                     select new BeneficiarioPrepagoIndex
                                     {
                                         Afiliado = new AfiliadoSumaIndex
                                         {
                                             //ENTIDAD Affiliate 
                                             id = q.id,
                                             docnumber = q.docnumber,
                                             typeid = q.typeid,
                                             //ENTIDAD CLIENTE
                                             name = q.name,
                                             lastname1 = q.lastname1,
                                             email = q.email,
                                             //ENTIDAD SumaStatuses
                                             estatus = s.name,
                                             //ENTIDAD Type
                                             type = ty.name,
                                             //ENTIDAD TARJETA
                                             pan = q.pan == 0 ? "" : q.pan.ToString(),
                                             estatustarjeta = q.estatustarjeta
                                         },
                                         Cliente = new ClientePrepago
                                         {
                                             idCliente = p.id,
                                             nameCliente = p.name,
                                             aliasCliente = p.alias,
                                             rifCliente = p.rif,
                                             addressCliente = p.address,
                                             phoneCliente = p.phone,
                                             emailCliente = p.email
                                         }
                                     }).OrderBy(n => n.Cliente.nameCliente).OrderBy(x => x.Afiliado.docnumber).ToList();
                }
                //BUSCAR POR NOMBRE O CORREO
                else if (name != null || email != null)
                {
                    var query = (from a in db.Affiliates
                                 join b in db.PrepaidBeneficiaries on a.id equals b.affiliateid
                                 where b.prepaidcustomerid.Equals(id)
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
                                     idCliente = b.prepaidcustomerid,
                                     name = c.NOMBRE_CLIENTE1,
                                     lastname1 = c.APELLIDO_CLIENTE1,
                                     email = c.E_MAIL
                                 }).OrderBy(d => d.docnumber);                    
                    beneficiarios = (from q in query.AsEnumerable()
                                     join p in db.PrepaidCustomers on q.idCliente equals p.id
                                     join s in db.SumaStatuses on q.sumastatusid equals s.id
                                     join ty in db.Types on q.typeid equals ty.id
                                     select new BeneficiarioPrepagoIndex
                                     {
                                         Afiliado = new AfiliadoSumaIndex
                                         {
                                             //ENTIDAD Affiliate 
                                             id = q.id,
                                             docnumber = q.docnumber,
                                             typeid = q.typeid,
                                             //ENTIDAD CLIENTE
                                             name = q.name,
                                             lastname1 = q.lastname1,
                                             email = q.email,
                                             //ENTIDAD SumaStatuses
                                             estatus = s.name,
                                             //ENTIDAD Type
                                             type = ty.name,
                                             //ENTIDAD TARJETA
                                             pan = q.pan == 0 ? "" : q.pan.ToString(),
                                             estatustarjeta = q.estatustarjeta
                                         },
                                         Cliente = new ClientePrepago
                                         {
                                             idCliente = p.id,
                                             nameCliente = p.name,
                                             aliasCliente = p.alias,
                                             rifCliente = p.rif,
                                             addressCliente = p.address,
                                             phoneCliente = p.phone,
                                             emailCliente = p.email
                                         }
                                     }).OrderBy(n => n.Cliente.nameCliente).OrderBy(x => x.Afiliado.docnumber).ToList();
                }
                //BUSCAR TODOS
                else if (numdoc == null && name == null && email == null && estadoAfiliacion == null && estadoTarjeta == null)
                {
                    var query = (from a in db.Affiliates
                                 join b in db.PrepaidBeneficiaries on a.id equals b.affiliateid
                                 where b.prepaidcustomerid.Equals(id)
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
                                     idCliente = b.prepaidcustomerid,
                                     name = c.NOMBRE_CLIENTE1,
                                     lastname1 = c.APELLIDO_CLIENTE1,
                                     email = c.E_MAIL
                                 }).OrderBy(d => d.docnumber);
                    beneficiarios = (from q in query.AsEnumerable()
                                     join p in db.PrepaidCustomers on q.idCliente equals p.id
                                     join s in db.SumaStatuses on q.sumastatusid equals s.id
                                     join ty in db.Types on q.typeid equals ty.id
                                     select new BeneficiarioPrepagoIndex
                                     {
                                         Afiliado = new AfiliadoSumaIndex
                                         {
                                             //ENTIDAD Affiliate 
                                             id = q.id,
                                             docnumber = q.docnumber,
                                             typeid = q.typeid,
                                             //ENTIDAD CLIENTE
                                             name = q.name,
                                             lastname1 = q.lastname1,
                                             email = q.email,
                                             //ENTIDAD SumaStatuses
                                             estatus = s.name,
                                             //ENTIDAD Type
                                             type = ty.name,
                                             //ENTIDAD TARJETA
                                             pan = q.pan == 0 ? "" : q.pan.ToString(),
                                             estatustarjeta = q.estatustarjeta
                                         },
                                         Cliente = new ClientePrepago
                                         {
                                             idCliente = p.id,
                                             nameCliente = p.name,
                                             aliasCliente = p.alias,
                                             rifCliente = p.rif,
                                             addressCliente = p.address,
                                             phoneCliente = p.phone,
                                             emailCliente = p.email
                                         }
                                     }).OrderBy(n => n.Cliente.nameCliente).OrderBy(x => x.Afiliado.docnumber).ToList();
                }
            }
            return beneficiarios;
        }

        public bool Save(ClientePrepago cliente)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString();
                var Cliente = new PrepaidCustomer()
                {
                    id = ClientePrepagoID(),
                    name = cliente.nameCliente,
                    phone = cliente.phoneCliente,
                    rif = cliente.rifCliente,
                    alias = cliente.aliasCliente,
                    address = cliente.addressCliente,
                    email = cliente.emailCliente,
                    creationdate = DateTime.Now,
                    userid = (int)HttpContext.Current.Session["userid"]
                };
                db.PrepaidCustomers.Add(Cliente);
                db.SaveChanges();
                return true;
            }
        }

        public bool SaveChanges(ClientePrepago cliente)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString();
                PrepaidCustomer Cliente = db.PrepaidCustomers.FirstOrDefault(c => c.id == cliente.idCliente);
                if (Cliente != null)
                {
                    Cliente.name = cliente.nameCliente;
                    Cliente.phone = cliente.phoneCliente;
                    Cliente.alias = cliente.aliasCliente;
                    Cliente.address = cliente.addressCliente;
                    Cliente.email = cliente.emailCliente;
                }
                db.SaveChanges();
                return true;
            }
        }

        public bool BorrarCliente(int id)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString();
                PrepaidCustomer Cliente = db.PrepaidCustomers.FirstOrDefault(c => c.id == id);
                if (Cliente != null)
                {
                    db.PrepaidCustomers.Remove(Cliente);
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