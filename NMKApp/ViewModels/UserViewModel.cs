using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMKApp.Models;
using NMKApp.Services;

namespace NMKApp.ViewModels;

/// <summary>
/// User management page ViewModel.
/// </summary>
public partial class UserViewModel : ObservableObject
{
  private readonly ISupabaseService _supabaseService;
  private MainWindowViewModel? _parent;

  public UserViewModel(ISupabaseService supabaseService)
  {
    _supabaseService = supabaseService;
  }

  public void Initialize(MainWindowViewModel parent)
  {
    _parent = parent;
  }

  [RelayCommand]
  private async Task AddAsync()
  {
    // TODO: Migrate UserAddCommandAsync
    await Task.CompletedTask;
  }

  [RelayCommand]
  private async Task EditAsync(object? parameter)
  {
    // TODO: Migrate UserEditCommandAsync
    await Task.CompletedTask;
  }

  [RelayCommand]
  private async Task DeleteAsync(object? parameter)
  {
    // TODO: Migrate UserDeleteCommandAsync
    await Task.CompletedTask;
  }

  [RelayCommand]
  private async Task ImageAsync(object? parameter)
  {
    // TODO: Migrate UserImageCommandAsync
    await Task.CompletedTask;
  }
}
