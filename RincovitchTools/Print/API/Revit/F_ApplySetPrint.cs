using RincovitchTools.General.Revit;
using RincovitchTools.Print.Models;
using RincovitchTools.Print.Models.ModelChilds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace RincovitchTools.Print.API.Revit
{
  public static class F_ApplySetPrint
  {
    public static void ApplySetPrint(NMK_M NMK_M , List<string> sheetNumbersInList)
    {
      try
      {
        // Đệ quy check các sheet trong treeview nếu có trong sheetlist, ngược lại bỏ check
        void CheckSheetsInTree(IEnumerable<NMK_M_SheetAndView> nodes)
        {
          foreach (var node in nodes)
          {
            if (node.IsItem && node.ViewSheet != null)
            {
              node.IsChecked = sheetNumbersInList.Contains(F_Versions.get_id(node.ViewSheet.Id));
            }
            if (node.Items != null && node.Items.Count > 0)
            {
              CheckSheetsInTree(node.Items);
            }
          }
        }
        CheckSheetsInTree(NMK_M.SelectionSheets);
        CheckSheetsInTree(NMK_M.SelectionViews);

        // Đệ quy cập nhật trạng thái parent (nếu tất cả con được check thì parent cũng check)
        void UpdateParentCheckedStateAll(IEnumerable<NMK_M_SheetAndView> nodes)
        {
          foreach (var node in nodes)
          {
            if (node.Items != null && node.Items.Count > 0)
            {
              UpdateParentCheckedStateAll(node.Items);
              bool? allChecked = node.Items.All(c => c.IsChecked == true) ? true : (node.Items.Any(c => c.IsChecked == true) ? null : false);
              node.IsChecked = allChecked;
              node.Count = node.Items.Count(c => c.IsChecked == true);
            }
          }
        }
        UpdateParentCheckedStateAll(NMK_M.SelectionSheets);
        UpdateParentCheckedStateAll(NMK_M.SelectionViews);

        // SheetViews chỉ chứa các sheet đã được check
        NMK_M.SheetViews.Clear();
        void AddCheckedSheets(IEnumerable<NMK_M_SheetAndView> nodes)
        {
          foreach (var node in nodes)
          {
            UpdateSheetViews(NMK_M, node);
          }
        }
        AddCheckedSheets(NMK_M.SelectionSheets);
        AddCheckedSheets(NMK_M.SelectionViews);

        // Sắp xếp lại SheetViews nếu cần
        SortAndUpdateIndex(NMK_M);
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"SheetBySchedulesDropDownClosed Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    public static void UpdateSheetViews(NMK_M NMK_M, NMK_M_SheetAndView item)
    {
      // If item is a sheet/view (not a folder), add or remove from SheetViews
      if (item.IsItem)
      {
        if (item.IsChecked == true)
        {
          // Add to SheetViews if entries for the specific format do not already exist
          if (NMK_M.Format.PDF)
          {
            bool existsPdf = NMK_M.SheetViews.Any(x => x.ViewSheet != null && x.ViewSheet.Id == item.ViewSheet.Id && x.Format == "PDF");
            if (!existsPdf)
            {
              var item_pdf = new NMK_M_SheetAndView()
              {
                IsChecked = item.IsChecked,
                NameDefault = item.NameDefault,
                SheetNumber = item.SheetNumber,
                SheetName = item.SheetName,
                Revision = item.Revision,
                RevisionDate = item.RevisionDate,
                Size = item.Size,
                Size_W = item.Size_W,
                Size_H = item.Size_H,
                Format = "PDF",
                Orientation = item.Orientation,
                ViewSheet = item.ViewSheet,
                IsSheet = item.IsSheet,
              };
              item_pdf.SheetName = F_APIRevit_Parameter.UpdateName(NMK_M, item_pdf);
              NMK_M.SheetViews.Add(item_pdf);
            }
          }

          if (NMK_M.Format.DWG)
          {
            bool existsDwg = NMK_M.SheetViews.Any(x => x.ViewSheet != null && x.ViewSheet.Id == item.ViewSheet.Id && x.Format == "DWG");
            if (!existsDwg)
            {
              var item_dwg = new NMK_M_SheetAndView()
              {
                IsChecked = item.IsChecked,
                NameDefault = item.NameDefault,
                SheetNumber = item.SheetNumber,
                SheetName = item.SheetName,
                Revision = item.Revision,
                RevisionDate = item.RevisionDate,
                Size = item.Size,
                Format = "DWG",
                Orientation = item.Orientation,
                ViewSheet = item.ViewSheet,
                IsSheet = item.IsSheet,
              };
              item_dwg.SheetName = F_APIRevit_Parameter.UpdateName(NMK_M, item_dwg);
              NMK_M.SheetViews.Add(item_dwg);
            }
          }
        }
        else
        {
          // Remove from SheetViews
          var remove = NMK_M.SheetViews.Where(x => x.ViewSheet != null && x.ViewSheet.Id == item.ViewSheet.Id).ToList();
          foreach (var item_ in remove)
            NMK_M.SheetViews.Remove(item_);
        }
      }

      // Process children recursively
      if (item.Items != null && item.Items.Count > 0)
      {
        foreach (var child in item.Items)
        {
          UpdateSheetViews(NMK_M, child);
        }
      }

      // Sort and update Index
      SortAndUpdateIndex(NMK_M);
    }

    public static void SortAndUpdateIndex(NMK_M NMK_M)
    {
      // Sort by SheetNumber
      var sortedItems = NMK_M.SheetViews
        .OrderByDescending(x => x.Format)
        .ThenBy(x => x.SheetNumber)
        .ToList();

      NMK_M.SheetViews.Clear();
      int i = 0;
      foreach (var item in sortedItems)
      {
        if (item.Format == "PDF")
        {
          item.Index = i + 1;
          i++;
        }
        else
          item.Index = 0;
        NMK_M.SheetViews.Add(item);

        var paper = NMK_M.Settings_PDF.Printers_PaperSize.Any(x => x.Name == item.Size || item.Size == "-");
        if (!paper)
        {
          PaperFormManager.CreatePaperFormMM(item.Size, item.Size_H, item.Size_W);
        }
      }

      NMK_M.Settings_Print.SheetCountCurrent = NMK_M.SheetViews.Where(x => x.IsSheet).GroupBy(x => F_Versions.get_id(x.ViewSheet.Id)).Count();
      NMK_M.Settings_Print.ViewCountCurrent = NMK_M.SheetViews.Where(x => !x.IsSheet).GroupBy(x => F_Versions.get_id(x.ViewSheet.Id)).Count();
      NMK_M.Settings_Print.UpdateSelectStatus();

      NMK_M.Settings_Print.ViewSheetCount = NMK_M.SheetViews.Count();
    }

    public static void SetChildrenChecked(NMK_M_SheetAndView parent, bool? isChecked)
    {
      foreach (var child in parent.Items.Where(x => x.IsVisible))
      {
        child.IsChecked = isChecked;

        // Recursively set children
        if (child.Items != null && child.Items.Count > 0)
        {
          SetChildrenChecked(child, isChecked);
        }
      }
    }

    public static void UpdateParentCheckedState(NMK_M NMK_M,NMK_M_SheetAndView item)
    {
      // Find parent folder in SelectionSheets tree and update its state
      foreach (var rootItem in NMK_M.SelectionSheets)
      {
        if (UpdateParentInTree(rootItem, item))
          break;
      }
      // Find parent folder in SelectionViews tree and update its state
      foreach (var rootItem in NMK_M.SelectionViews)
      {
        if (UpdateParentInTree(rootItem, item))
          break;
      }
    }

    public static bool UpdateParentInTree(NMK_M_SheetAndView current, NMK_M_SheetAndView target)
    {
      // Check if target is a direct child of current
      if (current.Items.Contains(target))
      {
        // Update current's checked state based on all children
        bool? allChecked = current.Items.All(c => c.IsChecked == true) ? true : (current.Items.Any(c => c.IsChecked == true) ? null : false);
        current.IsChecked = allChecked;
        current.Count = current.Items.Count(c => c.IsChecked == true);
        return true;
      }

      // Recursively search in children
      foreach (var child in current.Items)
      {
        if (child.Items != null && child.Items.Count > 0)
        {
          if (UpdateParentInTree(child, target))
          {
            // After child updated, update current's state
            bool? allChecked = current.Items.All(c => c.IsChecked == true) ? true : (current.Items.Any(c => c.IsChecked == true) ? null : false);
            current.IsChecked = allChecked;
            return true;
          }
        }
      }
      current.Count = current.Items.Count(c => c.IsChecked == true);
      return false;
    }
  }
}
