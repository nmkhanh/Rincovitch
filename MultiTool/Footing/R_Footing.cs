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
using System.Windows.Forms;
using System.Diagnostics;

namespace MultiTool.Footing
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
  internal class R_Footing : IExternalCommand
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
        using (TransactionGroup txGroup = new TransactionGroup(doc, "Footing"))
        {
          txGroup.Start();

          using (Transaction tx = new Transaction(doc, "Footing"))
          {
            tx.Start();
            GroupType groupType = new FilteredElementCollector(doc)
                .OfClass(typeof(GroupType))
                .Cast<GroupType>()
                .FirstOrDefault(x => x.Name.Equals("PF1"));

            FilteredElementCollector colCollector = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .OfCategory(BuiltInCategory.OST_StructuralColumns)
                .WhereElementIsNotElementType();
            List<FamilyInstance> columns = new List<FamilyInstance>();
            foreach (Element e in colCollector)
            {
              FamilyInstance fi = e as FamilyInstance;
              if (fi != null)
                columns.Add(fi);
            }

            foreach (FamilyInstance col in columns)
            {
              try
              {
                // Lấy điểm vị trí cột (nhiều cột có LocationPoint)
                LocationPoint locPt = col.Location as LocationPoint;
                if (locPt == null)
                {
                  Trace.WriteLine($"[CopyGroupToColumns] Cột {col.Id} không có LocationPoint, bỏ qua.");
                  continue;
                }

                XYZ basePoint = locPt.Point;

                // 1️⃣ Đặt group tại tâm cột
                Group group = doc.Create.PlaceGroup(basePoint, groupType);
                if (group == null)
                {
                  Trace.WriteLine($"Không thể tạo group tại cột {col.Id}");
                  continue;
                }

                // 2️⃣ Canh tâm group với cột (theo XY)
                BoundingBoxXYZ bbox = group.get_BoundingBox(null) ?? group.get_BoundingBox(null);
                if (bbox != null)
                {
                  XYZ groupCenter = (bbox.Min + bbox.Max) / 2.0;
                  XYZ moveVec = new XYZ(basePoint.X - groupCenter.X, basePoint.Y - groupCenter.Y, 0);
                  ElementTransformUtils.MoveElement(doc, group.Id, moveVec);
                }

                // 3️⃣ Tính góc xoay để cạnh dài group song song cạnh dài cột
                XYZ handDir = col.HandOrientation.Normalize(); // vector cạnh dài cột
                double angle = Math.Atan2(handDir.Y, handDir.X) + Math.PI / 2; // radian

                // 4️⃣ Xoay quanh trục Z tại tâm cột
                Line axis = Line.CreateBound(basePoint, basePoint + XYZ.BasisZ);
                ElementTransformUtils.RotateElement(doc, group.Id, axis, angle);
              }
              catch (Exception innerEx)
              {
                Trace.WriteLine($"[CopyGroupToColumns] Lỗi khi xử lý cột {col.Id}: {innerEx}");
                // tiếp tục cột tiếp theo
              }
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
