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
  public class NMK_M_RoleVisible : BaseViewModel
  {
    Visibility _VisibleOnlyAdminApp = Visibility.Collapsed;
    public Visibility VisibleOnlyAdminApp
    {
      get
      {
        return _VisibleOnlyAdminApp;
      }
      set
      {
        _VisibleOnlyAdminApp = value;
        OnPropertyChanged();
      }
    }


    Visibility _VisibleUser = Visibility.Collapsed;
    public Visibility VisibleUser
    {
      get
      {
        return _VisibleUser;
      }
      set
      {
        _VisibleUser = value;
        OnPropertyChanged();
      }
    }

    Visibility _VisibleAdmin = Visibility.Collapsed;
    public Visibility VisibleAdmin
    {
      get
      {
        return _VisibleAdmin;
      }
      set
      {
        _VisibleAdmin = value;
        OnPropertyChanged();
      }
    }

    Visibility _VisibleLeader = Visibility.Collapsed;
    public Visibility VisibleLeader
    {
      get
      {
        return _VisibleLeader;
      }
      set
      {
        _VisibleLeader = value;
        OnPropertyChanged();
      }
    }

    Visibility _VisibleAdminApp = Visibility.Collapsed;
    public Visibility VisibleAdminApp
    {
      get
      {
        return _VisibleAdminApp;
      }
      set
      {
        _VisibleAdminApp = value;
        OnPropertyChanged();
      }
    }

    Visibility _VisibleMiddle = Visibility.Collapsed;
    public Visibility VisibleMiddle
    {
      get
      {
        return _VisibleMiddle;
      }
      set
      {
        _VisibleMiddle = value;
        OnPropertyChanged();
      }
    }
  }
}
