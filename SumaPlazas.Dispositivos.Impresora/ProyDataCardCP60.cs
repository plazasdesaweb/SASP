using System;
using System.Runtime.InteropServices.Automation;

namespace SumaPlazas.Dispositivos.Impresora
{
    public class ProyDataCardCP60
    {
        public const string NombreImpresora = "CP Printer";

        public string Imprimir(ref string Nombre, ref string Apellido, ref string NroTarjeta, ref string Track1, ref string Track2, ref string TipoTarjeta, ref string Corporacion, ref string FechaVencimiento)
        {
            try
            {
                if (AutomationFactory.IsAvailable)
                {
                    using (dynamic ProyDataCardCP60 = AutomationFactory.CreateObject("SumaPlazas.Librerias.Impresora.CP60.ProyDataCardCP60"))
                    {
                        if (ProyDataCardCP60 != null)
                        {
                            bool Resultado = ProyDataCardCP60.Imprimir(NombreImpresora, Nombre, Apellido, NroTarjeta, Track1, Track2, TipoTarjeta, Corporacion, FechaVencimiento);
                            return Resultado.ToString();
                        }
                        else
                        {
                            //MessageBox.Show("Error de Aplicación. No se pudo imprimir en la cp60 1.", "SumaPlazas.Dispositivos.Impresora.CP60.ProyDataCardCP60.Imprimir", MessageBoxButton.OK);
                            return "Error de Automatización: No se pudo crear SumaPlazas.Librerias.Impresora.CP60.ProyDataCardCP60";
                            //return false;
                        }
                    }
                }
                else
                {
                    //MessageBox.Show("Error de Aplicación. No se pudo imprimir en la cp60 2.", "SumaPlazas.Dispositivos.Impresora.CP60.ProyDataCardCP60.Imprimir", MessageBoxButton.OK);
                    return "Error de Automatización: No está disponible";
                }
            }
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
                    using (dynamic ProyDataCardCP60 = AutomationFactory.CreateObject("SumaPlazas.Librerias.Impresora.CP60.ProyDataCardCP60"))
                    {
                        if (ProyDataCardCP60 != null)
                        {
                            string Resultado = ProyDataCardCP60.EstadoImpresora(NombreImpresora);
                            return Resultado;
                        }
                        else
                        {
                            //MessageBox.Show("Error de Aplicación. No se pudo recibir estado cp60 1.", "SumaPlazas.Dispositivos.Impresora.CP60ProyDataCardCP60.EstadoImpresora", MessageBoxButton.OK);
                            return "Error de Automatización: No se pudo crear SumaPlazas.Librerias.Impresora.CP60.ProyDataCardCP60";                            
                        }
                    }
                }
               else
                {
                    //MessageBox.Show("Error de Aplicación. No se pudo recibir estado cp60 2.", "SumaPlazas.Librerias.Impresora.CP60.ProyDataCardCP60.EstadoImpresora", MessageBoxButton.OK);
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
                    using (dynamic ProyDataCardCP60 = AutomationFactory.CreateObject("SumaPlazas.Librerias.Impresora.CP60.ProyDataCardCP60"))
                    {
                        if (ProyDataCardCP60 != null)
                        {
                            string Resultado = ProyDataCardCP60.ReanudarImpresora(NombreImpresora);
                            return Resultado;
                        }
                        else
                        {
                            //MessageBox.Show("Error de Aplicación. No se pudo recibir estado cp60 1.", "SumaPlazas.Dispositivos.Impresora.CP60ProyDataCardCP60.EstadoImpresora", MessageBoxButton.OK);
                            return "Error de Automatización: No se pudo crear SumaPlazas.Librerias.Impresora.CP60.ProyDataCardCP60";
                        }
                    }
                }
                else
                {
                    //MessageBox.Show("Error de Aplicación. No se pudo recibir estado cp60 2.", "SumaPlazas.Librerias.Impresora.CP60.ProyDataCardCP60.EstadoImpresora", MessageBoxButton.OK);
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
                    using (dynamic ProyDataCardCP60 = AutomationFactory.CreateObject("SumaPlazas.Librerias.Impresora.CP60.ProyDataCardCP60"))
                    {
                        if (ProyDataCardCP60 != null)
                        {
                            string Resultado = ProyDataCardCP60.LimpiarErrores(NombreImpresora);
                            return Resultado;
                        }
                        else
                        {
                            //MessageBox.Show("Error de Aplicación. No se pudo recibir estado cp60 1.", "SumaPlazas.Dispositivos.Impresora.CP60ProyDataCardCP60.EstadoImpresora", MessageBoxButton.OK);
                            return "Error de Automatización: No se pudo crear SumaPlazas.Librerias.Impresora.CP60.ProyDataCardCP60";
                        }
                    }
                }
                else
                {
                    //MessageBox.Show("Error de Aplicación. No se pudo recibir estado cp60 2.", "SumaPlazas.Librerias.Impresora.CP60.ProyDataCardCP60.EstadoImpresora", MessageBoxButton.OK);
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
