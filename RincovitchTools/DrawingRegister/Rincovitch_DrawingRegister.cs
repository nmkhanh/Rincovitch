using Autodesk.Revit. Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ClosedXML.Excel;
using Revit.Async;
using RincovitchTools.General.Revit;
using RincovitchTools.Print;
using RincovitchTools.Print.API.Revit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace RincovitchTools.DrawingRegister
{
  [Transaction(TransactionMode.Manual)]
  public class Rincovitch_DrawingRegister : IExternalCommand
  {
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
      try
      {
        UIApplication uiapp = commandData.Application;

        var hwndSource = HwndSource.FromHwnd(uiapp.MainWindowHandle);
        Window? revit = hwndSource.RootVisual as Window;

        RevitTask.Initialize(uiapp);

        DrawingRegisterView window = new DrawingRegisterView();
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        window.Owner = revit;
        window.Show();

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