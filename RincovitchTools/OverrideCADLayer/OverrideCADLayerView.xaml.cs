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

namespace RincovitchTools.OverrideCADLayer
{
  /// <summary>
  /// Interaction logic for OverrideCADLayerView.xaml
  /// </summary>
  public partial class OverrideCADLayerView : Window
  {
    public Rincovitch_OverrideCADLayerViewModel VM { get; set; }
    public OverrideCADLayerView()
    {
      InitializeComponent();
      this.DataContext = VM = new Rincovitch_OverrideCADLayerViewModel();
    }
  }
}
