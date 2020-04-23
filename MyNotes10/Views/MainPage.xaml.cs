using MyNotes10.Models;
using MyNotes10.ViewModels;
using MyNotes10.ViewModels.Base;
using MyNotes10.Views.Base;
using OneDriveSimple;
using OneDriveSimple.Helpers;
using OneDriveSimple.Response;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class MainPage : PageBase
    {        
        private MenuItem myItem;
        
        public MainPage()
        {
            this.InitializeComponent();
            this.SizeChanged += MainPage_SizeChanged;

            MainPageViewModel vm = this.DataContext as MainPageViewModel;

            MenuFlyout mnu = new MenuFlyout();
            MenuFlyoutItem item1 = new MenuFlyoutItem();
            MenuFlyoutItem item2 = new MenuFlyoutItem();
            item1.Text = traduce("B_EditarNota");
            mnu.Items.Add(item1);
            item1.Click += mflyoutEditar_Click;

            item2.Text = traduce("B_BorrarNota");
            item2.Command = vm.DeleteNotaCommand;  //"{Binding MainPageViewModel.DeleteNotaCommand, Source={StaticResource Locator}}";
            item2.Click += Item2_Click;

            mnu.Items.Add(item2);
            lstNotas.ContextFlyout = mnu;
        }
        
        private void mflyoutEditar_Click(object sender, RoutedEventArgs e)
        {
            if (App.SelNota == 0)
            {
                ContentDialog messageDialog = new ContentDialog()
                {
                    Title = traduce("NameApp"),
                    Content = traduce("MsgNoNotaEditarNoAbre")
                };

                messageDialog.PrimaryButtonText = "OK";
                var res = messageDialog.ShowAsync();
            }
            else
            {
                this.Frame.Navigate(typeof(GestionNota), "edit");
            }
        }

        private void Item2_Click(object sender, RoutedEventArgs e)
        {
            if (App.SelNota == 0)
            {
                ContentDialog messageDialog = new ContentDialog()
                {
                    Title = traduce("NameApp"),
                    Content = traduce("MsgNoNotaEditarNoAbre")
                };

                messageDialog.PrimaryButtonText = "OK";
                var res = messageDialog.ShowAsync();
            }
            //else
            //{
            //    String path = App.DbConnectionString;
            //    Nota nota = ((MyNotes10.Models.Nota)lstNotas.SelectedItem);
            //    using (var conn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            //    {
            //        conn.RunInTransaction(() =>
            //        {
            //            conn.Delete(nota);
            //        });
            //    }

            //    this.Frame.Navigate(typeof(MainPage)); //Recargar la lista
            //}
               
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (App.IsMenuVisible)
            {
                Separar.Width = 50;
            }
            else
            {
                Separar.Width = 0;
            }

        }

        private void CreaMenu()
        {
            if ((this.DataContext as ViewModelBase).Menu.Count != 0)
            {
                (this.DataContext as ViewModelBase).Menu.Clear();
            }
            
            myItem = new MenuItem() { Glyph = "", Text = traduce("MenuTodasNotas"), NavigationDestination = typeof(MainPage) };
            (this.DataContext as ViewModelBase).Menu.Add(myItem);

            myItem = new MenuItem() { Glyph = "", Text = traduce("MenuNueva"), NavigationDestination = typeof(GestionNota), Param = "New" };
            (this.DataContext as ViewModelBase).Menu.Add(myItem);

            myItem = new MenuItem() { Glyph = "", Text = traduce("MenuEditar"), NavigationDestination = typeof(GestionNota), Param = "Edit" };
            (this.DataContext as ViewModelBase).Menu.Add(myItem);

            myItem = new MenuItem() { Glyph = "", Text = traduce("MenuBackup"), NavigationDestination = typeof(BackupOneDrive), Param = "Backup" };
            (this.DataContext as ViewModelBase).Menu.Add(myItem);

            myItem = new MenuItem() { Glyph = "", Text = traduce("MenuValorar"), Command = (DataContext as MainPageViewModel).OpenStoreCommand };
            (this.DataContext as ViewModelBase).Menu.Add(myItem);

            myItem = new MenuItem() { Glyph = "", Text = "Language", NavigationDestination = typeof(Seldioma) };
            (this.DataContext as ViewModelBase).Menu.Add(myItem);

            myItem = new MenuItem() { Glyph = "", Text = traduce("Privacy"), Command = (DataContext as MainPageViewModel).OpenPrivacyCommand };
            (this.DataContext as ViewModelBase).Menu.Add(myItem);
        }

        string traduce(string cadena)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var str = loader.GetString(cadena);
            return str;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            CreaMenu();
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            var query = (this.DataContext as ViewModelBase).Menu.Select((menu, index) =>
                                                                new { index, str = menu });

            foreach (var obj in query)
            {
                System.Diagnostics.Debug.WriteLine("{0}", obj.str);
            }
                      
            base.OnNavigatedFrom(e);
        }

        private void ListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            ContentDialog messageDialog = new ContentDialog()
            {
                Title = "Mis notas",
                Content = lstNotas.SelectedItem.ToString()
            };

            messageDialog.PrimaryButtonText = "OK";
            var res = messageDialog.ShowAsync();
        }

       
    }
}
