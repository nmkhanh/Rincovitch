using NMKApp.Models;

namespace NMKApp.Services;

/// <summary>
/// Realtime subscription service interface.
/// Extracted from MainWindowViewModel.InitRealtime.
/// </summary>
public interface IRealtimeService
{
  /// <summary>
  /// Subscribe to realtime changes for tasks, notifications, leaves, and versions.
  /// </summary>
  Task SubscribeAsync(
    Action<TaskModel> onTaskUpdated,
    Action<NotifyModel> onNotifyInserted,
    Action<NotifyModel> onNotifyUpdated,
    Action<LeaveModel> onLeaveUpdated,
    Action<VersionModel> onVersionInserted);
}
