using NMKApp.Models;

namespace NMKApp.Services;

/// <summary>
/// Backup service interface.
/// Extracted from MainWindow.xaml.cs BackupDataAsync.
/// </summary>
public interface IBackupService
{
  Task BackupAsync(UserModel currentUser, TaskCollection tasks, TaskCollection tasksTemporary,
    UserCollection users, ProjectCollection projects, VersionModel? versionCurrent,
    VersionModel? versionLast, bool isVersionUpdate);
}
