using CommunityToolkit.Mvvm.ComponentModel;

namespace NMKApp.Models;

/// <summary>
/// Status filter item.
/// </summary>
public partial class StatusModel : ObservableObject
{
  [ObservableProperty] private string _name = string.Empty;
  [ObservableProperty] private bool _isChecked;
  [ObservableProperty] private string _key = string.Empty;
  [ObservableProperty] private int _state = 100;
}
