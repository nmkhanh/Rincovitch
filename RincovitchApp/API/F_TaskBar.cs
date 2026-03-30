using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using Brushes = System.Windows.Media.Brushes;
using FontFamily = System.Windows.Media.FontFamily;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Size = System.Windows.Size;

namespace RincovitchApp.API
{
  internal class F_TaskBar
  {
    public static ImageSource UpdateTaskbarBadge(int count)
    {
      if (count <= 0) return null;

      double size = 16;
      Grid grid = new Grid { Width = size, Height = size };

      // Vẽ nền tròn đỏ
      grid.Children.Add(new System.Windows.Shapes.Ellipse
      {
        Fill = Brushes.Red,
        Stroke = Brushes.Red,
        Width = size,
        Height = size,
      });

      // Vẽ số thông báo
      grid.Children.Add(new TextBlock
      {
        Text = count > 99 ? "99+" : count.ToString(),
        FontSize = 12,
        //FontWeight = FontWeights.Bold,
        Foreground = Brushes.White,
        TextAlignment = TextAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
      });

      grid.Measure(new Size(size, size));
      grid.Arrange(new Rect(new Size(size, size)));

      RenderTargetBitmap bmp = new RenderTargetBitmap((int)size, (int)size, 96, 96, PixelFormats.Pbgra32);
      bmp.Render(grid);
      return bmp;
    }
  }
}
