using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace MultiTool
{
  public static class F_Versions
  {
    public static double mm(double mm)
    {
			try
			{
        return
#if D2020
          UnitUtils.ConvertToInternalUnits(mm, DisplayUnitType.DUT_MILLIMETERS);
#elif D2021 || D2022 || D2023
          UnitUtils.ConvertToInternalUnits(mm, UnitTypeId.Millimeters);
#elif D2024 || D2025 || D2026
          UnitUtils.ConvertToInternalUnits(mm, UnitTypeId.Millimeters);
#endif
      }
      catch (Exception)
			{
        return 0;
			}
    }
  }
}
