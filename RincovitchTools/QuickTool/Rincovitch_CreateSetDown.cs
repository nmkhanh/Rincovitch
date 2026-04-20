using System;
using System.Linq;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

[Transaction(TransactionMode.Manual)]
public class CreateFloorFromInplaceGroup : IExternalCommand
{
  public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
  {
    UIDocument uidoc = commandData.Application.ActiveUIDocument;
    Document doc = uidoc.Document;

    string floorTypeName = "RINCO_FL_SLB_300";
    double offsetMm = 300;
    double offset = UnitUtils.ConvertToInternalUnits(offsetMm, UnitTypeId.Millimeters);

    Reference pick = uidoc.Selection.PickObject(ObjectType.Element, "Pick group");
    Group group = doc.GetElement(pick) as Group;
    if (group == null) return Result.Failed;

    FloorType floorType = new FilteredElementCollector(doc)
        .OfClass(typeof(FloorType))
        .Cast<FloorType>()
        .FirstOrDefault(x => x.Name == floorTypeName);

    if (floorType == null)
    {
      Autodesk.Revit.UI.TaskDialog.Show("Error", $"FloorType not found: {floorTypeName}");
      return Result.Failed;
    }

    // ===== LEVEL từ Active View =====
    var levelName = doc.ActiveView.get_Parameter(BuiltInParameter.PLAN_VIEW_LEVEL)?.AsValueString();
    if (levelName == null) return Result.Failed;

    Level level = new FilteredElementCollector(doc)
        .OfClass(typeof(Level))
        .Cast<Level>()
        .FirstOrDefault(x => x.Name == levelName);

    if (level == null) return Result.Failed;

    using (Transaction t = new Transaction(doc, "Create Floor"))
    {
      t.Start();

      foreach (var id in group.GetMemberIds())
      {
        Element e = doc.GetElement(id);

        // chỉ Generic Model (inplace)
#if D2026
        if (e.Category == null || e.Category.Id.Value != (int)BuiltInCategory.OST_GenericModel)
#else
        if (e.Category == null || e.Category.Id.IntegerValue != (int)BuiltInCategory.OST_GenericModel)
#endif
          continue;

        Options opt = new Options()
        {
          ComputeReferences = true,
          DetailLevel = ViewDetailLevel.Fine
        };

        GeometryElement geo = e.get_Geometry(opt);

        foreach (GeometryObject obj in geo)
        {
          if (obj is Solid solid && solid.Volume > 0)
          {
            foreach (Face face in solid.Faces)
            {
              if (face is PlanarFace pf && pf.FaceNormal.IsAlmostEqualTo(XYZ.BasisZ))
              {
                IList<CurveLoop> loops = pf.GetEdgesAsCurveLoops();
                if (loops.Count == 0) continue;

                // ===== loop chính =====
                CurveLoop mainLoop = loops
                    .OrderByDescending(l => Math.Abs(GetArea(l)))
                    .First();

                try
                {
                  // ===== OUTER OFFSET =====
                  CurveLoop outerLoop = CurveLoop.CreateViaOffset(
                      mainLoop,
                      offset,
                      XYZ.BasisZ
                  );

                  // ===== INNER = loop gốc =====
                  CurveLoop innerLoop = mainLoop;

                  List<CurveLoop> profile = new List<CurveLoop>();

                  profile.Add(EnsureLoopOrientation(outerLoop, true));  // outer CCW
                  profile.Add(EnsureLoopOrientation(innerLoop, false)); // inner CW

                  Floor.Create(
                      doc,
                      profile,
                      floorType.Id,
                      level.Id
                  );
                }
                catch
                {
                  // skip lỗi offset
                }
              }
            }
          }
        }
      }

      t.Commit();
    }

    return Result.Succeeded;
  }

  // =============================
  // TÍNH DIỆN TÍCH LOOP
  // =============================
  private double GetArea(CurveLoop loop)
  {
    double area = 0;
    List<XYZ> pts = new List<XYZ>();

    foreach (Curve c in loop)
      pts.Add(c.GetEndPoint(0));

    for (int i = 0; i < pts.Count; i++)
    {
      XYZ p1 = pts[i];
      XYZ p2 = pts[(i + 1) % pts.Count];

      area += (p1.X * p2.Y - p2.X * p1.Y);
    }

    return area / 2.0;
  }

  // =============================
  // FIX HƯỚNG LOOP
  // =============================
  private CurveLoop EnsureLoopOrientation(CurveLoop loop, bool isCCW)
  {
    double area = GetArea(loop);
    bool isCurrentlyCCW = area > 0;

    if (isCCW != isCurrentlyCCW)
    {
      CurveLoop reversed = new CurveLoop();

      foreach (var c in loop.Reverse())
        reversed.Append(c.CreateReversed());

      return reversed;
    }

    return loop;
  }
}