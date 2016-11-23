using FluxJpeg.Core;
using FluxJpeg.Core.Decoder;
using FluxJpeg.Core.Encoder;
using FluxJpeg.Core.Filtering;
using System;
using System.IO;
using System.Runtime.InteropServices.Automation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

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
                return ("Error de Aplicación: " + ex.Message);
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

        public string ImportarImagen(string NombreArchivo)
        {
            string MyPicturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string FilePath = MyPicturesPath + "\\" + NombreArchivo + ".jpg";
            string TempPath = MyPicturesPath + "\\Temp.jpg";
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                //Sólo se permiten archivos .jpg
                dialog.Filter = "Imagenes |*.jpg;";
                if (dialog.ShowDialog() == true)
                {
                    if (File.Exists(FilePath))
                    {
                        File.Delete(FilePath);
                    }
                    File.Copy(dialog.File.FullName, FilePath);
                    long fileLength = new FileInfo(FilePath).Length;
                    //MessageBox.Show("tamaño anterior: " + fileLength);
                    //Si la imagen escaneada es mayor 50Kb, se itera reduciendo la imagen a la mitad de su tamaño
                    while (fileLength > 51200)
                    {
                        if (File.Exists(TempPath))
                        {
                            File.Delete(TempPath);
                        }                        
                        if (File.Exists(FilePath))
                        {
                            File.Copy(FilePath, TempPath);
                            File.Delete(FilePath);
                        }
                        using (Stream Stream = File.OpenRead(TempPath))
                        {
                            BitmapImage image = new BitmapImage();
                            image.SetSource(Stream);
                            MemoryStream outStream = new MemoryStream();
                            Stream.Seek(0, SeekOrigin.Begin);
                            JpegDecoder decoder = new JpegDecoder(Stream);
                            DecodedJpeg jpeg = decoder.Decode();
                            ImageResizer resizer = new ImageResizer(jpeg.Image);
                            FluxJpeg.Core.Image small = resizer.Resize(Convert.ToInt32(Math.Floor(image.PixelWidth / 2)), Convert.ToInt32(Math.Floor(image.PixelHeight / 2)), ResamplingFilters.NearestNeighbor);
                            JpegEncoder encoder = new JpegEncoder(small, 90, outStream);
                            encoder.Encode();
                            outStream.Seek(0, SeekOrigin.Begin);
                            int bufferSize = Convert.ToInt32(outStream.Length);
                            byte[] buffer = new byte[bufferSize];
                            outStream.Read(buffer, 0, bufferSize);
                            outStream.Close();
                            Stream.Close();
                            File.WriteAllBytes(FilePath, buffer);
                        }
                        if (File.Exists(TempPath))
                        {
                            File.Delete(TempPath);
                        }
                        fileLength = new FileInfo(FilePath).Length;
                    }
                    return FilePath;
                }
                else
                {
                    return "Error: No se selecciono ningún archivo";
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
