using Microsoft.IdentityModel.Tokens;
using RincovitchApp.API.Date;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace RincovitchApp.Models.ModelChilds
{
  public class NMK_M_Plan : BaseViewModel
  {
    DateTime _Date = DateTime.Now;
    public DateTime Date
    {
      get { return _Date; }
      set
      {
        _Date = value;
        OnPropertyChanged();
        WeekSchedule = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(Date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        MonthSchedule = Date.Month;
        YearSchedule = Date.Year;
      }
    }

    int _WeekSchedule;
    //int _Year = 2025;
    public int WeekSchedule
    {
      get { return _WeekSchedule; }
      set
      {
        _WeekSchedule = value;
        OnPropertyChanged();
      }
    }

    int _MonthSchedule;
    public int MonthSchedule
    {
      get { return _MonthSchedule; }
      set
      {
        _MonthSchedule = value;
        OnPropertyChanged();
      }
    }

    int _YearSchedule;
    //int _Year = 2025;
    public int YearSchedule
    {
      get { return _YearSchedule; }
      set
      {
        _YearSchedule = value;
        OnPropertyChanged();
      }
    }

    private bool _IsProgress;
    public bool IsProgress
    {
      get => _IsProgress;
      set
      {
        _IsProgress = value;
        OnPropertyChanged();
      }
    }

    private bool _IsChecked;
    public bool IsChecked
    {
      get
      {
        return _IsChecked;
      }
      set
      {
        _IsChecked = value;
        OnPropertyChanged();
      }
    }

    bool _IsAssignedTo = false;
    public bool IsAssignedTo
    {
      get
      {
        return _IsAssignedTo;
      }
      set
      {
        _IsAssignedTo = value;
        OnPropertyChanged();
      }
    }

    Brush _ColorSchedule = Brushes.Black;
    public Brush ColorSchedule
    {
      get
      {
        return _ColorSchedule;
      }
      set
      {
        _ColorSchedule = value;
        OnPropertyChanged();
      }
    }

    Brush _Color;
    public Brush Color
    {
      get
      {
        return _Color;
      }
      set
      {
        _Color = value;
        OnPropertyChanged();
      }
    }

    Brush _ColorState;
    public Brush ColorState
    {
      get
      {
        return _ColorState;
      }
      set
      {
        _ColorState = value;
        OnPropertyChanged();
      }
    }

    // UI-calculated
    private double _Left;
    public double Left
    {
      get
      {
        return _Left;
      }
      set
      {
        _Left = value;
        OnPropertyChanged();
      }
    }

    private int _Day = 1;
    public int Day
    {
      get
      {
        return _Day;
      }
      set
      {
        _Day = value;
        OnPropertyChanged();
        DateEnd = F_Date.EndDayNotWeek(DateStart, Day).Name.Date.AddHours(DateEnd.Hour).AddMinutes(DateEnd.Minute);
      }
    }

    private double _Width;
    public double Width
    {
      get
      {
        return _Width;
      }
      set
      {
        _Width = value;
        OnPropertyChanged();
      }
    }

    private double _PixelsPerDay = 80;
    public double PixelsPerDay
    {
      get
      {
        return _PixelsPerDay;
      }
      set
      {
        _PixelsPerDay = value;
        OnPropertyChanged();
      }
    }

    private double _PixelsPerUser = 30;
    public double PixelsPerUser
    {
      get
      {
        return _PixelsPerUser;
      }
      set
      {
        _PixelsPerUser = value;
        OnPropertyChanged();
      }
    }


    public NMK_M_Plan()
    {
      NameFilter = NormalizeSearch(Name);

      if (DateStart.DayOfWeek == DayOfWeek.Saturday)
        DateStart = DateStart.AddDays(2);
      else if (DateStart.DayOfWeek == DayOfWeek.Sunday)
        DateStart = DateStart.AddDays(1);

      DateEnd = F_Date.EndDayNotWeek(DateStart, Day).Name;
    }

    ObservableCollection<NMK_M_Plan> _Items = new ObservableCollection<NMK_M_Plan>();
    public ObservableCollection<NMK_M_Plan> Items
    {
      get
      {
        return _Items;
      }
      set
      {
        _Items = value;
        OnPropertyChanged();
      }
    }

    string _ProjectId;
    public string ProjectId
    {
      get
      {
        return _ProjectId;
      }
      set
      {
        _ProjectId = value;
        OnPropertyChanged();
      }
    }

    NMK_M_Project _Project;
    public NMK_M_Project Project
    {
      get
      {
        return _Project;
      }
      set
      {
        _Project = value;
        OnPropertyChanged();
      }
    }

    string _UserId;
    public string UserId
    {
      get
      {
        return _UserId;
      }
      set
      {
        _UserId = value;
        OnPropertyChanged();
      }
    }

    NMK_M_User _User;
    public NMK_M_User User
    {
      get
      {
        return _User;
      }
      set
      {
        _User = value;
        OnPropertyChanged();
      }
    }

    DateTime _DateStart = DateTime.Now;
    public DateTime DateStart
    {
      get
      {
        return _DateStart;
      }
      set
      {
        _DateStart = value;
        OnPropertyChanged();
        if (DateStart.DayOfWeek == DayOfWeek.Saturday)
          DateStart = DateStart.AddDays(2);
        else if (DateStart.DayOfWeek == DayOfWeek.Sunday)
          DateStart = DateStart.AddDays(1);

        DateEnd = F_Date.EndDayNotWeek(DateStart, Day).Name.Date.AddHours(DateEnd.Hour).AddMinutes(DateEnd.Minute);
      }
    }

    DateTime _DateEnd = DateTime.Now;
    public DateTime DateEnd
    {
      get
      {
        return _DateEnd;
      }
      set
      {
        _DateEnd = value;
        OnPropertyChanged();
      }
    }

    private int _HourStart;
    public int HourStart
    {
      get
      {
        return _HourStart;
      }
      set
      {
        _HourStart = value;
        OnPropertyChanged();
      }
    }

    private int _MinutesStart;
    public int MinutesStart
    {
      get
      {
        return _MinutesStart;
      }
      set
      {
        _MinutesStart = value;
        OnPropertyChanged();
      }
    }

    private int _HourEnd;
    public int HourEnd
    {
      get
      {
        return _HourEnd;
      }
      set
      {
        _HourEnd = value;
        OnPropertyChanged();
      }
    }

    private int _MinutesEnd;
    public int MinutesEnd
    {
      get
      {
        return _MinutesEnd;
      }
      set
      {
        _MinutesEnd = value;
        OnPropertyChanged();
      }
    }

    DateTime _DateComplete = DateTime.Now;
    public DateTime DateComplete
    {
      get
      {
        return _DateComplete;
      }
      set
      {
        _DateComplete = value;
        OnPropertyChanged();
      }
    }

    DateTime _DateChecked = DateTime.Now;
    public DateTime DateChecked
    {
      get
      {
        return _DateChecked;
      }
      set
      {
        _DateChecked = value;
        OnPropertyChanged();
      }
    }
    
    DateTime _DateStarted = DateTime.Now;
    public DateTime DateStarted
    {
      get
      {
        return _DateStarted;
      }
      set
      {
        _DateStarted = value;
        OnPropertyChanged();
      }
    }

    DateTime _DateAccepted = DateTime.Now;
    public DateTime DateAccepted
    {
      get
      {
        return _DateAccepted;
      }
      set
      {
        _DateAccepted = value;
        OnPropertyChanged();
      }
    }

    #region Filter
    private bool _isVisible = true;
    public bool IsVisible
    {
      get => _isVisible;
      set
      {
        _isVisible = value;
        OnPropertyChanged();
      }
    }

    string NormalizeSearch(string text)
    {
      if (string.IsNullOrWhiteSpace(text))
        return string.Empty;

      text = text
          .Replace('đ', 'd')
          .Replace('Đ', 'd');

      var normalized = text.Normalize(NormalizationForm.FormD);
      var sb = new StringBuilder();

      foreach (var c in normalized)
      {
        if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
          sb.Append(c);
      }

      return sb
          .ToString()
          .ToLowerInvariant()
          .Normalize(NormalizationForm.FormC);
    }

    private bool FilterTime(int month, int year)
    {
      bool match = true;
      if (!string.IsNullOrEmpty(year.ToString()) && year != 0)
      {
        match &= DateStart.Date.Year == year || DateEnd.Date.Year == year;
        if (!string.IsNullOrEmpty(month.ToString()) && month != 0)
        {
          match &= DateStart.Date.Month == month || DateEnd.Date.Month == month;
        }
      }

      bool childMatch = false;
      foreach (var c in Items)
      {
        childMatch |= c.FilterTime(month, year);
      }

      return match || childMatch;
    }

    private bool FilterState(ObservableCollection<NMK_M_Status> filter_status)
    {
      List<int> check = filter_status.Where(x => x.IsChecked).Select(x => x.State).ToList();
      bool matchstatus = check.Contains(Status);

      bool childMatch = false;
      foreach (var c in Items)
      {
        childMatch |= c.FilterState(filter_status);
      }

      return matchstatus || childMatch;
    }

    private bool FilterProject(string projectId)
    {
      bool matchstatus = projectId == ProjectId;

      bool childMatch = false;
      foreach (var c in Items)
      {
        childMatch |= c.FilterProject(projectId);
      }

      return matchstatus || childMatch;
    }

    private bool FilterIsAssigned(bool isAssignedTo)
    {
      bool matchstatus = isAssignedTo == IsAssignedTo;

      bool childMatch = false;
      foreach (var c in Items)
      {
        childMatch |= c.FilterIsAssigned(isAssignedTo);
      }

      return matchstatus || childMatch;
    }

    public bool Filter(int month, int year, string projectId, ObservableCollection<NMK_M_Status> filter_status, bool isAssignedTo)
    {
      IsVisible = FilterTime(month, year) && FilterState(filter_status) && FilterProject(projectId) && FilterIsAssigned(isAssignedTo);
      return IsVisible;
    }
    #endregion

    string _Name = string.Empty;
    public string Name
    {
      get
      {
        return _Name;
      }
      set
      {
        _Name = value;
        OnPropertyChanged();
        NameFilter = NormalizeSearch(_Name);
      }
    }


    string _Area = "0.0";
    public string Area
    {
      get
      {
        return _Area;
      }
      set
      {
        _Area = value;
        OnPropertyChanged();
      }
    }

    string _OnlyName = string.Empty;
    public string OnlyName
    {
      get
      {
        return _OnlyName;
      }
      set
      {
        _OnlyName = value;
        OnPropertyChanged();
      }
    }

    string _Detail = string.Empty;
    public string Detail
    {
      get
      {
        return _Detail;
      }
      set
      {
        _Detail = value;
        OnPropertyChanged();
      }
    }

    string _NameFilter;
    public string NameFilter
    {
      get
      {
        return _NameFilter;
      }
      set
      {
        _NameFilter = value;
        OnPropertyChanged();
      }
    }

    int _IndexStatus = 0;
    public int IndexStatus
    {
      get
      {
        return _IndexStatus;
      }
      set
      {
        _IndexStatus = value;
        OnPropertyChanged();
      }
    }
    int _Index = 0;
    public int Index
    {
      get
      {
        return _Index;
      }
      set
      {
        _Index = value;
        OnPropertyChanged();
      }
    }

    int _Status = 0; // 0 : Complete, 1 : New dont send , 2 : Edit dont send, 3 : New , 4 : Checked, 5 : ReChecked, 6 : Start, 7 : Accepted , 10 : Interrupted
    public int Status
    {
      get
      {
        return _Status;
      }
      set
      {
        _Status = value;
        OnPropertyChanged();
        IsChecked = Status == 1 || Status == 2;

        IndexStatus = Status switch
        {
          0 => 100,
          1 => 99,
          2 => 98,
          3 => 2,
          4 => 4,
          5 => 5,
          6 => 3,
          7 => 1,
          10 => 97,
          _ => 0,
        };
        ColorState = Status switch
        {
          0 => (Brush)new BrushConverter().ConvertFromString("#15803D"),
          1 => (Brush)new BrushConverter().ConvertFromString("#2563EB"),
          2 => (Brush)new BrushConverter().ConvertFromString("#F59E0B"),
          3 => (Brush)new BrushConverter().ConvertFromString("#22C55E"),
          4 => (Brush)new BrushConverter().ConvertFromString("#C2410C"),
          5 => (Brush)new BrushConverter().ConvertFromString("#1D4ED8"),
          6 => (Brush)new BrushConverter().ConvertFromString("#7E22CE"),
          7 => (Brush)new BrushConverter().ConvertFromString("#F59E0B"),
          10 => (Brush)new BrushConverter().ConvertFromString("#9CA3AF"),
          _ => Brushes.Transparent,
        };

        State = Status switch
        {
          0 => "Complete",
          1 => "",
          2 => "",
          3 => "New",
          4 => "Checked",
          5 => "ReChecked",
          6 => "Start",
          7 => "Accepted",
          10 => "Interrupted",
          _ => "Unknown",
        };

        StatusVisible = Status == 3 || Status == 4 || Status == 5 || Status == 6 || Status == 7;
        StateStarted = Status == 6;
        StateInterrupted = Status == 10;
        StateAccepted = Status == 3;

        AttachVisibleTo = Status == 3 || Status == 5 || Status == 6 || Status == 7;
        AttachVisibleBy = Status == 4;
      }
    }

    bool _StateInterrupted;
    public bool StateInterrupted
    {
      get
      {
        return _StateInterrupted;
      }
      set
      {
        _StateInterrupted = value;
        OnPropertyChanged();
      }
    }

    bool _IsInterrupted;
    public bool IsInterrupted
    {
      get
      {
        return _IsInterrupted;
      }
      set
      {
        _IsInterrupted = value;
        OnPropertyChanged();
      }
    }

    List<string> _ListInterrupted = new List<string>();
    public List<string> ListInterrupted
    {
      get
      {
        return _ListInterrupted;
      }
      set
      {
        _ListInterrupted = value;
        OnPropertyChanged();
      }
    }

    bool _StateAccepted;
    public bool StateAccepted
    {
      get
      {
        return _StateAccepted;
      }
      set
      {
        _StateAccepted = value;
        OnPropertyChanged();
      }
    }

    bool _StateStarted;
    public bool StateStarted
    {
      get
      {
        return _StateStarted;
      }
      set
      {
        _StateStarted = value;
        OnPropertyChanged();
      }
    }

    bool _AttachVisibleTo;
    public bool AttachVisibleTo
    {
      get
      {
        return _AttachVisibleTo;
      }
      set
      {
        _AttachVisibleTo = value;
        OnPropertyChanged();
      }
    }

    bool _AttachVisibleBy;
    public bool AttachVisibleBy
    {
      get
      {
        return _AttachVisibleBy;
      }
      set
      {
        _AttachVisibleBy = value;
        OnPropertyChanged();
      }
    }


    bool _StatusVisible;
    public bool StatusVisible
    {
      get
      {
        return _StatusVisible;
      }
      set
      {
        _StatusVisible = value;
        OnPropertyChanged();
      }
    }

    bool _StatusVisibleRoleTo;
    public bool StatusVisibleRoleTo
    {
      get
      {
        return _StatusVisibleRoleTo;
      }
      set
      {
        _StatusVisibleRoleTo = value;
        OnPropertyChanged();
      }
    }

    bool _StatusVisibleRoleBy;
    public bool StatusVisibleRoleBy
    {
      get
      {
        return _StatusVisibleRoleBy;
      }
      set
      {
        _StatusVisibleRoleBy = value;
        OnPropertyChanged();
      }
    }

    string _State;
    public string State
    {
      get
      {
        return _State;
      }
      set
      {
        _State = value;
        OnPropertyChanged();
      }
    }

    string _Id = string.Empty;
    public string Id
    {
      get
      {
        return _Id;
      }
      set
      {
        _Id = value;
        OnPropertyChanged();
      }
    }

    string _ParentId = string.Empty;
    public string ParentId
    {
      get
      {
        return _ParentId;
      }
      set
      {
        _ParentId = value;
        OnPropertyChanged();
      }
    }

    bool _IsOnlyChecked = true;
    public bool IsOnlyChecked
    {
      get
      {
        return _IsOnlyChecked;
      }
      set
      {
        _IsOnlyChecked = value;
        OnPropertyChanged();
      }
    }

    string _CreateBy = string.Empty;
    public string CreateBy
    {
      get
      {
        return _CreateBy;
      }
      set
      {
        _CreateBy = value;
        OnPropertyChanged();
      }
    }

    DateTime _CreateAt;
    public DateTime CreateAt
    {
      get
      {
        return _CreateAt;
      }
      set
      {
        _CreateAt = value;
        OnPropertyChanged();
      }
    }

    public NMK_M_Plan Clone()
    {
      return new NMK_M_Plan
      {
        Id = this.Id,
        Name = this.Name,
        ProjectId = this.ProjectId,
        UserId = this.UserId,
        DateStart = this.DateStart,
        DateEnd = this.DateEnd,
        Index = this.Index,
        Detail = this.Detail,
        Color = this.Color,
        Status = this.Status,
        CreateAt = this.CreateAt,
        CreateBy = this.CreateBy,
        DateComplete = this.DateComplete,
        ParentId = this.ParentId,
        IsOnlyChecked = this.IsOnlyChecked,
        DateChecked = this.DateChecked,
        DateStarted = this.DateStarted,
        DateAccepted = this.DateAccepted,
        Area = this.Area.ToString(),

        OnlyName = this.OnlyName,
        Project = this.Project,
        User = this.User,
        Day = this.Day,
        Width = this.Width,
        IsAssignedTo = this.IsAssignedTo,
      };
    }

    public NMK_M_Plan New()
    {
      return new NMK_M_Plan
      {
        Id = Guid.NewGuid().ToString(),
        Name = this.Name,
        ProjectId = this.ProjectId,
        UserId = this.UserId,
        DateStart = this.DateStart,
        DateEnd = this.DateEnd,
        Index = this.Index,
        Detail = this.Detail,
        Color = this.Color,
        Status = this.Status,
      };
    }

    public void Set(NMK_Supabase_Plan task)
    {
      Id = task.Id;
      Name = task.Name;
      ProjectId = task.ProjectId;
      Index = task.Index;
      Color = !string.IsNullOrEmpty(task.Color) ? (Brush)new BrushConverter().ConvertFromString(task.Color) : null;
      UserId = task.UserId;
      DateStart = task.DateStart;
      DateEnd = task.DateEnd;
      Detail = task.Detail;
      Status = task.Status;
      CreateAt = task.CreatedAt;
      CreateBy = task.CreateBy;
      DateComplete = task.DateComplete;
      ParentId = task.ParentId;
      IsOnlyChecked = task.IsOnlyChecked;
      DateChecked = task.DateChecked;
      DateStarted = task.DateStarted;
      DateAccepted = task.DateAccepted;
      Area = task.Area.ToString();
    }
    public void Sort()
    {
      if (this.Items != null && this.Items.Count > 0)
      {
        var sorted = this.Items.OrderBy(i => i.Name).ToList();
        this.Items = new ObservableCollection<NMK_M_Plan>(sorted);
      }
    }
  }
}
