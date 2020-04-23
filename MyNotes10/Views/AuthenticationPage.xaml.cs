using OneDriveSimple;
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
    public sealed partial class AuthenticationPage
    {
        private readonly OneDriveService _service;

        public AuthenticationPage()
        {
            InitializeComponent();

             _service = ((App)Application.Current).ServiceInstance;

            Loaded += (s, e) =>
            {
                var uri = _service.GetStartUri();
                Web.Navigate(uri);
            };

            Web.NavigationCompleted += (s, e) =>
            {
                if (_service.CheckRedirectUrl(e.Uri.AbsoluteUri))
                {
                    _service.ContinueGetTokens(e.Uri);
                }
            };

            Web.NavigationFailed += (s, e) =>
            {
                _service.ContinueGetTokens(null);
            };
        }
    }
}