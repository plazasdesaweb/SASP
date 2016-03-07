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
            //string montoSinSeparador = Math.Truncate(Convert.ToDecimal(monto) * 100).ToString();
            string RespuestaCardsJson = WSL.Cards.addTransfer(numdocOrigen, numdocDestino, monto, tipoCuenta);
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