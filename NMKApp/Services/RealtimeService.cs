using NMKApp.Data.Entities;
using NMKApp.Models;
using static Supabase.Realtime.PostgresChanges.PostgresChangesOptions;

namespace NMKApp.Services;

/// <summary>
/// Realtime subscription service implementation.
/// TODO: Migrate detailed subscription logic from MainWindowViewModel.InitRealtime.
/// </summary>
public class RealtimeService(ISupabaseService supabaseService) : IRealtimeService
{
  public async Task SubscribeAsync(
    Action<TaskModel> onTaskUpdated,
    Action<NotifyModel> onNotifyInserted,
    Action<NotifyModel> onNotifyUpdated,
    Action<LeaveModel> onLeaveUpdated,
    Action<VersionModel> onVersionInserted)
  {
    var client = supabaseService.Client;

    // Version inserts
    var tableVersion = client.From<VersionEntity>();
    await tableVersion.On(ListenType.Inserts, (sender, change) =>
    {
      var entity = change.Model<VersionEntity>();
      onVersionInserted(entity.ToDomain());
    });

    // Notify inserts
    var tableNotify = client.From<NotifyEntity>();
    await tableNotify.On(ListenType.Inserts, (sender, change) =>
    {
      var entity = change.Model<NotifyEntity>();
      onNotifyInserted(entity.ToDomain());
    });

    // Notify updates
    await tableNotify.On(ListenType.Updates, (sender, change) =>
    {
      var entity = change.Model<NotifyEntity>();
      onNotifyUpdated(entity.ToDomain());
    });

    // Task updates
    var tableTask = client.From<TaskEntity>();
    await tableTask.On(ListenType.Updates, (sender, change) =>
    {
      var entity = change.Model<TaskEntity>();
      onTaskUpdated(entity.ToDomain());
    });

    // Leave updates
    var tableLeave = client.From<LeaveEntity>();
    await tableLeave.On(ListenType.Updates, (sender, change) =>
    {
      var entity = change.Model<LeaveEntity>();
      onLeaveUpdated(new LeaveModel
      {
        Id = entity.Id,
        Approval = entity.Approval,
        SendTo = entity.SendTo,
        CreateBy = entity.CreateBy,
        CCTo = entity.CCTo ?? string.Empty,
        UpdateAt = entity.UpdateAt
      });
    });
  }
}
