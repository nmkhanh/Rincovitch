using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ClosedXML.Excel;
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
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using View = Autodesk.Revit.DB.View;

namespace RincovitchTools.DrawingRegister
{
  public class Rincovitch_DrawingRegisterViewModel : BaseViewModel
  {
    #region ICommand
    public ICommand SheetBySchedulesDropDownClosedCommand { get; set; }

    public ICommand SaveViewSheetSetCommand { get; set; }

    public ICommand SheetCheckBoxClickCommand { get; set; }

    public ICommand SearchViewSheetCommand { get; set; }
    public ICommand RefreshCommand { get; set; }
    public ICommand CheckAllCommand { get; set; }
    public ICommand CreateDrawingRegisterCommand { get; set; }

    #endregion

    #region M_VM
    NMK_M _NMK_M = new NMK_M();
    public NMK_M NMK_M { get => _NMK_M; set { _NMK_M = value; OnPropertyChanged(); } }
    #endregion

    public Rincovitch_DrawingRegisterViewModel()
    {
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
      CheckAllCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        CheckAll(p);
      });
      CreateDrawingRegisterCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        CreateDrawingRegisterAsync(p);
      });
    }

    async Task LoadedAsync()
    {
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

      }
    }
    #region LoadViewSheet
    async Task LoadViewSheet(Document doc)
    {
      try
      {
        var title_blocks = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_TitleBlocks).WhereElementIsNotElementType().Cast<FamilyInstance>().ToList();
        var RevisionCloud = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_RevisionClouds).WhereElementIsNotElementType().Cast<RevisionCloud>().ToList();

        // Get sheets organized in tree structure using API
        NMK_M.SelectionSheets.Clear();
        NMK_M.SelectionSheets = new ObservableCollection<NMK_M_SheetAndView>(F_APIRevit_GetTreeView_Sheets.GetSheetTree(doc, title_blocks, RevisionCloud));

        // Get sheets organized in tree structure using API
        //NMK_M.SelectionViews.Clear();
        //NMK_M.SelectionViews = new ObservableCollection<NMK_M_SheetAndView>(F_APIRevit_GetTreeView_Views.GetViewTree(doc));

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
          .Where(x => F_Versions.get_id(x.Definition.CategoryId) == ((int)BuiltInCategory.OST_Sheets).ToString()/* || F_Versions.get_id(x.Definition.CategoryId) == ((int)BuiltInCategory.OST_Views).ToString()*/)
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

    List<NMK_M_SheetAndView> SheetsInTree(List<NMK_M_SheetAndView> nodes)
    {
      var result = new List<NMK_M_SheetAndView>();
      if (nodes == null) return result;

      foreach (var node in nodes)
      {
        if (node == null) continue;

        // If node is an item (sheet/view) and matches titleBlock filter, add it
        if (node.IsItem && node.ViewSheet != null && node.IsChecked == true)
        {
          result.Add(node);
        }

        // Recurse into children
        if (node.Items != null && node.Items.Count > 0)
        {
          result.AddRange(SheetsInTree(node.Items.ToList()));
        }
      }

      return result;
    }

    async void SaveViewSheetSetAsync()
    {
      try
      {
        // Collect selected sheets
        var selected = SheetsInTree(NMK_M.SelectionSheets.ToList()).Select(s => s.ViewSheet).ToList();
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
          sheetItem.IsChecked = sheetItem.Items != null && sheetItem.Items.Count > 0 ? sheetItem.Items.All(i => i.IsChecked == true) ? true : sheetItem.Items.Any(i => i.IsChecked == true) ? (bool?)null : false : sheetItem.IsChecked;
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"SheetCheckBoxClicked Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    void CheckAll(object parameter)
    {
      try
      {
        var isCheck = (bool)parameter;
        foreach (var sheetItem in NMK_M.SelectionSheets)
        {
          sheetItem.IsChecked = isCheck;
          // If it's a folder (has children), check/uncheck all children
          if (sheetItem.Items != null && sheetItem.Items.Count > 0)
          {
            F_ApplySetPrint.SetChildrenChecked(sheetItem, sheetItem.IsChecked);
          }

          // Update parent folder state
          F_ApplySetPrint.UpdateParentCheckedState(NMK_M, sheetItem);
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
          item.Filter(NMK_M.SearchViewSheet);
        foreach (var item in NMK_M.SelectionViews)
          item.Filter(NMK_M.SearchViewSheet);
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
        LoadedAsync();
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"Refresh Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }


    async Task CreateDrawingRegisterAsync(object parameter)
    {
      try
      {
        var selected = SheetsInTree(NMK_M.SelectionSheets.ToList()).Select(s => s.ViewSheet as ViewSheet).ToList();
        if (selected.Count == 0)
        {
          System.Windows.MessageBox.Show("No sheets selected to create.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
          return;
        }

        Document doc = null;
        await RevitTask.RunAsync((uiapp) =>
        {
          doc = uiapp.ActiveUIDocument.Document;
        });
        // Extract project information
        var projectInfo = ExtractProjectInformation(doc);

        // Get revision data
        var revisionData = GetRevisionData(doc, selected);

        // Get save location
        var saveFileDialog = new SaveFileDialog
        {
          Filter = "Excel Files|*.xlsx",
          Title = "Save Drawing Register",
          FileName = "DrawingRegister.xlsx"
        };

        if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
        // Export to Excel
        ExportToExcel(selected, revisionData, projectInfo, saveFileDialog.FileName);

        // Open the file
        var psi = new ProcessStartInfo(saveFileDialog.FileName)
        {
          UseShellExecute = true
        };
        Process.Start(psi);

        Autodesk.Revit.UI.TaskDialog.Show("Drawing Register", "Drawing register exported successfully!");
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"Refresh Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private ProjectInfo ExtractProjectInformation(Document doc)
    {
      var projectInfo = doc.ProjectInformation;
      var info = new ProjectInfo();

      // Project Number
      info.ProjectNumber = GetParameterValue(projectInfo, "Rincovitch Line 9 Job Number")
          ?? GetParameterValue(projectInfo, "Project Number")
          ?? "-";

      // Project Name
      info.ProjectName = GetParameterValue(projectInfo, "Rincovitch Line 1 Project Name")
          ?? GetParameterValue(projectInfo, "Project Name")
          ?? "-";

      // Project Address
      var address1 = GetParameterValue(projectInfo, "Rincovitch Line 2 Project Address");
      var address2 = GetParameterValue(projectInfo, "Rincovitch Line 3 Project Suburb");
      if (!string.IsNullOrEmpty(address1))
      {
        info.ProjectAddress = address1 + (!string.IsNullOrEmpty(address2) ? " " + address2 : "");
      }
      else
      {
        info.ProjectAddress = GetParameterValue(projectInfo, "Project Address") ?? "-";
      }

      // Client
      info.ClientName = GetParameterValue(projectInfo, "Rincovitch Line 5 Client")
          ?? GetParameterValue(projectInfo, "Client Name")
          ?? "-";

      return info;
    }

    private string GetParameterValue(Element element, string parameterName)
    {
      var param = element.LookupParameter(parameterName);
      return param?.AsString();
    }

    private RevisionDataCollection GetRevisionData(Document doc, List<ViewSheet> sheets)
    {
      var revisionData = new RevisionDataCollection();
      var revSequenceNumbers = new List<int>();

      foreach (var sheet in sheets)
      {
        var revIds = sheet.GetAllRevisionIds();
        foreach (var revId in revIds)
        {
          var rev = doc.GetElement(revId) as Revision;
          if (rev != null)
          {
            revSequenceNumbers.Add(rev.SequenceNumber);
          }
        }
      }

      var sortedSequenceNumbers = revSequenceNumbers.OrderBy(x => x).ToList();

      foreach (var seqNum in sortedSequenceNumbers)
      {
        foreach (var sheet in sheets)
        {
          var revIds = sheet.GetAllRevisionIds();
          foreach (var revId in revIds)
          {
            var rev = doc.GetElement(revId) as Revision;
            if (rev != null && rev.SequenceNumber == seqNum)
            {
              revisionData.AddRevision(seqNum, rev.RevisionDate);
              goto NextSequence;
            }
          }
        }
      NextSequence:;
      }

      return revisionData;
    }

    private void ExportToExcel(List<ViewSheet> sheets, RevisionDataCollection revisionData,
        ProjectInfo projectInfo, string filePath)
    {
      using (var workbook = new XLWorkbook())
      {
        var worksheet = workbook.Worksheets.Add("Drawing Register");

        int dataGridOffsetRow = 7;
        int dataGridOffsetColumn = 3;

        // Write project information header
        worksheet.Cell(1, 1).Value = "DRAWING REGISTER";
        worksheet.Cell(1, 1).Style.Font.FontSize = 24;
        worksheet.Cell(1, 1).Style.Font.Bold = true;

        worksheet.Cell(2, 1).Value = "Project No.:  " + projectInfo.ProjectNumber;
        worksheet.Cell(3, 1).Value = "Project Name:  " + projectInfo.ProjectName;
        worksheet.Cell(4, 1).Value = "Project Address: " + projectInfo.ProjectAddress;
        worksheet.Cell(5, 1).Value = "Client: " + projectInfo.ClientName;

        // Write headers
        worksheet.Cell(dataGridOffsetRow, 1).Value = "Sheet No.";
        worksheet.Cell(dataGridOffsetRow, 2).Value = "Sheet Name";
        worksheet.Cell(dataGridOffsetRow, 3).Value = "Curr. Rev.";

        // Write sheet data
        var document = sheets[0].Document;
        for (int i = 0; i < sheets.Count; i++)
        {
          var sheet = sheets[i];
          int row = i + dataGridOffsetRow + 1;

          worksheet.Cell(row, 1).Value = sheet.SheetNumber;
          worksheet.Cell(row, 2).Value = sheet.Name;

          // Get current revision
          var currentRevId = sheet.GetCurrentRevision();
          string currentRev = "-";
          if (F_Versions.get_id_int(currentRevId) != -1)
          {
            currentRev = sheet.GetRevisionNumberOnSheet(currentRevId);
          }
          worksheet.Cell(row, 3).Value = currentRev;
        }

        // Write revision dates and numbers
        for (int i = 0; i < revisionData.SequenceNumbers.Count; i++)
        {
          int col = i + dataGridOffsetColumn + 1;
          worksheet.Cell(dataGridOffsetRow, col).Value = revisionData.RevisionDates[i];

          for (int j = 0; j < sheets.Count; j++)
          {
            int row = j + dataGridOffsetRow + 1;
            var sheet = sheets[j];
            var revIds = sheet.GetAllRevisionIds();

            foreach (ElementId revId in revIds)
            {
              var rev = document.GetElement(revId) as Revision;
              if (rev != null && rev.SequenceNumber == revisionData.SequenceNumbers[i])
              {
                worksheet.Cell(row, col).Value = sheet.GetRevisionNumberOnSheet(revId);
                break;
              }
            }
          }
        }

        int dataEndRow = dataGridOffsetRow + sheets.Count;
        int dataEndColumn = dataGridOffsetColumn + revisionData.SequenceNumbers.Count;

        // Auto-fit columns
        worksheet.Columns().AdjustToContents();

        // Center align revision columns
        var revisionRange = worksheet.Range(dataGridOffsetRow, 3, dataEndRow, dataEndColumn);
        revisionRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Apply borders to entire data range
        var dataRange = worksheet.Range(dataGridOffsetRow, 1, dataEndRow, dataEndColumn);
        dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
        dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        // Top row border
        var topRowRange = worksheet.Range(dataGridOffsetRow, 1, dataGridOffsetRow, dataEndColumn);
        topRowRange.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

        // Sheet info border
        var sheetInfoRange = worksheet.Range(dataGridOffsetRow, 1, dataEndRow, 2);
        sheetInfoRange.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

        // Current revision column - highlight
        var currentRevRange = worksheet.Range(dataGridOffsetRow, 3, dataEndRow, 3);
        currentRevRange.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
        currentRevRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        // Set print settings
        worksheet.PageSetup.PrintAreas.Add(dataGridOffsetRow, 1, dataEndRow, dataEndColumn);
        worksheet.PageSetup.SetRowsToRepeatAtTop(dataGridOffsetRow, dataGridOffsetRow);

        // Save workbook
        workbook.SaveAs(filePath);
      }
    }
  }


  // Helper classes
  public class ProjectInfo
  {
    public string ProjectNumber { get; set; }
    public string ProjectName { get; set; }
    public string ProjectAddress { get; set; }
    public string ClientName { get; set; }
  }

  public class RevisionDataCollection
  {
    public List<int> SequenceNumbers { get; private set; }
    public List<string> RevisionDates { get; private set; }

    public RevisionDataCollection()
    {
      SequenceNumbers = new List<int>();
      RevisionDates = new List<string>();
    }

    public void AddRevision(int sequenceNumber, string revisionDate)
    {
      SequenceNumbers.Add(sequenceNumber);
      RevisionDates.Add(revisionDate);
    }
  }
}
