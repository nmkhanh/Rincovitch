using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;

namespace NMKApp.Models;

/// <summary>
/// Notification domain model.
/// </summary>
public partial class NotifyModel : ObservableObject
{
  [ObservableProperty] private string _id = string.Empty;
  [ObservableProperty] private string _taskId = string.Empty;
  [ObservableProperty] private string _title = string.Empty;
  [ObservableProperty] private string _sendTo = string.Empty;
  [ObservableProperty] private string _createBy = string.Empty;
  [ObservableProperty] private DateTime _createAt;
  [ObservableProperty] private DateTime _updateAt;
  [ObservableProperty] private int _type;
  [ObservableProperty] private int _status;
  [ObservableProperty] private bool _isRead;
  [ObservableProperty] private FontWeight _fontWeight = FontWeights.Bold;

  partial void OnIsReadChanged(bool value)
  {
    FontWeight = value ? FontWeights.Normal : FontWeights.Bold;
  }
}

public class NotifyCollection
{
  public ObservableCollection<NotifyModel> Items { get; set; } = [];
}
