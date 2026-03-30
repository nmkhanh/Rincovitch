using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using MultiTool.CreateFloor.Models;
using MultiTool.CreateFloor.Models.Childs;
using Revit.Async;
using Reference = Autodesk.Revit.DB.Reference;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;
using View = Autodesk.Revit.DB.View;

namespace MultiTool.CreateRebarDetail
{
  public class R_CreateRebarDetailViewModel : BaseViewModel
  {
    #region ICommand
    public ICommand LoadedCommand { get; set; }

    public ICommand RunLinkCommand { get; set; }
    public ICommand RunCommand { get; set; }
    #endregion

    B_VM _VM = new B_VM();
    public B_VM VM { get => _VM; set { _VM = value; OnPropertyChanged(); } }

    public R_CreateRebarDetailViewModel(UIApplication uiapp)
    {
      LoadedCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        LoadedAsync(uiapp);
      });

      RunCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        RunAsync();
      });

      RunLinkCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        RunLinkAsync();
      });

    }

    void LoadedAsync(UIApplication uiapp)
    {
      try
      {

      }
      catch (Exception)
      {

      }
    }
    async Task RunAsync()
    {
      try
      {
        await RevitTask.RunAsync((uiapp) =>
        {
          UIDocument uidoc = uiapp.ActiveUIDocument;
          Document doc = uidoc.Document;
          var view = doc.ActiveView as ViewSection;

          Selection selection = uidoc.Selection;
          var ref_ = selection.PickObject(ObjectType.Element, new Filter_Wall_Column(), "Pick wall Revit Link");
          if (ref_ == null) return;
          Wall wall = doc.GetElement(ref_) as Wall;
          if (wall == null) return;

          var thickness = wall.Width;

          Options opt = new Options();
          opt.IncludeNonVisibleObjects = true;
          opt.View = view;
          var geometry = wall.get_Geometry(opt);

          List<Line> lines = new List<Line>();
          List<XYZ> points = new List<XYZ>();
          foreach (GeometryObject geoObj in geometry)
          {
            if (geoObj is Line line)
            {
              lines.Add(line);
              points.Add(line.GetEndPoint(0));
              points.Add(line.GetEndPoint(1));
            }
          }
          XYZ point_bottom = points.Find(x => x.Z == points.Min(y => y.Z));
          var bottom = point_bottom.Z;
          try
          {
            var bottom_line = uidoc.Selection.PickObject(ObjectType.Face, "Select Bottom Edge");
            if (bottom_line != null)
            {
              PlanarFace face = doc.GetElement(bottom_line).GetGeometryObjectFromReference(bottom_line) as PlanarFace;
              bottom = face.Origin.Z;
            }
          }
          catch (Exception)
          {

          }

          create(uidoc, doc, view, wall, bottom, points, thickness, lines);
        });
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show(ex.Message);
      }
    }

    async Task RunLinkAsync()
    {
      try
      {
        await RevitTask.RunAsync((uiapp) =>
        {
          UIDocument uidoc = uiapp.ActiveUIDocument;
          Document doc = uidoc.Document;
          var view = doc.ActiveView as ViewSection;

          // Pick Floor trong Link
          Reference pickedRef = uidoc.Selection.PickObject(ObjectType.LinkedElement, "Pick wall Revit Link");
          if (pickedRef == null)
            return;

          // Lấy RevitLinkInstance
          RevitLinkInstance linkInstance = doc.GetElement(pickedRef) as RevitLinkInstance;
          if (linkInstance == null)
            return;

          // Lấy Link Document
          Document linkDoc = linkInstance.GetLinkDocument();
          if (linkDoc == null)
            return;

          // Lấy Floor trong LinkDocument
          ElementId linkedId = pickedRef.LinkedElementId;
          Wall wall = linkDoc.GetElement(linkedId) as Wall;
          if (wall == null)
            return;
          var thickness = wall.Width;

          Options opt = new Options();
          opt.IncludeNonVisibleObjects = true;
          opt.View = view;
          var geometry = wall.get_Geometry(opt);

          List<Line> lines = new List<Line>();
          List<XYZ> points = new List<XYZ>();
          foreach (GeometryObject geoObj in geometry)
          {
            if (geoObj is Line line)
            {
              lines.Add(line);
              points.Add(line.GetEndPoint(0));
              points.Add(line.GetEndPoint(1));
            }
          }
          XYZ point_bottom = points.Find(x => x.Z == points.Min(y => y.Z));
          var bottom = point_bottom.Z;
          try
          {
            var bottom_line = uidoc.Selection.PickObject(ObjectType.Face, "Select Bottom Edge");
            RevitLinkInstance linkInstance_ = doc.GetElement(bottom_line.ElementId) as RevitLinkInstance;
            if (linkInstance_ == null) return;
            Document linkDoc_ = linkInstance_.GetLinkDocument();
            if (linkDoc == null)
              return;
            if (bottom_line != null)
            {
              PlanarFace face = linkDoc_.GetElement(bottom_line).GetGeometryObjectFromReference(bottom_line) as PlanarFace;
              bottom = face.Origin.Z;
            }
          }
          catch (Exception)
          {

          }

          create(uidoc, doc, view, wall, bottom, points, thickness, lines);
        });
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show(ex.Message);
      }
    }

    void create(UIDocument uidoc, Document doc, ViewSection view, Wall wall, double bottom, List<XYZ> points, double thickness, List<Line> lines)
    {
      try
      {
        var feet_mm = 304.8;
        var cover = 30 / feet_mm;
        var cover_side = 45 / feet_mm;
        var dia = 20 / feet_mm;
        var dia_line = 4 / feet_mm;


        var fm_break = $"RINCO_DTL_Break Line";
        var fm_break_type = $"Scale = {20}";

        var fm_L = $"RINCO_DTL_\"L\" Bar";
        var fm_L_type = $"RINCO_DTL_L Bar";

        var fm_L_symbol = new FilteredElementCollector(doc)
            .OfCategory(BuiltInCategory.OST_DetailComponents)
            .OfClass(typeof(FamilySymbol))
            .WhereElementIsElementType()
            .Cast<FamilySymbol>()
            .FirstOrDefault(x => x.Family.Name == fm_L && x.Name == fm_L_type);

        var fm_U = $"RINCO_DTL_\"U\" Bar";
        var fm_U_type = $"RINCO_DTL_U Bar";

        var fm_U_symbol = new FilteredElementCollector(doc)
           .OfCategory(BuiltInCategory.OST_DetailComponents)
           .OfClass(typeof(FamilySymbol))
           .WhereElementIsElementType()
           .Cast<FamilySymbol>()
           .FirstOrDefault(x => x.Family.Name == fm_U && x.Name == fm_U_type);

        var fm_array = $"RINCO_DTL_Rebar Section Array";
        var fm_array_type = $"N{20}";

        var fm_array_symbol = new FilteredElementCollector(doc)
            .OfCategory(BuiltInCategory.OST_DetailComponents)
            .OfClass(typeof(FamilySymbol))
            .WhereElementIsElementType()
            .Cast<FamilySymbol>()
            .FirstOrDefault(x => x.Family.Name == fm_array && x.Name == fm_array_type);

        var fm_precast = $"RINCO_DTL_Grout Tube_cog bottom3";
        var fm_precast_type = $"RINCO_D_Grout Tube_cog bottom 3";

        var fm_precast_symbol = new FilteredElementCollector(doc)
            .OfCategory(BuiltInCategory.OST_DetailComponents)
            .OfClass(typeof(FamilySymbol))
            .WhereElementIsElementType()
            .Cast<FamilySymbol>()
            .FirstOrDefault(x => x.Family.Name == fm_precast && x.Name == fm_precast_type);



        if (wall.Name.Contains("PRECAST"))
        {
          create_precast(uidoc, doc, view, wall, fm_precast_symbol, points, thickness, feet_mm, cover, dia, dia_line, bottom);
        }
        else
        {
          create_not_precast(uidoc, doc, view, wall, points, thickness, lines, feet_mm, cover, dia, dia_line, cover_side, bottom, fm_L_symbol, fm_array_symbol, fm_U_symbol);
        }

      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show(ex.Message);
      }
    }

    static void create_precast(UIDocument uidoc, Document doc, ViewSection view, Wall wall, FamilySymbol fm_precast_symbol, List<XYZ> points, double thickness,
      double feet_mm, double cover, double dia, double dia_line, double bottom)
    {
      try
      {

        XYZ point_bottom = points.Find(x => x.Z == points.Min(y => y.Z));
        XYZ point_top = points.Find(x => x.Z == points.Max(y => y.Z));

        var location = point_bottom;

        using (Transaction tr = new Transaction(doc, "Create Rebar Detail"))
        {
          tr.Start();
          if (!fm_precast_symbol.IsActive) fm_precast_symbol.Activate();
          var instance_precast = doc.Create.NewFamilyInstance(location, fm_precast_symbol, view);
          var instance_precast_length = Math.Abs(point_bottom.Z - bottom) - cover - dia * 2 - 20 / feet_mm;
          instance_precast.LookupParameter("Half Dowel Length")?.Set(instance_precast_length);
          instance_precast.LookupParameter("Cog End")?.Set(300 / feet_mm);
          instance_precast.LookupParameter("Tube Height")?.Set(500 / feet_mm);
          instance_precast.LookupParameter("Tube Offset")?.Set(thickness / 2);
          instance_precast.LookupParameter("bottom grout width")?.Set(thickness / 2 - 25 / feet_mm);


          GraphicsStyle gs = GetGraphicsStyleByName(doc, "RINCO - 0.25 Pen - Hidden");
          Plane viewPlane = Plane.CreateByNormalAndOrigin(view.ViewDirection, view.Origin);
          XYZ start = ProjectPointToPlane(point_bottom, viewPlane) + XYZ.BasisZ * 20 / feet_mm;
          XYZ end = ProjectPointToPlane(point_top, viewPlane);
          var detail_line = doc.Create.NewDetailCurve(view, Line.CreateBound(start, end));
          detail_line.LineStyle = gs;
          var copys = ElementTransformUtils.CopyElement(doc, detail_line.Id, view.RightDirection.Normalize() * (-thickness / 2 + 25 / feet_mm)).ToList();
          ElementTransformUtils.MoveElement(doc, detail_line.Id, view.RightDirection.Normalize() * (thickness / 2 - 25 / feet_mm));
          copys.AddRange(new List<ElementId>() { instance_precast.Id, detail_line.Id });
          var group = doc.Create.NewGroup(copys);
          //group.GroupType.Name = $"{wall.Name}_REBAR_{wall.Id.Value}";
          tr.Commit();
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show(ex.Message);
      }
    }

    static XYZ ProjectPointToPlane(XYZ point, Plane plane)
    {
      XYZ origin = plane.Origin;
      XYZ normal = plane.Normal;  // giả sử normalized
      XYZ v = point - origin;
      double d = v.DotProduct(normal);
      XYZ q = point - d * normal;
      return q;
    }

    static GraphicsStyle GetGraphicsStyleByName(Document doc, string styleName)
    {
      // Tìm các GraphicsStyle thuộc subcategories của OST_Lines
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

    static void create_not_precast(UIDocument uidoc, Document doc, ViewSection view, Wall wall, List<XYZ> points, double thickness, List<Line> lines,
      double feet_mm, double cover, double dia, double dia_line, double cover_side, double bottom,
      FamilySymbol fm_L_symbol, FamilySymbol fm_array_symbol, FamilySymbol fm_U_symbol)
    {
      try
      {
        using (Transaction tr = new Transaction(doc, "Create Rebar Detail"))
        {
          tr.Start();

          XYZ point_bottom = points.Find(x => x.Z == points.Min(y => y.Z));
          XYZ point_top = points.Find(x => x.Z == points.Max(y => y.Z));

          var location = point_bottom + XYZ.BasisZ * (bottom - point_bottom.Z + cover + dia * 2);



          if (!fm_L_symbol.IsActive) fm_L_symbol.Activate();
          var instance_L = doc.Create.NewFamilyInstance(location, fm_L_symbol, view);
          var instance_L_length = lines.First().Length + Math.Abs(point_bottom.Z - bottom) - cover - dia * 2 - cover - dia_line * 4;
          instance_L.LookupParameter("L1")?.Set(instance_L_length);
          instance_L.LookupParameter("L2")?.Set(300 / feet_mm);
          ElementTransformUtils.MoveElement(doc, instance_L.Id, view.RightDirection.Normalize() * (thickness / 2 - cover_side));



          if (!fm_array_symbol.IsActive) fm_array_symbol.Activate();
          var instance_array = doc.Create.NewFamilyInstance(Line.CreateBound(point_bottom, point_top), fm_array_symbol, view);
          instance_array.LookupParameter("OPOSITE DIR REBAR")?.Set(0);
          instance_array.LookupParameter("Centres")?.Set(200 / feet_mm);
          ElementTransformUtils.MoveElement(doc, instance_array.Id, view.RightDirection.Normalize() * (thickness / 2 - cover_side - dia_line));

          Plane plane = Plane.CreateByNormalAndOrigin(view.RightDirection.Normalize(), point_bottom);
          var mirrors = ElementTransformUtils.MirrorElements(doc, new List<ElementId>() { instance_L.Id, instance_array.Id }, plane, true).ToList();



          if (!fm_U_symbol.IsActive) fm_U_symbol.Activate();
          var instance_U = doc.Create.NewFamilyInstance(point_top, fm_U_symbol, view);
          instance_U.LookupParameter("L1")?.Set(300 / feet_mm);
          instance_U.LookupParameter("L2")?.Set(thickness);
          ElementTransformUtils.RotateElement(doc, instance_U.Id, Line.CreateBound(point_top, point_top + view.ViewDirection.Normalize() * 10), Math.PI);

          mirrors.AddRange(new List<ElementId>() { instance_L.Id, instance_array.Id, instance_U.Id });
          var group = doc.Create.NewGroup(mirrors);
          //group.GroupType.Name = $"{wall.Name}_REBAR_{wall.Id.Value}";

          tr.Commit();
        }
      }
      catch
      {

      }
    }
  }
}
