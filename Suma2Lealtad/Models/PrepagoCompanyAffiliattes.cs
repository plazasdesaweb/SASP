using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Suma2Lealtad.Models
{
    public class PrepagoCompanyAffiliattes
    {
        public int companyid { get; set; }                   
        public string namecompañia { get; set; }             
        public string alias { get; set; }        
        public string rif { get; set; }             
        public string address { get; set; }         
        public string phone { get; set; }           
        public string email { get; set; }

        public List<Afiliado> Beneficiarios { set; get; }
        public List<Orden> Ordenes { set; get; }
    }

}