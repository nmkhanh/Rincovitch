using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Autodesk.Revit.UI;
using MultiTool.CreateFloor;

namespace MultiTool.CreateRebarDetail
{
  /// <summary>
  /// Interaction logic for R_CreateRebarDetailView.xaml
  /// </summary>
  public partial class R_CreateRebarDetailView : Window
  {
    public R_CreateRebarDetailViewModel ViewModel { get; set; }
    public R_CreateRebarDetailView(UIApplication uiapp)
    {
      InitializeComponent();
      this.DataContext = ViewModel = new R_CreateRebarDetailViewModel(uiapp);
    }
  }
}
