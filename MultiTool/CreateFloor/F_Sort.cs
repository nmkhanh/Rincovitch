using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MultiTool.CreateFloor.Models.Childs;

namespace MultiTool.CreateFloor
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

  public class F_SortFloorType : IComparer<B_FloorType>
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(B_FloorType x, B_FloorType y)
    {
      return StrCmpLogicalW((x as B_FloorType).Name, ((y as B_FloorType).Name));
    }
  }

}
