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
  public class NMK_M_Version : BaseViewModel
  {
    DateTime _CreateAt;
    public DateTime CreateAt
    {
      get { return _CreateAt; }
      set
      {
        _CreateAt = value;
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

    string _Version;
    public string Version
    {
      get { return _Version; }
      set
      {
        _Version = value;
        OnPropertyChanged();
      }
    }

    string _Data;
    public string Data
    {
      get { return _Data; }
      set
      {
        _Data = value;
        OnPropertyChanged();
      }
    }

    public NMK_Supabase_Version Supabase_Version;
  }
}
