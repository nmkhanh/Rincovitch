using Autodesk.Revit.DB;
using System.Collections.ObjectModel;
using System.Windows.Data;
using View = Autodesk.Revit.DB.View;

namespace RincovitchTools.Print.Models.ModelChilds
{
  public class NMK_M_Search : BaseViewModel
  {
    private string _ViewSheet;
    public string ViewSheet
    {
      get { return _ViewSheet; }
      set
      {
        _ViewSheet = value;
        OnPropertyChanged();
      }
    }

    private string _ParameterPDF;
    public string ParameterPDF
    {
      get { return _ParameterPDF; }
      set
      {
        _ParameterPDF = value;
        OnPropertyChanged();
      }
    }

    private string _ParameterDWG;
    public string ParameterDWG
    {
      get { return _ParameterDWG; }
      set
      {
        _ParameterDWG = value;
        OnPropertyChanged();
      }
    }

    private string _FilterSheet;
    public string FilterSheet
    {
      get { return _FilterSheet; }
      set
      {
        _FilterSheet = value;
        OnPropertyChanged();
      }
    }

    private string _FilterView;
    public string FilterView
    {
      get { return _FilterView; }
      set
      {
        _FilterView = value;
        OnPropertyChanged();
      }
    }
  }
}
