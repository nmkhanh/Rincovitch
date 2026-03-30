using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Revit.Async;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace MultiTool
{
  [Transaction(TransactionMode.Manual)]
  internal class R_ : IExternalCommand
  {
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
      try
      {
        UIApplication uiapp = commandData.Application;
        UIDocument uidoc = uiapp.ActiveUIDocument;
        Document doc = uidoc.Document;

        //var hwndSource = HwndSource.FromHwnd(uiapp.MainWindowHandle);
        //Window? revit = hwndSource.RootVisual as Window;

        //RevitTask.Initialize(uiapp);

        //R_View window = new R_View(uiapp);
        //window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        //window.Owner = revit;
        //window.Show();


        using (Transaction t = new Transaction(doc, "Change Tag Reference"))
        {
          t.Start();

          #region revision
          //var T = new FilteredElementCollector(doc)
          //  .OfClass(typeof(ViewSheet))
          //  .Cast<ViewSheet>()
          //  .ToList();
          //foreach (var sheet in T)
          //{
          //  if (sheet.get_Parameter(BuiltInParameter.SHEET_CURRENT_REVISION).AsValueString() == "A")
          //  {
          //    var T_ = new FilteredElementCollector(doc, sheet.Id)
          //      .OfCategory(BuiltInCategory.OST_TitleBlocks)
          //  .OfClass(typeof(FamilyInstance))
          //  .Cast<FamilyInstance>()
          //  .First();
          //    T_.LookupParameter("STAMP_Construction Issue").Set(1);
          //    T_.LookupParameter("STAMP_Not For Construction").Set(0);
          //  }
          //}
          #endregion

          #region step
          var fm_step = new FilteredElementCollector(doc, doc.ActiveView.Id)
            .OfClass(typeof(FamilyInstance))
            .OfCategory(BuiltInCategory.OST_GenericAnnotation)
            .Cast<FamilyInstance>()
            .Where(x => x.Symbol.Name == "RINCO_AN_Step")
            .ToList();

          var floors = new FilteredElementCollector(doc, doc.ActiveView.Id)
            .OfClass(typeof(Floor))
            .OfCategory(BuiltInCategory.OST_Floors)
            .Cast<Floor>()
            .ToList();

          foreach (var item in fm_step)
          {
            var step_point = (item.Location as LocationPoint).Point;
            var step_floors = GetFloorsContainingPoint(floors, step_point);
            if(step_floors.Count() > 1)
            {
              Floor floor_1 = step_floors.First();
              Floor floor_2 = step_floors.Last();
              string value = Math.Abs(Convert.ToInt16(floor_1.get_Parameter(BuiltInParameter.STRUCTURAL_ELEVATION_AT_TOP).AsValueString()) -
                Convert.ToInt16(floor_2.get_Parameter(BuiltInParameter.STRUCTURAL_ELEVATION_AT_TOP).AsValueString())).ToString();
              item.LookupParameter("RL STEP").Set(value);
            }
            else
            {

            }
          }
          #endregion

          t.Commit();
        }

        #region tag
        //var pick_tag = uidoc.Selection.PickObjects(ObjectType.Element, new Filter_Tags(), "Select a tag");
        //if (pick_tag == null || pick_tag.Count == 0)
        //  return Result.Cancelled;
        //var pick_floor = uidoc.Selection.PickObject(ObjectType.Element, new Filter_Floor(), "Select a floor");
        //if (pick_floor == null)
        //  return Result.Cancelled;

        //using (Transaction t = new Transaction(doc, "Change Tag Reference"))
        //{
        //  t.Start();
        //  var floor = doc.GetElement(pick_floor.ElementId) as Floor;
        //  foreach (var item in pick_tag)
        //  {
        //    var oldTag = doc.GetElement(item.ElementId) as IndependentTag;

        //    // Lấy thông tin từ tag cũ
        //    ElementId viewId = oldTag.OwnerViewId;
        //    View view = doc.GetElement(viewId) as View;
        //    XYZ headPos = oldTag.TagHeadPosition;
        //    TagOrientation orientation = oldTag.TagOrientation;
        //    ElementId oldTypeId = oldTag.GetTypeId();
        //    bool hasLeader = oldTag.HasLeader;
        //    bool isMaterialTag = oldTag.IsMaterialTag;
        //    LeaderEndCondition endCond = oldTag.LeaderEndCondition;
        //    var oldAngle = oldTag.RotationAngle;

        //    // Lấy thông tin leader (end & elbow) – nếu có
        //    var refs = oldTag.GetTaggedReferences().ToList();
        //    Reference singleRef = refs.Count() > 0 ? refs[0] : null;

        //    XYZ oldLeaderEnd = null;
        //    XYZ oldLeaderElbow = null;

        //    if (hasLeader && singleRef != null)
        //    {
        //      try
        //      {
        //        oldLeaderEnd = oldTag.GetLeaderEnd(singleRef);
        //      }
        //      catch { }

        //      try
        //      {
        //        oldLeaderElbow = oldTag.GetLeaderElbow(singleRef);
        //      }
        //      catch { }
        //    }

        //    // Xoá tag cũ
        //    doc.Delete(oldTag.Id);

        //    // Tạo tag mới với reference mới
        //    TagMode mode = isMaterialTag ? TagMode.TM_ADDBY_MATERIAL : TagMode.TM_ADDBY_CATEGORY;
        //    IndependentTag newTag = IndependentTag.Create(
        //        doc,
        //        viewId,
        //        new Reference(floor),
        //        hasLeader,
        //        mode,
        //        orientation,
        //        headPos
        //    );

        //    // Giữ lại kiểu tag (family type)
        //    newTag.ChangeTypeId(oldTypeId);

        //    // Thiết lập lại leader nếu có
        //    if (hasLeader && singleRef != null)
        //    {
        //      try
        //      {
        //        if (oldLeaderEnd != null)
        //          newTag.SetLeaderEnd(singleRef, oldLeaderEnd);
        //      }
        //      catch { }

        //      try
        //      {
        //        if (oldLeaderElbow != null)
        //          newTag.SetLeaderElbow(singleRef, oldLeaderElbow);
        //      }
        //      catch { }

        //      try
        //      {
        //        newTag.LeaderEndCondition = endCond;
        //      }
        //      catch { }
        //    }
        //    if (oldAngle != 0)
        //    {
        //      newTag.TagOrientation = TagOrientation.AnyModelDirection;
        //      newTag.RotationAngle = oldAngle;
        //    }

        //  }
        //  t.Commit();
        //}
        #endregion

        return Result.Succeeded;
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show(ex.Message);
        return Result.Failed;
      }
    }

    public List<Floor> GetFloorsContainingPoint(List<Floor> floors, XYZ pt)
    {
      List<Floor> result = new List<Floor>();
      double tolerance = 1e-6;

      try
      {
        foreach (Floor floor in floors)
        {
          Options opt = new Options();
          GeometryElement geo = floor.get_Geometry(opt);

          foreach (GeometryObject obj in geo)
          {
            Solid solid = obj as Solid;
            if (solid == null || solid.Faces.Size == 0) continue;

            foreach (Face face in solid.Faces)
            {
              // Chỉ lấy mặt top
              if (!IsTopFace(face)) continue;

              // Chiếu điểm xuống mặt
              IntersectionResult ir = face.Project(pt);
              if (ir == null) continue;

              UV uv = ir.UVPoint;

              // Kiểm tra xem UV có nằm trong boundary
              if (face.IsInside(uv))
              {
                result.Add(floor);
                break;
              }
            }
          }
        }
      }
      catch (Exception ex) 
      {
        System.Windows.MessageBox.Show(ex.Message);
      }

      return result;
    }
    private bool IsTopFace(Face face)
    {
      PlanarFace pf = face as PlanarFace;
      if (pf == null) return false;

      XYZ normal = pf.FaceNormal;

      // Top face có normal hướng +Z
      return normal.IsAlmostEqualTo(XYZ.BasisZ);
    }
  }
}
