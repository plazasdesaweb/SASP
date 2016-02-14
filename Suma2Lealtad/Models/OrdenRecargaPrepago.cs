using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Suma2Lealtad.Models
{
    public class OrdenRecargaPrepago
    {
        public ClientePrepago Cliente { get; set; }                                 //Datos del Cliente 
        public int id { set; get; }
        public string statusOrden { set; get; }
        public decimal montoOrden { set; get; }
        public DateTime creationdateOrden { set; get; }
        public string tipoOrden { set; get; }                                       //Individual(Indicar Recargas de forma manual) ó Masiva(Indicar Recargas desde archivo)          
        public string documento { set; get; }                                       //Documento de referencia de la facturación de la orden
        //public List<DetalleOrdenRecargaPrepago> DetalleOrden { set; get; }        //Detalle de la orden

        public List<PrepaidCustomer> ListaClientes { get; set; }

        #region Lista_EstadoDeOrden
        public class EstadoDeOrden
        {
            public string id { get; set; }
            public string estado { get; set; }
        }

        public IEnumerable<EstadoDeOrden> EstadoDeOrdenOptions =
            new List<EstadoDeOrden>
        {
              new EstadoDeOrden { id = "", estado = "Seleccione..."          },
              new EstadoDeOrden { id = "Nueva", estado = "Nueva" },
              new EstadoDeOrden { id = "Aprobada", estado = "Aprobada"  },
              new EstadoDeOrden { id = "Rechazada", estado = "Rechazada"  },
              new EstadoDeOrden { id = "Procesada", estado = "Procesada"  },
        };
        #endregion
    }
}