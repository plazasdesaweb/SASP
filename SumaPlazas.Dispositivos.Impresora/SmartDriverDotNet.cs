using System;
using System.Runtime.InteropServices.Automation;

namespace SumaPlazas.Dispositivos.Impresora
{
    public class SmartDriverDotNet
    {
        public const string NombreImpresora = "XPS Card Printer";
        
        public string Imprimir(string Nombre, string Apellido, string NroTarjeta, string Track1, string Track2, string TipoTarjeta, string Corporacion, string FechaVencimiento)
        {
            try
            {
                if (AutomationFactory.IsAvailable)
                {
                    using (dynamic SmartDriverDotNet = AutomationFactory.CreateObject("SumaPlazas.Librerias.Impresora.CD800.SmartDriverDotNet"))
                    {
                        if (SmartDriverDotNet != null)
                        {
                            //MessageBox.Show("Aqui voy");
                            bool Resultado = SmartDriverDotNet.Imprimir(NombreImpresora, Nombre, Apellido, NroTarjeta, Track1, Track2, TipoTarjeta, Corporacion, FechaVencimiento);
                            //MessageBox.Show("Resultado: " + Resultado );
                            return Resultado.ToString();
                        }
                        else
                        {
                            //MessageBox.Show("Error de Aplicación. No se pudo imprimir en la cd800 1.", "SumaPlazas.Dispositivos.Impresora.CD800.SmartDriverDotNet.Imprimir", MessageBoxButton.OK);
                            return "Error de Automatización: No se pudo crear SumaPlazas.Librerias.Impresora.CD800.SmartDriverDotNet";
                            //return false;
                        }
                    }
                }
               else
                {
                    //MessageBox.Show("Error de Aplicación. No se pudo imprimir en la cd800 2.", "SumaPlazas.Dispositivos.Impresora.CD800.SmartDriverDotNet.Imprimir", MessageBoxButton.OK);
                    return "Error de Automatización: No está disponible";
                }
            }
            //catch
            catch (Exception ex)
            {
                return ("Error de Aplicación: " + ex.Message);
                //return "";
            }
        }

        public string EstadoImpresora()
        {
            try
            {
                if (AutomationFactory.IsAvailable)
                {
                    using (dynamic SmartDriverDotNet = AutomationFactory.CreateObject("SumaPlazas.Librerias.Impresora.CD800.SmartDriverDotNet"))
                    {
                        if (SmartDriverDotNet != null)
                        {
                            string Resultado = SmartDriverDotNet.EstadoImpresora(NombreImpresora);
                            return Resultado;
                        }
                        else
                        {
                            //MessageBox.Show("Error de Aplicación. No se pudo recibir estado cd800 1.", "SumaPlazas.Dispositivos.Impresora.CD800.SmartDriverDotNet.EstadoImpresora", MessageBoxButton.OK);
                            return "Error de Automatización: No se pudo crear SumaPlazas.Librerias.Impresora.CD800.SmartDriverDotNet";                            
                        }
                    }
                }
                else
                {
                    //MessageBox.Show("Error de Aplicación. No se pudo recibir estado cd800 2.", "SumaPlazas.Dispositivos.Impresora.CD800.SmartDriverDotNet.EstadoImpresora", MessageBoxButton.OK);
                    return "Error de Automatización: No está disponible";
                }
            }
            catch (Exception ex)
            {
                return ("Error de Aplicación: " + ex.Message);
                //return "";
            }
        }

        public string ReanudarImpresora()
        {
            try
            {
                if (AutomationFactory.IsAvailable)
                {
                    using (dynamic SmartDriverDotNet = AutomationFactory.CreateObject("SumaPlazas.Librerias.Impresora.CD800.SmartDriverDotNet"))
                    {
                        if (SmartDriverDotNet != null)
                        {
                            string Resultado = SmartDriverDotNet.ReanudarImpresora(NombreImpresora);
                            return Resultado;
                        }
                        else
                        {
                            //MessageBox.Show("Error de Aplicación. No se pudo recibir estado cd800 1.", "SumaPlazas.Dispositivos.Impresora.CD800.SmartDriverDotNet.EstadoImpresora", MessageBoxButton.OK);
                            return "Error de Automatización: No se pudo crear SumaPlazas.Librerias.Impresora.CD800.SmartDriverDotNet";
                        }
                    }
                }
                else
                {
                    //MessageBox.Show("Error de Aplicación. No se pudo recibir estado cd800 2.", "SumaPlazas.Dispositivos.Impresora.CD800.SmartDriverDotNet.EstadoImpresora", MessageBoxButton.OK);
                    return "Error de Automatización: No está disponible";
                }
            }
            catch (Exception ex)
            {
                return ("Error de Aplicación: " + ex.Message);
                //return "";
            }
        }

        public string LimpiarErrores()
        {
            try
            {
                if (AutomationFactory.IsAvailable)
                {
                    using (dynamic SmartDriverDotNet = AutomationFactory.CreateObject("SumaPlazas.Librerias.Impresora.CD800.SmartDriverDotNet"))
                    {
                        if (SmartDriverDotNet != null)
                        {
                            string Resultado = SmartDriverDotNet.LimpiarErrores(NombreImpresora);
                            return Resultado;
                        }
                        else
                        {
                            //MessageBox.Show("Error de Aplicación. No se pudo recibir estado cd800 1.", "SumaPlazas.Dispositivos.Impresora.CD800.SmartDriverDotNet.EstadoImpresora", MessageBoxButton.OK);
                            return "Error de Automatización: No se pudo crear SumaPlazas.Librerias.Impresora.CD800.SmartDriverDotNet";
                        }
                    }
                }
                else
                {
                    //MessageBox.Show("Error de Aplicación. No se pudo recibir estado cd800 2.", "SumaPlazas.Dispositivos.Impresora.CD800.SmartDriverDotNet.EstadoImpresora", MessageBoxButton.OK);
                    return "Error de Automatización: No está disponible";
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
