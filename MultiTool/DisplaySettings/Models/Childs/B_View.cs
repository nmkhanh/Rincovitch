using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using View = Autodesk.Revit.DB.View;

namespace MultiTool.DisplaySettings.Models.Childs
{
  public class B_View : BaseViewModel
  {
    private string _Name;
    public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }

    private View _View;
    public View View { get => _View; set { _View = value; OnPropertyChanged(); } }
  }
}
