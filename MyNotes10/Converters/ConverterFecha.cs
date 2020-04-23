using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;


namespace MyNotes10.Converters
{
    public class ConverterFecha : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTime fecha = (DateTime)value;
            var culture = CultureInfo.CurrentCulture;     //new CultureInfo("en-GB");
            var dateValue = new DateTime(fecha.Year, fecha.Month, fecha.Day, fecha.Hour, fecha.Minute, fecha.Second);
            var result = dateValue.ToString("F", culture); //F: FullDateTimePattern(fecha larga y hora larga) https://msdn.microsoft.com/es-es/library/system.globalization.datetimeformatinfo%28v=vs.100%29.aspx

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
