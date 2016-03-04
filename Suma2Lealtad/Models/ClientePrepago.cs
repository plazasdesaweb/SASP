using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Suma2Lealtad.Models
{
    public class ClientePrepago
    {
        public int idCliente { get; set; }
        public string nameCliente { get; set; }
        public string aliasCliente { get; set; }
        public string rifCliente { get; set; }
        public string addressCliente { get; set; }
        public string phoneCliente { get; set; }
        public string emailCliente { get; set; }

        public List<PrepaidCustomer> ListaClientes { get; set; }

        #region Lista_ClaseDeOrden
        public class ClaseDeOrden
        {
            public string id { get; set; }
            public string clase { get; set; }
        }

        public IEnumerable<ClaseDeOrden> ClaseDeOrdenOptions =
            new List<ClaseDeOrden>
        {
              new ClaseDeOrden { id = "", clase = "Seleccione..."          },
              new ClaseDeOrden { id = "Orden de Recarga", clase = "Orden de Recarga" },
              new ClaseDeOrden { id = "Orden de Recarga desde Archivo", clase = "Orden de Recarga desde Archivo"  } ,
              new ClaseDeOrden { id = "Orden de Anulación", clase = "Orden de Anulación"  }
        };
        #endregion

        #region Lista_EstadoDeAfiliacion
        public class EstadoDeAfiliacion
        {
            public string id { get; set; }
            public string estado { get; set; }
        }

        public IEnumerable<EstadoDeAfiliacion> EstadoDeAfiliacionOptions =
            new List<EstadoDeAfiliacion>
        {
              new EstadoDeAfiliacion { id = "", estado = "Seleccione..." },
              new EstadoDeAfiliacion { id = "Nueva", estado = "Nueva" },
              new EstadoDeAfiliacion { id = "Inactiva", estado = "Inactiva"  },
              new EstadoDeAfiliacion { id = "Activa", estado = "Activa"  },
              new EstadoDeAfiliacion { id = "Eliminada", estado = "Eliminada"  }
        };
        #endregion

        #region Lista_EstadoDeTarjeta
        public class EstadoDeTarjeta
        {
            public string id { get; set; }
            public string estado { get; set; }
        }

        public IEnumerable<EstadoDeTarjeta> EstadoDeTarjetaOptions =
            new List<EstadoDeTarjeta>
        {
              new EstadoDeTarjeta { id = "", estado = "Seleccione..."          },
              new EstadoDeTarjeta { id = "Nueva", estado = "Nueva" },
              new EstadoDeTarjeta { id = "Inactiva", estado = "Inactiva"  },
              new EstadoDeTarjeta { id = "Activa", estado = "Activa"  },
              new EstadoDeTarjeta { id = "Eliminada", estado = "Eliminada"  },
              new EstadoDeTarjeta { id = "Suspendida", estado = "Suspendida"  }
        };
        #endregion

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