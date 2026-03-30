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

namespace RincovitchTools.Print.Models

{
  public class NMK_M : BaseViewModel
  {
    private NMK_Visible_Window _IsVisible_Window = new ();
    public NMK_Visible_Window IsVisible_Window
    {
      get { return _IsVisible_Window; }
      set
      {
        _IsVisible_Window = value;
        OnPropertyChanged();
      }
    }

    private NMK_Visible_ProgressBar _IsVisible_ProgressBar = new ();
    public NMK_Visible_ProgressBar IsVisible_ProgressBar
    {
      get { return _IsVisible_ProgressBar; }
      set
      {
        _IsVisible_ProgressBar = value;
        OnPropertyChanged();
      }
    }

    private int _PrintTimeout = 120;
    public int PrintTimeout
    {
      get { return _PrintTimeout; }
      set
      {
        _PrintTimeout = value;
        OnPropertyChanged();
      }
    }


    private ObservableCollection<NMK_M_SheetAndView> _SelectionViews = new();
    public ObservableCollection<NMK_M_SheetAndView> SelectionViews
    {
      get { return _SelectionViews; }
      set
      {
        _SelectionViews = value;
        OnPropertyChanged();
        // Count actual view items in the tree (not just top-level folders)
        Settings_Print.ViewCount = CountViewItems(_SelectionViews);
        Settings_Print.UpdateSelectStatus();
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
        Settings_Print.SheetCount = CountViewItems(SelectionSheets);
        Settings_Print.UpdateSelectStatus();
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





    private ObservableCollection<NMK_M_SheetAndView> _SheetViews = new ();
    public ObservableCollection<NMK_M_SheetAndView> SheetViews
    {
      get { return _SheetViews; }
      set
      {
        _SheetViews = value;
        OnPropertyChanged();
      }
    }


    private NMK_M_Settings_PDF _Settings_PDF = new ();
    public NMK_M_Settings_PDF Settings_PDF
    {
      get { return _Settings_PDF; }
      set
      {
        _Settings_PDF = value;
        OnPropertyChanged();
      }
    }


    private NMK_M_Settings_DWG _Settings_DWG = new ();
    public NMK_M_Settings_DWG Settings_DWG
    {
      get { return _Settings_DWG; }
      set
      {
        _Settings_DWG = value;
        OnPropertyChanged();
      }
    }


    private NMK_M_Print _Settings_Print = new ();
    public NMK_M_Print Settings_Print
    {
      get { return _Settings_Print; }
      set
      {
        _Settings_Print = value;
        OnPropertyChanged();
      }
    }

    private NMK_M_Search _Search = new();
    public NMK_M_Search Search
    {
      get { return _Search; }
      set
      {
        _Search = value;
        OnPropertyChanged();
      }
    }

    private NMK_M_Format _Format = new ();
    public NMK_M_Format Format
    {
      get { return _Format; }
      set
      {
        _Format = value;
        OnPropertyChanged();
      }
    }



    #region Parameters
    private string _Search_ParameterPDF;
    public string Search_ParameterPDF
    {
      get { return _Search_ParameterPDF; }
      set
      {
        _Search_ParameterPDF = value;
        OnPropertyChanged();
        ViewSheetParameters_ListBox_PDF.Refresh();
      }
    }

    private string _Preview_ParameterPDF;
    public string Preview_ParameterPDF
    {
      get { return _Preview_ParameterPDF; }
      set
      {
        _Preview_ParameterPDF = value;
        OnPropertyChanged();
      }
    }

    private string _Search_ParameterDWG;
    public string Search_ParameterDWG
    {
      get { return _Search_ParameterDWG; }
      set
      {
        _Search_ParameterDWG = value;
        OnPropertyChanged();
        ViewSheetParameters_ListBox_DWG.Refresh();
      }
    }

    private string _Preview_ParameterDWG;
    public string Preview_ParameterDWG
    {
      get { return _Preview_ParameterDWG; }
      set
      {
        _Preview_ParameterDWG = value;
        OnPropertyChanged();
      }
    }

    private ObservableCollection<NMK_M_Parameter> _ViewSheetParameters_PDF = new ObservableCollection<NMK_M_Parameter>();
    public ObservableCollection<NMK_M_Parameter> ViewSheetParameters_PDF { get { return _ViewSheetParameters_PDF; } set { _ViewSheetParameters_PDF = value; OnPropertyChanged(); } }

    ListCollectionView _ViewSheetParameters_ListBox_PDF;
    public ListCollectionView ViewSheetParameters_ListBox_PDF { get { return _ViewSheetParameters_ListBox_PDF; } set { _ViewSheetParameters_ListBox_PDF = value; OnPropertyChanged(); } }

    ListCollectionView _ViewSheetParameters_DataGrid_PDF;
    public ListCollectionView ViewSheetParameters_DataGrid_PDF { get { return _ViewSheetParameters_DataGrid_PDF; } set { _ViewSheetParameters_DataGrid_PDF = value; OnPropertyChanged(); } }

    private ObservableCollection<NMK_M_Parameter> _ViewSheetParameters_DWG = new ObservableCollection<NMK_M_Parameter>();
    public ObservableCollection<NMK_M_Parameter> ViewSheetParameters_DWG { get { return _ViewSheetParameters_DWG; } set { _ViewSheetParameters_DWG = value; OnPropertyChanged(); } }

    ListCollectionView _ViewSheetParameters_ListBox_DWG;
    public ListCollectionView ViewSheetParameters_ListBox_DWG { get { return _ViewSheetParameters_ListBox_DWG; } set { _ViewSheetParameters_ListBox_DWG = value; OnPropertyChanged(); } }

    ListCollectionView _ViewSheetParameters_DataGrid_DWG;
    public ListCollectionView ViewSheetParameters_DataGrid_DWG { get { return _ViewSheetParameters_DataGrid_DWG; } set { _ViewSheetParameters_DataGrid_DWG = value; OnPropertyChanged(); } }

    private Brush _ForegroundCustomFileName = Brushes.White;
    public Brush ForegroundCustomFileName
    {
      get { return _ForegroundCustomFileName; }
      set
      {
        _ForegroundCustomFileName = value;
        OnPropertyChanged();
      }
    }

    private ObservableCollection<NMK_M_Parameter_Load> _ViewSheetParameters_PDF_Selects = new ObservableCollection<NMK_M_Parameter_Load>();
    public ObservableCollection<NMK_M_Parameter_Load> ViewSheetParameters_PDF_Selects { get { return _ViewSheetParameters_PDF_Selects; } set { _ViewSheetParameters_PDF_Selects = value; OnPropertyChanged(); } }

    private NMK_M_Parameter_Load _ViewSheetParameters_PDF_Select = new();
    public NMK_M_Parameter_Load ViewSheetParameters_PDF_Select { get { return _ViewSheetParameters_PDF_Select; } set { _ViewSheetParameters_PDF_Select = value; OnPropertyChanged(); } }

    private ObservableCollection<NMK_M_Parameter_Load> _ViewSheetParameters_DWG_Selects = new ObservableCollection<NMK_M_Parameter_Load>();
    public ObservableCollection<NMK_M_Parameter_Load> ViewSheetParameters_DWG_Selects { get { return _ViewSheetParameters_DWG_Selects; } set { _ViewSheetParameters_DWG_Selects = value; OnPropertyChanged(); } }

    private NMK_M_Parameter_Load _ViewSheetParameters_DWG_Select = new();
    public NMK_M_Parameter_Load ViewSheetParameters_DWG_Select { get { return _ViewSheetParameters_DWG_Select; } set { _ViewSheetParameters_DWG_Select = value; OnPropertyChanged(); } }

    #endregion

    #region Filters
    private string _Search_FilterSheet;
    public string Search_FilterSheet
    {
      get { return _Search_FilterSheet; }
      set
      {
        _Search_FilterSheet = value;
        OnPropertyChanged();
        ViewSheetFilters_ListBox_Sheet.Refresh();
      }
    }

    private string _Search_FilterView;
    public string Search_FilterView
    {
      get { return _Search_FilterView; }
      set
      {
        _Search_FilterView = value;
        OnPropertyChanged();
        ViewSheetFilters_ListBox_View.Refresh();
      }
    }

    private ObservableCollection<NMK_M_Filter> _ViewSheetFilters_Sheet = new ObservableCollection<NMK_M_Filter>();
    public ObservableCollection<NMK_M_Filter> ViewSheetFilters_Sheet { get { return _ViewSheetFilters_Sheet; } set { _ViewSheetFilters_Sheet = value; OnPropertyChanged(); } }

    ListCollectionView _ViewSheetFilters_ListBox_Sheet;
    public ListCollectionView ViewSheetFilters_ListBox_Sheet { get { return _ViewSheetFilters_ListBox_Sheet; } set { _ViewSheetFilters_ListBox_Sheet = value; OnPropertyChanged(); } }

    ListCollectionView _ViewSheetFilters_DataGrid_Sheet;
    public ListCollectionView ViewSheetFilters_DataGrid_Sheet { get { return _ViewSheetFilters_DataGrid_Sheet; } set { _ViewSheetFilters_DataGrid_Sheet = value; OnPropertyChanged(); } }

    private ObservableCollection<NMK_M_Filter> _ViewSheetFilters_View = new ObservableCollection<NMK_M_Filter>();
    public ObservableCollection<NMK_M_Filter> ViewSheetFilters_View { get { return _ViewSheetFilters_View; } set { _ViewSheetFilters_View = value; OnPropertyChanged(); } }

    ListCollectionView _ViewSheetFilters_ListBox_View;
    public ListCollectionView ViewSheetFilters_ListBox_View { get { return _ViewSheetFilters_ListBox_View; } set { _ViewSheetFilters_ListBox_View = value; OnPropertyChanged(); } }

    ListCollectionView _ViewSheetFilters_DataGrid_View;
    public ListCollectionView ViewSheetFilters_DataGrid_View { get { return _ViewSheetFilters_DataGrid_View; } set { _ViewSheetFilters_DataGrid_View = value; OnPropertyChanged(); } }


    NMK_M_FilterRename _ViewSheetFilters_Rename = new ();
    public NMK_M_FilterRename ViewSheetFilters_Rename { get { return _ViewSheetFilters_Rename; } set { _ViewSheetFilters_Rename = value; OnPropertyChanged(); } }

    NMK_M_FilterDuplicate _ViewSheetFilters_Duplicate = new ();
    public NMK_M_FilterDuplicate ViewSheetFilters_Duplicate { get { return _ViewSheetFilters_Duplicate; } set { _ViewSheetFilters_Duplicate = value; OnPropertyChanged(); } }


    private bool _Filter_ControlBySelect = true;
    public bool Filter_ControlBySelect
    {
      get { return _Filter_ControlBySelect; }
      set
      {
        _Filter_ControlBySelect = value;
        OnPropertyChanged();
      }
    }
    #endregion
  }
}
