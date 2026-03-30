using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MultiTool.GroupFilter.Models.Childs;

namespace MultiTool.GroupFilter
{
  public class F_SortGroupType_ : IComparer
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(object x, object y)
    {
      return StrCmpLogicalW((x as B_GroupType).Name, ((y as B_GroupType).Name));
    }
  }

  public class F_SortGroupType : IComparer<B_GroupType>
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(B_GroupType x, B_GroupType y)
    {
      return StrCmpLogicalW((x as B_GroupType).Name, ((y as B_GroupType).Name));
    }
  }

}
