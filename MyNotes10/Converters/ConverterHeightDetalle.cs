using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.Xaml.Data;

namespace MyNotes10.Converters
{
    public class ConverterHeightDetalle : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                string _deviceFamily;
                Double resta;

                var qualifiers = ResourceContext.GetForCurrentView().QualifierValues;
                _deviceFamily = qualifiers.First(q => q.Key.Equals("DeviceFamily")).Value;

                if (_deviceFamily.Equals("Mobile"))
                {
                    resta = System.Convert.ToDouble(parameter) - 80;
                }
                else
                {
                    resta = System.Convert.ToDouble(parameter);
                }

                double val = System.Convert.ToDouble(value) - resta;    
                return val;
            }
            catch
            {
                return "300";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
