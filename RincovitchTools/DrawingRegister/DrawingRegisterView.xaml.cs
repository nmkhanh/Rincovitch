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

namespace RincovitchTools.DrawingRegister
{
  /// <summary>
  /// Interaction logic for DrawingRegisterView.xaml
  /// </summary>
  public partial class DrawingRegisterView : Window
  {
    public Rincovitch_DrawingRegisterViewModel ViewModel { get; set; }
    public DrawingRegisterView()
    {
      InitializeComponent();
      this.DataContext = ViewModel = new Rincovitch_DrawingRegisterViewModel();
    }
  }
}
