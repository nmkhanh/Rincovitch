using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Diagnostics;

namespace RincovitchTools.QuickTool
{
  [Transaction(TransactionMode.Manual)]
  public class Rincovitch_SplitSlab : IExternalCommand
  {
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
      UIDocument uidoc = commandData.Application.ActiveUIDocument;
      Document doc = uidoc.Document;

      View3D view3D = doc.ActiveView as View3D;

      if (view3D == null || !view3D.IsSectionBoxActive)
      {
        Debug.Print("View hiện tại không phải 3D hoặc chưa bật SectionBox");
        return Result.Failed;
      }

      // =========================
      // 🔹 CREATE SOLID FROM SECTION BOX
      // =========================
      var selectedIds = uidoc.Selection.GetElementIds();
      var collector_line = new FilteredElementCollector(doc, selectedIds)
          .WhereElementIsNotElementType()
          .Cast<ModelLine>()
          .Select(x => x.GeometryCurve as Line)
          .ToList();
      Solid sectionSolid = CreateSolidFromLoopLine(view3D, collector_line);

      // =========================
      // 🔹 FILTER ELEMENTS INTERSECT SECTION BOX
      // =========================
      var collector = new FilteredElementCollector(doc)
        .OfCategory(BuiltInCategory.OST_Floors)
          .WhereElementIsNotElementType()
          .WherePasses(new ElementIntersectsSolidFilter(sectionSolid));

      using (Transaction tx = new Transaction(doc, "Solids in SectionBox"))
      {
        tx.Start();

        List<string> errors = new List<string>();
        var insideSolids_all = new List<Solid>();
        foreach (Element el in collector)
        {
          try
          {
            var solids = GetSolids(el);

            if (solids.Count == 0)
              continue;

            var insideSolids = new List<Solid>();
            foreach (var s in solids)
            {
              try
              {
                Solid intersect = BooleanOperationsUtils.ExecuteBooleanOperation(s, sectionSolid, BooleanOperationsType.Intersect);

                if (intersect != null && intersect.Volume > 1e-6)
                {
                  insideSolids.Add(intersect);
                }
              }
              catch
              {
                errors.Add($"{el.Id.ToString()} - {el.Name}");
              }
            }

            //Debug.Print(insideSolids.Count.ToString());
            // 🔹 union lại thành 1 solid
            Solid merged = UnionSolids(insideSolids);

            if (merged == null || merged.Volume < 1e-6)
              continue;
            insideSolids_all.Add(merged);

            
          }
          catch
          {

          }
        }

        foreach (var err in errors)
        {
          Debug.Print(err);
        }
        Solid merged_all = UnionSolids(insideSolids_all);

        if (merged_all == null || merged_all.Volume < 1e-6)
          return Result.Failed;

        DirectShape ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_SpecialityEquipment));
        var volumn = Math.Round(merged_all.Volume * Math.Pow(0.3048, 3), 2); // convert sang m3
        ds.SetShape(new List<GeometryObject> { merged_all });
        ds.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set($"{volumn} m3");
        tx.Commit();
      }

      return Result.Succeeded;
    }

    // =========================
    // 🔹 CREATE SOLID FROM SECTION BOX
    // =========================
    public Solid CreateSolidFromLoopLine(View3D view3D, List<Line> lines)
    {
      BoundingBoxXYZ bbox = view3D.GetSectionBox();
      Transform t = bbox.Transform;

      XYZ min = bbox.Min;
      XYZ max = bbox.Max;

      // 🔹 convert lines về LOCAL space của section box
      Transform toLocal = t.Inverse;
      var sorted = SortLinesToLoop(lines);
      CurveLoop loop = new CurveLoop();

      foreach (var line in sorted)
      {
        // ✅ transform về local
        Curve localLine = line.CreateTransformed(toLocal);

        // ✅ move về mặt đáy (min.Z)
        XYZ p0 = localLine.GetEndPoint(0);
        XYZ p1 = localLine.GetEndPoint(1);

        XYZ p0_new = new XYZ(p0.X, p0.Y, min.Z);
        XYZ p1_new = new XYZ(p1.X, p1.Y, min.Z);

        loop.Append(Line.CreateBound(p0_new, p1_new));
      }

      double height = max.Z - min.Z;

      // 🔹 extrusion trong LOCAL
      Solid solidLocal = GeometryCreationUtilities.CreateExtrusionGeometry(
          new List<CurveLoop> { loop },
          XYZ.BasisZ,
          height
      );

      // 🔥 transform về WORLD (QUAN TRỌNG NHẤT)
      Solid solidWorld = SolidUtils.CreateTransformed(solidLocal, t);

      return solidWorld;
    }

    public static List<Curve> SortLinesToLoop(List<Line> lines, double tol = 1e-6)
    {
      var result = new List<Curve>();
      var used = new HashSet<int>();

      // Start with first line
      Line current = lines[0];
      result.Add(current);
      used.Add(0);

      XYZ currentEnd = current.GetEndPoint(1);

      while (used.Count < lines.Count)
      {
        bool found = false;

        for (int i = 0; i < lines.Count; i++)
        {
          if (used.Contains(i)) continue;

          Line line = lines[i];

          XYZ p0 = line.GetEndPoint(0);
          XYZ p1 = line.GetEndPoint(1);

          // Case 1: p0 matches currentEnd
          if (p0.DistanceTo(currentEnd) < tol)
          {
            result.Add(line);
            currentEnd = p1;
            used.Add(i);
            found = true;
            break;
          }

          // Case 2: p1 matches currentEnd → reverse
          if (p1.DistanceTo(currentEnd) < tol)
          {
            Line reversed = Line.CreateBound(p1, p0);
            result.Add(reversed);
            currentEnd = p0;
            used.Add(i);
            found = true;
            break;
          }
        }

        if (!found)
          throw new Exception("Cannot form a continuous loop");
      }

      return result;
    }

    // =========================
    // 🔹 CREATE SOLID FROM SECTION BOX
    // =========================
    public Solid CreateSolidFromSectionBox(View3D view3D)
    {
      BoundingBoxXYZ bbox = view3D.GetSectionBox();
      Transform t = bbox.Transform;

      XYZ min = bbox.Min;
      XYZ max = bbox.Max;

      XYZ p1 = t.OfPoint(new XYZ(min.X, min.Y, min.Z));
      XYZ p2 = t.OfPoint(new XYZ(max.X, min.Y, min.Z));
      XYZ p3 = t.OfPoint(new XYZ(max.X, max.Y, min.Z));
      XYZ p4 = t.OfPoint(new XYZ(min.X, max.Y, min.Z));

      CurveLoop loop = new CurveLoop();
      loop.Append(Line.CreateBound(p1, p2));
      loop.Append(Line.CreateBound(p2, p3));
      loop.Append(Line.CreateBound(p3, p4));
      loop.Append(Line.CreateBound(p4, p1));

      double height = max.Z - min.Z;
      XYZ dir = t.BasisZ;

      return GeometryCreationUtilities.CreateExtrusionGeometry(
          new List<CurveLoop> { loop },
          dir,
          height
      );
    }

    

    // =========================
    // 🔹 GET ALL SOLIDS (WITH TRANSFORM)
    // =========================
    public List<Solid> GetSolids(Element element)
    {
      List<Solid> solids = new List<Solid>();

      Options opt = new Options
      {
        IncludeNonVisibleObjects = true,
        DetailLevel = ViewDetailLevel.Fine
      };

      GeometryElement geo = element.get_Geometry(opt);

      if (geo == null) return solids;

      foreach (GeometryObject obj in geo)
      {
        ExtractSolid(obj, solids, Transform.Identity);
      }

      return solids;
    }

    private void ExtractSolid(GeometryObject obj, List<Solid> solids, Transform currentTransform)
    {
      if (obj is Solid solid)
      {
        if (solid.Volume > 1e-6)
        {
          //solids.Add(SolidUtils.CreateTransformed(solid, currentTransform));
          solids.Add(solid);
        }
      }
      else if (obj is GeometryInstance instance)
      {
        Transform newTransform = currentTransform.Multiply(instance.Transform);

        foreach (GeometryObject instObj in instance.GetInstanceGeometry())
        {
          ExtractSolid(instObj, solids, newTransform);
        }
      }
    }

    // =========================
    // 🔹 UNION SOLIDS
    // =========================
    public Solid UnionSolids(List<Solid> solids)
    {
      if (solids == null || solids.Count == 0)
        return null;

      Solid result = solids[0];

      for (int i = 1; i < solids.Count; i++)
      {
        try
        {
          result = BooleanOperationsUtils.ExecuteBooleanOperation(result, solids[i], BooleanOperationsType.Union);
        }
        catch
        {
          // skip lỗi
        }
      }

      return result;
    }
  }
}