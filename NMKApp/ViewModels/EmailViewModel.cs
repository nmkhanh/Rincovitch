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

  [ObservableProperty] private ListCollectionView? _tasksEmailCollection;
  [ObservableProperty] private ListCollectionView? _tasksEmailCollectionCount;

  public EmailViewModel(IMailService mailService)
  {
    _mailService = mailService;
  }

  public void Initialize(MainWindowViewModel parent)
  {
    _parent = parent;
    TasksEmailCollection = new ListCollectionView(parent.Tasks.Items);
    TasksEmailCollection.Filter = obj =>
      obj is TaskModel t && t.Status == 3 && !t.IsAssignedTo;
    TasksEmailCollectionCount = new ListCollectionView(parent.Tasks.Items);
    TasksEmailCollectionCount.Filter = obj =>
      obj is TaskModel t && t.Status == 3 && !t.IsAssignedTo && !t.StateAccepted;
  }

  [RelayCommand]
  private void SelectAll(object? parameter)
  {
    if (_parent == null) return;
    bool selectAll = parameter is bool b && b;
    foreach (var task in _parent.Tasks.Items)
      if (task.Status == 3 && !task.IsAssignedTo)
        task.IsChecked = selectAll;
  }

  [RelayCommand]
  private async Task SendAsync()
  {
    if (_parent?.CurrentUser == null) return;
    var selected = _parent.Tasks.Items
      .Where(t => t.IsChecked && t.Status == 3 && !t.IsAssignedTo)
      .ToList();
    if (selected.Count == 0) return;

    foreach (var task in selected)
    {
      if (task.User?.Email == null) continue;
      await _mailService.SendTaskMailTypedAsync(
        task.User.Email, task.Name, task.User.Name,
        string.Empty, task.DateStart, task.DateEnd,
        _parent.CurrentUser.Name);
      task.IsChecked = false;
    }
    RefreshViews();
  }

  public void RefreshViews()
  {
    TasksEmailCollection?.Refresh();
    TasksEmailCollectionCount?.Refresh();
  }
}
