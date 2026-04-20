using NMKApp.Core;
using NMKApp.Models;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Windows.Media;

namespace NMKApp.Data.Entities;

/// <summary>
/// Supabase entity for the NMK_Version table.
/// </summary>
[Table("NMK_Version")]
public class VersionEntity : BaseModel
{
  [PrimaryKey("id", false)]
  [Column("id")]
  public string Id { get; set; } = string.Empty;

  [Column("created_at")]
  public DateTime CreatedAt { get; set; }

  [Column("version")]
  public string Version { get; set; } = string.Empty;

  [Column("data")]
  public string? Data { get; set; }

  public VersionModel ToDomain() => new()
  {
    Id = Id,
    CreateAt = CreatedAt,
    Version = Version,
    Data = Data
  };
}

/// <summary>
/// Supabase entity for the NMK_User table.
/// </summary>
[Table("NMK_User")]
public class UserEntity : BaseModel
{
  [PrimaryKey("id", false)]
  [Column("id")]
  public string Id { get; set; } = string.Empty;

  [Column("index")]
  public int Index { get; set; }

  [Column("created_at")]
  public DateTime CreatedAt { get; set; }

  [Column("update_at")]
  public DateTime UpdateAt { get; set; }

  [Column("name")]
  public string Name { get; set; } = string.Empty;

  [Column("team")]
  public string Team { get; set; } = string.Empty;

  [Column("color")]
  public string? Color { get; set; }

  [Column("email")]
  public string Email { get; set; } = string.Empty;

  [Column("create_by")]
  public string CreateBy { get; set; } = string.Empty;

  [Column("update_by")]
  public string? UpdateBy { get; set; }

  [Column("user_role")]
  public string Role { get; set; } = string.Empty;

  [Column("image")]
  public string? ImageString { get; set; }

  [Column("location")]
  public string? Location { get; set; }

  public UserModel ToDomain() => new()
  {
    Id = Id,
    Index = Index,
    Name = Name,
    Team = Team,
    Color = !string.IsNullOrEmpty(Color) ? (Brush)new BrushConverter().ConvertFromString(Color) : null,
    Email = Email,
    Role = Role,
    CreateBy = CreateBy,
    CreateAt = CreatedAt,
    ImageString = ImageString,
    Location = Location ?? string.Empty,
    RoleEnum = RoleTypeExtensions.Parse(Role)
  };
}

/// <summary>
/// Supabase entity for the NMK_Project table.
/// </summary>
[Table("NMK_Project")]
public class ProjectEntity : BaseModel
{
  [PrimaryKey("id", false)]
  [Column("id")]
  public string Id { get; set; } = string.Empty;

  [Column("index")]
  public int Index { get; set; }

  [Column("created_at")]
  public DateTime CreatedAt { get; set; }

  [Column("update_at")]
  public DateTime UpdateAt { get; set; }

  [Column("name")]
  public string Name { get; set; } = string.Empty;

  [Column("key")]
  public string Key { get; set; } = string.Empty;

  [Column("color")]
  public string? Color { get; set; }

  [Column("create_by")]
  public string CreateBy { get; set; } = string.Empty;

  [Column("update_by")]
  public string? UpdateBy { get; set; }

  [Column("description")]
  public string? Description { get; set; }

  [Column("revit_version")]
  public string? RevitVersion { get; set; }

  [Column("image")]
  public string? ImageString { get; set; }

  public ProjectModel ToDomain() => new()
  {
    Id = Id,
    Index = Index,
    Name = Name,
    Key = Key,
    Color = !string.IsNullOrEmpty(Color) ? (Brush)new BrushConverter().ConvertFromString(Color) : null,
    CreateAt = CreatedAt,
    CreateBy = CreateBy,
    Description = Description ?? string.Empty,
    RevitVersion = RevitVersion ?? string.Empty
  };
}

/// <summary>
/// Supabase entity for the NMK_Task table.
/// </summary>
[Table("NMK_Task")]
public class TaskEntity : BaseModel
{
  [PrimaryKey("id", false)]
  [Column("id")]
  public string Id { get; set; } = string.Empty;

  [Column("index")]
  public int Index { get; set; }

  [Column("index_status")]
  public int IndexStatus { get; set; }

  [Column("created_at")]
  public DateTime CreatedAt { get; set; }

  [Column("update_at")]
  public DateTime UpdateAt { get; set; }

  [Column("name")]
  public string Name { get; set; } = string.Empty;

  [Column("status")]
  public int Status { get; set; }

  [Column("approval")]
  public int Approval { get; set; }

  [Column("project_id")]
  public string ProjectId { get; set; } = string.Empty;

  [Column("user_id")]
  public string UserId { get; set; } = string.Empty;

  [Column("date_start")]
  public DateTime DateStart { get; set; }

  [Column("date_end")]
  public DateTime DateEnd { get; set; }

  [Column("create_by")]
  public string CreateBy { get; set; } = string.Empty;

  [Column("update_by")]
  public string? UpdateBy { get; set; }

  [Column("folder")]
  public string? Folder { get; set; }

  public TaskModel ToDomain() => new()
  {
    Id = Id,
    Index = Index,
    IndexStatus = IndexStatus,
    Name = Name,
    OnlyName = Name.Contains(" : ") ? Name.Split(" : ").Last() : Name,
    Status = Status,
    Approval = Approval,
    ProjectId = ProjectId,
    UserId = UserId,
    DateStart = DateStart,
    DateEnd = DateEnd,
    CreateAt = CreatedAt,
    Folder = Folder ?? string.Empty
  };
}

/// <summary>
/// Supabase entity for the NMK_Notify table.
/// </summary>
[Table("NMK_Notify")]
public class NotifyEntity : BaseModel
{
  [PrimaryKey("id", false)]
  [Column("id")]
  public string Id { get; set; } = string.Empty;

  [Column("created_at")]
  public DateTime CreatedAt { get; set; }

  [Column("update_at")]
  public DateTime UpdateAt { get; set; }

  [Column("task_id")]
  public string TaskId { get; set; } = string.Empty;

  [Column("title")]
  public string Title { get; set; } = string.Empty;

  [Column("send_to")]
  public string SendTo { get; set; } = string.Empty;

  [Column("create_by")]
  public string CreateBy { get; set; } = string.Empty;

  [Column("is_read")]
  public bool IsRead { get; set; }

  [Column("type")]
  public int Type { get; set; }

  [Column("status")]
  public int Status { get; set; }

  public NotifyModel ToDomain() => new()
  {
    Id = Id,
    TaskId = TaskId,
    Title = Title,
    SendTo = SendTo,
    CreateBy = CreateBy,
    CreateAt = CreatedAt,
    UpdateAt = UpdateAt,
    Type = Type,
    Status = Status,
    IsRead = IsRead
  };
}

/// <summary>
/// Supabase entity for the NMK_Leave table.
/// </summary>
[Table("NMK_Leave")]
public class LeaveEntity : BaseModel
{
  [PrimaryKey("id", false)]
  [Column("id")]
  public string Id { get; set; } = string.Empty;

  [Column("created_at")]
  public DateTime CreatedAt { get; set; }

  [Column("update_at")]
  public DateTime UpdateAt { get; set; }

  [Column("create_by")]
  public string CreateBy { get; set; } = string.Empty;

  [Column("send_to")]
  public string SendTo { get; set; } = string.Empty;

  [Column("cc")]
  public string? CC { get; set; }

  [Column("cc_to")]
  public string? CCTo { get; set; }

  [Column("approval")]
  public int Approval { get; set; } = 2;

  [Column("type")]
  public string Type { get; set; } = string.Empty;

  [Column("reason")]
  public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Supabase entity for task backup data.
/// </summary>
[Table("NMK_Task_Backup")]
public class TaskBackupEntity : BaseModel
{
  [PrimaryKey("id", false)]
  [Column("id")]
  public string Id { get; set; } = string.Empty;

  [Column("created_at")]
  public DateTime CreatedAt { get; set; }

  [Column("create_by")]
  public string? CreateBy { get; set; }

  [Column("data")]
  public string? Data { get; set; }
}
