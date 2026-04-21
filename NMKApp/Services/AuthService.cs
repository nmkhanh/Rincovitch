using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Identity.Client;
using NMKApp.Core;

namespace NMKApp.Services;

/// <summary>
/// 4-step auth: Registry → COM Outlook → Graph/WAM silent → Graph/WAM interactive.
/// Adapted from RincovitchApp F_Mail.cs + F_MailGraph.cs.
/// </summary>
public class AuthService : IAuthService
{
  private static IPublicClientApplication? _msalApp;
  private static readonly string[] _scopes = AppConstants.MsalScopes;

  private static IPublicClientApplication MsalApp
  {
    get
    {
      if (_msalApp == null)
      {
        _msalApp = PublicClientApplicationBuilder
          .Create(AppConstants.MsalClientId)
          .WithTenantId(AppConstants.MsalTenantId)
          .WithRedirectUri("http://localhost")
          .Build();

        // Persist token cache to disk
        try
        {
          var cacheDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "NMKApp", "MsalCache");
          Directory.CreateDirectory(cacheDir);
          var cacheFile = Path.Combine(cacheDir, "nmkapp_msal.json");
          _msalApp.UserTokenCache.SetBeforeAccess(args =>
          {
            if (File.Exists(cacheFile))
              args.TokenCache.DeserializeMsalV3(File.ReadAllBytes(cacheFile));
          });
          _msalApp.UserTokenCache.SetAfterAccess(args =>
          {
            if (args.HasStateChanged)
              File.WriteAllBytes(cacheFile, args.TokenCache.SerializeMsalV3());
          });
        }
        catch (Exception ex)
        {
          Debug.WriteLine($"[Auth] Token cache setup error: {ex.Message}");
        }
      }
      return _msalApp;
    }
  }

  public async Task<(string Email, string Avatar, bool IsLoggedIn)> AuthenticateAsync()
  {
    // Step 1: Windows Registry (fastest — no process needed)
    var regEmail = GetEmailFromRegistry();
    if (!string.IsNullOrEmpty(regEmail))
    {
      Debug.WriteLine($"[Auth] Registry: {regEmail}");
      return (regEmail, string.Empty, true);
    }

    // Step 2: Classic Outlook COM (if Outlook is running)
    var comEmail = GetEmailFromCOM();
    if (!string.IsNullOrEmpty(comEmail))
    {
      Debug.WriteLine($"[Auth] COM Outlook: {comEmail}");
      return (comEmail, string.Empty, true);
    }

    // Step 3: Graph/WAM silent
    try
    {
      var accounts = await MsalApp.GetAccountsAsync();
      var account = accounts.FirstOrDefault();
      if (account != null)
      {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        var result = await MsalApp
          .AcquireTokenSilent(_scopes, account)
          .ExecuteAsync(cts.Token);

        if (!string.IsNullOrEmpty(result?.AccessToken))
        {
          var email = result.Account?.Username ?? await FetchEmailFromGraphAsync(result.AccessToken);
          if (!string.IsNullOrEmpty(email))
          {
            Debug.WriteLine($"[Auth] MSAL silent: {email}");
            return (email, string.Empty, true);
          }
        }
      }
    }
    catch (MsalException ex) when (
      ex.ErrorCode is "user_null" or "no_account_in_silent_token_cache")
    {
      Debug.WriteLine($"[Auth] MSAL silent miss: {ex.ErrorCode}");
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"[Auth] MSAL silent error: {ex.Message}");
    }

    // Step 4: Graph/WAM interactive (Windows account picker — no password prompt)
    try
    {
      var hwnd = GetMainWindowHandle();
      using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
      var result = await MsalApp
        .AcquireTokenInteractive(_scopes)
        .WithParentActivityOrWindow(hwnd)
        .ExecuteAsync(cts.Token);

      if (!string.IsNullOrEmpty(result?.AccessToken))
      {
        var email = result.Account?.Username ?? await FetchEmailFromGraphAsync(result.AccessToken);
        if (!string.IsNullOrEmpty(email))
        {
          Debug.WriteLine($"[Auth] WAM interactive: {email}");
          return (email, string.Empty, true);
        }
      }
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"[Auth] WAM interactive error: {ex.Message}");
    }

    return (string.Empty, string.Empty, false);
  }

  // ─── Registry ─────────────────────────────────────────────────────────────
  private static string GetEmailFromRegistry()
  {
    try
    {
      using var key = Microsoft.Win32.Registry.CurrentUser
        .OpenSubKey(@"Software\Microsoft\Office\16.0\Outlook\Profiles");
      if (key == null) return string.Empty;

      var defaultProfile = key.GetValue("DefaultProfile") as string;
      if (string.IsNullOrEmpty(defaultProfile)) return string.Empty;

      // Walk profile MAPI accounts
      using var profileKey = key.OpenSubKey(
        $@"{defaultProfile}\9375CFF0413111d3B88A00104B2A6676");
      if (profileKey == null) return string.Empty;

      foreach (var subKeyName in profileKey.GetSubKeyNames())
      {
        using var subKey = profileKey.OpenSubKey(subKeyName);
        if (subKey == null) continue;

        var emailBin = subKey.GetValue("New Signature") as byte[];
        var emailStr = subKey.GetValue("Account Name") as string;
        if (!string.IsNullOrEmpty(emailStr) && emailStr.Contains('@'))
          return emailStr.Trim().ToLowerInvariant();

        // Email stored as binary
        var displayBin = subKey.GetValue("Display Name") as byte[];
        if (displayBin != null)
        {
          var addr = Encoding.Unicode.GetString(displayBin).Trim('\0');
          if (addr.Contains('@')) return addr.Trim().ToLowerInvariant();
        }
      }
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"[Auth] Registry error: {ex.Message}");
    }
    return string.Empty;
  }

  // ─── COM Outlook (only if already running — never launches) ──────────────
  [DllImport("oleaut32.dll")]
  private static extern int GetActiveObject(
    [MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
    IntPtr pvReserved, [MarshalAs(UnmanagedType.IUnknown)] out object ppunk);

  private static string GetEmailFromCOM()
  {
    object? outlookObj = null;
    try
    {
      var hr = GetActiveObject(
        new Guid("0006F03A-0000-0000-C000-000000000046"),
        IntPtr.Zero, out outlookObj);
      if (hr != 0 || outlookObj == null) return string.Empty;

      var type = outlookObj.GetType();
      var ns = type.InvokeMember("GetNamespace",
        System.Reflection.BindingFlags.InvokeMethod, null,
        outlookObj, new object[] { "MAPI" });
      if (ns == null) return string.Empty;

      var nsType = ns.GetType();
      var accounts = nsType.InvokeMember("Accounts",
        System.Reflection.BindingFlags.GetProperty, null, ns, null);
      if (accounts == null) return string.Empty;

      var accType = accounts.GetType();
      var count = (int)accType.InvokeMember("Count",
        System.Reflection.BindingFlags.GetProperty, null, accounts, null);

      for (int i = 1; i <= count; i++)
      {
        var account = accType.InvokeMember("Item",
          System.Reflection.BindingFlags.InvokeMethod, null,
          accounts, new object[] { i });
        if (account == null) continue;

        var smtpAddr = account.GetType().InvokeMember("SmtpAddress",
          System.Reflection.BindingFlags.GetProperty, null, account, null) as string;
        if (!string.IsNullOrEmpty(smtpAddr) && smtpAddr.Contains('@'))
          return smtpAddr.Trim().ToLowerInvariant();
      }
    }
    catch (COMException)
    {
      // Outlook not running — expected
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"[Auth] COM error: {ex.Message}");
    }
    finally
    {
      if (outlookObj != null) Marshal.ReleaseComObject(outlookObj);
    }
    return string.Empty;
  }

  // ─── Graph helpers ────────────────────────────────────────────────────────
  private static async Task<string> FetchEmailFromGraphAsync(string accessToken)
  {
    try
    {
      using var http = new System.Net.Http.HttpClient();
      http.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
      var json = await http.GetStringAsync("https://graph.microsoft.com/v1.0/me?$select=mail,userPrincipalName");
      // naive parse — no Newtonsoft dependency needed
      var start = json.IndexOf("\"mail\":\"");
      if (start >= 0)
      {
        start += 8;
        var end = json.IndexOf('"', start);
        var mail = json[start..end];
        if (!string.IsNullOrEmpty(mail)) return mail.ToLowerInvariant();
      }
      start = json.IndexOf("\"userPrincipalName\":\"");
      if (start >= 0)
      {
        start += 21;
        var end = json.IndexOf('"', start);
        return json[start..end].ToLowerInvariant();
      }
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"[Auth] Graph email fetch error: {ex.Message}");
    }
    return string.Empty;
  }

  private static IntPtr GetMainWindowHandle()
  {
    try
    {
      if (System.Windows.Application.Current?.MainWindow is System.Windows.Window w)
      {
        var helper = new System.Windows.Interop.WindowInteropHelper(w);
        return helper.EnsureHandle();
      }
    }
    catch { }
    return IntPtr.Zero;
  }

  // ─── Graph token for MailService ─────────────────────────────────────────
  public static async Task<string?> GetGraphTokenAsync()
  {
    try
    {
      var accounts = await MsalApp.GetAccountsAsync();
      var account = accounts.FirstOrDefault();
      if (account == null) return null;
      using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
      var result = await MsalApp
        .AcquireTokenSilent(_scopes, account)
        .ExecuteAsync(cts.Token);
      return result?.AccessToken;
    }
    catch
    {
      return null;
    }
  }
}
