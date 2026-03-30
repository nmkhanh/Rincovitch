using System;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
using View = Autodesk.Revit.DB.View;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace RincovitchTools.Print.Models.ModelChilds
{
  public class NMK_M_SheetAndView : BaseViewModel
  {
    #region Filter
    private bool _isVisible = true;
    public bool IsVisible
    {
      get => _isVisible;
      set
      {
        _isVisible = value;
        OnPropertyChanged();
      }
    }

    public bool Filter(string keyword)
    {
      bool match = string.IsNullOrWhiteSpace(keyword) || (Name?.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);

      bool childMatch = false;
      foreach (var c in Items)
      {
        childMatch |= c.Filter(keyword);
      }

      IsVisible = match || childMatch;
      return IsVisible;
    }
    #endregion

    bool? _isChecked = false;
    public bool? IsChecked
    {
      get { return _isChecked; }
      set
      {
        _isChecked = value;
        OnPropertyChanged();
      }
    }

    string _name = string.Empty;
    public string Name
    {
      get { return _name; }
      set
      {
        _name = value;
        OnPropertyChanged();
      }
    }

    int _Count;
    public int Count
    {
      get { return _Count; }
      set
      {
        _Count = value;
        OnPropertyChanged();
      }
    }

    int _CountTotal;
    public int CountTotal
    {
      get { return _CountTotal; }
      set
      {
        _CountTotal = value;
        OnPropertyChanged();
      }
    }

    public string NameDefault { get; set; } = string.Empty;

    ObservableCollection<NMK_M_SheetAndView> _items = new ObservableCollection<NMK_M_SheetAndView>();
    public ObservableCollection<NMK_M_SheetAndView> Items
    {
      get { return _items; }
      set
      {
        _items = value;
        OnPropertyChanged();
      }
    }

    bool _isSheet;
    public bool IsSheet
    {
      get { return _isSheet; }
      set
      {
        _isSheet = value;
        OnPropertyChanged();
      }
    }

    bool _isItem;
    public bool IsItem
    {
      get { return _isItem; }
      set
      {
        _isItem = value;
        OnPropertyChanged();
      }
    }

    bool _IsCustomName = false;
    public bool IsCustomName
    {
      get { return _IsCustomName; }
      set
      {
        _IsCustomName = value;
        OnPropertyChanged();
      }
    }

    string _Error = string.Empty;
    public string Error
    {
      get { return _Error; }
      set
      {
        _Error = value;
        OnPropertyChanged();
        if(!string.IsNullOrEmpty(_Error))
          Foreground = Brushes.OrangeRed;
        else
          Foreground = Brushes.LightGreen;
      }
    }

    private Brush _Foreground = Brushes.LightGreen;
    public Brush Foreground
    {
      get { return _Foreground; }
      set
      {
        _Foreground = value;
        OnPropertyChanged();
      }
    }

    string _ID;
    public string ID
    {
      get { return _ID; }
      set
      {
        _ID = value;
        OnPropertyChanged();
      }
    }

    #region ViewSheet
    FamilyInstance _titleBlock;
    public FamilyInstance TitleBlock
    {
      get { return _titleBlock; }
      set
      {
        _titleBlock = value;
        OnPropertyChanged();
      }
    }

    List<RevisionCloud> _RevisionCloud;
    public List<RevisionCloud> RevisionCloud
    {
      get { return _RevisionCloud; }
      set
      {
        _RevisionCloud = value;
        OnPropertyChanged();
      }
    }

    int _Index;
    public int Index
    {
      get { return _Index; }
      set
      {
        _Index = value;
        OnPropertyChanged();
      }
    }
    
    string _SheetNumber = "-";
    public string SheetNumber
    {
      get { return _SheetNumber; }
      set
      {
        _SheetNumber = value;
        OnPropertyChanged();
      }
    }
    
    string _SheetName = "-";
    public string SheetName
    {
      get { return _SheetName; }
      set
      {
        _SheetName = value;
        OnPropertyChanged();
      }
    }
    
    string _Revision = "-";
    public string Revision
    {
      get { return _Revision; }
      set
      {
        _Revision = value;
        OnPropertyChanged();
      }
    }
    string _RevisionDate = "-";
    public string RevisionDate
    {
      get { return _RevisionDate; }
      set
      {
        _RevisionDate = value;
        OnPropertyChanged();
      }
    }
    
    string _Size = "-";
    public string Size
    {
      get { return _Size; }
      set
      {
        _Size = value;
        OnPropertyChanged();
      }
    }

    double _Size_W = 0;
    public double Size_W
    {
      get { return _Size_W; }
      set
      {
        _Size_W = value;
        OnPropertyChanged();
      }
    }
    double _Size_H = 0;
    public double Size_H
    {
      get { return _Size_H; }
      set
      {
        _Size_H = value;
        OnPropertyChanged();
      }
    }

    string _Format = "-";
    public string Format
    {
      get { return _Format; }
      set
      {
        _Format = value;
        OnPropertyChanged();
      }
    }
    
    string _Orientation = "-";
    public string Orientation
    {
      get { return _Orientation; }
      set
      {
        _Orientation = value;
        OnPropertyChanged();
      }
    }
    
    double _ProgressValue;
    public double ProgressValue
    {
      get { return _ProgressValue; }
      set
      {
        _ProgressValue = value;
        OnPropertyChanged();
      }
    }
    
    bool _ProgressVisible = true;
    public bool ProgressVisible
    {
      get { return _ProgressVisible; }
      set
      {
        _ProgressVisible = value;
        OnPropertyChanged();
      }
    }
    #endregion

    #region Sheet
    View _viewSheet;
    public View ViewSheet
    {
      get { return _viewSheet; }
      set
      {
        _viewSheet = value;
        OnPropertyChanged();
      }
    }
    #endregion
  }
}
