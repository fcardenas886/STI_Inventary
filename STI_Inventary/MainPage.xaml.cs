using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace STI_Inventary
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public Producto producto;
        public Inventario inv;
        public MainPage()
        {
            InitializeComponent();
            VersionTracking.Track();
            var currentVersion = VersionTracking.CurrentVersion;
            _lblversion.Text = "Version : " + currentVersion.ToString();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //BindingContext = this.producto;
        }

        private void Btn_buscar_Clicked(object sender, EventArgs e)
        {
            
            //DisplayAlert("Error", "buuscar", "OK");
            //if ((Ent_barra.Text==""))
          //  {
              //  DisplayAlert("Error", "if", "OK");
            // Scanner();
            // }
            // else
            //{
            if (String.IsNullOrEmpty( Ent_barra.Text))
            {
                //DisplayAlert("Error", "NULO", "OK");
                Scanner();
                
            }
            else {
               // DisplayAlert("Error", "NO NULO", "OK");
                producto = BaseDatos.ObtenerProd(Ent_barra.Text);
                Lbl_nombre.Text = producto.Nombre;
                Lbl_codigo.Text = producto.Id.ToString();

                inv = BaseDatos.ObtenerInvProd(producto.Id, Ent_pat.Text);
                Ent_actual.Text = inv.cantidad.ToString();
                Ent_stock.Focus();
            }

            //}

        }

        private void Btn_guardar_Clicked(object sender, EventArgs e)
        {

            if (String.IsNullOrEmpty(Ent_barra.Text) || (String.IsNullOrEmpty(Ent_pat.Text)) || Lbl_codigo.Text=="0" )
            {
                DisplayAlert("Error", "Verificar  " , "OK");
            }
            else
            {
                Inventario i = new Inventario
                {
                    id = int.Parse(Lbl_codigo.Text),
                    barra = Ent_barra.Text,
                    nombre = Lbl_nombre.Text,
                    ubicacion = Ent_pat.Text,
                    cantidad = (float.Parse(Ent_actual.Text) + float.Parse(Ent_stock.Text))
                };

                String x = BaseDatos.AgregarInventario(i);
                //Ent_ns.Text = x;
                if (x.Equals("ACT"))
                {
                    DisplayAlert("EXITO", "Actualizado a " + i.cantidad, "OK");
                    limpiar();
                }
                else if (x.Equals("INS"))
                {
                    DisplayAlert("EXITO", "Nuevo Agregado " + i.cantidad, "OK");
                    limpiar();
                }
                else
                {
                    DisplayAlert("ERROR", "ningun registro guardado", "volver");
                }

            }//else

        }

        private void limpiar()
        {
            Ent_barra.Text = "";
            Lbl_nombre.Text = "";
            Lbl_codigo.Text = "";
            Ent_actual.Text = "";
            Ent_stock.Text = "";



        }

        private async void Scanner()
        {
            //DisplayAlert("Error", "entro", "OK");
            try
            {
                ZXingScannerPage scannerPage = new ZXingScannerPage();
                scannerPage.Title = "Escanear Código de Barras";
                scannerPage.OnScanResult += (resultado) =>
                {
                    // Detener el escaneo
                    scannerPage.IsScanning = false;

                    // Acceder a la UI en el hilo principal
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        try
                        {
                            await Navigation.PopAsync();

                            string capturar = resultado.Text;
                            string capturarcod = capturar?.Trim();

                            if (!string.IsNullOrEmpty(capturarcod))
                            {
                                Ent_barra.Text = capturarcod;


                                // Descomenta y ajusta según sea necesario
                                //BindingContext = new vminventario();
                                //buscarfilter(capturarcod);
                                producto = BaseDatos.ObtenerProd(capturarcod);
                                Lbl_nombre.Text = producto.Nombre;
                                Lbl_codigo.Text = producto.Id.ToString();

                                inv = BaseDatos.ObtenerInvProd(producto.Id, Ent_pat.Text);
                                Ent_actual.Text = inv.cantidad.ToString();
                                Ent_stock.Focus();
                            }
                            else
                            {
                                await DisplayAlert("Error", "El código escaneado está vacío.", "OK");
                            }
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Error", $"Ocurrió un error al procesar el escaneo: {ex.Message}", "OK");
                        }
                    });
                };

                await Navigation.PushAsync(scannerPage);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error al iniciar el escaneo: {ex.Message}", "OK");
            }

        }
    }
}
