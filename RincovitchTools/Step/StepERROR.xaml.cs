using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

namespace RincovitchTools.Step
{
  /// <summary>
  /// Interaction logic for StepERROR.xaml
  /// </summary>
  public partial class StepERROR : Window
  {
    public ViewModel VM { get; set; }
    public StepERROR(string error_file)
    {
      InitializeComponent();
      this.DataContext = VM = new ViewModel(error_file);  
    }
  }

  public class ViewModel : BaseViewModel
  {
    public ICommand SelectCommand { get;set; }

    private  ObservableCollection<NMK_M_ErrorList> _ErrorList = new ();
    public ObservableCollection<NMK_M_ErrorList> ErrorList
    {
      get { return _ErrorList; }
      set
      {
        _ErrorList = value;
        OnPropertyChanged(nameof(ErrorList));
      }
    }

    public ViewModel(string error_file)
    {
      Loade(error_file);

      SelectCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        Select(p);
      });
    }

    void Loade(string error_file)
    {
      try
      {
        if (!File.Exists(error_file))
          return;

        foreach (var item in System.IO.File.ReadAllLines(error_file))
        {
          ErrorList.Add(new NMK_M_ErrorList()
          {
            id = item
          });
        }
      }
      catch (Exception)
      {

      }
    }

    async Task Select(object p)
    {
      try
      {
        if (!(p is NMK_M_ErrorList))
          return;

        var item = (NMK_M_ErrorList)p;
        await Revit.Async.RevitTask.RunAsync((uiapp) =>
        {
          var uidoc = uiapp.ActiveUIDocument;
          var doc = uidoc.Document;
          var element = doc.GetElement(item.id);

          uidoc.Selection.SetElementIds(new[] { element.Id });

          // Zoom + center view vào element
          uidoc.ShowElements(element.Id);
        });
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show(ex.Message);
      }
    }
  }

  public class NMK_M_ErrorList : BaseViewModel
  {
    public string id { get; set; }
  }
}
