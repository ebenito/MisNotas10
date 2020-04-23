using MyNotes10.Models;
using MyNotes10.ViewModels.Base;
using MyNotes10.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace MyNotes10.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        public ShellViewModel()
        {
            Menu.Clear(); //Para que no se repitan los items en la vista de diseño segun se accede varias veces a su visualización

            // Build the menu    //Para que no se repitan los items en la vista de diseño segun se accede varias veces a su visualización

            // Build the menu; Lo creo finalmente en MainPage, para borrar la selección de la ultima opción pulsada
            //Menu.Add(new MenuItem() { Glyph = "", Text = "Show all notes", NavigationDestination = typeof(MainPage) });
            //Menu.Add(new MenuItem() { Glyph = "", Text = "Add a new note", NavigationDestination = typeof(GestionNota), Param = "New" });
            //Menu.Add(new MenuItem() { Glyph = "", Text = "Edit", NavigationDestination = typeof(GestionNota), Param = "Edit" });
            //Menu.Add(new MenuItem() { Glyph = "", Text = "Backup", NavigationDestination = typeof(MainPage), Param = "Backup" });
            //Menu.Add(new MenuItem() { Glyph = "", Text = "Rate this App", NavigationDestination = typeof(MainPage), Param = "Rate" });
        }

        public override Task OnNavigatedFrom(NavigationEventArgs args)
        {
            return null;
        }

        public override Task OnNavigatedTo(NavigationEventArgs args)
        {
            return null;
        }
    }
}
