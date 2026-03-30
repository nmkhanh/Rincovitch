using System;
using System.Collections.Generic;
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

namespace RincovitchApp.API
{
  public class F_Image
  {
    //public static string GetBase64FromContact(Microsoft.Office.Interop.Outlook.ContactItem contact)
    //{
    //  if (contact == null || contact.Attachments.Count == 0) return null;

    //  foreach (Microsoft.Office.Interop.Outlook.Attachment attachment in contact.Attachments)
    //  {
    //    if (attachment.FileName.Equals("ContactPicture.jpg", StringComparison.OrdinalIgnoreCase))
    //    {
    //      string tempFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString() + ".jpg");
    //      try
    //      {
    //        attachment.SaveAsFile(tempFile);
    //        byte[] bytes = System.IO.File.ReadAllBytes(tempFile);

    //        // Xóa file ngay sau khi ??c vào m?ng byte
    //        if (System.IO.File.Exists(tempFile)) System.IO.File.Delete(tempFile);

    //        // Chuy?n ??i sang chu?i Base64
    //        return Convert.ToBase64String(bytes);
    //      }
    //      catch { return null; }
    //      finally { Marshal.ReleaseComObject(attachment); }
    //    }
    //    Marshal.ReleaseComObject(attachment);
    //  }
    //  return null;
    //}
    public static BitmapImage ByteArrayToBitmapImage(byte[] bytes)
    {
      if (bytes == null || bytes.Length == 0)
        return null;

      BitmapImage image = new BitmapImage();
      using (var ms = new MemoryStream(bytes))
      {
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad; // Quan tr?ng
        image.StreamSource = ms;
        image.EndInit();
        image.Freeze(); // Dùng cho binding / thread khác
      }
      return image;
    }

    public static Icon ByteArrayToIcon(byte[] byteData)
    {
      if (byteData == null || byteData.Length == 0)
        return null;

      using (MemoryStream ms = new MemoryStream(byteData))
      {
        // 1. T?o Bitmap t? m?ng byte
        using (Bitmap bitmap = new Bitmap(ms))
        {
          // 2. L?y Handle c?a Icon t? Bitmap
          IntPtr hIcon = bitmap.GetHicon();

          // 3. T?o ??i t??ng Icon t? Handle
          Icon icon = Icon.FromHandle(hIcon);

          // QUAN TR?NG: Ph?i sao chép icon và gi?i phóng Handle ?? tránh rò r? b? nh?
          Icon clonedIcon = (Icon)icon.Clone();
          DestroyIcon(hIcon);

          return clonedIcon;
        }
      }
    }

    // Khai báo hàm Win32 ?? gi?i phóng Icon Handle
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    extern static bool DestroyIcon(IntPtr handle);
    public static ImageSource Base64ToImageSource(string base64)
    {
      try
      {
        byte[] binaryData = Convert.FromBase64String(base64);

        var image = new BitmapImage();
        using (var stream = new MemoryStream(binaryData))
        {
          image.BeginInit();
          image.CacheOption = BitmapCacheOption.OnLoad;
          image.StreamSource = stream;
          image.EndInit();
          image.Freeze(); // T?i uu cho dùng da lu?ng/UI
        }
        return image;
      }
      catch
      {
        return null;
      }
    }

    public static ImageSource PathToImageSource(string path)
    {
      try
      {
        ImageSource image = new BitmapImage(new Uri(path, UriKind.Absolute));
        return image;
      }
      catch
      {
        return null;
      }
    }

    public static string BitmapToBase64(Bitmap bitmap, ImageFormat format = null)
    {
      try
      {
        if (bitmap == null)
          return null;

        using (var ms = new MemoryStream())
        {
          bitmap.Save(ms, format ?? ImageFormat.Png);
          byte[] imageBytes = ms.ToArray();
          return Convert.ToBase64String(imageBytes);
        }
      }
      catch (Exception ex)
      {
        System.Diagnostics.Trace.WriteLine("BitmapToBase64 error: " + ex.Message);
        return null;
      }
    }

    public static string ImagePathToBase64(string path)
    {
      try
      {
        byte[] bytes = File.ReadAllBytes(path);
        return Convert.ToBase64String(bytes);
      }
      catch (Exception ex)
      {
        // Log l?i n?u c?n
        return null;
      }
    }
  }
}
