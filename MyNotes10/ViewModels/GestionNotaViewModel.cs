using MyNotes10.Models;
using MyNotes10.Services.DialogService;
using MyNotes10.Services.NavigationService;
using MyNotes10.Services.NotaService;
using MyNotes10.ViewModels.Base;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml.Navigation;

namespace MyNotes10.ViewModels
{
    public class GestionNotaViewModel : ViewModelBase
    {
        #region Properties

        private IDialogService _dialogService;
        private INotaService _notaService;
        private INavigationService _navigationService;
        private Nota auxNota = new Nota();

        //private int id;
        //private string asunto;
        //private string detalle;
        //private DateTime fecha;
        //private int fsize;
        //private string cfondo;

        public Nota AuxNota
        {
            get { return auxNota; }
            set
            {
                auxNota = value;
                App.SelNota = (auxNota != null) ? auxNota.Id : 0;
                RaisePropertyChanged();
            }
        }

        //public int ID
        //{
        //    get { return auxNota.Id; }
        //    set
        //    {
        //        id = value;
        //        RaisePropertyChanged("ID");
        //    }
        //}

        //public string Asunto
        //{
        //    get { return auxNota.Asunto; }
        //    set
        //    {
        //        asunto = value;
        //        RaisePropertyChanged("Asunto");
        //    }
        //}

        //public string Detalle
        //{
        //    get { return auxNota.Detalle; }
        //    set
        //    {
        //        detalle = value;
        //        RaisePropertyChanged("Detalle");
        //    }
        //}

        //public DateTime Fecha
        //{
        //    get { return auxNota.Fecha; }
        //    set
        //    {
        //        fecha = value;
        //        RaisePropertyChanged("Fecha");
        //    }
        //}
        //public int FSize
        //{
        //    get { return auxNota.FSize; }
        //    set
        //    {
        //        fsize = value;
        //        RaisePropertyChanged("FSize");
        //    }
        //}

        //public string CFondo
        //{
        //    get { return auxNota.CFondo; }
        //    set
        //    {
        //        cfondo = value;
        //        RaisePropertyChanged("CFondo");
        //    }
        //}

        private string _msgAccionActual;
        public string MsgAccionActual
        {
            get { return _msgAccionActual; }
            set
            {
                _msgAccionActual = value;
                RaisePropertyChanged();
            }
        }

        #endregion




        #region Methods

        public GestionNotaViewModel(IDialogService dialogService, INotaService notaService, INavigationService navigationService)
        {
            _notaService = notaService;
            _dialogService = dialogService;
            _navigationService = navigationService;
        }

        private void NuevaNota()
        {            
            AuxNota = null;
            AuxNota = new Nota();

            AuxNota.Fecha = DateTime.Now;
            AuxNota.FSize = 22;
            AuxNota.CFondo = "#ED341D";
        }


        public override Task OnNavigatedFrom(NavigationEventArgs args)
        {
            return null;
        }
        
        public async override Task OnNavigatedTo(NavigationEventArgs args)
        {
            if (args.Parameter.ToString() == "Edit" && App.SelNota == 0)
            {
                _dialogService.ShowMessage(traduce("MsgNoNotaEditar"), traduce("NameApp"));
                if (((Windows.UI.Xaml.Controls.Frame)Windows.UI.Xaml.Window.Current.Content).CanGoBack)
                {
                    // _navigationService.Navigate(typeof(MyNotes10.Views.MainPage));
                    AppFrame.Navigate(typeof(MyNotes10.Views.MainPage));
                }
                else
                {
                    App.HasError = true;
                }
            }

            if ((string)args.Parameter == "New")
            {
                MsgAccionActual = traduce("LabelAccionNueva");
                if (await CheckIfIsCanAddNewNoteAsync())
                {
                    App.SelNota = 0;
                    NuevaNota();
                }
                else
                {
                    MsgAccionActual = traduce("TituloTrial");
                }
            }
            else
            {
                MsgAccionActual = traduce("LabelAccionEditar");
                if (App.SelNota > 0)
                {
                    AuxNota = _notaService.GetNota(App.SelNota);
                }

            }

            // return null;
        }

        async Task<bool> CheckIfIsCanAddNewNoteAsync()
        {
            if ((App.Current as App).IsTrial)
            {
                var messageDialog = new MessageDialog(traduce("MsgTrial"), traduce("TituloTrial"));
                messageDialog.Commands.Add(new Windows.UI.Popups.UICommand(traduce("txtSi")) { Id = 0 });
                messageDialog.Commands.Add(new Windows.UI.Popups.UICommand(traduce("txtNo")) { Id = 1 });

                messageDialog.DefaultCommandIndex = 0;
                messageDialog.CancelCommandIndex = 1;

                var result = await messageDialog.ShowAsync();

                if ((int)result.Id == 0)
                {
                    const string productId = "9NBLGGH08MCR"; //My Notes
                    var uri = new Uri("ms-windows-store://pdp/?ProductId=" + productId);
                    await Windows.System.Launcher.LaunchUriAsync(uri);

                    return false;
                }
                else
                {
                    if (_notaService.CountNotas() < 3) //Si es Trial solo dejo crear 3 notas
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return true;
            }
        }



        string traduce(string cadena)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var str = loader.GetString(cadena);
            return str;
        }

        #endregion




        #region Commands

        private DelegateCommand insertOrUpdateNotaCommand;
        private DelegateCommand deleteNotaCommand;
        private DelegateCommand sendByEmailCommand;
        private DelegateCommand pinToStartCommand;

        public ICommand InsertOrUpdateNotaCommand
        {
            get { return insertOrUpdateNotaCommand = insertOrUpdateNotaCommand ?? new DelegateCommand(InsertOrUpdateNotaCommandExecute); }
        }
        public ICommand DeleteNotaCommand
        {
            get { return deleteNotaCommand = deleteNotaCommand ?? new DelegateCommand(deleteNotaCommandExecute); }
        }
        public ICommand SendByEmailCommand
        {
            get { return sendByEmailCommand = sendByEmailCommand ?? new DelegateCommand(sendByEmailCommandExecute); }
        }
        public ICommand PinToStartCommand
        {
            get { return pinToStartCommand = pinToStartCommand ?? new DelegateCommand(pinToStartCommandExecute); }
        }

        private void InsertOrUpdateNotaCommandExecute()
        {
            //Nota editedNota = new Nota();
            //editedNota.Id = this.ID;
            //editedNota.Asunto = this.Asunto;
            //editedNota.Detalle = this.Detalle;
            //editedNota.Fecha = this.Fecha;
            //editedNota.FSize = this.FSize;
            //editedNota.CFondo = this.CFondo;

            try
            {
                _notaService.InsertOrUpdateNota(AuxNota);
                
                if (App.SelNota > 0)
                {
                    _dialogService.ShowMessage(traduce("MsgNotaActualizada"), traduce("NameApp"));
                }
                else
                {
                    _dialogService.ShowMessage("Note created successfully.", traduce("NameApp"));
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage(traduce("MsgNotaCreadaError") + " " + ex.Message, traduce("NameApp"));
            }
        }

        private void deleteNotaCommandExecute()
        {
            try
            {
                _notaService.DeleteNota(AuxNota);

                _dialogService.ShowMessage(traduce("MsgNotaBorrada"), traduce("NameApp"));
                //AppFrame.Navigate(typeof(MyNotes10.Views.MainPage));
                if (AppFrame.CanGoBack)
                    AppFrame.GoBack();
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage(traduce("MsgNotaBorradaError") + " " + ex.Message, traduce("NameApp"));
            }
        }


        private async void sendByEmailCommandExecute()
        {
            if (AuxNota.Id == 0)
            {
                _dialogService.ShowMessage("Before send the note you must save it.", traduce("NameApp"));
            }
            else
            {
                EmailMessage emailMessage = new EmailMessage();
                string asunto = AuxNota.Asunto;
                string cuerpo = AuxNota.Detalle;

                emailMessage.Subject = asunto;
                emailMessage.Body = cuerpo;

                //StorageFolder MyFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                //StorageFile attachmentFile = await MyFolder.GetFileAsync("MyTestFile.txt");
                //if (attachmentFile != null)
                //{
                //    var stream = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(attachmentFile);
                //    var attachment = new Windows.ApplicationModel.Email.EmailAttachment(
                //             attachmentFile.Name,
                //             stream);
                //    emailMessage.Attachments.Add(attachment);
                //}

                await EmailManager.ShowComposeNewEmailAsync(emailMessage);
            }
           
        }

        private async void pinToStartCommandExecute()
        {
            try
            {
                // Prepare the images for our tile to be pinned.
                string tileID = AuxNota.Id.ToString();
                // Uri square150x150Logo = new Uri("ms-appx:///Assets/Imagenes/Square150x150Tile.png"); 
                Uri square150x150Logo = new Uri("ms-appx:///Assets/Imagenes/" + traduce("ImgTileMediano"));
                Uri wide310x150Logo = new Uri("ms-appx:///Assets/Imagenes/Wide310x150Tile.png");

                // During creation of the secondary tile, an application may set additional arguments on the tile that will be passed in during activation.
                // These arguments should be meaningful to the application. In this sample, we'll pass in the date and time the secondary tile was pinned.
                string tileActivationArguments = tileID; // + " WasPinnedAt=" + DateTime.Now.ToLocalTime().ToString();

                // Create a Secondary tile with all the required properties and sets perfered size to Wide310x150.
                SecondaryTile secondaryTile = new SecondaryTile(tileID,
                                                                traduce("NameApp"),
                                                                tileActivationArguments,
                                                                square150x150Logo,
                                                                TileSize.Square150x150);

                // Adding the wide tile logo.
                secondaryTile.VisualElements.Wide310x150Logo = wide310x150Logo;

                // The display of the app name can be controlled for each tile size.
                secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
                secondaryTile.VisualElements.ShowNameOnWide310x150Logo = true;

                // Specify a foreground text value.
                // The tile background color is inherited from the parent unless a separate value is specified.
                secondaryTile.VisualElements.ForegroundText = ForegroundText.Dark;
                secondaryTile.VisualElements.BackgroundColor = GetRandomColor();

                await secondaryTile.RequestCreateAsync();


                //AÑADO DETALLES:
                if (AuxNota.Id == 0)
                {
                    _dialogService.ShowMessage("Before anchor the note you must save it.", traduce("NameApp"));
                }
                else
                { 
                    string tileXmlString = "<tile>"
                                        + "<visual version='2'>"
                                        + "<binding template='TileWide' branding='nameAndLogo'>"
                                        + "<text hint-style='subtitle' hint-align='center'>" + AuxNota.Asunto + "</text>"
                                        + "<text hint-style='captionSubtle' hint-wrap='true'>" + AuxNota.Detalle.Replace("\r\n", " - ") + "</text>"
                                        + "</binding>"
                                        + "<binding template='TileSquare150x150Text04' fallback='TileSquareText04' branding='none'>"
                                        + "<image placement='peek' src='Assets/Imagenes/" + traduce("ImgTileMediano") + "' />"
                                        + "<text id='1'>" + AuxNota.Asunto + ": " + AuxNota.Detalle + "</text>"
                                        + "</binding>"
                                        + "</visual>"
                                        + "</tile>";

                    Windows.Data.Xml.Dom.XmlDocument tileDOM = new Windows.Data.Xml.Dom.XmlDocument();
                    tileDOM.LoadXml(tileXmlString);
                    TileNotification tile = new TileNotification(tileDOM);

                    // Send the notification to the secondary tile by creating a secondary tile updater
                    TileUpdateManager.CreateTileUpdaterForSecondaryTile(tileID).Update(tile);

                    string _deviceFamily;

                    var qualifiers = ResourceContext.GetForCurrentView().QualifierValues;
                    _deviceFamily = qualifiers.First(q => q.Key.Equals("DeviceFamily")).Value;

                    if (_deviceFamily.Equals("Mobile"))
                    {
                        _dialogService.ShowMessage("Se ha anclado correctamente la nota al menú de inicio", traduce("NameApp"));
                    }      
                }                          
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage(traduce("MsgErrorGeneral") + " " + ex.Message, traduce("NameApp"));
            }
        }


        private Color GetRandomColor()
        {
            //Para crear un color aleatorio
            Random random = new Random();
            Color cfondo = new Color();

            cfondo.A = 255;
            cfondo.R = (byte)random.Next(80, 150); //Evito fondos muy claros
            cfondo.G = (byte)random.Next(80, 150);
            cfondo.B = (byte)random.Next(80, 150);

            return cfondo;
        }

        #endregion



    }
}
