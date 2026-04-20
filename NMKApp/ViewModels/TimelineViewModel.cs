using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMKApp.Models;
using NMKApp.Services;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace NMKApp.ViewModels;

/// <summary>
/// Timeline page ViewModel.
/// Handles timeline/Gantt view of tasks with day-based navigation.
/// </summary>
public partial class TimelineViewModel : ObservableObject
{
  private MainWindowViewModel? _parent;

  public ListCollectionView? TasksUserCollection { get; set; }
  public ListCollectionView? DaysWeekCollection { get; set; }

  [ObservableProperty] private DateTime _minDay;
  [ObservableProperty] private DateTime _maxDay;

  public ObservableCollection<DayModel> Days { get; set; } = [];

  public DashboardViewModel? DashboardVM => _parent?.DashboardVM;

  public void Initialize(MainWindowViewModel parent)
  {
    _parent = parent;
    // TODO: Migrate timeline collection views and day generation from NMK_M
  }

  [RelayCommand]
  private void FilterDay(object? parameter)
  {
    // TODO: Migrate FilterDayCommand logic
  }

  public void RefreshViews()
  {
    TasksUserCollection?.Refresh();
    DaysWeekCollection?.Refresh();
  }
}
