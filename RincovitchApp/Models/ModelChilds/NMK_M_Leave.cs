using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;

namespace RincovitchApp.Models.ModelChilds
{
  public class NMK_M_Leave : BaseViewModel
  {
    ObservableCollection<NMK_M_Leave> _Items = new ObservableCollection<NMK_M_Leave>();
    public ObservableCollection<NMK_M_Leave> Items
    {
      get { return _Items; }
      set
      {
        _Items = value;
        OnPropertyChanged();
      }
    }

    DateTime _CreateAt = DateTime.Now;
    public DateTime CreateAt
    {
      get { return _CreateAt; }
      set
      {
        _CreateAt = value;
        OnPropertyChanged();
      }
    }

    DateTime _UpdateAt = DateTime.Now;
    public DateTime UpdateAt
    {
      get { return _UpdateAt; }
      set
      {
        _UpdateAt = value;
        OnPropertyChanged();
      }
    }

    string _Id;
    public string Id
    {
      get { return _Id; }
      set
      {
        _Id = value;
        OnPropertyChanged();
      }
    }

    Brush _Background = new SolidColorBrush(Colors.White);
    public Brush Background
    {
      get { return _Background; }
      set
      {
        _Background = value;
        OnPropertyChanged();
      }
    }

    bool _IsChecked;
    public bool IsChecked
    {
      get { return _IsChecked; }
      set
      {
        _IsChecked = value;
        OnPropertyChanged();
      }
    }

    bool _IsProgress;
    public bool IsProgress
    {
      get { return _IsProgress; }
      set
      {
        _IsProgress = value;
        OnPropertyChanged();
      }
    }

    int _approval = 2;
    public int Approval
    {
      get { return _approval; }
      set
      {
        _approval = value;
        OnPropertyChanged();
        ApprovalStatus = Approval == 2;
        ApprovalStatusAssign = Approval != 0;
      }
    }

    bool _ApprovalStatus = true;
    public bool ApprovalStatus
    {
      get { return _ApprovalStatus; }
      set
      {
        _ApprovalStatus = value;
        OnPropertyChanged();
      }
    }

    bool _ApprovalStatusAssign = true;
    public bool ApprovalStatusAssign
    {
      get { return _ApprovalStatusAssign; }
      set
      {
        _ApprovalStatusAssign = value;
        OnPropertyChanged();
      }
    }

    string _LeaveType = "Leave Without Pay";
    public string LeaveType
    {
      get { return _LeaveType; }
      set
      {
        _LeaveType = value;
        OnPropertyChanged();
      }
    }

    List<string> _LeaveTypeList = new List<string>()
    {
      "Public Holiday",
      "Annual Leave",
      "Leave Without Pay",
      "Personal Leave (Sick, Carer)",
      "Long Service",
      "Parental Leave",
      "Community Service Leave - Unpaid",
      "Community Service Jury Duty - Paid",
      "Compassionate & Bereavement",
    };
    public List<string> LeaveTypeList
    {
      get { return _LeaveTypeList; }
      set
      {
        _LeaveTypeList = value;
        OnPropertyChanged();
      }
    }

    string _LeaveReason;
    public string LeaveReason
    {
      get { return _LeaveReason; }
      set
      {
        _LeaveReason = value;
        OnPropertyChanged();
      }
    }

    string _CreateBy;
    public string CreateBy
    {
      get { return _CreateBy; }
      set
      {
        _CreateBy = value;
        OnPropertyChanged();
      }
    }

    string _SendTo;
    public string SendTo
    {
      get { return _SendTo; }
      set
      {
        _SendTo = value;
        OnPropertyChanged();
      }
    }

    string _cc;
    public string CC
    {
      get { return _cc; }
      set
      {
        _cc = value;
        OnPropertyChanged();
      }
    }

    NMK_M_User _User;
    public NMK_M_User User
    {
      get { return _User; }
      set
      {
        _User = value;
        OnPropertyChanged();
      }
    }

    NMK_M_User _UserCC;
    public NMK_M_User UserCC
    {
      get { return _UserCC; }
      set
      {
        _UserCC = value;
        OnPropertyChanged();
      }
    }

    NMK_M_User _UserCreateBy;
    public NMK_M_User UserCreateBy
    {
      get { return _UserCreateBy; }
      set
      {
        _UserCreateBy = value;
        OnPropertyChanged();
      }
    }

    ListCollectionView _LeaveListCollectionView;
    public ListCollectionView LeaveListCollectionView
    {
      get { return _LeaveListCollectionView; }
      set
      {
        _LeaveListCollectionView = value;
        OnPropertyChanged();
      }
    }

    ObservableCollection<NMK_M_LeaveDay> _LeaveList = new ObservableCollection<NMK_M_LeaveDay>();
    public ObservableCollection<NMK_M_LeaveDay> LeaveList
    {
      get { return _LeaveList; }
      set
      {
        _LeaveList = value;
        OnPropertyChanged();
      }
    }

    public NMK_M_Leave Clone()
    {
      return new NMK_M_Leave
      {
        Id = this.Id,
        LeaveReason = this.LeaveReason,
        LeaveList = this.LeaveList,
        CreateAt = this.CreateAt,
        UpdateAt = this.UpdateAt,
        CreateBy = this.CreateBy,
        SendTo = this.SendTo,
        CC = this.CC,
        Approval = this.Approval,
        LeaveType = this.LeaveType,
      };
    }

    public void Set(NMK_M_Leave leave)
    {
      Approval = leave.Approval;
    }
  }

  public class NMK_M_LeaveDay : BaseViewModel
  {
    public NMK_M_LeaveDay()
    {

    }

    DateTime _LeaveStart;
    public DateTime LeaveStart
    {
      get { return _LeaveStart; }
      set
      {
        _LeaveStart = value;
        OnPropertyChanged();
      }
    }

    DateTime _LeaveEnd;
    public DateTime LeaveEnd
    {
      get { return _LeaveEnd; }
      set
      {
        _LeaveEnd = value;
        OnPropertyChanged();
      }
    }

    DateTime _Start;
    public DateTime Start
    {
      get { return _Start; }
      set
      {
        _Start = value;
        OnPropertyChanged();
        LeaveStart = new DateTime(Start.Year, Start.Month, Start.Day, StartH, StartM, 0);
      }
    }

    int _StartH;
    public int StartH
    {
      get { return _StartH; }
      set
      {
        _StartH = value;
        OnPropertyChanged();
        LeaveStart = new DateTime(Start.Year, Start.Month, Start.Day, StartH, StartM, 0);
      }
    }

    int _StartM;
    public int StartM
    {
      get { return _StartM; }
      set
      {
        _StartM = value;
        OnPropertyChanged();
        LeaveStart = new DateTime(Start.Year, Start.Month, Start.Day, StartH, StartM, 0);
      }
    }

    DateTime _End;
    public DateTime End
    {
      get { return _End; }
      set
      {
        _End = value;
        OnPropertyChanged();
        LeaveEnd = new DateTime(End.Year, End.Month, End.Day, EndH, EndM, 0);
      }
    }

    int _EndH;
    public int EndH
    {
      get { return _EndH; }
      set
      {
        _EndH = value;
        OnPropertyChanged();
        LeaveEnd = new DateTime(End.Year, End.Month, End.Day, EndH, EndM, 0);
      }
    }

    int _EndM;
    public int EndM
    {
      get { return _EndM; }
      set
      {
        _EndM = value;
        OnPropertyChanged();
        LeaveEnd = new DateTime(End.Year, End.Month, End.Day, EndH, EndM, 0);
      }
    }

    ObservableCollection<NMK_M_User> _Users = new ObservableCollection<NMK_M_User>();
    public ObservableCollection<NMK_M_User> Users
    {
      get { return _Users; }
      set
      {
        _Users = value;
        OnPropertyChanged();
        UsersList = Users != null && Users.Count() > 0 ? string.Join(", ", Users.Select(x => x.Name)) : string.Empty;
      }
    }

    string _UsersList;
    public string UsersList
    {
      get { return _UsersList; }
      set
      {
        _UsersList = value;
        OnPropertyChanged();
      }
    }

    int _Percent;
    public int Percent
    {
      get { return _Percent; }
      set
      {
        _Percent = value;
        OnPropertyChanged();
      }
    }
  }
}
