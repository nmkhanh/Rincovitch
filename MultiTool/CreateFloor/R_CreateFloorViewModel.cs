using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using MultiTool.CreateFloor.Models;
using MultiTool.CreateFloor.Models.Childs;
using Revit.Async;
using Reference = Autodesk.Revit.DB.Reference;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;
using View = Autodesk.Revit.DB.View;

namespace MultiTool.CreateFloor
{
  public class R_CreateFloorViewModel : BaseViewModel
  {
    #region ICommand
    public ICommand LoadedCommand { get; set; }

    public ICommand RunCommand { get; set; }
    #endregion

    B_VM _VM = new B_VM();
    public B_VM VM { get => _VM; set { _VM = value; OnPropertyChanged(); } }

    public R_CreateFloorViewModel(UIApplication uiapp)
    {
      LoadedCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        LoadedAsync(uiapp);
      });

      RunCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        RunAsync();
      });

    }

    void LoadedAsync(UIApplication uiapp)
    {
      try
      {
        var type = new FilteredElementCollector(uiapp.ActiveUIDocument.Document)
          .OfClass(typeof(FloorType))
          .Cast<FloorType>()
          .Where(x => x.Name.Contains("RINCO_FL_SLB_"))
          .ToList().Select(x => new B_FloorType()
          {
            Name = x.Name,
            Type = x
          }).ToList();
        type.Sort(new F_SortFloorType());
        VM.FloorType_s = new ObservableCollection<B_FloorType>(type);
        VM.FloorType = VM.FloorType_s.First();
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
          create(uidoc, doc);
        });
      }
      catch (Exception)
      {

      }
    }

    void create(UIDocument uidoc, Document doc)
    {
      try
      {
        // Pick Floor trong Link
        Reference pickedRef = uidoc.Selection.PickObject(ObjectType.LinkedElement, "Chọn sàn (Floor) trong Revit Link");
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
        ElementId linkedFloorId = pickedRef.LinkedElementId;
        Floor floor = linkDoc.GetElement(linkedFloorId) as Floor;
        if (floor == null)
          return;

        // Lấy mặt top bằng HostObjectUtils nếu muốn
        IList<Reference> bottomRefs = null;
        try
        {
          if (VM.TopFace)
            bottomRefs = HostObjectUtils.GetTopFaces(floor);
          else
            bottomRefs = HostObjectUtils.GetBottomFaces(floor);
        }
        catch
        {
          bottomRefs = null;
        }

        PlanarFace bottomFace = null;
        IList<CurveLoop> loops = null;

        if (bottomRefs != null && bottomRefs.Count > 0)
        {
          // Có top face references
          // Lấy mặt từ reference
          foreach (Reference rf in bottomRefs)
          {
            GeometryObject geoObj = floor.GetGeometryObjectFromReference(rf);
            PlanarFace pf = geoObj as PlanarFace;
            if (pf != null)
            {
              bottomFace = pf;
              break;
            }
          }
        }
        //if(bottomFace == null)
        //{
        //  // fallback: duyệt geometry
        //  Options opt = new Options();
        //  opt.ComputeReferences = true;
        //  GeometryElement ge = floor.get_Geometry(opt);
        //  foreach(GeometryObject obj in ge)
        //  {
        //    Solid solid = obj as Solid;
        //    if(solid == null)
        //      continue;
        //    foreach(Face face in solid.Faces)
        //    {
        //      PlanarFace pf = face as PlanarFace;
        //      if(pf == null)
        //        continue;
        //      // chọn mặt có Z-component normal lớn nhất
        //      // hoặc so sánh góc với trục Z
        //      double dot = pf.FaceNormal.DotProduct(XYZ.BasisZ);
        //      if(dot > 0.5) // threshold, bạn có thể điều chỉnh
        //      {
        //        bottomFace = pf;
        //        break;
        //      }
        //    }
        //    if(bottomFace != null)
        //      break;
        //  }
        //}

        if (bottomFace == null)
          return;

        loops = bottomFace.GetEdgesAsCurveLoops();
        if (loops == null || loops.Count == 0)
          return;

        double zLevel = 0;

        List<CurveLoop> loopsFlattened = new List<CurveLoop>();

        var bt = floor.get_Parameter(BuiltInParameter.STRUCTURAL_ELEVATION_AT_BOTTOM).AsValueString() == null;
        if (bt == false)
        {
          loopsFlattened = loops.ToList();
        }
        else
        {
          foreach (CurveLoop loop3d in loops)
          {
            CurveLoop newLoop = new CurveLoop();
            foreach (Curve c in loop3d)
            {
              // lấy endpoints 
              XYZ p1 = c.GetEndPoint(0);
              XYZ p2 = c.GetEndPoint(1);
              // flatten Z
              XYZ p1flat = new XYZ(p1.X, p1.Y, zLevel);
              XYZ p2flat = new XYZ(p2.X, p2.Y, zLevel);

              // tạo curve mới từ p1flat → p2flat
              Curve cFlat = Line.CreateBound(p1flat, p2flat);
              newLoop.Append(cFlat);
            }
            loopsFlattened.Add(newLoop);
          }
        }


        Floor newFloor = null;
        FloorType ftHost = null;
        if (string.IsNullOrEmpty(VM.FloorName))
        {
          ftHost = VM.FloorType.Type;
        }
        else
        {
          var name_type = Regex.Replace(VM.FloorType.Type.Name, @"\d+", VM.FloorName);
          var check = VM.FloorType_s
            .Where(x => x.Name == name_type);
          if (check.Count() == 0)
          {

            var type = DuplicateFloorTypeAndChangeThickness(doc, VM.FloorType.Type, Convert.ToDouble(VM.FloorName), 0,
              Regex.Replace(VM.FloorType.Type.Name, @"\d+", VM.FloorName));

            var new_ = new B_FloorType()
            {
              Name = type.Name,
              Type = type
            };
            VM.FloorType_s.Add(new_);
            VM.FloorType = new_;
          }
          else
          {
            VM.FloorType = check.First();
          }
          ftHost = VM.FloorType.Type;
        }
        using (TransactionGroup tx_g = new TransactionGroup(doc, "Create Floor Copy From Link"))
        {
          tx_g.Start();
          using (Transaction tx = new Transaction(doc, "Create Floor Copy From Link"))
          {
            tx.Start();

            // Chọn FloorType từ linkFloor nếu cùng loại (nếu có mapping)


            // Chọn Level phù hợp — có thể lấy Level gần nhất hoặc level tùy ý
            // Ví dụ: lấy level thấp nhất
            var linklevelId = linkDoc.GetElement(floor.LevelId) as Level;
            var levelHost_s = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .ToList();
            var levelHost_ = levelHost_s.Where(x => x.Elevation == linklevelId.Elevation);
            Level levelHost = levelHost_.Count() > 0 ? levelHost_.FirstOrDefault() : levelHost_s.FirstOrDefault();

            if (levelHost == null || ftHost == null)
            {
              TaskDialog.Show("Error", "Không tìm được Level hoặc FloorType trong host.");
              tx.RollBack();
              return;
            }

            // Tạo Floor
            // Note: Floor.Create overload trong Revit 2026
            //newFloor = Floor.Create(doc, loopsFlattened, ftHost.Id, levelHost.Id);
            //newFloor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(
            //  floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).AsDouble());
            tx.Commit();
          }
          if (bt == false)
          {
            using (Transaction tx = new Transaction(doc, "Create Floor Copy From Link"))
            {
              tx.Start();
              double minZ = Double.MaxValue;
              XYZ minPoint = null;
              foreach (CurveLoop loop in loops)
              {
                foreach (Curve c in loop)
                {
                  // Endpoints
                  XYZ p1 = c.GetEndPoint(0);
                  XYZ p2 = c.GetEndPoint(1);

                  if (p1.Z < minZ)
                  {
                    minZ = p1.Z;
                    minPoint = p1;
                  }
                  if (p2.Z < minZ)
                  {
                    minZ = p2.Z;
                    minPoint = p2;
                  }
                }
              }
              var off = minPoint.Z - newFloor.get_Parameter(BuiltInParameter.STRUCTURAL_ELEVATION_AT_BOTTOM).AsDouble();
              newFloor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(
                floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).AsDouble() + off * (VM.TopFace ? 0 : 1));
              tx.Commit();
            }
          }
          else
          {
            using (Transaction tx = new Transaction(doc, "Create Floor Copy From Link"))
            {
              tx.Start();

              double minZ = Double.MaxValue;
              XYZ minPoint = null;
              foreach (CurveLoop loop in loops)
              {
                foreach (Curve c in loop)
                {
                  // Endpoints
                  XYZ p1 = c.GetEndPoint(0);
                  XYZ p2 = c.GetEndPoint(1);

                  if (p1.Z < minZ)
                  {
                    minZ = p1.Z;
                    minPoint = p1;
                  }
                  if (p2.Z < minZ)
                  {
                    minZ = p2.Z;
                    minPoint = p2;
                  }
                }
              }
              var off = minPoint.Z - newFloor.get_Parameter(BuiltInParameter.STRUCTURAL_ELEVATION_AT_BOTTOM).AsDouble();
              newFloor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(
                floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).AsDouble() + off * (VM.TopFace ? 0 : 1));

              //if (floor.GetSlabShapeEditor().IsEnabled)
              //  newFloor.GetSlabShapeEditor().Enable();

              tx.Commit();
            }
            using (Transaction tx = new Transaction(doc, "Create Floor Copy From Link"))
            {
              tx.Start();

              //if (floor.GetSlabShapeEditor().IsEnabled)
              //{
              //  if (floor.GetSlabShapeEditor().SlabShapeVertices != null && floor.GetSlabShapeEditor().SlabShapeVertices.Size > 0)
              //  {
              //    foreach (SlabShapeVertex item in floor.GetSlabShapeEditor().SlabShapeVertices)
              //    {
              //      //newFloor.GetSlabShapeEditor().AddPoint(item.Position);
              //    }
              //  }
              //}

              
              tx.Commit();
            }
          }

          tx_g.Assimilate();
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show(ex.Message);
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
  }
}
