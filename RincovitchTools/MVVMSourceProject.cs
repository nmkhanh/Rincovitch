using System.Collections.ObjectModel;

namespace RincovitchTools
{
  public static class MVVMSourceProject
  {
    public static string schema_NMK = "426EE5A2-E3DF-4DFC-AEF3-451F76081CA6";
    public static string schema_customname = "workspaceProjectId";
  }

  public class NMK_M_Project : BaseViewModel
  {
    private int _no;
    public int No
    {
      get => _no;
      set { _no = value; OnPropertyChanged(); }
    }

    private string _projectCode;
    public string ProjectCode
    {
      get => _projectCode;
      set { _projectCode = value; OnPropertyChanged(); }
    }

    private string _projectName;
    public string ProjectName
    {
      get => _projectName;
      set { _projectName = value; OnPropertyChanged(); }
    }

    public NMK_M_Project() { }

    public NMK_M_Project(int no, string projectCode, string projectName)
    {
      No = no;
      ProjectCode = projectCode;
      ProjectName = projectName;
    }
  }

  public static class NMK_M_ProjectData
  {
    public static ObservableCollection<NMK_M_Project> GetProjects()
    {
      return new ObservableCollection<NMK_M_Project>
            {
                new NMK_M_Project(1, "MEL02", "Mel02"),
                new NMK_M_Project(2, "LEEDS", "Leeds St"),
                new NMK_M_Project(3, "RALEIGH", "Raleigh"),
                new NMK_M_Project(4, "CW2", "Collins Wharf 2"),
                new NMK_M_Project(5, "CW3", "Collins Wharf 3"),
                new NMK_M_Project(6, "MAC", "Macaulay"),
                new NMK_M_Project(7, "SKYVIEW", "Skyview"),
                new NMK_M_Project(8, "MASCOT", "Mascot Tower - Tower 1"),
                new NMK_M_Project(9, "FAIRFIELD", "Fairfield"),
                new NMK_M_Project(10, "373 CROWN", "373 Crown St"),
                new NMK_M_Project(11, "PALLADIAN", "ISG - 56 Queens Parade"),
                new NMK_M_Project(12, "FOOTSCRAY", "ISG - Assemble Footscray"),
                new NMK_M_Project(13, "EMBASSY", "Embassy"),
                new NMK_M_Project(14, "KAI", "Kai Apartments Huskisson"),
                new NMK_M_Project(15, "570LB", "570 LT BOURKE"),
                new NMK_M_Project(16, "EDEN", "Eden Rise"),
                new NMK_M_Project(17, "MORAY", "Moray St"),
                new NMK_M_Project(18, "LS04", "LYNEHAM STAGE 04"),
                new NMK_M_Project(19, "MEL01CD", "MEL01CD"),
                new NMK_M_Project(20, "ELM GROVE", "2 Elm Grove, Brunswick East"),
                new NMK_M_Project(21, "ENCLAVE", "Enclave"),
                new NMK_M_Project(22, "CROWN MEL", "Crown Melbourne"),
                new NMK_M_Project(23, "EBV", "East Brunswick Vic3057"),
                new NMK_M_Project(24, "DLD", "District Living, Docklands"),
            };
    }
  }
}
