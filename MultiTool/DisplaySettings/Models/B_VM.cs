using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using MultiTool.DisplaySettings.Models.Childs;

namespace MultiTool.DisplaySettings.Models
{
  public class B_VM : BaseViewModel
  {
    private ObservableCollection<B_RevitLink> _Project_Link_s = new ObservableCollection<B_RevitLink>();
    public ObservableCollection<B_RevitLink> Project_Link_s { get => _Project_Link_s; set { _Project_Link_s = value; OnPropertyChanged(); } }

    private B_RevitLink _Project_Link = new B_RevitLink();
    public B_RevitLink Project_Link { get => _Project_Link; set { _Project_Link = value; OnPropertyChanged(); } }

    private ObservableCollection<B_ViewView> _Data_s = new ObservableCollection<B_ViewView>();
    public ObservableCollection<B_ViewView> Data_s { get => _Data_s; set { _Data_s = value; OnPropertyChanged(); } }

    private int _Data_Count ;
    public int Data_Count { get => _Data_Count; set { _Data_Count = value; OnPropertyChanged(); } }


    private ListCollectionView _Data_Sort_s;
    public ListCollectionView Data_Sort_s { get => _Data_Sort_s; set { _Data_Sort_s = value; OnPropertyChanged(); } }

    private string _Search;
    public string Search { get => _Search; set { _Search = value; OnPropertyChanged(); Data_Sort_s.Refresh(); } }
  }
}
