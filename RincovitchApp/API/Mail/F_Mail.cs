using Microsoft.Win32;
using RincovitchApp.Models.ModelChilds;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace RincovitchApp.API.Mail
{
  public class F_Mail
  {
    [DllImport("oleaut32.dll", PreserveSig = false)]
    private static extern void GetActiveObject([In] ref Guid rclsid, IntPtr pvReserved, [MarshalAs(UnmanagedType.IUnknown)] out object ppunk);

    [DllImport("ole32.dll")]
    private static extern int CLSIDFromProgID([MarshalAs(UnmanagedType.LPWStr)] string lpszProgID, out Guid pclsid);

    // ──────────────────────────────────────────────────────────────
    // OUTLOOK TYPE DETECTION
    // ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Trả về true nếu Outlook COM (classic) đang chạy và có thể dùng được.
    /// New Outlook không đăng ký COM server nên sẽ trả về false.
    /// </summary>
    public static bool IsComOutlookAvailable()
    {
      try
      {
        CLSIDFromProgID("Outlook.Application", out Guid clsid);
        GetActiveObject(ref clsid, IntPtr.Zero, out object obj);
        var app = (Outlook.Application)obj;
        // Thử gọi 1 thuộc tính để xác nhận COM thực sự hoạt động
        _ = app.Version;
        Marshal.ReleaseComObject(app);
        return true;
      }
      catch
      {
        return false;
      }
    }

    // ──────────────────────────────────────────────────────────────
    // GET EMAIL – hỗ trợ cả COM và new Outlook (Graph)
    // ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Lấy email người dùng từ new Outlook / Microsoft account qua Graph API.
    /// Được gọi từ MainWindow khi COM không khả dụng.
    /// </summary>
    public static async Task<string> GetUserEmail_NewOutlook()
    {
      return await F_MailGraph.GetCurrentUserEmailAsync() ?? string.Empty;
    }

    // ──────────────────────────────────────────────────────────────
    // SEND MAIL – auto-detect COM vs Graph, đều là async
    // ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Gửi task mail. Tự động chọn COM Outlook hoặc Graph API.
    /// </summary>
    public static async Task SendTaskMailAsync(string toEmail, string taskName, string onlyname, string description, DateTime start, DateTime end, string assigner)
    {
      if (IsComOutlookAvailable())
        SendTaskMail(toEmail, taskName, onlyname, description, start, end, assigner);
      else
        await F_MailGraph.SendTaskMailAsync(toEmail, taskName, onlyname, description, start, end, assigner);
    }

    /// <summary>
    /// Gửi task complete mail. Tự động chọn COM Outlook hoặc Graph API.
    /// </summary>
    public static async Task SendTaskMailCompleteAsync(string toEmail, string taskName, string onlyname, string description, DateTime end, string assigner)
    {
      if (IsComOutlookAvailable())
        SendTaskMailComplete(toEmail, taskName, onlyname, description, end, assigner);
      else
        await F_MailGraph.SendTaskMailCompleteAsync(toEmail, taskName, onlyname, description, end, assigner);
    }

    /// <summary>
    /// Gửi leave request mail. Tự động chọn COM Outlook hoặc Graph API.
    /// </summary>
    public static async Task SendLeaveMailAsync(string toEmail, string toName, string leaveType, string description, ObservableCollection<NMK_M_LeaveDay> leaveDays, string assigner, string cc)
    {
      if (IsComOutlookAvailable())
        SendLeaveMail(toEmail, toName, leaveType, description, leaveDays, assigner, cc);
      else
        await F_MailGraph.SendLeaveMailAsync(toEmail, toName, leaveType, description, leaveDays, assigner, cc);
    }

    /// <summary>
    /// Gửi leave approval mail. Tự động chọn COM Outlook hoặc Graph API.
    /// </summary>
    public static async Task ApprovalLeaveMailAsync(string toEmail, string toName, string leaveType, string description, ObservableCollection<NMK_M_LeaveDay> leaveDays, string assigner, string cc)
    {
      if (IsComOutlookAvailable())
        ApprovalLeaveMail(toEmail, toName, leaveType, description, leaveDays, assigner, cc);
      else
        await F_MailGraph.ApprovalLeaveMailAsync(toEmail, toName, leaveType, description, leaveDays, assigner, cc);
    }

    /// <summary>
    /// Lấy instance Outlook COM đang chạy. Trả về null nếu Outlook chưa mở.
    /// KHÔNG tạo mới Outlook.Application vì process mới chưa có MAPI profile.
    /// </summary>
    public static Outlook.Application? GetActiveObject(string progId)
    {
      try
      {
        CLSIDFromProgID(progId, out Guid clsid);
        GetActiveObject(ref clsid, IntPtr.Zero, out object obj);
        return (Outlook.Application)obj;
      }
      catch
      {
        return null;
      }
    }

    /// <summary>
    /// Đọc email Outlook từ Windows Registry – không cần Outlook đang mở.
    /// Outlook lưu profile vào HKCU\Software\Microsoft\Office\xx.0\Outlook\Profiles\...
    /// </summary>
    public static string GetEmailFromRegistry()
    {
      try
      {
        // Thử từng phiên bản Office phổ biến (16.0 = Office 2016/2019/365, 15.0 = 2013, ...)
        string[] versions = ["16.0", "15.0", "14.0"];
        foreach (var ver in versions)
        {
          string profilesPath = $@"Software\Microsoft\Office\{ver}\Outlook\Profiles";
          using var profilesKey = Registry.CurrentUser.OpenSubKey(profilesPath);
          if (profilesKey == null) continue;

          foreach (var profileName in profilesKey.GetSubKeyNames())
          {
            using var profileKey = profilesKey.OpenSubKey(profileName);
            if (profileKey == null) continue;

            var email = SearchEmailInProfileKey(profileKey);
            if (!string.IsNullOrEmpty(email))
            {
              System.Diagnostics.Debug.WriteLine($"[Registry] Found email: {email} (profile: {profileName}, Office {ver})");
              return email;
            }
          }
        }
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"[Registry] Exception: {ex.Message}");
      }
      return null;
    }

    private static string SearchEmailInProfileKey(RegistryKey key)
    {
      // Tìm trong các subkey của profile (có thể lồng nhiều cấp)
      foreach (var subName in key.GetSubKeyNames())
      {
        using var sub = key.OpenSubKey(subName);
        if (sub == null) continue;

        // Tìm value "Email" hoặc "Account Name" chứa dấu @
        foreach (var valueName in sub.GetValueNames())
        {
          if (sub.GetValue(valueName) is string val && val.Contains("@") && val.Contains("."))
          {
            return val;
          }
        }

        // Đọc binary value 0x6603001e (SMTP address trong MAPI)
        var smtpRaw = sub.GetValue("\x00\x03\x66\x03\x00\x1e");
        if (smtpRaw is byte[] bytes && bytes.Length > 2)
        {
          // Chuỗi kết thúc bằng null bytes, decode UTF-8 hoặc ASCII
          var str = System.Text.Encoding.UTF8.GetString(bytes).TrimEnd('\0');
          if (str.Contains("@") && str.Contains("."))
            return str;
        }

        var nested = SearchEmailInProfileKey(sub);
        if (!string.IsNullOrEmpty(nested)) return nested;
      }
      return null;
    }

    public static bool IsOutlookLoggedIn(out string email, out string avatarBytes)
    {
      email = null;
      avatarBytes = null;
      Outlook.Application outlookApp = null;
      Outlook.NameSpace ns = null;

      try
      {
        // Chỉ lấy instance đang chạy – KHÔNG tạo mới (tránh MAPI chưa có profile)
        outlookApp = GetActiveObject("Outlook.Application");
        if (outlookApp == null)
        {
          System.Diagnostics.Debug.WriteLine("[COM] Outlook process không chạy");
          return false;
        }
        ns = outlookApp.GetNamespace("MAPI");

        // Force Outlook connect profile
        ns.Logon(Type.Missing, Type.Missing, false, false);

        // --- CHIẾN THUẬT 1: Lấy từ danh sách Accounts (Thường là ổn định nhất) ---
        if (ns.Accounts.Count > 0)
        {
          foreach (Outlook.Account account in ns.Accounts)
          {
            // Kiểm tra SmtpAddress có dấu '@' và dấu '.' (đảm bảo là email đầy đủ)
            if (!string.IsNullOrEmpty(account.SmtpAddress) && account.SmtpAddress.Contains("@"))
            {
              email = account.SmtpAddress;
              break;
            }
          }
        }

        // --- CHIẾN THUẬT 2: Nếu Accounts thất bại, thử lấy từ CurrentUser ---
        if (string.IsNullOrEmpty(email))
        {
          Outlook.AddressEntry addrEntry = ns.CurrentUser.AddressEntry;
          if (addrEntry.Type == "EX")
          {
            Outlook.ExchangeUser exchUser = addrEntry.GetExchangeUser();
            email = exchUser?.PrimarySmtpAddress;
          }

          // Nếu vẫn null hoặc không phải định dạng email (như máy 2 bị nhan@)
          if (string.IsNullOrEmpty(email) || !email.Contains("@"))
          {
            email = addrEntry.Address;
          }
        }

        // --- CHIẾN THUẬT 3: Xử lý chuỗi nếu máy 2 trả về định dạng lạ ---
        // Đôi khi Outlook trả về kiểu "/o=ExchangeLabs/ou=Exchange+Administrative+Group/..."
        if (email != null && email.Contains("/cn="))
        {
          // Nếu dính chuỗi Exchange Legacy DN, ép buộc lấy lại từ PrimarySmtpAddress
          email = ns.CurrentUser.AddressEntry.GetExchangeUser()?.PrimarySmtpAddress;
        }

        return !string.IsNullOrEmpty(email);
      }
      catch
      {
        return false;
      }
      finally
      {
        if (ns != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(ns);
        if (outlookApp != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(outlookApp);
      }
    }

    public static void SendTaskMail(string toEmail, string taskName, string onlyname, string description, DateTime start, DateTime end, string assigner)
    {
      Outlook.Application outlookApp = null;
      Outlook.MailItem mailItem = null;

      try
      {
        outlookApp = GetActiveObject("Outlook.Application");
        mailItem = (Outlook.MailItem)outlookApp.CreateItem(Outlook.OlItemType.olMailItem);

        mailItem.To = toEmail;
        mailItem.Subject = $"[TASK] : {taskName}";

        mailItem.HTMLBody = $@"
        <div style='font-family:Verdana; font-size:12px;'>
        <p>Hello,</p>

        <p>You have been assigned a new task in the system.</p>

        <ul style='margin:0; padding-left:0px;'>
          <li><b>Task:</b> {onlyname}</li>
          <li><b>Description:</b> {description}</li>
          <li><b>Deadline:</b> {start:dd/MM/yyyy HH:mm} - {end:dd/MM/yyyy HH:mm}</li>
          <li><b>Assigned by:</b> {assigner}</li>
        </ul>

        <p>Please review the task and complete it on schedule.</p>

        <p>Thank you and Best Regards,</p>
        </div>";

        // Gửi luôn (không mở Outlook UI)
        mailItem.Send();

        // Hoặc nếu muốn mở mail cho user xem trước:
        // mailItem.Display(false);
      }
      catch (Exception ex)
      {
        // NÊN log lại
        throw new Exception("Gửi email bằng Outlook thất bại", ex);
      }
      finally
      {
        // Giải phóng COM object (rất quan trọng)
        if (mailItem != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(mailItem);
        if (outlookApp != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(outlookApp);

        mailItem = null;
        outlookApp = null;
      }
    }
    public static void SendTaskMailComplete(string toEmail, string taskName, string onlyname, string description, DateTime end, string assigner)
    {
      Outlook.Application outlookApp = null;
      Outlook.MailItem mailItem = null;

      try
      {
        outlookApp = GetActiveObject("Outlook.Application");
        mailItem = (Outlook.MailItem)outlookApp.CreateItem(Outlook.OlItemType.olMailItem);

        mailItem.To = toEmail;
        mailItem.Subject = $"[TASK] : {taskName}";

        mailItem.HTMLBody = $@"
        <div style='font-family:Verdana; font-size:12px;'>
        <p>Hello,</p>

        <p>This is to inform you that the task below has been marked as completed.</p>

        <ul style='margin:0; padding-left:0px;'>
          <li><b>Task:</b> {onlyname}</li>
          <li><b>Description:</b> {description}</li>
          <li><b>Completion date:</b> {end:dd/MM/yyyy HH:mm}</li>
          <li><b>Confirmed by:</b> {assigner}</li>
        </ul>

        <p>Thank you for your effort.</p>

        <p>Best Regards,</p>
        </div>";

        // Gửi luôn (không mở Outlook UI)
        mailItem.Send();

        // Hoặc nếu muốn mở mail cho user xem trước:
        // mailItem.Display(false);
      }
      catch (Exception ex)
      {
        // NÊN log lại
        throw new Exception("Gửi email bằng Outlook thất bại", ex);
      }
      finally
      {
        // Giải phóng COM object (rất quan trọng)
        if (mailItem != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(mailItem);
        if (outlookApp != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(outlookApp);

        mailItem = null;
        outlookApp = null;
      }
    }

    public static void ReplyTaskMail(Outlook.MailItem originalMail, string taskName, string onlyname, string description, DateTime start, DateTime end, string assigner)
    {
      Outlook.MailItem reply = null;

      try
      {
        reply = originalMail.Reply(); // hoặc ReplyAll()

        string newHtml = $@"
        <div style='font-family:Verdana; font-size:12px;'>
          <p>Hello,</p>

          <p>You have been assigned a new task in the system.</p>

          <ul style='margin:0; padding-left:15px;'>
            <li><b>Task:</b> {onlyname}</li>
            <li><b>Description:</b> {description}</li>
            <li><b>Deadline:</b> {start:dd/MM/yyyy HH:mm} - {end:dd/MM/yyyy HH:mm}</li>
            <li><b>Assigned by:</b> {assigner}</li>
          </ul>

          <p>Please review the task and complete it on schedule.</p>

          <p>Thank you and Best Regards,</p>
        </div>
        <br/>
        ";

        // 👉 CHÈN LÊN TRÊN, GIỮ NGUYÊN MAIL CŨ + SIGNATURE
        reply.HTMLBody = newHtml + reply.HTMLBody;

        // Gửi luôn
        reply.Send();

        // Hoặc cho user xem trước:
        // reply.Display(false);
      }
      catch (Exception ex)
      {
        throw new Exception("Reply email bằng Outlook thất bại", ex);
      }
      finally
      {
        if (reply != null) Marshal.ReleaseComObject(reply);
      }
    }

    public static void SendLeaveMail(string toEmail, string toName, string leaveType, string description, ObservableCollection<NMK_M_LeaveDay> leaveDays, string assigner, string cc)
    {
      Outlook.Application outlookApp = null;
      Outlook.MailItem mailItem = null;
      try
      {
        outlookApp = GetActiveObject("Outlook.Application");
        mailItem = (Outlook.MailItem)outlookApp.CreateItem(Outlook.OlItemType.olMailItem);
        mailItem.To = toEmail;
        mailItem.Subject = $"[LEAVE REQUEST] : {description}";
        mailItem.HTMLBody = $@"
        <div style='font-family:Verdana; font-size:12px;'>
        <p>Dear {toName}</p>
        <p>You have a new leave request.</p>
        <ul style='margin:0; padding-left:0px;'>
          <li><b>Leave Type:</b> {leaveType}</li>
          <li><b>Description:</b> {description}</li>
          <li><b>Leave Days:</b>
            <ul style='margin:0; padding-left:15px;'>
              {string.Join("", leaveDays.Select(ld => $"<li>{ld.Start:dd/MM/yyyy} {ld.StartH:D2}:{ld.StartM:D2} - {ld.End:dd/MM/yyyy} {ld.EndH:D2}:{ld.EndM:D2}</li>"))}
            </ul>
          </li>
          <li><b>Requested by:</b> {assigner}</li>
        </ul>
        <p>Please review the leave request and respond accordingly.</p>
        <p>Thank you and Best Regards,</p>
        </div>";
        // Gửi luôn (không mở Outlook UI)
        mailItem.Send();
        // Hoặc nếu muốn mở mail cho user xem trước:
        // mailItem.Display(false);
      }
      catch (Exception ex)
      {
        // NÊN log lại
        throw new Exception("Gửi email bằng Outlook thất bại", ex);
      }
      finally
      {
        // Giải phóng COM object (rất quan trọng)
        if (mailItem != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(mailItem);
        if (outlookApp != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(outlookApp);
        mailItem = null;
        outlookApp = null;
      }
    }
    public static void ApprovalLeaveMail(string toEmail, string toName, string leaveType, string description, ObservableCollection<NMK_M_LeaveDay> leaveDays, string assigner, string cc)
    {
      Outlook.Application outlookApp = null;
      Outlook.MailItem mailItem = null;
      try
      {
        outlookApp = GetActiveObject("Outlook.Application");
        mailItem = (Outlook.MailItem)outlookApp.CreateItem(Outlook.OlItemType.olMailItem);
        mailItem.To = toEmail;
        if (!string.IsNullOrEmpty(cc))
          mailItem.CC = cc;
        mailItem.Subject = $"[LEAVE APPROVAL] : {description}";
        mailItem.HTMLBody = $@"
        <div style='font-family:Verdana; font-size:12px;'>
        <p>Dear {toName}</p>
        <p>Your leave request has been approved.</p>
        <ul style='margin:0; padding-left:0px;'>
          <li><b>Leave Type:</b> {leaveType}</li>
          <li><b>Description:</b> {description}</li>
          <li><b>Leave Days:</b>
            <ul style='margin:0; padding-left:15px;'>
              {string.Join("", leaveDays.Select(ld => $"<li>{ld.Start:dd/MM/yyyy} {ld.StartH:D2}:{ld.StartM:D2} - {ld.End:dd/MM/yyyy} {ld.EndH:D2}:{ld.EndM:D2}</li>"))}
            </ul>
          </li>
          <li><b>Approved by:</b> {assigner}</li>
        </ul>
        <p>Please make a note of the approved leave days.</p>
        <p>Thank you and Best Regards,</p>
        </div>";
        // Gửi luôn (không mở Outlook UI)
        mailItem.Send();
        // Hoặc nếu muốn mở mail cho user xem trước:
        // mailItem.Display(false);
      }
      catch (Exception ex)
      {
        // NÊN log lại
        throw new Exception("Gửi email bằng Outlook thất bại", ex);
      }
      finally
      {
        // Giải phóng COM object (rất quan trọng)
        if (mailItem != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(mailItem);
        if (outlookApp != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(outlookApp);
        mailItem = null;
        outlookApp = null;
      }
    }
  }
}
