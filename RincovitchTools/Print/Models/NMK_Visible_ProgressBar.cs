using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RincovitchTools.Print.Models
{
  public class NMK_Visible_ProgressBar : BaseViewModel
  {
    private bool _Selection_Sheet;
    public bool Selection_Sheet
    {
      get { return _Selection_Sheet; }
      set
      {
        _Selection_Sheet = value;
        OnPropertyChanged();
      }
    }

    #region Settings

    #endregion
  }
}
