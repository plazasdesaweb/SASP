using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Suma2Lealtad.Models
{
    public class ReporteSuma
    {
        public EncabezadoReporteSuma Encabezado { get; set; }
        public AfiliadoSumaIndex Afiliado { get; set; }
        public DateTime fecha { get; set; }
        public int monto { get; set; }        
        public string comentario { get; set; }
        public string batchid { get; set; }
        public decimal numerotarjeta { get; set; }
        //public string estatustarjeta { get; set; }
        //public string usuario { get; set; }        
        public string detalle { get; set; }
        public string tipo { get; set; }
        public ParametrosReporteSuma Parametros {get;set;}

        //public List<PrepaidCustomer> ListaClientes { get; set; }

        //#region Lista_TipoConsulta
        //public class TipoConsulta
        //{
        //    public string id { get; set; }
        //    public string tipo { get; set; }
        //}

        //public IEnumerable<TipoConsulta> TipoConsultaOptions =
        //    new List<TipoConsulta>
        //{
        //      new TipoConsulta { id = "Cliente", tipo = "Por Cliente" },
        //      new TipoConsulta { id = "Beneficiario", tipo = "Por Beneficiario" },
        //};
        //#endregion

        #region Lista_TipoTransaccion
        public class ModoTransaccion
        {
            public string id { get; set; }
            public string tipo { get; set; }
        }

        public IEnumerable<ModoTransaccion> TipoTransaccionOptions =
            new List<ModoTransaccion>
        {
              new ModoTransaccion { id = "Todas", tipo = "Todas" },
              new ModoTransaccion { id = "Acreditacion", tipo = "Acreditacion" },
              new ModoTransaccion { id = "Canje", tipo = "Canje" },
              new ModoTransaccion { id = "Transferencias Credito", tipo = "Transferencias de Puntos: Crédito" },
              new ModoTransaccion { id = "Transferencias Debito", tipo = "Transferencias de Puntos: Débito" },
        };
        #endregion

        //#region Lista_EstadoDeTarjeta
        //public class EstadoDeTarjeta
        //{
        //    public string id { get; set; }
        //    public string estado { get; set; }
        //}

        //public IEnumerable<EstadoDeTarjeta> EstadoDeTarjetaOptions =
        //    new List<EstadoDeTarjeta>
        //{
        //      new EstadoDeTarjeta { id = "", estado = "" },
        //      new EstadoDeTarjeta { id = "Nueva", estado = "Nueva" },
        //      new EstadoDeTarjeta { id = "Activa", estado = "Activa"  },
        //      new EstadoDeTarjeta { id = "Suspendida", estado = "Suspendida"  }
        //};
        //#endregion

    }

}