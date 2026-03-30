using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RincovitchApp.UITheme.Converter
{
    [ValueConversion(typeof(bool), typeof(double))]
    public sealed class ConverterBoolToDouble : IValueConverter
    {
        public double TrueValue { get; set; }
        public double FalseValue { get; set; }
        public ConverterBoolToDouble()
        {
            TrueValue = 0;
            FalseValue = 0;
        }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return null;
            return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
