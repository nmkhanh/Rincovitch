using NMKApp.Models;
using System.Collections;
using System.Runtime.InteropServices;

namespace NMKApp.Helpers;

/// <summary>
/// Collection comparers using Windows natural sort (StrCmpLogicalW).
/// </summary>
public abstract class NaturalSortComparer : IComparer
{
  [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
  protected static extern int StrCmpLogicalW(string x, string y);
  public abstract int Compare(object? x, object? y);
}

public class SortLeaveCollection : NaturalSortComparer
{
  public override int Compare(object? x, object? y) =>
    StrCmpLogicalW(
      $"{(x as LeaveModel)?.Approval}-{(x as LeaveModel)?.CreateAt:yyyy/MM/dd HH:mm:ss}",
      $"{(y as LeaveModel)?.Approval}-{(y as LeaveModel)?.CreateAt:yyyy/MM/dd HH:mm:ss}");
}

public class SortTasksByProject : NaturalSortComparer
{
  public override int Compare(object? x, object? y) =>
    StrCmpLogicalW(
      $"{(x as TaskModel)?.Project?.Key}{(x as TaskModel)?.IndexStatus}{(x as TaskModel)?.DateEnd:yyyy/MM/dd HH:mm:ss}",
      $"{(y as TaskModel)?.Project?.Key}{(y as TaskModel)?.IndexStatus}{(y as TaskModel)?.DateEnd:yyyy/MM/dd HH:mm:ss}");
}

public class SortTasksByProjectSchedule : NaturalSortComparer
{
  public override int Compare(object? x, object? y) =>
    StrCmpLogicalW(
      $"{(x as TaskModel)?.Index}-{(x as TaskModel)?.Project?.Name}{(x as TaskModel)?.IndexStatus}{(x as TaskModel)?.DateEnd:yyyy/MM/dd HH:mm:ss}",
      $"{(y as TaskModel)?.Index}-{(y as TaskModel)?.Project?.Name}{(y as TaskModel)?.IndexStatus}{(y as TaskModel)?.DateEnd:yyyy/MM/dd HH:mm:ss}");
}

public class SortTasksByProjectTimeline : NaturalSortComparer
{
  public override int Compare(object? x, object? y) =>
    StrCmpLogicalW(
      $"{(x as TaskModel)?.Project?.Key}{(x as TaskModel)?.IndexStatus}{(x as TaskModel)?.DateEnd:yyyy/MM/dd HH:mm:ss}",
      $"{(y as TaskModel)?.Project?.Key}{(y as TaskModel)?.IndexStatus}{(y as TaskModel)?.DateEnd:yyyy/MM/dd HH:mm:ss}");
}

public class SortTasksByUser : NaturalSortComparer
{
  public override int Compare(object? x, object? y) =>
    StrCmpLogicalW(
      $"{(x as TaskModel)?.User?.Team}{(x as TaskModel)?.User?.Name}{(x as TaskModel)?.DateEnd:yyyy/MM/dd HH:mm:ss}",
      $"{(y as TaskModel)?.User?.Team}{(y as TaskModel)?.User?.Name}{(y as TaskModel)?.DateEnd:yyyy/MM/dd HH:mm:ss}");
}

public class SortTasksByUserAdmin : NaturalSortComparer
{
  public override int Compare(object? x, object? y) =>
    StrCmpLogicalW(
      $"{(x as TaskModel)?.User?.Team}-{(x as TaskModel)?.User?.Name}",
      $"{(y as TaskModel)?.User?.Team}-{(y as TaskModel)?.User?.Name}");
}

public class SortNotifyByDate : NaturalSortComparer
{
  public override int Compare(object? x, object? y) =>
    StrCmpLogicalW(
      $"{(y as NotifyModel)?.CreateAt:yyyy/MM/dd HH:mm:ss}",
      $"{(x as NotifyModel)?.CreateAt:yyyy/MM/dd HH:mm:ss}");
}

public class SortUsersByTeam : NaturalSortComparer
{
  public override int Compare(object? x, object? y) =>
    StrCmpLogicalW(
      $"{(x as UserModel)?.Team}{(x as UserModel)?.Role}{(x as UserModel)?.Name}",
      $"{(y as UserModel)?.Team}{(y as UserModel)?.Role}{(y as UserModel)?.Name}");
}

public class SortProjectsByKey : NaturalSortComparer
{
  public override int Compare(object? x, object? y) =>
    StrCmpLogicalW((x as ProjectModel)?.Key ?? "", (y as ProjectModel)?.Key ?? "");
}

public class SortProjectsByTaskCount : NaturalSortComparer
{
  public override int Compare(object? x, object? y) =>
    StrCmpLogicalW(
      $"{(y as ProjectModel)?.Tasks?.Count}{(y as ProjectModel)?.Key}",
      $"{(x as ProjectModel)?.Tasks?.Count}{(x as ProjectModel)?.Key}");
}

public class SortTasksByDateEnd : NaturalSortComparer
{
  public override int Compare(object? x, object? y) =>
    StrCmpLogicalW(
      $"{(x as TaskModel)?.DateEnd:yyyy/MM/dd HH:mm:ss}",
      $"{(y as TaskModel)?.DateEnd:yyyy/MM/dd HH:mm:ss}");
}

public class SortTasksByDateStart : NaturalSortComparer
{
  public override int Compare(object? x, object? y) =>
    StrCmpLogicalW(
      $"{(x as TaskModel)?.CreateAt:yyyy/MM/dd HH:mm:ss}",
      $"{(y as TaskModel)?.CreateAt:yyyy/MM/dd HH:mm:ss}");
}
