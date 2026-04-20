using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace NMKApp.Services;

/// <summary>
/// Navigation service implementation using DI to resolve ViewModels.
/// </summary>
public class NavigationService(IServiceProvider serviceProvider) : INavigationService
{
  private ObservableObject? _currentViewModel;

  public ObservableObject? CurrentViewModel
  {
    get => _currentViewModel;
    private set
    {
      _currentViewModel = value;
      OnNavigated?.Invoke();
    }
  }

  public event Action? OnNavigated;

  public void NavigateTo<TViewModel>() where TViewModel : ObservableObject
  {
    var viewModel = serviceProvider.GetRequiredService<TViewModel>();
    CurrentViewModel = viewModel;
  }

  public void NavigateTo(ObservableObject viewModel)
  {
    CurrentViewModel = viewModel;
  }
}
