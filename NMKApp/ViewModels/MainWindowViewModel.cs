using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMKApp.Data.Entities;
using NMKApp.Models;
using NMKApp.Services;
using System.Diagnostics;
using System.Windows;

namespace NMKApp.ViewModels;

/// <summary>
/// Main window ViewModel — holds AppModel (NmkM) + all commands.
/// Mirrors NMK_M pattern from the sample project.
/// All collection views are created in AppModel.Refresh() before data loads.
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
  private readonly IAuthService _authService;
  private readonly ISupabaseService _supabaseService;
  private readonly IRealtimeService _realtimeService;
  private readonly IBackupService _backupService;
  private readonly IMailService _mailService;
  private readonly IToastService _toastService;

  /// <summary>
  /// Central application model. Collection views exist immediately (before data loads).
  /// Pages bind to NmkM.XxxCollection, NmkM.CurrentUser, NmkM.RoleVisible, etc.
  /// </summary>
  [ObservableProperty] private AppModel _nmkM = new();

  public MainWindowViewModel(
    IAuthService authService,
    ISupabaseService supabaseService,
    IRealtimeService realtimeService,
    IBackupService backupService,
    IMailService mailService,
    IToastService toastService)
  {
    _authService = authService;
    _supabaseService = supabaseService;
    _realtimeService = realtimeService;
    _backupService = backupService;
    _mailService = mailService;
    _toastService = toastService;
  }

  // ─── Load / Refresh ──────────────────────────────────────────────

  [RelayCommand]
  private async Task LoadAsync()
  {
    try
    {
      var auth = await _authService.AuthenticateAsync();
      if (!auth.IsLoggedIn)
      {
        var dlg = new Views.Dialogs.LoginEmailDialog();
        if (dlg.ShowDialog() == true && !string.IsNullOrWhiteSpace(dlg.Email))
          auth = (dlg.Email, string.Empty, true);
        else
        {
          MessageBox.Show(
            "Cannot sign in. Please ensure you are signed in to a Microsoft/Outlook account on this machine.",
            "Sign-in required", MessageBoxButton.OK, MessageBoxImage.Warning);
          Application.Current.Shutdown();
          return;
        }
      }

      await _supabaseService.InitializeAsync();
      await LoadDataAsync(auth.Email, auth.Avatar);
      NmkM.ReloadTask();
      await SubscribeRealtimeAsync();
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"[Load] FATAL: {ex}");
      MessageBox.Show($"Error loading app:\n\n{ex.Message}\n\nCheck your Supabase connection and credentials.",
        "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
  }

  private async Task LoadDataAsync(string email, string avatar)
  {
    // Load users
    var usersResult = await _supabaseService.GetUsersAsync();
    if (usersResult.Success)
    {
      foreach (var user in usersResult.Data!)
        NmkM.Users.Items.Add(user);

      NmkM.CurrentUser = NmkM.Users.Items.FirstOrDefault(u =>
        u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

      if (NmkM.CurrentUser != null)
        NmkM.UpdateRoleVisibility(NmkM.CurrentUser.RoleEnum);
    }

    // Load projects
    var projectsResult = await _supabaseService.GetProjectsAsync();
    if (projectsResult.Success)
      foreach (var project in projectsResult.Data!)
        NmkM.Projects.Items.Add(project);

    // Load tasks
    var tasksResult = await _supabaseService.GetTasksAsync();
    if (tasksResult.Success)
      foreach (var task in tasksResult.Data!)
      {
        task.Project = NmkM.Projects.Items.FirstOrDefault(p => p.Id == task.ProjectId);
        task.User = NmkM.Users.Items.FirstOrDefault(u => u.Id == task.UserId);
        task.IsAssignedTo = task.UserId == NmkM.CurrentUser?.Id;
        NmkM.Tasks.Items.Add(task);
      }

    // Load notifications
    if (NmkM.CurrentUser != null)
    {
      var notifysResult = await _supabaseService.GetNotifysAsync(NmkM.CurrentUser.Email);
      if (notifysResult.Success)
        foreach (var notify in notifysResult.Data!)
          NmkM.Notifys.Items.Add(notify);
    }

    // Load versions
    var versionsResult = await _supabaseService.GetVersionsAsync();
    if (versionsResult.Success && versionsResult.Data!.Count > 0)
    {
      NmkM.VersionLast = versionsResult.Data.OrderByDescending(v => v.CreateAt).First();
      NmkM.VersionCurrent = NmkM.VersionLast;
    }

    // Load leaves
    var leavesResult = await _supabaseService.GetLeavesAsync();
    if (leavesResult.Success)
    {
      foreach (var leave in leavesResult.Data!)
      {
        leave.User = NmkM.Users.Items.FirstOrDefault(u => u.Email.Equals(leave.CreateBy, StringComparison.OrdinalIgnoreCase));
        leave.UserCreateBy = leave.User;
        leave.UserCC = NmkM.Users.Items.FirstOrDefault(u => u.Email.Equals(leave.CC, StringComparison.OrdinalIgnoreCase));

        if (NmkM.CurrentUser != null && leave.CreateBy.Equals(NmkM.CurrentUser.Email, StringComparison.OrdinalIgnoreCase))
          NmkM.Leaves.Items.Add(leave);
        if (NmkM.CurrentUser != null && leave.SendTo.Equals(NmkM.CurrentUser.Email, StringComparison.OrdinalIgnoreCase))
          NmkM.LeaveAssignTo.Items.Add(leave);
      }
    }
  }

  private async Task SubscribeRealtimeAsync()
  {
    try
    {
      await _realtimeService.SubscribeAsync(
        onTaskUpdated: task => Application.Current.Dispatcher.BeginInvoke(() => OnTaskUpdated(task)),
        onNotifyInserted: notify => Application.Current.Dispatcher.BeginInvoke(() => OnNotifyInserted(notify)),
        onNotifyUpdated: notify => Application.Current.Dispatcher.BeginInvoke(() => OnNotifyUpdated(notify)),
        onLeaveUpdated: leave => Application.Current.Dispatcher.BeginInvoke(() => OnLeaveUpdated(leave)),
        onVersionInserted: version => Application.Current.Dispatcher.BeginInvoke(() => OnVersionInserted(version))
      );
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"[Realtime] Subscribe failed: {ex.Message}");
    }
  }

  [RelayCommand]
  private async Task RefreshAsync()
  {
    try
    {
      NmkM.Users.Items.Clear();
      NmkM.Projects.Items.Clear();
      NmkM.Tasks.Items.Clear();
      NmkM.Notifys.Items.Clear();
      NmkM.Leaves.Items.Clear();
      NmkM.LeaveAssignTo.Items.Clear();

      var auth = await _authService.AuthenticateAsync();
      await LoadDataAsync(auth.Email, auth.Avatar);
      NmkM.ReloadTask();
    }
    catch (Exception ex)
    {
      NmkM.DialogMessage = new MessageModel
      {
        Show = true, Title = "Error", Message = ex.Message, Icon = MessageModel.Icons[1]
      };
    }
  }

  public async Task BackupDataAsync()
  {
    if (NmkM.CurrentUser == null) return;
    await _backupService.BackupAsync(NmkM.CurrentUser, NmkM.Tasks, NmkM.TasksTemporary,
      NmkM.Users, NmkM.Projects, NmkM.VersionCurrent, NmkM.VersionLast, NmkM.IsVersionUpdate);
  }

  // ─── Realtime Handlers ───────────────────────────────────────────
  private void OnTaskUpdated(TaskModel task)
  {
    NmkM.ReloadTask();
  }

  private void OnNotifyInserted(NotifyModel notify)
  {
    if (NmkM.CurrentUser == null || notify.SendTo != NmkM.CurrentUser.Email) return;
    NmkM.Notifys.Items.Add(notify);
    NmkM.ReloadTask();
  }

  private void OnNotifyUpdated(NotifyModel notify)
  {
    if (NmkM.CurrentUser == null || notify.SendTo != NmkM.CurrentUser.Email) return;
    var existing = NmkM.Notifys.Items.FirstOrDefault(n => n.Id == notify.Id);
    if (existing != null)
    {
      existing.IsRead = notify.IsRead;
      existing.UpdateAt = notify.UpdateAt;
    }
    NmkM.ReloadTask();
  }

  private void OnLeaveUpdated(LeaveModel leave)
  {
    NmkM.ReloadTask();
  }

  private void OnVersionInserted(VersionModel version)
  {
    NmkM.DialogMessage = new MessageModel
    {
      Show = true,
      Title = "New Version Available",
      Message = $"Version {version.Version} is available. Please update.",
      Icon = MessageModel.Icons[0],
      SupportButtonTitle = "Later",
      MainButtonTitle = "Update"
    };
  }

  // ─── Helper ──────────────────────────────────────────────────────
  private void ShowError(string? msg)
  {
    NmkM.DialogMessage = new MessageModel
    {
      Show = true, Title = "Error", Message = msg ?? "Unknown error", Icon = MessageModel.Icons[1]
    };
  }

  // ─── Task Commands ───────────────────────────────────────────────
  [RelayCommand]
  private async Task TaskNewAsync()
  {
    if (NmkM.CurrentUser == null) return;
    var dlg = new Views.Dialogs.TaskEditDialog(NmkM.Users.Items, NmkM.Projects.Items);
    if (dlg.ShowDialog() != true) return;

    NmkM.IsLoading = true;
    try
    {
      var entity = BuildTaskEntityFromDialog(dlg, createBy: NmkM.CurrentUser.Email);
      var result = await _supabaseService.InsertTaskAsync(entity);
      if (!result.Success) { ShowError(result.Error); return; }

      var task = result.Data!;
      task.Project = NmkM.Projects.Items.FirstOrDefault(p => p.Id == task.ProjectId);
      task.User = NmkM.Users.Items.FirstOrDefault(u => u.Id == task.UserId);
      task.IsAssignedTo = task.UserId == NmkM.CurrentUser.Id;
      NmkM.Tasks.Items.Add(task);
      NmkM.ReloadTask();

      if (task.User?.Email != null)
      {
        _ = _mailService.SendTaskMailTypedAsync(
          task.User.Email, task.Name, task.User.Name,
          dlg.Description, task.DateStart, task.DateEnd,
          NmkM.CurrentUser.Name);
      }
    }
    catch (Exception ex) { ShowError(ex.Message); }
    finally { NmkM.IsLoading = false; }
  }

  [RelayCommand]
  private async Task TaskEditAsync(object? parameter)
  {
    var task = parameter as TaskModel ?? NmkM.SelectedTask;
    if (task == null || NmkM.CurrentUser == null) return;

    var dlg = new Views.Dialogs.TaskEditDialog(NmkM.Users.Items, NmkM.Projects.Items, task);
    if (dlg.ShowDialog() != true) return;

    NmkM.IsLoading = true;
    try
    {
      var entity = BuildTaskEntityFromDialog(dlg, createBy: task.CreateAt.ToString(), id: task.Id);
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
      task.Project = NmkM.Projects.Items.FirstOrDefault(p => p.Id == updated.ProjectId);
      task.User = NmkM.Users.Items.FirstOrDefault(u => u.Id == updated.UserId);
      task.IsAssignedTo = updated.UserId == NmkM.CurrentUser.Id;
      NmkM.ReloadTask();
    }
    catch (Exception ex) { ShowError(ex.Message); }
    finally { NmkM.IsLoading = false; }
  }

  [RelayCommand]
  private async Task TaskDeleteAsync(object? parameter)
  {
    var task = parameter as TaskModel ?? NmkM.SelectedTask;
    if (task == null) return;

    var confirm = MessageBox.Show(
      $"Delete task \"{task.Name}\"?\nThis cannot be undone.",
      "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
    if (confirm != MessageBoxResult.Yes) return;

    NmkM.IsLoading = true;
    try
    {
      await _supabaseService.DeleteTaskAsync(task.Id);
      NmkM.Tasks.Items.Remove(task);
      NmkM.ReloadTask();
    }
    catch (Exception ex) { ShowError(ex.Message); }
    finally { NmkM.IsLoading = false; }
  }

  [RelayCommand]
  private async Task TaskCompleteAsync(object? parameter)
  {
    var task = parameter as TaskModel ?? NmkM.SelectedTask;
    if (task == null || NmkM.CurrentUser == null) return;

    NmkM.IsLoading = true;
    try
    {
      var entity = ToTaskEntity(task);
      entity.Status = 0;
      entity.UpdateBy = NmkM.CurrentUser.Email;
      var result = await _supabaseService.UpdateTaskAsync(entity);
      if (!result.Success) { ShowError(result.Error); return; }

      task.Status = 0;
      NmkM.ReloadTask();
      _toastService.ShowTaskComplete(task.Id, task.Name);

      if (task.User?.Email != null)
      {
        _ = _mailService.SendTaskCompleteMailTypedAsync(
          task.User.Email, task.Name, task.User.Name,
          string.Empty, task.DateEnd, NmkM.CurrentUser.Name);
      }
    }
    catch (Exception ex) { ShowError(ex.Message); }
    finally { NmkM.IsLoading = false; }
  }

  [RelayCommand]
  private async Task TaskStartAsync(object? parameter)
  {
    var task = parameter as TaskModel ?? NmkM.SelectedTask;
    if (task == null || NmkM.CurrentUser == null) return;

    NmkM.IsLoading = true;
    try
    {
      var entity = ToTaskEntity(task);
      entity.Status = 1;
      entity.UpdateBy = NmkM.CurrentUser.Email;
      var result = await _supabaseService.UpdateTaskAsync(entity);
      if (!result.Success) { ShowError(result.Error); return; }
      task.Status = 1;
      NmkM.ReloadTask();
    }
    catch (Exception ex) { ShowError(ex.Message); }
    finally { NmkM.IsLoading = false; }
  }

  [RelayCommand]
  private async Task TaskCheckedAsync(object? parameter)
  {
    var task = parameter as TaskModel ?? NmkM.SelectedTask;
    if (task == null || NmkM.CurrentUser == null) return;

    NmkM.IsLoading = true;
    try
    {
      var entity = ToTaskEntity(task);
      entity.Status = 2;
      entity.UpdateBy = NmkM.CurrentUser.Email;
      var result = await _supabaseService.UpdateTaskAsync(entity);
      if (!result.Success) { ShowError(result.Error); return; }
      task.Status = 2;
      NmkM.ReloadTask();
      _toastService.ShowTaskComplete(task.Id, $"Checked: {task.Name}");
    }
    catch (Exception ex) { ShowError(ex.Message); }
    finally { NmkM.IsLoading = false; }
  }

  [RelayCommand]
  private async Task TaskReCheckedAsync(object? parameter)
  {
    var task = parameter as TaskModel ?? NmkM.SelectedTask;
    if (task == null || NmkM.CurrentUser == null) return;

    NmkM.IsLoading = true;
    try
    {
      var entity = ToTaskEntity(task);
      entity.Status = 1;
      entity.UpdateBy = NmkM.CurrentUser.Email;
      var result = await _supabaseService.UpdateTaskAsync(entity);
      if (!result.Success) { ShowError(result.Error); return; }
      task.Status = 1;
      NmkM.ReloadTask();
    }
    catch (Exception ex) { ShowError(ex.Message); }
    finally { NmkM.IsLoading = false; }
  }

  [RelayCommand]
  private async Task TaskAcceptAsync(object? parameter)
  {
    var task = parameter as TaskModel ?? NmkM.SelectedTask;
    if (task == null || NmkM.CurrentUser == null) return;

    NmkM.IsLoading = true;
    try
    {
      var entity = ToTaskEntity(task);
      entity.Status = 3;
      entity.UpdateBy = NmkM.CurrentUser.Email;
      var result = await _supabaseService.UpdateTaskAsync(entity);
      if (!result.Success) { ShowError(result.Error); return; }
      task.Status = 3;
      task.StateAccepted = true;
      NmkM.ReloadTask();
    }
    catch (Exception ex) { ShowError(ex.Message); }
    finally { NmkM.IsLoading = false; }
  }

  [RelayCommand]
  private void FilesDropped(object? parameter)
  {
    if (parameter is not System.Windows.DragEventArgs e) return;
    if (!e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop)) return;
    var files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
    foreach (var file in files)
      Debug.WriteLine($"[Dashboard] File dropped: {file}");
  }

  // ─── User Commands ───────────────────────────────────────────────
  [RelayCommand]
  private async Task UserAddAsync()
  {
    if (NmkM.CurrentUser == null) return;
    var dlg = new Views.Dialogs.UserEditDialog();
    if (dlg.ShowDialog() != true) return;
    try
    {
      var entity = new UserEntity
      {
        Id = Guid.NewGuid().ToString(),
        Name = dlg.UserName, Email = dlg.Email,
        Role = dlg.Role, Team = dlg.Team,
        CreateBy = NmkM.CurrentUser.Email
      };
      var result = await _supabaseService.InsertUserAsync(entity);
      if (result.Success) NmkM.Users.Items.Add(result.Data!);
      NmkM.ReloadTask();
    }
    catch (Exception ex) { Debug.WriteLine($"[User] Add error: {ex.Message}"); }
  }

  [RelayCommand]
  private async Task UserEditAsync(object? parameter)
  {
    var user = parameter as UserModel ?? NmkM.SelectedUser;
    if (user == null || NmkM.CurrentUser == null) return;
    var dlg = new Views.Dialogs.UserEditDialog(user);
    if (dlg.ShowDialog() != true) return;
    try
    {
      var entity = new UserEntity
      {
        Id = user.Id, Name = dlg.UserName, Email = dlg.Email,
        Role = dlg.Role, Team = dlg.Team,
        CreateBy = user.CreateBy, UpdateBy = NmkM.CurrentUser.Email
      };
      var result = await _supabaseService.UpdateUserAsync(entity);
      if (result.Success)
      {
        user.Name = dlg.UserName; user.Email = dlg.Email;
        user.Team = dlg.Team; user.Role = dlg.Role;
      }
      NmkM.ReloadTask();
    }
    catch (Exception ex) { Debug.WriteLine($"[User] Edit error: {ex.Message}"); }
  }

  [RelayCommand]
  private async Task UserDeleteAsync(object? parameter)
  {
    var user = parameter as UserModel ?? NmkM.SelectedUser;
    if (user == null) return;
    var confirm = MessageBox.Show($"Delete user {user.Name}?", "Confirm",
      MessageBoxButton.YesNo, MessageBoxImage.Warning);
    if (confirm != MessageBoxResult.Yes) return;
    await _supabaseService.DeleteUserAsync(user.Id);
    NmkM.Users.Items.Remove(user);
    NmkM.ReloadTask();
  }

  [RelayCommand]
  private async Task UserImageAsync(object? parameter)
  {
    var user = parameter as UserModel ?? NmkM.SelectedUser;
    if (user == null) return;
    var ofd = new Microsoft.Win32.OpenFileDialog
    {
      Filter = "Image files|*.png;*.jpg;*.jpeg;*.bmp;*.gif",
      Title = "Select profile image"
    };
    if (ofd.ShowDialog() != true) return;
    var bytes = await System.IO.File.ReadAllBytesAsync(ofd.FileName);
    user.ImageString = Convert.ToBase64String(bytes);
    var entity = new UserEntity
    {
      Id = user.Id, Name = user.Name, Email = user.Email,
      Role = user.Role, Team = user.Team, CreateBy = user.CreateBy,
      ImageString = user.ImageString
    };
    await _supabaseService.UpdateUserAsync(entity);
    NmkM.ReloadTask();
  }

  // ─── Project Commands ────────────────────────────────────────────
  [RelayCommand]
  private async Task ProjectAddAsync() { await Task.CompletedTask; /* TODO */ }

  [RelayCommand]
  private async Task ProjectEditAsync(object? parameter) { await Task.CompletedTask; /* TODO */ }

  [RelayCommand]
  private async Task ProjectDeleteAsync(object? parameter) { await Task.CompletedTask; /* TODO */ }

  [RelayCommand]
  private void ProjectColor(object? parameter) { /* TODO */ }

  [RelayCommand]
  private void ProjectImage(object? parameter) { /* TODO */ }

  // ─── Email Commands ──────────────────────────────────────────────
  [RelayCommand]
  private void EmailSelectAll(object? parameter)
  {
    bool selectAll = parameter?.ToString() == "True";
    foreach (var task in NmkM.Tasks.Items)
      if (task.Status == 3 && !task.IsAssignedTo)
        task.IsChecked = selectAll;
  }

  [RelayCommand]
  private async Task EmailSendAsync()
  {
    if (NmkM.CurrentUser == null) return;
    var selected = NmkM.Tasks.Items
      .Where(t => t.IsChecked && t.Status == 3 && !t.IsAssignedTo)
      .ToList();
    if (selected.Count == 0) return;

    foreach (var task in selected)
    {
      if (task.User?.Email == null) continue;
      await _mailService.SendTaskMailTypedAsync(
        task.User.Email, task.Name, task.User.Name,
        string.Empty, task.DateStart, task.DateEnd,
        NmkM.CurrentUser.Name);
      task.IsChecked = false;
    }
    NmkM.ReloadTask();
  }

  // ─── Temporary Commands ──────────────────────────────────────────
  [RelayCommand]
  private async Task TemporaryAddTaskAsync() { await Task.CompletedTask; /* TODO */ }

  [RelayCommand]
  private void TemporarySelectAll(object? parameter) { /* TODO */ }

  [RelayCommand]
  private async Task TemporaryDeleteAsync(object? parameter) { await Task.CompletedTask; /* TODO */ }

  [RelayCommand]
  private async Task TemporarySaveAsync() { await Task.CompletedTask; /* TODO */ }

  // ─── Notify Commands ─────────────────────────────────────────────
  [RelayCommand]
  private async Task NotifyReadAsync(object? parameter)
  {
    var notify = parameter as NotifyModel;
    if (notify == null || notify.IsRead) return;
    try
    {
      var entity = new NotifyEntity
      {
        Id = notify.Id, TaskId = notify.TaskId, Title = notify.Title,
        SendTo = notify.SendTo, CreateBy = notify.CreateBy,
        IsRead = true, Type = notify.Type, Status = notify.Status
      };
      await _supabaseService.UpdateNotifyAsync(entity);
      notify.IsRead = true;
      NmkM.ReloadTask();
    }
    catch (Exception ex) { Debug.WriteLine($"[Notify] Read error: {ex.Message}"); }
  }

  [RelayCommand]
  private async Task NotifyReadAllAsync()
  {
    var unread = NmkM.Notifys.Items.Where(n => !n.IsRead).ToList();
    foreach (var n in unread)
    {
      n.IsRead = true;
      var entity = new NotifyEntity
      {
        Id = n.Id, TaskId = n.TaskId, Title = n.Title,
        SendTo = n.SendTo, CreateBy = n.CreateBy,
        IsRead = true, Type = n.Type, Status = n.Status
      };
      await _supabaseService.UpdateNotifyAsync(entity);
    }
    NmkM.ReloadTask();
  }

  // ─── Leave Commands ──────────────────────────────────────────────
  [RelayCommand]
  private async Task LeaveApplyAsync()
  {
    if (NmkM.CurrentUser == null) return;
    var dlg = new Views.Dialogs.LeaveApplyDialog(NmkM.Users.Items);
    if (dlg.ShowDialog() != true) return;
    NmkM.IsLoading = true;
    try
    {
      var entity = new LeaveEntity
      {
        Id = Guid.NewGuid().ToString(),
        CreateBy = NmkM.CurrentUser.Email,
        SendTo = dlg.SendTo, CC = dlg.CC,
        Type = dlg.LeaveType, Reason = dlg.Reason, Approval = 2
      };
      var result = await _supabaseService.InsertLeaveAsync(entity);
      if (!result.Success) return;
      var leave = result.Data!;
      leave.User = NmkM.Users.Items.FirstOrDefault(u => u.Email == entity.CreateBy);
      leave.UserCreateBy = leave.User;
      NmkM.Leaves.Items.Add(leave);
      NmkM.ReloadTask();

      _ = _mailService.SendLeaveMailTypedAsync(
        dlg.SendTo, leave.User?.Name ?? string.Empty,
        dlg.LeaveType, dlg.Reason,
        dlg.LeaveDays, NmkM.CurrentUser.Name, dlg.CC);
    }
    catch (Exception ex) { Debug.WriteLine($"[Leave] Apply error: {ex.Message}"); }
    finally { NmkM.IsLoading = false; }
  }

  [RelayCommand]
  private async Task LeaveApproveAsync(object? parameter)
    => await SetLeaveApprovalAsync(parameter as LeaveModel ?? NmkM.SelectedLeave, approved: true);

  [RelayCommand]
  private async Task LeaveRejectAsync(object? parameter)
    => await SetLeaveApprovalAsync(parameter as LeaveModel ?? NmkM.SelectedLeave, approved: false);

  private async Task SetLeaveApprovalAsync(LeaveModel? leave, bool approved)
  {
    if (leave == null || NmkM.CurrentUser == null) return;
    NmkM.IsLoading = true;
    try
    {
      var entity = new LeaveEntity
      {
        Id = leave.Id, CreateBy = leave.CreateBy, SendTo = leave.SendTo,
        CC = leave.CC, Type = leave.Type, Reason = leave.Reason,
        Approval = approved ? 1 : 0
      };
      await _supabaseService.UpdateLeaveAsync(entity);
      leave.Approval = entity.Approval;
      NmkM.ReloadTask();

      _ = _mailService.ApprovalLeaveMailTypedAsync(
        leave.CreateBy, leave.User?.Name ?? string.Empty,
        leave.Type, leave.Reason, leave.LeaveList,
        NmkM.CurrentUser.Name, approved, leave.CC);
    }
    catch (Exception ex) { Debug.WriteLine($"[Leave] Approval error: {ex.Message}"); }
    finally { NmkM.IsLoading = false; }
  }

  // ─── Settings Commands ───────────────────────────────────────────
  [RelayCommand]
  private async Task FileVersionSelectAsync() { await Task.CompletedTask; /* TODO */ }

  [RelayCommand]
  private async Task FileVersionUploadAsync() { await Task.CompletedTask; /* TODO */ }

  [RelayCommand]
  private async Task FileVersionUpdateAsync() { await Task.CompletedTask; /* TODO */ }

  // ─── Timeline / Schedule Commands ────────────────────────────────
  [RelayCommand]
  private void FilterDay()
  {
    NmkM.UpdateDays(NmkM.MinDay, NmkM.MaxDay);
    NmkM.ReloadTask();
  }

  [RelayCommand]
  private void FilterDaySchedule()
  {
    NmkM.UpdateDaysSchedules(NmkM.MinDaySchedules, NmkM.MaxDaySchedules);
    NmkM.ReloadTask();
  }

  [RelayCommand]
  private void ScheduleStatusChange(object? parameter) { /* TODO */ }

  // ─── Task entity helpers ─────────────────────────────────────────
  private static TaskEntity ToTaskEntity(TaskModel task) => new()
  {
    Id = task.Id, Index = task.Index, IndexStatus = task.IndexStatus,
    Name = task.Name, Status = task.Status, Approval = task.Approval,
    ProjectId = task.ProjectId ?? string.Empty,
    UserId = task.UserId ?? string.Empty,
    DateStart = task.DateStart, DateEnd = task.DateEnd, Folder = task.Folder
  };

  private static TaskEntity BuildTaskEntityFromDialog(
    Views.Dialogs.TaskEditDialog dlg, string createBy, string? id = null) => new()
  {
    Id = id ?? Guid.NewGuid().ToString(),
    Name = dlg.TaskName, Status = 3, Approval = 0,
    ProjectId = dlg.SelectedProjectId ?? string.Empty,
    UserId = dlg.SelectedUserId ?? string.Empty,
    DateStart = dlg.DateStart, DateEnd = dlg.DateEnd,
    CreateBy = createBy
  };
}
