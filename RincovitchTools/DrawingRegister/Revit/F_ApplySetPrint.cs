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

namespace RincovitchTools.DrawingRegister
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


      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"SheetBySchedulesDropDownClosed Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
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
