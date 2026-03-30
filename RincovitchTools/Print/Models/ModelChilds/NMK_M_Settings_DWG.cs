using System.Collections.ObjectModel;

namespace RincovitchTools.Print.Models.ModelChilds
{
  public class NMK_M_Settings_DWG : BaseViewModel
  {
    #region Select Export Setup
    ObservableCollection<string> _exportSetups = new ObservableCollection<string>();
    public ObservableCollection<string> ExportSetups
    {
      get { return _exportSetups; }
      set
      {
        _exportSetups = value;
        OnPropertyChanged();
      }
    }

    string _selectedExportSetup = "";
    public string SelectedExportSetup
    {
      get { return _selectedExportSetup; }
      set
      {
        _selectedExportSetup = value;
        OnPropertyChanged();
      }
    }
    #endregion

    #region Options
    bool _exportViewsAsExternalReferences = false;
    public bool ExportViewsAsExternalReferences
    {
      get { return _exportViewsAsExternalReferences; }
      set
      {
        _exportViewsAsExternalReferences = value;
        OnPropertyChanged();
      }
    }
    #endregion
  }
}
