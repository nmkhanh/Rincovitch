using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMKApp.Models;
using NMKApp.Services;

namespace NMKApp.ViewModels;

/// <summary>
/// Project management page ViewModel.
/// </summary>
public partial class ProjectViewModel : ObservableObject
{
  private readonly ISupabaseService _supabaseService;
  private MainWindowViewModel? _parent;

  public ProjectViewModel(ISupabaseService supabaseService)
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
    // TODO: Migrate ProjectAddCommandAsync
    await Task.CompletedTask;
  }

  [RelayCommand]
  private async Task EditAsync(object? parameter)
  {
    // TODO: Migrate ProjectEditCommandAsync
    await Task.CompletedTask;
  }

  [RelayCommand]
  private async Task DeleteAsync(object? parameter)
  {
    // TODO: Migrate ProjectDeleteCommandAsync
    await Task.CompletedTask;
  }

  [RelayCommand]
  private void Color(object? parameter)
  {
    // TODO: Migrate ProjectColorCommandAsync
  }

  [RelayCommand]
  private void Image(object? parameter)
  {
    // TODO: Migrate ProjectImageCommandAsync
  }
}
