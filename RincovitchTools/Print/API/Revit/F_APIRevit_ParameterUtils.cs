using Autodesk.Revit.DB;
using Microsoft.Expression.Media;
using RincovitchTools.General.Revit;
using System;
using System.Globalization;

namespace RincovitchTools.Print.API.Revit
{
  public static class F_APIRevit_ParameterUtils
  {
    /// <summary>
    /// Get parameter value as string for all storage types. Prefer AsValueString when available.
    /// If Document is provided, ElementId values will be resolved to element name when possible.
    /// </summary>
    public static string GetParameterStringValue(Parameter p, Document doc = null)
    {
      if (p == null)
        return "null";

      try
      {
        // Prefer Revit's formatted string
        string valueStr = p.AsValueString();
        if (!string.IsNullOrEmpty(valueStr))
          return valueStr;

        switch (p.StorageType)
        {
          case StorageType.String:
            return p.AsString() ?? "null";

          case StorageType.Integer:
            return p.AsInteger().ToString(CultureInfo.InvariantCulture);

          case StorageType.Double:
            double d = p.AsDouble();
            // Try to provide a friendly unit for length-based values
            try
            {
#if D2022 || D2023 || D2024 || D2025 || D2026
              ForgeTypeId u = p.GetUnitTypeId();
              if (u != null && !string.IsNullOrEmpty(u.TypeId))
              {
                // For length-like units, convert to millimeters for readability
                // Many unit types exist; we handle common numeric case by returning rounded double
                double mm = 0;
                try
                {
                  mm = UnitUtils.ConvertFromInternalUnits(d, UnitTypeId.Millimeters);
                  return mm.ToString("0.###", CultureInfo.InvariantCulture) + " mm";
                }
                catch
                {
                  // fallback to raw double
                }
              }
#else
              double mm = 0;
              try
              {
                mm = UnitUtils.ConvertFromInternalUnits(d, DisplayUnitType.DUT_MILLIMETERS);
                return mm.ToString("0.###", CultureInfo.InvariantCulture) + " mm";
              }
              catch
              {
                // fallback to raw double
              }
#endif
            }
            catch
            {
              // ignore unit lookup errors
            }
            return d.ToString(CultureInfo.InvariantCulture);

          case StorageType.ElementId:
            ElementId id = p.AsElementId();
            if (id != null && F_Versions.get_id_int(id) > 0)
            {
              if (doc != null)
              {
                try
                {
                  var el = doc.GetElement(id);
                  if (el != null)
                    return el.Name ?? F_Versions.get_id(id).ToString(CultureInfo.InvariantCulture);
                }
                catch
                {
                  // ignore
                }
              }
              return F_Versions.get_id(id).ToString(CultureInfo.InvariantCulture);
            }
            return "null";

          default:
            return "null";
        }
      }
      catch (Exception)
      {
        return "null";
      }
    }
  }
}
