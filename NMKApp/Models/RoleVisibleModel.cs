using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace NMKApp.Models;

/// <summary>
/// Role-based visibility flags for UI elements.
/// </summary>
public partial class RoleVisibleModel : ObservableObject
{
  [ObservableProperty] private Visibility _visibleOnlyAdminApp = Visibility.Collapsed;
  [ObservableProperty] private Visibility _visibleUser = Visibility.Collapsed;
  [ObservableProperty] private Visibility _visibleAdmin = Visibility.Collapsed;
  [ObservableProperty] private Visibility _visibleLeader = Visibility.Collapsed;
  [ObservableProperty] private Visibility _visibleAdminApp = Visibility.Collapsed;
  [ObservableProperty] private Visibility _visibleMiddle = Visibility.Collapsed;
}
