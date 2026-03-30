using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Autodesk.Revit.DB;
using MultiTool.GroupFilter.Models.Childs;
using MultiTool.DisplaySettings.Models.Childs;

namespace MultiTool.GroupFilter.Models
{
  public class B_VM : BaseViewModel
  {
    private ObservableCollection<B_GroupType> _Type_s = new ObservableCollection<B_GroupType>();
    public ObservableCollection<B_GroupType> Type_s { get => _Type_s; set { _Type_s = value; OnPropertyChanged(); } }

    private ListCollectionView _Type_Sort;
    public ListCollectionView Type_Sort { get => _Type_Sort; set { _Type_Sort = value; OnPropertyChanged(); } }

    private B_GroupType _Type = new B_GroupType();
    public B_GroupType Type { get => _Type; set { _Type = value; OnPropertyChanged(); } }
  }
}
