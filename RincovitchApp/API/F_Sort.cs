using RincovitchApp.Models.ModelChilds;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RincovitchApp.API
{
  public class F_SortLeaveCollection : IComparer
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(object x, object y)
    {
      var taskX = x as NMK_M_Leave;
      var taskY = y as NMK_M_Leave;
      return StrCmpLogicalW($"{taskX.Approval}-{taskX.CreateBy}-{taskX.LeaveReason}-{taskX.CreateAt.ToString("yyyy/MM/dd HH:mm:ss")}",
                            $"{taskY.Approval}-{taskY.CreateBy}-{taskY.LeaveReason}-{taskY.CreateAt.ToString("yyyy/MM/dd HH:mm:ss")}");
    }
  }

  public class F_SortLeaveCollectionList : IComparer
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(object x, object y)
    {
      var taskX = x as NMK_M_LeaveDay;
      var taskY = y as NMK_M_LeaveDay;
      return StrCmpLogicalW($"{taskX.Start.ToString("yyyy/MM/dd HH:mm:ss")}",
                            $"{taskY.Start.ToString("yyyy/MM/dd HH:mm:ss")}");
    }
  }

  public class F_SortTasksProjectCollection : IComparer
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(object x, object y)
    {
      var taskX = x as NMK_M_Task;
      var taskY = y as NMK_M_Task;
      return StrCmpLogicalW($"{taskX.Project.Key}{taskX.IndexStatus}{taskX.DateEnd.ToString("yyyy/MM/dd HH:mm:ss")}", 
                            $"{taskY.Project.Key}{taskY.IndexStatus}{taskY.DateEnd.ToString("yyyy/MM/dd HH:mm:ss")}");
    }
  }

  public class F_SortTasksProjectCollectionSchedule : IComparer
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(object x, object y)
    {
      var taskX = x as NMK_M_Task;
      var taskY = y as NMK_M_Task;
      return StrCmpLogicalW($"{taskX.Index}-{taskX.Project.Name}{taskX.IndexStatus}{taskX.DateEnd.ToString("yyyy/MM/dd HH:mm:ss")}", 
                            $"{taskY.Index}-{taskY.Project.Name}{taskY.IndexStatus}{taskY.DateEnd.ToString("yyyy/MM/dd HH:mm:ss")}");
    }
  }

  public class F_SortTasksProjectCollectionTimeline : IComparer
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(object x, object y)
    {
      var taskX = x as NMK_M_Task;
      var taskY = y as NMK_M_Task;
      return StrCmpLogicalW($"{taskX.Project.Key}{taskX.IndexStatus}{taskX.DateEnd.ToString("yyyy/MM/dd HH:mm:ss")}", 
                            $"{taskY.Project.Key}{taskY.IndexStatus}{taskY.DateEnd.ToString("yyyy/MM/dd HH:mm:ss")}");
    }
  }

  public class F_SortTasksUserCollectionAdmin : IComparer
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(object x, object y)
    {
      var taskX = x as NMK_M_Task;
      var taskY = y as NMK_M_Task;
      return StrCmpLogicalW($"{taskX.User.Team}-{taskX.User.Name}", 
                            $"{taskY.User.Team}-{taskY.User.Name}");
    }
  }

  public class F_SortTasksUserCollection : IComparer
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(object x, object y)
    {
      var taskX = x as NMK_M_Task;
      var taskY = y as NMK_M_Task;
      return StrCmpLogicalW($"{taskX.User.Team}{taskX.User.Name}{taskX.DateEnd.ToString("yyyy/MM/dd HH:mm:ss")}", 
                            $"{taskY.User.Team}{taskY.User.Name}{taskY.DateEnd.ToString("yyyy/MM/dd HH:mm:ss")}");
    }
  }

  public class F_SortNotify : IComparer
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(object x, object y)
    {
      return StrCmpLogicalW((y as NMK_M_Notify).CreateAt.ToString("yyyy/MM/dd HH:mm:ss"), (x as NMK_M_Notify).CreateAt.ToString("yyyy/MM/dd HH:mm:ss"));
    }
  }

  public class F_SortByUser : IComparer
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(object x, object y)
    {
      return StrCmpLogicalW((x as NMK_M_Task).User.Team + (x as NMK_M_Task).User.Name, (y as NMK_M_Task).User.Team + (y as NMK_M_Task).User.Name);
    }
  }

  public class F_SortByDateStart : IComparer
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(object x, object y)
    {
      return StrCmpLogicalW((x as NMK_M_Task).CreateAt.ToString("yyyy/MM/dd HH:mm:ss"), (y as NMK_M_Task).CreateAt.ToString("yyyy/MM/dd  HH:mm:ss"));
    }
  }

  public class F_SortByDateEnd : IComparer
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(object x, object y)
    {
      return StrCmpLogicalW((x as NMK_M_Task).DateEnd.ToString("yyyy/MM/dd HH:mm:ss"), (y as NMK_M_Task).DateEnd.ToString("yyyy/MM/dd HH:mm:ss"));
    }
  }

  public class F_SortUser : IComparer
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(object x, object y)
    {
      return StrCmpLogicalW((x as NMK_M_User).Team + (x as NMK_M_User).Role + (x as NMK_M_User).Name, (y as NMK_M_User).Team + (y as NMK_M_User).Role + (y as NMK_M_User).Name);
    }
  }

  public class F_SortByUser_Object : IComparer<NMK_M_User>
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(NMK_M_User x, NMK_M_User y)
    {
      return StrCmpLogicalW(x.Name, y.Name);
    }
  }

  public class F_SortByProject : IComparer
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(object x, object y)
    {
      return StrCmpLogicalW((x as NMK_M_Task).Name, (y as NMK_M_Task).Name);
    }
  }

  public class F_SortByIsCheckedProject : IComparer
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(object x, object y)
    {
      return StrCmpLogicalW((x as NMK_M_Task).IsChecked + (x as NMK_M_Task).Name, (y as NMK_M_Task).IsChecked + (y as NMK_M_Task).Name);
    }
  }

  public class F_SortByProjectDashboard : IComparer
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(object x, object y)
    {
      return StrCmpLogicalW((x as NMK_M_Task).Project.Key, (y as NMK_M_Task).Project.Key);
    }
  }

  public class F_SortProject : IComparer
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(object x, object y)
    {
      return StrCmpLogicalW((x as NMK_M_Project).Key, (y as NMK_M_Project).Key);
    }
  }

  public class F_SortProjectItems : IComparer
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(object x, object y)
    {
      return StrCmpLogicalW((y as NMK_M_Project).Tasks.Count().ToString() + (y as NMK_M_Project).Key, (x as NMK_M_Project).Tasks.Count().ToString() + (x as NMK_M_Project).Key);
    }
  }

  public class F_SortByProject_Object : IComparer<NMK_M_Task>
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string x, string y);
    public int Compare(NMK_M_Task x, NMK_M_Task y)
    {
      return StrCmpLogicalW(x.Name, y.Name);
    }
  }
}
