using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RincovitchTools.Print.Models
{
  public class NMK_Visible_Window : BaseViewModel
  {
    private bool _IsVisible;
    public bool IsVisible
    {
      get { return _IsVisible; }
      set
      {
        _IsVisible = value;
        OnPropertyChanged();
      }
    }

    private bool _Selection = true;
    public bool Selection
    {
      get { return _Selection; }
      set
      {
        _Selection = value;
        OnPropertyChanged();
      }
    }

    private bool  _Settings;
    public bool Settings
    {
      get { return _Settings; }
      set
      {
        _Settings = value;
        OnPropertyChanged();
      }
    }

    private bool _Filter;
    public bool Filter
    {
      get { return _Filter; }
      set
      {
        _Filter = value;
        OnPropertyChanged();
      }
    }

    #region Selection
    private bool _Selection_IsSheet = true;
    public bool Selection_IsSheet
    {
      get { return _Selection_IsSheet; }
      set
      {
        _Selection_IsSheet = value;
        OnPropertyChanged();
      }
    }

    private bool _Selection_IsView = false;
    public bool Selection_IsView
    {
      get { return _Selection_IsView; }
      set
      {
        _Selection_IsView = value;
        OnPropertyChanged();
      }
    }
    #endregion

    #region Settings
    private bool _Settings_PDF = true;
    public bool Settings_PDF
    {
      get { return _Settings_PDF; }
      set
      {
        _Settings_PDF = value;
        OnPropertyChanged();
      }
    }
    private bool _Settings_DWG;
    public bool Settings_DWG
    {
      get { return _Settings_DWG; }
      set
      {
        _Settings_DWG = value;
        OnPropertyChanged();
      }
    }
    #endregion

    #region Dialog
    private bool _Dialog;
    public bool Dialog
    {
      get { return _Dialog; }
      set
      {
        _Dialog = value;
        OnPropertyChanged();
      }
    }
    private bool _Dialog_Parameter = false;
    public bool Dialog_Parameter
    {
      get { return _Dialog_Parameter; }
      set
      {
        _Dialog_Parameter = value;
        OnPropertyChanged();
      }
    }

    private bool _Dialog_Parameter_PDF = true;
    public bool Dialog_Parameter_PDF
    {
      get { return _Dialog_Parameter_PDF; }
      set
      {
        _Dialog_Parameter_PDF = value;
        OnPropertyChanged();
      }
    }

    private bool _Dialog_Parameter_DWG = false;
    public bool Dialog_Parameter_DWG
    {
      get { return _Dialog_Parameter_DWG; }
      set
      {
        _Dialog_Parameter_DWG = value;
        OnPropertyChanged();
      }
    }
    #endregion
  }
}
