using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace RevitFamilyManager.UIThemes.Converter
{
  [ValueConversion(typeof(bool), typeof(Brush))]
  public sealed class ConverterBoolToForeground : IValueConverter
  {
    public Brush Color { get; set; }
    public bool BoolValue { get; set; }
    public ConverterBoolToForeground()
    {
      BoolValue = true;
      Color = Brushes.LightGreen;
    }

    public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
      if (!(value is bool))
        return null;
      return (bool)value == BoolValue ? Color : Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
