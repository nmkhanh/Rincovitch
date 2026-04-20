using CommunityToolkit.Mvvm.ComponentModel;
using NMKApp.Core;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace NMKApp.Models;

/// <summary>
/// User domain model. Observable for WPF binding.
/// </summary>
public partial class UserModel : ObservableObject
{
  [ObservableProperty] private string _id = string.Empty;
  [ObservableProperty] private int _index;
  [ObservableProperty] private string _name = string.Empty;
  [ObservableProperty] private string _team = string.Empty;
  [ObservableProperty] private string _email = string.Empty;
  [ObservableProperty] private string _role = string.Empty;
  [ObservableProperty] private RoleType _roleEnum = RoleType.User;
  [ObservableProperty] private Brush? _color;
  [ObservableProperty] private Brush? _colorStatus;
  [ObservableProperty] private string _createBy = string.Empty;
  [ObservableProperty] private DateTime _createAt;
  [ObservableProperty] private string? _imageString;
  [ObservableProperty] private string _location = string.Empty;
  [ObservableProperty] private bool _isProgress;
  [ObservableProperty] private bool _isVisible = true;

  public ObservableCollection<string> ProjectIds { get; set; } = [];
}

/// <summary>
/// Container for user collections with filtering.
/// </summary>
public class UserCollection
{
  public ObservableCollection<UserModel> Items { get; set; } = [];

  public void Filter(string search)
  {
    // Filtering handled by ListCollectionView in ViewModel
  }
}
