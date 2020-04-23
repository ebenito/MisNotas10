using MyNotes10.Models;
using MyNotes10.Services.NotaService;
using OneDriveSimple;
using OneDriveSimple.Response;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
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
    public sealed partial class BackupOneDrive : Page
    {
        private static string _folderPath = "MyNotes";
        private static int _numNotes = 0;
        private static int _nNote = 0;
        private readonly OneDriveService _service = ((App)Application.Current).ServiceInstance;
        private  string _savedId;
        private string displayName = "ehhh!";

        private INotaService _notaService;

        public BackupOneDrive()
        {
            this.InitializeComponent();            

            var navManager = SystemNavigationManager.GetForCurrentView();
            navManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            //GetUserName();

            //T_Info.Text = "You must log in with your Microsoft Account.";
        }

        public BackupOneDrive(INotaService notaService)
        {
            _notaService = notaService;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (_service.IsAuthenticated)
            {
                T_Info.Text = displayName + traduce("InfoAutenticadoUsuario");
                B_SingInOnedrive.Content = traduce("SalirOneDrive");
                B_Backup.IsEnabled = true;
                B_Restore.IsEnabled = true;
            }
            else
            {
                B_Backup.IsEnabled = false;
                B_Restore.IsEnabled = false;
            }
        }

        private async void B_SingInOnedrive_Click(object sender, RoutedEventArgs e)
        {
            ShowBusy(true);

            if (B_SingInOnedrive.Content.ToString() == traduce("EntrarOnedrive"))
            {
                if (!_service.CheckAuthenticate(
                    () =>
                    {
                        //var dialog = new MessageDialog("You are authenticated!", "Success!");
                        //await dialog.ShowAsync();
                        T_Info.Text = traduce("InfoAutenticado"); ;
                        ShowBusy(false);

                        if (Frame.SourcePageType.Name == "AuthenticationPage")
                            Frame.GoBack();
                    },
                    async () =>
                    {
                        T_Info.Text = traduce("ErrorAutenticacion");
                        var dialog = new MessageDialog(traduce("ProblemaAutenticacion"), traduce("NameApp"));
                        await dialog.ShowAsync();
                        ShowBusy(false);
                        Frame.GoBack();
                    }))
                {
                    Frame.Navigate(typeof(AuthenticationPage));
                }
            }
            else
            {
                //Log Out
                Exception error = null;
                ShowBusy(true);

                try
                {
                    await _service.Logout();
                }
                catch (Exception ex)
                {
                    error = ex;
                }

                if (error != null)
                {
                    var dialog = new MessageDialog(traduce("MsgErrorGeneral") + " " + error.Message, traduce("NameApp"));
                    await dialog.ShowAsync();
                    ShowBusy(false);
                    return;
                }


                T_Info.Text = traduce("MsgSalidaOneDrive");
                //var successDialog = new MessageDialog("You are now logged off", "Success");
                //await successDialog.ShowAsync();
                B_SingInOnedrive.Content = traduce("EntrarOnedrive");
                ShowBusy(false);
            }
        }

        async void GetUserName()
        {
            //http://stackoverflow.com/questions/33394019/get-username-in-a-windows-10-c-sharp-uwp-universal-windows-app
            //Requiere la capacidad: Información de cuenta de usuario 
            IReadOnlyList<User> users = await User.FindAllAsync();

            var current = users.Where(p => p.AuthenticationStatus == UserAuthenticationStatus.LocallyAuthenticated &&
                                p.Type == UserType.LocalUser).FirstOrDefault();

            // user may have username
            var data = await current.GetPropertyAsync(KnownUserProperties.AccountName);
            displayName = (string)data;

            //or may be authinticated using hotmail 
            if (String.IsNullOrEmpty(displayName))
            {

                string a = (string)await current.GetPropertyAsync(KnownUserProperties.FirstName);
                string b = (string)await current.GetPropertyAsync(KnownUserProperties.LastName);

                var usuarios = await User.FindAllAsync(UserType.LocalUser);
                var nombre = await users.FirstOrDefault().GetPropertyAsync(KnownUserProperties.AccountName);

                displayName = string.Format("{0} {1}", a, b);
            }

            if (displayName.Trim() == "")
                displayName = "Hey";

            if (_service.IsAuthenticated)
            {
                T_Info.Text = displayName + traduce("InfoAutenticadoUsuario");
            }
            else
            {
                T_Info.Text = displayName + traduce("InfoEntrarCuentaMS");
            }
        }


        private async void B_Restore_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new Windows.UI.Popups.MessageDialog(traduce("MsgSelectOrigenRestauracion"), traduce("MsgSelectOrigenRestauracionCap"));

                dialog.Commands.Add(new Windows.UI.Popups.UICommand("App Windows Phone") { Id = 0 });
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("App Windows 10") { Id = 1 });                

                if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
                {
                    // Adding a 3rd command will crash the app when running on Mobile !!!
                    dialog.Commands.Add(new Windows.UI.Popups.UICommand(traduce("btnCancelar")) { Id = 2 });
                }

                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 1;

                var result = await dialog.ShowAsync();

                System.Diagnostics.Debug.WriteLine($"Result: {result.Label} ({result.Id})");

                if ((int)result.Id == 0)
                {
                    BrowseSubfolder(_folderPath);
                }
                else if ((int)result.Id == 2)
                {
                    //Nothing, exit
                }
                else
                {
                    RestauraDB();
                }
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(traduce("MsgErrorGeneral") + " " + ex.Message, traduce("NameApp"));
                await dialog.ShowAsync();
            }
        }

        private async void BrowseSubfolder(string FolderPathText)
        {
            if (string.IsNullOrEmpty(FolderPathText))
            {
                var dialog = new MessageDialog("Please enter a path to a folder, for example Apps/My Notes", "Error!");
                await dialog.ShowAsync();
                return;
            }

            Exception error = null;
            ItemInfoResponse subfolder = null;

            ShowBusy(true);

            try
            {
                subfolder = await _service.GetItem(FolderPathText);
            }
            catch (Exception ex)
            {
                error = ex;
            }

            if (error != null)
            {
                var dialog = new MessageDialog(traduce("MsgErrorGeneral") + " " + error.Message, traduce("NameApp"));
                await dialog.ShowAsync();
                ShowBusy(false);
                return;
            }

            if (subfolder == null)
            {
                var dialog = new MessageDialog(traduce("MsgNoEncontrado") + " " + FolderPathText);
                await dialog.ShowAsync();
            }
            else
            {
                var children = await _service.PopulateChildren(subfolder);
                //DisplayHelper.ShowContent(
                //    "SHOW SUBFOLDER CONTENT ------------------------",
                //    subfolder,
                //    children,
                //    async message =>
                //    {
                //        var dialog = new MessageDialog(message);
                //        await dialog.ShowAsync();
                //    });

                _numNotes = children.Count;
                T_Info.Text = _numNotes + " " + traduce("MsgCantNotasEncontradas");

                foreach (var child in children)
                {
                    _nNote += 1;
                    DownloadFile("MyNotes/" + child.Name);
                }

                //ShowBusy(false);
            }
        }

        private async void DownloadFile(string DownloadFilePathText)
        {
            if (string.IsNullOrEmpty(DownloadFilePathText))
            {
                var dialog = new MessageDialog("Please enter a path to an existing file, for example MyNotes/Note1.txt", "Error!");
                await dialog.ShowAsync();
                return;
            }

            Exception error = null;
            ItemInfoResponse foundFile = null;
            Stream contentStream = null;

            ShowBusy(true);

            try
            {
                //T_ProgRestaura.Text = $"Downloading {_nNote} notes, please wait ...";
                T_ProgRestaura.Text = traduce("MsgDescargando") + " " + _nNote + " " + traduce("MsgDescargando2Parte");
               
                foundFile = await _service.GetItem(DownloadFilePathText);

                if (foundFile == null)
                {
                    var dialog = new MessageDialog(traduce("MsgNoEncontrado") + " " + DownloadFilePathText);
                    await dialog.ShowAsync();
                    ShowBusy(false);
                    return;
                }

                // Get the file's content
                contentStream = await _service.RefreshAndDownloadContent(foundFile, false);

                if (contentStream == null)
                {
                    var dialog = new MessageDialog(traduce("MsgContenidoNoEncontrado") + DownloadFilePathText);
                    await dialog.ShowAsync();
                    ShowBusy(false);
                    return;
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            if (error != null)
            {
                var dialog = new MessageDialog(traduce("MsgErrorGeneral") + " " + error.Message, traduce("NameApp"));
                await dialog.ShowAsync();
                ShowBusy(false);
                return;
            }

            // Save the retrieved stream to the local drive            
            IsolatedStorageFile isoFile;
            isoFile = IsolatedStorageFile.GetUserStoreForApplication();

            if (!isoFile.DirectoryExists("MyNotes"))
            {
                isoFile.CreateDirectory("MyNotes");
            }

            // Open or create a writable file.
            using (IsolatedStorageFileStream isoStream =
                new IsolatedStorageFileStream(Path.Combine("MyNotes", foundFile.Name),
                FileMode.OpenOrCreate,
                FileAccess.Write,
                isoFile))
            {
                using (var writer = new BinaryWriter(isoStream))
                {
                    contentStream.Position = 0;

                    using (var reader = new BinaryReader(contentStream))
                    {
                        byte[] bytes;

                        do
                        {
                            bytes = reader.ReadBytes(1024);
                            writer.Write(bytes);
                        }
                        while (bytes.Length == 1024);
                    }
                }
            }

            //var successDialog = new MessageDialog("Done saving the file!", "Success");
            //await successDialog.ShowAsync();
            CreaNotasDeArchivosTextoAlmacenamientoAislado();

           // ShowBusy(false);
        }


        private async void CreaNotasDeArchivosTextoAlmacenamientoAislado()
        {
            try
            {
                ShowBusy(true);
                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    string[] fileNames = isoStore.GetFileNames("./MyNotes/*.txt");

                    if (fileNames.Length > 0)
                    {
                        for (int i = 0; i < fileNames.Length; ++i)
                        {
                            System.Diagnostics.Debug.WriteLine(await System.Threading.Tasks.Task.Run(delegate
                            {
                                return traduce("MsgRestaurando") + " " + fileNames[i].Replace(".txt", "");
                            }));


                            using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("MyNotes/" + fileNames[i], FileMode.Open, isoStore))
                            {
                                using (StreamReader reader = new StreamReader(isoStream))
                                {
                                    string asunto = fileNames[i].Replace(".txt", "");
                                    string detalle = "";
                                    DateTime fecha = DateTime.Now;
                                    int fsize = 10;
                                    string temp;
                                    while (!reader.EndOfStream)
                                    {
                                        temp = reader.ReadLine();
                                        if (temp.StartsWith("[date:"))
                                        {
                                            try
                                            {
                                                fecha = Convert.ToDateTime(temp.Substring(7, temp.Length - 9));
                                            }
                                            catch
                                            {
                                                fecha = DateTime.Now;
                                            }
                                            
                                        }
                                        else if (temp.StartsWith("[fsize:"))
                                        {
                                            fsize = Convert.ToInt32(temp.Substring(8, temp.Length - 10));
                                        }
                                        else
                                        {
                                            if (temp != "")
                                                detalle += temp + System.Environment.NewLine;
                                        }
                                    }

                                    using (var conn = new SQLiteConnection(new SQLitePlatformWinRT(), App.DbConnectionString))
                                    {
                                        Nota nota = new Nota()
                                        {
                                            Id = SearchNotaID(asunto, detalle),
                                            Asunto = asunto,
                                            Fecha = fecha,
                                            Detalle = detalle,
                                            FSize = fsize,
                                            CFondo = "#ED341D"
                                        };

                                        //conn.InsertOrReplace(new Nota() { Asunto=asunto, Detalle = detalle, Fecha = fecha, FSize = fsize, CFondo = "#ED341D" }, typeof(Nota));

                                        if (nota.Id.Equals(0))
                                        {
                                            conn.RunInTransaction(() =>
                                            {
                                                conn.Insert(nota);
                                            });
                                        }
                                        else
                                        {
                                            conn.RunInTransaction(() =>
                                            {
                                                conn.Update(nota);
                                            });
                                        }
                                        
                                    }
                                }
                            }
                        }
                        // Confirm that no files remain.
                        //fileNames = isoStore.GetFileNames("*.txt");

                        ShowBusy(false);
                        T_Info.Text = traduce("MsgRestauracionFin");
                        T_ProgRestaura.Text = traduce("MsgFinalizado");
                        System.Diagnostics.Debug.WriteLine("Finalizado");
                    }
                }
            }
            catch (Exception ex)
            {
                var successDialog = new MessageDialog(traduce("MsgErrorGeneral") + " " + ex.Message, traduce("NameApp"));
                await successDialog.ShowAsync();
            }
        }

        private int SearchNotaID(string asunto, string detalles)
        {
            int result;
            using (var conn = new SQLiteConnection(new SQLitePlatformWinRT(), App.DbConnectionString))
            {
                result = (from n in conn.Table<Nota>()
                          where (n.Asunto.Equals(asunto) && n.Detalle.Equals(detalles))
                          select n.Id).FirstOrDefault();
            }
            return result;
        }

        private void ShowBusy(bool isBusy)
        {
            Progress.IsActive = isBusy;
            PleaseWaitCache.Visibility = isBusy ? Visibility.Visible : Visibility.Collapsed;
        }
        



        private async void B_Backup_Click(object sender, RoutedEventArgs e)
        {
            //var dialog3 = new MessageDialog($"Coming soon!. \r\nRestore function is now enabled to restore the backup of 'My Notes' for Windows Phone. If you were a user of this app and you backed up your notes, you can restore them now.");
            //await dialog3.ShowAsync();

            var dialog = new Windows.UI.Popups.MessageDialog(traduce("MsgAlertaBackup"), traduce("NameApp"));

            dialog.Commands.Add(new Windows.UI.Popups.UICommand(traduce("txtSi")) { Id = 0 });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand(traduce("txtNo")) { Id = 1 });
            
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            var result = await dialog.ShowAsync();
            
            if ((int)result.Id == 0)
            {
                CreaBackupDB();
            }           
        }


        private async void CreaBackupDB()
        {
            // Obtengo la ruta completa al archivo de DB de Sqlite
            var lfolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var dbFile = await lfolder.GetFileAsync("Storage.sqlite");

            // Proceso a subir a OneDrive el archvio, dentro de la carpeta especial Aplicaciones
            ShowBusy(true);

            Exception error = null;

            try
            {
                var folder = await _service.GetAppRoot();

                using (var stream = await dbFile.OpenStreamForReadAsync())
                {
                    var info = await _service.SaveFile(folder.Id, dbFile.Name, stream);

                    // Save for the GetLink demo
                    _savedId = info.Id;

                    var successDialog = new MessageDialog(traduce("MsgBackupFin"), traduce("NameApp"));
                    await successDialog.ShowAsync();
                }

                ShowBusy(false);
            }
            catch (Exception ex)
            {
                error = ex;
            }

            if (error != null)
            {
                var dialog2 = new MessageDialog(traduce("MsgErrorGeneral") + " " + error.Message, traduce("NameApp"));
                await dialog2.ShowAsync();
                ShowBusy(false);
            }
        }

        private async void RestauraDB()
        {
            var folder = await _service.GetSpecialFolder(OneDriveSimple.Request.SpecialFolder.AppRoot);
            string rpath = "Storage.sqlite";

            if (string.IsNullOrEmpty(rpath))
            {
                var dialog = new MessageDialog("Please enter a path to an existing file, for example Apps/My Notes/Storage.sqlite", "Error!");
                await dialog.ShowAsync();
                return;
            }

            Exception error = null;
            ItemInfoResponse foundFile = null;
            Stream contentStream = null;

            ShowBusy(true);

            try
            {
                foundFile = await _service.GetItem(folder.Id, rpath);

                if (foundFile == null)
                {
                    var dialog = new MessageDialog(traduce("MsgNoEncontrado") + rpath);
                    await dialog.ShowAsync();
                    ShowBusy(false);
                    return;
                }

                // Get the file's content
                contentStream = await _service.RefreshAndDownloadContent(foundFile, false);

                if (contentStream == null)
                {
                    var dialog = new MessageDialog(traduce("MsgContenidoNoEncontrado") + rpath);
                    await dialog.ShowAsync();
                    ShowBusy(false);
                    return;
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            if (error != null)
            {
                var dialog = new MessageDialog(traduce("MsgErrorGeneral") + " " + error.Message, traduce("NameApp"));
                await dialog.ShowAsync();
                ShowBusy(false);
                return;
            }
            
            // Save the retrieved stream to the local drive            
            IsolatedStorageFile isoFile;
            isoFile = IsolatedStorageFile.GetUserStoreForApplication();

            // Open or create a writable file.
            using (IsolatedStorageFileStream isoStream =
                new IsolatedStorageFileStream(Path.Combine("", foundFile.Name),
                FileMode.OpenOrCreate,
                FileAccess.Write,
                isoFile))
            {
                using (var writer = new BinaryWriter(isoStream))
                {
                    contentStream.Position = 0;

                    using (var reader = new BinaryReader(contentStream))
                    {
                        byte[] bytes;

                        do
                        {
                            bytes = reader.ReadBytes(1024);
                            writer.Write(bytes);
                        }
                        while (bytes.Length == 1024);
                    }
                }
            }

            var successDialog = new MessageDialog(traduce("MsgRestauracionFin"), traduce("NameApp"));
            await successDialog.ShowAsync();
            ShowBusy(false);
        }


#pragma warning disable IDE1006 // Estilos de nombres
        string traduce(string cadena)
#pragma warning restore IDE1006 // Estilos de nombres
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var str = loader.GetString(cadena);
            return str;
        }

    }
}

