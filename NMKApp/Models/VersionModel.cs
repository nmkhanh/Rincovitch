using CommunityToolkit.Mvvm.ComponentModel;

namespace NMKApp.Models;

/// <summary>
/// Version model for app update tracking.
/// </summary>
public partial class VersionModel : ObservableObject
{
  [ObservableProperty] private string _id = string.Empty;
  [ObservableProperty] private DateTime _createAt;
  [ObservableProperty] private string _version = string.Empty;
  [ObservableProperty] private string? _data;
}
