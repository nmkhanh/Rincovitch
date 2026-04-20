using System.Diagnostics;

namespace NMKApp.Services;

/// <summary>
/// Authentication service implementation.
/// TODO: Migrate detailed Outlook COM / Graph logic from RincovitchApp/API/Mail/F_Mail.cs
/// </summary>
public class AuthService : IAuthService
{
  public async Task<(string Email, string Avatar, bool IsLoggedIn)> AuthenticateAsync()
  {
    string email = string.Empty;
    string avatar = string.Empty;
    bool isLoggedIn = false;

    try
    {
      // Step 1: Try Windows Registry
      email = GetEmailFromRegistry();
      if (!string.IsNullOrEmpty(email))
      {
        isLoggedIn = true;
        Debug.WriteLine($"[Auth] Registry OK: {email}");
        return (email, avatar, isLoggedIn);
      }

      // Step 2: Try Classic Outlook COM
      // TODO: Migrate IsOutlookLoggedIn from F_Mail

      // Step 3: Try Microsoft Graph / WAM
      // TODO: Migrate GetUserEmail_NewOutlook from F_Mail
      await Task.CompletedTask;
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"[Auth] Error: {ex.Message}");
    }

    return (email, avatar, isLoggedIn);
  }

  private static string GetEmailFromRegistry()
  {
    // TODO: Migrate from F_Mail.GetEmailFromRegistry()
    return string.Empty;
  }
}
