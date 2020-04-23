using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;


namespace MyNotes10.Converters
{
    public class ConverterFechaLocal : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTime fecha = (DateTime)value;
            fecha = fecha.ToUniversalTime();

            return fecha.ToLocalTime(); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
