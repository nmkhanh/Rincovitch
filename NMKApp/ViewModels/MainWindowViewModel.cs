using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMKApp.Models;
using NMKApp.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace NMKApp.ViewModels;

/// <summary>
/// Main window ViewModel — orchestrator role only.
/// Delegates domain logic to child ViewModels.
/// Split from 3000+ line monolith in RincovitchApp.
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
  private readonly IAuthService _authService;
  private readonly ISupabaseService _supabaseService;
  private readonly IRealtimeService _realtimeService;
  private readonly IBackupService _backupService;
  private readonly INavigationService _navigationService;
  private readonly IToastService _toastService;

  // ─── App State ───────────────────────────────────────────────────
  [ObservableProperty] private UserModel? _currentUser;
  [ObservableProperty] private bool _isDashboard = true;
  [ObservableProperty] private RoleVisibleModel _roleVisible = new();
  [ObservableProperty] private MessageModel _dialogMessage = new();
  [ObservableProperty] private ImageSource? _taskbarOverlay;
  [ObservableProperty] private VersionModel? _versionCurrent;
  [ObservableProperty] private VersionModel? _versionLast;
  [ObservableProperty] private bool _isVersionUpdate;

  // ─── Shared Collections (accessible to child ViewModels) ────────
  public UserCollection Users { get; } = new();
  public ProjectCollection Projects { get; } = new();
  public TaskCollection Tasks { get; } = new();
  public TaskCollection TasksTemporary { get; } = new();
  public NotifyCollection Notifys { get; } = new();
  public LeaveCollection Leaves { get; } = new();
  public LeaveCollection LeaveAssignTo { get; } = new();

  // ─── Collection Views ───────────────────────────────────────────
  [ObservableProperty] private ListCollectionView? _usersCollection;
  [ObservableProperty] private ListCollectionView? _usersCollectionRole;
  [ObservableProperty] private ListCollectionView? _projectsCollection;
  [ObservableProperty] private ListCollectionView? _notifysCollection;
  [ObservableProperty] private ListCollectionView? _notifysCollectionCount;
  [ObservableProperty] private ListCollectionView? _leavesCollection;
  [ObservableProperty] private ListCollectionView? _leavesAssignToCollection;
  [ObservableProperty] private ListCollectionView? _leavesAssignToCollectionCount;

  // ─── Child ViewModels ───────────────────────────────────────────
  [ObservableProperty] private DashboardViewModel? _dashboardVM;
  [ObservableProperty] private TimelineViewModel? _timelineVM;
  [ObservableProperty] private EmailViewModel? _emailVM;
  [ObservableProperty] private UserViewModel? _userVM;
  [ObservableProperty] private ProjectViewModel? _projectVM;
  [ObservableProperty] private TemporaryViewModel? _temporaryVM;
  [ObservableProperty] private NotifyViewModel? _notifyVM;
  [ObservableProperty] private LeaveViewModel? _leaveVM;
  [ObservableProperty] private ScheduleViewModel? _scheduleVM;
  [ObservableProperty] private SettingsViewModel? _settingsVM;

  // ─── Filters ────────────────────────────────────────────────────
  [ObservableProperty] private int _filterMonth = DateTime.Now.Month;
  [ObservableProperty] private int _filterYear = DateTime.Now.Year;
  [ObservableProperty] private bool _filterToday;
  [ObservableProperty] private bool _isAssignedTo = true;
  [ObservableProperty] private ProjectModel? _selectedProject;

  public ObservableCollection<StatusModel> FiltersStatus { get; } =
  [
    new() { Name = "Completed", State = 0, IsChecked = false },
    new() { Name = "In Progress", State = 1, IsChecked = false },
    new() { Name = "Checked", State = 2, IsChecked = false },
    new() { Name = "Assigned", State = 3, IsChecked = true },
  ];

  // ─── Search ─────────────────────────────────────────────────────
  [ObservableProperty] private string _searchUser = string.Empty;
  [ObservableProperty] private string _searchProject = string.Empty;

  public MainWindowViewModel(
    IAuthService authService,
    ISupabaseService supabaseService,
    IRealtimeService realtimeService,
    IBackupService backupService,
    INavigationService navigationService,
    IToastService toastService,
    DashboardViewModel dashboardVM,
    TimelineViewModel timelineVM,
    EmailViewModel emailVM,
    UserViewModel userVM,
    ProjectViewModel projectVM,
    TemporaryViewModel temporaryVM,
    NotifyViewModel notifyVM,
    LeaveViewModel leaveVM,
    ScheduleViewModel scheduleVM,
    SettingsViewModel settingsVM)
  {
    _authService = authService;
    _supabaseService = supabaseService;
    _realtimeService = realtimeService;
    _backupService = backupService;
    _navigationService = navigationService;
    _toastService = toastService;

    // Assign child VMs immediately so bindings are not null from the start
    DashboardVM = dashboardVM;
    TimelineVM = timelineVM;
    EmailVM = emailVM;
    UserVM = userVM;
    ProjectVM = projectVM;
    TemporaryVM = temporaryVM;
    NotifyVM = notifyVM;
    LeaveVM = leaveVM;
    ScheduleVM = scheduleVM;
    SettingsVM = settingsVM;
  }

  /// <summary>
  /// Initialize the ViewModel: authenticate, load data, subscribe to realtime.
  /// Called from MainWindow.Loaded.
  /// </summary>
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
      InitializeCollectionViews();
      InitializeChildViewModels();
      await SubscribeRealtimeAsync();
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[Load] FATAL: {ex}");
      MessageBox.Show($"Error loading app:\n\n{ex.Message}\n\nCheck your Supabase connection and credentials.",
        "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
  }

  private void InitializeChildViewModels()
  {
    DashboardVM?.Initialize(this);
    TimelineVM?.Initialize(this);
    EmailVM?.Initialize(this);
    UserVM?.Initialize(this);
    ProjectVM?.Initialize(this);
    TemporaryVM?.Initialize(this);
    NotifyVM?.Initialize(this);
    LeaveVM?.Initialize(this);
    ScheduleVM?.Initialize(this);
    SettingsVM?.Initialize(this);
  }

  private async Task LoadDataAsync(string email, string avatar)
  {
    // Load users
    var usersResult = await _supabaseService.GetUsersAsync();
    if (usersResult.Success)
    {
      foreach (var user in usersResult.Data!)
        Users.Items.Add(user);

      CurrentUser = Users.Items.FirstOrDefault(u =>
        u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

      if (CurrentUser != null)
        UpdateRoleVisibility(CurrentUser.RoleEnum);
    }

    // Load projects
    var projectsResult = await _supabaseService.GetProjectsAsync();
    if (projectsResult.Success)
      foreach (var project in projectsResult.Data!)
        Projects.Items.Add(project);

    // Load tasks
    var tasksResult = await _supabaseService.GetTasksAsync();
    if (tasksResult.Success)
      foreach (var task in tasksResult.Data!)
      {
        task.Project = Projects.Items.FirstOrDefault(p => p.Id == task.ProjectId);
        task.User = Users.Items.FirstOrDefault(u => u.Id == task.UserId);
        task.IsAssignedTo = task.UserId == CurrentUser?.Id;
        Tasks.Items.Add(task);
      }

    // Load notifications
    if (CurrentUser != null)
    {
      var notifysResult = await _supabaseService.GetNotifysAsync(CurrentUser.Email);
      if (notifysResult.Success)
        foreach (var notify in notifysResult.Data!)
          Notifys.Items.Add(notify);
    }

    // Load versions
    var versionsResult = await _supabaseService.GetVersionsAsync();
    if (versionsResult.Success && versionsResult.Data!.Count > 0)
    {
      VersionLast = versionsResult.Data.OrderByDescending(v => v.CreateAt).First();
      VersionCurrent = VersionLast;
    }

    // Load leaves
    var leavesResult = await _supabaseService.GetLeavesAsync();
    if (leavesResult.Success)
    {
      foreach (var leave in leavesResult.Data!)
      {
        leave.User = Users.Items.FirstOrDefault(u => u.Email.Equals(leave.CreateBy, StringComparison.OrdinalIgnoreCase));
        leave.UserCreateBy = leave.User;
        leave.UserCC = Users.Items.FirstOrDefault(u => u.Email.Equals(leave.CC, StringComparison.OrdinalIgnoreCase));

        if (CurrentUser != null && leave.CreateBy.Equals(CurrentUser.Email, StringComparison.OrdinalIgnoreCase))
          Leaves.Items.Add(leave);
        if (CurrentUser != null && leave.SendTo.Equals(CurrentUser.Email, StringComparison.OrdinalIgnoreCase))
          LeaveAssignTo.Items.Add(leave);
      }
    }
  }

  private void InitializeCollectionViews()
  {
    ProjectsCollection = new ListCollectionView(Projects.Items);
    UsersCollection = new ListCollectionView(Users.Items);
    UsersCollectionRole = new ListCollectionView(Users.Items);
    NotifysCollection = new ListCollectionView(Notifys.Items);
    NotifysCollectionCount = new ListCollectionView(Notifys.Items);
    LeavesCollection = new ListCollectionView(Leaves.Items);
    LeavesAssignToCollection = new ListCollectionView(LeaveAssignTo.Items);
    LeavesAssignToCollectionCount = new ListCollectionView(LeaveAssignTo.Items);
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
      // Realtime failure is non-fatal — app works without live updates
      System.Diagnostics.Debug.WriteLine($"[Realtime] Subscribe failed: {ex.Message}");
    }
  }

  [RelayCommand]
  private async Task RefreshAsync()
  {
    try
    {
      Users.Items.Clear();
      Projects.Items.Clear();
      Tasks.Items.Clear();
      Notifys.Items.Clear();
      Leaves.Items.Clear();
      LeaveAssignTo.Items.Clear();

      var auth = await _authService.AuthenticateAsync();
      await LoadDataAsync(auth.Email, auth.Avatar);
      RefreshAllViews();
      InitializeChildViewModels();
    }
    catch (Exception ex)
    {
      DialogMessage = new MessageModel
      {
        Show = true,
        Title = "Error",
        Message = ex.Message,
        Icon = MessageModel.Icons[1]
      };
    }
  }

  public void RefreshAllViews()
  {
    UsersCollection?.Refresh();
    UsersCollectionRole?.Refresh();
    ProjectsCollection?.Refresh();
    NotifysCollection?.Refresh();
    NotifysCollectionCount?.Refresh();
    LeavesCollection?.Refresh();
    LeavesAssignToCollection?.Refresh();
    LeavesAssignToCollectionCount?.Refresh();
    DashboardVM?.RefreshViews();
  }

  public async Task BackupDataAsync()
  {
    if (CurrentUser == null) return;
    await _backupService.BackupAsync(CurrentUser, Tasks, TasksTemporary,
      Users, Projects, VersionCurrent, VersionLast, IsVersionUpdate);
  }

  // ─── Realtime Handlers ──────────────────────────────────────────
  private void OnTaskUpdated(TaskModel task)
  {
    // TODO: Migrate realtime task update logic
    RefreshAllViews();
  }

  private void OnNotifyInserted(NotifyModel notify)
  {
    if (CurrentUser == null || notify.SendTo != CurrentUser.Email) return;
    Notifys.Items.Add(notify);
    RefreshAllViews();
  }

  private void OnNotifyUpdated(NotifyModel notify)
  {
    if (CurrentUser == null || notify.SendTo != CurrentUser.Email) return;
    var existing = Notifys.Items.FirstOrDefault(n => n.Id == notify.Id);
    if (existing != null)
    {
      existing.IsRead = notify.IsRead;
      existing.UpdateAt = notify.UpdateAt;
    }
    RefreshAllViews();
  }

  private void OnLeaveUpdated(LeaveModel leave)
  {
    // TODO: Migrate realtime leave update logic
    RefreshAllViews();
  }

  private void OnVersionInserted(VersionModel version)
  {
    DialogMessage = new MessageModel
    {
      Show = true,
      Title = "New Version Available",
      Message = $"Version {version.Version} is available. Please update.",
      Icon = MessageModel.Icons[0],
      SupportButtonTitle = "Later",
      MainButtonTitle = "Update"
    };
  }

  private void UpdateRoleVisibility(Core.RoleType role)
  {
    RoleVisible = new RoleVisibleModel
    {
      VisibleUser = Visibility.Visible,
      VisibleAdmin = role is Core.RoleType.Admin or Core.RoleType.AdminApp ? Visibility.Visible : Visibility.Collapsed,
      VisibleLeader = role is Core.RoleType.Leader or Core.RoleType.Admin or Core.RoleType.AdminApp ? Visibility.Visible : Visibility.Collapsed,
      VisibleAdminApp = role == Core.RoleType.AdminApp ? Visibility.Visible : Visibility.Collapsed,
      VisibleMiddle = role is Core.RoleType.Admin or Core.RoleType.AdminApp or Core.RoleType.Leader ? Visibility.Visible : Visibility.Collapsed,
      VisibleOnlyAdminApp = role == Core.RoleType.AdminApp ? Visibility.Visible : Visibility.Collapsed,
    };
  }
}
