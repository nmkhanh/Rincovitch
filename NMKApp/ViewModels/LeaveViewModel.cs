using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMKApp.Models;
using NMKApp.Services;
using System.Windows.Data;

namespace NMKApp.ViewModels;

/// <summary>
/// Leave management page ViewModel.
/// </summary>
public partial class LeaveViewModel : ObservableObject
{
  private readonly ISupabaseService _supabaseService;
  private readonly IMailService _mailService;
  private MainWindowViewModel? _parent;

  [ObservableProperty] private ListCollectionView? _usersCollectionLeave;

  public LeaveViewModel(ISupabaseService supabaseService, IMailService mailService)
  {
    _supabaseService = supabaseService;
    _mailService = mailService;
  }

  public void Initialize(MainWindowViewModel parent)
  {
    _parent = parent;
    UsersCollectionLeave = new ListCollectionView(parent.Users.Items);
  }

  [ObservableProperty] private LeaveModel? _selectedLeave;
  [ObservableProperty] private bool _isLoading;

  [RelayCommand]
  private async Task ApplyAsync()
  {
    if (_parent?.CurrentUser == null) return;
    var dlg = new Views.Dialogs.LeaveApplyDialog(_parent.Users.Items);
    if (dlg.ShowDialog() != true) return;
    IsLoading = true;
    try
    {
      var entity = new Data.Entities.LeaveEntity
      {
        Id = Guid.NewGuid().ToString(),
        CreateBy = _parent.CurrentUser.Email,
        SendTo = dlg.SendTo,
        CC = dlg.CC,
        Type = dlg.LeaveType,
        Reason = dlg.Reason,
        Approval = 2
      };
      var result = await _supabaseService.InsertLeaveAsync(entity);
      if (!result.Success) return;
      var leave = result.Data!;
      leave.User = _parent.Users.Items.FirstOrDefault(u => u.Email == entity.CreateBy);
      leave.UserCreateBy = leave.User;
      _parent.Leaves.Items.Add(leave);
      _parent.RefreshAllViews();

      _ = _mailService.SendLeaveMailTypedAsync(
        dlg.SendTo, leave.User?.Name ?? string.Empty,
        dlg.LeaveType, dlg.Reason,
        dlg.LeaveDays, _parent.CurrentUser.Name, dlg.CC);
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[Leave] Apply error: {ex.Message}");
    }
    finally { IsLoading = false; }
  }

  [RelayCommand]
  private async Task ApproveAsync(object? parameter)
  {
    await SetApprovalAsync(parameter as LeaveModel ?? SelectedLeave, approved: true);
  }

  [RelayCommand]
  private async Task RejectAsync(object? parameter)
  {
    await SetApprovalAsync(parameter as LeaveModel ?? SelectedLeave, approved: false);
  }

  private async Task SetApprovalAsync(LeaveModel? leave, bool approved)
  {
    if (leave == null || _parent?.CurrentUser == null) return;
    IsLoading = true;
    try
    {
      var entity = new Data.Entities.LeaveEntity
      {
        Id = leave.Id, CreateBy = leave.CreateBy, SendTo = leave.SendTo,
        CC = leave.CC, Type = leave.Type, Reason = leave.Reason,
        Approval = approved ? 1 : 0
      };
      await _supabaseService.UpdateLeaveAsync(entity);
      leave.Approval = entity.Approval;
      _parent.RefreshAllViews();

      _ = _mailService.ApprovalLeaveMailTypedAsync(
        leave.CreateBy, leave.User?.Name ?? string.Empty,
        leave.Type, leave.Reason, leave.LeaveList,
        _parent.CurrentUser.Name, approved, leave.CC);
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[Leave] Approval error: {ex.Message}");
    }
    finally { IsLoading = false; }
  }
}
