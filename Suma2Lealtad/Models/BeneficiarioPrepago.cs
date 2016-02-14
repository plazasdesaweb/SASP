using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Suma2Lealtad.Models
{
    public class BeneficiarioPrepago
    {
        public AfiliadoSuma Afiliado;
        public ClientePrepago Cliente;

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
              new EstadoDeTarjeta { id = "Activa", estado = "Activa"  },
              new EstadoDeTarjeta { id = "Suspendida", estado = "Suspendida"  }
        };
        #endregion

        // fuera de línea/monto de recarga para compras fuera de línea
        public string monto { get; set; }

        // retornar el valor concatenado de los campos : name + lastname1
        public string beneficiario { get { return Afiliado.name + " " + Afiliado.lastname1; } }

    }
}