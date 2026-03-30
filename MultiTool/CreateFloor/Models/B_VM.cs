using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Autodesk.Revit.DB;
using MultiTool.CreateFloor.Models.Childs;
using MultiTool.DisplaySettings.Models.Childs;

namespace MultiTool.CreateFloor.Models
{
  public class B_VM : BaseViewModel
  {
    private ObservableCollection<B_FloorType> _FloorType_s = new ObservableCollection<B_FloorType>();
    public ObservableCollection<B_FloorType> FloorType_s { get => _FloorType_s; set { _FloorType_s = value; OnPropertyChanged(); } }

    private B_FloorType _FloorType = new B_FloorType();
    public B_FloorType FloorType { get => _FloorType; set { _FloorType = value; OnPropertyChanged(); } }

    private string _FloorName;
    public string FloorName { get => _FloorName; set { _FloorName = value; OnPropertyChanged(); } }

    private bool _TopFace = false;
    public bool TopFace { get => _TopFace; set { _TopFace = value; OnPropertyChanged(); } }
  }
}
