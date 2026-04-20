using System.Windows;

namespace NMKApp.UITheme;

/// <summary>
/// Theme manager for switching between Light/Dark themes.
/// Adapted from RincovitchApp.MVVMWindows_Themes.
/// </summary>
public static class ThemeManager
{
  private static readonly string BaseThemeUri = "pack://application:,,,/NMKApp;component/UITheme/BaseTheme.xaml";
  private static readonly string ConverterUri = "pack://application:,,,/NMKApp;component/UITheme/Converter.xaml";

  private static readonly Dictionary<string, string> ThemeUris = new()
  {
    ["Dark"] = "pack://application:,,,/NMKApp;component/UITheme/DarkTheme.xaml",
    ["Light"] = "pack://application:,,,/NMKApp;component/UITheme/LightTheme.xaml"
  };

  public static void ApplyTheme(ResourceDictionary targetResources, string themeName)
  {
    targetResources.MergedDictionaries.Clear();

    targetResources.MergedDictionaries.Add(new ResourceDictionary
    {
      Source = new Uri(BaseThemeUri, UriKind.RelativeOrAbsolute)
    });

    targetResources.MergedDictionaries.Add(new ResourceDictionary
    {
      Source = new Uri(ConverterUri, UriKind.RelativeOrAbsolute)
    });

    if (ThemeUris.TryGetValue(themeName, out var themeUri))
    {
      targetResources.MergedDictionaries.Add(new ResourceDictionary
      {
        Source = new Uri(themeUri, UriKind.RelativeOrAbsolute)
      });
    }
  }
}
