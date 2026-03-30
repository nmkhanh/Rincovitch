using Autodesk.Revit.DB;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RincovitchTools.Print.API.Revit
{
  public static class F_PDF24
  {
    private const string KeyPath = @"Software\PDF24\Services\PDF";

    public static void SetAutoSave(string outputDir)
    {
      try
      {
        using (var key = Registry.CurrentUser.CreateSubKey(KeyPath))
        {

          key.SetValue("AutoSaveDir", outputDir, RegistryValueKind.String);
          key.SetValue("AutoSaveFilename", "$fileName", RegistryValueKind.String);
          key.SetValue("ShowSaveDialog", 0, RegistryValueKind.DWord);
          key.SetValue("AutoSaveEnabled", 1, RegistryValueKind.DWord);

          key.SetValue("LoadInCreatorIfOpen", 0, RegistryValueKind.DWord);

          key.SetValue("AutoSaveProfile", "default/high", RegistryValueKind.String);


          key.SetValue("AutoSaveOpenDir", 0, RegistryValueKind.DWord);
          key.SetValue("AutoSaveOverwriteFile", 1, RegistryValueKind.DWord);

          key.SetValue("AutoSaveShowProgress", 0, RegistryValueKind.DWord);
          key.SetValue("AutoSaveUseFileChooser", 0, RegistryValueKind.DWord);
          key.SetValue("AutoSaveUseFileCmd", 0, RegistryValueKind.DWord);

          //key.SetValue("Handler", "assistant", RegistryValueKind.String);
          key.SetValue("Handler", "autoSave", RegistryValueKind.String);
        }
      }
      catch (Exception)
      {

      }
    }

    public static void SetAutoSaveDir(string outputDir)
    {
      try
      {
        using (var key = Registry.CurrentUser.CreateSubKey(KeyPath))
        {
          key.SetValue("AutoSaveDir", outputDir, RegistryValueKind.String);
        }
      }
      catch (Exception)
      {
      }
    }
  }
}
