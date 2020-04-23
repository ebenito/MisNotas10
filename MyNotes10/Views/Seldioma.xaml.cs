using MyNotes10.Views.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
//using Microsoft.Toolkit.Uwp.UI.Controls;

// La plantilla de elemento Página en blanco está documentada en http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyNotes10.Views
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class Seldioma : PageBase
    {
        public Seldioma()
        {
            this.InitializeComponent();
        }

        private void B_Ingles_Click(object sender, RoutedEventArgs e)
        {
            EstableceIdioma("en");
        }
        
        private void B_Español_Click(object sender, RoutedEventArgs e)
        {
            EstableceIdioma("es");
        }

        private void B_Frances_Click(object sender, RoutedEventArgs e)
        {
            EstableceIdioma("fr");
        }

        private void B_Italiano_Click(object sender, RoutedEventArgs e)
        {
            EstableceIdioma("it");
        }

        private void B_Aleman_Click(object sender, RoutedEventArgs e)
        {
            EstableceIdioma("de");
        }
        
        private void B_Portugues_Click(object sender, RoutedEventArgs e)
        {
            EstableceIdioma("pt");
        }

        private void B_Arabe_Click(object sender, RoutedEventArgs e)
        {
            EstableceIdioma("ar");
        }

        private void B_Catalan_Click(object sender, RoutedEventArgs e)
        {
            EstableceIdioma("ca");
        }

        private void B_Fines_Click(object sender, RoutedEventArgs e)
        {
            EstableceIdioma("fi-FI");
        }

        private void B_Ucraniano_Click(object sender, RoutedEventArgs e)
        {
            EstableceIdioma("uk");
        }


        private void B_Chino_Click(object sender, RoutedEventArgs e)
        {
            EstableceIdioma("zh-Hans");
        }


        private void B_Hindi_Click(object sender, RoutedEventArgs e)
        {
            EstableceIdioma("hi");
        }

        private void B_Japones_Click(object sender, RoutedEventArgs e)
        {
            EstableceIdioma("ja");
        }

        private void B_Ruso_Click(object sender, RoutedEventArgs e)
        {
            EstableceIdioma("ru");
        }



        private async void EstableceIdioma(string languageCode)
        {
            //http://stackoverflow.com/questions/32715690/c-sharp-change-app-language-programmatically-uwp-realtime

            ApplicationLanguages.PrimaryLanguageOverride = languageCode;
            await Task.Delay(100);

            InitializeLanguage();

            await Task.Delay(100);
            Frame.Navigate(typeof(MainPage));
        }

        private void InitializeLanguage()
        {
            try
            {
                // Set the font to match the display language defined by the
                // "Idioma" resource string for each supported language.

                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Language = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("Idioma");
                System.Diagnostics.Debug.WriteLine("IDIOMA: " + rootFrame.Language);

                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each
                // supported language.
                //
                if (Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ResourceFlowDirection") == "LeftToRight")
                {
                    rootFrame.FlowDirection = FlowDirection.LeftToRight;
                    System.Diagnostics.Debug.WriteLine("Sentido texto: LeftToRight");
                }
                else
                {
                    rootFrame.FlowDirection = FlowDirection.RightToLeft;
                    System.Diagnostics.Debug.WriteLine("Sentido texto: RightToLeft");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al establecer el idioma: " + ex.Message);
            }

        }
    }
}
