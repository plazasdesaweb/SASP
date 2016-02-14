using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Suma2Lealtad.Models
{
    public class CompanyAfiliadoRecarga
    {
        public int companyid { get; set; }
        public string namecompañia { get; set; }
        public string rif { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string email { get; set; }

        public int Orderid { get; set; }
        public string Orderstatus { get; set; }
  
        public int Afiliadoid { get; set; }
        public string docnumber { get; set; }
        public string name { get; set; }
        public string lastname1 { get; set; }
        public int typeid { get; set; }
        public string type { get; set; }
        public int statusid { get; set; }
        public string estatus { get; set; }

        public decimal MontoRecarga { get; set; }
        public string ResultadoRecarga { get; set; }
        public string TipoRecarga { get; set; }      
    }
}