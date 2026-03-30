using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace RincovitchTools.General.Revit
{
  public static class F_Versions
  {
    public static string get_id(ElementId id)
    {
			try
			{
        return
#if D2020 || D2021 || D2022 || D2023
          id.IntegerValue.ToString();
#elif D2024 || D2025 || D2026
          id.Value.ToString();
#endif
      }
      catch (Exception)
			{
        return "";
			}
    }

    public static int get_id_int(ElementId id)
    {
      try
      {
        return
#if D2020 || D2021 || D2022 || D2023
          Convert.ToInt32(id.IntegerValue);
#elif D2024 || D2025 || D2026
          Convert.ToInt32(id.Value);
#endif
      }
      catch (Exception)
      {
        return -1;
      }
    }
  }
}
