using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using View = Autodesk.Revit.DB.View;

namespace RincovitchTools.Print
{
  public class Rincovitch_Print_ParameterViewModel : BaseViewModel
  {
    #region ICommand
    public ICommand UpdatePreviewCommand { get; set; }
    public ICommand CheckedUpdatePreviewCommand { get; set; }
    public ICommand IndexUpdatePreviewCommand { get; set; }

    public ICommand ImportCommand { get; set; }
    public ICommand ExportCommand { get; set; }
    public ICommand SelectCommand { get; set; }
    public ICommand ClearCommand { get; set; }
    public ICommand SyncCommand { get; set; }


    public ICommand ApplyCommand { get; set; }

    #endregion

    #region M_VM
    NMK_VM _NMK_VM;
    public NMK_VM NMK_VM { get => _NMK_VM; set { _NMK_VM = value; OnPropertyChanged(); } }

    NMK_M _NMK_M;
    public NMK_M NMK_M { get => _NMK_M; set { _NMK_M = value; OnPropertyChanged(); } }
    #endregion

    public Rincovitch_Print_ParameterViewModel(NMK_VM _NMK_VM_, NMK_M _NMK_M_)
    {
      NMK_VM = _NMK_VM_;
      NMK_M = _NMK_M_;
      LoadedAsync();

      UpdatePreviewCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        UpdatePreview();
      });

      CheckedUpdatePreviewCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        CheckedUpdatePreview(p);
      });

      IndexUpdatePreviewCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        IndexUpdatePreview(p);
      });


      ApplyCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        Apply(p);
      });

      ImportCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        Import(p);
      });

      ExportCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        Export(p);
      });

      SelectCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        Select(p);
      });

      ClearCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        Clear(p);
      });

      SyncCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        Sync(p);
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

        LoadParameter(doc);
        LoadTempalteAsync(doc);
        foreach (var item in NMK_M.SheetViews)
        {
          item.SheetName = F_APIRevit_Parameter.UpdateName(NMK_M, item);
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

    // Load Parameter
    void LoadParameter(Document doc)
    {
      try
      {
        var sheets = NMK_M.SheetViews.Where(x => x.IsSheet).ToList();
        //if (NMK_M.SheetViews == null || sheets.Count() == 0)
        //{
        //  System.Windows.MessageBox.Show($"LoadParameter Error: No Sheet View found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //  return;
        //}


        var sheetNode = CheckSheetsInTree(NMK_M.SelectionSheets);
        var viewNode = CheckSheetsInTree(NMK_M.SelectionViews);
        var list = new List<NMK_M_Parameter>();

        if (sheetNode != null)
        {
          list = sheetNode.ViewSheet.ParametersMap.Cast<Parameter>().OrderBy(x => x.Definition.Name).Select(x => new NMK_M_Parameter()
          {
            Type = "Para",
            Id = F_Versions.get_id(x.Id),
            FontWeight = "Normal",
            Name = $"Sheet : {x.Definition.Name}",
            ParameterInfo = x,
          }).ToList();
        }

        foreach (var item in doc.ProjectInformation.ParametersMap.Cast<Parameter>().OrderBy(x => x.Definition.Name))
        {
          list.Add(new NMK_M_Parameter()
          {
            Type = "Project",
            Id = F_Versions.get_id(item.Id),
            FontWeight = "Bold",
            Name = $"Project : {item.Definition.Name}",
            ParameterInfo = item,
          });
        }

        if (viewNode != null)
        {
          foreach (var item in viewNode.ViewSheet.ParametersMap.Cast<Parameter>().OrderBy(x => x.Definition.Name))
          {
            list.Add(new NMK_M_Parameter()
            {
              Type = "Para",
              Id = F_Versions.get_id(item.Id),
              FontWeight = "Normal",
              Name = $"View : {item.Definition.Name}",
              ParameterInfo = item,
            });
          }
        }

        NMK_M.ViewSheetParameters_PDF = new ObservableCollection<NMK_M_Parameter>(list.Select(x => x.Clone()));
        NMK_M.ViewSheetParameters_ListBox_PDF = new ListCollectionView(NMK_M.ViewSheetParameters_PDF);
        NMK_M.ViewSheetParameters_ListBox_PDF.CustomSort = new F_SortViewSheetParameter();
        NMK_M.ViewSheetParameters_ListBox_PDF.Filter = (obj) =>
        {
          if (!(obj is NMK_M_Parameter)) return false;
          if (string.IsNullOrWhiteSpace(NMK_M.Search_ParameterPDF))
            return true;
          var item = obj as NMK_M_Parameter;
          return item.Name.IndexOf(NMK_M.Search_ParameterPDF, StringComparison.OrdinalIgnoreCase) >= 0;
        };
        NMK_M.ViewSheetParameters_DataGrid_PDF = new ListCollectionView(NMK_M.ViewSheetParameters_PDF);
        NMK_M.ViewSheetParameters_DataGrid_PDF.CustomSort = new F_SortViewSheetParameter();


        NMK_M.ViewSheetParameters_DWG = new ObservableCollection<NMK_M_Parameter>(list.Select(x => x.Clone()));
        NMK_M.ViewSheetParameters_ListBox_DWG = new ListCollectionView(NMK_M.ViewSheetParameters_DWG);
        NMK_M.ViewSheetParameters_ListBox_DWG.CustomSort = new F_SortViewSheetParameter();
        NMK_M.ViewSheetParameters_ListBox_DWG.Filter = (obj) =>
        {
          if (!(obj is NMK_M_Parameter)) return false;
          if (string.IsNullOrWhiteSpace(NMK_M.Search_ParameterDWG))
            return true;
          var item = obj as NMK_M_Parameter;
          return item.Name.IndexOf(NMK_M.Search_ParameterDWG, StringComparison.OrdinalIgnoreCase) >= 0;
        };
        NMK_M.ViewSheetParameters_DataGrid_DWG = new ListCollectionView(NMK_M.ViewSheetParameters_DWG);
        NMK_M.ViewSheetParameters_DataGrid_DWG.CustomSort = new F_SortViewSheetParameter();

        UpdatePreview();
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"LoadParameter Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    void LoadTempalteAsync(Document doc)
    {
      try
      {
        List<List<NMK_M_Parameter_Load>> customname_data = new();
        string customname_data_ = F_APIGeneral_Revit_Schema.CheckSchemaAndGetSchema(doc.ProjectInformation, MVVMSourceProject.schema_NMK, MVVMSourceProject.schema_customname);
        if (!string.IsNullOrEmpty(customname_data_))
          customname_data = JsonConvert.DeserializeObject<List<List<NMK_M_Parameter_Load>>>(customname_data_);

        void load(ObservableCollection<NMK_M_Parameter> source, List<NMK_M_Parameter> check)
        {
          foreach (var item in source)
          {
            var exist = check.Any(x => x.Id == item.Id);
            if (exist)
              item.Load(check.First(x => x.Id == item.Id));
          }
        }

        if (customname_data.Count() > 0)
        {
          var data_default = customname_data[0];

          if (data_default[0].IsCurrent)
            load(NMK_M.ViewSheetParameters_PDF, data_default[0].Datas);
          else
          {
            var data_PDF = customname_data[1];
            if (data_PDF.Count() > 0 && data_PDF.Any(x => x.IsCurrent))
              load(NMK_M.ViewSheetParameters_PDF, data_PDF.First(x => x.IsCurrent).Datas);
          }

          if (data_default[1].IsCurrent)
            load(NMK_M.ViewSheetParameters_DWG, data_default[1].Datas);
          else
          {
            var data_DWG = customname_data[2];
            if (data_DWG.Count() > 0 && data_DWG.Any(x => x.IsCurrent))
              load(NMK_M.ViewSheetParameters_DWG, data_DWG.First(x => x.IsCurrent).Datas);
          }

          NMK_M.ViewSheetParameters_ListBox_PDF.Refresh();
          NMK_M.ViewSheetParameters_DataGrid_PDF.Refresh();
          NMK_M.ViewSheetParameters_ListBox_DWG.Refresh();
          NMK_M.ViewSheetParameters_DataGrid_DWG.Refresh();
          UpdatePreview(true);

          NMK_M.ViewSheetParameters_PDF_Selects = new ObservableCollection<NMK_M_Parameter_Load>(customname_data[1]);
          NMK_M.ViewSheetParameters_PDF_Select = NMK_M.ViewSheetParameters_PDF_Selects.Any(x => x.IsCurrent) ? NMK_M.ViewSheetParameters_PDF_Selects.First(x => x.IsCurrent) : new NMK_M_Parameter_Load();
          NMK_M.ViewSheetParameters_DWG_Selects = new ObservableCollection<NMK_M_Parameter_Load>(customname_data[2]);
          NMK_M.ViewSheetParameters_DWG_Select = NMK_M.ViewSheetParameters_DWG_Selects.Any(x => x.IsCurrent) ? NMK_M.ViewSheetParameters_DWG_Selects.First(x => x.IsCurrent) : new NMK_M_Parameter_Load();
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"LoadTempalte Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    NMK_M_SheetAndView CheckSheetsInTree(IEnumerable<NMK_M_SheetAndView> nodes)
    {
      NMK_M_SheetAndView node_ = null;
      foreach (var node in nodes)
      {
        if (node.IsItem && node.ViewSheet != null)
        {
          node_ = node;
          break;
        }
        if (node.Items != null && node.Items.Count > 0)
        {
          node_ = CheckSheetsInTree(node.Items);
        }
      }
      return node_;
    }

    void UpdatePreview(bool updateAll = false)
    {
      try
      {
        var sheetNode = CheckSheetsInTree(NMK_M.SelectionSheets);
        var viewNode = CheckSheetsInTree(NMK_M.SelectionViews);
        if (updateAll)
        {
          F_APIRevit_Parameter.GetParameterPreview_PDF(NMK_M, sheetNode);
          F_APIRevit_Parameter.GetParameterPreview_DWG(NMK_M, sheetNode);
        }
        else
        {
          if (NMK_M.IsVisible_Window.Dialog_Parameter_PDF)
            F_APIRevit_Parameter.GetParameterPreview_PDF(NMK_M, sheetNode);
          if (NMK_M.IsVisible_Window.Dialog_Parameter_DWG)
            F_APIRevit_Parameter.GetParameterPreview_DWG(NMK_M, sheetNode);
        }

        //NMK_M.Preview_ParameterPDF = string.Join("-", NMK_M.ViewSheetParameters_PDF.Where(x => x.IsChecked).OrderBy(x => x.Index).Select(x => $"{x.Name}"));

        NMK_M.ForegroundCustomFileName = (NMK_M.ViewSheetParameters_PDF.Any(x => x.IsChecked) || NMK_M.ViewSheetParameters_DWG.Any(x => x.IsChecked)) ? Brushes.OrangeRed : Brushes.White;
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"UpdatePreview Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }
    void IndexUpdatePreview(object p)
    {
      try
      {
        if (!(p is NMK_M_Parameter))
          return;

        var para = p as NMK_M_Parameter;
        if (NMK_M.IsVisible_Window.Dialog_Parameter_PDF)
        {
          var sheet = NMK_M.ViewSheetParameters_PDF.First(x => x.Index == para.Index && x.Id != para.Id);
          sheet.Index = para.Index_Old;
          sheet.Index_Old = para.Index_Old;
          NMK_M.ViewSheetParameters_DataGrid_PDF.Refresh();
        }

        if (NMK_M.IsVisible_Window.Dialog_Parameter_DWG)
        {
          var sheet = NMK_M.ViewSheetParameters_DWG.First(x => x.Index == para.Index && x.Id != para.Id);
          sheet.Index = para.Index_Old;
          sheet.Index_Old = para.Index_Old;
          NMK_M.ViewSheetParameters_DataGrid_DWG.Refresh();
        }

        UpdatePreview();
        para.Index_Old = para.Index;
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"UpdatePreview Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    void CheckedUpdatePreview(object p)
    {
      try
      {
        if (!(p is NMK_M_Parameter))
          return;

        int index = 1;
        var viewsheet = NMK_M.IsVisible_Window.Dialog_Parameter_PDF ? NMK_M.ViewSheetParameters_PDF : NMK_M.ViewSheetParameters_DWG;
        var sheet = viewsheet.Where(x => x.IsChecked).OrderBy(x => x.Index);
        foreach (var item in sheet)
        {
          if (item.Index == 0)
          {
            item.Index = sheet.Count();
            item.Index_Old = sheet.Count();
          }
          else
          {
            item.Index = index;
            item.Index_Old = index;
            index++;
          }
        }

        var para = p as NMK_M_Parameter;
        if (para.IsChecked == false)
        {
          para.Index = 0;
          para.Index_Old = 0;
        }

        if (NMK_M.IsVisible_Window.Dialog_Parameter_PDF)
          NMK_M.ViewSheetParameters_DataGrid_PDF.Refresh();
        else
          NMK_M.ViewSheetParameters_DataGrid_DWG.Refresh();

        UpdatePreview();
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"UpdatePreview Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }


    void Apply(object p)
    {
      try
      {
        if (NMK_M.SheetViews.Count() == 0)
          return;

        foreach (var item in NMK_M.SheetViews)
        {
          item.SheetName = F_APIRevit_Parameter.UpdateName(NMK_M, item);
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"Apply Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    void Import(object p)
    {
      try
      {
        var dialog = new Microsoft.Win32.OpenFileDialog()
        {
          Title = "Import JSON File",
          Filter = "JSON files (*.json)|*.json",
          DefaultExt = "json",
          Multiselect = true,
        };

        if (dialog.ShowDialog() == true)
        {
          foreach (var item in dialog.FileNames)
          {
            var name = item;
            var datas = JsonConvert.DeserializeObject<List<NMK_M_Parameter>>(System.IO.File.ReadAllText(name));

            if (NMK_M.IsVisible_Window.Dialog_Parameter_PDF)
            {
              var check = NMK_M.ViewSheetParameters_PDF_Selects.Where(x => x.Name == System.IO.Path.GetFileNameWithoutExtension(name)).ToList();
              if (check.Any())
              {
                check.First().Datas = datas;
                NMK_M.ViewSheetParameters_PDF_Select = check.First();
              }
              else
              {
                NMK_M.ViewSheetParameters_PDF_Selects.Add(new NMK_M_Parameter_Load()
                {
                  Name = System.IO.Path.GetFileNameWithoutExtension(name),
                  Datas = datas
                });
                NMK_M.ViewSheetParameters_PDF_Select = NMK_M.ViewSheetParameters_PDF_Selects.Last();
              }
            }
            if (NMK_M.IsVisible_Window.Dialog_Parameter_DWG)
            {
              var check = NMK_M.ViewSheetParameters_DWG_Selects.Where(x => x.Name == System.IO.Path.GetFileNameWithoutExtension(name)).ToList();
              if (check.Any())
              {
                check.First().Datas = datas;
                NMK_M.ViewSheetParameters_DWG_Select = check.First();
              }
              else
              {
                NMK_M.ViewSheetParameters_DWG_Selects.Add(new NMK_M_Parameter_Load()
                {
                  Name = System.IO.Path.GetFileNameWithoutExtension(name),
                  Datas = datas
                });
                NMK_M.ViewSheetParameters_DWG_Select = NMK_M.ViewSheetParameters_DWG_Selects.Last();
              }
            }
          }
          Select(p);
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"Import Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    void Export(object p)
    {
      try
      {

        if (NMK_M.IsVisible_Window.Dialog_Parameter_PDF)
        {
          var dialog = new Microsoft.Win32.SaveFileDialog
          {
            FileName = NMK_M.ViewSheetParameters_PDF_Select != null && !string.IsNullOrEmpty(NMK_M.ViewSheetParameters_PDF_Select.Name) ? NMK_M.ViewSheetParameters_PDF_Select.Name : "",
            Title = "Export JSON File",
            Filter = "JSON files (*.json)|*.json",
            DefaultExt = "json",
          };

          if (dialog.ShowDialog() == true)
          {
            var name = dialog.FileName;
            string json = JsonConvert.SerializeObject(NMK_M.ViewSheetParameters_PDF.Select(x => new NMK_M_Parameter()
            {
              IsChecked = x.IsChecked,
              Index = x.Index,
              Index_Old = x.Index_Old,
              Prefix = x.Prefix,
              Suffix = x.Suffix,
              Separator = x.Separator,
              Id = x.Id,
              Name = x.Name,
            }));
            System.IO.File.WriteAllText(name, json);
          }
        }
        if (NMK_M.IsVisible_Window.Dialog_Parameter_DWG)
        {
          var dialog = new Microsoft.Win32.SaveFileDialog
          {
            FileName = NMK_M.ViewSheetParameters_DWG_Select != null && !string.IsNullOrEmpty(NMK_M.ViewSheetParameters_DWG_Select.Name) ? NMK_M.ViewSheetParameters_DWG_Select.Name : "",
            Title = "Export JSON File",
            Filter = "JSON files (*.json)|*.json",
            DefaultExt = "json",
          };

          if (dialog.ShowDialog() == true)
          {
            var name = dialog.FileName;
            string json = JsonConvert.SerializeObject(NMK_M.ViewSheetParameters_DWG.Select(x => new NMK_M_Parameter()
            {
              IsChecked = x.IsChecked,
              Index = x.Index,
              Index_Old = x.Index_Old,
              Prefix = x.Prefix,
              Suffix = x.Suffix,
              Separator = x.Separator,
              Id = x.Id,
              Name = x.Name,
            }));
            System.IO.File.WriteAllText(name, json);
          }
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"Export Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    void Select(object p)
    {
      try
      {

        if (NMK_M.IsVisible_Window.Dialog_Parameter_PDF)
        {
          if (NMK_M.ViewSheetParameters_PDF_Select == null || string.IsNullOrEmpty(NMK_M.ViewSheetParameters_PDF_Select.Name))
            return;

          foreach (var item in NMK_M.ViewSheetParameters_PDF_Selects)
            item.IsCurrent = false;
          var item_ = NMK_M.ViewSheetParameters_PDF_Select;
          item_.IsCurrent = true;
          var datas = item_.Datas;

          foreach (var item in NMK_M.ViewSheetParameters_PDF)
          {
            var exist = datas.Any(x => x.Id == item.Id);
            if (exist)
              item.Load(datas.First(x => x.Id == item.Id));
          }
          NMK_M.ViewSheetParameters_ListBox_PDF.Refresh();
          NMK_M.ViewSheetParameters_DataGrid_PDF.Refresh();
          UpdatePreview();
        }
        if (NMK_M.IsVisible_Window.Dialog_Parameter_DWG)
        {
          if (NMK_M.ViewSheetParameters_DWG_Select == null || string.IsNullOrEmpty(NMK_M.ViewSheetParameters_DWG_Select.Name))
            return;

          foreach (var item in NMK_M.ViewSheetParameters_DWG_Selects)
            item.IsCurrent = false;
          var item_ = NMK_M.ViewSheetParameters_DWG_Select;
          item_.IsCurrent = true;
          var datas = item_.Datas;
          foreach (var item in NMK_M.ViewSheetParameters_DWG)
          {
            var exist = datas.Any(x => x.Id == item.Id);
            if (exist)
              item.Load(datas.First(x => x.Id == item.Id));
          }
          NMK_M.ViewSheetParameters_ListBox_DWG.Refresh();
          NMK_M.ViewSheetParameters_DataGrid_DWG.Refresh();
          UpdatePreview();
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"Select Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    void Clear(object p)
    {
      try
      {
        if (NMK_M.IsVisible_Window.Dialog_Parameter_PDF)
        {
          if (NMK_M.ViewSheetParameters_PDF_Select == null || string.IsNullOrEmpty(NMK_M.ViewSheetParameters_PDF_Select.Name))
            return;

          NMK_M.ViewSheetParameters_PDF_Selects.Remove(NMK_M.ViewSheetParameters_PDF_Select);
        }
        if (NMK_M.IsVisible_Window.Dialog_Parameter_DWG)
        {
          if (NMK_M.ViewSheetParameters_DWG_Select == null || string.IsNullOrEmpty(NMK_M.ViewSheetParameters_DWG_Select.Name))
            return;

          NMK_M.ViewSheetParameters_DWG_Selects.Remove(NMK_M.ViewSheetParameters_DWG_Select);
        }

        Sync(p);
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"Clear Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    void Sync(object p)
    {
      try
      {
        if (NMK_M.IsVisible_Window.Dialog_Parameter_PDF)
        {
          NMK_M.ViewSheetParameters_PDF_Select = null;
          foreach (var item in NMK_M.ViewSheetParameters_PDF)
          {
            item.Clear();
          }
          NMK_M.ViewSheetParameters_ListBox_PDF.Refresh();
          NMK_M.ViewSheetParameters_DataGrid_PDF.Refresh();
          UpdatePreview();
        }
        if (NMK_M.IsVisible_Window.Dialog_Parameter_DWG)
        {
          NMK_M.ViewSheetParameters_DWG_Select = null;
          foreach (var item in NMK_M.ViewSheetParameters_DWG)
          {
            item.Clear();
          }
          NMK_M.ViewSheetParameters_ListBox_DWG.Refresh();
          NMK_M.ViewSheetParameters_DataGrid_DWG.Refresh();
          UpdatePreview();
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"Sync Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }
  }

}
