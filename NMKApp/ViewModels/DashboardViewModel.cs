using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMKApp.Models;
using NMKApp.Services;
using System.Windows.Data;

namespace NMKApp.ViewModels;

/// <summary>
/// Dashboard page ViewModel.
/// Handles task display, filtering, and task actions (complete, start, checked, etc.)
/// Extracted from MainWindowViewModel task-related commands.
/// </summary>
public partial class DashboardViewModel : ObservableObject
{
  private readonly ISupabaseService _supabaseService;
  private readonly IMailService _mailService;
  private readonly IToastService _toastService;

  // Parent reference for shared data
  private MainWindowViewModel? _parent;

  public ListCollectionView? TasksProjectCollection { get; set; }
  public ListCollectionView? TasksProjectCollectionCount { get; set; }
  public ListCollectionView? TasksProjectCollectionTimeline { get; set; }

  public DashboardViewModel(ISupabaseService supabaseService, IMailService mailService, IToastService toastService)
  {
    _supabaseService = supabaseService;
    _mailService = mailService;
    _toastService = toastService;
  }

  public void Initialize(MainWindowViewModel parent)
  {
    _parent = parent;
    InitializeCollectionViews();
  }

  private void InitializeCollectionViews()
  {
    if (_parent == null) return;

    TasksProjectCollection = new ListCollectionView(_parent.Tasks.Items);
    TasksProjectCollection.GroupDescriptions.Add(new PropertyGroupDescription("Project"));
    TasksProjectCollection.Filter = FilterTask;

    TasksProjectCollectionCount = new ListCollectionView(_parent.Tasks.Items);
    TasksProjectCollectionCount.Filter = obj =>
    {
      if (obj is not TaskModel task) return false;
      if (task.Status < 3) return false;
      return task.IsAssignedTo == _parent.IsAssignedTo;
    };

    TasksProjectCollectionTimeline = new ListCollectionView(_parent.Tasks.Items);
    TasksProjectCollectionTimeline.GroupDescriptions.Add(new PropertyGroupDescription("Id"));
  }

  private bool FilterTask(object obj)
  {
    if (obj is not TaskModel task || _parent == null) return false;

    bool matchTime = true;
    if (_parent.FilterYear != 0)
    {
      matchTime = task.DateStart.Year == _parent.FilterYear || task.DateEnd.Year == _parent.FilterYear;
      if (_parent.FilterMonth != 0)
        matchTime &= task.DateStart.Month == _parent.FilterMonth || task.DateEnd.Month == _parent.FilterMonth;
    }

    bool matchToday = !_parent.FilterToday ||
      (task.DateStart.Date <= DateTime.Now.Date && task.DateEnd.Date >= DateTime.Now.Date);

    var checkedStatuses = _parent.FiltersStatus.Where(x => x.IsChecked).Select(x => x.State).ToList();
    bool matchStatus = checkedStatuses.Count > 0 && checkedStatuses.Contains(task.Status);

    bool matchProject = _parent.SelectedProject == null ||
      string.IsNullOrEmpty(_parent.SelectedProject.Id) ||
      _parent.SelectedProject.Id == task.ProjectId;

    bool matchAssignedTo = task.IsAssignedTo == _parent.IsAssignedTo;

    return matchTime && matchStatus && matchProject && matchAssignedTo && matchToday;
  }

  public void RefreshViews()
  {
    TasksProjectCollection?.Refresh();
    TasksProjectCollectionCount?.Refresh();
    TasksProjectCollectionTimeline?.Refresh();
  }

  // ─── Task Commands ──────────────────────────────────────────────

  [RelayCommand]
  private async Task TaskNewAsync()
  {
    // TODO: Migrate TaskNewCommandAsync from MainWindowViewModel
    await Task.CompletedTask;
  }

  [RelayCommand]
  private async Task TaskEditAsync(object? parameter)
  {
    // TODO: Migrate TaskEditCommandAsync
    await Task.CompletedTask;
  }

  [RelayCommand]
  private async Task TaskDeleteAsync(object? parameter)
  {
    // TODO: Migrate TaskDeleteCommandAsync
    await Task.CompletedTask;
  }

  [RelayCommand]
  private async Task TaskCompleteAsync(object? parameter)
  {
    // TODO: Migrate TaskCompleteCommandAsync
    await Task.CompletedTask;
  }

  [RelayCommand]
  private async Task TaskStartAsync(object? parameter)
  {
    // TODO: Migrate TaskStartCommandAsync
    await Task.CompletedTask;
  }

  [RelayCommand]
  private async Task TaskCheckedAsync(object? parameter)
  {
    // TODO: Migrate TaskCheckedCommandAsync
    await Task.CompletedTask;
  }

  [RelayCommand]
  private async Task TaskReCheckedAsync(object? parameter)
  {
    // TODO: Migrate TaskReCheckedCommandAsync
    await Task.CompletedTask;
  }

  [RelayCommand]
  private async Task TaskAcceptAsync(object? parameter)
  {
    // TODO: Migrate TaskAcceptCommandAsync
    await Task.CompletedTask;
  }

  [RelayCommand]
  private void FilesDropped(object? parameter)
  {
    // TODO: Migrate FilesDroppedCommand
  }
}
