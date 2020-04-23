using MyNotes10.Models;
using MyNotes10.Services.DialogService;
using MyNotes10.Views;
using OneDriveSimple;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.IO;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Store;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MyNotes10
{
    /// <summary>
    /// Proporciona un comportamiento específico de la aplicación para complementar la clase Application predeterminada.
    /// </summary>
    sealed partial class App : Application
    {
        public bool IsTrial { get; set; }
        public static int SelNota { get; set; }
        public static string SelMenu { get; set; }
        public static bool SecTile { get; set; }
        public static bool IsMenuVisible { get; set; }
        public static bool HasError { get; set; }

        LicenseInformation licenseInformation;
        //private IDialogService _dialogService;


        public OneDriveService ServiceInstance
        {
            get;
            private set;
        }

        /// <summary>
        /// Inicializa el objeto de aplicación Singleton. Esta es la primera línea de código creado
        /// ejecutado y, como tal, es el equivalente lógico de main() o WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);

            this.InitializeComponent();
            this.Suspending += OnSuspending;
            
            // OneDrive Client ID
            ServiceInstance = new OneDriveService("0000000040110993"); //Mis Notas
        }

        /// <summary>
        /// Se invoca cuando el usuario final inicia la aplicación normalmente. Se usarán otros puntos
        /// de entrada cuando la aplicación se inicie para abrir un archivo específico, por ejemplo.
        /// </summary>
        /// <param name="e">Información detallada acerca de la solicitud y el proceso de inicio.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = false;
            }
#endif
            try
            {
                // Para TEST: C:\Users\ebenito\AppData\Local\Packages\64068EstebanBenito.MyNotesPro_af28qns5pen9p\LocalState\Microsoft\Windows Store\ApiData\WindowsStoreProxy.xml
                //licenseInformation = CurrentAppSimulator.LicenseInformation;
                licenseInformation = Windows.ApplicationModel.Store.CurrentApp.LicenseInformation;
                //licenseInformation.LicenseChanged += LicenseInformation_LicenseChanged;

                IsTrial = licenseInformation.IsTrial;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al obtener el tipo de licencia: " + ex.Message);
            }

            //Se creará la tabla Notas, si no existe.
            using (var conn = new SQLiteConnection(new SQLitePlatformWinRT(), DbConnectionString))
            {
                conn.CreateTable<Nota>();

#if DEBUG
                Nota nota = new Nota
                {
                    Id = 1,
                    Asunto = "Welcome to MyNotes",
                    Fecha = DateTime.Now,
                    Detalle = "Welcome to MyNotes for Windows 10. Now MyNotes it's an UWP App, that's mean that you can run it in your Phone, Tablet and PC, under Windows 10. Enjoy it!",
                    FSize = 22,
                    CFondo = "#ED341D"
                };

                var i = conn.InsertOrReplace(nota);
                
                nota.Id = 2;
                nota.Asunto = "Receipt: Spanish omelete";
                nota.Fecha = DateTime.Now;
                nota.Detalle = "Ingredients: \n\n 500g new potato \n 1 onion, preferably white \n 150ml extra-virgin olive oil \n\n 3 tbsp chopped flatleaf parsley \n 6 eggs\n\n Method \n Scrape the potatoes or leave the skins on, if you prefer. Cut them into thick slices. Chop the onion. \n Heat the oil in a large frying pan, add the potatoes and onion and stew gently, partially covered, for 30 minutes, stirring occasionally until the potatoes are softened. Strain the potatoes and onions through a colander into a large bowl (set the strained oil aside). \n Beat the eggs separately, then stir into the potatoes with the parsley and plenty of salt and pepper. Heat a little of the strained oil in a smaller pan. Tip everything into the pan and cook on a moderate heat, using a spatula to shape the omelette into a cushion.  \n When almost set, invert on a plate and slide back into the pan and cook a few more minutes. Invert twice more, cooking the omelette briefly each time and pressing the edges to keep the cushion shape. Slide on to a plate and cool for 10 minutes before serving.";
                nota.FSize = 15;
                nota.CFondo = "#ED341D";

                i = conn.InsertOrReplace(nota);

                nota.Id = 3;
                nota.Asunto = "Lorem ipsum";
                nota.Fecha = DateTime.Now;
                nota.Detalle = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam quis lectus nisl. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Duis suscipit lobortis magna, nec varius turpis aliquet sit amet. Donec nunc nisl, faucibus nec tellus a, dignissim porttitor odio. Duis quis rhoncus orci. Morbi a suscipit nulla. Fusce tristique rutrum maximus. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus eu turpis finibus, semper elit quis, rutrum erat. Curabitur ornare volutpat sapien eget mattis. Fusce bibendum ligula enim, sed facilisis nisl bibendum id. ";
                nota.FSize = 22;
                nota.CFondo = "#ED341D";

                i = conn.InsertOrReplace(nota);
#endif
            }

            App.HasError = false;
            Frame rootFrame = Window.Current.Content as Frame;

            // No repetir la inicialización de la aplicación si la ventana tiene contenido todavía,
            // solo asegurarse de que la ventana está activa.
            if (rootFrame == null)
            {
                // Crear un marco para que actúe como contexto de navegación y navegar a la primera página.
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Cargar el estado de la aplicación suspendida previamente
                }

                // Poner el marco en la ventana actual.
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null || e.TileId != "App")
                {
                    // Cuando no se restaura la pila de navegación, navegar a la primera página,
                    // configurando la nueva página pasándole la información requerida como
                    // parámetro de navegación
                    if (e.TileId != "App")
                    {
                        App.SelNota = Int32.Parse(e.TileId);
                        App.SecTile = true;
                        rootFrame.Content = null;
                        rootFrame.Navigate(typeof(Shell), e.TileId);
                    }
                    else
                    {
                        App.SecTile = false;
                        rootFrame.Navigate(typeof(Shell), e.Arguments);
                    }
                }
                
                // Asegurarse de que la ventana actual está activa.
                Window.Current.Activate();
            }


           InitializeLanguage();
        }

        // https://docs.microsoft.com/es-es/windows/uwp/monetize/exclude-or-limit-features-in-a-trial-version-of-your-app
        //private void LicenseInformation_LicenseChanged()
        //{
        //    ReloadLicense();
        //}

        //void ReloadLicense()
        //{
        //    if (licenseInformation.IsActive)
        //    {
        //        if (licenseInformation.IsTrial)
        //        {
        //            // Show the features that are available during trial only.
        //            _dialogService.ShowMessage("comprelo", traduce("NameApp"));
        //        }
        //        else
        //        {
        //            // Show the features that are available only with a full license.
        //            _dialogService.ShowMessage("Gracias por comprar", traduce("NameApp"));
        //        }
        //    }
        //    else
        //    {
        //        // A license is inactive only when there' s an error.
        //        _dialogService.ShowMessage("Error", traduce("NameApp"));
        //    }
        //}

        //string traduce(string cadena)
        //{
        //    var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
        //    var str = loader.GetString(cadena);
        //    return str;
        //}

        /// <summary>
        /// Se invoca cuando la aplicación la inicia normalmente el usuario final. Se usarán otros puntos
        /// </summary>
        /// <param name="sender">Marco que produjo el error de navegación</param>
        /// <param name="e">Detalles sobre el error de navegación</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Se invoca al suspender la ejecución de la aplicación. El estado de la aplicación se guarda
        /// sin saber si la aplicación se terminará o se reanudará con el contenido
        /// de la memoria aún intacto.
        /// </summary>
        /// <param name="sender">Origen de la solicitud de suspensión.</param>
        /// <param name="e">Detalles sobre la solicitud de suspensión.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Guardar el estado de la aplicación y detener toda actividad en segundo plano
            deferral.Complete();
        }

        public static string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            // return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }

        public static string DbConnectionString
        {
            get
            {
                return Path.Combine(ApplicationData.Current.LocalFolder.Path, "Storage.sqlite");
            }
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
