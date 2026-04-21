using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMKApp.Models;
using NMKApp.Services;
using System.Windows.Data;

namespace NMKApp.ViewModels;

/// <summary>
/// Temporary tasks page ViewModel.
/// </summary>
public partial class TemporaryViewModel : ObservableObject
{
  private readonly ISupabaseService _supabaseService;
  private MainWindowViewModel? _parent;

  [ObservableProperty] private ListCollectionView? _tasksTemporaryCollection;

  public TemporaryViewModel(ISupabaseService supabaseService)
  {
    _supabaseService = supabaseService;
  }

  public void Initialize(MainWindowViewModel parent)
  {
    _parent = parent;
    TasksTemporaryCollection = new ListCollectionView(parent.TasksTemporary.Items);
  }

  [RelayCommand]
  private void SelectAll(object? parameter)
  {
    // TODO: Migrate TemporarySelectAllCommandAsync
  }

  [RelayCommand]
  private async Task SaveAsync()
  {
    // TODO: Migrate TemporarySaveCommandAsync
    await Task.CompletedTask;
  }

  [RelayCommand]
  private async Task AddTaskAsync()
  {
    // TODO: Migrate TemporaryAddTaskCommandAsync
    await Task.CompletedTask;
  }

  [RelayCommand]
  private async Task DeleteAsync(object? parameter)
  {
    // TODO: Migrate TemporaryDeleteCommandAsync
    await Task.CompletedTask;
  }

  [RelayCommand]
  private void Add(object? parameter)
  {
    // TODO: Migrate TemporaryAddCommandAsync
  }

  public void RefreshViews()
  {
    TasksTemporaryCollection?.Refresh();
  }
}
