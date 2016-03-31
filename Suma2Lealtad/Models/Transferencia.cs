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
        public int idAfiliadoOrigen { get; set; }
        public string typeAfiliadoOrigen { get; set; }
        public string nameAfiliadoOrigen { get; set; }
        public string lastname1AfiliadoOrigen { get; set; }

        //public string idAfiliadoDestino { get; set; }
        public string docnumberAfiliadoDestino { get; set; }
        public int idAfiliadoDestino { get; set; }
        public string typeAfiliadoDestino { get; set; }
        public string nameAfiliadoDestino { get; set; }
        public string lastname1AfiliadoDestino { get; set; }

        public string ResumenTransferenciaSuma { get; set; }
        public string ResumenTransferenciaPrepago { get; set; }

        //resultado de la orden de transferencia
        public string statusDetalleOrdenOrigenSuma { get; set; }          
        public string resultadoTransferenciaOrigenSuma { get; set; }
        public string statusDetalleOrdenOrigenPrepago { get; set; }          
        public string resultadoTransferenciaOrigenPrepago { get; set; }
        public string statusDetalleOrdenDestinoSuma { get; set; }
        public string resultadoTransferenciaDestinoSuma { get; set; }
        public string statusDetalleOrdenDestinoPrepago { get; set; }
        public string resultadoTransferenciaDestinoPrepago { get; set; }

        public string DenominacionCuentaOrigenSuma { get; set; }
        public string DenominacionCuentaOrigenPrepago { get; set; }

        public Saldo datosCuentaSumaOrigen { get; set; }
        public Saldo datosCuentaPrepagoOrigen { get; set; }

        public Saldo datosCuentaSumaDestino { get; set; }
        public Saldo datosCuentaPrepagoDestino { get; set; }

        //datos de la orden
        public int id { set; get; }
        public string statusOrden { set; get; }
        public decimal montoOrden { set; get; }
        public DateTime creationdateOrden { set; get; }
        public string tipoOrden { set; get; }                                       //Individual(Indicar Recargas de forma manual) ó Masiva(Indicar Recargas desde archivo)          

    }
}