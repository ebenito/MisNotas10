using System;

namespace MyNotes10.Services.NavigationService
{
    public interface INavigationService
    {
        void Navigate(Type sourcePageType);
        void Navigate(Type sourcePageType, object parameter);
        void GoBack();

    }
}
