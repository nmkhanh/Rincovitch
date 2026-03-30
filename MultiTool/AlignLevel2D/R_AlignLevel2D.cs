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
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace MultiTool.AlignLevel2D
{
  public class FilterLevel : ISelectionFilter
  {
    public bool AllowElement(Element elem)
    {
      return elem is Level;
    }

    public bool AllowReference(Reference reference, XYZ position)
    {
      return false;
    }
  }

  [Transaction(TransactionMode.Manual)]
  internal class R_AlignLevel2D : IExternalCommand
  {
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
      try
      {
        UIApplication uiapp = commandData.Application;
        UIDocument uidoc = uiapp.ActiveUIDocument;
        Document doc = uidoc.Document;

        create(uidoc, doc);

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
        using (TransactionGroup txGroup = new TransactionGroup(doc, "Move Level 2D"))
        {
          txGroup.Start();
          using (Transaction tx = new Transaction(doc, "Move Level 2D"))
          {
            tx.Start();
            if (doc.ActiveView.get_Parameter(BuiltInParameter.VIEWER_CROP_REGION).AsInteger() == 1)
            {
              doc.ActiveView.get_Parameter(BuiltInParameter.VIEWER_CROP_REGION).Set(0);
            }
            tx.Commit();
          }

          using (Transaction tx = new Transaction(doc, "Move Level 2D"))
          {
            tx.Start();
            Selection selection = uidoc.Selection;
            var level_ = doc.GetElement(selection.PickObject(ObjectType.Element, new FilterLevel(), "Select level tenplate")) as Level;
            Line curve_ = level_.GetCurvesInView(DatumExtentType.ViewSpecific, doc.ActiveView).First() as Line;
            var length = curve_.Length;
            var dir = curve_.Direction;
            var levels = new FilteredElementCollector(doc, doc.ActiveView.Id)
              .OfClass(typeof(Level))
              .WhereElementIsNotElementType()
              .Cast<Level>()
              .ToList();


            foreach (var level in levels.Where(x => x.Name != level_.Name))
            {
              // Lấy đường datum (đường gốc) của level
              DatumExtentType extentType = DatumExtentType.ViewSpecific;

              // Đặt extent thành View-specific để thay đổi được trong view này
              level.SetDatumExtentType(DatumEnds.End0, doc.ActiveView, extentType);
              level.SetDatumExtentType(DatumEnds.End1, doc.ActiveView, extentType);

              Line curve = level.GetCurvesInView(DatumExtentType.ViewSpecific, doc.ActiveView).First() as Line;
              var z = curve.GetEndPoint(0).Z;

              // Thay đổi vị trí điểm cuối của level (2D)
              var start = new XYZ(curve_.GetEndPoint(0).X, curve_.GetEndPoint(0).Y, z);
              var end = start + dir.Multiply(length);
              //var lien_new = Line
              level.SetCurveInView(DatumExtentType.ViewSpecific, doc.ActiveView, Line.CreateBound(start, end));
            }

            tx.Commit();
          }

          using (Transaction tx = new Transaction(doc, "Move Level 2D"))
          {
            tx.Start();
            if (doc.ActiveView.get_Parameter(BuiltInParameter.VIEWER_CROP_REGION).AsInteger() == 0)
            {
              doc.ActiveView.get_Parameter(BuiltInParameter.VIEWER_CROP_REGION).Set(1);
            }
            tx.Commit();
          }
          txGroup.Assimilate();
        }
      }
      catch (Exception ex)
      {

        TaskDialog.Show("ERROR", ex.Message, TaskDialogCommonButtons.Ok);

      }
    }
  }
}
