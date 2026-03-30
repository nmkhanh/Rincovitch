using Newtonsoft.Json.Linq;
using RincovitchApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;

namespace RincovitchApp.API
{
  public class F_VersionApp
  {
    public static async Task UpdateFromDatabaseByte(NMK_Supabase_Version version)
    {
      string tempMsiPath = Path.Combine(Path.GetTempPath(), $"RincovitchApp-v{version.Version}.msi");

      try
      {

        var result = await NMK_Supabase.downloadfile_VersionAsync(version.Data);
        if (result.Success)
        {
          Properties.Settings.Default.VersionCurrent = version.Version;
          Properties.Settings.Default.Save();
          await File.WriteAllBytesAsync(tempMsiPath, result.Data);
          // 4. Ch?y l?nh cài ??t ?n
          Process.Start(new ProcessStartInfo
          {
            FileName = "msiexec.exe",
            Arguments = $"/i \"{tempMsiPath}\" /qf /norestart",
            UseShellExecute = true,
            WindowStyle = ProcessWindowStyle.Maximized
          });
          Environment.Exit(0);
        }
        else
        {
          Debug.WriteLine($"L?i khi t?i file cài ??t: {result.Error}");
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"L?i truy v?n Database: {ex.Message}");
      }
    }
  }
}
