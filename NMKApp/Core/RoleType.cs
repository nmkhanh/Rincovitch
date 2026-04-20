namespace NMKApp.Core;

/// <summary>
/// Role type enumeration with parsing utility.
/// </summary>
public enum RoleType
{
  AdminApp,
  Admin,
  Leader,
  User
}

public static class RoleTypeExtensions
{
  public static RoleType Parse(string? roleString)
  {
    return (roleString?.ToLowerInvariant()) switch
    {
      AppConstants.Roles.AdminApp => RoleType.AdminApp,
      AppConstants.Roles.Admin => RoleType.Admin,
      AppConstants.Roles.Leader => RoleType.Leader,
      AppConstants.Roles.User => RoleType.User,
      _ => RoleType.User,
    };
  }
}
