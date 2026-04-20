using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using NMKApp.Core;
using NMKApp.Data.Entities;
using NMKApp.Models;

namespace NMKApp.Services;

/// <summary>
/// Backup service implementation.
/// </summary>
public class BackupService(ISupabaseService supabaseService) : IBackupService
{
  public async Task BackupAsync(UserModel currentUser, TaskCollection tasks,
    TaskCollection tasksTemporary, UserCollection users, ProjectCollection projects,
    VersionModel? versionCurrent, VersionModel? versionLast, bool isVersionUpdate)
  {
    try
    {
      // Save version settings
      // TODO: Migrate Properties.Settings logic

      string path = AppConstants.BackupFolder;
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);

      string file = Path.Combine(path, Guid.NewGuid().ToString() + ".json");
      var data = new
      {
        Tasks = tasks.Items.ToList(),
        TasksTemporary = tasksTemporary.Items.ToList(),
        Users = users.Items.ToList(),
        Projects = projects.Items.ToList(),
      };

      var backup = new
      {
        CreateAt = DateTime.Now,
        CreateBy = currentUser.Email,
        Id = Guid.NewGuid().ToString(),
        Data = data
      };

      File.WriteAllText(file, JsonConvert.SerializeObject(backup));

      await supabaseService.InsertBackupAsync(new TaskBackupEntity
      {
        Id = backup.Id,
        CreatedAt = backup.CreateAt,
        CreateBy = backup.CreateBy,
        Data = JsonConvert.SerializeObject(data),
      });
    }
    catch (Exception ex)
    {
      Debug.WriteLine("Backup Error: " + ex.Message);
    }
  }
}
