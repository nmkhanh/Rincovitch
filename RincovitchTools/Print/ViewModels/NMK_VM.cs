using RincovitchTools.Print.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RincovitchTools.Print.ViewModels
{
  public class NMK_VM : BaseViewModel
  {
    private Rincovitch_Print_SheetViewViewModel _SheetViewVM;
    public Rincovitch_Print_SheetViewViewModel SheetViewVM
    {
      get { return _SheetViewVM; }
      set
      {
        _SheetViewVM = value;
        OnPropertyChanged();
      }
    }

    private Rincovitch_Print_SettingsViewModel _SettingsVM;
    public Rincovitch_Print_SettingsViewModel SettingsVM
    {
      get { return _SettingsVM; }
      set
      {
        _SettingsVM = value;
        OnPropertyChanged();
      }
    }

    private Rincovitch_Print_ParameterViewModel _ParameterVM;
    public Rincovitch_Print_ParameterViewModel ParameterVM
    {
      get { return _ParameterVM; }
      set
      {
        _ParameterVM = value;
        OnPropertyChanged();
      }
    }

    private Rincovitch_Print_Filter _FilterVM;
    public Rincovitch_Print_Filter FilterVM
    {
      get { return _FilterVM; }
      set
      {
        _FilterVM = value;
        OnPropertyChanged();
      }
    }

    public NMK_VM()
    {
      
    }
  }
}