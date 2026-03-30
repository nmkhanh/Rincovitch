using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RincovitchTools.UITheme.Converter
{
  [ValueConversion(typeof(bool), typeof(bool))]
  public sealed class ConverterMultiBoolToDouble : IMultiValueConverter
  {
    public string State { get; set; }
    public double Value { get; set; }
    public ConverterMultiBoolToDouble()
    {
      State = "Or";
      Value = 0;
    }

    public object Convert(object[] values, Type targetType,
            object parameter, CultureInfo culture)
    {
      if (values.Where(x => !(x is bool)).Count() > 0)
        return null;

      if (State == "Or")
        return values.Where(x => (bool)x == true).Count() > 0 ? Value : 0;
      else if (State == "And")
        return values.Where(x => (bool)x == false).Count() > 0 ? 0 : Value;
      else
        return 0;
    }

    public object[] ConvertBack(object values, Type[] targetType,
        object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
