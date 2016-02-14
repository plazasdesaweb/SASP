using System;
using System.Windows;

namespace SumaPlazas.Dispositivos.Impresora
{
    public partial class App : Application
    {

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // parámetro llamado Nombre
            string Nombre = e.InitParams["Nombre"];
            // parámetro llamado Apellido
            string Apellido = e.InitParams["Apellido"];
            // parámetro llamado NroTarjeta
            string NroTarjeta = e.InitParams["NroTarjeta"];
            // parámetro llamado Track1
            string Track1 = e.InitParams["Track1"];
            // parámetro llamado Track2
            string Track2 = e.InitParams["Track2"];
            // parámetro llamado TipoTarjeta
            string TipoTarjeta = e.InitParams["TipoTarjeta"];
            // parámetro llamado Corporacion
            string Corporacion = e.InitParams["Corporacion"];
            // parámetro llamado FechaVencimiento
            string FechaVencimiento = e.InitParams["FechaVencimiento"];

            // parámetro llamado idElementoHtmlControl
            string idElementoHtmlControl = e.InitParams["idElementoHtmlControl"];

            //Pasar a la página principal como parámetros  
            this.RootVisual = new MainPage(Nombre, Apellido, NroTarjeta, Track1, Track2, TipoTarjeta, Corporacion, FechaVencimiento, idElementoHtmlControl);
        }

        private void Application_Exit(object sender, EventArgs e)
        {

        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }

        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}
