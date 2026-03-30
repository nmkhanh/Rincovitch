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
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Drawing;
using Group = Autodesk.Revit.DB.Group;
using View = Autodesk.Revit.DB.View;
using System.Windows.Controls;

namespace MultiTool.WallElevation
{
  //class FilterLevel : ISelectionFilter
  //{
  //  public bool AllowElement(Element elem)
  //  {
  //    return elem is Level;
  //  }

  //  public bool AllowReference(Reference reference, XYZ position)
  //  {
  //    return false;
  //  }
  //}

  [Transaction(TransactionMode.Manual)]
  internal class R_WallElevationDetail : IExternalCommand
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

    static List<ElevationWall> datas()
    {
      return new List<ElevationWall>()
      {
        new ElevationWall()
        {
          name = "W.1",
          data = new List<ElevationWallLoad>()
          {
            new ElevationWallLoad()
            {
              position = "L",
              level_start = "Level 1",
              level_next = "Level 4",
              rebar = "N32-250",
              load = "50Mpa \r\nN20-200 V EF\r\nN12-200 H EF"
            },
            new ElevationWallLoad()
            {
              position = "L",
              level_start = "Level 4",
              level_next = "Level 6",
              rebar = "N32-400",
              load = "50Mpa \r\nN16-200 V EF\r\nN12-200 H EF"
            },
            new ElevationWallLoad()
            {
              position = "L",
              level_start = "Level 6",
              level_next = "Level 10",
              rebar = "N28-500",
              load = "50Mpa \r\nN12-250 V EF\r\nN12-250 H EF"
            },
          }
        },
      };
    }

    static void create(UIDocument uidoc, Document doc)
    {
      try
      {
        List<(string rebar, string key)> rebars = new List<(string rebar, string key)>
        {
          ("DN","N20-1500"),
          ("D1","N24-1200"),
          ("D2","N24-1000"),
          ("D3","N24-600"),
          ("D4","N28-800"),
          ("D5","N28-600"),
          ("D6","N28-500"),
          ("D7","N32-600"),
          ("D8","N32-500"),
          ("D9","N32-400"),
          ("D11","N32-300"),
          ("D12","N32-250"),
          ("D13","N32-200"),
        };

        var datas_ = datas();

        var levels = new FilteredElementCollector(doc)
            .OfClass(typeof(Level))
            .Cast<Level>()
            .OrderBy(x => x.Elevation)
            .ToList();
        View view = doc.ActiveView;
        Plane viewPlane = Plane.CreateByNormalAndOrigin(view.ViewDirection, view.Origin);

        var walls = new FilteredElementCollector(doc, view.Id)
          .OfClass(typeof(Wall))
          .Cast<Wall>()
          .ToList();



        //var refs = uidoc.Selection.PickObjects(ObjectType.Face, "Select faces of walls");
        //if (refs == null || refs.Count == 0)
        //  return;

        //var faces = refs.Select(x => doc.GetElement(x).GetGeometryObjectFromReference(x) as PlanarFace).ToList();

        //var load_fm = "RINCO_DTL_TAG_Elevation Callout_100";
        //var load = "Rincovitch_Anno_Elevation Callout_100";
        //var load_type = new FilteredElementCollector(doc)
        //    .OfClass(typeof(FamilySymbol))
        //    .Cast<FamilySymbol>()
        //    .First(x => x.FamilyName == load_fm && x.Name == load);

        //var rebar_fm = "RINCO_DTL_Reo Bar 'A' Bar";
        //var rebar = "RINCO_DTL_Reo Bar 'D' Bar";
        //var rebar_type = new FilteredElementCollector(doc)
        //    .OfClass(typeof(FamilySymbol))
        //    .Cast<FamilySymbol>()
        //    .First(x => x.FamilyName == rebar_fm && x.Name == rebar);

        ////var bullout_fm = "RINCO_DTL_Pullout Bars Line Elevation";
        ////var bullout = "Rincovitch _D_Pullout Bars Line Elevation";
        ////var bullout_type = new FilteredElementCollector(doc)
        ////    .OfClass(typeof(FamilySymbol))
        ////    .Cast<FamilySymbol>()
        ////    .First(x => x.FamilyName == bullout_fm && x.Name == bullout);


        //var point_xy = bottom_top_face(faces[0], viewPlane).bt;
        //List<(string level, string rebar)> loads = new List<(string level, string rebar)>
        //{
        //  //("B01", "N32-200"),
        //  //("LEVEL 03", "N32-200"),
        //  //("LEVEL 06", "N32-600"),
        //  //("LEVEL 10", "N32-300"),
        //  //("LEVEL 12", "0")

        //  ("LEVEL 13", "N24-600"),
        //  //("LEVEL 12", "N28-600"),
        //  //("LEVEL 25 (LOWER ROOF)", "0"),
        //  ("LEVEL 26 (UPPER ROOF)", "0"),
        //};

        //using (TransactionGroup txGroup = new TransactionGroup(doc, "Wall elevation"))
        //{
        //  txGroup.Start();
        //  // LOAD
        //  using (Transaction tx = new Transaction(doc, "LOAD"))
        //  {
        //    tx.Start();
        //    var load_group = new List<ElementId>();
        //    for (int i = 0; i < loads.Count() - 1; i++)
        //    {
        //      var level_start = levels.First(x => x.Name == loads[i].level);
        //      var level_next = levels.First(x => x.Name == loads[i + 1].level);
        //      var point = new XYZ(point_xy.X, point_xy.Y, level_start.Elevation + (level_next.Elevation - level_start.Elevation) / 2);
        //      var item = doc.Create.NewFamilyInstance(point, load_type, view);
        //      item.LookupParameter("ELEVATION CALLOUT HEIGHT")?.Set(level_next.Elevation - level_start.Elevation);
        //      item.LookupParameter("TEXT GAP")?.Set(F_Versions.mm(200));
        //      item.LookupParameter("WIDTH COVER")?.Set(F_Versions.mm(2000));
        //      if (i == 0)
        //        item.LookupParameter("BOTTOM COVER")?.Set(F_Versions.mm(10000));
        //      else
        //        item.LookupParameter("BOTTOM COVER")?.Set(F_Versions.mm(0));

        //      if (i == loads.Count() - 2)
        //        item.LookupParameter("TOP COVER")?.Set(F_Versions.mm(10000));
        //      else
        //        item.LookupParameter("TOP COVER")?.Set(F_Versions.mm(0));

        //      load_group.Add(item.Id);
        //    }
        //    doc.Create.NewGroup(load_group);
        //    tx.Commit();
        //  }

        //  // REBAR
        //  var rebar_group = new List<Element>();
        //  using (Transaction tx = new Transaction(doc, "Rebar"))
        //  {
        //    tx.Start();
        //    foreach (var face in faces)
        //    {

        //      Wall wall = doc.GetElement(refs[faces.IndexOf(face)]) as Wall;
        //      Level level = doc.GetElement(wall.LevelId) as Level;

        //      var length = wall.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
        //      var point = center_bottom_face(face) + XYZ.BasisZ * F_Versions.mm(300);
        //      var item = doc.Create.NewFamilyInstance(point, rebar_type, view);

        //      rebar_group.Add(item);
        //    }

        //    tx.Commit();
        //  }

        //  using (Transaction tx = new Transaction(doc, "RebarPara"))
        //  {
        //    tx.Start();
        //    int a = 0;
        //    foreach (var item in rebar_group)
        //    {
        //      Wall wall = doc.GetElement(refs[a]) as Wall;
        //      Level level = doc.GetElement(wall.LevelId) as Level;

        //      var length = wall.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
        //      string value = "";
        //      string value_ = "";
        //      for (int i = 0; i < loads.Count() - 1; i++)
        //      {
        //        var level_start = levels.First(x => x.Name == loads[i].level);
        //        var level_next = levels.First(x => x.Name == loads[i + 1].level);
        //        var list_level = levels.Where(x => x.Elevation >= level_start.Elevation && x.Elevation <= level_next.Elevation).ToList();
        //        var check = list_level.Select(X => X.Name).Contains(level.Name);

        //        //MessageBox.Show(String.Join("\n", list_level.Select(X => X.Name)));
        //        if (check)
        //        {
        //          //MessageBox.Show(rebars.First(a => a.key == loads[i].rebar).rebar);
        //          value = rebars.First(a => a.key == loads[i].rebar).rebar;
        //          value_ = rebars.First(a => a.key == loads[i].rebar).key;
        //          break;
        //        }
        //      }
        //      //MessageBox.Show(value);
        //      item.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS)?.Set(value_);
        //      item.LookupParameter("Reo Text Top")?.Set(value);



        //      item.LookupParameter("Cog Length")?.Set(F_Versions.mm(200));
        //      item.LookupParameter("L")?.Set(length / 2);
        //      item.LookupParameter("L2")?.Set(length / 2);
        //      item.LookupParameter("La")?.Set(F_Versions.mm(700));
        //      item.LookupParameter("Lb")?.Set(F_Versions.mm(1300));
        //      item.LookupParameter("Lx")?.Set(F_Versions.mm(150));
        //      item.LookupParameter("Lx1")?.Set(F_Versions.mm(150));
        //      item.LookupParameter("TEXT Right")?.Set(F_Versions.mm(400));
        //      item.LookupParameter("TEXT Left")?.Set(F_Versions.mm(400));
        //      item.LookupParameter("Text Ext Line Middle Dim")?.Set(F_Versions.mm(0));

        //      item.LookupParameter("Extension Side 1")?.Set(F_Versions.mm(500));
        //      item.LookupParameter("Extension Side 2")?.Set(F_Versions.mm(500));

        //      item.LookupParameter("Arrow Visible")?.Set(1);
        //      item.LookupParameter("Cog 1")?.Set(0);
        //      item.LookupParameter("Cog 2")?.Set(0);
        //      item.LookupParameter("Cog 3")?.Set(0);
        //      item.LookupParameter("Cog 4")?.Set(0);
        //      item.LookupParameter("Dot")?.Set(1);
        //      item.LookupParameter("Reo")?.Set(1);
        //      item.LookupParameter("Text Ext Line Middle")?.Set(0);
        //      item.LookupParameter("Text Ext Line Side 1")?.Set(0);
        //      item.LookupParameter("Text Ext Line Side 2")?.Set(0);
        //      item.LookupParameter("Text Side 1")?.Set(0);
        //      item.LookupParameter("Text Side 1 Btm")?.Set(0);
        //      item.LookupParameter("Text Side 2")?.Set(1);
        //      item.LookupParameter("Text Side 2 Btm")?.Set(0);


        //      a++;
        //    }

        //    doc.Create.NewGroup(rebar_group.Select(x => x.Id).ToList());

        //    tx.Commit();
        //  }
        //  txGroup.Assimilate();
        //}
      }
      catch (Exception ex)
      {
        TaskDialog.Show("ERROR", ex.Message, TaskDialogCommonButtons.Ok);
      }
    }

    static XYZ ProjectPointToPlane(XYZ point, Plane plane)
    {
      XYZ origin = plane.Origin;
      XYZ normal = plane.Normal;  // Assume normalized
      XYZ v = point - origin;
      double d = v.DotProduct(normal);
      XYZ q = point - d * normal;
      return q;
    }

    static GraphicsStyle GetGraphicsStyleByName(Document doc, string styleName)
    {
      // Find GraphicsStyle in subcategories of OST_Lines
      var styles = new FilteredElementCollector(doc)
          .OfClass(typeof(GraphicsStyle))
          .Cast<GraphicsStyle>()
          .Where(gs => gs.GraphicsStyleCategory != null
                       && gs.GraphicsStyleCategory.Parent != null
                       && gs.GraphicsStyleCategory.Parent.Id ==
                          new ElementId(BuiltInCategory.OST_Lines));
      foreach (var gs in styles)
      {
        if (gs.Name.Equals(styleName, StringComparison.OrdinalIgnoreCase))
          return gs;
      }
      return null;
    }

    static (XYZ bt, XYZ t) bottom_top_face(PlanarFace face, Plane viewPlane)
    {
      try
      {
        #region PLAN POINT
        List<XYZ> boundaryPoints = new List<XYZ>();
        foreach (EdgeArray loop in face.EdgeLoops)
        {
          foreach (Edge edge in loop)
          {
            IList<XYZ> pts = edge.Tessellate();
            boundaryPoints.AddRange(pts);
          }
        }

        // Hướng pháp tuyến (normal) của mặt
        XYZ normal = face.FaceNormal.Normalize();

        // Tìm điểm cao nhất và thấp nhất theo hướng normal
        XYZ origin = face.Origin;

        XYZ topPoint = null;
        XYZ bottomPoint = null;
        double maxProj = double.MinValue;
        double minProj = double.MaxValue;

        foreach (XYZ p in boundaryPoints)
        {
          double proj = (p - origin).DotProduct(normal);
          if (proj > maxProj)
          {
            maxProj = proj;
            topPoint = p;
          }
          if (proj < minProj)
          {
            minProj = proj;
            bottomPoint = p;
          }
        }
        var point_bt = ProjectPointToPlane(bottomPoint, viewPlane);
        var point_t = ProjectPointToPlane(topPoint, viewPlane);

        return (point_bt, point_t);
        #endregion
      }
      catch (Exception)
      {
        return (XYZ.Zero, XYZ.Zero);
      }
    }

    static XYZ center_bottom_face(PlanarFace face)
    {
      XYZ middleOfBottomLine = null;
      try
      {
        // Hướng xét "bottom" — dùng global Z hoặc pháp tuyến mặt
        XYZ normal = XYZ.BasisZ; // hoặc face.FaceNormal.Normalize();

        // Duyệt toàn bộ cạnh của mặt
        Edge bottomEdge = null;
        double minAvgProj = double.MaxValue;

        foreach (EdgeArray loop in face.EdgeLoops)
        {
          foreach (Edge edge in loop)
          {
            IList<XYZ> pts = edge.Tessellate();
            if (pts.Count < 2) continue;

            // Tính trung bình chiếu của hai đầu điểm theo hướng normal
            XYZ mid = (pts.First() + pts.Last()) / 2.0;
            double avgProj = mid.DotProduct(normal);

            if (avgProj < minAvgProj)
            {
              minAvgProj = avgProj;
              bottomEdge = edge;
            }
          }
        }

        // Lấy điểm giữa của cạnh thấp nhất
        if (bottomEdge != null)
        {
          IList<XYZ> tess = bottomEdge.Tessellate();
          middleOfBottomLine = (tess.First() + tess.Last()) / 2.0;
          Trace.WriteLine($"Middle of bottom line: {middleOfBottomLine}");
        }
        else
        {
          Trace.WriteLine("Không tìm thấy bottom edge!");
        }

      }
      catch (Exception)
      {

      }
      return middleOfBottomLine;
    }

    public void CreateLinesByWall(Document doc, Wall wall)
    {
      try
      {
        LocationCurve locCurve = wall.Location as LocationCurve;
        if (locCurve == null) return;

        Curve wallCurve = locCurve.Curve;
        XYZ start = wallCurve.GetEndPoint(0);
        XYZ end = wallCurve.GetEndPoint(1);
        Line baseLine = Line.CreateBound(start, end);

        // Lấy danh sách cửa thuộc tường
        IList<ElementId> hostedIds = wall.FindInserts(true, true, true, true);
        List<(double start, double end)> cutRanges = new List<(double, double)>();

        foreach (ElementId id in hostedIds)
        {
          Element e = doc.GetElement(id);
          if (e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Doors)
          {
            LocationPoint lp = e.Location as LocationPoint;
            if (lp == null) continue;

            double doorWidth = e.get_Parameter(BuiltInParameter.DOOR_WIDTH)?.AsDouble() ?? 0.0;
            double halfWidth = doorWidth / 2;

            // Tính vị trí cửa theo tường
            double param;
            //wallCurve.Project(lp.Point, out param, out _);

            //cutRanges.Add((param - halfWidth, param + halfWidth));
          }
        }

        // Sắp xếp theo vị trí
        cutRanges = cutRanges.OrderBy(c => c.start).ToList();

        List<(double, double)> segments = new List<(double, double)>();
        double wallStart = 0.0;
        double wallEnd = wallCurve.Length;

        // Cắt các đoạn cửa ra
        foreach (var (cs, ce) in cutRanges)
        {
          if (cs > wallStart)
            segments.Add((wallStart, Math.Max(wallStart, cs)));
          wallStart = Math.Max(wallStart, ce);
        }

        if (wallStart < wallEnd)
          segments.Add((wallStart, wallEnd));

        // Vẽ các đoạn còn lại
        using (Transaction tran = new Transaction(doc, "Create Lines by Wall"))
        {
          tran.Start();

          SketchPlane sp = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(XYZ.BasisZ, XYZ.Zero));

          foreach (var (s, e) in segments)
          {
            XYZ ps = wallCurve.Evaluate(s / wallCurve.Length, true);
            XYZ pe = wallCurve.Evaluate(e / wallCurve.Length, true);
            Line l = Line.CreateBound(ps, pe);
            doc.Create.NewModelCurve(l, sp);
          }

          tran.Commit();
        }
      }
      catch (Exception ex)
      {
        System.Diagnostics.Trace.WriteLine($"CreateLinesByWall error: {ex.Message}");
      }
    }
  }

  class ElevationWall
  {
    public string name { get; set; }
    public List<ElevationWallLoad> data { get; set; }
  }

  class ElevationWallLoad
  {
    public string position { get; set; }
    public string level_start { get; set; }
    public string level_next { get; set; }
    public string rebar { get; set; }
    public string load { get; set; }
  }
}
