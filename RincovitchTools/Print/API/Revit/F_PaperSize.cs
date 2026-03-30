using System;
using System.Runtime.InteropServices;

namespace RincovitchTools.Print.API.Revit
{
  public static class PaperFormManager
  {
    [StructLayout(LayoutKind.Sequential)]
    private struct SIZEL
    {
      public int cx; // microns
      public int cy;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECTL
    {
      public int left;
      public int top;
      public int right;
      public int bottom;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct FORM_INFO_1
    {
      public uint Flags;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pName;
      public SIZEL Size;
      public RECTL ImageableArea;
    }

    [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool OpenPrinter(string pPrinterName, out IntPtr phPrinter, IntPtr pDefault);

    [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool AddForm(IntPtr hPrinter, uint Level, ref FORM_INFO_1 pForm);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool ClosePrinter(IntPtr hPrinter);

    public static void CreatePaperFormMM(string formName, double widthMm, double heightMm)
    {
      if (string.IsNullOrWhiteSpace(formName))
        throw new ArgumentException("Form name is empty");

      if (widthMm <= 0 || heightMm <= 0)
        throw new ArgumentException("Invalid paper size");

      var form = new FORM_INFO_1
      {
        Flags = 0,
        pName = formName,
        Size = new SIZEL
        {
          cx = (int)(widthMm * 1000.0),
          cy = (int)(heightMm * 1000.0)
        },
        ImageableArea = new RECTL
        {
          left = 0,
          top = 0,
          right = (int)(widthMm * 1000.0),
          bottom = (int)(heightMm * 1000.0)
        }
      };

      // Open local print server/printer. Use null to open default/local spooler.
      if (!OpenPrinter(null, out IntPtr hPrinter, IntPtr.Zero))
      {
        int err = Marshal.GetLastWin32Error();
        throw new InvalidOperationException($"OpenPrinter failed. Win32Error={err}");
      }

      try
      {
        bool ok = AddForm(hPrinter, 1, ref form);
        if (!ok)
        {
          int err = Marshal.GetLastWin32Error();
          string msg = err switch
          {
            5 => "Access denied (run as Administrator)",
            6 => "Invalid handle (printer handle is invalid)",
            183 => "Form already exists",
            _ => $"AddForm failed. Win32Error={err}"
          };
          throw new InvalidOperationException(msg);
        }
      }
      finally
      {
        ClosePrinter(hPrinter);
      }
    }
  }
}