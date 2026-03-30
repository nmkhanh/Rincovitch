using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTool
{
  public class Filter_Floor : ISelectionFilter
  {
    public bool AllowElement(Element elem)
    {
      if (elem.Category != null && elem.Category.Name == "Floors")
        return true;
      else
        return false;
    }
    public bool AllowReference(Reference reference, XYZ position)
    {
      return false;
    }
  }

  public class Filter_Wall : ISelectionFilter
  {
    public bool AllowElement(Element elem)
    {
      if (elem.Category != null && elem.Category.Name == "Walls")
        return true;
      else
        return false;
    }
    public bool AllowReference(Reference reference, XYZ position)
    {
      return false;
    }
  }

  public class Filter_Wall_Column : ISelectionFilter
  {
    public bool AllowElement(Element elem)
    {
      if (elem.Category != null && (elem.Category.Name == "Walls" || elem.Category.Name == "Structural Columns"))
        return true;
      else
        return false;
    }
    public bool AllowReference(Reference reference, XYZ position)
    {
      return false;
    }
  }

  public class Filter_Tags : ISelectionFilter
  {
    public bool AllowElement(Element elem)
    {
      if (elem.Category != null && elem.Category.Name.Contains("Tags"))
        return true;
      else
        return false;
    }
    public bool AllowReference(Reference reference, XYZ position)
    {
      return false;
    }
  }
}
