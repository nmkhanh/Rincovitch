using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WPF;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Brush = System.Windows.Media.Brush;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;

namespace RincovitchApp.Models.ModelChilds
{

  public partial class NMK_M_ProjectSchedule : ObservableObject
  {
    private double _Time = 80;
    public double Time
    {
      get
      {
        return _Time;
      }
      set
      {
        _Time = value;
        OnPropertyChanged();
      }
    }

    private SolidColorPaint _Color;
    public SolidColorPaint Color 
    {
      get
      {
        return _Color;
      }
      set
      {
        _Color = value;
        OnPropertyChanged();
      }
    }

    private double _PixelsPerDay = 80;
    public double PixelsPerDay
    {
      get
      {
        return _PixelsPerDay;
      }
      set
      {
        _PixelsPerDay = value;
        OnPropertyChanged();
      }
    }

    private double _PixelsPerUser = 30;
    public double PixelsPerUser
    {
      get
      {
        return _PixelsPerUser;
      }
      set
      {
        _PixelsPerUser = value;
        OnPropertyChanged();
      }
    }

    [ObservableProperty]
    private ISeries[] _Series;
    [ObservableProperty]
    private Axis[] _XAxes;
    [ObservableProperty]
    private Axis[] _YAxes;

    ObservableCollection<double> _TotalTime = new ObservableCollection<double>();
    public ObservableCollection<double> TotalTime
    {
      get { return _TotalTime; }
      set
      {
        _TotalTime = value;
        OnPropertyChanged();
      }
    }

    private Func<ChartPoint, string> _MyFormatter = point => $"{point.Coordinate.PrimaryValue:F2}";
    public Func<ChartPoint, string> MyFormatter
    {
      get
      {
        return _MyFormatter;
      }
      set
      {
        _MyFormatter = value;
        OnPropertyChanged();
      }
    }
  }

  public class NMK_M_DateSchedule : BaseViewModel
  {
    private double _Left;
    public double Left
    {
      get
      {
        return _Left;
      }
      set
      {
        _Left = value;
        OnPropertyChanged();
      }
    }

    #region Filter
    private bool _isVisible = true;
    public bool IsVisible
    {
      get => _isVisible;
      set
      {
        _isVisible = value;
        OnPropertyChanged();
      }
    }

    public bool Filter(int week, int month, int year)
    {
      bool match = true;
      if (!string.IsNullOrEmpty(year.ToString()) && year != 0)
      {
        match &= YearSchedule == year;
        if (!string.IsNullOrEmpty(month.ToString()) && month != 0)
        {
          match &= MonthSchedule == month;
          if (!string.IsNullOrEmpty(week.ToString()) && week != 0)
          {
            match &= WeekSchedule == week;
          }
        }
      }

      bool childMatch = false;

      IsVisible = match || childMatch;
      return IsVisible;
    }
    #endregion

    DateTime _Date = DateTime.Now;
    public DateTime Date
    {
      get { return _Date; }
      set
      {
        _Date = value;
        OnPropertyChanged();
        WeekSchedule = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(Date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        MonthSchedule = Date.Month;
        YearSchedule = Date.Year;
      }
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
      }
    }

    int _MonthSchedule;
    public int MonthSchedule
    {
      get { return _MonthSchedule; }
      set
      {
        _MonthSchedule = value;
        OnPropertyChanged();
      }
    }

    int _YearSchedule;
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

    string _Task;
    //int _Year = 2025;
    public string Task
    {
      get { return _Task; }
      set
      {
        _Task = value;
        OnPropertyChanged();
      }
    }

    ObservableCollection<NMK_M_DateSchedule> _Tasks = new ObservableCollection<NMK_M_DateSchedule>();
    public ObservableCollection<NMK_M_DateSchedule> Tasks
    {
      get { return _Tasks; }
      set
      {
        _Tasks = value;
        OnPropertyChanged();
      }
    }

    double _Time = 0;
    public double Time
    {
      get { return _Time; }
      set
      {
        _Time = value;
        OnPropertyChanged();
      }
    }
  }
}
