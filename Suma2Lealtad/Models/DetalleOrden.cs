using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Suma2Lealtad.Models
{
    public class DetalleOrden
    {
        public int id { get; set; }
        public int orderid { get; set; }
        public int customerid { get; set; }
        public decimal amount { get; set; }
        public string comments { get; set; }
        public int statusid { get; set; }
    }
}