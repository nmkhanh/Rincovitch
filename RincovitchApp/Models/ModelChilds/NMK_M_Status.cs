using RincovitchApp.API.Date;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RincovitchApp.Models.ModelChilds
{
  public class NMK_M_Status : BaseViewModel
  {
    string _Name;
    public string Name
    {
      get { return _Name; }
      set
      {
        _Name = value;
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

    string _Key;
    public string Key
    {
      get { return _Key; }
      set
      {
        _Key = value;
        OnPropertyChanged();
      }
    }

    int _State = 100;
    public int State
    {
      get { return _State; }
      set
      {
        _State = value;
        OnPropertyChanged();
      }
    }
  }
}
