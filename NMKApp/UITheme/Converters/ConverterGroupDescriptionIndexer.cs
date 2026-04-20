using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NMKApp.UITheme.Converters
{
  public class GroupDescriptionIndexerConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var collection = value as ObservableCollection<GroupDescription>;
      int index = int.Parse(parameter.ToString());

      if (collection != null && collection.Count > index)
      {
        // Trả về PropertyName của group (ví dụ: "Category")
        return ((PropertyGroupDescription)collection[index]).PropertyName;
      }
      return null;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }
}
