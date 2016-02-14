using System;
using System.Runtime.InteropServices.Automation;

namespace SumaPlazas.Dispositivos.PinPad
{
    public class Vpos
    {
        public string CrearPin(string CI, string CVV2)
        {
            try
            {
                if (AutomationFactory.IsAvailable)
                {
                    using (dynamic Vpos = AutomationFactory.CreateObject("SumaPlazas.Librerias.PinPad.Vpos"))
                    {
                        if (Vpos != null)
                        {
                            string Resultado = Vpos.CrearPin(CI, CVV2);
                            return Resultado;
                        }
                        else
                        {
                            return "Error de Automatización: No se pudo crear SumaPlazas.Librerias.PinPad.Vpos"; ;
                            //MessageBox.Show("Error de Aplicación: No se pudo crear objeto Vpos.", NombreMetodo, MessageBoxButton.OK);                           
                        }
                    }
                }
                else
                {
                    return "Error de Automatización: No está disponible";
                    //MessageBox.Show("Error de Aplicación: AutomationFactory no disponible.", NombreMetodo, MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                return ("Error de Aplicación: " + ex.Message);
                //return "";
            }
        }

        public string CambiarPin(string CI, string CVV2)
        {
            try
            {
                if (AutomationFactory.IsAvailable)
                {
                    using (dynamic Vpos = AutomationFactory.CreateObject("SumaPlazas.Librerias.PinPad.Vpos"))
                    {
                        if (Vpos != null)
                        {
                            string Resultado = Vpos.CambiarPin(CI, CVV2);
                            return Resultado;
                        }
                        else
                        {
                            return "Error de Automatización: No se pudo crear SumaPlazas.Librerias.PinPad.Vpos"; ;                            
                            //MessageBox.Show("Error de Aplicación. No se pudo cambiar pin 1.", "SumaPlazas.Dispositivos.PinPad.Vpos.CambiarPin", MessageBoxButton.OK);
                        }
                    }
                }
                else
                {
                    return "Error de Automatización: No está disponible";
                    //MessageBox.Show("Error de Aplicación. No se pudo cambiar pin 2.", "SumaPlazas.Dispositivos.PinPad.Vpos.CambiarPin", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                return ("Error de Aplicación: " + ex.Message);
                //return "";
            }
        }

        public string ReiniciarPin(string CI, string CVV2)
        {
            try
            {
                if (AutomationFactory.IsAvailable)
                {
                    using (dynamic Vpos = AutomationFactory.CreateObject("SumaPlazas.Librerias.PinPad.Vpos"))
                    {
                        if (Vpos != null)
                        {
                            string Resultado = Vpos.ReiniciarPin(CI, CVV2);
                            return Resultado;
                        }
                        else
                        {
                            return "Error de Automatización: No se pudo crear SumaPlazas.Librerias.PinPad.Vpos"; ;
                            //MessageBox.Show("Error de Aplicación. No se pudo reiniciar pin 1.", "SumaPlazas.Dispositivos.PinPad.Vpos.ReiniciarPin", MessageBoxButton.OK);
                        }
                    }
                }
                else
                {
                    return "Error de Automatización: No está disponible";
                    //MessageBox.Show("Error de Aplicación. No se pudo reiniciar pin 2.", "SumaPlazas.Dispositivos.PinPad.Vpos.ReiniciarPin", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                return ("Error de Aplicación: " + ex.Message);
                //return "";
            }
        }

    }
}
