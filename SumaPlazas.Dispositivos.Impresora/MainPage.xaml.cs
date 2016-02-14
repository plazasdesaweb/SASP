using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;

namespace SumaPlazas.Dispositivos.Impresora
{
    public partial class MainPage : UserControl
    {
        //objetos impresora
        SmartDriverDotNet smartDriverDotNet;
        ProyDataCardCP60 proyDataCardCP60;

        //Nombre de la impresora activa
        String Impresora = "";

        //valores para la demora de los Workers
        private int demora = 500;
        private int demora2 = 1000;

        //Operacion solicitada
        string Operacion = "";

        //parámetros para la impresión
        string nombre;
        string apellido;
        string nroTarjeta;
        string track1;
        string track2;
        string tipoTarjeta;
        string corporacion;
        string fechaVencimiento;

        //se escribira en este elemento Htlml "Impresa", "No impresa"
        HtmlElement ElementoHtmlControl;

        public MainPage(string Nombre, string Apellido, string NroTarjeta, string Track1, string Track2, string TipoTarjeta, string Corporacion, string FechaVencimiento, string idElementoHtmlControl)
        {
            nombre = Nombre;
            apellido = Apellido;
            nroTarjeta = NroTarjeta;
            track1 = Track1;
            track2 = Track2;
            tipoTarjeta = TipoTarjeta;
            corporacion = Corporacion;
            fechaVencimiento = FechaVencimiento;

            ElementoHtmlControl = HtmlPage.Document.GetElementById(idElementoHtmlControl);

            if (ElementoHtmlControl == null)
            {
                MessageBox.Show("Error: Elemento de control con id '" + idElementoHtmlControl + "' no encontrado.");
            }

            InitializeComponent();
            HtmlPage.RegisterScriptableObject("MainPage", this);
        }

        #region DetectarImpresora
        private void DetectarImpresora()
        {
            listBox1.Items.Clear();
            BackgroundWorker w = new BackgroundWorker();
            w.DoWork += new DoWorkEventHandler(w_DoWorkDetectarImpresora);
            w.RunWorkerCompleted += new RunWorkerCompletedEventHandler(w_RunWorkerCompletedDetectarImpresora);
            w.RunWorkerAsync();
        }

        private void w_DoWorkDetectarImpresora(object sender, DoWorkEventArgs e)
        {
            //int demora = 500;
            //int demora2 = 1000;
            Impresora = "";
            bool PermisosElevados = Application.Current.HasElevatedPermissions;
            if (PermisosElevados == true)
            {
                Thread.Sleep(demora);
                Dispatcher.BeginInvoke(delegate()
                {
                    listBox1.Items.Add("Iniciando detección de impresora...");
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                });
                Thread.Sleep(demora);
                Dispatcher.BeginInvoke(delegate()
                {
                    listBox1.Items.Add("Estableciendo comunicación con dispositivos...");
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                });
                smartDriverDotNet = new SmartDriverDotNet();
                string ResultadoCD800 = smartDriverDotNet.EstadoImpresora();
                Thread.Sleep(demora2);
                Dispatcher.BeginInvoke(delegate()
                {
                    listBox1.Items.Add("Impresora detectada: '" + SmartDriverDotNet.NombreImpresora + "', modelo: 'CD800'.");
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                });
                if (ResultadoCD800 != "")
                {
                    Thread.Sleep(demora);
                    Dispatcher.BeginInvoke(delegate()
                    {
                        listBox1.Items.Add("Estado: '" + SmartDriverDotNet.NombreImpresora + "', modelo: 'CD800': " + ResultadoCD800 + ".");
                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    });
                    if (ResultadoCD800.ToLower() == "la impresora está respondiendo")
                    {
                        Thread.Sleep(demora);
                        Dispatcher.BeginInvoke(delegate()
                        {
                            listBox1.Items.Add("Resultado: Se utilizará este dispositivo para imprimir.");
                            listBox1.SelectedIndex = listBox1.Items.Count - 1;
                        });
                        Impresora = SmartDriverDotNet.NombreImpresora;
                    }
                    else
                    {
                        Thread.Sleep(demora);
                        Dispatcher.BeginInvoke(delegate()
                        {
                            listBox1.Items.Add("Resultado: Imposible asignar la impresora '" + SmartDriverDotNet.NombreImpresora + "', modelo: 'CD800'.");
                            listBox1.SelectedIndex = listBox1.Items.Count - 1;
                        });
                        Thread.Sleep(demora);
                        Dispatcher.BeginInvoke(delegate()
                        {
                            listBox1.Items.Add("Continuando detección de impresora...");
                            listBox1.SelectedIndex = listBox1.Items.Count - 1;
                        });
                        Thread.Sleep(demora);
                        Dispatcher.BeginInvoke(delegate()
                        {
                            listBox1.Items.Add("Re-estableciendo comunicación con dispositivos...");
                            listBox1.SelectedIndex = listBox1.Items.Count - 1;
                        });
                        proyDataCardCP60 = new ProyDataCardCP60();
                        string ResultadoCP60 = proyDataCardCP60.EstadoImpresora();
                        Thread.Sleep(demora2);
                        Dispatcher.BeginInvoke(delegate()
                        {
                            listBox1.Items.Add("Impresora detectada: '" + ProyDataCardCP60.NombreImpresora + "', modelo: 'CP60'.");
                            listBox1.SelectedIndex = listBox1.Items.Count - 1;
                        });
                        if (ResultadoCP60 != "")
                        {
                            Thread.Sleep(demora);
                            Dispatcher.BeginInvoke(delegate()
                            {
                                listBox1.Items.Add("Estado: '" + ProyDataCardCP60.NombreImpresora + "', modelo: 'CP60': " + ResultadoCP60 + ".");
                                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                            });
                            if (ResultadoCP60.ToLower() == "la impresora está respondiendo")
                            {
                                Thread.Sleep(demora);
                                Dispatcher.BeginInvoke(delegate()
                                {
                                    listBox1.Items.Add("Resultado: Se utilizará este dispositivo para imprimir.");
                                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                                });
                                Impresora = ProyDataCardCP60.NombreImpresora;
                            }
                        }
                        else
                        {
                            Thread.Sleep(demora);
                            Dispatcher.BeginInvoke(delegate()
                            {
                                listBox1.Items.Add("Resultado: Imposible asignar la impresora '" + ProyDataCardCP60.NombreImpresora + "', modelo: 'CP60'.");
                                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                            });
                        }
                    }
                }
                else
                {
                    Thread.Sleep(demora);
                    Dispatcher.BeginInvoke(delegate()
                    {
                        listBox1.Items.Add("Resultado: No es posible comunicarse con la impresora '" + SmartDriverDotNet.NombreImpresora + "', modelo: 'CD800'.");
                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    });
                    Thread.Sleep(demora);
                    Dispatcher.BeginInvoke(delegate()
                    {
                        listBox1.Items.Add("Continuando detección de impresora...");
                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    });
                    Thread.Sleep(demora);
                    Dispatcher.BeginInvoke(delegate()
                    {
                        listBox1.Items.Add("Re-estableciendo comunicación con dispositivos...");
                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    });
                    proyDataCardCP60 = new ProyDataCardCP60();
                    string ResultadoCP60 = proyDataCardCP60.EstadoImpresora();
                    Thread.Sleep(demora2);
                    Dispatcher.BeginInvoke(delegate()
                    {
                        listBox1.Items.Add("Impresora detectada: '" + ProyDataCardCP60.NombreImpresora + "', modelo: 'CP60'.");
                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    });
                    if (ResultadoCP60 != "")
                    {
                        Thread.Sleep(demora);
                        Dispatcher.BeginInvoke(delegate()
                        {
                            listBox1.Items.Add("Estado: '" + ProyDataCardCP60.NombreImpresora + "', modelo: 'CP60': " + ResultadoCP60 + ".");
                            listBox1.SelectedIndex = listBox1.Items.Count - 1;
                        });
                        if (ResultadoCP60.ToLower() == "la impresora está respondiendo")
                        {
                            Thread.Sleep(demora);
                            Dispatcher.BeginInvoke(delegate()
                            {
                                listBox1.Items.Add("Resultado: Se utilizará este dispositivo para imprimir.");
                                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                            });
                            Impresora = ProyDataCardCP60.NombreImpresora;
                        }
                        else
                        {
                            Thread.Sleep(demora);
                            Dispatcher.BeginInvoke(delegate()
                            {
                                listBox1.Items.Add("Resultado: Imposible asignar la impresora '" + ProyDataCardCP60.NombreImpresora + "', modelo: 'CP60'.");
                                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                            });
                        }
                    }
                    else
                    {
                        Thread.Sleep(demora);
                        Dispatcher.BeginInvoke(delegate()
                        {
                            listBox1.Items.Add("Resultado: No es posible comunicarse con la impresora '" + ProyDataCardCP60.NombreImpresora + "', modelo: 'CP60'.");
                            listBox1.SelectedIndex = listBox1.Items.Count - 1;
                        });
                    }
                }
            }
            else
            {
                MessageBox.Show("Error de Aplicación: No están configurados permisos elevados.", "SumaPlazas.Dispositivos.Impresora.MainPage.DetectarImpresora", MessageBoxButton.OK);
            }
            Thread.Sleep(demora);
            Dispatcher.BeginInvoke(delegate()
            {
                listBox1.Items.Add("Finalizó detección de impresora...");
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            });
            if (Impresora == "")
            {
                Dispatcher.BeginInvoke(delegate()
                {
                    listBox1.Items.Add("Error: No se pudo determinar dispositivo de impresión. Operación cancelada.");
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                });
            }
        }

        private void w_RunWorkerCompletedDetectarImpresora(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Operacion == "ImprimirTarjeta" && Impresora != "")
            {
                BackgroundWorker w = new BackgroundWorker();
                w.DoWork += new DoWorkEventHandler(w_DoWorkImprimirTarjeta);
                w.RunWorkerCompleted += new RunWorkerCompletedEventHandler(w_RunWorkerCompletedImprimirTarjeta);
                w.RunWorkerAsync();
            }
            else if (Operacion == "ReanudarImpresora" && Impresora != "")
            {
                BackgroundWorker w = new BackgroundWorker();
                w.DoWork += new DoWorkEventHandler(w_DoWorkReanudarImpresora);
                w.RunWorkerCompleted += new RunWorkerCompletedEventHandler(w_RunWorkerCompletedReanudarImpresora);
                w.RunWorkerAsync();
            }
            else if (Operacion == "LimpiarErrores" && Impresora != "")
            {
                BackgroundWorker w = new BackgroundWorker();
                w.DoWork += new DoWorkEventHandler(w_DoWorkLimpiarErrores);
                w.RunWorkerCompleted += new RunWorkerCompletedEventHandler(w_RunWorkerCompletedLimpiarErrores);
                w.RunWorkerAsync();
            }
            else if (Impresora == "")
            {
            }
            else
            {
                MessageBox.Show("Error de Aplicación. Operacion no definida.", "SumaPlazas.Dispositivos.Impresora.MainPage.w_RunWorkerCompletedDetectarImpresora", MessageBoxButton.OK);
            }

        }
        #endregion

        //OJO: NA
        //Hay que revisar las dll nativas de cada modelo de impresora para poner a funcionar los metodos ReanudarImpresora y LimpiarErrores. Ahorita no hacen nada.
        #region ReanudarImpresora
        [ScriptableMember]
        public void ReanudarImpresora()
        {
            bool PermisosElevados = Application.Current.HasElevatedPermissions;
            if (PermisosElevados == true)
            {
                DetectarImpresora();
            }
            else
            {
                MessageBox.Show("Error de Aplicación. No están configurados permisos elevados.", "SumaPlazas.Dispositivos.Impresora.MainPage.ReanudarImpresora", MessageBoxButton.OK);
            }
            Operacion = "ReanudarImpresora";
            //BackgroundWorker w = new BackgroundWorker();
            //w.DoWork += new DoWorkEventHandler(w_DoWorkReanudarImpresora);
            //w.RunWorkerCompleted += new RunWorkerCompletedEventHandler(w_RunWorkerCompletedReanudarImpresora);
            //w.RunWorkerAsync();
        }

        private void w_DoWorkReanudarImpresora(object sender, DoWorkEventArgs e)
        {
            if (Impresora == ProyDataCardCP60.NombreImpresora)
            {
                Thread.Sleep(demora);
                Dispatcher.BeginInvoke(delegate()
                {
                    listBox1.Items.Add("Iniciando reanudación de impresora...dispositivo asignado: '" + ProyDataCardCP60.NombreImpresora + "', modelo: 'CP60'");
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                });
                string Resultado = proyDataCardCP60.ReanudarImpresora();
                Thread.Sleep(demora);
                Dispatcher.BeginInvoke(delegate()
                {
                    listBox1.Items.Add("Resultado de la operación: " + Resultado + ".");
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                });
            }
            else if (Impresora == SmartDriverDotNet.NombreImpresora)
            {
                Thread.Sleep(demora);
                Dispatcher.BeginInvoke(delegate()
                {
                    listBox1.Items.Add("Iniciando reanudación de impresora...dispositivo asignado: '" + SmartDriverDotNet.NombreImpresora + "', modelo: 'CD800'");
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                });
                string Resultado = smartDriverDotNet.ReanudarImpresora();
                Thread.Sleep(demora);
                Dispatcher.BeginInvoke(delegate()
                {
                    listBox1.Items.Add("Resultado de la operación: " + Resultado + ".");
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                });
            }
            else
            {
                Thread.Sleep(demora);
                Dispatcher.BeginInvoke(delegate()
                {
                    listBox1.Items.Add("Error: No es posible comunicarse con la impresora.'");
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                });
            }
        }

        private void w_RunWorkerCompletedReanudarImpresora(object sender, RunWorkerCompletedEventArgs e)
        {
            listBox1.Items.Add("Finalizó reanudación de impresora...");
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }
        #endregion

        #region LimpiarErrores
        [ScriptableMember]
        public void LimpiarErrores()
        {
            bool PermisosElevados = Application.Current.HasElevatedPermissions;
            if (PermisosElevados == true)
            {
                DetectarImpresora();
            }
            else
            {
                MessageBox.Show("Error de Aplicación. No están configurados permisos elevados.", "SumaPlazas.Dispositivos.Impresora.MainPage.ReanudarImpresora", MessageBoxButton.OK);
            }
            Operacion = "LimpiarErrores";
            //BackgroundWorker w = new BackgroundWorker();
            //w.DoWork += new DoWorkEventHandler(w_DoWorkLimpiarErrores);
            //w.RunWorkerCompleted += new RunWorkerCompletedEventHandler(w_RunWorkerCompletedLimpiarErrores);
            //w.RunWorkerAsync();
        }

        private void w_DoWorkLimpiarErrores(object sender, DoWorkEventArgs e)
        {
            if (Impresora == ProyDataCardCP60.NombreImpresora)
            {
                Thread.Sleep(demora);
                Dispatcher.BeginInvoke(delegate()
                {
                    listBox1.Items.Add("Iniciando limpieza de errores de impresora...dispositivo asignado: '" + ProyDataCardCP60.NombreImpresora + "', modelo: 'CP60'");
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                });
                string Resultado = proyDataCardCP60.ReanudarImpresora();
                Thread.Sleep(demora);
                Dispatcher.BeginInvoke(delegate()
                {
                    listBox1.Items.Add("Resultado de la operación: " + Resultado + ".");
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                });
            }
            else if (Impresora == SmartDriverDotNet.NombreImpresora)
            {
                Thread.Sleep(demora);
                Dispatcher.BeginInvoke(delegate()
                {
                    listBox1.Items.Add("Iniciando limpieza de errores de impresora...dispositivo asignado: '" + SmartDriverDotNet.NombreImpresora + "', modelo: 'CD800'");
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                });
                string Resultado = smartDriverDotNet.ReanudarImpresora();
                Thread.Sleep(demora);
                Dispatcher.BeginInvoke(delegate()
                {
                    listBox1.Items.Add("Resultado de la operación: " + Resultado + ".");
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                });
            }
            else
            {
                Thread.Sleep(demora);
                Dispatcher.BeginInvoke(delegate()
                {
                    listBox1.Items.Add("Error: No es posible comunicarse con la impresora.'");
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                });
            }
        }

        private void w_RunWorkerCompletedLimpiarErrores(object sender, RunWorkerCompletedEventArgs e)
        {
            listBox1.Items.Add("Finalizó limpieza de errores de impresora...");
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }
        #endregion

        #region ImprimirTarjeta
        [ScriptableMember]
        public void ImprimirTarjeta()
        {
            bool PermisosElevados = Application.Current.HasElevatedPermissions;
            if (PermisosElevados == true)
            {
                //ElementoHtmlControl.SetProperty("value", "No impresa");
                ElementoHtmlControl.SetProperty("innerHTML", "No impresa");                
                DetectarImpresora();
            }
            else
            {
                MessageBox.Show("Error de Aplicación. No están configurados permisos elevados.", "SumaPlazas.Dispositivos.Impresora.MainPage.ImprimirTarjeta", MessageBoxButton.OK);
            }
            Operacion = "ImprimirTarjeta";
            //BackgroundWorker w = new BackgroundWorker();
            //w.DoWork += new DoWorkEventHandler(w_DoWorkImprimirTarjeta);
            //w.RunWorkerCompleted += new RunWorkerCompletedEventHandler(w_RunWorkerCompletedImprimirTarjeta);
            //w.RunWorkerAsync();
        }

        private void w_DoWorkImprimirTarjeta(object sender, DoWorkEventArgs e)
        {
            //int demora = 500;
            if (Impresora == ProyDataCardCP60.NombreImpresora)
            {
                Thread.Sleep(demora);
                Dispatcher.BeginInvoke(delegate()
                {
                    listBox1.Items.Add("Iniciando impresión de tarjeta...dispositivo asignado: '" + ProyDataCardCP60.NombreImpresora + "', modelo: 'CP60'");
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                });
                Thread.Sleep(demora);
                Dispatcher.BeginInvoke(delegate()
                {
                    listBox1.Items.Add("Los datos a imprimir son: " + nombre + "," + apellido + "," + nroTarjeta); //+ "," + track1 + "," + track2 + "," + tipoTarjeta + "," + corporacion + "," + fechaVencimiento);
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                });
                string Resultado = proyDataCardCP60.Imprimir(ref nombre, ref apellido, ref nroTarjeta, ref track1, ref track2, ref tipoTarjeta, ref corporacion, ref fechaVencimiento);
                //La pregunta la haremos en la página web, no en el silverlight
                //Respuesta = MessageBox.Show("¿Se imprimió correctamente la tarjeta?", "Pregunta", MessageBoxButton.);
                //Thread.Sleep(demora);
                //Dispatcher.BeginInvoke(delegate()
                //{
                //    listBox1.Items.Add("Resultado de la operación: " + Resultado);
                //    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                //});
                //if (Resultado == true)
                //{
                //    MessageBox.Show("Impresa");
                //    ElementoHtmlControl.SetProperty("innerHTML", "Impresa");
                //}
                //else
                //{
                //    MessageBox.Show("No impresa");
                //    ElementoHtmlControl.SetProperty("innerHTML", "No impresa");
                //}
            }
            else if (Impresora == SmartDriverDotNet.NombreImpresora)
            {
                Thread.Sleep(demora);
                Dispatcher.BeginInvoke(delegate()
                {
                    listBox1.Items.Add("Iniciando impresión de tarjeta...dispositivo asignado: '" + SmartDriverDotNet.NombreImpresora + "', modelo: 'CD800'");
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                });
                Thread.Sleep(demora);
                Dispatcher.BeginInvoke(delegate()
                {
                    listBox1.Items.Add("Los datos a imprimir son: " + nombre + "," + apellido + "," + nroTarjeta); // + "," + track1 + "," + track2 + "," + tipoTarjeta + "," + corporacion + "," + fechaVencimiento);
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                });
                string Resultado = smartDriverDotNet.Imprimir(nombre, apellido, nroTarjeta, track1, track2, tipoTarjeta, corporacion, fechaVencimiento);
                //La pregunta la haremos en la página web, no en el silverlight
                //Respuesta = MessageBox.Show("¿Se imprimió correctamente la tarjeta?", "Pregunta", MessageBoxButton.);
                //Thread.Sleep(demora);
                //Dispatcher.BeginInvoke(delegate()
                //{
                //    listBox1.Items.Add("Resultado de la operación: " + Resultado);
                //    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                //});
                //if (Resultado == true)
                //{
                //    MessageBox.Show("Impresa");
                //    ElementoHtmlControl.SetProperty("innerHTML", "Impresa");
                //}
                //else
                //{
                //    MessageBox.Show("No impresa");
                //    ElementoHtmlControl.SetProperty("innerHTML", "No impresa");
                //}
            }
            else
            {
                Thread.Sleep(demora);
                Dispatcher.BeginInvoke(delegate()
                {
                    listBox1.Items.Add("Error: No es posible comunicarse con la impresora.'");
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                });
            }
        }

        private void w_RunWorkerCompletedImprimirTarjeta(object sender, RunWorkerCompletedEventArgs e)
        {
            listBox1.Items.Add("Finalizó impresión de tarjeta...");
            listBox1.SelectedIndex = listBox1.Items.Count - 1;

            //ElementoHtmlControl.SetProperty("value", "Impresa");
            ElementoHtmlControl.SetProperty("innerHTML", "Impresa");            
        }
        #endregion
    }
}
