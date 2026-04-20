namespace NMKApp.Core;

/// <summary>
/// Application-wide constants. Replaces scattered hardcoded strings.
/// </summary>
public static class AppConstants
{
  // Supabase
  public const string SupabaseUrl = ""; // TODO: Move to appsettings or environment variable
  public const string SupabaseKey = ""; // TODO: Move to appsettings or environment variable

  // Microsoft Graph / MSAL
  public const string MsalClientId = ""; // TODO: Configure
  public const string MsalTenantId = ""; // TODO: Configure
  public static readonly string[] MsalScopes = ["Mail.Send", "Mail.ReadWrite", "User.Read"];

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
