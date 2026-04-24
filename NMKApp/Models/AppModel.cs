using CommunityToolkit.Mvvm.ComponentModel;
using NMKApp.Core;
using NMKApp.Helpers;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace NMKApp.Models;

/// <summary>
/// Central application model — equivalent to NMK_M in the sample project.
/// All ListCollectionViews are created synchronously in Refresh() (called from constructor).
/// Data is filled asynchronously; views auto-update via ObservableCollection.CollectionChanged.
/// </summary>
public partial class AppModel : ObservableObject
{
  // ─── Raw Collections ─────────────────────────────────────────────
  public UserCollection Users { get; } = new();
  public ProjectCollection Projects { get; } = new();
  public TaskCollection Tasks { get; } = new();
  public TaskCollection TasksTemporary { get; } = new();
  public NotifyCollection Notifys { get; } = new();
  public LeaveCollection Leaves { get; } = new();
  public LeaveCollection LeaveAssignTo { get; } = new();

  // ─── State ───────────────────────────────────────────────────────
  [ObservableProperty] private UserModel? _currentUser;
  [ObservableProperty] private RoleVisibleModel _roleVisible = new();
  [ObservableProperty] private VersionModel? _versionCurrent;
  [ObservableProperty] private VersionModel? _versionLast;
  [ObservableProperty] private bool _isVersionUpdate;
  [ObservableProperty] private ImageSource? _taskbarOverlay;
  [ObservableProperty] private bool _isDashboard = true;
  [ObservableProperty] private MessageModel _dialogMessage = new();
  [ObservableProperty] private bool _isLoading;
  [ObservableProperty] private int _unreadCount;

  // ─── Filters ─────────────────────────────────────────────────────
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

  // ─── Search ──────────────────────────────────────────────────────
  [ObservableProperty] private string _searchUser = string.Empty;
  [ObservableProperty] private string _searchProject = string.Empty;
  [ObservableProperty] private string _searchEmail = string.Empty;
  [ObservableProperty] private string _searchTaskTemporary = string.Empty;

  // ─── Selected Items ──────────────────────────────────────────────
  [ObservableProperty] private TaskModel? _selectedTask;
  [ObservableProperty] private UserModel? _selectedUser;
  [ObservableProperty] private LeaveModel? _selectedLeave;

  // ─── Timeline ────────────────────────────────────────────────────
  [ObservableProperty] private DateTime _minDay;
  [ObservableProperty] private DateTime _maxDay;
  public ObservableCollection<DayModel> Days { get; set; } = [];

  // ─── Schedule ────────────────────────────────────────────────────
  [ObservableProperty] private DateTime _minDaySchedules;
  [ObservableProperty] private DateTime _maxDaySchedules;
  public ObservableCollection<DayModel> DaysSchedules { get; set; } = [];

  // ─── Collection Views ────────────────────────────────────────────
  [ObservableProperty] private ListCollectionView _tasksProjectCollection = null!;
  [ObservableProperty] private ListCollectionView _tasksProjectCollectionCount = null!;
  [ObservableProperty] private ListCollectionView _tasksProjectCollectionTimeline = null!;
  [ObservableProperty] private ListCollectionView _tasksUserCollection = null!;
  [ObservableProperty] private ListCollectionView _tasksEmailCollection = null!;
  [ObservableProperty] private ListCollectionView _tasksEmailCollectionCount = null!;
  [ObservableProperty] private ListCollectionView _tasksTemporaryCollection = null!;
  [ObservableProperty] private ListCollectionView _usersCollection = null!;
  [ObservableProperty] private ListCollectionView _usersCollectionRole = null!;
  [ObservableProperty] private ListCollectionView _usersCollectionLeave = null!;
  [ObservableProperty] private ListCollectionView _projectsCollection = null!;
  [ObservableProperty] private ListCollectionView _notifysCollection = null!;
  [ObservableProperty] private ListCollectionView _notifysCollectionCount = null!;
  [ObservableProperty] private ListCollectionView _leavesCollection = null!;
  [ObservableProperty] private ListCollectionView _leavesAssignToCollection = null!;
  [ObservableProperty] private ListCollectionView _leavesAssignToCollectionCount = null!;
  [ObservableProperty] private ListCollectionView _daysWeekCollection = null!;
  [ObservableProperty] private ListCollectionView _daysWeekCollectionSchedules = null!;
  [ObservableProperty] private ListCollectionView _tasksUserCollectionAdminSchedule = null!;

  public AppModel()
  {
    int year = DateTime.Now.Year;
    int month = DateTime.Now.Month;
    MinDay = new DateTime(year, month, 1);
    MaxDay = new DateTime(year, month, DateTime.DaysInMonth(year, month));
    MinDaySchedules = MinDay;
    MaxDaySchedules = MaxDay;

    foreach (var d in DateHelper.CreateDayList(MinDay, MaxDay))
      Days.Add(d);
    foreach (var d in DateHelper.CreateDayList(MinDaySchedules, MaxDaySchedules))
      DaysSchedules.Add(d);

    // Subscribe to filter checkbox changes for auto-refresh
    foreach (var s in FiltersStatus)
      s.PropertyChanged += (_, _) => ReloadTask();

    Refresh();
  }

  // ─── Auto-refresh on filter property changes ─────────────────────
  partial void OnFilterMonthChanged(int value) => ReloadTask();
  partial void OnFilterYearChanged(int value) => ReloadTask();
  partial void OnFilterTodayChanged(bool value) => ReloadTask();
  partial void OnIsAssignedToChanged(bool value) => ReloadTask();
  partial void OnSelectedProjectChanged(ProjectModel? value) => ReloadTask();
  partial void OnCurrentUserChanged(UserModel? value) { UsersCollection?.Refresh(); UsersCollectionLeave?.Refresh(); }
  partial void OnSearchUserChanged(string value) { UsersCollection?.Refresh(); UsersCollectionRole?.Refresh(); }
  partial void OnSearchProjectChanged(string value) => ProjectsCollection?.Refresh();
  partial void OnSearchEmailChanged(string value) => TasksEmailCollection?.Refresh();
  partial void OnSearchTaskTemporaryChanged(string value) => TasksTemporaryCollection?.Refresh();

  /// <summary>
  /// Creates all ListCollectionViews wrapping empty ObservableCollections.
  /// Views auto-update when items are added to the underlying collections.
  /// </summary>
  public void Refresh()
  {
    ProjectsCollection = new ListCollectionView(Projects.Items);
    ProjectsCollection.CustomSort = new SortProjectsByKey();

    UsersCollection = new ListCollectionView(Users.Items);
    UsersCollection.GroupDescriptions.Add(new PropertyGroupDescription("Team"));
    UsersCollection.CustomSort = new SortUsersByTeam();
    UsersCollection.Filter = FilterUser;

    UsersCollectionRole = new ListCollectionView(Users.Items);

    UsersCollectionLeave = new ListCollectionView(Users.Items);
    UsersCollectionLeave.Filter = FilterUserLeave;

    TasksProjectCollection = new ListCollectionView(Tasks.Items);
    TasksProjectCollection.GroupDescriptions.Add(new PropertyGroupDescription("Project"));
    TasksProjectCollection.CustomSort = new SortTasksByProject();
    TasksProjectCollection.Filter = FilterTask;

    TasksProjectCollectionCount = new ListCollectionView(Tasks.Items);
    TasksProjectCollectionCount.Filter = obj =>
    {
      if (obj is not TaskModel task) return false;
      if (task.Status < 3) return false;
      return task.IsAssignedTo == IsAssignedTo;
    };

    TasksProjectCollectionTimeline = new ListCollectionView(Tasks.Items);
    TasksProjectCollectionTimeline.CustomSort = new SortTasksByProjectTimeline();
    TasksProjectCollectionTimeline.Filter = FilterTask;

    TasksUserCollection = new ListCollectionView(Tasks.Items);
    TasksUserCollection.GroupDescriptions.Add(new PropertyGroupDescription("User.Team"));
    TasksUserCollection.GroupDescriptions.Add(new PropertyGroupDescription("User"));
    TasksUserCollection.CustomSort = new SortTasksByUser();
    TasksUserCollection.Filter = FilterTask;

    TasksEmailCollection = new ListCollectionView(Tasks.Items);
    TasksEmailCollection.Filter = obj =>
      obj is TaskModel t && t.Status == 3 && !t.IsAssignedTo;

    TasksEmailCollectionCount = new ListCollectionView(Tasks.Items);
    TasksEmailCollectionCount.Filter = obj =>
      obj is TaskModel t && t.Status == 3 && !t.IsAssignedTo && !t.StateAccepted;

    TasksTemporaryCollection = new ListCollectionView(TasksTemporary.Items);
    TasksTemporaryCollection.CustomSort = new SortTasksByDateStart();
    TasksTemporaryCollection.Filter = FilterTaskTemporary;

    TasksUserCollectionAdminSchedule = new ListCollectionView(Tasks.Items);
    TasksUserCollectionAdminSchedule.GroupDescriptions.Add(new PropertyGroupDescription("User.Team"));
    TasksUserCollectionAdminSchedule.GroupDescriptions.Add(new PropertyGroupDescription("User"));
    TasksUserCollectionAdminSchedule.CustomSort = new SortTasksByUserAdmin();

    NotifysCollection = new ListCollectionView(Notifys.Items);
    NotifysCollection.CustomSort = new SortNotifyByDate();

    NotifysCollectionCount = new ListCollectionView(Notifys.Items);
    NotifysCollectionCount.Filter = obj => obj is NotifyModel n && !n.IsRead;

    LeavesCollection = new ListCollectionView(Leaves.Items);
    LeavesCollection.CustomSort = new SortLeaveCollection();

    LeavesAssignToCollection = new ListCollectionView(LeaveAssignTo.Items);
    LeavesAssignToCollection.CustomSort = new SortLeaveCollection();

    LeavesAssignToCollectionCount = new ListCollectionView(LeaveAssignTo.Items);
    LeavesAssignToCollectionCount.Filter = obj => obj is LeaveModel l && l.Approval == 1;

    DaysWeekCollection = new ListCollectionView(Days);
    DaysWeekCollection.GroupDescriptions.Add(new PropertyGroupDescription("Year"));
    DaysWeekCollection.GroupDescriptions.Add(new PropertyGroupDescription("Month"));
    DaysWeekCollection.GroupDescriptions.Add(new PropertyGroupDescription("Week"));

    DaysWeekCollectionSchedules = new ListCollectionView(DaysSchedules);
    DaysWeekCollectionSchedules.GroupDescriptions.Add(new PropertyGroupDescription("Year"));
    DaysWeekCollectionSchedules.GroupDescriptions.Add(new PropertyGroupDescription("Week"));
  }

  /// <summary>
  /// Refreshes all collection views. Call after any data mutation.
  /// </summary>
  public void ReloadTask()
  {
    TasksProjectCollection?.Refresh();
    TasksProjectCollectionCount?.Refresh();
    TasksProjectCollectionTimeline?.Refresh();
    TasksUserCollection?.Refresh();
    TasksEmailCollection?.Refresh();
    TasksEmailCollectionCount?.Refresh();
    TasksTemporaryCollection?.Refresh();
    UsersCollection?.Refresh();
    UsersCollectionRole?.Refresh();
    UsersCollectionLeave?.Refresh();
    ProjectsCollection?.Refresh();
    NotifysCollection?.Refresh();
    NotifysCollectionCount?.Refresh();
    LeavesCollection?.Refresh();
    LeavesAssignToCollection?.Refresh();
    LeavesAssignToCollectionCount?.Refresh();
    DaysWeekCollection?.Refresh();
    DaysWeekCollectionSchedules?.Refresh();
    UpdateUnreadCount();
  }

  public void UpdateUnreadCount()
  {
    UnreadCount = Notifys.Items.Count(n => !n.IsRead);
    TaskbarOverlay = TaskBarHelper.UpdateTaskbarBadge(UnreadCount);
  }

  public void UpdateDays(DateTime min, DateTime max)
  {
    Days.Clear();
    foreach (var d in DateHelper.CreateDayList(min, max))
      Days.Add(d);
    DaysWeekCollection?.Refresh();
  }

  public void UpdateDaysSchedules(DateTime min, DateTime max)
  {
    DaysSchedules.Clear();
    foreach (var d in DateHelper.CreateDayList(min, max))
      DaysSchedules.Add(d);
    DaysWeekCollectionSchedules?.Refresh();
  }

  public void UpdateRoleVisibility(RoleType role)
  {
    RoleVisible = new RoleVisibleModel
    {
      VisibleUser = Visibility.Visible,
      VisibleAdmin = role is RoleType.Admin or RoleType.AdminApp ? Visibility.Visible : Visibility.Collapsed,
      VisibleLeader = role is RoleType.Leader or RoleType.Admin or RoleType.AdminApp ? Visibility.Visible : Visibility.Collapsed,
      VisibleAdminApp = role == RoleType.AdminApp ? Visibility.Visible : Visibility.Collapsed,
      VisibleMiddle = role is RoleType.Admin or RoleType.AdminApp or RoleType.Leader ? Visibility.Visible : Visibility.Collapsed,
      VisibleOnlyAdminApp = role == RoleType.AdminApp ? Visibility.Visible : Visibility.Collapsed,
    };
  }

  // ─── Filter predicates ───────────────────────────────────────────
  private bool FilterUser(object obj)
  {
    if (obj is not UserModel user) return true;
    if (CurrentUser == null) return true;
    if (CurrentUser.RoleEnum != RoleType.Admin && CurrentUser.RoleEnum != RoleType.AdminApp)
    {
      if (user.Team != CurrentUser.Team) return false;
      if (user.RoleEnum == RoleType.AdminApp) return false;
    }
    return string.IsNullOrEmpty(SearchUser) ||
           user.Name.Contains(SearchUser, StringComparison.OrdinalIgnoreCase) ||
           user.Team.Contains(SearchUser, StringComparison.OrdinalIgnoreCase) ||
           user.Email.Contains(SearchUser, StringComparison.OrdinalIgnoreCase);
  }

  private bool FilterUserLeave(object obj)
  {
    if (obj is not UserModel user || CurrentUser == null) return false;
    if (CurrentUser.Id == user.Id) return false;
    if (user.RoleEnum == RoleType.User) return false;
    if (user.RoleEnum == RoleType.AdminApp) return false;
    return true;
  }

  private bool FilterTask(object obj)
  {
    if (obj is not TaskModel task) return false;

    bool matchTime = true;
    if (FilterYear != 0)
    {
      matchTime = task.DateStart.Year == FilterYear || task.DateEnd.Year == FilterYear;
      if (FilterMonth != 0)
        matchTime &= task.DateStart.Month == FilterMonth || task.DateEnd.Month == FilterMonth;
    }

    bool matchToday = !FilterToday ||
      (task.DateStart.Date <= DateTime.Now.Date && task.DateEnd.Date >= DateTime.Now.Date);

    var checkedStatuses = FiltersStatus.Where(x => x.IsChecked).Select(x => x.State).ToList();
    bool matchStatus = checkedStatuses.Count > 0 && checkedStatuses.Contains(task.Status);

    bool matchProject = SelectedProject == null ||
      string.IsNullOrEmpty(SelectedProject.Id) ||
      SelectedProject.Id == task.ProjectId;

    bool matchAssignedTo = task.IsAssignedTo == IsAssignedTo;

    return matchTime && matchStatus && matchProject && matchAssignedTo && matchToday;
  }

  private bool FilterTaskTemporary(object obj)
  {
    if (obj is not TaskModel task) return false;
    return string.IsNullOrEmpty(SearchTaskTemporary) ||
           task.Name.Contains(SearchTaskTemporary, StringComparison.OrdinalIgnoreCase) ||
           (task.User?.Name.Contains(SearchTaskTemporary, StringComparison.OrdinalIgnoreCase) ?? false);
  }
}
