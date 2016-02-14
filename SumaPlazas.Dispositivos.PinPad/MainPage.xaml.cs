using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;

namespace SumaPlazas.Dispositivos.PinPad
{
    public partial class MainPage : UserControl
    {
        //valores para la demora delos Workers
        private int demora = 500;

        //parámetros para la impresión
        string ci;
        string cvv2;

        //se escribira en este elemento Htlml "Pin creado", "Pin cambiado", "Pin reiniciado", "No realizado"
        HtmlElement ElementoHtmlControl;

        public MainPage(string CI, string CVV2, string idElementoHtmlControl)
        {
            ci = CI;
            cvv2 = CVV2;

            ElementoHtmlControl = HtmlPage.Document.GetElementById(idElementoHtmlControl);

            if (ElementoHtmlControl == null)
            {
                MessageBox.Show("Error: Elemento de control con id '" + idElementoHtmlControl + "' no encontrado.");
            }

            InitializeComponent();
            HtmlPage.RegisterScriptableObject("MainPage", this);
        }

        #region CrearPin
        [ScriptableMember]
        public void CrearPin()
        {
            bool PermisosElevados = Application.Current.HasElevatedPermissions;
            if (PermisosElevados == true)
            {
                listBox1.Items.Clear();
                ElementoHtmlControl.SetProperty("innerHTML", "No realizado");
                BackgroundWorker w = new BackgroundWorker();
                w.DoWork += new DoWorkEventHandler(w_DoWorkCrearPin);
                w.RunWorkerCompleted += new RunWorkerCompletedEventHandler(w_RunWorkerCompletedCrearPin);
                w.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("Error de Aplicación. No están configurados permisos elevados", "SumaPlazas.Dispositivos.PinPad.MainPage.CrearPin", MessageBoxButton.OK);
            }
        }

        private void w_DoWorkCrearPin(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(demora);
            Dispatcher.BeginInvoke(delegate()
            {
                listBox1.Items.Add("Activando pinpad...");
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            });
            Thread.Sleep(demora);
            Dispatcher.BeginInvoke(delegate()
            {
                listBox1.Items.Add("Activado.");
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            });
        }

        private void w_RunWorkerCompletedCrearPin(object sender, RunWorkerCompletedEventArgs e)
        {
            Vpos Vpos = new Vpos();
            string Resultado = Vpos.CrearPin(ci,cvv2);
            listBox1.Items.Add("Pinpad desactivado.");
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            string[] ArrResultado = Resultado.Split(',');
            if (ArrResultado[0] != "00")
            {
                listBox1.Items.Add("Respuesta recibida: Pin no creado.");
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                listBox1.Items.Add(Resultado);
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
            else
            {
                listBox1.Items.Add("Respuesta recibida: Pin creado.");
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                ElementoHtmlControl.SetProperty("innerHTML", "Pin creado");
            }
        }
        #endregion

        #region CambiarPin
        [ScriptableMember]
        public void CambiarPin()
        {
            bool PermisosElevados = Application.Current.HasElevatedPermissions;
            if (PermisosElevados == true)
            {
                listBox1.Items.Clear();
                ElementoHtmlControl.SetProperty("innerHTML", "No realizado");
                BackgroundWorker w = new BackgroundWorker();
                w.DoWork += new DoWorkEventHandler(w_DoWorkCambiarPin);
                w.RunWorkerCompleted += new RunWorkerCompletedEventHandler(w_RunWorkerCompletedCambiarPin);
                w.RunWorkerAsync();
            }
        }

        private void w_DoWorkCambiarPin(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(demora);
            Dispatcher.BeginInvoke(delegate()
            {
                listBox1.Items.Add("Activando pinpad...");
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            });
            Thread.Sleep(demora);
            Dispatcher.BeginInvoke(delegate()
            {
                listBox1.Items.Add("Activado.");
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            });
        }

        private void w_RunWorkerCompletedCambiarPin(object sender, RunWorkerCompletedEventArgs e)
        {
            Vpos Vpos = new Vpos();
            string Resultado = Vpos.CambiarPin(ci, cvv2);
            string[] ArrResultado = Resultado.Split(',');
            if (ArrResultado[0] != "00")
            {
                listBox1.Items.Add("Respuesta recibida: Pin no cambiado.");
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                listBox1.Items.Add(Resultado);
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
            else
            {
                listBox1.Items.Add("Respuesta recibida: Pin cambiado.");
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                ElementoHtmlControl.SetProperty("innerHTML", "Pin cambiado");
            }
        }
        #endregion

        #region ReiniciarPin
        [ScriptableMember]
        public void ReiniciarPin()
        {
            bool PermisosElevados = Application.Current.HasElevatedPermissions;
            if (PermisosElevados == true)
            {
                listBox1.Items.Clear();
                ElementoHtmlControl.SetProperty("innerHTML", "No realizado");
                BackgroundWorker w = new BackgroundWorker();
                w.DoWork += new DoWorkEventHandler(w_DoWorkReiniciarPin);
                w.RunWorkerCompleted += new RunWorkerCompletedEventHandler(w_RunWorkerCompletedReiniciarPin);
                w.RunWorkerAsync();
            }
        }

        private void w_DoWorkReiniciarPin(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(demora);
            Dispatcher.BeginInvoke(delegate()
            {
                listBox1.Items.Add("Activando pinpad...");
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            });
            Thread.Sleep(demora);
            Dispatcher.BeginInvoke(delegate()
            {
                listBox1.Items.Add("Activado.");
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            });
        }

        private void w_RunWorkerCompletedReiniciarPin(object sender, RunWorkerCompletedEventArgs e)
        {
            Vpos Vpos = new Vpos();
            string Resultado = Vpos.ReiniciarPin(ci, cvv2);
            string[] ArrResultado = Resultado.Split(',');
            if (ArrResultado[0] != "00")
            {
                listBox1.Items.Add("Respuesta recibida: Pin no reiniciado.");
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                listBox1.Items.Add(Resultado);
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
            else
            {
                listBox1.Items.Add("Respuesta recibida: Pin reiniciado.");
                ElementoHtmlControl.SetProperty("innerHTML", "Pin reiniciado");
            }
        }
        #endregion

    }
}
