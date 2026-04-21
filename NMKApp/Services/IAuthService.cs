namespace NMKApp.Services;

/// <summary>
/// Authentication service interface.
/// Extracted login logic from MainWindow.xaml.cs.
/// </summary>
public interface IAuthService
{
  Task<(string Email, string Avatar, bool IsLoggedIn)> AuthenticateAsync();
}
