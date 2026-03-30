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

namespace MultiTool.CreateFloor
{
  [Transaction(TransactionMode.Manual)]
  internal class R_FilterFloor : IExternalCommand
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

        //R_DisplaySettingsView window = new R_DisplaySettingsView(uiapp);
        //window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        //window.Owner = revit;
        //window.Show();

        create(uidoc, doc);

        return Result.Succeeded;
      }
      catch(Exception)
      {
        return Result.Failed;
      }
    }

    static void create(UIDocument uidoc, Document doc)
    {
      try
      {
        Reference pickedRef = uidoc.Selection.PickObject(ObjectType.LinkedElement, "Chọn sàn (Floor) trong Revit Link");
        if(pickedRef == null)
          return;
        ElementId linkedFloorId = pickedRef.LinkedElementId;
        RevitLinkInstance linkInstance = doc.GetElement(pickedRef) as RevitLinkInstance;
        if(linkInstance == null)
          return;

        //var collector = new FilteredElementCollector(doc, doc.ActiveView.Id)
        //    .WhereElementIsNotElementType().ToElements().ToList();

        //var collector_link = new FilteredElementCollector(linkInstance.Document)
        //.WhereElementIsNotElementType().ToElements().ToList();
        List<string> toHide = new List<string>()
        {
          "GRASS",
          "EARTH",
          "DUCT",
          "CABLE TRAY",
        };
        var elemsFromRevitLinkInstance = new FilteredElementCollector(linkInstance.GetLinkDocument())
          .WhereElementIsNotElementType()
          .Where(x => x.Category != null && x.Category.Name == "Floors")
          .Where(x => toHide.Where(a => x.Name.Contains(a)).Count() > 0)
          .ToList();

        // Isolate them
        var refs = elemsFromRevitLinkInstance.Select(x => new Reference(x).CreateLinkReference(linkInstance))
            .ToList();
        //System.Windows.MessageBox.Show(toHide.Count().ToString());
        using(Transaction tx = new Transaction(doc, "Filter"))
        {
          tx.Start();
          //uidoc.Selection.SetReferences(refs);
          uidoc.Application.PostCommand(RevitCommandId.LookupPostableCommandId(PostableCommand.HideElements));
          tx.Commit();
        }

      }
      catch(Exception ex)
      {
        System.Windows.MessageBox.Show(ex.Message);
      }
    }


  }
}
