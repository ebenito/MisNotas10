using MyNotes10.Models;
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
    public sealed partial class Shell : PageBase
    {
        public Shell()
        {
            this.InitializeComponent();

            // Navigate to the first page.
            if ((DataContext as ShellViewModel).Menu.Count > 0)
            {
                var type = (DataContext as ShellViewModel).Menu.First().NavigationDestination;
                SplitViewFrame.Navigate(type);
            }
            else
            {
                SplitViewFrame.Navigate(typeof(MainPage)); // Por defecto cargo MainPage, si no hay elementos de menú creados desde ShellViewModel
            }

            this.SizeChanged += Shell_SizeChanged;
            this.MySplitView.LayoutUpdated += MySplitView_LayoutUpdated;

            T_Version.Text = App.GetAppVersion();
        }

        private void MySplitView_LayoutUpdated(object sender, object e)
        {
            try
            {
                Logo.Visibility = MySplitView.IsPaneOpen ? Visibility.Visible : Visibility.Collapsed;

                if (!MySplitView.IsPaneOpen && MySplitView.DisplayMode == SplitViewDisplayMode.CompactOverlay)
                {
                    App.IsMenuVisible = true;                    
                }
                else
                {  
                    App.IsMenuVisible = MySplitView.IsPaneOpen;
                }
               

                //QuitaLogoSiApaisado();
                //string x = MySplitView.IsPaneOpen ? "Sí" : "No";
                //System.Diagnostics.Debug.WriteLine("MySplitView_LayoutUpdated: " +  x);
            }
            catch
            {
                //NADA
            }
        }


        // Update width indicator at bottom right of the screen.
        private void Shell_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            WidthIndicator.Text = "<- " + e.NewSize.Width.ToString() + " ->";
        }

        // Navigate to another page.
        private void Menu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var menuItem = e.AddedItems.First() as MenuItem;
                if (menuItem.IsNavigation)
                {
                    if (menuItem.Param != null)
                    {
                        SplitViewFrame.Navigate(menuItem.NavigationDestination, menuItem.Param);
                    }
                    else
                    {
                        SplitViewFrame.Navigate(menuItem.NavigationDestination);
                    }
                }
                else
                {
                    menuItem.Command.Execute(null);
                }
            }
        }

        // Swipe to open the splitview panel.
        private void SplitViewOpener_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (e.Cumulative.Translation.X > 50)
            {
                MySplitView.IsPaneOpen = true;
                Logo.Visibility = Visibility.Visible;
                QuitaLogoSiApaisado();
            }
        }

        // Swipe to close the splitview panel.
        private void SplitViewPane_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (e.Cumulative.Translation.X < -50)
            {
                MySplitView.IsPaneOpen = false;
                Logo.Visibility = Visibility.Collapsed;
            }
        }

        // Open or close the splitview panel through Hamburger button.
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
            if (MySplitView.IsPaneOpen)
            {
                Logo.Visibility = Visibility.Visible;
                QuitaLogoSiApaisado();
            }
            else
            {
                Logo.Visibility = Visibility.Collapsed;
            }
        }

        private void QuitaLogoSiApaisado()
        {
            try
            {
                if (this.ActualHeight < this.ActualWidth) //Modo apaisado
                {
                    Logo.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Logo.Visibility = Visibility.Visible;
                }
            }
            catch
            {
                Logo.Visibility = Visibility.Collapsed;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // tile ID sent from App.xaml.cs in the NavigationEventArgs parameter
            if (App.SecTile == true)
            {
                App.SelNota = Int32.Parse(e.Parameter.ToString());
                SplitViewFrame.Navigate(typeof(GestionNota), "Edit");
            }
        }
    }
}
