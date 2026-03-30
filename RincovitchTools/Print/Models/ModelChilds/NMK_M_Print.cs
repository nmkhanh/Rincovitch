using System;

namespace RincovitchTools.Print.Models.ModelChilds
{
  public class NMK_M_Print : BaseViewModel
  {
    #region File Section

    private bool _isCreateSeparateFiles = Properties.Settings.Default.CreateSeparate;
    public bool IsCreateSeparateFiles
    {
      get => _isCreateSeparateFiles;
      set { _isCreateSeparateFiles = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsCombineMultipleFiles)); }
    }

    public bool IsCombineMultipleFiles
    {
      get => !_isCreateSeparateFiles;
      set { IsCreateSeparateFiles = !value; }
    }

    private bool _useNamingConvention = Properties.Settings.Default.UseCombineName;
    public bool UseNamingConvention
    {
      get => _useNamingConvention;
      set
      {
        _useNamingConvention = value; OnPropertyChanged();
        if (_useNamingConvention)
          FileCombineNameConvention = $"{NamingDay}_{NamingProjectCode}-{NamingNode}";
      }
    }

    private string _NamingDay = DateTime.Now.ToString("yyMMdd");
    public string NamingDay
    {
      get => _NamingDay;
      set { _NamingDay = value; OnPropertyChanged(); FileCombineNameConvention = $"{NamingDay}_{NamingProjectCode}-{NamingNode}"; }
    }

    private string _NamingProjectCode = "";
    public string NamingProjectCode
    {
      get => _NamingProjectCode;
      set { _NamingProjectCode = value; OnPropertyChanged(); FileCombineNameConvention = $"{NamingDay}_{NamingProjectCode}-{NamingNode}"; }
    }

    private string _NamingNode = !string.IsNullOrEmpty(Properties.Settings.Default.CombineNameInfo) ? Properties.Settings.Default.CombineNameInfo : "GA PLANS";
    public string NamingNode
    {
      get => _NamingNode;
      set { _NamingNode = value; OnPropertyChanged(); FileCombineNameConvention = $"{NamingDay}_{NamingProjectCode}-{NamingNode}"; }
    }

    private string _fileCombineNameConvention;
    public string FileCombineNameConvention
    {
      get => _fileCombineNameConvention;
      set { _fileCombineNameConvention = value; OnPropertyChanged(); }
    }

    private string _fileCombineName = !string.IsNullOrEmpty(Properties.Settings.Default.CombineName) ? Properties.Settings.Default.CombineName : "Combined_File_Name";
    public string FileCombineName
    {
      get => _fileCombineName;
      set { _fileCombineName = value; OnPropertyChanged(); }
    }
    #endregion

    #region Print Section

    private bool _isSaveAllFilesInSameFolder = Properties.Settings.Default.SaveSameFolder;
    public bool IsSaveAllFilesInSameFolder
    {
      get => _isSaveAllFilesInSameFolder;
      set { _isSaveAllFilesInSameFolder = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsSaveSplitFilesByFormat)); }
    }

    public bool IsSaveSplitFilesByFormat
    {
      get => !_isSaveAllFilesInSameFolder;
      set { IsSaveAllFilesInSameFolder = !value; }
    }

    private string _folderSelection = !string.IsNullOrEmpty(Properties.Settings.Default.CurentPath) ? Properties.Settings.Default.CurentPath : Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    public string FolderSelection
    {
      get => _folderSelection;
      set { _folderSelection = value; OnPropertyChanged(); }
    }

    private double _progressValue;
    public double ProgressValue
    {
      get => _progressValue;
      set { _progressValue = value; OnPropertyChanged(); }
    }

    private double _progressMaximum = 100;
    public double ProgressMaximum
    {
      get => _progressMaximum;
      set { _progressMaximum = value; OnPropertyChanged(); }
    }


    private string _PrintStatus;
    public string PrintStatus
    {
      get => _PrintStatus;
      set { _PrintStatus = value; OnPropertyChanged(); }
    }

    private bool _IsPrintStatus;
    public bool IsPrintStatus
    {
      get => _IsPrintStatus;
      set { _IsPrintStatus = value; OnPropertyChanged(); }
    }

    private string _SelectStatus;
    public string SelectStatus
    {
      get => _SelectStatus;
      set { _SelectStatus = value; OnPropertyChanged(); }
    }

    public int SheetCount { get; set; }
    public int SheetCountCurrent { get; set; }
    public int ViewCount { get; set; }
    public int ViewCountCurrent { get; set; }

    public int ViewSheetCount { get; set; }
    public int ViewSheetCountCurrent { get; set; }
    #endregion

    public NMK_M_Print()
    {
      UpdateSelectStatus();
    }

    public void UpdatePrintStatus()
    {
      PrintStatus = $"{ViewSheetCountCurrent} / {ViewSheetCount} ({ProgressValue} %)";
    }

    public void UpdateSelectStatus()
    {
        SelectStatus = $"{SheetCountCurrent} / {SheetCount} Sheets and {ViewCountCurrent} / {ViewCount} Views selected. Total: {SheetCount + ViewCount}";
    }
  }
}
