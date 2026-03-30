using Autodesk.Revit.DB;
using DocumentFormat.OpenXml.Spreadsheet;
using RincovitchTools.General.Revit;
using RincovitchTools.Print.Models.ModelChilds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using View = Autodesk.Revit.DB.View;

namespace RincovitchTools.Print.API.Revit
{
  public static class F_APIRevit_GetTreeView_Views
  {
    private static NMK_M_SheetAndView CreateViewItem(View view)
    {
      string name = view?.Name ?? "";

      return new NMK_M_SheetAndView
      {
        ID = F_Versions.get_id(view.Id),
        ViewSheet = view,
        Name = name,
        NameDefault = name,
        SheetNumber = F_Versions.get_id(view.Id).ToString(),
        SheetName = name,
        IsSheet = false,
        IsItem = true,
      };
    }

    private static NMK_M_SheetAndView CreateFolderItem(string folderName)
    {
      return new NMK_M_SheetAndView
      {
        Name = folderName,
        IsSheet = false,
        IsItem = false,
      };
    }

    public static List<NMK_M_SheetAndView> GetViewTree(Document doc)
    {
      var result = new List<NMK_M_SheetAndView>();

      try
      {
        // Try to get BrowserOrganization for Views
        BrowserOrganization browserOrg = null;
        try
        {
          browserOrg = BrowserOrganization.GetCurrentBrowserOrganizationForViews(doc);
        }
        catch
        {
          browserOrg = null;
        }

        // Collect views (exclude templates and sheets)
        var allViews = new FilteredElementCollector(doc)
          .OfClass(typeof(View))
          .Cast<View>()
          .Where(v => !v.IsTemplate && !(v is ViewSheet) && !(v is ViewSchedule))
          .ToDictionary(v => v.Id, v => v);

        if (browserOrg != null && allViews.Count > 0)
        {
          BuildViewTreeFromFolders(doc, browserOrg, allViews, result);
        }
        else
        {
          // Fallback: flat list
          foreach (var view in allViews.Values.OrderBy(v => v.Name))
          {
            result.Add(CreateViewItem(view));
          }
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"GetViewTree Error: {ex.Message}\n\n{ex.StackTrace}", "Error",
          System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
      }

      return result;
    }

    private static void BuildViewTreeFromFolders(Document doc, BrowserOrganization browserOrg,
      Dictionary<ElementId, View> allViews, List<NMK_M_SheetAndView> result)
    {
      try
      {
        var folderPaths = new Dictionary<View, List<string>>();

        foreach (var view in allViews.Values)
        {
          try
          {
            var folderItems = browserOrg.GetFolderItems(view.Id);
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

            folderPaths[view] = pathParts;
          }
          catch
          {
            folderPaths[view] = new List<string>();
          }
        }

        var rootNodes = new Dictionary<string, NMK_M_SheetAndView>();

        foreach (var kvp in folderPaths)
        {
          var view = kvp.Key;
          var path = kvp.Value;

          if (path.Count == 0)
          {
            result.Add(CreateViewItem(view));
          }
          else
          {
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

              if (i < path.Count - 1)
              {
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

            if (currentParent != null)
            {
              currentParent.Items.Add(CreateViewItem(view));
            }
          }
        }

        foreach (var rootFolder in rootNodes.Values.OrderBy(f => f.Name))
        {
          SortFolderItems(rootFolder);
          result.Add(rootFolder);
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"BuildViewTreeFromFolders Error: {ex.Message}\n\n{ex.StackTrace}", "Error",
          System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

        // Fallback to flat list
        foreach (var v in allViews.Values.OrderBy(v => v.Name))
        {
          result.Add(CreateViewItem(v));
        }
      }
    }

    private static void SortFolderItems(NMK_M_SheetAndView folder)
    {
      // Separate folders and sheets
      var folders = folder.Items.Where(i => i.ViewSheet == null).OrderBy(x => x, new F_SortViewSheetName()).ToList();
      var views = folder.Items.Where(i => i.ViewSheet != null).OrderBy(x => x, new F_SortViewSheetName()).ToList();

      folder.Items.Clear();

      foreach (var f in folders)
      {
        SortFolderItems(f);
        folder.Items.Add(f);
      }

      foreach (var v in views)
      {
        folder.Items.Add(v);
      }
      folder.CountTotal = folder.Items.Count;
    }
  }
}
