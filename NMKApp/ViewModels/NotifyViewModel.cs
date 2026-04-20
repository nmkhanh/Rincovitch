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

  [RelayCommand]
  private async Task ReadAsync(object? parameter)
  {
    // TODO: Migrate ReadNotifyCommandAsync
    await Task.CompletedTask;
  }
}
