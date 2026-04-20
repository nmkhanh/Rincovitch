namespace NMKApp.Services;

/// <summary>
/// Authentication service interface.
/// Extracted login logic from MainWindow.xaml.cs.
/// </summary>
public interface IAuthService
{
  /// <summary>
  /// Attempts to authenticate the user via multiple methods:
  /// 1. Windows Registry (offline)
  /// 2. Classic Outlook COM
  /// 3. Microsoft Graph API (WAM)
  /// </summary>
  /// <returns>Tuple of (email, avatar) or empty if failed.</returns>
  Task<(string Email, string Avatar, bool IsLoggedIn)> AuthenticateAsync();
}
