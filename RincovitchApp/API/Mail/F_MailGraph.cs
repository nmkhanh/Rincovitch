using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Broker;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RincovitchApp.Models.ModelChilds;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RincovitchApp.API.Mail
{
  /// <summary>
  /// Gửi mail qua Microsoft Graph API.
  /// Tận dụng WAM (Windows Account Manager) để dùng session Outlook/Microsoft
  /// đang đăng nhập sẵn trên máy – không popup login lại.
  /// </summary>
  internal static class F_MailGraph
  {
    // ===== CẤU HÌNH AZURE AD APP =====
    public static string ClientId = "30a2d671-3ac4-4b85-b799-fdef947dae3c";
    public static string TenantId = "common";

    private static readonly string[] Scopes = ["User.Read", "Mail.Send"];

    private static readonly HttpClient _http = new();

    // App MSAL được build một lần duy nhất (singleton) để giữ token cache
    private static IPublicClientApplication? _msalApp;
    private static IPublicClientApplication MsalApp
    {
      get
      {
        if (_msalApp == null)
        {
          // WAM Broker: đọc tài khoản Windows/Outlook đang đăng nhập sẵn
          var brokerOptions = new BrokerOptions(BrokerOptions.OperatingSystems.Windows)
          {
            Title = "Rincovitch App",
            // Cho phép dùng tài khoản Microsoft cá nhân (MSA) lẫn AAD
            MsaPassthrough = true,
          };

          _msalApp = PublicClientApplicationBuilder
            .Create(ClientId)
            .WithAuthority(AzureCloudInstance.AzurePublic, TenantId)
            .WithBroker(brokerOptions)          // WAM: tận dụng session Windows
            .WithDefaultRedirectUri()
            .Build();
        }
        return _msalApp;
      }
    }

    // ──────────────────────────────────────────────────────────────
    // AUTHENTICATION – WAM silent trước, interactive chỉ khi cần
    // ──────────────────────────────────────────────────────────────

    internal static async Task<string?> AcquireTokenAsync()
    {
      using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(30));
      try
      {
        // 1. Thử lấy tài khoản đã cache
        var accounts = await MsalApp.GetAccountsAsync();
        var account = accounts.FirstOrDefault();

        // 2. Nếu chưa có account nào trong cache, dùng OperatingSystemAccount
        //    WAM sẽ tự đọc tài khoản Windows đang đăng nhập (Outlook, Teams, ...)
        account ??= PublicClientApplication.OperatingSystemAccount;

        // Silent only: không bao giờ popup, nếu thất bại trả null
        var result = await MsalApp
          .AcquireTokenSilent(Scopes, account)
          .ExecuteAsync(cts.Token);
        return result.AccessToken;
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"[F_MailGraph] AcquireTokenAsync failed: {ex.GetType().Name}: {ex.Message}");
        return null;
      }
    }

    // ──────────────────────────────────────────────────────────────
    // GET USER EMAIL
    // ──────────────────────────────────────────────────────────────

    internal static async Task<string?> GetCurrentUserEmailAsync()
    {
      try
      {
        var token = await AcquireTokenAsync();
        if (token == null) return null;

        using var req = new HttpRequestMessage(HttpMethod.Get,
          "https://graph.microsoft.com/v1.0/me?$select=mail,userPrincipalName");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resp = await _http.SendAsync(req);
        resp.EnsureSuccessStatusCode();

        var json = JObject.Parse(await resp.Content.ReadAsStringAsync());
        var email = json["mail"]?.ToString();

        if (string.IsNullOrEmpty(email))
          email = json["userPrincipalName"]?.ToString();

        System.Diagnostics.Debug.WriteLine($"[F_MailGraph] GetCurrentUserEmailAsync: {email}");
        return email;
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"[F_MailGraph] GetCurrentUserEmailAsync failed: {ex.GetType().Name}: {ex.Message}");
        return null;
      }
    }

    // ──────────────────────────────────────────────────────────────
    // SEND MAIL HELPER
    // ──────────────────────────────────────────────────────────────

    private static async Task SendMailAsync(string toEmail, string subject, string htmlBody, string? ccEmail = null)
    {
      var token = await AcquireTokenAsync();
      if (token == null) throw new InvalidOperationException("Không thể lấy token Microsoft Graph.");

      var toRecipients = new JArray
      {
        new JObject { ["emailAddress"] = new JObject { ["address"] = toEmail } }
      };

      var ccRecipients = new JArray();
      if (!string.IsNullOrEmpty(ccEmail))
        ccRecipients.Add(new JObject { ["emailAddress"] = new JObject { ["address"] = ccEmail } });

      var message = new JObject
      {
        ["subject"] = subject,
        ["body"] = new JObject { ["contentType"] = "HTML", ["content"] = htmlBody },
        ["toRecipients"] = toRecipients
      };

      if (ccRecipients.Count > 0)
        message["ccRecipients"] = ccRecipients;

      var payload = new JObject { ["message"] = message, ["saveToSentItems"] = true };

      using var req = new HttpRequestMessage(HttpMethod.Post, "https://graph.microsoft.com/v1.0/me/sendMail");
      req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
      req.Content = new StringContent(payload.ToString(Formatting.None), Encoding.UTF8, "application/json");

      var resp = await _http.SendAsync(req);
      if (!resp.IsSuccessStatusCode)
      {
        var err = await resp.Content.ReadAsStringAsync();
        throw new Exception($"Graph API sendMail thất bại ({(int)resp.StatusCode}): {err}");
      }
    }

    // ──────────────────────────────────────────────────────────────
    // PUBLIC SEND METHODS
    // ──────────────────────────────────────────────────────────────

    internal static async Task SendTaskMailAsync(string toEmail, string taskName, string onlyname, string description, DateTime start, DateTime end, string assigner)
    {
      string html = $@"
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
      await SendMailAsync(toEmail, $"[TASK] : {taskName}", html);
    }

    internal static async Task SendTaskMailCompleteAsync(string toEmail, string taskName, string onlyname, string description, DateTime end, string assigner)
    {
      string html = $@"
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
      await SendMailAsync(toEmail, $"[TASK] : {taskName}", html);
    }

    internal static async Task SendLeaveMailAsync(string toEmail, string toName, string leaveType, string description, ObservableCollection<NMK_M_LeaveDay> leaveDays, string assigner, string cc)
    {
      string rows = string.Join("", leaveDays.Select(ld =>
        $"<li>{ld.Start:dd/MM/yyyy} {ld.StartH:D2}:{ld.StartM:D2} - {ld.End:dd/MM/yyyy} {ld.EndH:D2}:{ld.EndM:D2}</li>"));
      string html = $@"
      <div style='font-family:Verdana; font-size:12px;'>
      <p>Dear {toName}</p>
      <p>You have a new leave request.</p>
      <ul style='margin:0; padding-left:0px;'>
        <li><b>Leave Type:</b> {leaveType}</li>
        <li><b>Description:</b> {description}</li>
        <li><b>Leave Days:</b><ul style='margin:0; padding-left:15px;'>{rows}</ul></li>
        <li><b>Requested by:</b> {assigner}</li>
      </ul>
      <p>Please review the leave request and respond accordingly.</p>
      <p>Thank you and Best Regards,</p>
      </div>";
      await SendMailAsync(toEmail, $"[LEAVE REQUEST] : {description}", html, cc);
    }

    internal static async Task ApprovalLeaveMailAsync(string toEmail, string toName, string leaveType, string description, ObservableCollection<NMK_M_LeaveDay> leaveDays, string assigner, string cc)
    {
      string rows = string.Join("", leaveDays.Select(ld =>
        $"<li>{ld.Start:dd/MM/yyyy} {ld.StartH:D2}:{ld.StartM:D2} - {ld.End:dd/MM/yyyy} {ld.EndH:D2}:{ld.EndM:D2}</li>"));
      string html = $@"
      <div style='font-family:Verdana; font-size:12px;'>
      <p>Dear {toName}</p>
      <p>Your leave request has been approved.</p>
      <ul style='margin:0; padding-left:0px;'>
        <li><b>Leave Type:</b> {leaveType}</li>
        <li><b>Description:</b> {description}</li>
        <li><b>Leave Days:</b><ul style='margin:0; padding-left:15px;'>{rows}</ul></li>
        <li><b>Approved by:</b> {assigner}</li>
      </ul>
      <p>Please make a note of the approved leave days.</p>
      <p>Thank you and Best Regards,</p>
      </div>";
      await SendMailAsync(toEmail, $"[LEAVE APPROVAL] : {description}", html, cc);
    }
  }
}

