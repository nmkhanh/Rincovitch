using Autodesk.Revit.DB;
using System.Collections.ObjectModel;
using System.Windows;
using View = Autodesk.Revit.DB.View;

namespace RincovitchTools.Print.Models.ModelChilds
{
  public class NMK_M_FilterValue : BaseViewModel
  {
    string _Value;
    public string Value { get { return _Value; } set { _Value = value; OnPropertyChanged(); } }

    List<NMK_M_SheetAndView> _SheetViews = new List<NMK_M_SheetAndView>();
    public List<NMK_M_SheetAndView> SheetViews { get { return _SheetViews; } set { _SheetViews = value; OnPropertyChanged(); } }

    NMK_M_SheetAndView _SheetView = new NMK_M_SheetAndView();
    public NMK_M_SheetAndView SheetView { get { return _SheetView; } set { _SheetView = value; OnPropertyChanged(); } }
  }

  public class NMK_M_Filter : NMK_M_Parameter
  {
    public string FontWeight { get; set; }

    bool _IsSheet;
    public bool IsSheet { get { return _IsSheet; } set { _IsSheet = value; OnPropertyChanged(); } }

    ObservableCollection<NMK_M_FilterValue> _Values = new ObservableCollection<NMK_M_FilterValue>();
    public ObservableCollection<NMK_M_FilterValue> Values { get { return _Values; } set { _Values = value; OnPropertyChanged(); } }

    NMK_M_FilterValue _Value = new NMK_M_FilterValue();
    public NMK_M_FilterValue Value { get { return _Value; } set { _Value = value; OnPropertyChanged(); ValueText = Value.Value; } }

    bool _IsValueText;
    public bool IsValueText { get { return _IsValueText; } set { _IsValueText = value; OnPropertyChanged(); } }

    string _ValueText;
    public string ValueText { get { return _ValueText; } set { _ValueText = value; OnPropertyChanged(); } }

    ObservableCollection<string> _Rules = new()
    {
      "Equals",
      "Does not equal",
      "Contains",
      "Does not contain",
      "Begins with",
      "Does not begin with",
      "Ends with",
      "Does not end with"
    };
    public ObservableCollection<string> Rules { get { return _Rules; } set { _Rules = value; OnPropertyChanged(); } }

    string _Rule = "Equals";
    public string Rule { get { return _Rule; } set { _Rule = value; OnPropertyChanged(); IsValueText = !_Rule.ToLower().Contains("equal"); } }

    public NMK_M_Filter Clone()
    {
      return new NMK_M_Filter
      {
        Id = this.Id,
        Index = this.Index,
        Index_Old = this.Index_Old,
        IsChecked = this.IsChecked,
        Type = this.Type,
        Name = this.Name,
        Prefix = this.Prefix,
        Suffix = this.Suffix,
        Separator = this.Separator,
        ParameterInfo = this.ParameterInfo
      };
    }

    public void Clear()
    {
      this.Index = 0;
      this.Index_Old = 0;
      this.IsChecked = false;
      this.Prefix = string.Empty;
      this.Suffix = string.Empty;
      this.Separator = "-";
    }

    public void Load(NMK_M_Filter item)
    {
      if(item.Id != this.Id && item.Name != this.Name)
      {
        return;
      }
      this.Index = item.Index;
      this.Index_Old = item.Index_Old;
      this.IsChecked = item.IsChecked;
      this.Prefix = item.Prefix;
      this.Suffix = item.Suffix;
      this.Separator = item.Separator;
    }
  }
}
