using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using NMKApp.Models;

namespace NMKApp.Services;

/// <summary>
/// Dual-mode mail: COM Outlook (classic) → Graph API (new Outlook / WAM).
/// Adapted from RincovitchApp F_Mail.cs + F_MailGraph.cs.
/// </summary>
public class MailService : IMailService
{
  // ─── COM Detection ────────────────────────────────────────────────────────
  [DllImport("oleaut32.dll")]
  private static extern int GetActiveObject(
    [MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
    IntPtr pvReserved, [MarshalAs(UnmanagedType.IUnknown)] out object ppunk);

  public bool IsOutlookAvailable()
  {
    try
    {
      var hr = GetActiveObject(
        new Guid("0006F03A-0000-0000-C000-000000000046"),
        IntPtr.Zero, out _);
      return hr == 0;
    }
    catch
    {
      return false;
    }
  }

  // ─── Route ────────────────────────────────────────────────────────────────
  private static async Task SendAsync(string to, string subject, string htmlBody, string? cc = null)
  {
    // Try COM first (classic Outlook, no Auth header needed)
    if (TrySendViaCOM(to, subject, htmlBody, cc))
    {
      Debug.WriteLine("[Mail] Sent via COM Outlook");
      return;
    }

    // Fall back to Graph API
    var token = await AuthService.GetGraphTokenAsync();
    if (!string.IsNullOrEmpty(token))
    {
      await SendViaGraphAsync(token, to, subject, htmlBody, cc);
      Debug.WriteLine("[Mail] Sent via Graph API");
      return;
    }

    Debug.WriteLine("[Mail] No mail channel available — skipped.");
  }

  // ─── COM Send ─────────────────────────────────────────────────────────────
  private static bool TrySendViaCOM(string to, string subject, string htmlBody, string? cc)
  {
    object? outlookApp = null;
    object? mailItem = null;
    try
    {
      var hr = GetActiveObject(
        new Guid("0006F03A-0000-0000-C000-000000000046"),
        IntPtr.Zero, out outlookApp);
      if (hr != 0 || outlookApp == null) return false;

      var appType = outlookApp.GetType();
      mailItem = appType.InvokeMember("CreateItem",
        System.Reflection.BindingFlags.InvokeMethod, null, outlookApp, new object[] { 0 });
      if (mailItem == null) return false;

      var itemType = mailItem.GetType();
      itemType.InvokeMember("To", System.Reflection.BindingFlags.SetProperty, null, mailItem, new object[] { to });
      itemType.InvokeMember("Subject", System.Reflection.BindingFlags.SetProperty, null, mailItem, new object[] { subject });
      itemType.InvokeMember("HTMLBody", System.Reflection.BindingFlags.SetProperty, null, mailItem, new object[] { htmlBody });

      if (!string.IsNullOrEmpty(cc))
        itemType.InvokeMember("CC", System.Reflection.BindingFlags.SetProperty, null, mailItem, new object[] { cc });

      itemType.InvokeMember("Send", System.Reflection.BindingFlags.InvokeMethod, null, mailItem, null);
      return true;
    }
    catch (COMException ex)
    {
      Debug.WriteLine($"[Mail] COM error: {ex.Message}");
      return false;
    }
    finally
    {
      if (mailItem != null) Marshal.ReleaseComObject(mailItem);
      if (outlookApp != null) Marshal.ReleaseComObject(outlookApp);
    }
  }

  // ─── Graph Send ───────────────────────────────────────────────────────────
  private static async Task SendViaGraphAsync(string token, string to, string subject, string htmlBody, string? cc)
  {
    var toRecipients = to.Split(';', StringSplitOptions.RemoveEmptyEntries)
      .Select(e => new { emailAddress = new { address = e.Trim() } });

    var payload = new
    {
      message = new
      {
        subject,
        body = new { contentType = "HTML", content = htmlBody },
        toRecipients,
        ccRecipients = string.IsNullOrEmpty(cc)
          ? Array.Empty<object>()
          : cc.Split(';', StringSplitOptions.RemoveEmptyEntries)
               .Select(e => new { emailAddress = new { address = e.Trim() } })
               .ToArray<object>()
      },
      saveToSentItems = true
    };

    var json = JsonSerializer.Serialize(payload);
    using var http = new HttpClient();
    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    using var content = new StringContent(json, Encoding.UTF8, "application/json");
    var response = await http.PostAsync("https://graph.microsoft.com/v1.0/me/sendMail", content);
    if (!response.IsSuccessStatusCode)
    {
      var err = await response.Content.ReadAsStringAsync();
      Debug.WriteLine($"[Mail] Graph send failed: {response.StatusCode} — {err}");
    }
  }

  // ─── HTML Templates ───────────────────────────────────────────────────────
  public async Task SendTaskMailAsync(string to, string subject, string body, string? cc = null)
    => await SendAsync(to, subject, body, cc);

  public async Task SendTaskCompleteMailAsync(string to, string subject, string body)
    => await SendAsync(to, subject, body);

  public async Task SendLeaveMailAsync(string to, string subject, string body, string? cc = null)
    => await SendAsync(to, subject, body, cc);

  public async Task ApprovalLeaveMailAsync(string to, string subject, string body)
    => await SendAsync(to, subject, body);

  // ─── Typed helpers (domain-level) ─────────────────────────────────────────
  public async Task SendTaskMailTypedAsync(
    string toEmail, string taskName, string assigneeName,
    string description, DateTime start, DateTime end, string assignerName)
  {
    var subject = $"Task Assigned: {taskName}";
    var html = BuildTaskHtml(taskName, assigneeName, description, start, end, assignerName, isComplete: false);
    await SendAsync(toEmail, subject, html);
  }

  public async Task SendTaskCompleteMailTypedAsync(
    string toEmail, string taskName, string assigneeName,
    string description, DateTime end, string assignerName)
  {
    var subject = $"Task Completed: {taskName}";
    var html = BuildTaskHtml(taskName, assigneeName, description, DateTime.MinValue, end, assignerName, isComplete: true);
    await SendAsync(toEmail, subject, html);
  }

  public async Task SendLeaveMailTypedAsync(
    string toEmail, string toName, string leaveType, string reason,
    IEnumerable<DayModel> leaveDays, string assignerName, string? cc = null)
  {
    var subject = $"Leave Request: {leaveType} — {toName}";
    var html = BuildLeaveHtml(toName, leaveType, reason, leaveDays, assignerName, approved: null);
    await SendAsync(toEmail, subject, html, cc);
  }

  public async Task ApprovalLeaveMailTypedAsync(
    string toEmail, string toName, string leaveType, string reason,
    IEnumerable<DayModel> leaveDays, string assignerName, bool approved, string? cc = null)
  {
    var subject = approved
      ? $"Leave Approved: {leaveType} — {toName}"
      : $"Leave Rejected: {leaveType} — {toName}";
    var html = BuildLeaveHtml(toName, leaveType, reason, leaveDays, assignerName, approved);
    await SendAsync(toEmail, subject, html, cc);
  }

  // ─── HTML builders ────────────────────────────────────────────────────────
  private static string BuildTaskHtml(
    string taskName, string assigneeName, string description,
    DateTime start, DateTime end, string assignerName, bool isComplete)
  {
    var sb = new StringBuilder();
    sb.Append("<html><body style='font-family:Segoe UI,Arial;font-size:13px;color:#222;'>");
    sb.Append($"<h3 style='color:{(isComplete ? "#2e7d32" : "#1565c0")};'>");
    sb.Append(isComplete ? "✅ Task Completed" : "📋 New Task Assigned");
    sb.Append("</h3><table style='border-collapse:collapse;width:100%;max-width:600px;'>");
    sb.Append(Row("Task", taskName));
    sb.Append(Row("Assigned to", assigneeName));
    if (!isComplete) sb.Append(Row("Start", start.ToString("dd/MM/yyyy HH:mm")));
    sb.Append(Row("Due", end.ToString("dd/MM/yyyy HH:mm")));
    sb.Append(Row("By", assignerName));
    if (!string.IsNullOrWhiteSpace(description)) sb.Append(Row("Description", description));
    sb.Append("</table></body></html>");
    return sb.ToString();
  }

  private static string BuildLeaveHtml(
    string toName, string leaveType, string reason,
    IEnumerable<DayModel> leaveDays, string assignerName, bool? approved)
  {
    var sb = new StringBuilder();
    string headerColor = approved switch { true => "#2e7d32", false => "#c62828", _ => "#1565c0" };
    string headerText = approved switch { true => "✅ Leave Approved", false => "❌ Leave Rejected", _ => "📅 Leave Request" };
    sb.Append("<html><body style='font-family:Segoe UI,Arial;font-size:13px;color:#222;'>");
    sb.Append($"<h3 style='color:{headerColor};'>{headerText}</h3>");
    sb.Append("<table style='border-collapse:collapse;width:100%;max-width:600px;'>");
    sb.Append(Row("Name", toName));
    sb.Append(Row("Type", leaveType));
    sb.Append(Row("Reason", reason));
    sb.Append(Row("By", assignerName));

    var days = leaveDays.ToList();
    if (days.Count > 0)
    {
      var daysHtml = string.Join("<br/>", days.Select(d => d.Name.ToString("dd/MM/yyyy")));
      sb.Append(Row("Days", daysHtml));
    }

    sb.Append("</table></body></html>");
    return sb.ToString();
  }

  private static string Row(string label, string value) =>
    $"<tr><td style='padding:6px 12px;background:#f5f5f5;font-weight:600;width:140px;'>{label}</td>" +
    $"<td style='padding:6px 12px;border-bottom:1px solid #e0e0e0;'>{value}</td></tr>";
}
