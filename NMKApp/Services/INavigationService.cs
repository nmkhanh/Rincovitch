using CommunityToolkit.Mvvm.ComponentModel;

namespace NMKApp.Services;

/// <summary>
/// Navigation service interface.
/// Manages page switching in the main window.
/// </summary>
public interface INavigationService
{
  ObservableObject? CurrentViewModel { get; }
  event Action? OnNavigated;
  void NavigateTo<TViewModel>() where TViewModel : ObservableObject;
  void NavigateTo(ObservableObject viewModel);
}
