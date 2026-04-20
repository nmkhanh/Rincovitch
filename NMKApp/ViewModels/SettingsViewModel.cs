using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMKApp.Services;

namespace NMKApp.ViewModels;

/// <summary>
/// Settings page ViewModel.
/// Handles version updates, file management, theme switching.
/// </summary>
public partial class SettingsViewModel : ObservableObject
{
  private readonly ISupabaseService _supabaseService;
  private MainWindowViewModel? _parent;

  public SettingsViewModel(ISupabaseService supabaseService)
  {
    _supabaseService = supabaseService;
  }

  public void Initialize(MainWindowViewModel parent)
  {
    _parent = parent;
  }

  [RelayCommand]
  private async Task FileVersionUpdateAsync()
  {
    // TODO: Migrate FileVersionUpdateCommandAsync
    await Task.CompletedTask;
  }

  [RelayCommand]
  private async Task FileVersionSelectAsync()
  {
    // TODO: Migrate FileVersionSelectCommandAsync
    await Task.CompletedTask;
  }

  [RelayCommand]
  private async Task FileVersionUploadAsync()
  {
    // TODO: Migrate FileVersionUploadCommandAsync
    await Task.CompletedTask;
  }
}
