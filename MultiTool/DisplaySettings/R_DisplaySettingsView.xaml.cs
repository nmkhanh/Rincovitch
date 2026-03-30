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

namespace MultiTool.DisplaySettings
{
  /// <summary>
  /// Interaction logic for R_DisplaySettingsView.xaml
  /// </summary>
  public partial class R_DisplaySettingsView : Window
  {
    public R_DisplaySettingsViewModel ViewModel { get; set; }
    public R_DisplaySettingsView(UIApplication uiapp)
    {
      InitializeComponent();
      this.DataContext = ViewModel = new R_DisplaySettingsViewModel(uiapp);
    }
  }
}
