using System.Collections.Generic;
using System.Windows;
using System;

namespace RincovitchTools
{
  public class MVVMWindows_Themes
  {
    public static readonly string GenericDict = "pack://application:,,,/RincovitchTools;component/UITheme/BaseTheme.xaml";
    public static readonly string DarkDict = "pack://application:,,,/RincovitchTools;component/UITheme/DarkTheme.xaml";
    public static readonly string LightDict = "pack://application:,,,/RincovitchTools;component/UITheme/LightTheme.xaml";

    public static readonly Dictionary<string, string> ThemeMap = new Dictionary<string, string>
        {
            { "Dark", DarkDict },
            { "Light", LightDict }
        };

    public static void ApplyTheme(ResourceDictionary targetResources, string themeName)
    {
      targetResources.MergedDictionaries.Clear();

      // Luôn load Generic trước
      targetResources.MergedDictionaries.Add(new ResourceDictionary
      {
        Source = new Uri(GenericDict, UriKind.RelativeOrAbsolute)
      });

      // Rồi load theme cụ thể
      targetResources.MergedDictionaries.Add(new ResourceDictionary
      {
        Source = new Uri(ThemeMap[themeName], UriKind.RelativeOrAbsolute)
      });
    }
  }
}

