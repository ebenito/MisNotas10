using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace MyNotes10.Services.LoaderService
{
    public class LoaderService : ILoaderService
    {
        ResourceLoader loader;
        public LoaderService()
        {
            loader = ResourceLoader.GetForCurrentView();
        }
        public string getString(string key)
        {
            return loader.GetString(key);
        }
    }
}


