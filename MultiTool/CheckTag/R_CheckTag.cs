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
using Color = Autodesk.Revit.DB.Color;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;
using View = Autodesk.Revit.DB.View;

namespace MultiTool.CheckTag
{
  [Transaction(TransactionMode.Manual)]
  internal class R_CheckTag : IExternalCommand
  {
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
      try
      {
        UIApplication uiapp = commandData.Application;
        UIDocument uidoc = uiapp.ActiveUIDocument;
        Document doc = uidoc.Document;

        run(doc);

        return Result.Succeeded;
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show(ex.Message);
        return Result.Failed;
      }
    }

    void run(Document doc)
    {
      try
      {
        using (Transaction t = new Transaction(doc, "Check Tag"))
        {
          t.Start();

          var elements = new FilteredElementCollector(doc, doc.ActiveView.Id)
            .WhereElementIsNotElementType()
            .Where(x => x.Category != null && x.Category.CategoryType == CategoryType.Model)
            .Where(x => x.Category.Name != "Detail Items")
            .ToList();

          var tags = new FilteredElementCollector(doc, doc.ActiveView.Id)
            .OfClass(typeof(IndependentTag))
            .Cast<IndependentTag>()
            .ToList();

          FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id);
          foreach (Element elem in collector)
          {
            doc.ActiveView.SetElementOverrides(elem.Id, new OverrideGraphicSettings());
          }

          FillPatternElement solidFill = new FilteredElementCollector(doc)
                .OfClass(typeof(FillPatternElement))
                .Cast<FillPatternElement>()
                .FirstOrDefault(x => x.GetFillPattern().IsSolidFill);

          Random rnd = new Random();
          foreach (var element in elements)
          {
            var linkedTag = tags.Where(x => x.GetTaggedLocalElementIds().First() == element.Id).ToList();

            byte r = (byte)rnd.Next(0, 256);
            byte g = (byte)rnd.Next(0, 256);
            byte b = (byte)rnd.Next(0, 256);

            Color randomColor = new Color(r, g, b);
            List<ElementId> ids = new List<ElementId>() { element.Id};
            ids.AddRange(linkedTag.Select(x => x.Id));
            foreach (ElementId id in ids)
            {

              OverrideGraphicSettings ogs = new OverrideGraphicSettings();
              ogs.SetSurfaceForegroundPatternColor(randomColor);
              ogs.SetProjectionLineColor(randomColor);
              ogs.SetSurfaceForegroundPatternId(solidFill.Id);
              //ogs.SetSurfaceTransparency(10);

              ogs.SetCutForegroundPatternColor(randomColor);
              ogs.SetCutLineColor(randomColor);
              ogs.SetCutForegroundPatternId(solidFill.Id);

              doc.ActiveView.SetElementOverrides(id, ogs);
            }
          }

          t.Commit();
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show(ex.Message);
      }
    }
  }
}
