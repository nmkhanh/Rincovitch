using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMKApp.Data.Entities;
using NMKApp.Models;
using NMKApp.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;

namespace NMKApp.ViewModels;

/// <summary>
/// Dashboard page ViewModel.
/// Handles task display, filtering, and task actions (complete, start, checked, re-check, accept).
/// </summary>
public partial class DashboardViewModel : ObservableObject
{
  private readonly ISupabaseService _supabaseService;
  private readonly IMailService _mailService;
  private readonly IToastService _toastService;

  private MainWindowViewModel? _parent;

  [ObservableProperty] private TaskModel? _selectedTask;
  [ObservableProperty] private bool _isLoading;

  [ObservableProperty] private ListCollectionView? _tasksProjectCollection;
  [ObservableProperty] private ListCollectionView? _tasksProjectCollectionCount;
  [ObservableProperty] private ListCollectionView? _tasksProjectCollectionTimeline;

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
    TasksProjectCollection.SortDescriptions.Add(
      new System.ComponentModel.SortDescription("DateEnd", System.ComponentModel.ListSortDirection.Ascending));
    TasksProjectCollection.Filter = FilterTask;

    TasksProjectCollectionCount = new ListCollectionView(_parent.Tasks.Items);
    TasksProjectCollectionCount.Filter = obj =>
    {
      if (obj is not TaskModel task) return false;
      if (task.Status < 3) return false;
      return task.IsAssignedTo == _parent.IsAssignedTo;
    };

    TasksProjectCollectionTimeline = new ListCollectionView(_parent.Tasks.Items);
    TasksProjectCollectionTimeline.SortDescriptions.Add(
      new System.ComponentModel.SortDescription("DateStart", System.ComponentModel.ListSortDirection.Ascending));
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

  // ─── Task New ─────────────────────────────────────────────────────────────
  [RelayCommand]
  private async Task TaskNewAsync()
  {
    if (_parent?.CurrentUser == null) return;
    var dlg = new Views.Dialogs.TaskEditDialog(_parent.Users.Items, _parent.Projects.Items);
    if (dlg.ShowDialog() != true) return;

    IsLoading = true;
    try
    {
      var entity = BuildEntityFromDialog(dlg, createBy: _parent.CurrentUser.Email);
      var result = await _supabaseService.InsertTaskAsync(entity);
      if (!result.Success) { ShowError(result.Error); return; }

      var task = result.Data!;
      task.Project = _parent.Projects.Items.FirstOrDefault(p => p.Id == task.ProjectId);
      task.User = _parent.Users.Items.FirstOrDefault(u => u.Id == task.UserId);
      task.IsAssignedTo = task.UserId == _parent.CurrentUser.Id;
      _parent.Tasks.Items.Add(task);
      RefreshViews();

      // Notify assignee by mail
      if (task.User?.Email != null)
      {
        _ = _mailService.SendTaskMailTypedAsync(
          task.User.Email, task.Name, task.User.Name,
          dlg.Description, task.DateStart, task.DateEnd,
          _parent.CurrentUser.Name);
      }
    }
    catch (Exception ex) { ShowError(ex.Message); }
    finally { IsLoading = false; }
  }

  // ─── Task Edit ────────────────────────────────────────────────────────────
  [RelayCommand]
  private async Task TaskEditAsync(object? parameter)
  {
    var task = parameter as TaskModel ?? SelectedTask;
    if (task == null || _parent?.CurrentUser == null) return;

    var dlg = new Views.Dialogs.TaskEditDialog(_parent.Users.Items, _parent.Projects.Items, task);
    if (dlg.ShowDialog() != true) return;

    IsLoading = true;
    try
    {
      var entity = BuildEntityFromDialog(dlg, createBy: task.CreateAt.ToString(), id: task.Id);
      entity.Id = task.Id;
      var result = await _supabaseService.UpdateTaskAsync(entity);
      if (!result.Success) { ShowError(result.Error); return; }

      var updated = result.Data!;
      task.Name = updated.Name;
      task.OnlyName = updated.OnlyName;
      task.DateStart = updated.DateStart;
      task.DateEnd = updated.DateEnd;
      task.ProjectId = updated.ProjectId;
      task.UserId = updated.UserId;
      task.Project = _parent.Projects.Items.FirstOrDefault(p => p.Id == updated.ProjectId);
      task.User = _parent.Users.Items.FirstOrDefault(u => u.Id == updated.UserId);
      task.IsAssignedTo = updated.UserId == _parent.CurrentUser.Id;
      RefreshViews();
    }
    catch (Exception ex) { ShowError(ex.Message); }
    finally { IsLoading = false; }
  }

  // ─── Task Delete ──────────────────────────────────────────────────────────
  [RelayCommand]
  private async Task TaskDeleteAsync(object? parameter)
  {
    var task = parameter as TaskModel ?? SelectedTask;
    if (task == null) return;

    var confirm = MessageBox.Show(
      $"Delete task \"{task.Name}\"?\nThis cannot be undone.",
      "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
    if (confirm != MessageBoxResult.Yes) return;

    IsLoading = true;
    try
    {
      await _supabaseService.DeleteTaskAsync(task.Id);
      _parent?.Tasks.Items.Remove(task);
      RefreshViews();
    }
    catch (Exception ex) { ShowError(ex.Message); }
    finally { IsLoading = false; }
  }

  // ─── Task Complete ────────────────────────────────────────────────────────
  [RelayCommand]
  private async Task TaskCompleteAsync(object? parameter)
  {
    var task = parameter as TaskModel ?? SelectedTask;
    if (task == null || _parent?.CurrentUser == null) return;

    IsLoading = true;
    try
    {
      var entity = ToEntity(task);
      entity.Status = 0; // Complete
      entity.UpdateBy = _parent.CurrentUser.Email;
      var result = await _supabaseService.UpdateTaskAsync(entity);
      if (!result.Success) { ShowError(result.Error); return; }

      task.Status = 0;
      RefreshViews();

      // Notify assigner
      var assigner = _parent.Users.Items.FirstOrDefault(u => u.Email == task.User?.Email);
      var creator = _parent.Users.Items.FirstOrDefault(u => u.Email == _parent.CurrentUser.Email);
      if (creator != null && task.User?.Email != null)
      {
        _ = _mailService.SendTaskCompleteMailTypedAsync(
          creator.Email ?? _parent.CurrentUser.Email,
          task.Name, task.User.Name, string.Empty,
          task.DateEnd, creator.Name);
      }

      _toastService.ShowTaskComplete(task.Id, task.Name);
    }
    catch (Exception ex) { ShowError(ex.Message); }
    finally { IsLoading = false; }
  }

  // ─── Task Start ───────────────────────────────────────────────────────────
  [RelayCommand]
  private async Task TaskStartAsync(object? parameter)
  {
    var task = parameter as TaskModel ?? SelectedTask;
    if (task == null || _parent?.CurrentUser == null) return;

    IsLoading = true;
    try
    {
      var entity = ToEntity(task);
      entity.Status = 1; // In Progress
      entity.UpdateBy = _parent.CurrentUser.Email;
      var result = await _supabaseService.UpdateTaskAsync(entity);
      if (!result.Success) { ShowError(result.Error); return; }
      task.Status = 1;
      RefreshViews();
    }
    catch (Exception ex) { ShowError(ex.Message); }
    finally { IsLoading = false; }
  }

  // ─── Task Checked ─────────────────────────────────────────────────────────
  [RelayCommand]
  private async Task TaskCheckedAsync(object? parameter)
  {
    var task = parameter as TaskModel ?? SelectedTask;
    if (task == null || _parent?.CurrentUser == null) return;

    IsLoading = true;
    try
    {
      var entity = ToEntity(task);
      entity.Status = 2; // Checked
      entity.UpdateBy = _parent.CurrentUser.Email;
      var result = await _supabaseService.UpdateTaskAsync(entity);
      if (!result.Success) { ShowError(result.Error); return; }
      task.Status = 2;
      RefreshViews();
      _toastService.ShowTaskComplete(task.Id, $"Checked: {task.Name}");
    }
    catch (Exception ex) { ShowError(ex.Message); }
    finally { IsLoading = false; }
  }

  // ─── Task Re-Check ────────────────────────────────────────────────────────
  [RelayCommand]
  private async Task TaskReCheckedAsync(object? parameter)
  {
    var task = parameter as TaskModel ?? SelectedTask;
    if (task == null || _parent?.CurrentUser == null) return;

    IsLoading = true;
    try
    {
      var entity = ToEntity(task);
      entity.Status = 1; // Revert to In Progress
      entity.UpdateBy = _parent.CurrentUser.Email;
      var result = await _supabaseService.UpdateTaskAsync(entity);
      if (!result.Success) { ShowError(result.Error); return; }
      task.Status = 1;
      RefreshViews();
    }
    catch (Exception ex) { ShowError(ex.Message); }
    finally { IsLoading = false; }
  }

  // ─── Task Accept ──────────────────────────────────────────────────────────
  [RelayCommand]
  private async Task TaskAcceptAsync(object? parameter)
  {
    var task = parameter as TaskModel ?? SelectedTask;
    if (task == null || _parent?.CurrentUser == null) return;

    IsLoading = true;
    try
    {
      var entity = ToEntity(task);
      entity.Status = 3; // Accepted/Assigned
      entity.UpdateBy = _parent.CurrentUser.Email;
      var result = await _supabaseService.UpdateTaskAsync(entity);
      if (!result.Success) { ShowError(result.Error); return; }
      task.Status = 3;
      task.StateAccepted = true;
      RefreshViews();
    }
    catch (Exception ex) { ShowError(ex.Message); }
    finally { IsLoading = false; }
  }

  // ─── Files Dropped ────────────────────────────────────────────────────────
  [RelayCommand]
  private void FilesDropped(object? parameter)
  {
    if (parameter is not System.Windows.DragEventArgs e) return;
    if (!e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop)) return;
    var files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
    foreach (var file in files)
      Debug.WriteLine($"[Dashboard] File dropped: {file}");
  }

  // ─── Helpers ──────────────────────────────────────────────────────────────
  private static TaskEntity ToEntity(TaskModel task) => new()
  {
    Id = task.Id,
    Index = task.Index,
    IndexStatus = task.IndexStatus,
    Name = task.Name,
    Status = task.Status,
    Approval = task.Approval,
    ProjectId = task.ProjectId ?? string.Empty,
    UserId = task.UserId ?? string.Empty,
    DateStart = task.DateStart,
    DateEnd = task.DateEnd,
    Folder = task.Folder
  };

  private static TaskEntity BuildEntityFromDialog(
    Views.Dialogs.TaskEditDialog dlg, string createBy, string? id = null) => new()
  {
    Id = id ?? Guid.NewGuid().ToString(),
    Name = dlg.TaskName,
    Status = 3, // Assigned
    Approval = 0,
    ProjectId = dlg.SelectedProjectId ?? string.Empty,
    UserId = dlg.SelectedUserId ?? string.Empty,
    DateStart = dlg.DateStart,
    DateEnd = dlg.DateEnd,
    CreateBy = createBy
  };

  private void ShowError(string? msg)
  {
    if (_parent == null) return;
    _parent.DialogMessage = new MessageModel
    {
      Show = true, Title = "Error", Message = msg ?? "Unknown error",
      Icon = MessageModel.Icons[1]
    };
  }
}
