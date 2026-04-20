using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace NMKApp.UITheme.Converters
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public sealed class ConverterNullToVisibility : IValueConverter
    {
        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public ConverterNullToVisibility()
        {
            // set defaults
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            // Fix: Cast value to MaterialIconKind before comparison
            if (value is Material.Icons.MaterialIconKind kind)
                return kind != Material.Icons.MaterialIconKind.Abacus ? TrueValue : FalseValue;
            // If value is not MaterialIconKind, treat as not Abacus
            return TrueValue;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (Equals(value, TrueValue))
                return true;
            if (Equals(value, FalseValue))
                return false;
            return null;
        }
    }
}
