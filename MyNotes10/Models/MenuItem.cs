using MyNotes10.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyNotes10.Models
{
    public class MenuItem : BindableBase
    {
        private string glyph;
        private string text;
        private DelegateCommand command;
        private string commandParam;
        private Type navigationDestination;
        private Object param;

        public string Glyph
        {
            get { return glyph; }
            set { SetProperty(ref glyph, value); }
        }

        public string Text
        {
            get { return text; }
            set { SetProperty(ref text, value); }
        }

        public ICommand Command
        {
            get { return command; }
            set { SetProperty(ref command, (DelegateCommand)value); }
        }

        public string CommandParam
        {
            get { return commandParam; }
            set { SetProperty(ref commandParam, value); }
        }
        
        public Type NavigationDestination
        {
            get { return navigationDestination; }
            set { SetProperty(ref navigationDestination, value); }
        }

        public Object Param
        {
            get { return param; }
            set { SetProperty(ref param, value); }
        }

        public bool IsNavigation
        {
            get { return navigationDestination != null; }
        }
    }
}
