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
  [ValueConversion(typeof(Visibility), typeof(Visibility))]
  public sealed class ConverterVisibilityReverse : IValueConverter
  {
    public Visibility TrueValue { get; set; }
    public Visibility FalseValue { get; set; }

    public ConverterVisibilityReverse()
    {
      // set defaults
      TrueValue = Visibility.Visible;
      FalseValue = Visibility.Collapsed;
    }

    public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
      if (!(value is Visibility visible))
        return FalseValue;
      if (visible ==  Visibility.Visible)
        return FalseValue;
      if (visible == Visibility.Collapsed)
        return TrueValue;
      return visible;
    }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
    {
      if (Equals(value, TrueValue))
        return FalseValue;
      if (Equals(value, FalseValue))
        return TrueValue;
      return FalseValue;
    }
  }
}
