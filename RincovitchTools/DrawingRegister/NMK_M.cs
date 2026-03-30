using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using RincovitchTools.Print.Models.ModelChilds;
using System.Windows.Data;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace RincovitchTools.DrawingRegister

{
  public class NMK_M : BaseViewModel
  {
    private ObservableCollection<NMK_M_SheetAndView> _SelectionViews = new();
    public ObservableCollection<NMK_M_SheetAndView> SelectionViews
    {
      get { return _SelectionViews; }
      set
      {
        _SelectionViews = value;
        OnPropertyChanged();
      }
    }

    private int CountViewItems(IEnumerable<NMK_M_SheetAndView> nodes)
    {
      if (nodes == null) return 0;
      int count = 0;
      foreach (var n in nodes)
      {
        if (n == null) continue;
        // if node represents a view/sheet item (has a ViewSheet) count it
        if (n.IsItem != null)
          count++;
        // recurse children
        if (n.Items != null && n.Items.Count > 0)
          count += CountViewItems(n.Items);
      }
      return count;
    }

    private ObservableCollection<NMK_M_SheetAndView> _SelectionSheets = new ();
    public ObservableCollection<NMK_M_SheetAndView> SelectionSheets
    {
      get { return _SelectionSheets; }
      set
      {
        _SelectionSheets = value;
        OnPropertyChanged();
      }
    }

    private ObservableCollection<NMK_M_SheetAndView_List> _ListViewSheets = new ();
    public ObservableCollection<NMK_M_SheetAndView_List> ListViewSheets
    {
      get { return _ListViewSheets; }
      set
      {
        _ListViewSheets = value;
        OnPropertyChanged();
      }
    }

    private NMK_M_SheetAndView_List _ListViewSheet/* = new ()*/;
    public NMK_M_SheetAndView_List ListViewSheet
    {
      get { return _ListViewSheet; }
      set
      {
        _ListViewSheet = value;
        OnPropertyChanged();
      }
    }

    private string _ViewSheetSet_Name;
    public string ViewSheetSet_Name
    {
      get { return _ViewSheetSet_Name; }
      set
      {
        _ViewSheetSet_Name = value;
        OnPropertyChanged();
      }
    }

    private string _SearchViewSheet;
    public string SearchViewSheet
    {
      get { return _SearchViewSheet; }
      set
      {
        _SearchViewSheet = value;
        OnPropertyChanged();
      }
    }
  }
}
