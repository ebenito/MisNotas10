using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// http://stackoverflow.com/questions/26380175/navigating-to-a-new-page-from-the-view-model-in-windows-phone-8-1-universal-app

namespace MyNotes10.Services.NavigationService
{
    public class NavigationService : INavigationService
    {
        public void Navigate(Type sourcePageType)
        {
            ((Frame)Window.Current.Content).Navigate(sourcePageType);
        }

        public void Navigate(Type sourcePageType, object parameter)
        {
            ((Frame)Window.Current.Content).Navigate(sourcePageType, parameter);
        }

        public void GoBack()
        {
            ((Frame)Window.Current.Content).GoBack();
        }
    }
}
