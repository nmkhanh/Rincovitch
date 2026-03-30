using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RincovitchApp.Models.ModelChilds
{
  public class NMK_M_FilterDay : BaseViewModel
  {
    int _Month = DateTime.Now.Month;
    public int Month
    {
      get { return _Month; }
      set
      {
        _Month = value;
        OnPropertyChanged();
      }
    }

    int _Year = DateTime.Now.Year;
    //int _Year = 2025;
    public int Year
    {
      get { return _Year; }
      set
      {
        _Year = value;
        OnPropertyChanged();
      }
    }

    DateTime _SearchSchedule = DateTime.Now.Date;
    public DateTime SearchSchedule
    {
      get { return _SearchSchedule; }
      set
      {
        _SearchSchedule = value;
        OnPropertyChanged();
        WeekSchedule = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(SearchSchedule, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
      }
    }

    public NMK_M_FilterDay()
    {
      WeekSchedule = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(SearchSchedule, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    }

    int _WeekSchedule;
    //int _Year = 2025;
    public int WeekSchedule
    {
      get { return _WeekSchedule; }
      set
      {
        _WeekSchedule = value;
        OnPropertyChanged();
        if (WeekSchedule != 0)
        {
          DateTime monday = ISOWeek.ToDateTime(YearSchedule, WeekSchedule, DayOfWeek.Monday);
          MonthSchedule = monday.Month;
        }
      }
    }


    int _MonthSchedule = DateTime.Now.Month;
    public int MonthSchedule
    {
      get { return _MonthSchedule; }
      set
      {
        _MonthSchedule = value;
        OnPropertyChanged();
      }
    }

    int _YearSchedule = DateTime.Now.Year;
    //int _Year = 2025;
    public int YearSchedule
    {
      get { return _YearSchedule; }
      set
      {
        _YearSchedule = value;
        OnPropertyChanged();
      }
    }
  }
}
