using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows;
using Microsoft.Toolkit.Uwp.Notifications;

namespace RincovitchApp
{
  public partial class App : System.Windows.Application
  {
    public static MainWindow MyMainWindow
    {
      get; set;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      // Đăng ký dịch vụ thông báo
      ToastService.Initialize();

      // Xử lý khi người dùng Click vào Toast thông báo
      ToastNotificationManagerCompat.OnActivated += toastArgs =>
      {
        var args = ToastArguments.Parse(toastArgs.Argument);
        string taskId = args.Get("taskId");
        // Sử dụng Dispatcher để thao tác với UI từ luồng khác
        Current.Dispatcher.Invoke(() =>
        {
          if (MyMainWindow != null)
          {
            MyMainWindow.ShowApp();

            if (!string.IsNullOrEmpty(taskId))
              // 2. Ép nhảy về màn hình Task
              MyMainWindow.NavigateToTaskView(taskId);
          }
        });
      };
    }
  }

  #region Toast Service
  public static class ToastService
  {
    public const string AppId = "Rincovitch.TaskManager.WPF";
    private const string ShortcutName = "My Task Manager";

    public static void Initialize()
    {
      try
      {
        CreateShortcutIfNeeded();
      }
      catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
    }

    public static void Show(string title, string message, string taskId = "")
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
          "Programs", $"{ShortcutName}.lnk");

      if (!File.Exists(shortcutPath))
        ShortcutHelper.CreateShortcut(shortcutPath, AppId);
    }
  }
  #endregion

  #region Shortcut Helper (Đã sửa lỗi ref using variable)
  public static class ShortcutHelper
  {
    private static Guid AppUserModelIdKey = new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3");

    public static void CreateShortcut(string shortcutPath, string appId)
    {
      IShellLink link = (IShellLink)new ShellLink();
      link.SetPath(Environment.ProcessPath);

      IPropertyStore propertyStore = (IPropertyStore)link;

      // FIX: Không dùng 'using' trực tiếp cho biến truyền vào 'ref'
      PropVariant appIdData = new PropVariant(appId);
      try
      {
        // Truyền ref thành công
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
    private class ShellLink
    {
    }

    [ComImport, Guid("000214F9-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IShellLink
    {
      void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);
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
}