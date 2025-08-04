using MyNotes10.Models;
using MyNotes10.Services.DialogService;
using MyNotes10.Services.LoaderService;
using MyNotes10.Services.NotaService;
using MyNotes10.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MyNotes10.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {

        #region Properties

        private ILoaderService _loaderService;
        private IDialogService _dialogService;
        private INotaService _notaService;
        private Nota auxNota = new Nota();

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

        #endregion



        #region Collections

        private ObservableCollection<Nota> notaList;
        public ObservableCollection<Nota> NotaList
        {
            get { return notaList; }
            set
            {
                notaList = value;
                RaisePropertyChanged();
            }
        }

        #endregion




        #region Methods

        public MainPageViewModel(IDialogService dialogService, INotaService notaService, ILoaderService loaderService)
        {
            _dialogService = dialogService;
            _notaService = notaService;
            _loaderService = loaderService;
        }
        public override Task OnNavigatedFrom(NavigationEventArgs args)
        {
            return null;
        }
        public override Task OnNavigatedTo(NavigationEventArgs args)
        {
            if (args.Parameter != null)
            {
                if (args.Parameter.ToString() == "Backup")
                {
                    //_dialogService.ShowMessage("Coming soon.", "My Notes");
                    App.SelMenu = "Backup";
                }
            }

            RefreshListNota();
            return null;
        }

        private void RefreshListNota()
        {
            NotaList = new ObservableCollection<Nota>(_notaService.GetNotas());
            ClearNota();
        }
        private void ClearNota()
        {
            AuxNota = null;
            AuxNota = new Nota();
        }

        #endregion




        #region Commands

        private DelegateCommand insertOrUpdateNotaCommand;
        private DelegateCommand newNotaCommand;
        private DelegateCommand deleteNotaCommand;
        private DelegateCommand openPrivacyCommand;
        
        private DelegateCommand<string> _TextoABuscarCommand;
        //private DelegateCommand<string> _TraduceCadenaCommand;
        // private DelegateCommand<string> _CambiaIdiomaCommand;

        private DelegateCommand openStoreCommand;


        public ICommand InsertOrUpdateNotaCommand
        {
            get { return insertOrUpdateNotaCommand = insertOrUpdateNotaCommand ?? new DelegateCommand(InsertOrUpdateNotaCommandExecute); }
        }
        public ICommand NewNotaCommand
        {
            get { return newNotaCommand = newNotaCommand ?? new DelegateCommand(NewNotaCommandExecute); }
        }
        public ICommand DeleteNotaCommand
        {
            get { return deleteNotaCommand = deleteNotaCommand ?? new DelegateCommand(DeleteNotaCommandExecute); }
        }
        public ICommand OpenStoreCommand
        {
            get { return openStoreCommand = openStoreCommand ?? new DelegateCommand(OpenStoreCommandExecute); }
        }
        public ICommand OpenPrivacyCommand
        {
            get { return openPrivacyCommand = openPrivacyCommand ?? new DelegateCommand(OpenPrivacyCommandExecute); }
        }

        private void InsertOrUpdateNotaCommandExecute()
        {
            _notaService.InsertOrUpdateNota(auxNota);
            RefreshListNota();
        }
        private void NewNotaCommandExecute()
        {
            ClearNota();
        }
        private async void DeleteNotaCommandExecute()
        {
            //if (await _dialogService.ShowMessageYesNo("¿Está seguro de querer borrar la nota seleccionada?"))
            //{
            //    _notaService.DeleteNota(AuxNota);
            //    RefreshListNota();
            //}

            var messageDialog = new MessageDialog(Traduce("MsgAlertBorraNota"));
            messageDialog.Commands.Add(new Windows.UI.Popups.UICommand(Traduce("txtSi")) { Id = 0 });
            messageDialog.Commands.Add(new Windows.UI.Popups.UICommand(Traduce("txtNo")) { Id = 1 });

            messageDialog.DefaultCommandIndex = 0;
            messageDialog.CancelCommandIndex = 1;

            var result = await messageDialog.ShowAsync();

            if ((int)result.Id == 0)
            {
                _notaService.DeleteNota(AuxNota);
                RefreshListNota();
            }
        }

        public ICommand SortCommand
        {
            get { return new DelegateCommand<string>(SortCommandExecute); }
        }

        public ICommand TextoABuscarCommand
        {
            get { return _TextoABuscarCommand = _TextoABuscarCommand ?? new DelegateCommand<string>(TextoABuscarCommandExecute); }
        }

        private void SortCommandExecute(string SortOrder)
        {
            NotaList = new ObservableCollection<Nota>(_notaService.SortNotas(SortOrder));
            ClearNota();
        }
        private void TextoABuscarCommandExecute(string busca)
        {
            NotaList = new ObservableCollection<Nota>(_notaService.SearchNotas(busca));
            ClearNota();
        }


        //public ICommand TraduceCadenaCommand
        //{
        //    get { return _TraduceCadenaCommand = _TraduceCadenaCommand ?? new DelegateCommand<string>(TraduceCadenaCommandExecute); }
        //}
        //private void TraduceCadenaCommandExecute(string cadena)
        //{
        //    _dialogService.ShowMessage(_loaderService.getString(cadena),"Cap");
        //}


        //public ICommand CambiaIdioma
        //{
        //    get { return _CambiaIdiomaCommand = _CambiaIdiomaCommand ?? new DelegateCommand<string>(CambiaIdiomaCommandExecute); }
        //}
        //private async void CambiaIdiomaCommandExecute(string newLang)
        //{
        //    var resourceContext = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView();

        //    while (true)
        //    {
        //        if (resourceContext.Languages[0] == newLang)
        //        {
        //            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "en-US";
        //            Windows.UI.Xaml.Controls.Frame.Navigate(this.GetType());
        //            break;
        //        }
        //        await Task.Delay(100);
        //    }
        //}


        string Traduce(string cadena)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var str = loader.GetString(cadena);
            return str;
        }

        private async void OpenStoreCommandExecute()
        {
            await OpenStore();
        }

        public async Task OpenStore()
        {
            //const string productId = "9NBLGGH4RP1N";  //MyNotes10
            const string productId = "9NBLGGH08MCR"; //My Notes
            var uri = new Uri("ms-windows-store://review/?ProductId=" + productId);
            await Launcher.LaunchUriAsync(uri);
        }


        private async void OpenPrivacyCommandExecute()
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame?.Navigate(typeof(Views.PrivacyPage));

            //string @url;
            //if (Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("Idioma") == "es")
            //{
            //    @url = "https://www.darweb.es/PoliticaPrivacidadOneDrive.htm";
            //}
            //else
            //{
            //    @url = "https://www.darweb.es/PrivacyOneDrive.html";
            //}
            //var uri = new Uri(@url);
            //await Launcher.LaunchUriAsync(uri);
        }


        #endregion




    }
}
