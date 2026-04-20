using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.SkiaSharpView;
using Newtonsoft.Json.Linq;
using RincovitchApp;
using RincovitchApp.API;
using RincovitchApp.API.Date;
using RincovitchApp.Models.ModelChilds;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using FlowDirection = System.Windows.FlowDirection;
using Point = System.Windows.Point;

namespace RincovitchApp.Models
{
  public partial class NMK_M : ObservableObject
  {
    public NMK_M()
    {
      _ = InitializeHolidaysAsync();
      refresh();
    }

    public void ReloadTask()
    {
      TasksProjectCollection.Refresh();
      TasksProjectCollectionCount.Refresh();
      TasksProjectCollectionTimeline.Refresh();

      TasksUserCollection.Refresh();

      TasksEmailCollection.Refresh();
      TasksEmailCollectionCount.Refresh();

      TasksTemporaryCollection.Refresh();


      UsersCollection.Refresh();

      UsersCollectionRole.Refresh();

      ProjectsCollection.Refresh();

      NotifysCollection.Refresh();

      NotifysCollectionCount.Refresh();

      DaysWeekCollection.Refresh();

      DaysWeekCollectionSchedules.Refresh();

      LeavesCollection.Refresh();
      LeavesAssignToCollection.Refresh();
      LeavesAssignToCollectionCount.Refresh();
      UsersCollectionLeave.Refresh();

      update_day();
      update_day_schedule();
      if (UserCurrent != null && (UserCurrent.RoleEnum == F_Role.RoleType.AdminApp || UserCurrent.RoleEnum == F_Role.RoleType.Admin))
      {
        update_day_schedule_admin();
        TasksUserCollectionAdminSchedule.Refresh();
      }
    }

    public void refresh()
    {
      Notifys = new NMK_M_Notify();

      Projects = new NMK_M_Project();
      Users = new NMK_M_User();
      Tasks = new NMK_M_Task();
      TasksTemporary = new NMK_M_Task();
      TasksAdmin = new NMK_M_Task();

      DaysLeaves = new NMK_M_Day();
      Leaves = new NMK_M_Leave();
      LeaveAssignTo = new NMK_M_Leave();


      Days = new NMK_M_Day() { Items = new System.Collections.ObjectModel.ObservableCollection<NMK_M_Day>(F_Date.CreateDayListByMonth()) };
      MinDay = Days.Items.Min(x => x.Name);
      MaxDay = Days.Items.Max(x => x.Name);

      DaysSchedules = new NMK_M_Day() { Items = new System.Collections.ObjectModel.ObservableCollection<NMK_M_Day>(F_Date.CreateDayListByMonth()) };
      MinDaySchedules = DaysSchedules.Items.Min(x => x.Name);
      MaxDaySchedules = DaysSchedules.Items.Max(x => x.Name);


      ProjectSchedules_AssignTo = new NMK_M_ProjectSchedule();
      UsersSchedules_Admin = new NMK_M_ProjectSchedule();

      ProjectsCollection = new ListCollectionView(Projects.Items);

      UsersCollection = new ListCollectionView(Users.Items);
      UsersCollectionRole = new ListCollectionView(Users.Items);
      UsersCollection.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("Team"));
      UsersCollection.CustomSort = new F_SortUser();
      UsersCollection.Filter = (obj) =>
      {
        if (obj is not NMK_M_User task)
          return false;

        if(UserCurrent.RoleEnum != F_Role.RoleType.Admin && UserCurrent.RoleEnum != F_Role.RoleType.AdminApp)
        {
          if (task.Team != UserCurrent.Team)
            return false;

          if (task.RoleEnum == F_Role.RoleType.AdminApp)
            return false;
        }

        return task.Name.IndexOf(SearchUser, StringComparison.OrdinalIgnoreCase) >= 0 ||
        task.Team.IndexOf(SearchUser, StringComparison.OrdinalIgnoreCase) >= 0 ||
        task.Email.IndexOf(SearchUser, StringComparison.OrdinalIgnoreCase) >= 0 ||
        task.CreateAt.ToString("dd/MM/yyyy").IndexOf(SearchUser, StringComparison.OrdinalIgnoreCase) >= 0 ||
        task.CreateAt.ToString("dddd").IndexOf(SearchUser, StringComparison.OrdinalIgnoreCase) >= 0;
      };

      LeavesCollection = new ListCollectionView(Leaves.Items);

      TasksProjectCollection = new ListCollectionView(Tasks.Items);
      TasksProjectCollection.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("Project"));
      TasksProjectCollection.CustomSort = new F_SortTasksProjectCollection();
      TasksProjectCollection.Filter = (obj) =>
      {
        if (obj is not NMK_M_Task task)
          return false;

        bool matchtime = true;
        var year = FilterYear;
        var month = FilterMonth;
        if (!string.IsNullOrEmpty(year.ToString()) && year != 0)
        {
          matchtime &= task.DateStart.Date.Year == year || task.DateEnd.Date.Year == year;
          if (!string.IsNullOrEmpty(month.ToString()) && month != 0)
          {
            matchtime &= task.DateStart.Date.Month == month || task.DateEnd.Date.Month == month;
          }
        }

        bool matchtoday = true;
        if (FilterToday)
        {
          matchtoday &= task.DateStart.Date <= DateTime.Now.Date && task.DateEnd.Date >= DateTime.Now.Date;
        }

        bool matchstatus = false;
        List<int> check = Filters_Status.Where(x => x.IsChecked).Select(x => x.State).ToList();
        if (check.Count > 0)
          matchstatus = check.Contains(task.Status);

        bool matchproject = true;
        if (Project != null && !string.IsNullOrEmpty(Project.Id))
          matchproject = Project.Id == task.ProjectId;

        bool matchassignedto = task.IsAssignedTo == IsAssignedTo;

        task.IsVisible = matchtime;

        return matchtime && matchstatus && matchproject && matchassignedto && matchtoday;
      };

      TasksProjectCollectionCount = new ListCollectionView(Tasks.Items);
      TasksProjectCollectionCount.Filter = (obj) =>
      {
        if (obj is not NMK_M_Task task)
          return false;

        if (task.Status < 3)
          return false;

        bool matchassignedto = task.IsAssignedTo == IsAssignedTo;

        return matchassignedto;
      };


      TasksProjectCollectionTimeline = new ListCollectionView(Tasks.Items);
      TasksProjectCollectionTimeline.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("Id"));
      TasksProjectCollectionTimeline.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("Name"));
      TasksProjectCollectionTimeline.CustomSort = new F_SortTasksProjectCollectionTimeline();
      TasksProjectCollectionTimeline.Filter = (obj) =>
      {
        if (obj is not NMK_M_Task task)
          return false;

        bool matchtime = true;
        var year = FilterYear;
        var month = FilterMonth;
        if (!string.IsNullOrEmpty(year.ToString()) && year != 0)
        {
          matchtime &= task.DateStart.Date.Year == year || task.DateEnd.Date.Year == year;
          if (!string.IsNullOrEmpty(month.ToString()) && month != 0)
          {
            matchtime &= task.DateStart.Date.Month == month || task.DateEnd.Date.Month == month;
          }
        }

        bool matchtoday = true;
        if (FilterToday)
        {
          matchtoday &= task.DateStart.Date <= DateTime.Now.Date && task.DateEnd.Date >= DateTime.Now.Date;
        }

        bool matchstatus = false;
        List<int> check = Filters_Status.Where(x => x.IsChecked).Select(x => x.State).ToList();
        if (check.Count > 0)
          matchstatus = check.Contains(task.Status);

        bool matchproject = true;
        if (Project != null && !string.IsNullOrEmpty(Project.Id))
          matchproject = Project.Id == task.ProjectId;

        bool matchassignedto = task.IsAssignedTo == IsAssignedTo;

        task.IsVisible = matchtime;

        return matchtime && matchstatus && matchproject && matchassignedto && matchtoday;
      };

      TasksUserCollection = new ListCollectionView(Tasks.Items);
      TasksUserCollection.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("User.Team"));
      TasksUserCollection.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("User"));
      TasksUserCollection.CustomSort = new F_SortTasksUserCollection();
      TasksUserCollection.Filter = (obj) =>
      {
        if (obj is not NMK_M_Task task)
          return false;

        bool matchtime = true;
        var year = FilterYear;
        var month = FilterMonth;
        if (!string.IsNullOrEmpty(year.ToString()) && year != 0)
        {
          matchtime &= task.DateStart.Date.Year == year || task.DateEnd.Date.Year == year;
          if (!string.IsNullOrEmpty(month.ToString()) && month != 0)
          {
            matchtime &= task.DateStart.Date.Month == month || task.DateEnd.Date.Month == month;
          }
        }

        bool matchtoday = true;
        if (FilterToday)
        {
          matchtoday &= task.DateStart.Date <= DateTime.Now.Date && task.DateEnd.Date >= DateTime.Now.Date;
        }

        bool matchstatus = false;
        List<int> check = Filters_Status.Where(x => x.IsChecked).Select(x => x.State).ToList();
        if (check.Count > 0)
          matchstatus = check.Contains(task.Status);

        bool matchproject = true;
        if (Project != null && !string.IsNullOrEmpty(Project.Id))
          matchproject = Project.Id == task.ProjectId;

        bool matchassignedto = task.IsAssignedTo == IsAssignedTo;

        task.IsVisible = matchtime;

        return matchtime && matchstatus && matchproject && matchassignedto && matchtoday;
      };

      TasksUserCollectionAdminSchedule = new ListCollectionView(TasksAdminSchedule.Items);
      TasksUserCollectionAdminSchedule.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("User.Team"));
      TasksUserCollectionAdminSchedule.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("User"));
      TasksUserCollectionAdminSchedule.CustomSort = new F_SortTasksUserCollectionAdmin();
      TasksUserCollectionAdminSchedule.Filter = (obj) =>
      {
        if (obj is not NMK_M_Task task)
          return false;

        //bool matchtime = true;
        //var year = FilterYear;
        //var month = FilterMonth;
        //if (!string.IsNullOrEmpty(year.ToString()) && year != 0)
        //{
        //  matchtime &= task.DateStart.Date.Year == year || task.DateEnd.Date.Year == year;
        //  if (!string.IsNullOrEmpty(month.ToString()) && month != 0)
        //  {
        //    matchtime &= task.DateStart.Date.Month == month || task.DateEnd.Date.Month == month;
        //  }
        //}

        //bool matchtoday = true;
        //if (FilterToday)
        //{
        //  matchtoday &= task.DateStart.Date <= DateTime.Now.Date && task.DateEnd.Date >= DateTime.Now.Date;
        //}

        //bool matchstatus = false;
        //List<int> check = Filters_Status.Where(x => x.IsChecked).Select(x => x.State).ToList();
        //if (check.Count > 0)
        //  matchstatus = check.Contains(task.Status);

        //bool matchproject = true;
        //if (Project != null && !string.IsNullOrEmpty(Project.Id))
        //  matchproject = Project.Id == task.ProjectId;

        //bool matchassignedto = task.IsAssignedTo == IsAssignedTo;

        //task.IsVisible = matchtime;

        return true;
      };

      TasksEmailCollection = new ListCollectionView(Tasks.Items);
      TasksEmailCollection.Filter = (obj) =>
      {
        if (obj is not NMK_M_Task task)
          return false;

        if (task.Status != 1 && task.Status != 2)
          return false;

        if (task.IsAssignedTo)
          return false;

        return task.Name.IndexOf(SearchEmail, StringComparison.OrdinalIgnoreCase) >= 0 ||
        task.User.Name.IndexOf(SearchEmail, StringComparison.OrdinalIgnoreCase) >= 0 ||
        task.User.Email.IndexOf(SearchEmail, StringComparison.OrdinalIgnoreCase) >= 0 ||
        task.DateStart.ToString("dd/MM/yyyy").IndexOf(SearchEmail, StringComparison.OrdinalIgnoreCase) >= 0 ||
        task.DateEnd.ToString("dd/MM/yyyy").IndexOf(SearchEmail, StringComparison.OrdinalIgnoreCase) >= 0 ||
        task.DateStart.ToString("dddd").IndexOf(SearchEmail, StringComparison.OrdinalIgnoreCase) >= 0 ||
        task.DateEnd.ToString("dddd").IndexOf(SearchEmail, StringComparison.OrdinalIgnoreCase) >= 0;
      };

      TasksEmailCollectionCount = new ListCollectionView(Tasks.Items);
      TasksEmailCollectionCount.Filter = (obj) =>
      {
        if (obj is not NMK_M_Task task)
          return false;

        if (task.Status != 1 && task.Status != 2)
          return false;

        if (task.IsAssignedTo)
          return false;

        return true;
      };

      TasksTemporaryCollection = new ListCollectionView(TasksTemporary.Items);
      TasksTemporaryCollection.CustomSort = new F_SortByDateStart();
      TasksTemporaryCollection.Filter = (obj) =>
      {
        if (obj is not NMK_M_Task task)
          return false;

        return task.Name.IndexOf(SearchTaskTemporary, StringComparison.OrdinalIgnoreCase) >= 0 ||
        (task.User != null ? task.User.Name.IndexOf(SearchTaskTemporary, StringComparison.OrdinalIgnoreCase) >= 0 : string.IsNullOrEmpty(SearchTaskTemporary)) ||
        (task.User != null ? task.User.Email.IndexOf(SearchTaskTemporary, StringComparison.OrdinalIgnoreCase) >= 0 : string.IsNullOrEmpty(SearchTaskTemporary)) ||
        task.DateStart.ToString("dd/MM/yyyy").IndexOf(SearchTaskTemporary, StringComparison.OrdinalIgnoreCase) >= 0 ||
        task.DateEnd.ToString("dd/MM/yyyy").IndexOf(SearchTaskTemporary, StringComparison.OrdinalIgnoreCase) >= 0 ||
        task.DateStart.ToString("dddd").IndexOf(SearchTaskTemporary, StringComparison.OrdinalIgnoreCase) >= 0 ||
        task.DateEnd.ToString("dddd").IndexOf(SearchTaskTemporary, StringComparison.OrdinalIgnoreCase) >= 0;
      };


      DaysWeekCollection = new ListCollectionView(Days.Items);
      DaysWeekCollection.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("Year"));
      DaysWeekCollection.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("Month"));
      DaysWeekCollection.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("Week"));
      DaysWeekCollection.Filter = (obj) =>
      {
        if (obj is not NMK_M_Day day)
          return false;

        return day.Name.Date >= MinDay.Date && day.Name.Date <= MaxDay.Date;
      };
      DaysWeekCollectionSchedules = new ListCollectionView(DaysSchedules.Items);
      DaysWeekCollectionSchedules.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("Year"));
      DaysWeekCollectionSchedules.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("Week"));
      DaysWeekCollectionSchedules.Filter = (obj) =>
      {
        if (obj is not NMK_M_Day day)
          return false;

        return day.Name.Date >= MinDaySchedules.Date && day.Name.Date <= MaxDaySchedules.Date;
      };


      NotifysCollection = new ListCollectionView(Notifys.Items);
      NotifysCollection.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("Message"));
      NotifysCollection.CustomSort = new F_SortNotify();
      NotifysCollection.Filter = (obj) =>
      {
        if (obj is not NMK_M_Notify notify)
          return false;

        if (notify.IsRead)
          if (!Notifys_Read)
            return false;

        return true;
      };
      NotifysCollectionCount = new ListCollectionView(Notifys.Items);
      NotifysCollectionCount.Filter = (obj) =>
      {
        if (obj is not NMK_M_Notify notify)
          return false;

        return !notify.IsRead;
      };


      // Schedule
      TasksProjectCollectionSchedule = new ListCollectionView(TasksSchedule.Items);
      TasksProjectCollectionSchedule.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("Project"));
      TasksProjectCollectionSchedule.CustomSort = new F_SortTasksProjectCollectionSchedule();
      TasksProjectCollectionSchedule.Filter = (obj) =>
      {
        if (obj is not NMK_M_Task task)
          return false;

        if (task.Time <= 0)
          return false;

        return task.IsAssignedTo == IsAssignedToSchedule;
      };

      // Leave
      UsersCollectionLeave = new ListCollectionView(Users.Items);
      UsersCollectionLeave.CustomSort = new F_SortUser();
      UsersCollectionLeave.Filter = (obj) =>
      {
        if (obj is not NMK_M_User user)
          return false;

        if (UserCurrent.Id == user.Id)
          return false;

        if (user.RoleEnum == F_Role.RoleType.User)
          return false;

        if (user.RoleEnum == F_Role.RoleType.AdminApp)
          return false;

        if (UserCurrent.RoleEnum == F_Role.RoleType.Admin)
          if (user.RoleEnum != F_Role.RoleType.Admin)
            return false;

        if (user.RoleEnum != F_Role.RoleType.Admin)
        {
          if (user.Team != UserCurrent.Team)
            return false;
        }

        return true;
      };
      LeavesCollection = new ListCollectionView(Leaves.Items);
      LeavesCollection.CustomSort = new F_SortLeaveCollection();

      LeavesAssignToCollection = new ListCollectionView(LeaveAssignTo.Items);
      LeavesAssignToCollection.CustomSort = new F_SortLeaveCollection();
      LeavesAssignToCollection.Filter = (obj) =>
      {
        if (obj is not NMK_M_Leave leave)
          return false;

        if (UserCurrent.Id == leave.CC)
        {
          if (leave.Approval != 0)
          {
            return false;
          }
        }

        return true;
      };

      LeavesAssignToCollectionCount = new ListCollectionView(LeaveAssignTo.Items);
      LeavesAssignToCollectionCount.Filter = (obj) =>
      {
        if (obj is not NMK_M_Leave leave)
          return false;

        return leave.Approval == 1;
      };

      Schedules_Status = Schedules_Statuss.First();
    }

    #region Collection
    [ObservableProperty]
    ListCollectionView _TasksUserCollectionAdminSchedule;

    [ObservableProperty]
    ListCollectionView _TasksProjectCollectionSchedule;

    [ObservableProperty]
    ListCollectionView _TasksProjectCollection;

    [ObservableProperty]
    ListCollectionView _TasksProjectCollectionCount;

    [ObservableProperty]
    ListCollectionView _TasksProjectCollectionTimeline;

    [ObservableProperty]
    ListCollectionView _TasksUserCollection;

    [ObservableProperty]
    ListCollectionView _TasksEmailCollection;

    [ObservableProperty]
    ListCollectionView _TasksEmailCollectionCount;

    [ObservableProperty]
    ListCollectionView _TasksTemporaryCollection;

    [ObservableProperty]
    ListCollectionView _UsersCollection;

    [ObservableProperty]
    ListCollectionView _UsersCollectionRole;

    [ObservableProperty]
    ListCollectionView _UsersCollectionLeave;

    [ObservableProperty]
    ListCollectionView _ProjectsCollection;

    [ObservableProperty]
    ListCollectionView _notifysCollection;

    [ObservableProperty]
    ListCollectionView _notifysCollectionCount;

    [ObservableProperty]
    ListCollectionView _leavesCollection;

    [ObservableProperty]
    ListCollectionView _leavesAssignToCollection;

    [ObservableProperty]
    ListCollectionView _leavesAssignToCollectionCount;

    [ObservableProperty]
    ListCollectionView _DaysWeekCollection;

    [ObservableProperty]
    ListCollectionView _DaysWeekCollectionSchedules;
    #endregion

    #region Taskbar notify
    [ObservableProperty]
    bool _notifys_IsReadAll = false;

    [ObservableProperty]
    NMK_M_Notify _notifys = new NMK_M_Notify();

    [ObservableProperty]
    bool _notifys_Read = false;
    partial void OnNotifys_ReadChanged(bool oldValue, bool newValue)
    {
      NotifysCollection.Refresh();
    }

    [ObservableProperty]
    ImageSource _TaskbarOverlay = null;
    #endregion

    #region Leave
    [ObservableProperty]
    bool _LeaveChooseDay = false;
    partial void OnLeaveChooseDayChanged(bool oldValue, bool newValue)
    {
      LeaveChooseDayVisible = newValue ? Visibility.Collapsed : Visibility.Visible;
    }

    [ObservableProperty]
    Visibility _LeaveChooseDayVisible = Visibility.Visible;

    [ObservableProperty]
    NMK_M_Leave _LeaveAssignTo = new NMK_M_Leave();

    [ObservableProperty]
    NMK_M_Leave _LeaveSelect = new NMK_M_Leave();

    [ObservableProperty]
    NMK_M_Day _DaysLeaves = new NMK_M_Day();

    [ObservableProperty]
    NMK_M_Leave _leaves = new NMK_M_Leave();

    [ObservableProperty]
    int _FilterMonthLeave = DateTime.Now.Month;
    partial void OnFilterMonthLeaveChanged(int oldValue, int newValue)
    {
      update_day_leave();
      LeavesCollection.Refresh();
    }

    [ObservableProperty]
    int _FilterYearLeave = DateTime.Now.Year;
    partial void OnFilterYearLeaveChanged(int oldValue, int newValue)
    {
      _ = RefreshDataAsync(newValue);
    }

    private async Task RefreshDataAsync(int year)
    {
      try
      {
        // Fetch data without blocking the UI thread
        var holidayData = await F_Date.GetHolidaysAsync(year);

        // Update the collection on the UI thread
        PublicHolidays = new ObservableCollection<HolidayResponse>(holidayData);

        update_day_leave();
        LeavesCollection.Refresh();
      }
      catch (Exception ex)
      {
        // Handle potential network or API errors here
        Debug.WriteLine($"Error refreshing holidays: {ex.Message}");
      }
    }


    [ObservableProperty]
    ObservableCollection<HolidayResponse> _PublicHolidays = new ObservableCollection<HolidayResponse>();
    public async Task InitializeHolidaysAsync()
    {
      try
      {
        // Await the task to keep the UI responsive
        var holidays = await F_Date.GetHolidaysAsync(DateTime.Now.Year);

        // Update the collection
        PublicHolidays = new ObservableCollection<HolidayResponse>(holidays);
      }
      catch (Exception ex)
      {
        // Handle potential network or API errors
        Debug.WriteLine($"Failed to load holidays: {ex.Message}");
      }
    }


    public void update_day_leave()
    {
      #region Thay đổi hiển thị thời gian
      int month = FilterMonthLeave;
      int year = FilterYearLeave;
      var minday = DateTime.Now;
      var maxday = DateTime.Now;
      if (month == 0)
      {
        int lastDayOfMonth = DateTime.DaysInMonth(year, 12);

        minday = new DateTime(year, 1, 1);
        maxday = new DateTime(year, 12, lastDayOfMonth);
      }
      else
      {
        int lastDayOfMonth = DateTime.DaysInMonth(year, month);

        minday = new DateTime(year, month, 1);
        maxday = new DateTime(year, month, lastDayOfMonth);
      }
      if (minday.DayOfWeek == DayOfWeek.Saturday)
        minday = minday.AddDays(2);
      if (minday.DayOfWeek == DayOfWeek.Sunday)
        minday = minday.AddDays(1);
      if (maxday.DayOfWeek == DayOfWeek.Saturday)
        maxday = maxday.AddDays(2);
      if (maxday.DayOfWeek == DayOfWeek.Sunday)
        maxday = maxday.AddDays(1);

      var week_start = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(minday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
      var week_end = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(maxday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
      List<int> weeks = Enumerable.Range(week_start, week_end - week_start).ToList();

      minday = F_Date.GetWeekdaysOfWeek(year, week_start).Start;
      maxday = F_Date.GetWeekdaysOfWeek(year, week_end).End;

      foreach (var item in Leaves.Items)
      {
        //item.Left = (F_Date.CreateDayListNotWeek(MinDay, item.DateStart.Date).Count() - 1) * Tasks.PixelsPerDay;
      }

      DaysLeaves.Items.Clear();
      foreach (var item in F_Date.CreateDayListNotWeek(minday, maxday))
      {
        if (Leaves.Items.Any(x => x.LeaveList.Select(x => x.Start.Date).Contains(item.Name.Date)))
        {
          item.Leave = Leaves.Items.First(x => x.LeaveList.Select(x => x.Start.Date).Contains(item.Name.Date));
          item.IsChecked = true;
        }
        else
        {
          item.IsChecked = false;
        }
        item.Holiday = PublicHolidays.FirstOrDefault(x => x.Date == item.Name.Date);
        DaysLeaves.Items.Add(item);
      }
      #endregion
    }
    #endregion

    #region VersionApp
    [ObservableProperty]
    private NMK_M_Version _versionCurrent = new();

    [ObservableProperty]
    private NMK_M_Version _versionLast = new();

    [ObservableProperty]
    private bool _isVersionUpdate;

    // Cập nhật logic khi VersionCurrent thay đổi
    partial void OnVersionCurrentChanged(NMK_M_Version value) => UpdateStatus();

    // Cập nhật logic khi VersionLast thay đổi
    partial void OnVersionLastChanged(NMK_M_Version value) => UpdateStatus();

    private void UpdateStatus()
    {
      IsVersionUpdate = VersionLast?.Version != VersionCurrent?.Version;
    }

    [ObservableProperty]
    private bool _isVersionUpdateProgress = false;

    [ObservableProperty]
    Visibility _isVersionUpload = Visibility.Collapsed;

    [ObservableProperty]
    string _versionName = string.Empty;

    [ObservableProperty]
    string _versionFile = string.Empty;

    [ObservableProperty]
    bool _isVersionUploadProgress = false;
    #endregion

    #region default
    public List<Brush> Colors = new List<Brush>();
    public ColorDialog colorDialog = new ColorDialog()
    {
      AllowFullOpen = true,
      FullOpen = true,
      AnyColor = true,
    };

    [ObservableProperty]
    NMK_M_User _UserCurrent = new NMK_M_User();

    [ObservableProperty]
    bool _IsAdmin = false;
    partial void OnUserCurrentChanged(NMK_M_User value) => UpdateUserCurrent();
    private void UpdateUserCurrent()
    {
      if (UserCurrent.RoleEnum == F_Role.RoleType.Admin)
      {
        RoleVisible.VisibleAdmin = Visibility.Visible;
        RoleVisible.VisibleLeader = Visibility.Visible;
        RoleVisible.VisibleAdminApp = Visibility.Visible;
        RoleVisible.VisibleUser = Visibility.Visible;
        IsAssignedTo = false;

        RoleVisible.VisibleMiddle = Visibility.Visible;
        IsAdmin = true;
      }
      else if (UserCurrent.RoleEnum == F_Role.RoleType.AdminApp)
      {
        RoleVisible.VisibleOnlyAdminApp = Visibility.Visible;
        IsVersionUpload = Visibility.Visible;

        RoleVisible.VisibleAdmin = Visibility.Visible;
        RoleVisible.VisibleLeader = Visibility.Visible;
        RoleVisible.VisibleAdminApp = Visibility.Visible;
        RoleVisible.VisibleUser = Visibility.Visible;
        IsAssignedTo = false;

        RoleVisible.VisibleMiddle = Visibility.Visible;
        IsAdmin = true;
      }
      else if (UserCurrent.RoleEnum == F_Role.RoleType.Leader)
      {
        RoleVisible.VisibleAdminApp = Visibility.Collapsed;
        RoleVisible.VisibleAdmin = Visibility.Collapsed;
        RoleVisible.VisibleLeader = Visibility.Visible;
        RoleVisible.VisibleUser = Visibility.Visible;
        IsAssignedTo = true;

        RoleVisible.VisibleMiddle = Visibility.Visible;
        IsAdmin = false;
      }
      else if (UserCurrent.RoleEnum == F_Role.RoleType.User)
      {
        RoleVisible.VisibleAdminApp = Visibility.Collapsed;
        RoleVisible.VisibleAdmin = Visibility.Collapsed;
        RoleVisible.VisibleLeader = Visibility.Collapsed;
        RoleVisible.VisibleUser = Visibility.Visible;
        IsAssignedTo = true;

        RoleVisible.VisibleMiddle = Visibility.Collapsed;
        IsAdmin = false;
      }
    }

    [ObservableProperty]
    NMK_M_RoleVisible _RoleVisible = new NMK_M_RoleVisible();

    [ObservableProperty]
    bool _IsAssignedTo = false;
    partial void OnIsAssignedToChanged(bool oldValue, bool newValue)
    {
      ReloadTask();
    }

    [ObservableProperty]
    bool _IsAssignedToSchedule = true;
    partial void OnIsAssignedToScheduleChanged(bool oldValue, bool newValue)
    {
      ReloadTask();
    }

    [ObservableProperty]
    bool _IsLoading = false;

    [ObservableProperty]
    bool _IsDashboard = true;
    #endregion

    #region Schedule
    #region Day Schedules
    [ObservableProperty]
    NMK_M_Day _DaysSchedules = new NMK_M_Day();

    [ObservableProperty]
    DateTime _MinDaySchedules;

    [ObservableProperty]
    DateTime _MaxDaySchedules;

    #endregion
    [ObservableProperty]
    NMK_M_Status _Schedules_Status = new NMK_M_Status();
    partial void OnSchedules_StatusChanged(NMK_M_Status? oldValue, NMK_M_Status newValue)
    {
      ReloadTask();
    }

    [ObservableProperty]
    ObservableCollection<NMK_M_Status> _Schedules_Statuss = new ObservableCollection<NMK_M_Status>()
    {
      new NMK_M_Status()
      {
        Name = "DateChecked",
        Key = "Checked"
      },
      new NMK_M_Status()
      {
        Name = "DateComplete",
        Key = "Complete"
      },
      new NMK_M_Status()
      {
        Name = "DateEnd",
        Key = "End",
      },
    };

    [ObservableProperty]
    NMK_M_ProjectSchedule _ProjectSchedules_AssignTo = new NMK_M_ProjectSchedule();

    [ObservableProperty]
    NMK_M_ProjectSchedule _UsersSchedules_Admin = new NMK_M_ProjectSchedule();
    #endregion

    #region Filter
    private void update_day_schedule()
    {
      #region Thay đổi hiển thị thời gian
      int week = FilterWeekSchedule;
      int month = FilterMonthSchedule;
      int year = FilterYearSchedule;

      if (month == 0)
      {
        int lastDayOfMonth = DateTime.DaysInMonth(year, 12);

        MinDaySchedules = new DateTime(year, 1, 1);
        MaxDaySchedules = new DateTime(year, 12, lastDayOfMonth);
      }
      else
      {
        if (week == 0)
        {
          int lastDayOfMonth = DateTime.DaysInMonth(year, month);

          MinDaySchedules = new DateTime(year, month, 1);
          MaxDaySchedules = new DateTime(year, month, lastDayOfMonth);
        }
        else
        {
          MinDaySchedules = F_Date.GetWeekdaysOfWeek(year, week).Start;
          MaxDaySchedules = F_Date.GetWeekdaysOfWeek(year, week).End;
        }
      }

      // Cập nhật lại danh sách ngày hiển thị trên lịch
      DaysSchedules.Items.Clear();
      foreach (var item in F_Date.CreateDayListNotWeek(MinDaySchedules, MaxDaySchedules))
      {
        DaysSchedules.Items.Add(item);
      }

      // Cập nhật project theo thời gian
      TasksSchedule.Items.Clear();
      List<NMK_M_Task> tasks = new List<NMK_M_Task>();
      foreach (var task in Tasks.Items)
      {
        if (task.Status == 0)
        {
          var start = task.DateStart;
          var end = task.DateChecked;
          if (Schedules_Status.Key == "End")
            end = task.DateEnd;
          else if (Schedules_Status.Key == "Complete")
            end = task.DateComplete;
          var dateList = F_Date.BuildDateSchedules(start, end);
          foreach (var item in dateList)
          {
            if ((item.WeekSchedule == FilterWeekSchedule || FilterWeekSchedule == 0) &&
              (item.MonthSchedule == FilterMonthSchedule || FilterMonthSchedule == 0 || FilterWeekSchedule != 0) &&
              (item.YearSchedule == FilterYearSchedule || FilterYearSchedule == 0))
            {
              var task_child = task.Clone();

              task_child.Date = item.Date;
              task_child.MonthSchedule = item.MonthSchedule;
              task_child.WeekSchedule = item.WeekSchedule;
              task_child.YearSchedule = item.YearSchedule;
              task_child.Time = item.Time;
              task_child.Left = (F_Date.CreateDayListNotWeek(MinDaySchedules, task_child.Date.Date).Count() - 1) * Tasks.PixelsPerDay;

              tasks.Add(task_child);
            }
          }
        }
      }
      foreach (var item in tasks.GroupBy(x => new { project = x.Project, date = x.Date, IsAssignedTo = x.IsAssignedTo }))
      {
        var Time = item.Sum(x => x.Time);
        if (item.Key.IsAssignedTo)
        {
          var task_assignby = tasks.Where(x => x.Date.Date == item.Key.date.Date && !x.IsAssignedTo && x.Project.Key == item.Key.project.Key && !string.IsNullOrEmpty(x.ParentId)).ToList();
          var parentIds = task_assignby.Select(x => x.ParentId).ToList();
          if (item.Any(x => parentIds.Contains(x.Id) && x.IsOnlyChecked))
          {
            foreach (var item_ in item)
            {
              if (task_assignby.Any(x => x.ParentId == item_.Id))
              {
                var child = task_assignby.Where(x => x.ParentId == item_.Id);
                item_.TaskChild = new NMK_M_Task();
                item_.TaskChild.Items = new ObservableCollection<NMK_M_Task>(child);
              }
            }
            Time = Time - task_assignby.Sum(x => x.Time);
          }
        }
        TasksSchedule.Items.Add(new NMK_M_Task()
        {
          Index = 0,
          IsAssignedTo = item.Key.IsAssignedTo,
          Project = item.Key.project,
          Date = item.Key.date,
          Left = item.First().Left,
          WeekSchedule = item.First().WeekSchedule,
          MonthSchedule = item.First().MonthSchedule,
          YearSchedule = item.First().YearSchedule,
          Time = Time,
          ColorSchedule = Time > 8 ? (Brush)new BrushConverter().ConvertFromString("#FF6B00") : Brushes.Black,
          TasksSchedule = new ObservableCollection<NMK_M_Task>(item.OrderBy(x => x.Name))
        });
      }
      var project = new NMK_M_Project() { Name = "Other", Key = "OTHER" };
      List<NMK_M_Task> tasks_other = new List<NMK_M_Task>();
      foreach (var day in DaysSchedules.Items)
      {
        task_other_add(TasksSchedule.Items.Where(x => x.Date.Date == day.Name.Date && x.IsAssignedTo).ToList(), true, day.Name.Date);
        task_other_add(TasksSchedule.Items.Where(x => x.Date.Date == day.Name.Date && !x.IsAssignedTo).ToList(), false, day.Name.Date);
      }
      tasks_other.ForEach(x => TasksSchedule.Items.Add(x));

      void task_other_add(List<NMK_M_Task> group, bool IsAssignedTo, DateTime date)
      {
        if (!group.Any())
        {
          tasks_other.Add(new NMK_M_Task()
          {
            Index = 1,
            IsAssignedTo = IsAssignedTo,
            Project = project,
            Date = date,
            Left = (F_Date.CreateDayListNotWeek(MinDaySchedules, date.Date).Count() - 1) * Tasks.PixelsPerDay,
            Time = 8,
          });
        }
        else
        {
          var time = group.Sum(x => x.Time);
          tasks_other.Add(new NMK_M_Task()
          {
            Index = 1,
            IsAssignedTo = IsAssignedTo,
            Project = project,
            Date = date,
            Left = group.First().Left,
            Time = 8 - time > 0 ? 8 - time : 0,
          });
        }
      }

      ProjectSchedules_AssignTo.TotalTime.Clear();
      ProjectSchedules_AssignTo.TotalTime.Add(TasksSchedule.Items.Where(x => x.Project.Key != "OTHER" && x.IsAssignedTo == IsAssignedToSchedule).Sum(x => x.Time));
      ProjectSchedules_AssignTo.TotalTime.Add(TasksSchedule.Items.Where(x => x.Project.Key == "OTHER" && x.IsAssignedTo == IsAssignedToSchedule).Sum(x => x.Time));
      ProjectSchedules_AssignTo.TotalTime.Add(TasksSchedule.Items.Where(x => x.IsAssignedTo == IsAssignedToSchedule).Sum(x => x.Time));
      ProjectSchedules_AssignTo.TotalTime.Add(ProjectSchedules_AssignTo.TotalTime[0] / 40 * 100);

      var values = new List<NMK_M_ProjectSchedule>();
      var data = TasksSchedule.Items.Where(x => x.IsAssignedTo == IsAssignedToSchedule).GroupBy(x => x.Project);
      foreach (var item in data)
      {
        values.Add(new NMK_M_ProjectSchedule()
        {
          Time = item.Sum(x => x.Time),
          Color = item.Key.ColorString
        });
      }
      ProjectSchedules_AssignTo.XAxes = [
        new Axis()
          {
            Labels = data.Select(x => x.Key.Key).ToArray(),
          }
      ];

      ProjectSchedules_AssignTo.Series = new ISeries[]
      {
            new ColumnSeries<NMK_M_ProjectSchedule>
            {
                DataLabelsFormatter = ProjectSchedules_AssignTo.MyFormatter,
                Values = values,
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                ShowDataLabels = true,
                Mapping = (item, index) => new(index, item.Time)
            }
            .OnPointMeasured(point =>
            {
                if (point.Visual is null) return;

                var data = point.Model;

                point.Visual.Fill = data.Color;
            })
      };
      #endregion
    }
    private void update_day_schedule_admin()
    {
      // Cập nhật project theo thời gian
      TasksAdminSchedule.Items.Clear();
      List<NMK_M_Task> tasks = new List<NMK_M_Task>();
      foreach (var task in TasksAdmin.Items)
      {
        if (0 == 0)
        {
          var start = task.DateStart;
          var end = task.DateChecked;
          if (Schedules_Status.Key == "End")
            end = task.DateEnd;
          else if (Schedules_Status.Key == "Complete")
            end = task.DateComplete;
          var dateList = F_Date.BuildDateSchedules(start, end);
          foreach (var item in dateList)
          {
            if ((item.WeekSchedule == FilterWeekSchedule || FilterWeekSchedule == 0) &&
              (item.MonthSchedule == FilterMonthSchedule || FilterMonthSchedule == 0 || FilterWeekSchedule != 0) &&
              (item.YearSchedule == FilterYearSchedule || FilterYearSchedule == 0))
            {
              var task_child = task.Clone();

              task_child.Date = item.Date;
              task_child.MonthSchedule = item.MonthSchedule;
              task_child.WeekSchedule = item.WeekSchedule;
              task_child.YearSchedule = item.YearSchedule;
              task_child.Time = item.Time;
              task_child.Left = (F_Date.CreateDayListNotWeek(MinDaySchedules, task_child.Date.Date).Count() - 1) * TasksAdmin.PixelsPerDay;

              tasks.Add(task_child);
            }
          }
        }
      }
      foreach (var item in tasks.GroupBy(x => new { user = x.User, date = x.Date }))
      {
        var Time = item.Sum(x => x.Time);
        var task_assignby = tasks.Where(x => x.Date.Date == item.Key.date.Date && !string.IsNullOrEmpty(x.ParentId)).ToList();
        var parentIds = task_assignby.Select(x => x.ParentId).ToList();
        if (item.Any(x => parentIds.Contains(x.Id) && x.IsOnlyChecked))
        {
          foreach (var item_ in item)
          {
            if (task_assignby.Any(x => x.ParentId == item_.Id))
            {
              var child = task_assignby.Where(x => x.ParentId == item_.Id);
              item_.TaskChild = new NMK_M_Task();
              item_.TaskChild.Items = new ObservableCollection<NMK_M_Task>(child);
            }
          }
          Time = Time - task_assignby.Sum(x => x.Time);
        }
        TasksAdminSchedule.Items.Add(new NMK_M_Task()
        {
          Index = 0,
          State = item.First().State,
          User = item.Key.user,
          Date = item.Key.date,
          Left = item.First().Left,
          WeekSchedule = item.First().WeekSchedule,
          MonthSchedule = item.First().MonthSchedule,
          YearSchedule = item.First().YearSchedule,
          Time = Time,
          ColorSchedule = Time > 8 ? (Brush)new BrushConverter().ConvertFromString("#FF6B00") : Brushes.Black,
          TasksSchedule = new ObservableCollection<NMK_M_Task>(item.OrderBy(x => x.Name))
        });
      }

      var values = new List<NMK_M_ProjectSchedule>();
      var data = TasksAdminSchedule.Items.OrderBy(x => x.User.Team).ThenBy(x => x.User.Name).GroupBy(x => x.User);
      foreach (var item in data)
      {
        values.Add(new NMK_M_ProjectSchedule()
        {
          Time = item.Sum(x => x.Time),
          //Color = item.Key.ColorString
        });
      }
      UsersSchedules_Admin.XAxes = [
        new Axis()
          {
            Labels = data.Select(x => x.Key.Key).ToArray(),
          }
      ];

      UsersSchedules_Admin.Series = new ISeries[]
      {
            new ColumnSeries<NMK_M_ProjectSchedule>
            {
                DataLabelsFormatter = UsersSchedules_Admin.MyFormatter,
                Values = values,
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                ShowDataLabels = true,
                Mapping = (item, index) => new(index, item.Time)
            }
            .OnPointMeasured(point =>
            {
                if (point.Visual is null) return;

                var data = point.Model;

                point.Visual.Fill = data.Color;
            })
      };
    }
    private void update_day()
    {
      #region Thay đổi hiển thị thời gian
      // Change IsVisible
      TasksProjectCollectionTimeline.Refresh();
      TasksUserCollection.Refresh();

      int month = FilterMonth;
      int year = FilterYear;
      if (month == 0)
      {
        int lastDayOfMonth = DateTime.DaysInMonth(year, 12);

        MinDay = new DateTime(year, 1, 1);
        MaxDay = new DateTime(year, 12, lastDayOfMonth);
      }
      else
      {
        int lastDayOfMonth = DateTime.DaysInMonth(year, month);

        MinDay = new DateTime(year, month, 1);
        MaxDay = new DateTime(year, month, lastDayOfMonth);
      }

      var visible_task = Tasks.Items.Where(x => x.IsVisible).ToList();
      if (visible_task.Count() > 0)
      {
        MinDay = MinDay < visible_task.Min(x => x.DateStart.Date) ? MinDay : visible_task.Min(x => x.DateStart.Date);
        MaxDay = MaxDay > visible_task.Max(x => x.DateEnd.Date) ? MaxDay : visible_task.Max(x => x.DateEnd.Date);
        foreach (var item in visible_task)
          item.Left = (F_Date.CreateDayListNotWeek(MinDay, item.DateStart.Date).Count() - 1) * Tasks.PixelsPerDay;
      }

      Days.Items.Clear();
      foreach (var item in F_Date.CreateDayListNotWeek(MinDay, MaxDay))
      {
        Days.Items.Add(item);
      }

      // update left change
      TasksProjectCollectionTimeline.Refresh();
      TasksUserCollection.Refresh();
      #endregion
    }

    [ObservableProperty]
    bool _FilterToday = true;
    partial void OnFilterTodayChanged(bool oldValue, bool newValue)
    {
      ReloadTask();
    }

    [ObservableProperty]
    int _FilterMonth = DateTime.Now.Month;
    partial void OnFilterMonthChanged(int oldValue, int newValue)
    {
      ReloadTask();
    }

    [ObservableProperty]
    int _FilterYear = DateTime.Now.Year;
    partial void OnFilterYearChanged(int oldValue, int newValue)
    {
      ReloadTask();
    }

    [ObservableProperty]
    int _FilterMonthSchedule = DateTime.Now.Month;
    partial void OnFilterMonthScheduleChanged(int oldValue, int newValue)
    {
      ReloadTask();
    }

    [ObservableProperty]
    int _FilterYearSchedule = DateTime.Now.Year;
    partial void OnFilterYearScheduleChanged(int oldValue, int newValue)
    {
      ReloadTask();
    }

    [ObservableProperty]
    int _FilterWeekSchedule = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    partial void OnFilterWeekScheduleChanged(int oldValue, int newValue)
    {
      if (FilterWeekSchedule != 0)
      {
        DateTime monday = ISOWeek.ToDateTime(FilterYearSchedule, FilterWeekSchedule, DayOfWeek.Monday);
        FilterMonthSchedule = monday.Month;
      }

      ReloadTask();
    }

    [ObservableProperty]
    DateTime _FilterDateSchedule = DateTime.Now;
    partial void OnFilterDateScheduleChanged(DateTime oldValue, DateTime newValue)
    {
      FilterWeekSchedule = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(newValue, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

      ReloadTask();
    }

    [ObservableProperty]
    ObservableCollection<NMK_M_Status> _Filters_Status = new ObservableCollection<NMK_M_Status>()
      {
        new NMK_M_Status()
        {
          Name = "Task All",
          Key = "All",
          State = 1000,
        },
        new NMK_M_Status()
        {
          Name = "Task Accepted",
          Key = "Accepted",
          IsChecked = true,
          State = 7,
        },
        new NMK_M_Status()
        {
          Name = "Task Start",
          Key = "Start",
          IsChecked = true,
          State = 6,
        },
        new NMK_M_Status()
        {
          Name = "Task New",
          Key = "New",
          IsChecked = true,
          State = 3,
        },
        new NMK_M_Status()
        {
          Name = "Task Checked",
          Key = "Checked",
          IsChecked = true,
          State = 4,
        },
          new NMK_M_Status()
        {
          Name = "Task ReChecked",
          Key = "ReChecked",
          IsChecked = true,
          State = 5,
        },
          new NMK_M_Status()
        {
          Name = "Task Complete",
          Key = "Complete",
          State = 0,
        },
      };
    #endregion

    #region Search
    [ObservableProperty]
    string _SearchProject = string.Empty;
    partial void OnSearchProjectChanged(string? oldValue, string newValue)
    {
      ProjectsCollection.Refresh();
    }

    [ObservableProperty]
    string _SearchUser = string.Empty;
    partial void OnSearchUserChanged(string? oldValue, string newValue)
    {
      UsersCollection.Refresh();
    }

    [ObservableProperty]
    string _SearchTask = string.Empty;

    [ObservableProperty]
    string _SearchEmail = string.Empty;
    partial void OnSearchEmailChanged(string? oldValue, string newValue)
    {
      TasksEmailCollection.Refresh();
      TasksEmailCollectionCount.Refresh();
    }

    [ObservableProperty]
    string _SearchTaskTemporary = string.Empty;
    partial void OnSearchTaskTemporaryChanged(string? oldValue, string newValue)
    {
      TasksTemporaryCollection.Refresh();
    }
    #endregion

    #region Projects
    [ObservableProperty]
    NMK_M_Project _Projects = new NMK_M_Project();

    [ObservableProperty]
    NMK_M_Project _Project = new NMK_M_Project();
    partial void OnProjectChanged(NMK_M_Project value)
    {
      ReloadTask();
    }
    #endregion

    #region Users
    [ObservableProperty]
    NMK_M_User _Users = new NMK_M_User();
    #endregion

    #region Tasks
    [ObservableProperty]
    NMK_M_Task _Tasks = new NMK_M_Task();

    [ObservableProperty]
    NMK_M_Task _TasksSchedule = new NMK_M_Task();

    [ObservableProperty]
    NMK_M_Task _TasksAdmin = new NMK_M_Task();


    [ObservableProperty]
    NMK_M_Task _TasksAdminSchedule = new NMK_M_Task();

    #endregion

    #region Task Temporary
    [ObservableProperty]
    NMK_M_Task _TasksTemporary = new NMK_M_Task();
    #endregion

    #region Days
    [ObservableProperty]
    NMK_M_Day _Days = new NMK_M_Day();

    [ObservableProperty]
    NMK_M_Day _Day = new NMK_M_Day();

    [ObservableProperty]
    DateTime _MinDay;

    [ObservableProperty]
    DateTime _MaxDay;
    #endregion

    #region Dialog
    [ObservableProperty]
    NMK_M_NewTask _DialogNewTask = new NMK_M_NewTask();

    [ObservableProperty]
    NMK_M_Message _DialogMessage = new NMK_M_Message();

    [ObservableProperty]
    NMK_M_NewUser _DialogNewUser = new NMK_M_NewUser();

    [ObservableProperty]
    NMK_M_NewProject _DialogNewProject = new NMK_M_NewProject();

    [ObservableProperty]
    NMK_M_NewLeave _DialogNewLeave = new NMK_M_NewLeave();
    #endregion
  }
}
