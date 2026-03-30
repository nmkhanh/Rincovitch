using Autodesk.Revit.DB;
using RincovitchTools.General.Revit;
using RincovitchTools.Print.Models.ModelChilds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace RincovitchTools.Print.API.Revit
{
  // Class to store TitleBlock info
  public class TitleBlockInfo
  {
    public string Size { get; set; } = "";
    public double Size_W { get; set; } = 0;
    public double Size_H { get; set; } = 0;
    public string Orientation { get; set; } = "";
    public FamilyInstance titleBlock
    {
      get; set;
    }
    public List<RevisionCloud> revisionCloud
    {
      get; set;
    }
  }

  public static class F_APIRevit_GetTreeView_Sheets
  {
    // Cache for TitleBlock info
    private static Dictionary<ElementId, TitleBlockInfo> _titleBlockCache;

    #region Helper Functions

    /// <summary>
    /// Get all TitleBlock info once for all sheets
    /// </summary>
    public static void LoadTitleBlockInfo(Document doc, IEnumerable<ViewSheet> sheets, List<FamilyInstance> title_blocks, List<RevisionCloud> RevisionCloud)
    {
      _titleBlockCache = new Dictionary<ElementId, TitleBlockInfo>();
      foreach (var sheet in sheets)
      {
        var info = new TitleBlockInfo();

        var titleBlock = title_blocks.FirstOrDefault(x => F_Versions.get_id(x.OwnerViewId) == F_Versions.get_id(sheet.Id));
        var RevisionClouds = RevisionCloud.Where(x => F_Versions.get_id(x.OwnerViewId) == F_Versions.get_id(sheet.Id)).ToList();

        if (titleBlock != null)
        {
          var widthParam = titleBlock.get_Parameter(BuiltInParameter.SHEET_WIDTH);
          var heightParam = titleBlock.get_Parameter(BuiltInParameter.SHEET_HEIGHT);

          if (widthParam != null && heightParam != null)
          {
#if D2022 || D2023 || D2024 || D2025 || D2026
            double width = UnitUtils.ConvertFromInternalUnits(widthParam.AsDouble(), UnitTypeId.Millimeters);
            double height = UnitUtils.ConvertFromInternalUnits(heightParam.AsDouble(), UnitTypeId.Millimeters);
#else
            double width = UnitUtils.ConvertFromInternalUnits(widthParam.AsDouble(), DisplayUnitType.DUT_MILLIMETERS);
            double height = UnitUtils.ConvertFromInternalUnits(heightParam.AsDouble(), DisplayUnitType.DUT_MILLIMETERS);
#endif

            info.Size = GetPaperSize(width, height);
            info.Size_W = width;
            info.Size_H = height;
            info.Orientation = width > height ? "Landscape" : "Portrait";

            info.titleBlock = titleBlock;
            info.revisionCloud = RevisionClouds;
          }
        }

        _titleBlockCache[sheet.Id] = info;
      }
    }

    private static NMK_M_SheetAndView CreateSheetItem(ViewSheet sheet)
    {
      // Get TitleBlock info from cache
      string size = "";
      string orientation = "";
      double size_W = 0;
      double size_H = 0;
      FamilyInstance titleBlock = null;
      List<RevisionCloud> revisionCloud = new();

      if (_titleBlockCache != null && _titleBlockCache.TryGetValue(sheet.Id, out var titleBlockInfo))
      {
        size = titleBlockInfo.Size;
        orientation = titleBlockInfo.Orientation;
        size_W = titleBlockInfo.Size_W;
        size_H = titleBlockInfo.Size_H;
        titleBlock = titleBlockInfo.titleBlock;
        revisionCloud = titleBlockInfo.revisionCloud;
      }

      // Get current revision
      string revision = sheet.get_Parameter(BuiltInParameter.SHEET_CURRENT_REVISION).AsValueString();
      string revision_date = sheet.get_Parameter(BuiltInParameter.SHEET_CURRENT_REVISION_DATE).AsValueString();

      return new NMK_M_SheetAndView
      {
        ID = F_Versions.get_id(sheet.Id),
        ViewSheet = sheet,
        Name = $"{sheet.SheetNumber} - {sheet.Name}",
        NameDefault = sheet.Name,
        SheetNumber = sheet.SheetNumber,
        SheetName = sheet.Name,
        Size = size,
        Orientation = orientation,
        Revision = revision,
        RevisionDate = revision_date,
        IsSheet = true,
        IsItem = true,

        Size_W = size_W,
        Size_H = size_H,
        TitleBlock = titleBlock,
        RevisionCloud = revisionCloud
      };
    }

    private static string GetPaperSize(double width, double height)
    {
      // Ensure width is the larger dimension for comparison
      double w = Math.Max(width, height);
      double h = Math.Min(width, height);

      // Standard paper sizes in mm (with tolerance)
      if (IsSize(w, h, 1189, 841))
        return "A0";
      if (IsSize(w, h, 841, 594))
        return "A1";
      if (IsSize(w, h, 594, 420))
        return "A2";
      if (IsSize(w, h, 420, 297))
        return "A3";
      if (IsSize(w, h, 297, 210))
        return "A4";
      if (IsSize(w, h, 210, 148))
        return "A5";

      // US paper sizes
      if (IsSize(w, h, 279, 216))
        return "Letter";
      if (IsSize(w, h, 356, 216))
        return "Legal";
      if (IsSize(w, h, 432, 279))
        return "Tabloid";

      return $"PS_{Math.Round(w)}x{Math.Round(h)}";
    }

    private static bool IsSize(double w, double h, double stdW, double stdH, double tolerance = 5)
    {
      return Math.Abs(w - stdW) <= tolerance && Math.Abs(h - stdH) <= tolerance;
    }

    private static NMK_M_SheetAndView CreateFolderItem(string folderName)
    {
      return new NMK_M_SheetAndView
      {
        Name = folderName,
        IsSheet = true,
        IsItem = false,
      };
    }
    #endregion

    /// <summary>
    /// Get sheets organized in tree structure based on BrowserOrganization
    /// </summary>
    public static List<NMK_M_SheetAndView> GetSheetTree(Document doc, List<FamilyInstance> title_blocks, List<RevisionCloud> RevisionCloud)
    {
      var result = new List<NMK_M_SheetAndView>();

      try
      {
        // Get BrowserOrganization for Sheets
        BrowserOrganization browserOrg = BrowserOrganization.GetCurrentBrowserOrganizationForSheets(doc);

        // Get all sheets
        var allSheets = new FilteredElementCollector(doc)
          .OfClass(typeof(ViewSheet))
          .Cast<ViewSheet>()
          .Where(x => !x.IsTemplate)
          .ToDictionary(x => x.Id, x => x);

        // Load TitleBlock info once for all sheets
        LoadTitleBlockInfo(doc, allSheets.Values, title_blocks, RevisionCloud);

        if (browserOrg != null && allSheets.Count > 0)
        {
          // Build tree from BrowserOrganization folder structure
          BuildSheetTreeFromFolders(doc, browserOrg, allSheets, result);
        }
        else
        {
          // Fallback: load sheets without grouping
          LoadSheetsFlat(allSheets.Values.ToList(), result);
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"GetSheetTree Error: {ex.Message}\n\n{ex.StackTrace}", "Error",
          System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
      }

      return result;
    }

    private static void BuildSheetTreeFromFolders(Document doc, BrowserOrganization browserOrg,
      Dictionary<ElementId, ViewSheet> allSheets, List<NMK_M_SheetAndView> result)
    {
      try
      {
        // Build a tree structure based on BrowserOrganization folder hierarchy
        // Each sheet can have a path like: Folder1 > Folder2 > Sheet
        // We use GetFolderItems(sheetId) to get the folder path for each sheet

        // Dictionary to store folder hierarchy for each sheet
        var folderPaths = new Dictionary<ViewSheet, List<string>>();

        foreach (var sheet in allSheets.Values)
        {
          try
          {
            // Get folder items for this sheet - returns the folder hierarchy
            var folderItems = browserOrg.GetFolderItems(sheet.Id);
            var pathParts = new List<string>();

            if (folderItems != null && folderItems.Count > 0)
            {
              foreach (var folderItem in folderItems)
              {
                if (!string.IsNullOrEmpty(folderItem.Name))
                {
                  pathParts.Add(folderItem.Name);
                }
              }
            }

            folderPaths[sheet] = pathParts;
          }
          catch
          {
            // If GetFolderItems fails, sheet has no folder
            folderPaths[sheet] = new List<string>();
          }
        }

        // Build tree structure from folder paths
        // Root level nodes dictionary
        var rootNodes = new Dictionary<string, NMK_M_SheetAndView>();

        foreach (var kvp in folderPaths)
        {
          var sheet = kvp.Key;
          var path = kvp.Value;

          if (path.Count == 0)
          {
            // Sheet has no folder, add directly to root
            result.Add(CreateSheetItem(sheet));
          }
          else
          {
            // Navigate/create folder hierarchy
            NMK_M_SheetAndView currentParent = null;
            Dictionary<string, NMK_M_SheetAndView> currentLevel = rootNodes;

            for (int i = 0; i < path.Count; i++)
            {
              var folderName = path[i];

              if (!currentLevel.ContainsKey(folderName))
              {
                var newFolder = CreateFolderItem(folderName);

                currentLevel[folderName] = newFolder;

                if (currentParent != null)
                {
                  currentParent.Items.Add(newFolder);
                }
              }

              currentParent = currentLevel[folderName];

              // Prepare next level dictionary (for children of this folder)
              if (i < path.Count - 1)
              {
                // Build a dictionary from existing children for fast lookup
                currentLevel = new Dictionary<string, NMK_M_SheetAndView>();
                foreach (var child in currentParent.Items)
                {
                  if (child.ViewSheet == null && !currentLevel.ContainsKey(child.Name))
                  {
                    currentLevel[child.Name] = child;
                  }
                }
              }
            }

            // Add sheet to the deepest folder
            if (currentParent != null)
            {
              currentParent.Items.Add(CreateSheetItem(sheet));
            }
          }
        }

        // Add root folders to result (sorted)
        foreach (var rootFolder in rootNodes.Values.OrderBy(f => f, new F_SortViewSheetName()))
        {
          // Sort items within each folder recursively
          SortFolderItems(rootFolder);
          result.Add(rootFolder);
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"BuildSheetTreeFromFolders Error: {ex.Message}\n\n{ex.StackTrace}", "Error",
          System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        LoadSheetsFlat(allSheets.Values.ToList(), result);
      }
    }

    private static void SortFolderItems(NMK_M_SheetAndView folder)
    {
      // Separate folders and sheets
      var folders = folder.Items.Where(i => i.ViewSheet == null).OrderBy(x => x, new F_SortViewSheetName()).ToList();
      var sheets = folder.Items.Where(i => i.ViewSheet != null).OrderBy(x => x, new F_SortViewSheetName()).ToList();

      folder.Items.Clear();

      // Add folders first (sorted), then sheets (sorted)
      foreach (var f in folders)
      {
        SortFolderItems(f); // Recursively sort
        folder.Items.Add(f);
      }

      foreach (var s in sheets)
      {
        folder.Items.Add(s);
      }
      folder.CountTotal = folder.Items.Count;
    }

    private static void LoadSheetsFlat(List<ViewSheet> sheets, List<NMK_M_SheetAndView> result)
    {
      try
      {
        foreach (var sheet in sheets.OrderBy(x => x.SheetNumber))
        {
          result.Add(CreateSheetItem(sheet));
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"LoadSheetsFlat Error: {ex.Message}", "Error",
          System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
      }
    }
  }
}
