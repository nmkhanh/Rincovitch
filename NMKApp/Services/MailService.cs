namespace NMKApp.Services;

/// <summary>
/// Mail service implementation.
/// TODO: Migrate COM/Graph dual Outlook logic from RincovitchApp/API/Mail/F_Mail.cs and F_MailGraph.cs
/// </summary>
public class MailService : IMailService
{
  public bool IsOutlookAvailable()
  {
    // TODO: Migrate from F_Mail.IsComOutlookAvailable()
    return false;
  }

  public async Task SendTaskMailAsync(string to, string subject, string body, string? cc = null)
  {
    // TODO: Migrate from F_Mail.SendTaskMailAsync
    await Task.CompletedTask;
  }

  public async Task SendTaskCompleteMailAsync(string to, string subject, string body)
  {
    // TODO: Migrate from F_Mail.SendTaskMailCompleteAsync
    await Task.CompletedTask;
  }

  public async Task SendLeaveMailAsync(string to, string subject, string body, string? cc = null)
  {
    // TODO: Migrate from F_Mail.SendLeaveMailAsync
    await Task.CompletedTask;
  }

  public async Task ApprovalLeaveMailAsync(string to, string subject, string body)
  {
    // TODO: Migrate from F_Mail.ApprovalLeaveMailAsync
    await Task.CompletedTask;
  }
}
