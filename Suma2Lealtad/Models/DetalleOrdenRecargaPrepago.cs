using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Suma2Lealtad.Models
{
    public class DetalleOrdenRecargaPrepago
    {
        //public BeneficiarioPrepago Beneficiario { get; set; }   //Beneficiario 

        public int idCliente { get; set; }
        public string nameCliente { get; set; }
        public string rifCliente { get; set; }
        public string phoneCliente { get; set; }
        
        public int idOrden { get; set; }
        public string tipoOrden { get; set; }
        public string statusOrden { get; set; }
        public string documentoOrden { get; set; }
        public string batchid { get; set; }                    //Para Ordenes de Anulación, aqui se guarda el batchid de la recarga anulada
        
        public int idAfiliado { get; set; }
        public string docnumberAfiliado { get; set; }
        public string nameAfiliado { get; set; }
        public string lastname1Afiliado { get; set; }
        
        public decimal montoRecarga { get; set; }               //Monto de Recarga de Saldo
        public string resultadoRecarga { get; set; }            //Resultado de la Operación de Recarga de Saldo en Cards
        //public bool beneficiarioExcluido {get;set;}           //Indica si un beneficiario fue excluido de la Orden de Recarga en su revisión
        public string observacionesExclusion { get; set; }      //Información relativa a la exclusión (motivos)
        public string statusDetalleOrden { get; set; }          //Estatus del detalle de la orden
    }
}