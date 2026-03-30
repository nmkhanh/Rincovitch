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

namespace MultiTool.CreateFloor
{
  /// <summary>
  /// Interaction logic for R_CreateFloorView.xaml
  /// </summary>
  public partial class R_CreateFloorView : Window
  {
    public R_CreateFloorViewModel ViewModel { get; set; }
    public R_CreateFloorView(UIApplication uiapp)
    {
      InitializeComponent();
      this.DataContext = ViewModel = new R_CreateFloorViewModel(uiapp);
    }

    private void CreateFloorButton_Click(object sender, RoutedEventArgs e)
    {

    }
  }
}
