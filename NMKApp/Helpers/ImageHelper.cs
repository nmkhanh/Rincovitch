using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NMKApp.Helpers;

public static class ImageHelper
{
  public static BitmapImage? ByteArrayToBitmapImage(byte[]? bytes)
  {
    if (bytes == null || bytes.Length == 0) return null;

    var image = new BitmapImage();
    using var ms = new MemoryStream(bytes);
    image.BeginInit();
    image.CacheOption = BitmapCacheOption.OnLoad;
    image.StreamSource = ms;
    image.EndInit();
    image.Freeze();
    return image;
  }

  public static Icon? ByteArrayToIcon(byte[]? byteData)
  {
    if (byteData == null || byteData.Length == 0) return null;

    using var ms = new MemoryStream(byteData);
    using var bitmap = new Bitmap(ms);
    IntPtr hIcon = bitmap.GetHicon();
    var icon = Icon.FromHandle(hIcon);
    var clonedIcon = (Icon)icon.Clone();
    DestroyIcon(hIcon);
    return clonedIcon;
  }

  [DllImport("user32.dll", CharSet = CharSet.Auto)]
  private static extern bool DestroyIcon(IntPtr handle);

  public static ImageSource? Base64ToImageSource(string? base64)
  {
    try
    {
      if (string.IsNullOrEmpty(base64)) return null;

      byte[] binaryData = Convert.FromBase64String(base64);
      var image = new BitmapImage();
      using var stream = new MemoryStream(binaryData);
      image.BeginInit();
      image.CacheOption = BitmapCacheOption.OnLoad;
      image.StreamSource = stream;
      image.EndInit();
      image.Freeze();
      return image;
    }
    catch { return null; }
  }

  public static ImageSource? PathToImageSource(string? path)
  {
    try
    {
      if (string.IsNullOrEmpty(path)) return null;
      ImageSource image = new BitmapImage(new Uri(path, UriKind.Absolute));
      return image;
    }
    catch { return null; }
  }

  public static string? BitmapToBase64(Bitmap? bitmap, ImageFormat? format = null)
  {
    try
    {
      if (bitmap == null) return null;

      using var ms = new MemoryStream();
      bitmap.Save(ms, format ?? ImageFormat.Png);
      byte[] imageBytes = ms.ToArray();
      return Convert.ToBase64String(imageBytes);
    }
    catch (Exception ex)
    {
      System.Diagnostics.Trace.WriteLine("BitmapToBase64 error: " + ex.Message);
      return null;
    }
  }

  public static string? ImagePathToBase64(string? path)
  {
    try
    {
      if (string.IsNullOrEmpty(path)) return null;
      byte[] bytes = File.ReadAllBytes(path);
      return Convert.ToBase64String(bytes);
    }
    catch { return null; }
  }
}
