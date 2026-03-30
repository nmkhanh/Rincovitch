using RincovitchApp.API;
using RincovitchApp.API.Date;
using System;
using System.Windows.Data;
using System.Windows.Input;
using Brush = System.Windows.Media.Brush;

namespace RincovitchApp.Models.ModelChilds
{
  public class NMK_M_NewTask : NMK_M_Message
  {
    private bool _IsNew = true;
    public bool IsNew
    {
      get => _IsNew;
      set
      {
        _IsNew = value;
        OnPropertyChanged();
      }
    }

    NMK_M_Task _Tasks = new NMK_M_Task();
    public NMK_M_Task Tasks
    {
      get { return _Tasks; }
      set
      {
        _Tasks = value;
        OnPropertyChanged();
      }
    }

    string _Search = "";
    public string Search
    {
      get { return _Search; }
      set
      {
        _Search = value;
        OnPropertyChanged();
        TasksCollection.Refresh();
      }
    }

    ListCollectionView _TasksCollection;
    public ListCollectionView TasksCollection
    {
      get
      {
        return _TasksCollection;
      }
      set
      {
        _TasksCollection = value;
        OnPropertyChanged();
      }
    }


    NMK_M_Project _Projects = new NMK_M_Project();
    public NMK_M_Project Projects
    {
      get { return _Projects; }
      set
      {
        _Projects = value;
        OnPropertyChanged();
      }
    }

    NMK_M_Project _Project = new NMK_M_Project();
    public NMK_M_Project Project
    {
      get { return _Project; }
      set
      {
        _Project = value;
        OnPropertyChanged();
        Validation = keyup();
      }
    }

    NMK_M_User _Users = new NMK_M_User();
    public NMK_M_User Users
    {
      get { return _Users; }
      set
      {
        _Users = value;
        OnPropertyChanged();
      }
    }


    NMK_M_User _User = new NMK_M_User();
    public NMK_M_User User
    {
      get { return _User; }
      set
      {
        _User = value;
        OnPropertyChanged();
        Validation = keyup();
      }
    }

    private string _TitleTask = "NEW TASK";
    public string TitleTask
    {
      get => _TitleTask;
      set
      {
        _TitleTask = value;
        OnPropertyChanged();
        Validation = keyup();
      }
    }

    private string _Detail;
    public string Detail
    {
      get => _Detail;
      set
      {
        _Detail = value;
        OnPropertyChanged();
        Validation = keyup();
      }
    }

    private string _Area = "0.0";
    public string Area
    {
      get => _Area;
      set
      {
        _Area = value;
        OnPropertyChanged();
      }
    }

    private int _DayOld = 0;
    public int DayOld
    {
      get => _DayOld;
      set
      {
        _DayOld = value;
        OnPropertyChanged();
      }
    }

    private int _Day = 1;
    public int Day
    {
      get => _Day;
      set
      {
        _Day = value;
        OnPropertyChanged();
        DateEnd = F_Date.EndDayNotWeek(DateStart, Day).Name;
        Validation = keyup();
      }
    }

    private DateTime _DateStart = DateTime.Now;
    public DateTime DateStart
    {
      get => _DateStart;
      set
      {
        _DateStart = value;
        OnPropertyChanged();
        if (DateStart.DayOfWeek == DayOfWeek.Saturday)
          DateStart = DateStart.AddDays(2);
        else if (DateStart.DayOfWeek == DayOfWeek.Sunday)
          DateStart = DateStart.AddDays(1);

        DateEnd = F_Date.EndDayNotWeek(DateStart, Day).Name;
        Validation = keyup();
      }
    }

    private DateTime _DateEnd = DateTime.Now;
    public DateTime DateEnd
    {
      get => _DateEnd;
      set
      {
        _DateEnd = value;
        OnPropertyChanged();
        Validation = keyup();
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


    private bool _IsInterrupted = false;
    public bool IsInterrupted
    {
      get => _IsInterrupted;
      set
      {
        _IsInterrupted = value;
        OnPropertyChanged();
        Validation = keyup();
      }
    }

    private ICommand _SelectInterruptedCommand;
    public ICommand SelectInterruptedCommand
    {
      get => _SelectInterruptedCommand;
      set
      {
        _SelectInterruptedCommand = value;
        OnPropertyChanged();
      }
    }

    //NMK_M_User _SupportTaskUser = new NMK_M_User();
    //public NMK_M_User SupportTaskUser
    //{
    //  get { return _SupportTaskUser; }
    //  set
    //  {
    //    _SupportTaskUser = value;
    //    OnPropertyChanged();
    //    Validation = keyup();
    //  }
    //}

    //private string _SupportTaskTitleTask;
    //public string SupportTaskTitleTask
    //{
    //  get => _SupportTaskTitleTask;
    //  set
    //  {
    //    _SupportTaskTitleTask = value;
    //    OnPropertyChanged();
    //    Validation = keyup();
    //  }
    //}

    //private string _SupportTaskDetail;
    //public string SupportTaskDetail
    //{
    //  get => _SupportTaskDetail;
    //  set
    //  {
    //    _SupportTaskDetail = value;
    //    OnPropertyChanged();
    //    Validation = keyup();
    //  }
    //}

    //private string _SupportTaskArea = "0.0";
    //public string SupportTaskArea
    //{
    //  get => _SupportTaskArea;
    //  set
    //  {
    //    _SupportTaskArea = value;
    //    OnPropertyChanged();
    //  }
    //}

    //private int _SupportTaskDay = 1;
    //public int SupportTaskDay
    //{
    //  get => _SupportTaskDay;
    //  set
    //  {
    //    _SupportTaskDay = value;
    //    OnPropertyChanged();
    //    SupportTaskDateEnd = F_Date.EndDayNotWeek(SupportTaskDateStart, SupportTaskDay).Name;
    //    Validation = keyup();
    //  }
    //}

    //private DateTime _SupportTaskDateStart = DateTime.Now;
    //public DateTime SupportTaskDateStart
    //{
    //  get => _SupportTaskDateStart;
    //  set
    //  {
    //    _SupportTaskDateStart = value;
    //    OnPropertyChanged();

    //    if (SupportTaskDateStart.DayOfWeek == DayOfWeek.Saturday)
    //      SupportTaskDateStart = SupportTaskDateStart.AddDays(2);
    //    else if (SupportTaskDateStart.DayOfWeek == DayOfWeek.Sunday)
    //      SupportTaskDateStart = SupportTaskDateStart.AddDays(1);

    //    SupportTaskDateEnd = F_Date.EndDayNotWeek(SupportTaskDateStart, SupportTaskDay).Name;
    //    Validation = keyup();
    //  }
    //}

    //private DateTime _SupportTaskDateEnd = DateTime.Now;
    //public DateTime SupportTaskDateEnd
    //{
    //  get => _SupportTaskDateEnd;
    //  set
    //  {
    //    _SupportTaskDateEnd = value;
    //    OnPropertyChanged();
    //    Validation = keyup();
    //  }
    //}

    private string _Validation;
    public string Validation
    {
      get => _Validation;
      set
      {
        _Validation = value;
        OnPropertyChanged();
      }
    }

    #region Duplicate
    private bool _IsDuplicate = false;
    public bool IsDuplicate
    {
      get => _IsDuplicate;
      set
      {
        _IsDuplicate = value;
        OnPropertyChanged();
        if (IsDuplicate)
        {
          MainButtonTitle = "Duplicate";
          Title = "Duplicate Task";
        }
        else
        {
          MainButtonTitle = "Edit";
          Title = "Edit Task";
        }
        Validation = keyup();
      }
    }

    private bool _IsDuplicateMust = false;
    public bool IsDuplicateMust
    {
      get => _IsDuplicateMust;
      set
      {
        _IsDuplicateMust = value;
        OnPropertyChanged();
      }
    }

    private string _DuplicateSupport = "_";
    public string DuplicateSupport
    {
      get => _DuplicateSupport;
      set
      {
        _DuplicateSupport = value;
        OnPropertyChanged();
        Validation = keyup();
      }
    }
    #endregion

    public NMK_M_NewTask()
    {
      Title = "New Task";
      MainButtonTitle = "Create";

      if (DateStart.DayOfWeek == DayOfWeek.Saturday)
        DateStart = DateStart.AddDays(2);
      else if (DateStart.DayOfWeek == DayOfWeek.Sunday)
        DateStart = DateStart.AddDays(1);

      DateEnd = F_Date.EndDayNotWeek(DateStart, Day).Name;

      HourStart = DateStart.Hour;
      MinutesStart = DateStart.Minute;
      HourEnd = DateEnd.Hour;
      MinutesEnd = DateEnd.Minute;


      TasksCollection = new System.Windows.Data.ListCollectionView(Tasks.Items);
      TasksCollection.CustomSort = new F_SortByProject();
      TasksCollection.Filter = (obj) =>
      {
        if (obj is not NMK_M_Task task)
          return false;


        return task.Name.IndexOf(Search, StringComparison.OrdinalIgnoreCase) >= 0 ||
        task.User.Name.IndexOf(Search, StringComparison.OrdinalIgnoreCase) >= 0 ||
        task.DateStart.ToString("dd/MM/yyyy").IndexOf(Search, StringComparison.OrdinalIgnoreCase) >= 0 ||
        task.DateStart.ToString("dddd").IndexOf(Search, StringComparison.OrdinalIgnoreCase) >= 0;
      };
      //SupportTaskDateStart = this.DateStart;
      //SupportTaskDay = this.Day;
      //SupportTaskTitleTask = this.TitleTask;
      //SupportTaskDetail = this.Detail;
    }

    string keyup()
  {
      if (Project == null || string.IsNullOrEmpty(Project.Id))
        return "Error";
      if (User == null || string.IsNullOrEmpty(User.Id))
        return "Error";
      if (string.IsNullOrEmpty(TitleTask))
        return "Error";

      //if (IsSupportTask)
      //{
      //  if (SupportTaskUser == null || string.IsNullOrEmpty(SupportTaskUser.Id))
      //    return "Error";
      //  if (SupportTaskDateStart < DateStart.AddDays(1).Date)
      //    return "Error";
      //}

      if (IsDuplicate)
      {
        if (string.IsNullOrEmpty(DuplicateSupport))
          return "Error";
      }

      return "";
    }
  }

  public class NMK_M_NewUser : NMK_M_Message
  {
    private bool _IsNew = true;
    public bool IsNew
    {
      get => _IsNew;
      set
      {
        _IsNew = value;
        OnPropertyChanged();
      }
    }

    private string _Name;
    public string Name
    {
      get => _Name;
      set
      {
        _Name = value;
        OnPropertyChanged();
        Validation = keyup();
      }
    }

    private string _Email;
    public string Email
    {
      get => _Email;
      set
      {
        _Email = value;
        OnPropertyChanged();
        Validation = keyup();
      }
    }

    private string _Team;
    public string Team
    {
      get => _Team;
      set
      {
        _Team = value;
        OnPropertyChanged();
        Validation = keyup();
      }
    }

    private List<string> _Teams;
    public List<string> Teams
    {
      get => _Teams;
      set
      {
        _Teams = value;
        OnPropertyChanged();
      }
    }

    private string _Role = "User";
    public string Role
    {
      get => _Role;
      set
      {
        _Role = value;
        OnPropertyChanged();
        Validation = keyup();
      }
    }

    private List<string> _Roles = new List<string>
    {
      F_Role.RoleType.Admin.ToString(),
      F_Role.RoleType.Leader.ToString(),
      F_Role.RoleType.User.ToString(),
    };
    public List<string> Roles
    {
      get => _Roles;
      set
      {
        _Roles = value;
        OnPropertyChanged();
      }
    }

    private string _Validation;
    public string Validation
    {
      get => _Validation;
      set
      {
        _Validation = value;
        OnPropertyChanged();
      }
    }

    public NMK_M_NewUser()
    {
      Title = "New User";
      MainButtonTitle = "Create";

      Validation = keyup();
    }

    string keyup()
    {
      if (string.IsNullOrEmpty(Name) || string.IsNullOrWhiteSpace(Name))
        return "Error";
      if (string.IsNullOrEmpty(Email) || string.IsNullOrWhiteSpace(Email))
        return "Error";
      if (string.IsNullOrEmpty(Team) || string.IsNullOrWhiteSpace(Team))
        return "Error";
      if (string.IsNullOrEmpty(Role) || string.IsNullOrWhiteSpace(Role))
        return "Error";

      return "";
    }
  }

  public class NMK_M_NewProject : NMK_M_Message
  {
    private bool _IsNew = true;
    public bool IsNew
    {
      get => _IsNew;
      set
      {
        _IsNew = value;
        OnPropertyChanged();
      }
    }

    private string _RevitVersion;
    public string RevitVersion
    {
      get => _RevitVersion;
      set
      {
        _RevitVersion = value;
        OnPropertyChanged();
        Validation = keyup();
      }
    }
    private string _Name;
    public string Name
    {
      get => _Name;
      set
      {
        _Name = value;
        OnPropertyChanged();
        Validation = keyup();
      }
    }

    private string _Key;
    public string Key
    {
      get => _Key;
      set
      {
        _Key = value;
        OnPropertyChanged();
        Validation = keyup();
      }
    }

    private NMK_M_Project _Project;
    public NMK_M_Project Project
    {
      get => _Project;
      set
      {
        _Project = value;
        OnPropertyChanged();
      }
    }

    private string _Description;
    public string Description
    {
      get => _Description;
      set
      {
        _Description = value;
        OnPropertyChanged();
        Validation = keyup();
      }
    }

    private Brush _Color;
    public Brush Color
    {
      get => _Color;
      set
      {
        _Color = value;
        OnPropertyChanged();
      }
    }

    private string _Validation;
    public string Validation
    {
      get => _Validation;
      set
      {
        _Validation = value;
        OnPropertyChanged();
      }
    }

    public NMK_M_NewProject()
    {
      Title = "New Project";
      MainButtonTitle = "Create";

      Validation = keyup();
    }

    string keyup()
    {
      if (string.IsNullOrEmpty(Name) || string.IsNullOrWhiteSpace(Name))
        return "Error";
      if (string.IsNullOrEmpty(Key) || string.IsNullOrWhiteSpace(Key))
        return "Error";
      if (string.IsNullOrEmpty(RevitVersion) || string.IsNullOrWhiteSpace(RevitVersion))
        return "Error";
      if (Project.Items.Any(x => x.Key == Key) && IsNew)
        return "Error";

      return "";
    }
  }


  public class NMK_M_NewLeave: NMK_M_Message
  {
    NMK_M_User _Users = new NMK_M_User();
    public NMK_M_User Users
    {
      get { return _Users; }
      set
      {
        _Users = value;
        OnPropertyChanged();
      }
    }


    NMK_M_User _User_Sendto = new NMK_M_User();
    public NMK_M_User User_Sendto
    {
      get { return _User_Sendto; }
      set
      {
        _User_Sendto = value;
        OnPropertyChanged();
        Validation = keyup();
      }
    }

    NMK_M_User _User_CC = new NMK_M_User();
    public NMK_M_User User_CC
    {
      get { return _User_CC; }
      set
      {
        _User_CC = value;
        OnPropertyChanged();
      }
    }

    private string _Reason;
    public string Reason
    {
      get => _Reason;
      set
      {
        _Reason = value;
        OnPropertyChanged();
        Validation = keyup();
      }
    }

    private string _Validation;
    public string Validation
    {
      get => _Validation;
      set
      {
        _Validation = value;
        OnPropertyChanged();
      }
    }

    public NMK_M_NewLeave()
    {
      Title = "New";
      MainButtonTitle = "Add";

      Validation = keyup();
    }

    string keyup()
    {
      if (string.IsNullOrEmpty(Reason) || string.IsNullOrWhiteSpace(Reason))
        return "Error";
      if (User_Sendto == null || string.IsNullOrEmpty(User_Sendto.Email))
        return "Error";

      return "";
    }
  }
}
