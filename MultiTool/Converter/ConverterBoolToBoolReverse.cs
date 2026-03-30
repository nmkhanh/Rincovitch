using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RevitFamilyManager.UIThemes.Converter
{
  [ValueConversion(typeof(bool), typeof(double))]
  public sealed class ConverterBoolToBoolReverse : IValueConverter
  {
    public ConverterBoolToBoolReverse()
    {

    }

    public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
      if (!(value is bool))
        return null;
      return !(bool)value;
    }

    public object ConvertBack(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
