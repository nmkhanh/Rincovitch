using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit.Async;
using RincovitchTools._00_General;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace RincovitchTools.Print
{
  [Transaction(TransactionMode.Manual)]
  internal class Revit_Print : IExternalCommand
  {
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
      try
      {
        UIApplication uiapp = commandData.Application;

        var hwndSource = HwndSource.FromHwnd(uiapp.MainWindowHandle);
        Window? revit = hwndSource.RootVisual as Window;

        RevitTask.Initialize(uiapp);

        Rincovitch_Print window = new Rincovitch_Print();
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        window.Owner = revit;

        return Result.Succeeded;
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return Result.Failed;
      }
    }
  }
}
