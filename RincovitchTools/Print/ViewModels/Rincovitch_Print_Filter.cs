using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit.Async;
using RincovitchTools.Print.API.Revit;
using RincovitchTools.Print.Models;
using RincovitchTools.Print.Models.ModelChilds;
using RincovitchTools.Print.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using View = Autodesk.Revit.DB.View;
using System.Windows.Controls.Primitives;
using System.Text.RegularExpressions;
using RincovitchTools.General.Revit;

namespace RincovitchTools.Print
{
  public class Rincovitch_Print_Filter : BaseViewModel
  {
    #region ICommand
    public ICommand CheckedUpdatePreviewCommand
    {
      get; set;
    }
    public ICommand SelectSheetViewCommand
    {
      get; set;
    }
    public ICommand SelectTitleBlockCommand
    {
      get; set;
    }
    public ICommand SelectRevisionCommand
    {
      get; set;
    }

    public ICommand RenameCommand
    {
      get; set;
    }
    public ICommand DuplicateCommand
    {
      get; set;
    }


    public ICommand ApplyCommand
    {
      get; set;
    }

    #endregion

    #region M_VM
    NMK_VM _NMK_VM;
    public NMK_VM NMK_VM
    {
      get => _NMK_VM; set
      {
        _NMK_VM = value;
        OnPropertyChanged();
      }
    }

    NMK_M _NMK_M;
    public NMK_M NMK_M
    {
      get => _NMK_M; set
      {
        _NMK_M = value;
        OnPropertyChanged();
      }
    }
    #endregion

    public Rincovitch_Print_Filter(NMK_VM _NMK_VM_, NMK_M _NMK_M_)
    {
      NMK_VM = _NMK_VM_;
      NMK_M = _NMK_M_;
      LoadedAsync();

      CheckedUpdatePreviewCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        CheckedUpdatePreview(p);
      });

      SelectSheetViewCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        SelectSheetView(p);
      });

      SelectTitleBlockCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        SelectTitleBlock(p);
      });

      SelectRevisionCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        SelectRevision(p);
      });

      ApplyCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        Apply(p);
      });

      RenameCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        Rename(p);
      });

      DuplicateCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        Duplicate(p);
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

        LoadFilter(doc);
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"LoadedAsync Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
      finally
      {

      }
    }

    // Load Filter
    void LoadFilter(Document doc)
    {
      try
      {
        //if (NMK_M.SheetViews == null || sheets.Count() == 0)
        //{
        //  System.Windows.MessageBox.Show($"LoadFilter Error: No Sheet View found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //  return;
        //}

        var list_sheet = new List<NMK_M_Filter>();
        var sheetNode = CheckSheetsInTree(NMK_M.SelectionSheets);
        var sheetNode_titleBlock = CheckSheetsInTree(NMK_M.SelectionSheets, true);
        if (sheetNode != null && sheetNode.ViewSheet != null)
        {
          list_sheet = sheetNode.ViewSheet.ParametersMap.Cast<Parameter>().OrderBy(x => x.Definition.Name).Select(x => new NMK_M_Filter()
          {
            IsSheet = true,
            Type = "Para",
            Id = F_Versions.get_id(x.Id),
            FontWeight = "Normal",
            Name = $"Sheet : {x.Definition.Name}",
            ParameterInfo = x,
          }).ToList();
        }

        if (sheetNode_titleBlock != null && sheetNode_titleBlock.TitleBlock != null)
        {
          foreach (var x in sheetNode_titleBlock.TitleBlock.ParametersMap.Cast<Parameter>().OrderBy(x => x.Definition.Name))
          {
            list_sheet.Add(new NMK_M_Filter()
            {
              IsSheet = true,
              Type = "TitleBlock",
              Id = F_Versions.get_id(x.Id),
              FontWeight = "Normal",
              Name = $"TitleBlock : {x.Definition.Name}",
              ParameterInfo = x,
            });
          }
        }

        NMK_M.ViewSheetFilters_Sheet = new ObservableCollection<NMK_M_Filter>(list_sheet);
        NMK_M.ViewSheetFilters_ListBox_Sheet = new ListCollectionView(NMK_M.ViewSheetFilters_Sheet);
        NMK_M.ViewSheetFilters_ListBox_Sheet.CustomSort = new F_SortViewSheetFilter();
        NMK_M.ViewSheetFilters_ListBox_Sheet.Filter = (obj) =>
        {
          if (!(obj is NMK_M_Filter))
            return false;
          if (string.IsNullOrWhiteSpace(NMK_M.Search_FilterSheet))
            return true;
          var item = obj as NMK_M_Filter;
          return item.Name.IndexOf(NMK_M.Search_FilterSheet, StringComparison.OrdinalIgnoreCase) >= 0;
        };
        NMK_M.ViewSheetFilters_DataGrid_Sheet = new ListCollectionView(NMK_M.ViewSheetFilters_Sheet);
        NMK_M.ViewSheetFilters_DataGrid_Sheet.CustomSort = new F_SortViewSheetFilter();


        var viewNode = CheckSheetsInTree(NMK_M.SelectionViews);
        var list_view = new List<NMK_M_Filter>();

        list_view = viewNode.ViewSheet.ParametersMap.Cast<Parameter>().OrderBy(x => x.Definition.Name).Select(x => new NMK_M_Filter()
        {
          Type = "Para",
          Id = F_Versions.get_id(x.Id),
          FontWeight = "Normal",
          Name = $"View : {x.Definition.Name}",
          ParameterInfo = x,
        }).ToList();

        NMK_M.ViewSheetFilters_View = new ObservableCollection<NMK_M_Filter>(list_view);
        NMK_M.ViewSheetFilters_ListBox_View = new ListCollectionView(NMK_M.ViewSheetFilters_View);
        NMK_M.ViewSheetFilters_ListBox_View.CustomSort = new F_SortViewSheetFilter();
        NMK_M.ViewSheetFilters_ListBox_View.Filter = (obj) =>
        {
          if (!(obj is NMK_M_Filter))
            return false;
          if (string.IsNullOrWhiteSpace(NMK_M.Search_FilterView))
            return true;
          var item = obj as NMK_M_Filter;
          return item.Name.IndexOf(NMK_M.Search_FilterView, StringComparison.OrdinalIgnoreCase) >= 0;
        };
        NMK_M.ViewSheetFilters_DataGrid_View = new ListCollectionView(NMK_M.ViewSheetFilters_View);
        NMK_M.ViewSheetFilters_DataGrid_View.CustomSort = new F_SortViewSheetFilter();

      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"LoadFilter Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    async Task SelectSheetView(object p)
    {
      try
      {

        List<NMK_M_SheetAndView> selects = GetSheetView();

        await RevitTask.RunAsync((uiapp) =>
        {
          var uidoc = uiapp.ActiveUIDocument;
          uidoc.Selection.SetElementIds(selects.Select(x => x.ViewSheet.Id).ToList());
        });
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"SelectSheetView Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    async Task SelectTitleBlock(object p)
    {
      try
      {

        List<NMK_M_SheetAndView> selects = GetSheetView();

        await RevitTask.RunAsync((uiapp) =>
        {
          var uidoc = uiapp.ActiveUIDocument;
          uidoc.Selection.SetElementIds(selects.Where(x => x.TitleBlock != null).Select(x => x.TitleBlock).Select(x => x.Id).ToList());
        });
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"SelectTitleBlock Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    async Task SelectRevision(object p)
    {
      try
      {
        await RevitTask.RunAsync((uiapp) =>
        {
          var uidoc = uiapp.ActiveUIDocument;
          if (!NMK_M.Filter_ControlBySelect)
          {
            List<NMK_M_SheetAndView> selects = GetSheetView();
            uidoc.Selection.SetElementIds(selects.Where(x => x.RevisionCloud.Count() > 0).SelectMany(x => x.RevisionCloud).Select(x => x.Id).ToList());
          }
          else
          {
            var sheets = SheetsInTree(NMK_M.SelectionSheets.ToList());
            uidoc.Selection.SetElementIds(sheets.Where(x => x.RevisionCloud.Count() > 0).SelectMany(x => x.RevisionCloud).Select(x => x.Id).ToList());
          }
        });
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"SelectRevision Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    async Task Apply(object p)
    {
      try
      {
        List<NMK_M_SheetAndView> selects = GetSheetView();

        var sheetNumbersInList = selects.Select(s => F_Versions.get_id(s.ViewSheet.Id)).ToList();
        F_ApplySetPrint.ApplySetPrint(NMK_M, sheetNumbersInList);
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"Apply Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }


    async Task Rename(object p)
    {
      try
      {
        if (!string.IsNullOrEmpty(NMK_M.ViewSheetFilters_Rename.From))
        {
          var Sheets = SheetsInTree(NMK_M.SelectionSheets.ToList());
          var Views = SheetsInTree(NMK_M.SelectionViews.ToList());

          var data = NMK_M.IsVisible_Window.Selection_IsSheet ? Sheets : Views;
          var data_final = NMK_M.ViewSheetFilters_Rename.BySelect ? data.Where(x => x.IsChecked == true).ToList() : data;

          if (data_final.Count() > 0)
          {
            List<List<string>> existing_ = new();
            existing_.Add(data.Select(x => x.NameDefault).ToList());
            existing_.Add(data_final.Select(x => x.NameDefault.Replace(NMK_M.ViewSheetFilters_Rename.From, NMK_M.ViewSheetFilters_Rename.To)).ToList());
            var existing = existing_
             .Aggregate((prev, next) => prev.Intersect(next).ToList())
             .ToList();
            if (existing.Count() > 0)
            {
              System.Windows.MessageBox.Show("View name is existing : \n" + string.Join("\n", existing));
            }
            else
            {
              await RevitTask.RunAsync(() =>
              {
                Document doc = Sheets.First().ViewSheet.Document;

                using (Transaction tr = new Transaction(doc, "Rename"))
                {
                  tr.Start();

                  foreach (var item in data_final)
                  {
                    var new_name = item.NameDefault.Replace(NMK_M.ViewSheetFilters_Rename.From, NMK_M.ViewSheetFilters_Rename.To);
                    item.ViewSheet.Name = new_name;
                    item.SheetName = item.SheetName != item.NameDefault ? item.SheetName : new_name;
                    item.NameDefault = new_name;
                    item.Name = item.IsSheet ? $"{(item.ViewSheet as ViewSheet).SheetNumber} - {new_name}" : new_name;

                  }

                  System.Windows.MessageBox.Show("Rename success!");

                  tr.Commit();
                }

              });
            }
          }
          else
          {
            System.Windows.MessageBox.Show("Please select at least one sheet/view to rename.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
          }
        }
        else
        {
          System.Windows.MessageBox.Show("Replace from can't empty!");
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"Rename Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    async Task Duplicate(object p)
    {
      try
      {
        var Sheets = SheetsInTree(NMK_M.SelectionSheets.ToList());
        var Views = SheetsInTree(NMK_M.SelectionViews.ToList());

        var data = NMK_M.IsVisible_Window.Selection_IsSheet ? Sheets : Views;
        var data_final = NMK_M.ViewSheetFilters_Rename.BySelect ? data.Where(x => x.IsChecked == true).ToList() : data;

        if (data_final.Count() > 0)
        {
          await RevitTask.RunAsync(() =>
          {
            Document doc = Sheets.First().ViewSheet.Document;

            using (Transaction tr = new Transaction(doc, "Duplicate"))
            {
              tr.Start();

              char? suffix_char = null;
              int? suffix_int = null;
              if (!string.IsNullOrEmpty(NMK_M.ViewSheetFilters_Duplicate.Suffix))
              {
                if (char.IsDigit(Convert.ToChar(NMK_M.ViewSheetFilters_Duplicate.Suffix)))
                {
                  suffix_int = int.Parse(NMK_M.ViewSheetFilters_Duplicate.Suffix.ToString());
                }
                else if (char.IsLetter(Convert.ToChar(NMK_M.ViewSheetFilters_Duplicate.Suffix)))
                {
                  suffix_char = Convert.ToChar(NMK_M.ViewSheetFilters_Duplicate.Suffix);
                }
                else
                {
                  suffix_int = 1;
                }
              }

              if (data_final.First().IsSheet)
              {
                var new_number = NMK_M.ViewSheetFilters_Duplicate.SheetNumberStart;
                for (int i = 0; i < NMK_M.ViewSheetFilters_Duplicate.Count; i++)
                {
                  string suffix = string.Empty;

                  if (!string.IsNullOrEmpty(NMK_M.ViewSheetFilters_Duplicate.Suffix))
                  {
                    if (suffix_char.HasValue)
                    {
                      suffix = suffix_char.Value.ToString();
                      suffix_char++;   // nếu bạn có logic tăng char riêng
                    }
                    else
                    {
                      suffix = suffix_int++.ToString();
                    }
                  }

                  foreach (var item in data_final.OrderBy(x => x, new F_SortViewSheetName()))
                  {
                    var new_name = $"{(NMK_M.ViewSheetFilters_Duplicate.ByCurrentName ? item.NameDefault : NMK_M.ViewSheetFilters_Duplicate.CustomName)}{NMK_M.ViewSheetFilters_Duplicate.Separator}{suffix}";
                    var sheet = (item.ViewSheet as ViewSheet);
                    ElementId duplicated_view = ElementId.InvalidElementId;
#if D2022 || D2023 || D2024 || D2025 || D2026
                    if (NMK_M.ViewSheetFilters_Duplicate.IncludeAll)
                    {
                      if (sheet.CanBeDuplicated(SheetDuplicateOption.DuplicateSheetWithViewsAndDetailing))
                        duplicated_view = (item.ViewSheet as ViewSheet).Duplicate(SheetDuplicateOption.DuplicateSheetWithViewsAndDetailing);
                      else if (sheet.CanBeDuplicated(SheetDuplicateOption.DuplicateSheetWithDetailing))
                        duplicated_view = (item.ViewSheet as ViewSheet).Duplicate(SheetDuplicateOption.DuplicateSheetWithDetailing);
                      else
                        duplicated_view = (item.ViewSheet as ViewSheet).Duplicate(SheetDuplicateOption.DuplicateEmptySheet);
                    }
                    else
                      duplicated_view = (item.ViewSheet as ViewSheet).Duplicate(SheetDuplicateOption.DuplicateEmptySheet);
#endif
                    var sheet_ = doc.GetElement(duplicated_view) as ViewSheet;
                    sheet_.Name = new_name;
                    sheet_.SheetNumber = new_number;
                    new_number = AutoIncrease(new_number);
                  }
                }
              }
              else
              {
                for (int i = 0; i < NMK_M.ViewSheetFilters_Duplicate.Count; i++)
                {
                  string suffix = string.Empty;
                  if (!string.IsNullOrEmpty(NMK_M.ViewSheetFilters_Duplicate.Suffix))
                  {
                    if (suffix_char.HasValue)
                    {
                      suffix = suffix_char.Value.ToString();
                      suffix_char++;   // nếu bạn có logic tăng char riêng
                    }
                    else
                    {
                      suffix = suffix_int++.ToString();
                    }
                  }
                  foreach (var item in data_final.OrderBy(x => x, new F_SortViewSheetName()))
                  {
                    var new_name = $"{(NMK_M.ViewSheetFilters_Duplicate.ByCurrentName ? item.NameDefault : NMK_M.ViewSheetFilters_Duplicate.CustomName)}{NMK_M.ViewSheetFilters_Duplicate.Separator}{suffix}";
                    ElementId duplicated_view = ElementId.InvalidElementId;
                    if (item.ViewSheet.CanViewBeDuplicated(ViewDuplicateOption.WithDetailing))
                      duplicated_view = item.ViewSheet.Duplicate(ViewDuplicateOption.WithDetailing);
                    else
                      duplicated_view = item.ViewSheet.Duplicate(ViewDuplicateOption.Duplicate);
                    doc.GetElement(duplicated_view).Name = new_name;
                  }
                }
              }

              NMK_VM.SheetViewVM = new Rincovitch_Print_SheetViewViewModel(NMK_VM, NMK_M);
              System.Windows.MessageBox.Show("Duplicate success!");

              tr.Commit();
            }

          });
        }
        else
        {
          System.Windows.MessageBox.Show("Please select at least one sheet/view to duplicate.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"Duplicate Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    string AutoIncrease(string input)
    {
      var match = Regex.Match(input, @"^(.*?)(\d+)$");
      if (!match.Success)
        return input; // không có số ở cuối → giữ nguyên

      string prefix = match.Groups[1].Value;
      string numberPart = match.Groups[2].Value;

      int number = int.Parse(numberPart) + 1;

      // Giữ nguyên số lượng chữ số (leading zero)
      return prefix + number.ToString(new string('0', numberPart.Length));
    }

    List<NMK_M_SheetAndView> GetSheetView()
    {
      List<NMK_M_SheetAndView> selects = new List<NMK_M_SheetAndView>();
      try
      {
        List<List<NMK_M_SheetAndView>> selects_sheet = new();
        var sheets_para = NMK_M.ViewSheetFilters_Sheet.Where(x => x.IsChecked).ToList();

        foreach (var item in sheets_para)
        {
          if (item.Rule == "Equals")
          {
            if (item.Value != null && !string.IsNullOrEmpty(item.Value.Value))
              selects_sheet.Add(item.Value.SheetViews);
          }
          else if (item.Rule == "Does not equal")
          {
            var filtered_sheets = item.Values
              .Where(v => v.Value != item.ValueText)
              .SelectMany(v => v.SheetViews)
              .ToList();
            selects_sheet.Add(filtered_sheets);
          }
          else if (item.Rule == "Contains")
          {
            var filtered_sheets = item.Values
              .Where(v => v.Value != null && v.Value.Contains(item.ValueText))
              .SelectMany(v => v.SheetViews)
              .ToList();
            selects_sheet.Add(filtered_sheets);
          }
          else if (item.Rule == "Does not contain")
          {
            var filtered_sheets = item.Values
              .Where(v => v.Value == null || !v.Value.Contains(item.ValueText))
              .SelectMany(v => v.SheetViews)
              .ToList();
            selects_sheet.Add(filtered_sheets);
          }
          else if (item.Rule == "Begins with")
          {
            var filtered_sheets = item.Values
              .Where(v => v.Value != null && v.Value.StartsWith(item.ValueText))
              .SelectMany(v => v.SheetViews)
              .ToList();
            selects_sheet.Add(filtered_sheets);
          }
          else if (item.Rule == "Does not begin with")
          {
            var filtered_sheets = item.Values
              .Where(v => v.Value == null || !v.Value.StartsWith(item.ValueText))
              .SelectMany(v => v.SheetViews)
              .ToList();
            selects_sheet.Add(filtered_sheets);
          }
          else if (item.Rule == "Ends with")
          {
            var filtered_sheets = item.Values
              .Where(v => v.Value != null && v.Value.EndsWith(item.ValueText))
              .SelectMany(v => v.SheetViews)
              .ToList();
            selects_sheet.Add(filtered_sheets);
          }
          else if (item.Rule == "Does not end with")
          {
            var filtered_sheets = item.Values
              .Where(v => v.Value == null || !v.Value.EndsWith(item.ValueText))
              .SelectMany(v => v.SheetViews)
              .ToList();
            selects_sheet.Add(filtered_sheets);
          }
        }

        if (selects_sheet.Count() > 0)
        {
          var result_sheet = selects_sheet
          .Aggregate((prev, next) => prev.Intersect(next).ToList())
          .ToList();
          selects.AddRange(result_sheet);
        }

        List<List<NMK_M_SheetAndView>> selects_view = new();
        var view_para = NMK_M.ViewSheetFilters_View.Where(x => x.IsChecked).ToList();

        foreach (var item in view_para)
        {
          if (item.Rule == "Equals")
          {
            if (item.Value != null && !string.IsNullOrEmpty(item.Value.Value))
              selects_view.Add(item.Value.SheetViews);
          }
          else if (item.Rule == "Does not equal")
          {
            var filtered_views = item.Values
              .Where(v => v.Value != item.ValueText)
              .SelectMany(v => v.SheetViews)
              .ToList();
            selects_view.Add(filtered_views);
          }
          else if (item.Rule == "Contains")
          {
            var filtered_views = item.Values
              .Where(v => v.Value != null && v.Value.Contains(item.ValueText))
              .SelectMany(v => v.SheetViews)
              .ToList();
            selects_view.Add(filtered_views);
          }
          else if (item.Rule == "Does not contain")
          {
            var filtered_views = item.Values
              .Where(v => v.Value == null || !v.Value.Contains(item.ValueText))
              .SelectMany(v => v.SheetViews)
              .ToList();
            selects_view.Add(filtered_views);
          }
          else if (item.Rule == "Begins with")
          {
            var filtered_views = item.Values
              .Where(v => v.Value != null && v.Value.StartsWith(item.ValueText))
              .SelectMany(v => v.SheetViews)
              .ToList();
            selects_view.Add(filtered_views);
          }
          else if (item.Rule == "Does not begin with")
          {
            var filtered_views = item.Values
              .Where(v => v.Value == null || !v.Value.StartsWith(item.ValueText))
              .SelectMany(v => v.SheetViews)
              .ToList();
            selects_view.Add(filtered_views);
          }
          else if (item.Rule == "Ends with")
          {
            var filtered_views = item.Values
              .Where(v => v.Value != null && v.Value.EndsWith(item.ValueText))
              .SelectMany(v => v.SheetViews)
              .ToList();
            selects_view.Add(filtered_views);
          }
          else if (item.Rule == "Does not end with")
          {
            var filtered_views = item.Values
              .Where(v => v.Value == null || !v.Value.EndsWith(item.ValueText))
              .SelectMany(v => v.SheetViews)
              .ToList();
            selects_view.Add(filtered_views);
          }
        }

        if (selects_view.Count() > 0)
        {
          var result_view = selects_view
          .Aggregate((prev, next) => prev.Intersect(next).ToList())
          .ToList();
          selects.AddRange(result_view);
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"GetSheetView Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }

      return selects;
    }



    NMK_M_SheetAndView CheckSheetsInTree(IEnumerable<NMK_M_SheetAndView> nodes, bool tilteBlock = false)
    {
      NMK_M_SheetAndView node_ = null;
      foreach (var node in nodes)
      {
        if (node.IsItem && node.ViewSheet != null)
        {
          if (tilteBlock)
          {
            if (node.TitleBlock != null)
            {
              node_ = node;
              break;
            }
          }
          else
          {
            node_ = node;
            break;
          }
        }
        if (node.Items != null && node.Items.Count > 0)
        {
          node_ = CheckSheetsInTree(node.Items);
        }
      }
      return node_;
    }

    List<NMK_M_SheetAndView> SheetsInTree(List<NMK_M_SheetAndView> nodes, bool tilteBlock = false)
    {
      var result = new List<NMK_M_SheetAndView>();
      if (nodes == null)
        return result;

      foreach (var node in nodes)
      {
        if (node == null)
          continue;

        // If node is an item (sheet/view) and matches titleBlock filter, add it
        if (node.IsItem && node.ViewSheet != null)
        {
          if (!tilteBlock || (tilteBlock && node.TitleBlock != null))
            result.Add(node);
        }

        // Recurse into children
        if (node.Items != null && node.Items.Count > 0)
        {
          result.AddRange(SheetsInTree(node.Items.ToList(), tilteBlock));
        }
      }

      return result;
    }

    void CheckedUpdatePreview(object p)
    {
      try
      {
        if (!(p is NMK_M_Filter))
          return;

        var para = p as NMK_M_Filter;
        var Sheets = SheetsInTree(NMK_M.SelectionSheets.ToList());
        var Views = SheetsInTree(NMK_M.SelectionViews.ToList());


        Document doc = Sheets.First().ViewSheet.Document;
        if (para.IsSheet)
        {
          if (para.Type == "TitleBlock")
          {
            var sheet_titleBlock = Sheets.Where(x => x.TitleBlock != null);
            List<NMK_M_FilterValue> values = new List<NMK_M_FilterValue>();
            foreach (var item in sheet_titleBlock)
            {
              var titleBlock = item.TitleBlock;
              var para_titleBlock = titleBlock.get_Parameter(para.ParameterInfo.Definition);
              values.Add(new NMK_M_FilterValue()
              {
                Value = F_APIRevit_ParameterUtils.GetParameterStringValue(para_titleBlock),
                SheetView = item,
              });
            }

            List<NMK_M_FilterValue> values_ = new List<NMK_M_FilterValue>();
            foreach (var item in values.GroupBy(x => x.Value))
            {
              values_.Add(new NMK_M_FilterValue()
              {
                Value = item.Key,
                SheetViews = item.Select(x => x.SheetView).ToList(),
              });
            }
            para.Values = new ObservableCollection<NMK_M_FilterValue>(values_);
          }
          else
          {
            List<NMK_M_FilterValue> values = new List<NMK_M_FilterValue>();
            foreach (var item in Sheets)
            {
              var para_sheet = item.ViewSheet.get_Parameter(para.ParameterInfo.Definition);
              values.Add(new NMK_M_FilterValue()
              {
                Value = F_APIRevit_ParameterUtils.GetParameterStringValue(para_sheet),
                SheetView = item,
              });
            }

            List<NMK_M_FilterValue> values_ = new List<NMK_M_FilterValue>();
            foreach (var item in values.GroupBy(x => x.Value))
            {
              values_.Add(new NMK_M_FilterValue()
              {
                Value = item.Key,
                SheetViews = item.Select(x => x.SheetView).ToList(),
              });
            }
            para.Values = new ObservableCollection<NMK_M_FilterValue>(values_);
          }
        }
        else
        {
          List<NMK_M_FilterValue> values = new List<NMK_M_FilterValue>();
          foreach (var item in Views)
          {
            var para_view = item.ViewSheet.get_Parameter(para.ParameterInfo.Definition);
            values.Add(new NMK_M_FilterValue()
            {
              Value = F_APIRevit_ParameterUtils.GetParameterStringValue(para_view),
              SheetView = item,
            });
          }

          List<NMK_M_FilterValue> values_ = new List<NMK_M_FilterValue>();
          foreach (var item in values.GroupBy(x => x.Value))
          {
            values_.Add(new NMK_M_FilterValue()
            {
              Value = item.Key,
              SheetViews = item.Select(x => x.SheetView).ToList(),
            });
          }
          para.Values = new ObservableCollection<NMK_M_FilterValue>(values_);
        }
        if (para.Values.Count() > 0)
          para.Value = para.Values.First();
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show($"UpdatePreview Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }
  }
}
