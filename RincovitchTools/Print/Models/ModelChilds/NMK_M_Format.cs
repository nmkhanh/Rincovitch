using Autodesk.Revit.DB;
using System.Collections.ObjectModel;
using System.Windows.Data;
using View = Autodesk.Revit.DB.View;

namespace RincovitchTools.Print.Models.ModelChilds
{
  public class NMK_M_Format : BaseViewModel
  {
    private bool _PDF = Properties.Settings.Default.PDF;
    public bool PDF
    {
      get { return _PDF; }
      set
      {
        _PDF = value;
        OnPropertyChanged();
      }
    }

    private bool _DWG = Properties.Settings.Default.DWG;
    public bool DWG
    {
      get { return _DWG; }
      set
      {
        _DWG = value;
        OnPropertyChanged();
      }
    }
  }
}
