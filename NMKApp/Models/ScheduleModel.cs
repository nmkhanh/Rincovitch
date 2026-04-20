using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using System.Collections.ObjectModel;

namespace NMKApp.Models;

/// <summary>
/// Schedule visualization model for charts.
/// </summary>
public partial class ScheduleModel : ObservableObject
{
  [ObservableProperty] private DateTime _date;
  [ObservableProperty] private double _time = 80;
  [ObservableProperty] private SolidColorPaint? _color;
  [ObservableProperty] private double _pixelsPerDay = 40;
  [ObservableProperty] private double _pixelsPerUser;

  public ISeries[] Series { get; set; } = [];
  public Axis[] XAxes { get; set; } = [];
  public Axis[] YAxes { get; set; } = [];
  public ObservableCollection<double> TotalTime { get; set; } = [];
  public Func<ChartPoint, string>? MyFormatter { get; set; }

  public ObservableCollection<ScheduleModel> Items { get; set; } = [];
}
