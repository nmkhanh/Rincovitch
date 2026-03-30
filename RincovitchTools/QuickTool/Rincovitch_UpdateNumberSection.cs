using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Microsoft.Expression.Media;
using Revit.Async;
using RincovitchTools.General.Revit;
using RincovitchTools.Print.API.Revit;
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

namespace RincovitchTools.QuickTool
{
  [Transaction(TransactionMode.Manual)]
  internal class Rincovitch_UpdateNumberSection : IExternalCommand
  {
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
      try
      {
        UIApplication uiapp = commandData.Application;
        UIDocument uidoc = uiapp.ActiveUIDocument;
        Document doc = uidoc.Document;

        using (Transaction t = new Transaction(doc, "UpdateNumberSection"))
        {
          t.Start();

          #region code
          // Collect currently selected sheets.
          IList<ElementId> selectedIds = uidoc.Selection.GetElementIds().ToList();
          List<ElementId> sheetIds = selectedIds
            .Select(id => doc.GetElement(id))
            .OfType<ViewSheet>()
            .Select(s => s.Id)
            .ToList();

          if (!sheetIds.Any())
          {
            TaskDialog.Show("Update Number Section", "Please select one or more sheets before running the tool.");
            t.RollBack();
            return Result.Cancelled;
          }

          // Gather section viewports placed on the selected sheets.
          IEnumerable<Viewport> sectionViewports = new FilteredElementCollector(doc)
            .OfClass(typeof(Viewport))
            .Cast<Viewport>()
            .Where(vp => sheetIds.Contains(vp.SheetId))
            .Where(vp =>
            {
              View view = doc.GetElement(vp.ViewId) as View;
              return view != null && view.ViewType == ViewType.Section;
            })
            .ToList();

          if (!sectionViewports.Any())
          {
            TaskDialog.Show("Update Number Section", "No section views found on the selected sheets.");
            t.RollBack();
            return Result.Cancelled;
          }

          // Order by sheet number, then cluster rows by Y tolerance, then X within each row.
#if D2022 || D2023 || D2024 || D2025 || D2026
          double rowTolerance = UnitUtils.ConvertToInternalUnits(50, UnitTypeId.Millimeters); // ~10 mm tolerance for same row
#else
          double rowTolerance = UnitUtils.ConvertToInternalUnits(50, DisplayUnitType.DUT_MILLIMETERS); // ~10 mm tolerance for same row
#endif

          List<Viewport> orderedViewports = new List<Viewport>();
          Dictionary<int, (int sheetId, string sheetNumber, string sheetName, int rowIndex, double rowY, double x, double y)> viewportInfo =
            new Dictionary<int, (int, string, string, int, double, double, double)>();
          Dictionary<int, int> sheetRowCounts = new Dictionary<int, int>();
          Dictionary<int, List<string>> groupingDebug = new Dictionary<int, List<string>>();

          // IMPORTANT: group by SheetId (ElementId). Do NOT group by ViewSheet object reference.
          var sheetGroups = sectionViewports
            .GroupBy(vp => vp.SheetId)
            .Select(g => new
            {
              SheetId = g.Key,
              Sheet = doc.GetElement(g.Key) as ViewSheet,
              Viewports = g.ToList()
            })
            .Where(x => x.Sheet != null)
            .OrderBy(x => x.Sheet.SheetNumber, StringComparer.Ordinal)
            .ToList();

          foreach (var sheetGroup in sheetGroups)
          {
            // Sắp xếp Y giảm dần rồi tạo hàng mới khi lệch quá tolerance.
            List<Viewport> sortedByY = sheetGroup.Viewports
              .OrderByDescending(vp => vp.GetBoxCenter().Y)
              .ToList();

            List<string> debugLines = new List<string>();
            int sheetIdInt = F_Versions.get_id_int(sheetGroup.SheetId);
            groupingDebug[sheetIdInt] = debugLines;
            debugLines.Add($"  DebugGroup: sheetId={sheetIdInt} | sheetNo={sheetGroup.Sheet.SheetNumber} | count={sortedByY.Count}");
#if D2022 || D2023 || D2024 || D2025 || D2026
            debugLines.Add($"  DebugGroup: rowToleranceInternal={rowTolerance:R} | rowToleranceMm={UnitUtils.ConvertFromInternalUnits(rowTolerance, UnitTypeId.Millimeters):0.###}");
#else
            debugLines.Add($"  DebugGroup: rowToleranceInternal={rowTolerance:R} | rowToleranceMm={UnitUtils.ConvertFromInternalUnits(rowTolerance, DisplayUnitType.DUT_MILLIMETERS):0.###}");
#endif

            List<(double seedY, List<Viewport> vps)> rows = new List<(double, List<Viewport>)>();

            foreach (Viewport vp in sortedByY)
            {
              double y = vp.GetBoxCenter().Y;
              double? lastSeed = rows.Any() ? rows.Last().seedY : (double?)null;
              double? diff = lastSeed.HasValue ? Math.Abs(lastSeed.Value - y) : (double?)null;
              bool isNewRow = !rows.Any() || (diff.HasValue && diff.Value > rowTolerance);

#if D2022 || D2023 || D2024 || D2025 || D2026
              double yMm = UnitUtils.ConvertFromInternalUnits(y, UnitTypeId.Millimeters);
              string seedMmText = lastSeed.HasValue ? UnitUtils.ConvertFromInternalUnits(lastSeed.Value, UnitTypeId.Millimeters).ToString("0.###") : "-";
              string diffMmText = diff.HasValue ? UnitUtils.ConvertFromInternalUnits(diff.Value, UnitTypeId.Millimeters).ToString("0.###") : "-";
              debugLines.Add($"    yMm={yMm:0.###} | lastSeedMm={seedMmText} | diffMm={diffMmText} | newRow={isNewRow}");
#else
              double yMm = UnitUtils.ConvertFromInternalUnits(y, DisplayUnitType.DUT_MILLIMETERS);
              string seedMmText = lastSeed.HasValue ? UnitUtils.ConvertFromInternalUnits(lastSeed.Value, DisplayUnitType.DUT_MILLIMETERS).ToString("0.###") : "-";
              string diffMmText = diff.HasValue ? UnitUtils.ConvertFromInternalUnits(diff.Value, DisplayUnitType.DUT_MILLIMETERS).ToString("0.###") : "-";
              debugLines.Add($"    yMm={yMm:0.###} | lastSeedMm={seedMmText} | diffMm={diffMmText} | newRow={isNewRow}");
#endif

              if (isNewRow)
              {
                rows.Add((y, new List<Viewport> { vp }));
              }
              else
              {
                rows.Last().vps.Add(vp);
              }
            }

            sheetRowCounts[sheetIdInt] = rows.Count;

            // Giữ thứ tự hàng từ trên xuống, và trong hàng sắp từ trái qua phải.
            for (int rowIdx = 0; rowIdx < rows.Count; rowIdx++)
            {
              double rowY = rows[rowIdx].seedY;
              List<Viewport> orderedRow = rows[rowIdx].vps
                .OrderBy(vp => vp.GetBoxCenter().X)
                .ToList();

              foreach (Viewport vp in orderedRow)
              {
                XYZ center = vp.GetBoxCenter();
                viewportInfo[F_Versions.get_id_int(vp.Id)] = (sheetIdInt, sheetGroup.Sheet.SheetNumber, sheetGroup.Sheet.Name, rowIdx, rowY, center.X, center.Y);
                orderedViewports.Add(vp);
              }
            }
          }

          // Pass 1: assign unique temp values to avoid collisions.
          string tempToken = Guid.NewGuid().ToString("N");
          int counter_ = 10001;
          foreach (Viewport vp in orderedViewports)
          {
            Parameter detailNumberParam = vp.get_Parameter(BuiltInParameter.VIEWPORT_DETAIL_NUMBER);
            if (detailNumberParam == null || detailNumberParam.IsReadOnly)
            {
              continue;
            }

             detailNumberParam.Set($"TMP_{tempToken}_{counter_}");

            Parameter detailNumberParam_ = vp.get_Parameter(BuiltInParameter.VIEW_NAME);
            if (detailNumberParam_ == null || detailNumberParam_.IsReadOnly)
            {
              continue;
            }

            detailNumberParam_.Set($"CONCRETE SECTION TMP {counter_}");

            counter_++;
          }

          // Pass 2: assign final sequential values.
          int counter = 1;
          foreach (Viewport vp in orderedViewports)
          {
            Parameter detailNumberParam = vp.get_Parameter(BuiltInParameter.VIEWPORT_DETAIL_NUMBER);
            if (detailNumberParam == null || detailNumberParam.IsReadOnly)
            {
              continue;
            }

            detailNumberParam.Set($"CS{counter}");

            Parameter detailNumberParam_ = vp.get_Parameter(BuiltInParameter.VIEW_NAME);
            if (detailNumberParam_ == null || detailNumberParam_.IsReadOnly)
            {
              continue;
            }

            detailNumberParam_.Set($"CONCRETE SECTION {counter}");

            counter++;
          }

          // Export ordering to txt per sheet and Y group.
          List<string> reportLines = new List<string>();
          var reportGroups = orderedViewports
            .Select(vp => new { vp, info = viewportInfo[F_Versions.get_id_int(vp.Id)] })
            .GroupBy(x => (x.info.sheetId, x.info.sheetNumber, x.info.sheetName))
            .OrderBy(g => g.Key.sheetNumber, StringComparer.Ordinal);

          foreach (var sheetGroup in reportGroups)
          {
            reportLines.Add($"Sheet {sheetGroup.Key.sheetNumber} - {sheetGroup.Key.sheetName} (Id={sheetGroup.Key.sheetId})");

#if D2022 || D2023 || D2024 || D2025 || D2026
            double rowToleranceMm = UnitUtils.ConvertFromInternalUnits(rowTolerance, UnitTypeId.Millimeters);
#else
            double rowToleranceMm = UnitUtils.ConvertFromInternalUnits(rowTolerance, DisplayUnitType.DUT_MILLIMETERS);
#endif
            sheetRowCounts.TryGetValue(sheetGroup.Key.sheetId, out int computedRowCount);
            reportLines.Add($"  Debug: rowTolerance≈{rowToleranceMm:0.##} mm | computedRows={computedRowCount}");

            if (groupingDebug.TryGetValue(sheetGroup.Key.sheetId, out List<string> dbg) && dbg != null && dbg.Any())
            {
              reportLines.AddRange(dbg);
            }

            var rows = sheetGroup
              .GroupBy(x => x.info.rowIndex)
              .OrderByDescending(g => g.First().info.rowY);

            int rowNo = 1;
            foreach (var row in rows)
            {

#if D2022 || D2023 || D2024 || D2025 || D2026
              double rowYmm = UnitUtils.ConvertFromInternalUnits(row.First().info.rowY, UnitTypeId.Millimeters);
#else
              double rowYmm = UnitUtils.ConvertFromInternalUnits(row.First().info.rowY, DisplayUnitType.DUT_MILLIMETERS);
#endif
              reportLines.Add($"  Row {rowNo} (Y≈{rowYmm:0.##} mm)");

              foreach (var item in row.OrderBy(x => x.info.x))
              {
                string detailNumber = item.vp.get_Parameter(BuiltInParameter.VIEWPORT_DETAIL_NUMBER)?.AsString();
                View view = doc.GetElement(item.vp.ViewId) as View;
#if D2022 || D2023 || D2024 || D2025 || D2026
                double xMm = UnitUtils.ConvertFromInternalUnits(item.info.x, UnitTypeId.Millimeters);
                double yMm = UnitUtils.ConvertFromInternalUnits(item.info.y, UnitTypeId.Millimeters);
#else
                double xMm = UnitUtils.ConvertFromInternalUnits(item.info.x, DisplayUnitType.DUT_MILLIMETERS);
                double yMm = UnitUtils.ConvertFromInternalUnits(item.info.y, DisplayUnitType.DUT_MILLIMETERS);
#endif
                double deltaYmm = Math.Abs(yMm - rowYmm);
                reportLines.Add($"    {detailNumber} | View: {view?.Name} | rowIndex={item.info.rowIndex} | X≈{xMm:0.##} mm | Y≈{yMm:0.##} mm | dY≈{deltaYmm:0.##} mm");
              }

              rowNo++;
            }

            reportLines.Add(string.Empty);
          }

          string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "SectionOrder.txt");
          File.WriteAllLines(outputPath, reportLines, Encoding.UTF8);

#endregion

          t.Commit();
        }

        return Result.Succeeded;
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show(ex.Message);
        return Result.Failed;
      }
    }
  }
}
