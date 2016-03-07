using Newtonsoft.Json;
using Suma2Lealtad.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Suma2Lealtad.Models.Repositorios
{
    public class OperacionesRepository
    {
        public string Transferir(string numdocOrigen, string numdocDestino, string tipoCuenta, string monto)
        {
            string montoSinSeparador;
            if (tipoCuenta == Globals.TIPO_CUENTA_PREPAGO)
            {
                montoSinSeparador = Math.Truncate(Convert.ToDecimal(monto) * 100).ToString();
            }
            else
            {
                montoSinSeparador = monto;
            }
            string RespuestaCardsJson = WSL.Cards.addTransfer(numdocOrigen, numdocDestino, montoSinSeparador, tipoCuenta, (string)HttpContext.Current.Session["login"]);
            if (WSL.Cards.ExceptionServicioCards(RespuestaCardsJson))
            {
                return null;
            }
            RespuestaCards RespuestaCards = (RespuestaCards)JsonConvert.DeserializeObject<RespuestaCards>(RespuestaCardsJson);
            if (Convert.ToDecimal(RespuestaCards.excode) < 0)
            {
                return null;
            }
            else
            {
                return RespuestaCards.exdetail;
            }
        }
    }
}