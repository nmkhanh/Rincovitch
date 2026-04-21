using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMKApp.Models;
using NMKApp.Services;

namespace NMKApp.ViewModels;

/// <summary>
/// Notifications page ViewModel.
/// </summary>
public partial class NotifyViewModel : ObservableObject
{
  private readonly ISupabaseService _supabaseService;
  private MainWindowViewModel? _parent;

  public NotifyViewModel(ISupabaseService supabaseService)
  {
    _supabaseService = supabaseService;
  }

  public void Initialize(MainWindowViewModel parent)
  {
    _parent = parent;
  }

  [ObservableProperty] private int _unreadCount;

  [RelayCommand]
  private async Task ReadAsync(object? parameter)
  {
    var notify = parameter as NotifyModel;
    if (notify == null || notify.IsRead || _parent == null) return;

    try
    {
      var entity = new Data.Entities.NotifyEntity
      {
        Id = notify.Id,
        TaskId = notify.TaskId,
        Title = notify.Title,
        SendTo = notify.SendTo,
        CreateBy = notify.CreateBy,
        IsRead = true,
        Type = notify.Type,
        Status = notify.Status
      };
      await _supabaseService.UpdateNotifyAsync(entity);
      notify.IsRead = true;
      _parent.RefreshAllViews();
      UnreadCount = _parent.Notifys.Items.Count(n => !n.IsRead);
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[Notify] Read error: {ex.Message}");
    }
  }

  [RelayCommand]
  private async Task ReadAllAsync()
  {
    if (_parent == null) return;
    var unread = _parent.Notifys.Items.Where(n => !n.IsRead).ToList();
    foreach (var n in unread)
    {
      n.IsRead = true;
      var entity = new Data.Entities.NotifyEntity
      {
        Id = n.Id, TaskId = n.TaskId, Title = n.Title,
        SendTo = n.SendTo, CreateBy = n.CreateBy,
        IsRead = true, Type = n.Type, Status = n.Status
      };
      await _supabaseService.UpdateNotifyAsync(entity);
    }
    UnreadCount = 0;
    _parent.RefreshAllViews();
  }
}
