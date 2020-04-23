using Microsoft.Practices.Unity;
using MyNotes10.Services.NotaService;
using MyNotes10.Services.DialogService;
using MyNotes10.Services.LoaderService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyNotes10.Services.NavigationService;
using Microsoft.Practices.ServiceLocation;

namespace MyNotes10.ViewModels.Base
{
    public class ViewModelLocator : IDisposable
    {
        readonly IUnityContainer _container;

        public ViewModelLocator()
        {
            _container = new UnityContainer();

            _container.RegisterType<MainPageViewModel>();

            _container.RegisterType<ILoaderService, LoaderService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<INotaService, NotaService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IDialogService, DialogService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<INavigationService, NavigationService>(new ContainerControlledLifetimeManager());
        }

        public MainPageViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainPageViewModel>();
            }
        }

        public MainPageViewModel MainPageViewModel
        {
            get { return _container.Resolve<MainPageViewModel>(); }
        }

        public GestionNotaViewModel GestionNotaViewModel
        {
            get { return _container.Resolve<GestionNotaViewModel>(); }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                _container.Dispose();
            }
            // free native resources if there are any.
        }


    }
}