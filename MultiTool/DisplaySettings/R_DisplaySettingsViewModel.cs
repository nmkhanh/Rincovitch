using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using MultiTool.DisplaySettings.Functions;
using MultiTool.DisplaySettings.Models;
using MultiTool.DisplaySettings.Models.Childs;
using Revit.Async;
using View = Autodesk.Revit.DB.View;

namespace MultiTool.DisplaySettings
{
  public class R_DisplaySettingsViewModel : BaseViewModel
  {
    #region ICommand
    public ICommand LoadedCommand { get; set; }

    public ICommand ViewActiveCommand { get; set; }

    public ICommand ChangeLinkCommand { get; set; }

    public ICommand RunCommand { get; set; }

    public ICommand RunAllCommand { get; set; }
    #endregion

    B_VM _VM = new B_VM();
    public B_VM VM { get => _VM; set { _VM = value; OnPropertyChanged(); } }

    public R_DisplaySettingsViewModel(UIApplication uiapp)
    {
      LoadedCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        LoadedAsync(uiapp);
      });

      ViewActiveCommand = new RelayCommand<B_ViewView>((p) => { return true; }, (p) =>
      {
        ViewActive(p);
      });

      ChangeLinkCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        ChangeLink(uiapp);
      });

      RunCommand = new RelayCommand<B_ViewView>((p) => { return true; }, (p) =>
      {
        RunAsync(p);
      });

      RunAllCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        RunAllAsync();
      });
    }

    void Loaded()
    {
      try
      {

      }
      catch (Exception)
      {

      }
    }
    
    async Task ViewActive(B_ViewView item)
    {
      try
      {
        await RevitTask.RunAsync((uiapp) =>
        {
          UIDocument uidoc = uiapp.ActiveUIDocument;
          Document doc = uidoc.Document;
          uidoc.ActiveView = item.ViewCurrent.View;
        });
      }
      catch (Exception)
      {

      }
    }

    void LoadedAsync(UIApplication uiapp)
    {
      try
      {
        UIDocument uidoc = uiapp.ActiveUIDocument;
        Document doc = uidoc.Document;

        var revit_links = new FilteredElementCollector(doc)
           .OfClass(typeof(RevitLinkInstance))
           .Cast<RevitLinkInstance>()
           .ToList();

        if (revit_links.Count() > 0)
        {
          VM.Project_Link_s = new ObservableCollection<B_RevitLink>(revit_links.Select(x => new B_RevitLink()
          {
            Doc = x.Document,
            Name = GetRevitDocumentNameAndPath(x),
            Instance = x,
            Views = new ObservableCollection<B_View>(LoadedAsync_get_viewlink(x.Document))
          }));
          if (VM.Project_Link_s.Count() > 0)
            VM.Project_Link = VM.Project_Link_s.First();
        }

        VM.Data_s = new ObservableCollection<B_ViewView>(LoadedAsync_get_viewlink(doc).Select(x => new B_ViewView
        {
          ViewCurrent = x,
          ViewLink_s = !string.IsNullOrEmpty(VM.Project_Link.Name) ? VM.Project_Link.Views : new ObservableCollection<B_View>(),
          ViewLink = !string.IsNullOrEmpty(VM.Project_Link.Name) && VM.Project_Link.Views.Count() > 0
          ? (VM.Project_Link.Views.Where(a => a.Name == x.Name).Count() > 0 ?
            VM.Project_Link.Views.First(a => a.Name == x.Name) : null)
          : null
        }));

        VM.Data_Count = VM.Data_s.Count();

        VM.Data_Sort_s = CollectionViewSource.GetDefaultView(VM.Data_s) as ListCollectionView;
        VM.Data_Sort_s.CustomSort = new F_DisplaySettings_Sort();
        VM.Data_Sort_s.Filter = Filter;
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show(ex.Message);
      }
    }

    private bool Filter(object item)
    {
      if (item is B_ViewView data)
      {
        return string.IsNullOrEmpty(VM.Search)
            || data.ViewCurrent.Name.IndexOf(VM.Search, StringComparison.OrdinalIgnoreCase) >= 0;
      }
      return false;
    }

    public string GetRevitDocumentNameAndPath(RevitLinkInstance instance)
    {
      return instance.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsValueString();
    }

    List<B_View> LoadedAsync_get_viewlink(Document doc)
    {
      return new FilteredElementCollector(doc)
       .OfClass(typeof(View))
       .Cast<View>()
       .Where(x => !x.IsTemplate && x.ViewType != ViewType.Schedule && x.ViewType != ViewType.Legend && x.ViewType != ViewType.DrawingSheet)
       .Select(x => new B_View()
       {
         Name = x.Title,
         View = x
       })
       .ToList();
    }


    void ChangeLink(UIApplication uiapp)
    {
      try
      {
        UIDocument uidoc = uiapp.ActiveUIDocument;
        Document doc = uidoc.Document;

        VM.Data_s = new ObservableCollection<B_ViewView>(LoadedAsync_get_viewlink(doc).Select(x => new B_ViewView
        {
          ViewCurrent = x,
          ViewLink_s = !string.IsNullOrEmpty(VM.Project_Link.Name) ? VM.Project_Link.Views : new ObservableCollection<B_View>(),
          ViewLink = !string.IsNullOrEmpty(VM.Project_Link.Name) && VM.Project_Link.Views.Count() > 0
          ? (VM.Project_Link.Views.Where(a => a.Name == x.Name).Count() > 0 ?
            VM.Project_Link.Views.First(a => a.Name == x.Name) : null)
          : null
        }));

        VM.Data_Count = VM.Data_s.Count();

        VM.Data_Sort_s = CollectionViewSource.GetDefaultView(VM.Data_s) as ListCollectionView;
        VM.Data_Sort_s.CustomSort = new F_DisplaySettings_Sort();
        VM.Data_Sort_s.Filter = Filter;
      }
      catch (Exception)
      {

      }
    }

    async Task RunAsync(B_ViewView item)
    {
      try
      {
        await RevitTask.RunAsync((uiapp) =>
        {
          using (TransactionGroup tr_group = new TransactionGroup(uiapp.ActiveUIDocument.Document))
          {
            tr_group.Start("Set");
            if (item.ViewLink.View != null && !string.IsNullOrEmpty(item.ViewLink.Name))
              UpdateOverridesInView(item.ViewCurrent.View, VM.Project_Link.Instance.Id, item.ViewLink.View.Id);
            tr_group.Assimilate();
          }
        });
      }
      catch (Exception)
      {

      }
    }

    async Task RunAllAsync()
    {
      try
      {
        await RevitTask.RunAsync((uiapp) =>
        {
          using (TransactionGroup tr_group = new TransactionGroup(uiapp.ActiveUIDocument.Document))
          {
            tr_group.Start("Set");
            foreach (var item in VM.Data_s)
            {
              if (item.ViewLink.View != null && !string.IsNullOrEmpty(item.ViewLink.Name))
                UpdateOverridesInView(item.ViewCurrent.View, VM.Project_Link.Instance.Id, item.ViewLink.View.Id);
            }
            tr_group.Assimilate();
          }
        });
      }
      catch (Exception)
      {

      }
    }

    static void UpdateOverridesInView(View view, ElementId linkElementId, ElementId linkedViewId)
    {
      try
      {
        //RevitLinkGraphicsSettings settings = new RevitLinkGraphicsSettings();
        //settings.LinkVisibilityType = LinkVisibility.ByLinkView;
        //settings.LinkedViewId = linkedViewId;

        //using (Transaction transaction = new Transaction(view.Document, "Set link graphical overrides"))
        //{
        //  transaction.Start();
        //  view.SetLinkOverrides(linkElementId, settings);
        //  transaction.Commit();
        //}
      }
      catch (Exception)
      {

      }
    }
  }
}
