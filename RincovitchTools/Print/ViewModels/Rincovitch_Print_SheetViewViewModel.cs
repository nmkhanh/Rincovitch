using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json;
using Revit.Async;
using RincovitchTools.General.Revit;
using RincovitchTools.Print.API.Revit;
using RincovitchTools.Print.Models;
using RincovitchTools.Print.Models.ModelChilds;
using RincovitchTools.Print.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using View = Autodesk.Revit.DB.View;

namespace RincovitchTools.Print
{
  public class Rincovitch_Print_SheetViewViewModel : BaseViewModel
  {
    #region ICommand
    public ICommand SheetBySchedulesDropDownClosedCommand { get; set; }

    public ICommand SaveViewSheetSetCommand { get; set; }

    public ICommand SheetCheckBoxClickCommand { get; set; }

    public ICommand SearchViewSheetCommand { get; set; }
    public ICommand RefreshCommand { get; set; }

    #endregion

    #region M_VM
    NMK_VM _NMK_VM;
    public NMK_VM NMK_VM { get => _NMK_VM; set { _NMK_VM = value; OnPropertyChanged(); } }

    NMK_M _NMK_M;
    public NMK_M NMK_M { get => _NMK_M; set { _NMK_M = value; OnPropertyChanged(); } }
    #endregion

    public Rincovitch_Print_SheetViewViewModel(NMK_VM _NMK_VM_, NMK_M _NMK_M_)
    {
      NMK_VM = _NMK_VM_;
      NMK_M = _NMK_M_;
      LoadedAsync();

      SheetBySchedulesDropDownClosedCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        SheetBySchedulesDropDownClosed(p);
      });

      SaveViewSheetSetCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        SaveViewSheetSetAsync();
      });

      SheetCheckBoxClickCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        SheetCheckBoxClicked(p);
      });

      SearchViewSheetCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        SearchViewSheet(p);
      });
      RefreshCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        Refresh(p);
      });
    }

    async Task LoadedAsync()
    {
      NMK_M.IsVisible_ProgressBar.Selection_Sheet = true;
      try
      {
        Document doc = null;
        await RevitTask.RunAsync((uiapp) =>
        {
          doc = uiapp.ActiveUIDocument.Document;
        });
        await LoadViewSheet(doc);
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"LoadedAsync Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
      finally
      {
        NMK_M.IsVisible_ProgressBar.Selection_Sheet = false;
      }
    }
    #region LoadViewSheet
    async Task LoadViewSheet(Document doc)
    {
      try
      {
        var title_blocks = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_TitleBlocks).WhereElementIsNotElementType().Cast<FamilyInstance>().ToList();
        var RevisionCloud = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_RevisionClouds).WhereElementIsNotElementType().Cast<RevisionCloud>().ToList();

        NMK_M.IsVisible_ProgressBar.Selection_Sheet = true;
        // Get sheets organized in tree structure using API
        NMK_M.SelectionSheets.Clear();
        NMK_M.SelectionSheets = new ObservableCollection<NMK_M_SheetAndView>(F_APIRevit_GetTreeView_Sheets.GetSheetTree(doc, title_blocks, RevisionCloud));

        // Get sheets organized in tree structure using API
        NMK_M.SelectionViews.Clear();
        NMK_M.SelectionViews = new ObservableCollection<NMK_M_SheetAndView>(F_APIRevit_GetTreeView_Views.GetViewTree(doc));

        // Ví dụ: lấy sheet trong schedule
        var sheets = GetSheetsFromViewSchedule(doc);
        sheets.AddRange(GetViewSheetsFromViewSheetSet(doc));
        sheets.Insert(0, new NMK_M_SheetAndView_List()
        {
          Name = "Select by user",
        });

        NMK_M.ListViewSheets.Clear();
        NMK_M.ListViewSheets = new ObservableCollection<NMK_M_SheetAndView_List>(sheets);
        if(NMK_M.ListViewSheets.Count() == 1)
        {
          List<string> data = new List<string>();
          if (!string.IsNullOrEmpty(Properties.Settings.Default.SheetSelected))
            data.AddRange(JsonConvert.DeserializeObject<List<string>>(Properties.Settings.Default.SheetSelected));
          if (!string.IsNullOrEmpty(Properties.Settings.Default.ViewSelected))
            data.AddRange(JsonConvert.DeserializeObject<List<string>>(Properties.Settings.Default.ViewSelected));
          if (data.Count > 0)
          {
            var sheetNumbersInList_saved = data.ToList();
            F_ApplySetPrint.ApplySetPrint(NMK_M, sheetNumbersInList_saved);
          }
        }
        else
        {
          if (NMK_M.ListViewSheets.Any(x => x.Name == Properties.Settings.Default.SelectBy) && Properties.Settings.Default.SelectBy != "Select by user")
          {
            NMK_M.ListViewSheet = NMK_M.ListViewSheets.FirstOrDefault(x => x.Name == Properties.Settings.Default.SelectBy);
            var sheetNumbersInList = NMK_M.ListViewSheet.ViewSheets.Select(s => F_Versions.get_id(s.Id)).ToList();
            F_ApplySetPrint.ApplySetPrint(NMK_M, sheetNumbersInList);
          }
          else
          {
            NMK_M.ListViewSheet = NMK_M.ListViewSheets[0];
            List<string> data = new List<string>();
            if (!string.IsNullOrEmpty(Properties.Settings.Default.SheetSelected))
              data.AddRange(JsonConvert.DeserializeObject<List<string>>(Properties.Settings.Default.SheetSelected));
            if (!string.IsNullOrEmpty(Properties.Settings.Default.ViewSelected))
              data.AddRange(JsonConvert.DeserializeObject<List<string>>(Properties.Settings.Default.ViewSelected));
            if (data.Count > 0)
            {
              var sheetNumbersInList = data.ToList();
              F_ApplySetPrint.ApplySetPrint(NMK_M, sheetNumbersInList);
            }
          }
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"LoadedAsync Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
      finally
      {
        NMK_M.IsVisible_ProgressBar.Selection_Sheet = false;
      }
    }

    // Hàm lấy danh sách sheet từ ViewSchedule với bộ lọc áp dụng
    List<NMK_M_SheetAndView_List> GetSheetsFromViewSchedule(Document doc)
    {
      var result = new List<NMK_M_SheetAndView_List>();
      try
      {
        var schedule_sheets = new FilteredElementCollector(doc)
          .OfClass(typeof(ViewSchedule))
          .WhereElementIsNotElementType()
          .Cast<ViewSchedule>()
          .Where(x => x.IsTemplate == false)
          .Where(x => F_Versions.get_id(x.Definition.CategoryId) == ((int)BuiltInCategory.OST_Sheets).ToString() || F_Versions.get_id(x.Definition.CategoryId) == ((int)BuiltInCategory.OST_Views).ToString())
          .ToList();
        foreach (var item in schedule_sheets)
        {
          var sheets = new FilteredElementCollector(doc, item.Id)
            .OfClass(typeof(View))
            .WhereElementIsNotElementType()
            .Cast<View>()
            .ToList();
          result.Add(new NMK_M_SheetAndView_List
          {
            Name = $"Schedule : {item.Name}",
            ViewSheets = new ObservableCollection<View>(sheets),
            Status = 1
          });
        }
        result.OrderBy(x => x.Name);
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show(ex.Message + Environment.NewLine + "Error get documents");
      }
      return result;
    }
    // Hàm lấy danh sách viewsheet từ ViewSheetSet
    List<NMK_M_SheetAndView_List> GetViewSheetsFromViewSheetSet(Document doc)
    {
      var result = new List<NMK_M_SheetAndView_List>();
      try
      {
        var viewsheetset = new FilteredElementCollector(doc).OfClass(typeof(ViewSheetSet)).Cast<ViewSheetSet>().ToList();
        foreach (var item in viewsheetset)
        {
          result.Add(new NMK_M_SheetAndView_List()
          {
            Name = $"ViewSheetSet : {item.Name}",
            ViewSheets = new ObservableCollection<View>(item.Views.Cast<View>()),
          });
        }
        result.OrderBy(x => x.Name);
      }
      catch (Exception)
      {

      }
      return result;
    }
    #endregion


    void SheetBySchedulesDropDownClosed(object parameter)
    {
      try
      {
        // Lấy sheetlist (schedule) đang được chọn từ combobox
        var selectedSheetList = NMK_M.ListViewSheet;
        if (selectedSheetList == null) return;

        var sheetNumbersInList = selectedSheetList.ViewSheets.Select(s => F_Versions.get_id(s.Id)).ToList();

        F_ApplySetPrint.ApplySetPrint(NMK_M, sheetNumbersInList);
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"SheetBySchedulesDropDownClosed Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    async void SaveViewSheetSetAsync()
    {
      try
      {
        // Collect selected sheets
        var selected = NMK_M.SheetViews.Where(s => s.IsChecked == true && s.ViewSheet != null).Select(s => s.ViewSheet).ToList();
        if (selected.Count == 0)
        {
          System.Windows.MessageBox.Show("No sheets selected to save.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
          return;
        }

        string setName = NMK_M.ViewSheetSet_Name;
        if (string.IsNullOrWhiteSpace(setName))
        {
          System.Windows.MessageBox.Show("Please enter a valid ViewSheetSet name.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
          return;
        }

        await RevitTask.RunAsync((uiapp) =>
        {
          UIDocument uidoc = uiapp.ActiveUIDocument;
          Document doc = uidoc.Document;

          PrintManager pm = doc.PrintManager;
          pm.PrintRange = Autodesk.Revit.DB.PrintRange.Select;
          ViewSheetSetting vss = pm.ViewSheetSetting;
          using (Transaction tx = new Transaction(doc, "Create ViewSheetSet"))
          {
            try
            {
              tx.Start();
              var existing = new FilteredElementCollector(doc)
                             .OfClass(typeof(ViewSheetSet))
                             .Cast<ViewSheetSet>()
                             .FirstOrDefault(vs => vs.Name == setName);
              if (existing != null)
              {
                vss.CurrentViewSheetSet = existing;
                vss.Delete();
              }

              ViewSet myViewSet = new ViewSet();
              foreach (var item in selected)
              {
                myViewSet.Insert(item);
              }
              vss.CurrentViewSheetSet.Views = myViewSet;

              bool ok = vss.SaveAs(setName);
              if (ok)
              {
                NMK_M.ListViewSheets.Add(new NMK_M_SheetAndView_List()
                {
                  Name = $"ViewSheetSet : {setName}",
                  ViewSheets = new ObservableCollection<View>(selected),
                });
                NMK_M.ListViewSheets.OrderBy(x => x.Name);
              }
              tx.Commit();
            }
            catch
            {
              if (tx.GetStatus() == TransactionStatus.Started)
                tx.RollBack();
              throw;
            }
          }
        });

        System.Windows.MessageBox.Show($"ViewSheetSet '{setName}' created.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"SaveViewSheetSet Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    #region SheetCheckBoxClicked
    void SheetCheckBoxClicked(object parameter)
    {
      try
      {
        if (parameter is NMK_M_SheetAndView sheetItem)
        {
          // If it's a folder (has children), check/uncheck all children
          if (sheetItem.Items != null && sheetItem.Items.Count > 0)
          {
            F_ApplySetPrint.SetChildrenChecked(sheetItem, sheetItem.IsChecked);
          }

          // Update parent folder state
          F_ApplySetPrint.UpdateParentCheckedState(NMK_M, sheetItem);

          // Add or remove item from SheetViews based on check state
          F_ApplySetPrint.UpdateSheetViews(NMK_M, sheetItem);
          sheetItem.IsChecked = sheetItem.Items != null && sheetItem.Items.Count > 0 ? sheetItem.Items.All(i => i.IsChecked == true) ? true : sheetItem.Items.Any(i => i.IsChecked == true) ? (bool?)null : false : sheetItem.IsChecked;
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"SheetCheckBoxClicked Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    
    #endregion

    void SearchViewSheet(object parameter)
    {
      try
      {
        foreach (var item in NMK_M.SelectionSheets)
          item.Filter(NMK_M.Search.ViewSheet);
        foreach (var item in NMK_M.SelectionViews)
          item.Filter(NMK_M.Search.ViewSheet);
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"SearchViewSheet Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }
    void Refresh(object parameter)
    {
      try
      {
        NMK_VM.SheetViewVM = new Rincovitch_Print_SheetViewViewModel(NMK_VM, NMK_M);
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"Refresh Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }
  }

}
