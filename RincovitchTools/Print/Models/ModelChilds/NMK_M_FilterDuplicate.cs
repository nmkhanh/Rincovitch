using Autodesk.Revit.DB;
using System.Collections.ObjectModel;
using System.Windows.Data;
using View = Autodesk.Revit.DB.View;

namespace RincovitchTools.Print.Models.ModelChilds
{
  public class NMK_M_FilterDuplicate : BaseViewModel
  {
    private int _Count = 1;
    public int Count
    {
      get { return _Count; }
      set
      {
        _Count = value;
        OnPropertyChanged();
      }
    }
    private string _SheetNumberStart;
    public string SheetNumberStart
    {
      get { return _SheetNumberStart; }
      set
      {
        _SheetNumberStart = value;
        OnPropertyChanged();
      }
    }

    private bool _ByCurrentName = true;
    public bool ByCurrentName
    {
      get { return _ByCurrentName; }
      set
      {
        _ByCurrentName = value;
        OnPropertyChanged();
      }
    }

    private bool _ByCustomName;
    public bool ByCustomName
    {
      get { return _ByCustomName; }
      set
      {
        _ByCustomName = value;
        OnPropertyChanged();
      }
    }

    private string _CustomName;
    public string CustomName
    {
      get { return _CustomName; }
      set
      {
        _CustomName = value;
        OnPropertyChanged();
      }
    }

    private string _Separator;
    public string Separator
    {
      get { return _Separator; }
      set
      {
        _Separator = value;
        OnPropertyChanged();
      }
    }

    private string _Suffix;
    public string Suffix
    {
      get { return _Suffix; }
      set
      {
        _Suffix = value;
        OnPropertyChanged();
      }
    }

    private bool _IncludeAll = true;
    public bool IncludeAll
    {
      get { return _IncludeAll; }
      set
      {
        _IncludeAll = value;
        OnPropertyChanged();
      }
    }
  }
}
