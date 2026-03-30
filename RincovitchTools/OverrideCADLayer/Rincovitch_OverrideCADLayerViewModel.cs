using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit.Async;
using RincovitchTools._00_General;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace RincovitchTools.OverrideCADLayer
{
  public class Rincovitch_OverrideCADLayerViewModel : BaseViewModel
  {
    public ICommand SearchCommand { get; set; }
    public ICommand OverrideCommand { get; set; }


    public Rincovitch_OverrideCADLayerViewModel()
    {
      SearchCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        Search(p);
      });
    }


    void Search(object p)
    {
      try
      {

      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }
  }
}
