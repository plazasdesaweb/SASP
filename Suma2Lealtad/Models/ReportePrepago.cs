using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Suma2Lealtad.Models
{
    public class ReportePrepago
    {
        public EncabezadoReporte Encabezado { get; set; }
        public BeneficiarioPrepagoIndex Beneficiario { get; set; }
        public DateTime fecha { get; set; }
        public Decimal monto { get; set; }        
        public string comentario { get; set; }
        public decimal numerotarjeta { get; set; }
        public string estatustarjeta { get; set; }
        public string usuario { get; set; }        
        public string detalle { get; set; }
        public string tipo { get; set; }
        public int nroordenrecarga { get; set; }
        public string referenciarecarga { get; set; }
        public ParametrosReporte Parametros {get;set;}

        public List<PrepaidCustomer> ListaClientes { get; set; }

        #region Lista_TipoConsulta
        public class TipoConsulta
        {
            public string id { get; set; }
            public string tipo { get; set; }
        }

        public IEnumerable<TipoConsulta> TipoConsultaOptions =
            new List<TipoConsulta>
        {
              new TipoConsulta { id = "Cliente", tipo = "Por Cliente" },
              new TipoConsulta { id = "Beneficiario", tipo = "Por Beneficiario" },
        };
        #endregion

        #region Lista_ModoTransaccion
        public class ModoTransaccion
        {
            public string id { get; set; }
            public string modo { get; set; }
        }

        public IEnumerable<ModoTransaccion> ModoTransaccionOptions =
            new List<ModoTransaccion>
        {
              new ModoTransaccion { id = "Todas", modo = "Todas" },
              new ModoTransaccion { id = "En Linea", modo = "En Linea" },
              new ModoTransaccion { id = "Fuera de Linea", modo = "Fuera de Linea" }          
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
              new EstadoDeTarjeta { id = "", estado = "" },
              new EstadoDeTarjeta { id = "Nueva", estado = "Nueva" },
              new EstadoDeTarjeta { id = "Activa", estado = "Activa"  },
              new EstadoDeTarjeta { id = "Suspendida", estado = "Suspendida"  }
        };
        #endregion

    }

}