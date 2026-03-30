using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using MultiTool.CreateFloor.Models;
using MultiTool.CreateFloor.Models.Childs;
using Revit.Async;
using Reference = Autodesk.Revit.DB.Reference;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;
using View = Autodesk.Revit.DB.View;

namespace MultiTool
{
  public class R_ViewModel : BaseViewModel
  {
    #region ICommand
    public ICommand LoadedCommand { get; set; }

    public ICommand RunCommand { get; set; }
    #endregion

    B_VM _VM = new B_VM();
    public B_VM VM { get => _VM; set { _VM = value; OnPropertyChanged(); } }

    public R_ViewModel(UIApplication uiapp)
    {
      LoadedCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        LoadedAsync(uiapp);
      });

      RunCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        RunAsync();
      });

    }

    void LoadedAsync(UIApplication uiapp)
    {
      try
      {
        
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
          create(uidoc, doc);
        });
      }
      catch (Exception)
      {

      }
    }

    void create(UIDocument uidoc, Document doc)
    {
      try
      {
        
      }
      catch(Exception)
      {

      }
    }
  }
}
