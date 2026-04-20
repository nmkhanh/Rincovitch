using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMKApp.Models;
using NMKApp.Services;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace NMKApp.ViewModels;

/// <summary>
/// Schedule page ViewModel.
/// Handles schedule grid with project/user days visualization.
/// </summary>
public partial class ScheduleViewModel : ObservableObject
{
  private MainWindowViewModel? _parent;

  public ListCollectionView? DaysWeekCollectionSchedules { get; set; }
  public ListCollectionView? TasksUserCollectionAdminSchedule { get; set; }

  public ObservableCollection<DayModel> DaysSchedules { get; set; } = [];

  [ObservableProperty] private DateTime _minDaySchedules;
  [ObservableProperty] private DateTime _maxDaySchedules;

  public ScheduleModel ProjectSchedulesAssignTo { get; set; } = new();
  public ScheduleModel UsersSchedulesAdmin { get; set; } = new();

  public void Initialize(MainWindowViewModel parent)
  {
    _parent = parent;
    // TODO: Migrate schedule collection views from NMK_M
  }

  [RelayCommand]
  private void FilterDay(object? parameter)
  {
    // TODO: Migrate FilterDayScheduleCommand
  }

  [RelayCommand]
  private void StatusChange(object? parameter)
  {
    // TODO: Migrate ScheduleStatusChangeCommand
  }

  public void RefreshViews()
  {
    DaysWeekCollectionSchedules?.Refresh();
    TasksUserCollectionAdminSchedule?.Refresh();
  }
}
