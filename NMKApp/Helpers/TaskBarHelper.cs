using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NMKApp.Helpers;

public static class TaskBarHelper
{
  public static ImageSource? UpdateTaskbarBadge(int count)
  {
    if (count <= 0) return null;

    double size = 16;
    var grid = new Grid { Width = size, Height = size };

    grid.Children.Add(new System.Windows.Shapes.Ellipse
    {
      Fill = Brushes.Red,
      Stroke = Brushes.Red,
      Width = size,
      Height = size,
    });

    grid.Children.Add(new TextBlock
    {
      Text = count > 99 ? "99+" : count.ToString(),
      FontSize = 12,
      Foreground = Brushes.White,
      TextAlignment = TextAlignment.Center,
      VerticalAlignment = VerticalAlignment.Center,
    });

    grid.Measure(new Size(size, size));
    grid.Arrange(new Rect(new Size(size, size)));

    var bmp = new RenderTargetBitmap((int)size, (int)size, 96, 96, PixelFormats.Pbgra32);
    bmp.Render(grid);
    return bmp;
  }
}
