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
    [ValueConversion(typeof(bool), typeof(bool))]
    public sealed class ConverterMultiBoolToBool : IMultiValueConverter
    {
        public bool TrueValue { get; set; }
        public bool FalseValue { get; set; }

        public ConverterMultiBoolToBool()
        {
            // set defaults
            TrueValue = true;
            FalseValue = false;
        }

        public object Convert(object[] values, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (values.Where(x => !(x is bool)).Count() > 0)
                return null;

            return values.Where(x => (bool)x == true).Count() > 0 ? TrueValue : FalseValue;
        }

        public object[] ConvertBack(object values, Type[] targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
