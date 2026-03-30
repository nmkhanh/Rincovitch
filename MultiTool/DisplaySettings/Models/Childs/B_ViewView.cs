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
  public class B_ViewView : BaseViewModel
  {
    private B_View _ViewCurrent;
    public B_View ViewCurrent { get => _ViewCurrent; set { _ViewCurrent = value; OnPropertyChanged(); } }

    private B_View _ViewLink;
    public B_View ViewLink { get => _ViewLink; set { _ViewLink = value; OnPropertyChanged(); } }

    private ObservableCollection<B_View> _ViewLink_s = new ObservableCollection<B_View>();
    public ObservableCollection<B_View> ViewLink_s { get => _ViewLink_s; set { _ViewLink_s = value; OnPropertyChanged(); } }
  }
}
