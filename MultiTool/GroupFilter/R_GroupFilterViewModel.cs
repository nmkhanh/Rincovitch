using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using MultiTool.GroupFilter.Models;
using MultiTool.GroupFilter.Models.Childs;
using Revit.Async;
using Color = Autodesk.Revit.DB.Color;
using Group = Autodesk.Revit.DB.Group;
using Reference = Autodesk.Revit.DB.Reference;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;
using View = Autodesk.Revit.DB.View;

namespace MultiTool.GroupFilter
{
  public class R_GroupFilterViewModel : BaseViewModel
  {
    #region ICommand
    public ICommand LoadedCommand { get; set; }

    public ICommand RefreshCommand { get; set; }
    public ICommand SelectCommand { get; set; }
    public ICommand HighlightCommand { get; set; }
    public ICommand ColorCommand { get; set; }
    #endregion

    B_VM _VM = new B_VM();
    public B_VM VM { get => _VM; set { _VM = value; OnPropertyChanged(); } }

    public R_GroupFilterViewModel(UIApplication uiapp)
    {
      LoadedCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        LoadedAsync(uiapp);
      });

      HighlightCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        RunAsync();
      });

      SelectCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        SelectAsync();
      });

      RefreshCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        LoadedAsync(uiapp);
      });

      ColorCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        ColordAsync();
      });

    }

    void LoadedAsync(UIApplication uiapp)
    {
      try
      {
        UIDocument uidoc = uiapp.ActiveUIDocument;
        Document doc = uidoc.Document;

        var groups = new FilteredElementCollector(doc).OfClass(typeof(Group)).Cast<Group>().ToList();
        var grouptypes = new FilteredElementCollector(doc)
          .OfClass(typeof(GroupType)).Cast<GroupType>()
          .Where(x => x.FamilyName == "Model Group")
          .Where(x => x.Name.Contains("SLAB")).ToList();
        VM.Type_s = new ObservableCollection<B_GroupType>(grouptypes.Select(x => new B_GroupType()
        {
          Count = groups.Where(y => y.GroupType.Name == x.Name).Count(),
          Name = x.Name,
          Type = x,
          BG = System.Windows.Media.Brushes.Transparent,
        }));
        VM.Type_Sort = new ListCollectionView(VM.Type_s);
        VM.Type_Sort.CustomSort = new F_SortGroupType_();
      }
      catch (Exception)
      {

      }
    }
    async Task RunAsync()
    {
      try
      {
        await RevitTask.RunAsync((uiapp) =>
        {
          UIDocument uidoc = uiapp.ActiveUIDocument;
          Document doc = uidoc.Document;
          
          Selection selection = uidoc.Selection;
          ICollection<ElementId> ids = selection.GetElementIds();
          var group = doc.GetElement(ids.First()) as Group;
          foreach (var item in VM.Type_s)
          {
            //if (item.Name == group.GroupType.Name) item.BG = System.Windows.Media.Brushes.LightGreen;
            //else item.BG = System.Windows.Media.Brushes.Transparent;
            if (item.Name == group.GroupType.Name)
            {
              VM.Type = item;
              break;
            }
          }
        });
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show(ex.Message);
      }
    }

    async Task SelectAsync()
    {
      try
      {
        await RevitTask.RunAsync((uiapp) =>
        {
          UIDocument uidoc = uiapp.ActiveUIDocument;
          Document doc = uidoc.Document;

          Selection selection = uidoc.Selection;
          var groups = new FilteredElementCollector(doc).OfClass(typeof(Group)).Cast<Group>().ToList().Where(x => x.GroupType.Name == VM.Type.Name).Select(x => x.Id);
          selection.SetElementIds(groups.ToList());
        });
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show(ex.Message);
      }
    }
    
    async Task ColordAsync()
    {
      try
      {
        await RevitTask.RunAsync((uiapp) =>
        {
          UIDocument uidoc = uiapp.ActiveUIDocument;
          Document doc = uidoc.Document;

          View activeView = doc.ActiveView;

          using (Transaction trans = new Transaction(doc, "Override Group in View"))
          {
            trans.Start();

            FilteredElementCollector collector = new FilteredElementCollector(doc, activeView.Id);
            foreach (Element elem in collector)
            {
              activeView.SetElementOverrides(elem.Id, new OverrideGraphicSettings());
            }

            FillPatternElement solidFill = new FilteredElementCollector(doc)
                .OfClass(typeof(FillPatternElement))
                .Cast<FillPatternElement>()
                .FirstOrDefault(x => x.GetFillPattern().IsSolidFill);

            // Lấy group (ví dụ lấy bằng PickObject hoặc filter)
            Random rnd = new Random();
            foreach (var item in VM.Type_s)
            {
              var groups = new FilteredElementCollector(doc)
              .OfClass(typeof(Group))
              .Cast<Group>().ToList()
              .Where(x => x.GroupType.Name == item.Name);

              byte r = (byte)rnd.Next(0, 256);
              byte g = (byte)rnd.Next(0, 256);
              byte b = (byte)rnd.Next(0, 256);

              Color randomColor = new Color(r, g, b);
              foreach (var item_ in groups)
              {
                foreach (ElementId id in item_.GetMemberIds())
                {

                  OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                  ogs.SetSurfaceForegroundPatternColor(randomColor);
                  ogs.SetProjectionLineColor(randomColor);
                  ogs.SetSurfaceForegroundPatternId(solidFill.Id);

                  activeView.SetElementOverrides(id, ogs);
                }
              }
            }

            trans.Commit();
          }
        });
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show(ex.Message);
      }
    }

  }
}
