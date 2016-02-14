using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Suma2Lealtad.Models
{
    public class AfiliadoSumaIndex
    {
        public int id { get; set; }
        public string docnumber { get; set; }
        public int typeid { get; set; }
        public int sumastatusid { get; set; }
        //ENTIDAD CLIENTE
        public string name { get; set; }
        public string lastname1 { get; set; }
        public string email { get; set; }
        //ENTIDAD SumaStatuses
        public string estatus { get; set; }
        //ENTIDAD Type
        public string type { get; set; }
        //ENTIDAD TARJETA
        public string pan { get; set; }
        public string estatustarjeta { get; set; }
    }
}