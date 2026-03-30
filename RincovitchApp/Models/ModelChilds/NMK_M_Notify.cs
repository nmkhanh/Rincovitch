using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RincovitchApp.Models.ModelChilds
{
  public class NMK_M_Notify : BaseViewModel
  {
    ObservableCollection<NMK_M_Notify> _Items = new ObservableCollection<NMK_M_Notify>();
    public ObservableCollection<NMK_M_Notify> Items
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

    string _TaskId;
    public string TaskId
    {
      get { return _TaskId; }
      set
      {
        _TaskId = value;
        OnPropertyChanged();
      }
    }

    string _Title;
    public string Title
    {
      get { return _Title; }
      set
      {
        _Title = value;
        OnPropertyChanged();
      }
    }

    bool _IsRead;
    public bool IsRead
    {
      get { return _IsRead; }
      set
      {
        _IsRead = value;
        OnPropertyChanged();
        FontWeight = IsRead ? FontWeights.Normal : FontWeights.Bold;
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

    string _Message;
    public string Message
    {
      get { return _Message; }
      set
      {
        _Message = value;
        OnPropertyChanged();
      }
    }

    FontWeight _FontWeight;
    public FontWeight FontWeight
    {
      get { return _FontWeight; }
      set
      {
        _FontWeight = value;
        OnPropertyChanged();
      }
    }

    public NMK_M_Notify Clone()
    {
      return new NMK_M_Notify
      {
        Id = this.Id,
        IsRead = this.IsRead,
        CreateAt = this.CreateAt,
        UpdateAt = this.UpdateAt,
        CreateBy = this.CreateBy,
        SendTo = this.SendTo,
        Message = this.Message,
        TaskId = this.TaskId,
        Title = this.Title
      };
    }
  }
}
