using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace NMKApp.Models;

/// <summary>
/// Day model for timeline/schedule views.
/// </summary>
public partial class DayModel : ObservableObject
{
  [ObservableProperty] private DateTime _name;
  [ObservableProperty] private string? _holiday;
  [ObservableProperty] private bool _isVisible = true;
  [ObservableProperty] private bool _isChecked;
  [ObservableProperty] private bool _isEnabled = true;
  [ObservableProperty] private LeaveModel? _leave;
  [ObservableProperty] private int _week;
  [ObservableProperty] private int _month;
  [ObservableProperty] private int _year;

  partial void OnNameChanged(DateTime value)
  {
    Year = value.Year;
    Month = value.Month;
    Week = System.Globalization.ISOWeek.GetWeekOfYear(value);
  }

  public bool Filter(DateTime start, DateTime end)
  {
    return Name >= start && Name <= end;
  }

  public ObservableCollection<DayModel> Items { get; set; } = [];
}

public class DayCollection
{
  public ObservableCollection<DayModel> Items { get; set; } = [];
}
