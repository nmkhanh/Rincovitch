using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Binding = System.Windows.Data.Binding;

namespace NMKApp.UITheme.Converters
{
  [ValueConversion(typeof(bool), typeof(bool))]
  public sealed class ConverterBoolReverse : IValueConverter
  {

    public ConverterBoolReverse()
    {
      // set defaults
    }
    public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
      return !(bool)value;
    }

    public object ConvertBack(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
      if (value == DependencyProperty.UnsetValue || value == null)
        return Binding.DoNothing;
      if (!(value is bool))
        throw new InvalidOperationException("ConverterBoolReverse expects a boolean.");
      return !(bool)value;
    }
  }
}
