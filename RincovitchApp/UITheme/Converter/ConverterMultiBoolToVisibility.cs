using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using static System.Windows.Forms.AxHost;

namespace RincovitchApp.UITheme.Converter
{
  [ValueConversion(typeof(bool), typeof(Visibility))]
  public sealed class ConverterMultiBoolToVisibility : IMultiValueConverter
  {
    public string State { get; set; }
    public Visibility TrueValue { get; set; }
    public Visibility FalseValue { get; set; }

    public ConverterMultiBoolToVisibility()
    {
      State = "And";

      // set defaults
      TrueValue = Visibility.Visible;
      FalseValue = Visibility.Collapsed;
    }

    public object Convert(object[] values, Type targetType,
            object parameter, CultureInfo culture)
    {
      if (values.Where(x => !(x is bool)).Count() > 0)
        return FalseValue;

      if (State == "Or")
        return values.Any(x => (bool)x == true) ? TrueValue : FalseValue;
      else if (State == "And")
        return values.All(x => (bool)x == true) ? TrueValue : FalseValue;
      else
        return FalseValue;
    }

    public object[] ConvertBack(object values, Type[] targetType,
        object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
