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
}
