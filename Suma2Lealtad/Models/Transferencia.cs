using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Suma2Lealtad.Models
{
    public class Transferencia
    {
        //public string idAfiliadoOrigen { get; set; }
        public string docnumberAfiliadoOrigen { get; set; }
        public string nameAfiliadoOrigen { get; set; }
        public string lastname1AfiliadoOrigen { get; set; }
        
        //public string idAfiliadoDestino { get; set; }
        public string docnumberAfiliadoDestino { get; set; }
        public string nameAfiliadoDestino { get; set; }
        public string lastname1AfiliadoDestino { get; set; }

        public string ResumenTransferenciaSuma { get; set; }
        public string ResumenTransferenciaPrepago { get; set; }
        
        public string DenominacionCuentaOrigenSuma { get; set; }
        public string DenominacionCuentaOrigenPrepago { get; set; }
        
        public Saldo datosCuentaSumaOrigen { get; set; }
        public Saldo datosCuentaPrepagoOrigen{ get; set; }

        public Saldo datosCuentaSumaDestino { get; set; }
        public Saldo datosCuentaPrepagoDestino { get; set; }
    }
}