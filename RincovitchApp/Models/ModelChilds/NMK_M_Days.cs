using RincovitchApp.API.Date;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RincovitchApp.Models.ModelChilds
{
  public partial class NMK_M_Day : ObservableObject
  {
    [ObservableProperty]
    ObservableCollection<NMK_M_Day> _Items = new ObservableCollection<NMK_M_Day>();

    [ObservableProperty]
    HolidayResponse _Holiday;

    #region Filter
    [ObservableProperty]
    private bool _isVisible = true;

    public bool Filter(DateTime start, DateTime end)
    {
      bool match = Name.Date >= start.Date && Name.Date <= end.Date;

      bool childMatch = false;
      foreach (var c in Items)
      {
        childMatch |= c.Filter(start, end);
      }

      IsVisible = match || childMatch;
      return IsVisible;
    }
    #endregion

    [ObservableProperty]
    DateTime _Name;
    partial void OnNameChanged(DateTime oldValue, DateTime newValue)
    {
      Year = Name.Year;
      Month = Name.Month;
      Week = $"Week : {CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(Name, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)}";
    }

    [ObservableProperty]
    bool _isChecked = false;

    [ObservableProperty]
    NMK_M_Leave _leave = new NMK_M_Leave();

    [ObservableProperty]
    string _Week;

    [ObservableProperty]
    int _Month;

    [ObservableProperty]
    int _Year;

    [ObservableProperty]
    bool _IsEnabled;

  }
}
