using CommunityToolkit.Mvvm.ComponentModel;
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
  public partial class NMK_M_Task_Temporary : ObservableObject
  {
    [ObservableProperty]
    bool _IsProgress;

    [ObservableProperty]
    bool _IsEdit;

    [ObservableProperty]
    bool _IsChecked;

    [ObservableProperty]
    bool _ViewDetail = false;

    [ObservableProperty]
    ObservableCollection<NMK_M_Task_Temporary> _Items = new ObservableCollection<NMK_M_Task_Temporary>();

    [ObservableProperty]
    string _Id;

    [ObservableProperty]
    string _Name;

    [ObservableProperty]
    string _ProjectId;

    [ObservableProperty]
    NMK_M_Project _Project;

    [ObservableProperty]
    DateTime _CreateAt;

    [ObservableProperty]
    DateTime _UpdateAt;

    [ObservableProperty]
    string _CreateBy;

    [ObservableProperty]
    string _UpdateBy;

    [ObservableProperty]
    DateTime _Time;

    [ObservableProperty]
    int _HH;

    [ObservableProperty]
    int _MM;

    [ObservableProperty]
    ObservableCollection<DateTime> _TimeList = new ObservableCollection<DateTime>();

    [ObservableProperty]
    string _Status;
    partial void OnStatusChanged(string value)
    {
      StatusColor = value switch
      {
        "WIP" => (Brush)new BrushConverter().ConvertFromString("#6366f1"),
        "DONE" => (Brush)new BrushConverter().ConvertFromString("#10b981"),
        "PENDING" => (Brush)new BrushConverter().ConvertFromString("#64748b"),
        "TMR" => (Brush)new BrushConverter().ConvertFromString("#f97316"),
        "PLANNING" => (Brush)new BrushConverter().ConvertFromString("#fbbf24"),
        "URGENT" => (Brush)new BrushConverter().ConvertFromString("#f43f5e"),
        "HIGH PRIORITY" => (Brush)new BrushConverter().ConvertFromString("#8b5cf6"),
        "ISSUE" => (Brush)new BrushConverter().ConvertFromString("#ef4444"),
        _ => Brushes.Black,
      };
    }

    [ObservableProperty]
    Brush _StatusColor;

    [ObservableProperty]
    List<string> _StatusList = new List<string>()
    {
      "WIP",
      "DONE",
      "PENDING",
      "TMR",
      "PLANNING",
      "URGENT",
      "HIGH PRIORITY",
      "ISSUE",
    };

    [ObservableProperty]
    int _Week;

    [ObservableProperty]
    int _Year;

    [ObservableProperty]
    string _UserId;

    [ObservableProperty]
    ObservableCollection<NMK_M_User> _UserList = new ObservableCollection<NMK_M_User>();
    partial void OnUserListChanged(ObservableCollection<NMK_M_User> value)
    {
      UserCount = value.Count(x => x.IsChecked);
    }

    [ObservableProperty]
    int _UserCount;

    [ObservableProperty]
    string _UserId_CC;

    [ObservableProperty]
    ObservableCollection<NMK_M_User> _UserList_CC = new ObservableCollection<NMK_M_User>();
    partial void OnUserList_CCChanged(ObservableCollection<NMK_M_User> value)
    {
      UserCount_CC = value.Count(x => x.IsChecked);
    }

    [ObservableProperty]
    int _UserCount_CC;


    public void Set(NMK_M_Task_Temporary task)
    {
      this.CreateAt = task.CreateAt;
      this.CreateBy = task.CreateBy;
      this.Id = task.Id;
      this.Name = task.Name;
      this.ProjectId = task.ProjectId;
      this.Project = task.Project;
      this.UserId = task.UserId;
      this.UpdateAt = task.UpdateAt;
      this.UpdateBy = task.UpdateBy;

      this.Time = task.Time;
      this.HH = task.HH;
      this.MM = task.MM;

      this.Status = task.Status;
      this.UserId_CC = task.UserId_CC;
      this.Week = task.Week;
      this.Year = task.Year;

      this.UserList_CC = task.UserList_CC;
      this.UserList = task.UserList;
      this.UserCount = task.UserCount;
      this.UserCount_CC = task.UserCount_CC;
      this.StatusColor = task.StatusColor;
      this.IsEdit = task.IsEdit;
    }

  }
}
