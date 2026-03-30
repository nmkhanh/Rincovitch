using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using View = Autodesk.Revit.DB.View;

namespace MultiTool.CreateFloor.Models.Childs
{
  public class B_FloorType : BaseViewModel
  {
    private string _Name;
    public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }

    private FloorType _Type;
    public FloorType Type { get => _Type; set { _Type = value; OnPropertyChanged(); } }
  }
}
