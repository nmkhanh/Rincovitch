using RincovitchApp.Models.ModelChilds;
using Supabase.Gotrue.Mfa;
using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace RincovitchApp.API.Mail
{
  public class F_Mail_
  {
    [DllImport("oleaut32.dll", PreserveSig = false)]
    private static extern void GetActiveObject([In] ref Guid rclsid, IntPtr pvReserved, [MarshalAs(UnmanagedType.IUnknown)] out object ppunk);

    [DllImport("ole32.dll")]
    private static extern int CLSIDFromProgID([MarshalAs(UnmanagedType.LPWStr)] string lpszProgID, out Guid pclsid);

    public static Outlook.Application GetActiveObject(string progId)
    {
      try
      {
        CLSIDFromProgID(progId, out Guid clsid);
        GetActiveObject(ref clsid, IntPtr.Zero, out object obj);
        return (Outlook.Application)obj;
      }
      catch (Exception)
      {
        return new Outlook.Application();
      }
    }

    public static bool IsOutlookLoggedIn(out string email, out string avatarBytes)
    {
      email = null;
      avatarBytes = null;
      Outlook.Application outlookApp = null;
      Outlook.NameSpace ns = null;

      try
      {
        // Thử lấy instance Outlook đang chạy, nếu không có thì khởi tạo mới
        try
        {
          outlookApp = GetActiveObject("Outlook.Application");
        }
        catch
        {
          outlookApp = new Outlook.Application();
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
