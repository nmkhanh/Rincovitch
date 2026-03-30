using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using System.Reflection.Emit;
using View = Autodesk.Revit.DB.View;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;
using Revit.Async;
using System.Windows.Interop;
using System.Windows;

namespace MultiTool.DisplaySettings
{
  [Transaction(TransactionMode.Manual)]
  internal class R_DisplaySettings : IExternalCommand
  {
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
      try
      {
        UIApplication uiapp = commandData.Application;

        var hwndSource = HwndSource.FromHwnd(uiapp.MainWindowHandle);
        Window? revit = hwndSource.RootVisual as Window;

        RevitTask.Initialize(uiapp);

        R_DisplaySettingsView window = new R_DisplaySettingsView(uiapp);
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        window.Owner = revit;
        window.Show();

        return Result.Succeeded;
      }
      catch (Exception)
      {
        return Result.Failed;
      }
    }

    static void create(UIDocument uidoc, Document doc)
    {
      try
      {
        

      }
      catch (Exception ex)
      {

      }
    }

    
  }
}
