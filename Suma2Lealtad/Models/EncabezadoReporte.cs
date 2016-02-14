using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suma2Lealtad.Models
{
    public class EncabezadoReporte
    {
        public string nombreReporte { set; get; }
        public string fechainicioReporte { set; get; }
        public string fechafinReporte { set; get; }
        public string tipoconsultaReporte { set; get; }
        public string parametrotipoconsultaReporte { set; get; }
        public string modotransaccionReporte { set; get; }
        public string documentoreferenciaReporte { set; get; }
        public string estatustarjetaReporte { set; get; }
    }
}
