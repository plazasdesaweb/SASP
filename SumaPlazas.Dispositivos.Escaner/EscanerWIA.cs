using System;
using System.IO;
using System.Runtime.InteropServices.Automation;

namespace SumaPlazas.Dispositivos.Escaner
{
    public class EscanerWIA
    {
        public string ObtenerImagen(string NombreArchivo)
        {
            try
            {
                if (AutomationFactory.IsAvailable)
                {
                    using (dynamic EscanerWIA = AutomationFactory.CreateObject("SumaPlazas.Librerias.Escaner.EscanerWIA"))
                    {
                        if (EscanerWIA != null)
                        {
                            string FilePath = EscanerWIA.ObtenerImagen(NombreArchivo);
                            //if (FilePath != "")
                            //{
                            //    long fileLength = new FileInfo(FilePath).Length;
                            //    if (fileLength > 51200)
                            //    {
                            //        //Hay q cambiar el tamaño de la imagen escaneada....
                            //        MessageBox.Show("El tamaño del archivo escaneado es muy grande: " + fileLength + " bytes. El máximo permitido es 50 Kb.");
                            //    }
                            //    else
                            //    {
                            //        MessageBox.Show("El tamaño del archivo escaneado es: " + fileLength + " bytes.");
                            //    }
                            //}
                            //MessageBox.Show("FilePath: " + FilePath);
                            return FilePath;
                        }
                        else
                        {
                            //MessageBox.Show("EscanerWia es null");
                            return "Error de Automatización: No se pudo crear SumaPlazas.Librerias.Escaner.EscanerWIA";
                        }
                    }
                }
                else
                {
                    //MessageBox.Show("No hay automatizacion");
                    return "Error de Automatización: No está disponible";
                }
            }
            catch (Exception ex)
            {
                return("Error de Aplicación: " + ex.Message);
                //return "";
            }
        }

        public void LimpiarImagen(string NombreArchivo)
        {
            string MyPicturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string FilePath = string.Format(MyPicturesPath + "\\" + NombreArchivo + ".jpg");
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }
    }
}
