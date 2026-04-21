namespace NMKApp.Services;

/// <summary>
/// Mail service interface.
/// Extracted from F_Mail and F_MailGraph.
/// </summary>
public interface IMailService
{
  Task SendTaskMailAsync(string to, string subject, string body, string? cc = null);
  Task SendTaskCompleteMailAsync(string to, string subject, string body);
  Task SendLeaveMailAsync(string to, string subject, string body, string? cc = null);
  Task ApprovalLeaveMailAsync(string to, string subject, string body);
  bool IsOutlookAvailable();

  // Typed helpers — build HTML internally from domain parameters
  Task SendTaskMailTypedAsync(string toEmail, string taskName, string assigneeName,
    string description, DateTime start, DateTime end, string assignerName);
  Task SendTaskCompleteMailTypedAsync(string toEmail, string taskName, string assigneeName,
    string description, DateTime end, string assignerName);
  Task SendLeaveMailTypedAsync(string toEmail, string toName, string leaveType, string reason,
    IEnumerable<NMKApp.Models.DayModel> leaveDays, string assignerName, string? cc = null);
  Task ApprovalLeaveMailTypedAsync(string toEmail, string toName, string leaveType, string reason,
    IEnumerable<NMKApp.Models.DayModel> leaveDays, string assignerName, bool approved, string? cc = null);
}
