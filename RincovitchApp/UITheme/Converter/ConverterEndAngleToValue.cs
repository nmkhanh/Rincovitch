using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RincovitchApp.UITheme.Converter
{
    [ValueConversion(typeof(double), typeof(double))]
    public sealed class ConverterEndAngleToValue : IMultiValueConverter
    {

        public ConverterEndAngleToValue()
        {
            // set defaults
        }

        public object Convert(object[] values, Type targetType,
            object parameter, CultureInfo culture)
        {
            return (double)values[0] * 360 / (double)values[1];
        }

        public object[] ConvertBack(object value, Type[] targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
