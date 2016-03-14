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
            int intentos;
            string respuesta = "";
            //Se intenta la operación 3 veces, antes de fallar
            for (intentos = 0; intentos <= 3; intentos++)
            {
                //Se llama al servicio para verificar q este activo
                //SERVICIO WSL.Cards.getClient !
                string clienteCardsJson = WSL.Cards.getClient(numdocOrigen.Substring(2));
                if (WSL.Cards.ExceptionServicioCards(clienteCardsJson))
                {
                    respuesta = "Servicio no responde";
                    intentos++;
                }
                else
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
                    string RespuestaCardsJson = WSL.Cards.addTransfer(numdocOrigen.Substring(2), numdocDestino.Substring(2), montoSinSeparador, tipoCuenta, (string)HttpContext.Current.Session["login"]);
                    if (WSL.Cards.ExceptionServicioCards(RespuestaCardsJson))
                    {
                        ExceptionJSON exceptionJson = (ExceptionJSON)JsonConvert.DeserializeObject<ExceptionJSON>(RespuestaCardsJson);
                        respuesta = exceptionJson.detail + "-" + exceptionJson.source;
                        intentos++;
                    }
                    else
                    {
                        RespuestaCards RespuestaCards = (RespuestaCards)JsonConvert.DeserializeObject<RespuestaCards>(RespuestaCardsJson);
                        if (Convert.ToDecimal(RespuestaCards.excode) < 0)
                        {
                            return RespuestaCards.exdetail;
                        }
                        else
                        {
                            return RespuestaCards.exdetail;
                        }
                    }
                }
            }
            return respuesta;
        }

    }
}