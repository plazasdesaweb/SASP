using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Suma2Lealtad.Models
{
    public class BeneficiarioPrepagoViewModel 
    {

        public AfiliadoSuma Afiliado;
        public ClientePrepago Cliente;

        public string numdoc { get; set; }
        public string beneficiario { get; set; }

        [Required(ErrorMessage = "El campo Monto de Transacción es requerido.")]
        [RegularExpression(@"[\d]{1,6}([,][\d]{2})?", ErrorMessage = "El campo Monto de Transacción permite únicamente números en formato decimal : 999999,99")]
        public string monto { get; set; }

        public decimal saldo { get; set; }

        public string documento { get { return numdoc.Replace("V-", "").Replace("J-", "").Replace("E-", ""); } }
        public string montotrx { get { return monto.Replace(",", "").Replace(".", ""); } }

        public string saldoactual { get { return saldo.ToString(); } }


    }
}
