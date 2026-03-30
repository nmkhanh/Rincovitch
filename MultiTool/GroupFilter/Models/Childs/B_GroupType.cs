using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using View = Autodesk.Revit.DB.View;

namespace MultiTool.GroupFilter.Models.Childs
{
  public class B_GroupType : BaseViewModel
  {
    private System.Windows.Media.Brush _BG;
    public System.Windows.Media.Brush BG { get => _BG; set { _BG = value; OnPropertyChanged(); } }

    private string _Name;
    public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }

    private int _Count;
    public int Count { get => _Count; set { _Count = value; OnPropertyChanged(); } }

    private GroupType _Type;
    public GroupType Type { get => _Type; set { _Type = value; OnPropertyChanged(); } }
  }
}
