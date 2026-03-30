using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MultiTool.DisplaySettings.Models.Childs;

namespace MultiTool.DisplaySettings.Functions
{
  public class F_DisplaySettings_Sort : IComparer
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(object x, object y)
    {
      return StrCmpLogicalW((x as B_ViewView).ViewCurrent.Name, ((y as B_ViewView).ViewCurrent.Name));
    }
  }
}
