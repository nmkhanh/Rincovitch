using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using Newtonsoft.Json;
using Revit.Async;
using RincovitchTools.DrawingRegister;
using RincovitchTools.General.Revit;
using RincovitchTools.Print.API.Revit;
using RincovitchTools.Print.Models;
using RincovitchTools.Print.Models.ModelChilds;
using RincovitchTools.Print.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using F_ApplySetPrint = RincovitchTools.Print.API.Revit.F_ApplySetPrint;
using NMK_M = RincovitchTools.Print.Models.NMK_M;
using View = Autodesk.Revit.DB.View;

namespace RincovitchTools.Print
{
  public class Rincovitch_PrintViewModel : BaseViewModel
  {
    #region ICommand
    public ICommand LoadCommand { get; set; }

    public ICommand ClosingCommand { get; set; }

    public ICommand CustomFileNameCommand { get; set; }



    public ICommand RunCommand { get; set; }

    public ICommand SelectFolderCommand { get; set; }

    public ICommand OpenFolderCommand { get; set; }

    public ICommand PDFCommand { get; set; }
    public ICommand DWGCommand { get; set; }

    #endregion

    #region M_VM
    NMK_VM _NMK_VM = new NMK_VM();
    public NMK_VM NMK_VM { get => _NMK_VM; set { _NMK_VM = value; OnPropertyChanged(); } }

    NMK_M _NMK_M = new NMK_M();
    public NMK_M NMK_M { get => _NMK_M; set { _NMK_M = value; OnPropertyChanged(); } }
    #endregion

    public Rincovitch_PrintViewModel()
    {
      LoadCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        LoadedAsync();
      });

      ClosingCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        ClosingAsync();
      });

      CustomFileNameCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        CustomFileName();
      });

      RunCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        RunAsync();
      });

      SelectFolderCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        SelectFolder();
      });

      OpenFolderCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        OpenFolder();
      });

      PDFCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        PDF();
      });

      DWGCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        DWG();
      });
    }

    async Task LoadedAsync()
    {
      try
      {
        Document doc = null;
        await RevitTask.RunAsync(uiapp =>
        {
          doc = uiapp.ActiveUIDocument.Document;
          using (Transaction tr = new Transaction(doc, "Create schema"))
          {
            tr.Start();
            F_APIGeneral_Revit_Schema.CreateSchema(MVVMSourceProject.schema_NMK, new List<string>
            {
              MVVMSourceProject.schema_customname,
            });

            tr.Commit();
          }
        });

        NMK_VM.SettingsVM = new Rincovitch_Print_SettingsViewModel(NMK_VM, NMK_M);
        NMK_VM.SheetViewVM = new Rincovitch_Print_SheetViewViewModel(NMK_VM, NMK_M);

        await LoadPrinterAsync(doc);

        // Set Project Code for naming convention
        NMK_M.Settings_Print.NamingProjectCode = !string.IsNullOrEmpty(doc.ProjectInformation.BuildingName) ? doc.ProjectInformation.BuildingName : Properties.Settings.Default.ProjectCode;

        // Load parameters for PDF and DWG
        NMK_VM.ParameterVM = new Rincovitch_Print_ParameterViewModel(NMK_VM, NMK_M);
        NMK_M.ForegroundCustomFileName = (NMK_M.ViewSheetParameters_PDF.Where(x => x.IsChecked).Any() || NMK_M.ViewSheetParameters_DWG.Where(x => x.IsChecked).Any()) ? Brushes.OrangeRed : Brushes.White;

        NMK_VM.FilterVM = new Rincovitch_Print_Filter(NMK_VM, NMK_M);
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"LoadedAsync Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    async Task LoadPrinterAsync(Document doc)
    {
      try
      {
        NMK_M.Settings_PDF.Printers = new ObservableCollection<string>(PrinterSettings.InstalledPrinters.Cast<string>().ToList());
        if (NMK_M.Settings_PDF.Printers.Contains("PDF24")) NMK_M.Settings_PDF.SelectedPrinter = "PDF24";
        else
        {
          NMK_M.Settings_PDF.SelectedPrinter = NMK_M.Settings_PDF.Printers.FirstOrDefault();
          System.Windows.MessageBox.Show($"Please install the PDF24 software to ensure a better printing experience and to avoid unexpected errors!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        if (!string.IsNullOrEmpty(NMK_M.Settings_PDF.SelectedPrinter))
        {
          PrinterSettings settings = new PrinterSettings
          {
            PrinterName = NMK_M.Settings_PDF.SelectedPrinter
          };

          NMK_M.Settings_PDF.SelectedPrinter_Window = settings;
          PrintManager printMgr = doc.PrintManager;
          printMgr.SelectNewPrintDriver(NMK_M.Settings_PDF.SelectedPrinter);
          NMK_M.Settings_PDF.Printers_PaperSize = new ObservableCollection<Autodesk.Revit.DB.PaperSize>(printMgr.PaperSizes.Cast<Autodesk.Revit.DB.PaperSize>());
          F_PDF24.SetAutoSave(NMK_M.Settings_Print.FolderSelection);
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"LoadPrinterAsync Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    async Task ClosingAsync()
    {
      try
      {
        List<List<NMK_M_Parameter_Load>> data = new List<List<NMK_M_Parameter_Load>>();
        data.Add(new List<NMK_M_Parameter_Load>()
            {
              new NMK_M_Parameter_Load()
              {
                Name = "PDF Parameters",
                IsCurrent = NMK_M.ViewSheetParameters_PDF_Select == null || string.IsNullOrEmpty(NMK_M.ViewSheetParameters_PDF_Select.Name),
                Datas = NMK_M.ViewSheetParameters_PDF.Select(x => x.Select()).ToList()
              },
              new NMK_M_Parameter_Load()
              {
                Name = "DWG Parameters",
                IsCurrent = NMK_M.ViewSheetParameters_DWG_Select == null || string.IsNullOrEmpty(NMK_M.ViewSheetParameters_DWG_Select.Name),
                Datas = NMK_M.ViewSheetParameters_DWG.Select(x => x.Select()).ToList()
              }
            });


        data.Add(new List<NMK_M_Parameter_Load>(NMK_M.ViewSheetParameters_PDF_Selects.Select(x => new NMK_M_Parameter_Load()
        {
          Name = x.Name,
          IsCurrent = x.IsCurrent,
          Datas = x.Datas.Select(x => x.Select()).ToList()
        })));
        data.Add(new List<NMK_M_Parameter_Load>(NMK_M.ViewSheetParameters_DWG_Selects.Select(x => new NMK_M_Parameter_Load()
        {
          Name = x.Name,
          IsCurrent = x.IsCurrent,
          Datas = x.Datas.Select(x => x.Select()).ToList()
        })));
        var json_data = JsonConvert.SerializeObject(data);

        Properties.Settings.Default.CombineName = NMK_M.Settings_Print.FileCombineName;
        Properties.Settings.Default.CombineNameInfo = NMK_M.Settings_Print.NamingNode;
        Properties.Settings.Default.UseCombineName = NMK_M.Settings_Print.UseNamingConvention;
        Properties.Settings.Default.CurentPath = NMK_M.Settings_Print.FolderSelection;
        Properties.Settings.Default.CreateSeparate = NMK_M.Settings_Print.IsCreateSeparateFiles;
        Properties.Settings.Default.ProjectCode = NMK_M.Settings_Print.NamingProjectCode;
        Properties.Settings.Default.PDF = NMK_M.Format.PDF;
        Properties.Settings.Default.DWG = NMK_M.Format.DWG;
        Properties.Settings.Default.SaveSameFolder = NMK_M.Settings_Print.IsSaveAllFilesInSameFolder;
        Properties.Settings.Default.SelectBy = NMK_M.ListViewSheet != null ? NMK_M.ListViewSheet.Name : "";
        Properties.Settings.Default.SheetSelected = JsonConvert.SerializeObject(SheetsInTree(NMK_M.SelectionSheets.ToList()));
        Properties.Settings.Default.ViewSelected = JsonConvert.SerializeObject(SheetsInTree(NMK_M.SelectionViews.ToList()));
        Properties.Settings.Default.Save();

        // Save selected parameters to schema
        await RevitTask.RunAsync(uiapp =>
        {
          Document doc = uiapp.ActiveUIDocument.Document;
          using (Transaction tr = new Transaction(doc, "Create schema"))
          {
            tr.Start();
            doc.ProjectInformation.BuildingName = NMK_M.Settings_Print.NamingProjectCode;

            F_APIGeneral_Revit_Schema.SetSchema(doc.ProjectInformation, MVVMSourceProject.schema_NMK, MVVMSourceProject.schema_customname, json_data);

            tr.Commit();
          }
        });
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"ClosingAsync Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    List<string> SheetsInTree(List<NMK_M_SheetAndView> nodes)
    {
      var result = new List<string>();
      if (nodes == null) return result;

      foreach (var node in nodes)
      {
        if (node == null) continue;

        // If node is an item (sheet/view) and matches titleBlock filter, add it
        if (node.IsItem && node.ViewSheet != null && node.IsChecked == true)
        {
            result.Add(node.ID);
        }

        // Recurse into children
        if (node.Items != null && node.Items.Count > 0)
        {
          result.AddRange(SheetsInTree(node.Items.ToList()));
        }
      }

      return result;
    }


    async Task CustomFileName()
    {
      try
      {
        NMK_M.IsVisible_Window.Dialog = true;
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"CustomFileName Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }



    async Task RunAsync()
    {
      try
      {
        var rd = new Random();
        if (NMK_M.SheetViews.Count == 0)
        {
          System.Windows.MessageBox.Show("Please select at least one sheet to export.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
          return;
        }

        #region Validate Folder Selection
        string path = NMK_M.Settings_Print.FolderSelection;
        F_APIRevit_ExportPDF.EnsureDirectoryExists(path);
        string path_PDF = path;
        string path_PDF_Combine = path;
        string path_DWG = path;
        if (NMK_M.Settings_Print.IsSaveSplitFilesByFormat)
        {
          path_PDF = System.IO.Path.Combine(path, "PDF");
          path_PDF_Combine = System.IO.Path.Combine(path, "PDF");
          path_DWG = System.IO.Path.Combine(path, "DWG");
          if (!System.IO.Directory.Exists(path_PDF))
            Directory.CreateDirectory(path_PDF);
          if (!System.IO.Directory.Exists(path_PDF_Combine))
            Directory.CreateDirectory(path_PDF_Combine);
          if (!System.IO.Directory.Exists(path_DWG))
            Directory.CreateDirectory(path_DWG);
        }
        #endregion

        #region Initialize progress
        NMK_M.Settings_Print.ProgressValue = 0;
        NMK_M.Settings_Print.ProgressMaximum = 100;

        NMK_M.Settings_Print.ViewSheetCountCurrent = 0;
        NMK_M.Settings_Print.UpdatePrintStatus();
        NMK_M.Settings_Print.IsPrintStatus = true;
        var sheets = NMK_M.SheetViews.ToList();
        int total = sheets.Count;
        int current = 0;

        foreach (var sheet in sheets)
        {
          sheet.ProgressValue = 0;
          sheet.Error = string.Empty;
        }
        #endregion

        List<string> opens = new List<string>();
        if (NMK_M.Settings_Print.IsCombineMultipleFiles)
        {
          string fileName = NMK_M.Settings_Print.UseNamingConvention ? NMK_M.Settings_Print.FileCombineNameConvention : NMK_M.Settings_Print.FileCombineName;
          string combinedFileName = F_APIRevit_ExportPDF.SanitizeFileName(fileName) + ".pdf";
          var result = F_APIRevit_ExportPDF.IsFileLocked(System.IO.Path.Combine(path_PDF_Combine, combinedFileName), out string reason);
          if (result)
          {
            opens.Add(combinedFileName);
          }
        }
        else
        {
          foreach (var item in sheets)
          {
            string fileName_Separate = F_APIRevit_ExportPDF.SanitizeFileName(item.SheetName + ".pdf");
            var result = F_APIRevit_ExportPDF.IsFileLocked(System.IO.Path.Combine(path_PDF_Combine, fileName_Separate), out string reason);
            if (result)
            {
              opens.Add(fileName_Separate);
            }
          }
        }
        if (opens.Count > 0)
        {
          string message = "Please close the following PDF files before exporting:\n\n";
          message += string.Join("\n", opens);
          System.Windows.MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
          NMK_M.Settings_Print.IsPrintStatus = false;
          return;
        }
        else
        {
          Document doc = null;
          await RevitTask.RunAsync((uiapp) =>
          {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;
            uidoc.Selection.SetElementIds(new Autodesk.Revit.DB.ElementId[] { });
          });
          PrintManager printManager = doc.PrintManager;
          printManager.SelectNewPrintDriver(NMK_M.Settings_PDF.SelectedPrinter);
          NMK_M.Settings_PDF.Printers_PaperSize = new ObservableCollection<Autodesk.Revit.DB.PaperSize>(printManager.PaperSizes.Cast<Autodesk.Revit.DB.PaperSize>());

          #region Export PDF
          List<string> exportedFiles = new List<string>();
          string path_temp = @"C:\ProgramData\Autodesk\ApplicationPlugins\Rincovitch.bundle\Temp";
          if (!Directory.Exists(path_temp))
            Directory.CreateDirectory(path_temp);
          path_PDF = path_temp;
          //foreach (var item in Directory.GetFiles(path_PDF, "*.pdf"))
          //{
          //  File.Delete(item);
          //}

          F_PDF24.SetAutoSaveDir(path_PDF);
          foreach (var sheet in sheets.Where(x => x.Format == "PDF"))
          {
            string fileName_temp = Guid.NewGuid().ToString();
            string fileName = F_APIRevit_ExportPDF.GetFileName(sheet, NMK_M.Settings_Print);
            string fullPath = System.IO.Path.Combine(path_PDF, fileName_temp + ".pdf");

            if (sheet.ViewSheet != null)
            {
              // Fake progress animation for this sheet
              for (int fake = 0; fake < rd.Next(45, 60); fake += 2)
              {
                sheet.ProgressValue = fake;
                double avgProgress = sheets.Average(s => s.ProgressValue);
                NMK_M.Settings_Print.ProgressValue = Math.Round(avgProgress, 0);
                await Task.Delay(5);
              }

              // Export thật sheet này
              await RevitTask.RunAsync((uiapp) =>
              {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
#if D2022 || D2023 || D2024 || D2025 || D2026
                if (sheet.IsSheet)
                  F_APIRevit_ExportPDF.PrintSinglePDF(doc, sheet, path_PDF, fileName_temp, NMK_M.Settings_PDF);
                else
                {
                  var pdfOptions = F_APIRevit_ExportPDF.CreatePDFExportOptions(NMK_M.Settings_PDF, NMK_M.Settings_Print);
                  F_APIRevit_ExportPDF.ExportSinglePDF(doc, sheet, pdfOptions, path_PDF, fileName_temp);
                }
#else
                F_APIRevit_ExportPDF.PrintSinglePDF(doc, sheet, path_PDF, fileName_temp, NMK_M.Settings_PDF);
#endif
              });
              exportedFiles.Add(fullPath);
              if (!NMK_M.Settings_Print.IsCombineMultipleFiles)
              {
                var new_path = System.IO.Path.Combine(path_PDF_Combine, fileName + ".pdf");
                var timeout = DateTime.Now.AddSeconds(NMK_M.PrintTimeout);
                while (!File.Exists(fullPath) && DateTime.Now < timeout && string.IsNullOrEmpty(sheet.Error))
                {
                  await Task.Delay(200);
                }
                if (string.IsNullOrEmpty(sheet.Error))
                  try
                  {
                    File.Copy(fullPath, new_path, true);
                  }
                  catch (Exception ex)
                  {
                    sheet.Error = $"ERROR: {ex.Message}";
                  }
                //exportedFiles.Add(new_path);
              }
            }
            for (int fake = (int)sheet.ProgressValue; fake < 100; fake += 2)
            {
              sheet.ProgressValue = fake;
              await Task.Delay(5);
            }
            current++;
            sheet.ProgressValue = 100;
            double avgProgressFinal = sheets.Average(s => s.ProgressValue);
            NMK_M.Settings_Print.ProgressValue = Math.Round(avgProgressFinal, 0);

            NMK_M.Settings_Print.ViewSheetCountCurrent = current;
            NMK_M.Settings_Print.UpdatePrintStatus();
          }

          // Combine PDFs if needed using PDFSharp
          if (NMK_M.Settings_Print.IsCombineMultipleFiles)
          {
            var timeout = DateTime.Now.AddSeconds(NMK_M.PrintTimeout);
            while (!exportedFiles.All(x => Directory.GetFiles(path_PDF, "*.pdf").Contains(x)) && DateTime.Now < timeout)
            {
              await Task.Delay(200);
            }
            await Task.Delay(500); // Small delay to ensure file write completion
            string fileName = NMK_M.Settings_Print.UseNamingConvention ? NMK_M.Settings_Print.FileCombineNameConvention : NMK_M.Settings_Print.FileCombineName;
            string combinedFileName = F_APIRevit_ExportPDF.SanitizeFileName(fileName) + ".pdf";
            F_APIRevit_ExportPDF.ExportCombinedPDF(exportedFiles, path_PDF_Combine, combinedFileName);
            await Task.Delay(500); // Small delay to ensure file write completion
            //foreach (var item in exportedFiles)
            //{
            //  File.Delete(item);
            //}
          }
#endregion


          #region Export DWG
          foreach (var sheet in sheets.Where(x => x.Format == "DWG"))
          {
            string fileName = F_APIRevit_ExportPDF.GetFileName(sheet, NMK_M.Settings_Print);

            if (sheet.ViewSheet != null)
            {
              // Fake progress animation for this sheet
              for (int fake = 0; fake < rd.Next(45, 60); fake += 2)
              {
                sheet.ProgressValue = fake;
                double avgProgress = sheets.Average(s => s.ProgressValue);
                NMK_M.Settings_Print.ProgressValue = Math.Round(avgProgress, 0);
                await Task.Delay(5);
              }

              // Export thật sheet này
              await RevitTask.RunAsync((uiapp) =>
              {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                F_APIRevit_ExportPDF.ExportDWG(doc, sheet, NMK_M.Settings_DWG.SelectedExportSetup, NMK_M.Settings_DWG.ExportViewsAsExternalReferences, path_DWG, fileName);
              });
            }

            for (int fake = (int)sheet.ProgressValue; fake < 100; fake += 2)
            {
              sheet.ProgressValue = fake;
              await Task.Delay(5);
            }
            current++;
            sheet.ProgressValue = 100;
            double avgProgressFinal = sheets.Average(s => s.ProgressValue);
            NMK_M.Settings_Print.ProgressValue = Math.Round(avgProgressFinal, 0);

            NMK_M.Settings_Print.ViewSheetCountCurrent = current;
            NMK_M.Settings_Print.UpdatePrintStatus();
          }
          #endregion

          #region Finalize progress
          NMK_M.Settings_Print.ProgressValue = 100;
          System.Windows.MessageBox.Show($"Export finish!\nLocation: {path}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
          NMK_M.Settings_Print.IsPrintStatus = false;
          #endregion
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"RunAsync Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    void OpenFolder()
    {
      try
      {

        string folderPath = NMK_M.Settings_Print.FolderSelection;
        if (!System.IO.Directory.Exists(folderPath))
          Directory.CreateDirectory(folderPath);
        Process.Start(new ProcessStartInfo
        {
          FileName = folderPath,
          UseShellExecute = true
        });
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"OpenFolder Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    void SelectFolder()
    {
      try
      {
        //using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
        //{
        //  dialog.Description = "Select folder to save files";
        //  dialog.SelectedPath = NMK_M.Settings_Print.FolderSelection;
        //  dialog.ShowNewFolderButton = true;

        //  if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //  {
        //    NMK_M.Settings_Print.FolderSelection = dialog.SelectedPath;
        //  }
        //}
        //var dialog = new Microsoft.Win32.OpenFolderDialog();
        //dialog.InitialDirectory = NMK_M.Settings_Print.FolderSelection;
        //dialog.Title = "Select export folder";

        //if (dialog.ShowDialog() == true)
        //{
        //  NMK_M.Settings_Print.FolderSelection = dialog.FolderName;
        //}

        string folderPath = NMK_M.Settings_Print.FolderSelection;
        if (!System.IO.Directory.Exists(folderPath))
          Directory.CreateDirectory(folderPath);
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
          CheckFileExists = false,
          CheckPathExists = true,
          Filter = "Folder|*.none",
          FileName = "Select Folder",
          InitialDirectory = folderPath
        };
        if (dialog.ShowDialog() == true)
        {
          NMK_M.Settings_Print.FolderSelection = Path.GetDirectoryName(dialog.FileName);
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"SelectFolder Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    void PDF()
    {
      try
      {
        var sheets = SheetsInTree(NMK_M.SelectionSheets.ToList());
        var views = SheetsInTree(NMK_M.SelectionViews.ToList());
        var sheetNumbersInList = sheets.Union(views).ToList();

        F_ApplySetPrint.ApplySetPrint(NMK_M, sheetNumbersInList);
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"PDF Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    void DWG()
    {
      try
      {
        var sheets = SheetsInTree(NMK_M.SelectionSheets.ToList());
        var views = SheetsInTree(NMK_M.SelectionViews.ToList());
        var sheetNumbersInList = sheets.Union(views).ToList();

        F_ApplySetPrint.ApplySetPrint(NMK_M, sheetNumbersInList);
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"DWG Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }
  }
}
