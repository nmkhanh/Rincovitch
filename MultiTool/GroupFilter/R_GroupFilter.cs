using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Revit.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;
using View = Autodesk.Revit.DB.View;

namespace MultiTool.GroupFilter
{
  [Transaction(TransactionMode.Manual)]
  internal class R_GroupFilter : IExternalCommand
  {
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
      try
      {
        UIApplication uiapp = commandData.Application;
        UIDocument uidoc = uiapp.ActiveUIDocument;
        Document doc = uidoc.Document;

        var hwndSource = HwndSource.FromHwnd(uiapp.MainWindowHandle);
        Window? revit = hwndSource.RootVisual as Window;

        RevitTask.Initialize(uiapp);

        R_GroupFilterView window = new R_GroupFilterView(uiapp);
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        window.Owner = revit;
        window.Show();

        return Result.Succeeded;
      }
      catch(Exception)
      {
        return Result.Failed;
      }
    }

    


  }
}
