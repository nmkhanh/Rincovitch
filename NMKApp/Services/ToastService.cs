using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Toolkit.Uwp.Notifications;
using NMKApp.Core;

namespace NMKApp.Services;

/// <summary>
/// Toast notification service.
/// Adapted from RincovitchApp.ToastService.
/// </summary>
public class ToastService : IToastService
{
  public void Initialize()
  {
    try
    {
      CreateShortcutIfNeeded();
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"[Toast] Init error: {ex.Message}");
    }
  }

  public void Show(string title, string message, string taskId = "")
  {
    new ToastContentBuilder()
      .AddText(title)
      .AddText(message)
      .AddArgument("taskId", taskId)
      .Show();
  }

  private static void CreateShortcutIfNeeded()
  {
    string shortcutPath = Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
      "Programs", $"{AppConstants.ToastShortcutName}.lnk");

    if (!File.Exists(shortcutPath))
      ShortcutHelper.CreateShortcut(shortcutPath, AppConstants.ToastAppId);
  }
}

#region Shortcut Helper
internal static class ShortcutHelper
{
  private static Guid AppUserModelIdKey = new("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3");

  public static void CreateShortcut(string shortcutPath, string appId)
  {
    IShellLink link = (IShellLink)new ShellLink();
    link.SetPath(Environment.ProcessPath!);

    IPropertyStore propertyStore = (IPropertyStore)link;
    PropVariant appIdData = new(appId);
    try
    {
      propertyStore.SetValue(ref AppUserModelIdKey, ref appIdData);
      propertyStore.Commit();
    }
    finally
    {
      appIdData.Dispose();
    }

    IPersistFile file = (IPersistFile)link;
    file.Save(shortcutPath, true);
  }

  [ComImport, Guid("00021401-0000-0000-C000-000000000046")]
  private class ShellLink { }

  [ComImport, Guid("000214F9-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  private interface IShellLink
  {
    void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);
    void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
  }

  [ComImport, Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  private interface IPropertyStore
  {
    void GetCount(out uint cProps);
    void GetAt(uint iProp, out IntPtr pkey);
    void GetValue(ref Guid pkey, out PropVariant pv);
    void SetValue(ref Guid pkey, ref PropVariant pv);
    void Commit();
  }

  [StructLayout(LayoutKind.Explicit)]
  public struct PropVariant : IDisposable
  {
    [FieldOffset(0)] public short vt;
    [FieldOffset(8)] public IntPtr ptr;

    public PropVariant(string value)
    {
      vt = 31;
      ptr = Marshal.StringToCoTaskMemUni(value);
    }

    public void Dispose()
    {
      if (ptr != IntPtr.Zero)
      {
        Marshal.FreeCoTaskMem(ptr);
        ptr = IntPtr.Zero;
      }
    }
  }
}
#endregion
