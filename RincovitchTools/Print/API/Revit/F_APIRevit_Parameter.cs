using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using RincovitchTools.Print.Models;
using RincovitchTools.Print.Models.ModelChilds;

namespace RincovitchTools.Print.API.Revit
{
  public static class F_APIRevit_Parameter
  {
    public static void GetParameterPreview_PDF(NMK_M NMK_M, NMK_M_SheetAndView item)
    {
      try
      {
        if(NMK_M.ViewSheetParameters_DataGrid_PDF == null || NMK_M.ViewSheetParameters_DataGrid_PDF.Count == 0)
          return;

        List<string> result = new List<string>();
        foreach (var item_ in NMK_M.ViewSheetParameters_DataGrid_PDF.Cast<NMK_M_Parameter>().Where(x => x.IsChecked).OrderBy(x => x.Index).ToList())
        {
          if (item_.Type == "Project")
            result.Add($"{item_.Prefix}{item.ViewSheet.Document.ProjectInformation.get_Parameter(item_.ParameterInfo.Definition).AsValueString()}{item_.Suffix}{item_.Separator}");
          else
            result.Add($"{item_.Prefix}{item.ViewSheet.get_Parameter(item_.ParameterInfo.Definition).AsValueString()}{item_.Suffix}{item_.Separator}");
        }
        NMK_M.Preview_ParameterPDF = string.Join("", result);
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"GetParameterPreviewPDF Error: {ex.Message}\n\n{ex.StackTrace}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
      }
    }

    public static void GetParameterPreview_DWG(NMK_M NMK_M, NMK_M_SheetAndView item)
    {
      try
      {
        if (NMK_M.ViewSheetParameters_DataGrid_DWG == null || NMK_M.ViewSheetParameters_DataGrid_DWG.Count == 0)
          return;

        List<string> result = new List<string>();
        foreach (var item_ in NMK_M.ViewSheetParameters_DataGrid_DWG.Cast<NMK_M_Parameter>().Where(x => x.IsChecked).OrderBy(x => x.Index).ToList())
        {
          if (item_.Type == "Project")
            result.Add($"{item_.Prefix}{item.ViewSheet.Document.ProjectInformation.get_Parameter(item_.ParameterInfo.Definition).AsValueString()}{item_.Suffix}{item_.Separator}");
          else
            result.Add($"{item_.Prefix}{item.ViewSheet.get_Parameter(item_.ParameterInfo.Definition).AsValueString()}{item_.Suffix}{item_.Separator}");
        }
        NMK_M.Preview_ParameterDWG = string.Join("", result);
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"GetParameterPreviewDWG Error: {ex.Message}\n\n{ex.StackTrace}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
      }
    }

    public static string UpdateName(NMK_M NMK_M, NMK_M_SheetAndView item)
    {
      string name = item.NameDefault;
      try
      {
        if(item.IsSheet)
        {
          if (item.Format == "PDF")
          {
            if (NMK_M.ViewSheetParameters_PDF.Any(x => x.IsChecked))
            {
              var list = new List<string>();
              foreach (var para in NMK_M.ViewSheetParameters_DataGrid_PDF.Cast<NMK_M_Parameter>().Where(x => x.IsChecked).OrderBy(x => x.Index).ToList())
              {
                list.Add(para.Prefix);
                if (para.Type == "Project") list.Add(para.ParameterInfo.AsValueString());
                else list.Add(item.ViewSheet.get_Parameter(para.ParameterInfo.Definition).AsValueString());
                list.Add(para.Suffix);
                list.Add(para.Separator);
              }
              name = string.Join("", list);
            }
          }
          if (item.Format == "DWG")
          {
            if (NMK_M.ViewSheetParameters_DWG.Any(x => x.IsChecked))
            {
              var list = new List<string>();
              foreach (var para in NMK_M.ViewSheetParameters_DataGrid_DWG.Cast<NMK_M_Parameter>().Where(x => x.IsChecked).OrderBy(x => x.Index).ToList())
              {
                list.Add(para.Prefix);
                if (para.Type == "Project") list.Add(para.ParameterInfo.AsValueString());
                else list.Add(item.ViewSheet.get_Parameter(para.ParameterInfo.Definition).AsValueString());
                list.Add(para.Suffix);
                list.Add(para.Separator);
              }
              name = string.Join("", list);
            }
          }
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"UpdateNameError: {ex.Message}\n\n{ex.StackTrace}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
      }
      return name;
    }
  }
}
