using Autodesk.Revit.DB;
using System.Collections.ObjectModel;
using System.Windows;
using View = Autodesk.Revit.DB.View;

namespace RincovitchTools.Print.Models.ModelChilds
{
  public class NMK_M_Parameter_Load : BaseViewModel
  {
    string _Name;
    public string Name { get { return _Name; } set { _Name = value; OnPropertyChanged(); } }

    bool _IsCurrent;
    public bool IsCurrent { get { return _IsCurrent; } set { _IsCurrent = value; OnPropertyChanged(); } }

    List<NMK_M_Parameter> _Datas = new List<NMK_M_Parameter>();
    public List<NMK_M_Parameter> Datas { get { return _Datas; } set { _Datas = value; OnPropertyChanged(); } }
  }

  public class NMK_M_Parameter : BaseViewModel
  {
    public string FontWeight { get; set; }

    string _Id;
    public string Id { get { return _Id; } set { _Id = value; OnPropertyChanged(); } }

    int _Index;
    public int Index { get { return _Index; } set { _Index = value; OnPropertyChanged(); } }

    int _Index_Old;
    public int Index_Old { get { return _Index_Old; } set { _Index_Old = value; OnPropertyChanged(); } }

    bool _IsChecked;
    public bool IsChecked { get { return _IsChecked; } set { _IsChecked = value; OnPropertyChanged(); } }

    Parameter _ParameterInfo;
    public Parameter ParameterInfo { get { return _ParameterInfo; } set { _ParameterInfo = value; OnPropertyChanged(); } }

    string _Type;
    public string Type { get { return _Type; } set { _Type = value; OnPropertyChanged(); } }

    string _Name;
    public string Name { get { return _Name; } set { _Name = value; OnPropertyChanged(); } }

    string _Prefix;
    public string Prefix { get { return _Prefix; } set { _Prefix = value; OnPropertyChanged(); } }

    string _Suffix;
    public string Suffix { get { return _Suffix; } set { _Suffix = value; OnPropertyChanged(); } }

    string _Separator = "-";
    public string Separator { get { return _Separator; } set { _Separator = value; OnPropertyChanged(); } }

    public NMK_M_Parameter Clone()
    {
      return new NMK_M_Parameter
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

    public NMK_M_Parameter Select()
    {
      return new NMK_M_Parameter
      {
        Id = this.Id,
        Index = this.Index,
        Index_Old = this.Index_Old,
        IsChecked = this.IsChecked,
        Name = this.Name,
        Prefix = this.Prefix,
        Suffix = this.Suffix,
        Separator = this.Separator,
      };
    }

    public NMK_M_Parameter SelectImport(NMK_M_Parameter parent)
    {
      return new NMK_M_Parameter
      {
        Id = this.Id,
        Index = this.Index,
        Index_Old = this.Index_Old,
        IsChecked = this.IsChecked,
        Prefix = this.Prefix,
        Suffix = this.Suffix,
        Separator = this.Separator,
        Type = parent.Type,
        Name = parent.Name,
        ParameterInfo = parent.ParameterInfo
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

    public void Load(NMK_M_Parameter item)
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
