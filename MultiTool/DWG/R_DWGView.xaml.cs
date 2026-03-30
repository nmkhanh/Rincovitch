using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MultiTool.CreateFloor;
using Revit.Async;
using Brush = System.Windows.Media.Brush;
using Button = System.Windows.Controls.Button;
using Color = Autodesk.Revit.DB.Color;
using View = Autodesk.Revit.DB.View;

namespace MultiTool
{
  public class ViewTemplate
  {
    public string Name { get; set; }
    public View view { get; set; }
  }

  public class CADLink
  {
    public string Name { get; set; }
    public bool IsSelected { get; set; }
    public CADLinkType cad { get; set; }
  }

  public class Key
  {
    public Brush Color { get; set; }
    public string Name { get; set; }
  }


  public partial class R_DWGView : Window
  {
    //public R_ViewModel ViewModel { get; set; }
    public R_DWGView(UIApplication uiapp)
    {
      InitializeComponent();
      //this.DataContext = ViewModel = new R_ViewModel(uiapp);
      load();
    }

    void load()
    {
      try
      {
        RevitTask.RunAsync(uiapp =>
        {
          Document doc = uiapp.ActiveUIDocument.Document;

          Template.ItemsSource = new List<ViewTemplate>
          (
          new FilteredElementCollector(doc)
          .OfClass(typeof(View))
          .Cast<View>()
          .Where(x => x.IsTemplate)
          .Select(x => new ViewTemplate
          {
            Name = x.Name,
            view = x,
          }));
          Template.SelectedIndex = 0;

          DWGList.ItemsSource = new List<CADLink>
          (
          new FilteredElementCollector(doc)
          .OfClass(typeof(CADLinkType))
          .Cast<CADLinkType>()
          .Select(x => new CADLink
          {
            Name = x.Name,
            cad = x,
            IsSelected = true
          })
          );

          var key = new List<Key>
          {
            new Key { Color = new SolidColorBrush(Colors.Red), Name = "Layer overridden" },
          };
          DataGrid.ItemsSource = key;
        });
      }
      catch (Exception)
      {

      }
    }

    private void PickColor_Click(object sender, RoutedEventArgs e)
    {
      var button = sender as Button;
      var dialog = new System.Windows.Forms.ColorDialog();
      if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        button.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(dialog.Color.A, dialog.Color.R, dialog.Color.G, dialog.Color.B));
      }
    }

    private void Run(object sender, RoutedEventArgs e)
    {
      try
      {
        RevitTask.RunAsync(uiapp =>
        {
          Document doc = uiapp.ActiveUIDocument.Document;

          var view_ = Template.SelectedValue as ViewTemplate;
          var view = view_.view;

          using (Transaction tx = new Transaction(doc, "Override DWG Layer"))
          {
            tx.Start();
            if (BySelected.IsChecked == false)
            {
              foreach (var item in DWGList.ItemsSource.Cast<CADLink>())
              {
                OverrideDWGLayer(view, item.cad);
              }
            }
            else
            {
              foreach (var item in DWGList.ItemsSource.Cast<CADLink>().Where(x => x.IsSelected))
              {
                OverrideDWGLayer(view, item.cad);
              }
            }
            tx.Commit();
          }

        });
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show(ex.Message);
      }
    }

    void OverrideDWGLayer(View view, CADLinkType symbol)
    {
      try
      {
        Document doc = view.Document;

        foreach (var item in DataGrid.ItemsSource.Cast<Key>())//GRID A-ANNO-NOTE
        {
          foreach (Category subCat in symbol.Category.SubCategories)
          {
            if (subCat.Name.Contains(item.Name))
            {
              OverrideGraphicSettings ogs = new OverrideGraphicSettings();
              var color = (item.Color as SolidColorBrush).Color;
              ogs.SetProjectionLineColor(new Color(color.R, color.G, color.B));

              view.SetCategoryOverrides(subCat.Id, ogs);
            }
          }
        }
      }
      catch (System.Exception ex)
      {
        System.Windows.MessageBox.Show(ex.Message);
      }
    }
  }
}
