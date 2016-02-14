using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SumaPlazas.Dispositivos.Escaner
{
    public partial class MainPage : UserControl
    {
        //Nombre para el archivo escaneado
        string NombreArchivo;

        //valores para la demora delos Workers
        private int demora = 500;

        //se escribira en este elemento Htlml "Escaneada", "No escaneada"
        HtmlElement ElementoHtmlControl;

        public MainPage(string nombreArchivo, string idElementoHtmlControl)
        {
            NombreArchivo = nombreArchivo;
            ElementoHtmlControl = HtmlPage.Document.GetElementById(idElementoHtmlControl);

            if (ElementoHtmlControl == null)
            {
                MessageBox.Show("Error: Elemento de control con id '" + idElementoHtmlControl + "' no encontrado.");
            }

            InitializeComponent();
            HtmlPage.RegisterScriptableObject("MainPage", this);
        }

        [ScriptableMember]
        public void Escanear()
        {
            bool PermisosElevados = Application.Current.HasElevatedPermissions;
            if (PermisosElevados == true)
            {
                listBox1.Items.Clear();
                ElementoHtmlControl.SetProperty("innerHTML", "No Escaneada");            
                BackgroundWorker w = new BackgroundWorker();
                w.DoWork += new DoWorkEventHandler(w_DoWorkEscanear);
                w.RunWorkerCompleted += new RunWorkerCompletedEventHandler(w_RunWorkerCompletedEscanear);
                w.RunWorkerAsync();                
            }
            else
            {
                MessageBox.Show("Error de Aplicación: No están configurados permisos elevados.", "SumaPlazas.Dispositivos.Escaner.MainPage.Escanear", MessageBoxButton.OK);
            }
        }

        private void w_DoWorkEscanear(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(demora);
            Dispatcher.BeginInvoke(delegate()
            {
                listBox1.Items.Add("Activando escaner...");
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            });
            Thread.Sleep(demora);
            Dispatcher.BeginInvoke(delegate()
            {
                listBox1.Items.Add("Activado.");
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            });                  
        }

        private void w_RunWorkerCompletedEscanear(object sender, RunWorkerCompletedEventArgs e) 
        {
            EscanerWIA EscanerWia = new EscanerWIA();
            string RutaArchivo = EscanerWia.ObtenerImagen(NombreArchivo);
            listBox1.Items.Add("Escaner desactivado.");
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            //listBox1.Items.Add("Resultado de la operación: " + RutaArchivo);
            //listBox1.SelectedIndex = listBox1.Items.Count - 1;
            if (RutaArchivo.Contains("Error"))
            //if (RutaArchivo == "")
            {
                //listBox1.Items.Add("Resultado de la operación: " + RutaArchivo);
                //listBox1.SelectedIndex = listBox1.Items.Count - 1;

                listBox1.Items.Add(RutaArchivo);
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                //MessageBox.Show("Error de Aplicación: No se pudo escanear el documento.", "SumaPlazas.Dispositivos.Escaner.MainPage.Escanear", MessageBoxButton.OK);
                //listBox1.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                using (Stream Stream = File.OpenRead(RutaArchivo))
                {
                    BitmapImage BitmapImage = new BitmapImage();
                    BitmapImage.SetSource(Stream);
                    listBox1.Visibility = System.Windows.Visibility.Collapsed;
                    scannedImage.Source = BitmapImage;
                }
                ElementoHtmlControl.SetProperty("innerHTML", "Escaneada");
            }             
        }

        [ScriptableMember]
        public void Limpiar()
        {
            ElementoHtmlControl.SetProperty("innerHTML", "No Escaneada");
            EscanerWIA EscanerWia = new EscanerWIA();
            EscanerWia.LimpiarImagen(NombreArchivo);
            scannedImage.Source = null;
            listBox1.Items.Clear();
            listBox1.Visibility = System.Windows.Visibility.Visible;            
        }

    }
}
