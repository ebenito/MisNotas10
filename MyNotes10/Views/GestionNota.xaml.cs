using MyNotes10.Services.DialogService;
using MyNotes10.ViewModels;
using MyNotes10.Views.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyNotes10.Views
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class GestionNota : PageBase
    {
        public GestionNota()
        {
            this.InitializeComponent();
            this.Loaded += GestionNota_Loaded;
            //if (App.SelNota == 0)
            //{
            //    T_Accion.Text = "Nuevo";
            //}
            //else
            //{
            //    T_Accion.Text = "Edita nota ID " + App.SelNota;
            //}
        }

        private void GestionNota_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.HasError)
            {
                App.HasError = false; //Reseteo la variable
                this.Frame.Navigate(typeof(MainPage));
            }
        }

        private void PageBase_Loading(FrameworkElement sender, object args)
        {
            //http://www.iteramos.com/pregunta/18658/mvvm-locura-comandos
            //GestionNotaViewModel vm = this.DataContext as GestionNotaViewModel;
            //if (vm == null) return; //Check if conversion succeeded

            //if (App.SelNota == 0)
            //{
            //    vm.MsgAccionActual = "Nuevo";
            //}
            //else
            //{
            //    vm.MsgAccionActual = "Edita nota ID " + App.SelNota;
            //}            
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void pageBase_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                T_Alto.Text = pageBase.ActualHeight.ToString();
                System.Diagnostics.Debug.WriteLine($"Alto: {pageBase.ActualHeight.ToString()}");
                System.Diagnostics.Debug.WriteLine($"Ancho: {pageBase.ActualWidth.ToString()}");

                if (pageBase.ActualWidth < 400)
                {
                    cmdBtnBack.Visibility = Visibility.Collapsed;
                }
                else
                {
                    cmdBtnBack.Visibility = Visibility.Visible;
                }
            }
            catch
            {
                T_Alto.Text = "400";
            }
        }

        //protected override void OnNavigatedFrom(NavigationEventArgs e)
        //{
        //    base.OnNavigatedFrom(e);
        //}

        private void AppBarButton_GotFocus(object sender, RoutedEventArgs e)
        {
            T_Fecha.SelectAll();
        }

        private void AppBarButton_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            T_Fecha.SelectAll();
        }

        

        private void AppBarButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            //29/05/2017: Por algun motivo, en la version de PC no se actualiza el ultimo campo modificado a menos que se produzca una perdida del foco
            T_Asunto.Focus(FocusState.Programmatic);
            T_Detalle.Focus(FocusState.Programmatic);
        }

        private void T_Accion_LayoutUpdated(object sender, object e)
        {
            if (T_Accion.Text == traduce("TituloTrial"))
            {
                cmdBtnSave.IsEnabled = false;
            }
            else
            {
                cmdBtnSave.IsEnabled = true;
            }
        }


        string traduce(string cadena)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var str = loader.GetString(cadena);
            return str;
        }
    }
}
