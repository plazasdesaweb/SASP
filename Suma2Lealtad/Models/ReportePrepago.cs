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
        public string batchid { get; set; }
        public decimal numerotarjeta { get; set; }
        public string estatustarjeta { get; set; }
        public string usuario { get; set; }        
        public string detalle { get; set; }
        public string tipo { get; set; }
        public int nroordenrecarga { get; set; }
        public string referenciarecarga { get; set; }
        public string observacionesrecarga { get; set; }
        public ParametrosReporte Parametros {get;set;}
        public List<PrepaidCustomer> ListaClientes { get; set; }

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
              new ModoTransaccion { id = "En Línea", modo = "En Línea" },
              new ModoTransaccion { id = "Fuera de Línea", modo = "Fuera de Línea" }          
        };
        #endregion

        #region Lista_TipoDetalle
        public class TipoDetalle
        {
            public string id { get; set; }
            public string tipo { get; set; }
        }

        public IEnumerable<TipoDetalle> TipoDetalleOptions =
            new List<TipoDetalle>
        {
              new TipoDetalle { id = "Consolidado por Beneficiario", tipo = "Consolidado por Beneficiario" },
              new TipoDetalle { id = "Detallado por Beneficiario", tipo = "Detallado por Beneficiario" },
        };
        #endregion

        #region Lista_TipoTransaccionSuma
        public class TipoTransaccionSuma
        {
            public string id { get; set; }
            public string tipo { get; set; }
        }

        public IEnumerable<TipoTransaccionSuma> TipoTransaccionSumaOptions =
            new List<TipoTransaccionSuma>
        {
              new TipoTransaccionSuma { id = "Todas", tipo = "Todas" },
              new TipoTransaccionSuma { id = "Acreditacion", tipo = "Acreditacion" },
              new TipoTransaccionSuma { id = "Canje", tipo = "Canje" },
              new TipoTransaccionSuma { id = "Transferencias de Puntos: Crédito", tipo = "Transferencias de Puntos: Crédito" },
              new TipoTransaccionSuma { id = "Transferencias de Puntos: Débito", tipo = "Transferencias de Puntos: Débito" },
              new TipoTransaccionSuma { id = "Anulación de Transferencias de Puntos", tipo = "Anulación de Transferencias de Puntos" },
        };
        #endregion

        #region Lista_TipoTransaccionPrepago
        public class TipoTransaccionPrepago
        {
            public string id { get; set; }
            public string tipo { get; set; }
        }

        public IEnumerable<TipoTransaccionPrepago> TipoTransaccionPrepagoOptions =
            new List<TipoTransaccionPrepago>
        {
              new TipoTransaccionPrepago { id = "Todas", tipo = "Todas" },
              new TipoTransaccionPrepago { id = "Compra", tipo = "Compra" },
              new TipoTransaccionPrepago { id = "Recarga", tipo = "Recarga" },
              new TipoTransaccionPrepago { id = "Anulación de Recarga", tipo = "Anulación de Recarga" },
              new TipoTransaccionPrepago { id = "Transferencias de Saldo: Crédito", tipo = "Transferencias de Saldo: Crédito" },
              new TipoTransaccionPrepago { id = "Transferencias de Saldo: Débito", tipo = "Transferencias de Saldo: Débito" },
              new TipoTransaccionPrepago { id = "Anulación de Transferencias de Saldo", tipo = "Anulación de Transferencias de Saldo" },
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