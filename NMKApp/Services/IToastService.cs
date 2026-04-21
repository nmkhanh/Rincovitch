namespace NMKApp.Services;

/// <summary>
/// Toast notification service interface.
/// </summary>
public interface IToastService
{
  void Initialize();
  void Show(string title, string message, string taskId = "");
  void ShowTaskComplete(string taskId, string taskName);
  void ShowNewNotification(string taskId, string message);
}
