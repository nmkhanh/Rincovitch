using NMKApp.Core;
using NMKApp.Data.Entities;
using NMKApp.Models;
using Supabase;

namespace NMKApp.Services;

/// <summary>
/// Supabase service implementation.
/// TODO: Migrate detailed logic from RincovitchApp/Models/NMK_Supabase.cs
/// </summary>
public class SupabaseService : ISupabaseService
{
  private Client? _client;

  public Client Client => _client ?? throw new InvalidOperationException("Supabase not initialized. Call InitializeAsync first.");

  public async Task InitializeAsync()
  {
    var options = new SupabaseOptions { AutoRefreshToken = true };
    _client = new Client(AppConstants.SupabaseUrl, AppConstants.SupabaseKey, options);
    await _client.InitializeAsync();
  }

  #region Version
  public async Task<Result<List<VersionModel>>> GetVersionsAsync()
  {
    try
    {
      var response = await Client.From<VersionEntity>().Get();
      var items = response.Models.Select(x => x.ToDomain()).ToList();
      return Result<List<VersionModel>>.Ok(items);
    }
    catch (Exception ex)
    {
      return Result<List<VersionModel>>.Fail(ex.Message);
    }
  }
  #endregion

  #region Users
  public async Task<Result<List<UserModel>>> GetUsersAsync()
  {
    try
    {
      var response = await Client.From<UserEntity>().Get();
      var items = response.Models.Select(x => x.ToDomain()).ToList();
      return Result<List<UserModel>>.Ok(items);
    }
    catch (Exception ex)
    {
      return Result<List<UserModel>>.Fail(ex.Message);
    }
  }

  public async Task<Result<UserModel>> InsertUserAsync(UserEntity entity)
  {
    try
    {
      var response = await Client.From<UserEntity>().Insert(entity);
      return Result<UserModel>.Ok(response.Models.First().ToDomain());
    }
    catch (Exception ex)
    {
      return Result<UserModel>.Fail(ex.Message);
    }
  }

  public async Task<Result<UserModel>> UpdateUserAsync(UserEntity entity)
  {
    try
    {
      var response = await Client.From<UserEntity>().Update(entity);
      return Result<UserModel>.Ok(response.Models.First().ToDomain());
    }
    catch (Exception ex)
    {
      return Result<UserModel>.Fail(ex.Message);
    }
  }

  public async Task DeleteUserAsync(string id)
  {
    await Client.From<UserEntity>().Where(x => x.Id == id).Delete();
  }
  #endregion

  #region Projects
  public async Task<Result<List<ProjectModel>>> GetProjectsAsync()
  {
    try
    {
      var response = await Client.From<ProjectEntity>().Get();
      var items = response.Models.Select(x => x.ToDomain()).ToList();
      return Result<List<ProjectModel>>.Ok(items);
    }
    catch (Exception ex)
    {
      return Result<List<ProjectModel>>.Fail(ex.Message);
    }
  }

  public async Task<Result<ProjectModel>> InsertProjectAsync(ProjectEntity entity)
  {
    try
    {
      var response = await Client.From<ProjectEntity>().Insert(entity);
      return Result<ProjectModel>.Ok(response.Models.First().ToDomain());
    }
    catch (Exception ex)
    {
      return Result<ProjectModel>.Fail(ex.Message);
    }
  }

  public async Task<Result<ProjectModel>> UpdateProjectAsync(ProjectEntity entity)
  {
    try
    {
      var response = await Client.From<ProjectEntity>().Update(entity);
      return Result<ProjectModel>.Ok(response.Models.First().ToDomain());
    }
    catch (Exception ex)
    {
      return Result<ProjectModel>.Fail(ex.Message);
    }
  }

  public async Task DeleteProjectAsync(string id)
  {
    await Client.From<ProjectEntity>().Where(x => x.Id == id).Delete();
  }
  #endregion

  #region Tasks
  public async Task<Result<List<TaskModel>>> GetTasksAsync()
  {
    try
    {
      var response = await Client.From<TaskEntity>().Get();
      var items = response.Models.Select(x => x.ToDomain()).ToList();
      return Result<List<TaskModel>>.Ok(items);
    }
    catch (Exception ex)
    {
      return Result<List<TaskModel>>.Fail(ex.Message);
    }
  }

  public async Task<Result<List<TaskModel>>> GetTasksByIdAsync(string id)
  {
    try
    {
      var response = await Client.From<TaskEntity>().Where(x => x.Id == id).Get();
      var items = response.Models.Select(x => x.ToDomain()).ToList();
      return Result<List<TaskModel>>.Ok(items);
    }
    catch (Exception ex)
    {
      return Result<List<TaskModel>>.Fail(ex.Message);
    }
  }

  public async Task<Result<TaskModel>> InsertTaskAsync(TaskEntity entity)
  {
    try
    {
      var response = await Client.From<TaskEntity>().Insert(entity);
      return Result<TaskModel>.Ok(response.Models.First().ToDomain());
    }
    catch (Exception ex)
    {
      return Result<TaskModel>.Fail(ex.Message);
    }
  }

  public async Task<Result<TaskModel>> UpdateTaskAsync(TaskEntity entity)
  {
    try
    {
      var response = await Client.From<TaskEntity>().Update(entity);
      return Result<TaskModel>.Ok(response.Models.First().ToDomain());
    }
    catch (Exception ex)
    {
      return Result<TaskModel>.Fail(ex.Message);
    }
  }

  public async Task DeleteTaskAsync(string id)
  {
    await Client.From<TaskEntity>().Where(x => x.Id == id).Delete();
  }
  #endregion

  #region Notifications
  public async Task<Result<List<NotifyModel>>> GetNotifysAsync(string email)
  {
    try
    {
      var response = await Client.From<NotifyEntity>().Where(x => x.SendTo == email).Get();
      var items = response.Models.Select(x => x.ToDomain()).ToList();
      return Result<List<NotifyModel>>.Ok(items);
    }
    catch (Exception ex)
    {
      return Result<List<NotifyModel>>.Fail(ex.Message);
    }
  }

  public async Task<Result<NotifyModel>> InsertNotifyAsync(NotifyEntity entity)
  {
    try
    {
      var response = await Client.From<NotifyEntity>().Insert(entity);
      return Result<NotifyModel>.Ok(response.Models.First().ToDomain());
    }
    catch (Exception ex)
    {
      return Result<NotifyModel>.Fail(ex.Message);
    }
  }

  public async Task<Result<NotifyModel>> UpdateNotifyAsync(NotifyEntity entity)
  {
    try
    {
      var response = await Client.From<NotifyEntity>().Update(entity);
      return Result<NotifyModel>.Ok(response.Models.First().ToDomain());
    }
    catch (Exception ex)
    {
      return Result<NotifyModel>.Fail(ex.Message);
    }
  }
  #endregion

  #region Leaves
  public async Task<Result<List<LeaveModel>>> GetLeavesAsync()
  {
    try
    {
      var response = await Client.From<LeaveEntity>().Get();
      var items = response.Models.Select(x => new LeaveModel
      {
        Id = x.Id,
        CreateAt = x.CreatedAt,
        UpdateAt = x.UpdateAt,
        CreateBy = x.CreateBy,
        SendTo = x.SendTo,
        CC = x.CC ?? string.Empty,
        CCTo = x.CCTo ?? string.Empty,
        Approval = x.Approval,
        Type = x.Type,
        Reason = x.Reason
      }).ToList();
      return Result<List<LeaveModel>>.Ok(items);
    }
    catch (Exception ex)
    {
      return Result<List<LeaveModel>>.Fail(ex.Message);
    }
  }

  public async Task<Result<LeaveModel>> InsertLeaveAsync(LeaveEntity entity)
  {
    try
    {
      var response = await Client.From<LeaveEntity>().Insert(entity);
      var e = response.Models.First();
      return Result<LeaveModel>.Ok(new LeaveModel { Id = e.Id, CreateAt = e.CreatedAt });
    }
    catch (Exception ex)
    {
      return Result<LeaveModel>.Fail(ex.Message);
    }
  }

  public async Task<Result<LeaveModel>> UpdateLeaveAsync(LeaveEntity entity)
  {
    try
    {
      var response = await Client.From<LeaveEntity>().Update(entity);
      var e = response.Models.First();
      return Result<LeaveModel>.Ok(new LeaveModel { Id = e.Id });
    }
    catch (Exception ex)
    {
      return Result<LeaveModel>.Fail(ex.Message);
    }
  }
  #endregion

  #region Backup
  public async Task InsertBackupAsync(TaskBackupEntity entity)
  {
    await Client.From<TaskBackupEntity>().Insert(entity);
  }
  #endregion
}
