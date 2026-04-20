using CommunityToolkit.Mvvm.ComponentModel;

namespace NMKApp.Models;

/// <summary>
/// Date filter model for timeline/schedule views.
/// </summary>
public partial class FilterDayModel : ObservableObject
{
  [ObservableProperty] private int _month;
  [ObservableProperty] private int _year;
  [ObservableProperty] private string _searchSchedule = string.Empty;
  [ObservableProperty] private int _weekSchedule;
  [ObservableProperty] private int _monthSchedule;
  [ObservableProperty] private int _yearSchedule;
}
