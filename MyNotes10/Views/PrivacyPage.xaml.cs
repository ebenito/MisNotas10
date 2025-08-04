using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace MyNotes10.Views
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class PrivacyPage : Page
    {
        public PrivacyPage()
        {
            this.InitializeComponent();

            string idioma = ResourceLoader.GetForCurrentView().GetString("Idioma");
            string archivo = idioma == "es" ? "privacy_es.html" : "privacy_en.html";
            string ruta = $"ms-appx-web:///Assets/Privacy/{archivo}";

            webView.Navigate(new Uri(ruta));
        }
    }
}
