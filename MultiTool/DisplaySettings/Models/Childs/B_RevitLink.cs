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
  public class B_RevitLink : BaseViewModel
  {
    private string _Name;
    public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }

    private RevitLinkInstance _Instance;
    public RevitLinkInstance Instance { get => _Instance; set { _Instance = value; OnPropertyChanged(); } }

    private Document _Doc;
    public Document Doc { get => _Doc; set { _Doc = value; OnPropertyChanged(); } }

    private ObservableCollection<B_View> _Views = new ObservableCollection<B_View>();
    public ObservableCollection<B_View> Views { get => _Views; set { _Views = value; OnPropertyChanged(); } }
  }
}
