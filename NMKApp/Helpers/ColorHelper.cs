using System.Windows.Media;

namespace NMKApp.Helpers;

public static class ColorHelper
{
  public static string? BrushToHexRgb(Brush? brush)
  {
    if (brush is SolidColorBrush scb)
    {
      Color c = scb.Color;
      return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
    }
    return null;
  }
}
