using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using View = Autodesk.Revit.DB.View;

namespace RincovitchTools.Print.Models.ModelChilds
{
  public class NMK_M_SheetAndView_List : BaseViewModel
  {
    private string _Name;
    public string Name
    {
      get { return _Name; }
      set
      {
        _Name = value;
        OnPropertyChanged();
      }
    }

    private ObservableCollection<View> _ViewSheets = new ObservableCollection<View>();
    public ObservableCollection<View> ViewSheets
    {
      get { return _ViewSheets; }
      set
      {
        _ViewSheets = value;
        OnPropertyChanged();
      }
    }

    private int _Status;
    public int Status
    {
      get { return _Status; }
      set
      {
        _Status = value;
        OnPropertyChanged();
      }
    }
  }
}
