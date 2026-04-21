namespace NMKApp.Core;

/// <summary>
/// Application-wide constants. Replaces scattered hardcoded strings.
/// </summary>
public static class AppConstants
{
  // Supabase
  public const string SupabaseUrl = "https://ondwkhoelyfpzugwyqnd.supabase.co";
  public const string SupabaseKey = "sb_publishable_lkCPpfLoeGVUIgIm0nFJkQ_ltk_pUeY";

  // Microsoft Graph / MSAL (Azure AD App Registration)
  public const string MsalClientId = "30a2d671-3ac4-4b85-b799-fdef947dae3c";
  public const string MsalTenantId = "common";
  public static readonly string[] MsalScopes = ["User.Read", "Mail.Send"];

  // Toast
  public const string ToastAppId = "NMKApp.TaskManager.WPF";
  public const string ToastShortcutName = "NMK Task Manager";

  // Paths
  public static string BackupFolder => Environment.ExpandEnvironmentVariables(
    @"%UserProfile%\AppData\Local\NMKApp\Temp");

  // Roles
  public static class Roles
  {
    public const string AdminApp = "adminapp";
    public const string Admin = "admin";
    public const string Leader = "leader";
    public const string User = "user";
  }

  // Work schedule
  public static readonly TimeSpan MorningStart = new(8, 30, 0);
  public static readonly TimeSpan MorningEnd = new(12, 30, 0);
  public static readonly TimeSpan AfternoonStart = new(13, 30, 0);
  public static readonly TimeSpan AfternoonEnd = new(17, 30, 0);

  // OneDrive path patterns (from MVVMSourceProject)
  public static string OneDrivePath => System.IO.Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
    "OneDrive - Rincovitch", "Nhan Nguyen's files - Rincovitch");

  public static readonly List<string> ProjectFolders =
  [
    "_COLUMN", "_ELEVATION WALL", "_FULL SET", "_GA PLAN",
    "_LOADING PLAN", "_SECTION DETAILS", "_SITE RETENTION"
  ];
}
