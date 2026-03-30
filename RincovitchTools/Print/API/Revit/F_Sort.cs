using RincovitchTools.Print.Models.ModelChilds;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RincovitchTools.Print.API.Revit
{
  //public class F_SortViewSheet : IComparer
  //{
  //  [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
  //  private static extern int StrCmpLogicalW(string x, string y);
  //  public int Compare(object x, object y)
  //  {
  //    return StrCmpLogicalW((x as B_ViewSheetInfo).Sort,((y as B_ViewSheetInfo).Sort));
  //  }
  //}

  //public class F_SortViewSheetParameterCustomName : IComparer
  //{
  //  [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
  //  private static extern int StrCmpLogicalW(string x, string y);
  //  public int Compare(object x, object y)
  //  {
  //    return StrCmpLogicalW((x as B_ViewSheetParameterCustom).Name, ((y as B_ViewSheetParameterCustom).Name));
  //  }
  //}

  //public class F_SortViewSheetIndex : IComparer
  //{
  //  [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
  //  private static extern int StrCmpLogicalW(string x, string y);
  //  public int Compare(object x, object y)
  //  {
  //    return StrCmpLogicalW((x as B_ViewSheetInfo).ViewSheet_Format_Sort + (x as B_ViewSheetInfo).Index, (y as B_ViewSheetInfo).ViewSheet_Format_Sort + (y as B_ViewSheetInfo).Index);
  //  }
  //}

  //public class F_SortViewSheet_ : IComparer<B_ViewSheetInfo>
  //{
  //  [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
  //  private static extern int StrCmpLogicalW(string x, string y);
  //  public int Compare(B_ViewSheetInfo x, B_ViewSheetInfo y)
  //  {
  //    return StrCmpLogicalW((x as B_ViewSheetInfo).Sort, ((y as B_ViewSheetInfo).Sort));
  //  }
  //}

  public class F_SortViewSheetParameter : IComparer
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(object x, object y)
    {
      return StrCmpLogicalW(((x as NMK_M_Parameter).Index + (x as NMK_M_Parameter).Type + (x as NMK_M_Parameter).Name),((y as NMK_M_Parameter).Index + (y as NMK_M_Parameter).Type + (y as NMK_M_Parameter).Name));
    }
  }

  public class F_SortViewSheetFilter : IComparer
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(object x, object y)
    {
      return StrCmpLogicalW(((x as NMK_M_Filter).Index + (x as NMK_M_Filter).Type + (x as NMK_M_Filter).Name), ((y as NMK_M_Filter).Index + (y as NMK_M_Filter).Type + (y as NMK_M_Filter).Name));
    }
  }

  public class F_SortViewSheetName : Comparer<NMK_M_SheetAndView>
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public override int Compare(NMK_M_SheetAndView x, NMK_M_SheetAndView y)
    {
      return StrCmpLogicalW((x as NMK_M_SheetAndView).Name , (y as NMK_M_SheetAndView).Name);
    }
  }

  public class F_SortViewSheetNumberName : Comparer<NMK_M_SheetAndView>
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public override int Compare(NMK_M_SheetAndView x, NMK_M_SheetAndView y)
    {
      return StrCmpLogicalW((x as NMK_M_SheetAndView).SheetNumber + (x as NMK_M_SheetAndView).Name, (y as NMK_M_SheetAndView).SheetNumber + (y as NMK_M_SheetAndView).Name);
    }
  }
}
