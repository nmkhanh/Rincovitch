using Autodesk.Revit.DB;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Expression.Media;
using RincovitchTools.Print.Models;
using RincovitchTools.Print.Models.ModelChilds;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Document = Autodesk.Revit.DB.Document;
using Path = System.IO.Path;
using View = Autodesk.Revit.DB.View;

namespace RincovitchTools.Print.API.Revit
{
  public static class F_APIRevit_ExportPDF
  {
    /// <summary>
    /// Check if file is locked
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="reason"></param>
    /// <returns></returns>
    public static bool IsFileLocked(string filePath, out string reason)
    {
      try
      {
        File.Delete(filePath);
        reason = null;
        return false;
      }
      catch (IOException ex)
      {
        reason = $"File locked: {ex.Message}";
        return true;
      }
      catch (UnauthorizedAccessException ex)
      {
        reason = $"No access: {ex.Message}";
        return true;
      }
    }

    /// <summary>
    /// Create PDF Export Options from settings
    /// </summary>
#if D2022 || D2023 || D2024 || D2025 || D2026
    public static PDFExportOptions CreatePDFExportOptions(NMK_M_Settings_PDF settings, NMK_M_Print printSettings)
    {
      var options = new PDFExportOptions();

      // Paper Placement
      if (settings.PaperPlacementCenter)
      {
        options.PaperPlacement = PaperPlacementType.Center;
      }
      else if (settings.PaperPlacementOffsetFromCorner)
      {
        options.PaperPlacement = PaperPlacementType.LowerLeft;

        if (settings.OffsetNoMargin)
        {
          options.OriginOffsetX = 0;
          options.OriginOffsetY = 0;
        }
        else if (settings.OffsetUserDefined)
        {
          // Convert mm to feet (Revit internal unit)
          options.OriginOffsetX = UnitUtils.ConvertToInternalUnits(settings.OffsetX, UnitTypeId.Millimeters);
          options.OriginOffsetY = UnitUtils.ConvertToInternalUnits(settings.OffsetY, UnitTypeId.Millimeters);
        }
      }

      // Zoom
      if (settings.ZoomFitToPage)
      {
        options.ZoomType = ZoomType.FitToPage;
      }
      else if (settings.ZoomCustom)
      {
        options.ZoomType = ZoomType.Zoom;
        options.ZoomPercentage = (int)settings.ZoomPercentage;
      }

      // Hidden Line Processing
      options.AlwaysUseRaster = settings.RasterProcessing;

      // Raster Quality
      switch (settings.SelectedRasterQuality)
      {
        case "Low":
          options.RasterQuality = RasterQualityType.Low;
          break;
        case "Medium":
          options.RasterQuality = RasterQualityType.Medium;
          break;
        case "High":
          options.RasterQuality = RasterQualityType.High;
          break;
        case "Presentation":
          options.RasterQuality = RasterQualityType.Presentation;
          break;
      }

      // Color
      switch (settings.SelectedColor)
      {
        case "Black Lines":
          options.ColorDepth = ColorDepthType.BlackLine;
          break;
        case "Gray Scale":
          options.ColorDepth = ColorDepthType.GrayScale;
          break;
        case "Color":
          options.ColorDepth = ColorDepthType.Color;
          break;
      }

      // Options
      options.ViewLinksInBlue = settings.ViewLinksInBlue;
      options.HideReferencePlane = settings.HideRefWorkPlanes;
      options.HideUnreferencedViewTags = settings.HideUnreferencedViewTags;
      options.HideScopeBoxes = settings.HideScopeBoxes;
      options.HideCropBoundaries = settings.HideCropBoundaries;
      options.ReplaceHalftoneWithThinLines = settings.ReplaceHalftoneWithThinLines;
      options.MaskCoincidentLines = settings.RegionEdgesMaskCoincidentLines;

      // Combine option
      options.Combine = true;

      // quality
      options.ExportQuality = PDFExportQualityType.DPI600;
      options.PaperFormat = ExportPaperFormat.Default;
      options.PaperOrientation = PageOrientationType.Auto;
      options.StopOnError = false;

      return options;
    }
#endif

    /// <summary>
    /// Ensure directory exists, create if not
    /// </summary>
    public static void EnsureDirectoryExists(string path)
    {
      try
      {
        var root = Path.GetPathRoot(path);

        // chặn ghi trực tiếp vào ổ gốc hoặc Users
        if (string.Equals(path.TrimEnd('\\'), root, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(path.TrimEnd('\\'), @"C:\Users", StringComparison.OrdinalIgnoreCase))
        {
          throw new UnauthorizedAccessException("Cannot create directory at system root.");
        }

        Directory.CreateDirectory(path);
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show(
            $"Directory creation error: {ex.Message}",
            "Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
      }
    }

    /// <summary>
    /// Get file name based on naming convention settings
    /// </summary>
    public static string GetFileName(NMK_M_SheetAndView sheet, NMK_M_Print settings)
    {
      if (sheet.Format == "PDF")
      {
        return SanitizeFileName(sheet.SheetName != sheet.NameDefault ? sheet.SheetName : (sheet.IsSheet ? $"{sheet.SheetNumber}-{sheet.SheetName}" : sheet.SheetName));
      }

      if (sheet.Format == "DWG")
      {
        return SanitizeFileName(sheet.SheetName != sheet.NameDefault ? sheet.SheetName : (sheet.IsSheet ? $"{sheet.SheetNumber}-{sheet.SheetName}" : sheet.SheetName));
      }

      return SanitizeFileName(sheet.SheetNumber);
    }

    /// <summary>
    /// Remove invalid characters from file name
    /// </summary>
    public static string SanitizeFileName(string fileName)
    {
      char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
      foreach (char c in invalidChars)
      {
        fileName = fileName.Replace(c, '_');
      }

      // xử lý khoảng trắng + dấu chấm ở cuối
      fileName = fileName.TrimEnd(' ', '.');


      return fileName;
    }

    /// <summary>
    /// Export single sheet to PDF
    /// </summary>
#if D2022 || D2023 || D2024 || D2025 || D2026
    public static void ExportSinglePDF(Document doc, NMK_M_SheetAndView sheet, PDFExportOptions options, string folderPath, string fileName)
    {
      try
      {
        var viewIds = new List<ElementId> { sheet.ViewSheet.Id };
        options.FileName = fileName;

        doc.Export(folderPath, viewIds, options);
      }
      catch (Exception ex)
      {
        sheet.Error = $"ERROR: {ex.Message}";
      }
    }
#endif

    /// <summary>
    /// Export multiple sheets to combined PDF
    /// </summary>
    public static void ExportCombinedPDF(List<string> exportedFiles, string folderPath, string fileName)
    {
      try
      {
        string combinedFilePath = System.IO.Path.Combine(folderPath, fileName);
        // Tạo file stream để ghi dữ liệu ra
        using (FileStream stream = new FileStream(combinedFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
          using (iTextSharp.text.Document doc = new iTextSharp.text.Document())
          {
            using (PdfCopy pdf = new PdfCopy(doc, stream))
            {
              doc.Open();
              foreach (string file in exportedFiles)
              {
                // Đọc file với quyền chỉ đọc để tránh xung đột
                using (PdfReader reader = new PdfReader(file))
                {
                  pdf.AddDocument(reader);
                  reader.Close();
                }
              }
              doc.Close();
            }
          }
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"PDF combine error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    /// <summary>
    /// Export DWG
    /// </summary>
    public static void ExportDWG(Document doc, NMK_M_SheetAndView sheet, string ExportDWG_Option, bool ExportDWG_MergedViews, string folderPath, string fileName)
    {
      try
      {
        DWGExportOptions options = new DWGExportOptions();
        if (!string.IsNullOrEmpty(ExportDWG_Option))
          options = DWGExportOptions.GetPredefinedOptions(doc, ExportDWG_Option);
        options.MergedViews = !ExportDWG_MergedViews;

        doc.Export(folderPath, fileName, new List<ElementId> { sheet.ViewSheet.Id }, options);
      }
      catch (Exception ex)
      {
        sheet.Error = $"ERROR: {ex.Message}";
      }
    }

    /// <summary>
    /// Print single sheet/view to PDF using Revit PrintManager
    /// </summary>
    /// <param name="doc">Revit document</param>
    /// <param name="viewSheet">View or sheet to print</param>
    /// <param name="printSettingName">Name of the print setting (leave null/empty for current setting)</param>
    /// <param name="printerName">Printer name (leave null/empty to use default or current printer)</param>
    /// <param name="folderPath">Output folder path</param>
    /// <param name="fileName">Output file name (without extension)</param>
    /// <param name="customSetupAction">Optional action to customize PrintSetup before submitting print</param>
    public static void PrintSinglePDF(Document doc, NMK_M_SheetAndView sheet, string folderPath, string fileName, NMK_M_Settings_PDF settings)
    {
      try
      {
        PrintManager printManager = doc.PrintManager;

        printManager.PrintRange = PrintRange.Select;
        printManager.PrintToFile = true;
        printManager.CombinedFile = true;
        printManager.PrintToFileName = System.IO.Path.Combine(folderPath, fileName + ".pdf");

        //settings.SelectedPrinter_Window.PrintFileName = printManager.PrintToFileName;
        //settings.SelectedPrinter_Window.PrintToFile = true;

        // Configure ViewSheetSetting
        ViewSheetSetting viewSheetSetting = printManager.ViewSheetSetting;
        ViewSet viewSet = new ViewSet();
        viewSet.Insert(sheet.ViewSheet);

        using (TransactionGroup g = new TransactionGroup(doc, "Print"))
        {
          g.Start();
          using (Transaction tr = new Transaction(doc, "PrintGroup"))
          {
            tr.Start();
            PrintSetup ps = printManager.PrintSetup;
            ps.SaveAs(settings.SelectedPrinter);
            var setup = doc.GetPrintSettingIds().Select(x => doc.GetElement(x) as Autodesk.Revit.DB.PrintSetting).ToList().First(x => x.Name == settings.SelectedPrinter);
            ps.CurrentPrintSetting = setup;
            CreatePrintSetupAction(settings, sheet, ps.CurrentPrintSetting.PrintParameters);
            printManager.Apply();
            ps.Save();
            doc.Print(viewSet, true);
            tr.Commit();
          }

          using (Transaction tr = new Transaction(doc, "PrintGroup"))
          {
            tr.Start();
            var setup = doc.GetPrintSettingIds().Select(x => doc.GetElement(x) as Autodesk.Revit.DB.PrintSetting).ToList().Where(x => x.Name == settings.SelectedPrinter);
            if (setup != null && setup.Count() > 0)
              doc.Delete(setup.First().Id);
            tr.Commit();
          }
          g.Assimilate();
        }

      }
      catch (Exception ex)
      {
        //System.Windows.MessageBox.Show($"PrintSinglePDF Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        sheet.Error = $"ERROR Print: {ex.Message}";
      }
    }

    public static void CreatePrintSetupAction(NMK_M_Settings_PDF settings, NMK_M_SheetAndView sheet, PrintParameters printParams)
    {
      try
      {
        // Paper Placement
        if (settings.PaperPlacementCenter)
        {
          printParams.PaperPlacement = PaperPlacementType.Center;
        }
        else if (settings.PaperPlacementOffsetFromCorner)
        {
#if D2022 || D2023 || D2024 || D2025 || D2026
          printParams.PaperPlacement = PaperPlacementType.LowerLeft;
#else
          printParams.PaperPlacement = PaperPlacementType.Margins;
#endif

          if (settings.OffsetUserDefined)
          {
            printParams.PaperPlacement = PaperPlacementType.Margins;
            // Convert mm to feet (Revit internal unit)
#if D2022 || D2023 || D2024 || D2025 || D2026
            printParams.OriginOffsetX = UnitUtils.ConvertToInternalUnits(settings.OffsetX, UnitTypeId.Millimeters);
            printParams.OriginOffsetY = UnitUtils.ConvertToInternalUnits(settings.OffsetY, UnitTypeId.Millimeters);
#else
            printParams.UserDefinedMarginX = UnitUtils.ConvertToInternalUnits(settings.OffsetX, DisplayUnitType.DUT_MILLIMETERS);
            printParams.UserDefinedMarginY = UnitUtils.ConvertToInternalUnits(settings.OffsetY, DisplayUnitType.DUT_MILLIMETERS);
#endif
          }
        }

        // Zoom
        if (settings.ZoomFitToPage)
        {
          printParams.ZoomType = ZoomType.FitToPage;
        }
        else if (settings.ZoomCustom)
        {
          printParams.ZoomType = ZoomType.Zoom;
          printParams.Zoom = (int)settings.ZoomPercentage;
        }

        // Raster Quality
        switch (settings.SelectedRasterQuality)
        {
          case "Low":
            printParams.RasterQuality = RasterQualityType.Low;
            break;
          case "Medium":
            printParams.RasterQuality = RasterQualityType.Medium;
            break;
          case "High":
            printParams.RasterQuality = RasterQualityType.High;
            break;
          case "Presentation":
            printParams.RasterQuality = RasterQualityType.Presentation;
            break;
        }

        // Color
        switch (settings.SelectedColor)
        {
          case "Black Lines":
            printParams.ColorDepth = ColorDepthType.BlackLine;
            break;
          case "Gray Scale":
            printParams.ColorDepth = ColorDepthType.GrayScale;
            break;
          case "Color":
            printParams.ColorDepth = ColorDepthType.Color;
            break;
        }

        // Options
        printParams.ViewLinksinBlue = settings.ViewLinksInBlue;
        printParams.HideUnreferencedViewTags = settings.HideUnreferencedViewTags;
        printParams.HideScopeBoxes = settings.HideScopeBoxes;
        printParams.HideCropBoundaries = settings.HideCropBoundaries;
        printParams.ReplaceHalftoneWithThinLines = settings.ReplaceHalftoneWithThinLines;
        printParams.MaskCoincidentLines = settings.RegionEdgesMaskCoincidentLines;
        printParams.HideReforWorkPlanes = settings.HideRefWorkPlanes;

        // PAPER SIZE AND ORIENTATION
        printParams.PageOrientation = sheet.Orientation == "Landscape" ? PageOrientationType.Landscape : PageOrientationType.Portrait;
        printParams.PaperSize = settings.Printers_PaperSize.First(x => x.Name == sheet.Size);
      }
      catch (Exception ex)
      {
        //System.Windows.MessageBox.Show($"CreatePrintSetupAction Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        sheet.Error = $"ERROR PrintParameters: {ex.Message}";
      }
    }
  }
}
