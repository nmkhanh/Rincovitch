using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;

namespace RincovitchApp.API
{
  public class F_Color
  {
    public static string BrushToHexRgb(Brush brush)
    {
      if (brush is SolidColorBrush scb)
      {
        Color c = scb.Color;
        return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
      }
      return null;
    }
  }
}
