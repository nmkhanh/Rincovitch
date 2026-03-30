using System;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit.Async;
using RincovitchTools.Print.ViewModels;
using RincovitchTools.Print.Models;
using RincovitchTools.Print.Models.ModelChilds;
using RincovitchTools.Print.API.Revit;
using System.Collections.ObjectModel;
using View = Autodesk.Revit.DB.View;
using System.Windows.Data;

namespace RincovitchTools.Print
{
  public class Rincovitch_Print_SettingsViewModel : BaseViewModel
  {
    #region ICommand
    public ICommand SheetBySchedulesDropDownClosedCommand { get; set; }

    #endregion

    #region M_VM
    NMK_VM _NMK_VM;
    public NMK_VM NMK_VM { get => _NMK_VM; set { _NMK_VM = value; OnPropertyChanged(); } }

    NMK_M _NMK_M;
    public NMK_M NMK_M { get => _NMK_M; set { _NMK_M = value; OnPropertyChanged(); } }
    #endregion

    public Rincovitch_Print_SettingsViewModel(NMK_VM _NMK_VM_, NMK_M _NMK_M_)
    {
      NMK_VM = _NMK_VM_;
      NMK_M = _NMK_M_;
      LoadedAsync();

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

        LoadDWGPrintSetups(doc);
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

    // Hàm lấy danh sách setup in DWG
    void LoadDWGPrintSetups(Document doc)
    {
      try
      {
        NMK_M.Settings_DWG.ExportSetups.Clear();
        var dwgSetups = ExportDWGSettings.ListNames(doc);
        foreach (var setup in dwgSetups)
        {
          NMK_M.Settings_DWG.ExportSetups.Add(setup);
        }
        if (NMK_M.Settings_DWG.ExportSetups.Count() > 0)
          NMK_M.Settings_DWG.SelectedExportSetup = NMK_M.Settings_DWG.ExportSetups.First();
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"LoadDWGPrintSetups Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }
  }

}
