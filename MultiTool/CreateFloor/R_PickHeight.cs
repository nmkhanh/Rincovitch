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
  internal class R_PickHeight : IExternalCommand
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

        //create(uidoc, doc);

        using (TransactionGroup tr_g = new TransactionGroup(doc, "change"))
        {
          tr_g.Start();

          var floor_type = new FilteredElementCollector(doc).
            OfClass(typeof(FloorType)).
            Cast<FloorType>().
            ToList();
          var type_old = floor_type.Where(x => x.Name.Contains("SUSPENDED")).ToList();
          var type_new = floor_type.Where(x => x.Name.Contains("RINCO_FL_SLB_")).ToList();
          foreach (var t in type_old)
          {
            var th = new string(t.Name.Where(char.IsDigit).ToArray());
            if (!type_new.Select(x => x.Name).Contains($"RINCO_FL_SLB_{th}"))
            {
              DuplicateFloorTypeAndChangeThickness(doc, type_new.First(), Convert.ToDouble(th), 0, $"RINCO_FL_SLB_{th}");
            }
          }
          using (Transaction tr = new Transaction(doc, "change"))
          {
            tr.Start();
            var floor_type_ = new FilteredElementCollector(doc).
            OfClass(typeof(FloorType)).
            Cast<FloorType>().
            ToList();
            var type_old_ = floor_type_.Where(x => x.Name.Contains("SUSPENDED")).ToList();
            var type_new_ = floor_type_.Where(x => x.Name.Contains("RINCO_FL_SLB_")).ToList();

            var floor = new FilteredElementCollector(doc).
            OfClass(typeof(Floor)).
            Cast<Floor>().
            ToList();
            foreach (var f in floor.Where(x => x.FloorType.Name.Contains("SUSPENDED")).ToList().GroupBy(x => x.FloorType.Name))
            {
              var th = new string(f.Key.Where(char.IsDigit).ToArray());
              var type = type_new_.First(x => x.Name == $"RINCO_FL_SLB_{th}").Id;
              Element.ChangeTypeId(doc, f.Select(X => X.Id).ToList(), type);
            }

            tr.Commit();
          }

          tr_g.Assimilate();
        }
        ;

        return Result.Succeeded;
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show(ex.Message);
        return Result.Failed;
      }
    }

    FloorType DuplicateFloorTypeAndChangeThickness(Document doc, FloorType originalFloorType, double newLayerThickness, int layerIndexToModify, string newTypeName)
    {
      FloorType newFloorType = null;
      using (Transaction t = new Transaction(doc, "Duplicate Floor Type and Change Thickness"))
      {
        t.Start();

        // Step 1: Duplicate FloorType
        ElementType duplicatedType = originalFloorType.Duplicate(newTypeName);
        newFloorType = duplicatedType as FloorType;
        if (newFloorType == null)
        {
          t.RollBack();
          throw new InvalidOperationException("Duplicated element is not a FloorType");
        }

        // Step 2: Get CompoundStructure from the new type
        CompoundStructure cs = newFloorType.GetCompoundStructure();
        if (cs == null)
        {
          t.RollBack();
          throw new InvalidOperationException("FloorType has no CompoundStructure");
        }

        // Step 3: Modify the width (thickness) of a specific layer
        // layerIndexToModify: chỉ số của layer bạn muốn thay đổi (0-based). 
        // Bạn cần đảm bảo chỉ số hợp lệ (0 <= layerIndex < cs.LayerCount)
        if (layerIndexToModify < 0 || layerIndexToModify >= cs.LayerCount)
        {
          t.RollBack();
          throw new ArgumentOutOfRangeException(nameof(layerIndexToModify));
        }

        // Đổi đơn vị nếu cần: trong Revit, đơn vị nội bộ là feet (hoặc feet/thickness nội bộ)
        // Giả sử newLayerThickness đã là đơn vị nội bộ.
        double newThicknessFeet = F_Versions.mm(newLayerThickness);
        cs.SetLayerWidth(layerIndexToModify, newThicknessFeet);

        // Step 4: Set lại CompoundStructure đã chỉnh sửa vào type mới
        newFloorType.SetCompoundStructure(cs);

        t.Commit();
      }
      return newFloorType;
    }

    //static void create(UIDocument uidoc, Document doc)
    //{
    //  try
    //  {
    //    // Pick Floor trong Link
    //    Reference pickedRef = uidoc.Selection.PickObject(ObjectType.LinkedElement, "Chọn sàn (Floor) trong Revit Link");
    //    if (pickedRef == null)
    //      return;

    //    // Lấy RevitLinkInstance
    //    RevitLinkInstance linkInstance = doc.GetElement(pickedRef) as RevitLinkInstance;
    //    if (linkInstance == null)
    //      return;

    //    // Lấy Link Document
    //    Document linkDoc = linkInstance.GetLinkDocument();
    //    if (linkDoc == null)
    //      return;

    //    // Lấy Floor trong LinkDocument
    //    ElementId linkedFloorId = pickedRef.LinkedElementId;
    //    Floor floor = linkDoc.GetElement(linkedFloorId) as Floor;
    //    if (floor == null)
    //      return;

    //    // Lấy mặt top bằng HostObjectUtils nếu muốn
    //    IList<Reference> bottomRefs = null;

    //    bottomRefs = HostObjectUtils.GetBottomFaces(floor);

    //    PlanarFace bottomFace = null;
    //    IList<CurveLoop> loops_bt = null;

    //    if (bottomRefs != null && bottomRefs.Count > 0)
    //    {
    //      // Có top face references
    //      // Lấy mặt từ reference
    //      foreach (Reference rf in bottomRefs)
    //      {
    //        GeometryObject geoObj = floor.GetGeometryObjectFromReference(rf);
    //        PlanarFace pf = geoObj as PlanarFace;
    //        if (pf != null)
    //        {
    //          bottomFace = pf;
    //          break;
    //        }
    //      }
    //    }

    //    loops_bt = bottomFace.GetEdgesAsCurveLoops();

    //    double minZ_bt = Double.MaxValue;
    //    XYZ minPoint_bt = null;
    //    foreach (CurveLoop loop in loops_bt)
    //    {
    //      foreach (Curve c in loop)
    //      {
    //        // Endpoints
    //        XYZ p1 = c.GetEndPoint(0);
    //        XYZ p2 = c.GetEndPoint(1);

    //        if (p1.Z < minZ_bt)
    //        {
    //          minZ_bt = p1.Z;
    //          minPoint_bt = p1;
    //        }
    //        if (p2.Z < minZ_bt)
    //        {
    //          minZ_bt = p2.Z;
    //          minPoint_bt = p2;
    //        }
    //      }
    //    }

    //    IList<Reference> topRefs = null;

    //    topRefs = HostObjectUtils.GetTopFaces(floor);

    //    PlanarFace topFace = null;
    //    IList<CurveLoop> loops_top = null;

    //    if (topRefs != null && topRefs.Count > 0)
    //    {
    //      // Có top face references
    //      // Lấy mặt từ reference
    //      foreach (Reference rf in topRefs)
    //      {
    //        GeometryObject geoObj = floor.GetGeometryObjectFromReference(rf);
    //        PlanarFace pf = geoObj as PlanarFace;
    //        if (pf != null)
    //        {
    //          topFace = pf;
    //          break;
    //        }
    //      }
    //    }

    //    loops_top = bottomFace.GetEdgesAsCurveLoops();
    //    double minZ_top = Double.MaxValue;
    //    XYZ minPoint_top = null;
    //    foreach (CurveLoop loop in loops_top)
    //    {
    //      foreach (Curve c in loop)
    //      {
    //        // Endpoints
    //        XYZ p1 = c.GetEndPoint(0);
    //        XYZ p2 = c.GetEndPoint(1);

    //        if (p1.Z < minZ_top)
    //        {
    //          minZ_top = p1.Z;
    //          minPoint_top = p1;
    //        }
    //        if (p2.Z < minZ_top)
    //        {
    //          minZ_top = p2.Z;
    //          minPoint_top = p2;
    //        }
    //      }
    //    }

    //    System.Windows.MessageBox.Show($"{(minPoint_top.Z - minPoint_bt.Z) * 304.8}");
    //  }
    //  catch (Exception ex)
    //  {
    //    System.Windows.MessageBox.Show(ex.Message);
    //  }
    //}


  }
}
