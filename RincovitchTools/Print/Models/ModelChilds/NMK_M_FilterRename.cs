using Autodesk.Revit.DB;
using System.Collections.ObjectModel;
using System.Windows.Data;
using View = Autodesk.Revit.DB.View;

namespace RincovitchTools.Print.Models.ModelChilds
{
  public class NMK_M_FilterRename : BaseViewModel
  {
    private bool _BySelect = true;
    public bool BySelect
    {
      get { return _BySelect; }
      set
      {
        _BySelect = value;
        OnPropertyChanged();
      }
    }

    private bool _ByAll;
    public bool ByAll
    {
      get { return _ByAll; }
      set
      {
        _ByAll = value;
        OnPropertyChanged();
      }
    }

    private string _From;
    public string From
    {
      get { return _From; }
      set
      {
        _From = value;
        OnPropertyChanged();
      }
    }

    private string _To;
    public string To
    {
      get { return _To; }
      set
      {
        _To = value;
        OnPropertyChanged();
      }
    }
  }
}
