using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMKApp.Models;
using NMKApp.Services;
using System.Windows.Data;

namespace NMKApp.ViewModels;

/// <summary>
/// Email/Request page ViewModel.
/// Handles email task notifications and sending.
/// </summary>
public partial class EmailViewModel : ObservableObject
{
  private readonly IMailService _mailService;
  private MainWindowViewModel? _parent;

  public ListCollectionView? TasksEmailCollection { get; set; }
  public ListCollectionView? TasksEmailCollectionCount { get; set; }

  public EmailViewModel(IMailService mailService)
  {
    _mailService = mailService;
  }

  public void Initialize(MainWindowViewModel parent)
  {
    _parent = parent;
    // TODO: Migrate email collection views from NMK_M
  }

  [RelayCommand]
  private void SelectAll(object? parameter)
  {
    // TODO: Migrate EmailSelectAllCommandAsync
  }

  [RelayCommand]
  private async Task SendAsync()
  {
    // TODO: Migrate EmailSendCommandAsync
    await Task.CompletedTask;
  }

  public void RefreshViews()
  {
    TasksEmailCollection?.Refresh();
    TasksEmailCollectionCount?.Refresh();
  }
}
