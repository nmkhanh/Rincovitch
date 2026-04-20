using NMKApp.Core;
using NMKApp.Data.Entities;
using NMKApp.Models;
using Supabase;

namespace NMKApp.Services;

/// <summary>
/// Interface for Supabase data access operations.
/// Extracted from the monolithic NMK_Supabase static class.
/// </summary>
public interface ISupabaseService
{
  Client Client { get; }
  Task InitializeAsync();

  // Version
  Task<Result<List<VersionModel>>> GetVersionsAsync();

  // Users
  Task<Result<List<UserModel>>> GetUsersAsync();
  Task<Result<UserModel>> InsertUserAsync(UserEntity entity);
  Task<Result<UserModel>> UpdateUserAsync(UserEntity entity);
  Task DeleteUserAsync(string id);

  // Projects
  Task<Result<List<ProjectModel>>> GetProjectsAsync();
  Task<Result<ProjectModel>> InsertProjectAsync(ProjectEntity entity);
  Task<Result<ProjectModel>> UpdateProjectAsync(ProjectEntity entity);
  Task DeleteProjectAsync(string id);

  // Tasks
  Task<Result<List<TaskModel>>> GetTasksAsync();
  Task<Result<List<TaskModel>>> GetTasksByIdAsync(string id);
  Task<Result<TaskModel>> InsertTaskAsync(TaskEntity entity);
  Task<Result<TaskModel>> UpdateTaskAsync(TaskEntity entity);
  Task DeleteTaskAsync(string id);

  // Notifications
  Task<Result<List<NotifyModel>>> GetNotifysAsync(string email);
  Task<Result<NotifyModel>> InsertNotifyAsync(NotifyEntity entity);
  Task<Result<NotifyModel>> UpdateNotifyAsync(NotifyEntity entity);

  // Leaves
  Task<Result<List<LeaveModel>>> GetLeavesAsync();
  Task<Result<LeaveModel>> InsertLeaveAsync(LeaveEntity entity);
  Task<Result<LeaveModel>> UpdateLeaveAsync(LeaveEntity entity);

  // Backup
  Task InsertBackupAsync(TaskBackupEntity entity);
}
