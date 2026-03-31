using LiveChartsCore.SkiaSharpView.Painting;
using Newtonsoft.Json;
using RincovitchApp.Models.ModelChilds;
using SkiaSharp;
using Supabase;
using Supabase.Postgrest;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static Supabase.Postgrest.Constants;
using static Supabase.Postgrest.QueryOptions;
using Brush = System.Windows.Media.Brush;

namespace RincovitchApp.Models
{
  [Table("NMK_Version")]
  public class NMK_Supabase_Version : BaseModel
  {
    [PrimaryKey("id", false)]
    [Column("id")]
    public string Id
    {
      get; set;
    }
    [Column("created_at")]
    public DateTime CreatedAt
    {
      get; set;
    }
    [Column("version")]
    public string Version
    {
      get; set;
    }
    [Column("data")]
    public string Data
    {
      get; set;
    }

    public NMK_M_Version Clone()
    {
      return new NMK_M_Version
      {
        Id = this.Id,
        CreateAt = this.CreatedAt,
        Version = this.Version,
        Data = this.Data,
        Supabase_Version = this
      };
    }
  }

  [Table("NMK_User")]
  public class NMK_Supabase_User : BaseModel
  {
    [PrimaryKey("id", false)]
    [Column("id")]
    public string Id
    {
      get; set;
    }
    [Column("index")]
    public int Index
    {
      get; set;
    }
    [Column("created_at")]
    public DateTime CreatedAt
    {
      get; set;
    }
    [Column("update_at")]
    public DateTime UpdateAt
    {
      get; set;
    }
    [Column("name")]
    public string Name
    {
      get; set;
    }
    [Column("team")]
    public string Team
    {
      get; set;
    }
    [Column("color")]
    public string Color
    {
      get; set;
    }
    [Column("email")]
    public string Email
    {
      get; set;
    }
    [Column("create_by")]
    public string CreateBy
    {
      get; set;
    }
    [Column("update_by")]
    public string UpdateBy
    {
      get; set;
    }
    [Column("user_role")]
    public string Role
    {
      get; set;
    }
    [Column("image")]
    public string ImageString
    {
      get; set;
    }
    [Column("location")]
    public string Location
    {
      get; set;
    }

    public NMK_M_User Clone()
    {
      return new NMK_M_User
      {
        Id = this.Id,
        Name = this.Name,
        Team = this.Team,
        Index = this.Index,
        Color = !string.IsNullOrEmpty(this.Color) ? (Brush)new BrushConverter().ConvertFromString(this.Color) : null,
        Email = this.Email,
        Role = this.Role,
        CreateBy = this.CreateBy,
        CreateAt = this.CreatedAt,
        ImageString = this.ImageString,
        Location = this.Location,
        RoleEnum = F_Role.GetRoleTypeFromString(this.Role)
      };
    }
  }

  [Table("NMK_Project")]
  public class NMK_Supabase_Project : BaseModel
  {
    [PrimaryKey("id", false)]
    [Column("id")]
    public string Id
    {
      get; set;
    }
    [Column("index")]
    public int Index
    {
      get; set;
    }
    [Column("created_at")]
    public DateTime CreatedAt
    {
      get; set;
    }
    [Column("update_at")]
    public DateTime UpdateAt
    {
      get; set;
    }
    [Column("name")]
    public string Name
    {
      get; set;
    }
    [Column("key")]
    public string Key
    {
      get; set;
    }
    [Column("color")]
    public string Color
    {
      get; set;
    }
    [Column("create_by")]
    public string CreateBy
    {
      get; set;
    }
    [Column("update_by")]
    public string UpdateBy
    {
      get; set;
    }
    [Column("image")]
    public string ImageString
    {
      get; set;
    }
    [Column("description")]
    public string Description
    {
      get; set;
    }
    [Column("revit_version")]
    public string RevitVersion
    {
      get; set;
    }

    public NMK_M_Project Clone()
    {
      return new NMK_M_Project
      {
        Id = this.Id,
        Name = this.Name,
        Key = this.Key,
        Index = this.Index,
        Color = !string.IsNullOrEmpty(this.Color) ? (Brush)new BrushConverter().ConvertFromString(this.Color) : null,
        ColorString = !string.IsNullOrEmpty(this.Color) ? new SolidColorPaint(SKColor.Parse(this.Color)) : null,
        Description = this.Description,
        CreateBy = this.CreateBy,
        CreateAt = this.CreatedAt,
        ImageString = this.ImageString,
        RevitVersion = this.RevitVersion
      };
    }
  }

  //[Table("NMK_Task_TestCode")]
  [Table("NMK_Task")]
  public class NMK_Supabase_Task : BaseModel
  {
    [PrimaryKey("id", false)]
    [Column("id")]
    public string Id
    {
      get; set;
    }
    [Column("index")]
    public int Index
    {
      get; set;
    }
    [Column("created_at")]
    public DateTime CreatedAt
    {
      get; set;
    }
    [Column("update_at")]
    public DateTime UpdateAt
    {
      get; set;
    }
    [Column("name")]
    public string Name
    {
      get; set;
    }
    [Column("project_id")]
    public string ProjectId
    {
      get; set;
    }
    [Column("user_id")]
    public string UserId
    {
      get; set;
    }
    [Column("date_start")]
    public DateTime DateStart
    {
      get; set;
    }
    [Column("date_end")]
    public DateTime DateEnd
    {
      get; set;
    }
    [Column("detail")]
    public string Detail
    {
      get; set;
    }
    [Column("color")]
    public string Color
    {
      get; set;
    }
    [Column("status")]
    public int Status
    {
      get; set;
    }
    [Column("create_by")]
    public string CreateBy
    {
      get; set;
    }
    [Column("update_by")]
    public string UpdateBy
    {
      get; set;
    }
    [Column("date_complete")]
    public DateTime DateComplete
    {
      get; set;
    }
    [Column("parent_id")]
    public string ParentId
    {
      get; set;
    }
    [Column("date_checked")]
    public DateTime DateChecked
    {
      get; set;
    }
    [Column("area")]
    public double Area
    {
      get; set;
    }
    [Column("is_interrupted")]
    public bool IsInterrupted
    {
      get; set;
    }
    [Column("list_interrupted")]
    public string ListInterrupted
    {
      get; set;
    }
    [Column("date_started")]
    public DateTime DateStarted
    {
      get; set;
    }
    [Column("date_accepted")]
    public DateTime DateAccepted
    {
      get; set;
    }
    [Column("file_attach")]
    public string FileAttach
    {
      get; set;
    }
    [Column("is_onlychecked")]
    public bool IsOnlyChecked
    {
      get; set;
    }

    public NMK_M_Task Clone()
    {
      return new NMK_M_Task
      {
        Id = this.Id,
        Name = this.Name,
        ProjectId = this.ProjectId,
        Index = this.Index,
        Color = !string.IsNullOrEmpty(this.Color) ? (Brush)new BrushConverter().ConvertFromString(this.Color) : null,
        UserId = this.UserId,
        DateStart = this.DateStart,
        HourStart = this.DateStart.Hour,
        MinutesStart = this.DateStart.Minute,
        DateEnd = this.DateEnd,
        HourEnd = this.DateEnd.Hour,
        MinutesEnd = this.DateEnd.Minute,
        Detail = this.Detail,
        Status = this.Status,
        CreateAt = this.CreatedAt,
        CreateBy = this.CreateBy,
        DateComplete = this.DateComplete,
        ParentId = this.ParentId,
        IsOnlyChecked = this.IsOnlyChecked,
        DateChecked = this.DateChecked,
        DateStarted = this.DateStarted,
        DateAccepted = this.DateAccepted,
        Area = this.Area.ToString(),

        IsInterrupted = this.IsInterrupted,
        ZIndex = this.IsInterrupted ? -1 : (this.Status == 0 ? -2 : 0),
        ListInterrupted = IsInterrupted ? this.ListInterrupted.Split(',').ToList() : new List<string>(),

        FileAttachs = !string.IsNullOrEmpty(this.FileAttach) ? JsonConvert.DeserializeObject<ObservableCollection<NMK_M_FileAttach>>(this.FileAttach) : new ObservableCollection<NMK_M_FileAttach>()
      };
    }
  }

  [Table("NMK_Task_Temporary")]
  public class NMK_Supabase_Task_Temporary : NMK_Supabase_Task
  {
  }

  [Table("NMK_Task_Backup")]
  public class NMK_Supabase_Task_Backup : BaseModel
  {
    [PrimaryKey("id", false)]
    [Column("id")]
    public string Id
    {
      get; set;
    }
    [Column("created_at")]
    public DateTime CreatedAt
    {
      get; set;
    }
    [Column("create_by")]
    public string CreateBy
    {
      get; set;
    }
    [Column("data")]
    public string Data
    {
      get; set;
    }

    public NMK_M_Task_Backup Clone()
    {
      return new NMK_M_Task_Backup
      {
        Id = this.Id,
        CreateAt = this.CreatedAt,
        CreateBy = this.CreateBy,
        Data = this.Data
      };
    }
  }

  //[Table("NMK_Notify_TestCode")]
  [Table("NMK_Notify")]
  public class NMK_Supabase_Notify : BaseModel
  {
    [PrimaryKey("id", false)]
    [Column("id")]
    public string Id
    {
      get; set;
    }
    [Column("created_at")]
    public DateTime CreatedAt
    {
      get; set;
    }
    [Column("update_at")]
    public DateTime UpdateAt
    {
      get; set;
    }
    [Column("create_by")]
    public string CreateBy
    {
      get; set;
    }
    [Column("send_to")]
    public string SendTo
    {
      get; set;
    }
    [Column("message")]
    public string Message
    {
      get; set;
    }
    [Column("is_read")]
    public bool IsRead
    {
      get; set;
    }

    [Column("title")]
    public string Title
    {
      get; set;
    }
    [Column("task_id")]
    public string TaskId
    {
      get; set;
    }

    public NMK_M_Notify Clone()
    {
      return new NMK_M_Notify
      {
        Id = this.Id,
        IsRead = this.IsRead,
        CreateAt = this.CreatedAt,
        UpdateAt = this.UpdateAt,
        CreateBy = this.CreateBy,
        SendTo = this.SendTo,
        Message = this.Message,
        TaskId = this.TaskId,
        Title = this.Title
      };
    }
  }

  [Table("NMK_Leave")]
  public class NMK_Supabase_Leave : BaseModel
  {
    [PrimaryKey("id", false)]
    [Column("id")]
    public string Id
    {
      get; set;
    }
    [Column("created_at")]
    public DateTime CreatedAt
    {
      get; set;
    }
    [Column("update_at")]
    public DateTime UpdateAt
    {
      get; set;
    }
    [Column("create_by")]
    public string CreateBy
    {
      get; set;
    }
    [Column("cc_to")]
    public string CCTo
    {
      get; set;
    }
    [Column("send_to")]
    public string SendTo
    {
      get; set;
    }
    [Column("leave_reason")]
    public string LeaveReason
    {
      get; set;
    }
    [Column("leave_list")]
    public string LeaveList
    {
      get; set;
    }
    [Column("approval")]
    public int Approval
    {
      get; set;
    }
    [Column("type")]
    public string LeaveType
    {
      get; set;
    }

    public NMK_M_Leave Clone()
    {
      return new NMK_M_Leave
      {
        Id = this.Id,
        Approval = this.Approval,
        LeaveReason = this.LeaveReason,
        LeaveList = !string.IsNullOrEmpty(this.LeaveList) 
        ? Newtonsoft.Json.JsonConvert.DeserializeObject<ObservableCollection<NMK_M_LeaveDay>>(this.LeaveList) 
        : new ObservableCollection<NMK_M_LeaveDay>(),
        CreateAt = this.CreatedAt,
        UpdateAt = this.UpdateAt,
        CreateBy = this.CreateBy,
        SendTo = this.SendTo,
        CC = this.CCTo,
        LeaveType = this.LeaveType
      };
    }
  }

  public class NMK_Supabase
  {
    static string url = "https://ondwkhoelyfpzugwyqnd.supabase.co";
    static string key = "sb_publishable_lkCPpfLoeGVUIgIm0nFJkQ_ltk_pUeY";

    private static Supabase.Client _client;
    public static Supabase.Client Client => _client;

    public static async Task InitializeAsync()
    {
      var options = new SupabaseOptions { AutoConnectRealtime = true };

      _client = new Supabase.Client(url, key, options);
      await _client.InitializeAsync();
    }

    public static async Task<NMK_M_Return<List<NMK_Supabase_User>>> get_usersAsync()
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_User>().Where(x => x.Location == "VietNam").Get();

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<List<NMK_Supabase_User>>
          {
            Success = true,
            Data = result.Models,
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<List<NMK_Supabase_User>>
          {
            Success = false,
            Data = null,
            Error = $"Get failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<List<NMK_Supabase_User>>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_User>> get_userAsync(string email)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_User>().Where(x => x.Location == "VietNam" && x.Email == email).Get();

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_User>
          {
            Success = true,
            Data = result.Model,
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_User>
          {
            Success = false,
            Data = null,
            Error = $"Get failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_User>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_User>> insert_usersAsync(NMK_Supabase_User user)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_User>().Insert(user, new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_User>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_User>
          {
            Success = false,
            Data = null,
            Error = $"Insert failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_User>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_User>> upsert_usersAsync(NMK_Supabase_User user)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_User>().Upsert(user, new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_User>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_User>
          {
            Success = false,
            Data = null,
            Error = $"Update failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_User>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_User>> updateImage_usersAsync(string id, string image)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_User>().Where(x => x.Id == id).Set(x => x.ImageString, image).Update(new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_User>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_User>
          {
            Success = false,
            Data = null,
            Error = $"Update failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_User>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<string>> delete_usersAsync(string id)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        await supabase.From<NMK_Supabase_User>().Where(x => x.Id == id).Delete();

        return new NMK_M_Return<string>
        {
          Success = true,
          Data = null,
          Error = null
        };
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<string>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }




    public static async Task<NMK_M_Return<List<NMK_Supabase_Project>>> get_ProjectsAsync()
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Project>().Get();
        var cities = result.Models;

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<List<NMK_Supabase_Project>>
          {
            Success = true,
            Data = result.Models,
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<List<NMK_Supabase_Project>>
          {
            Success = false,
            Data = null,
            Error = $"Get failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<List<NMK_Supabase_Project>>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_Project>> insert_ProjectsAsync(NMK_Supabase_Project Project)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Project>().Insert(Project, new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_Project>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_Project>
          {
            Success = false,
            Data = null,
            Error = $"Insert failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_Project>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_Project>> upsert_ProjectsAsync(NMK_Supabase_Project Project)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Project>().Upsert(Project, new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_Project>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_Project>
          {
            Success = false,
            Data = null,
            Error = $"Update failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_Project>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_Project>> update_ProjectsAsync(string id, string color)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Project>().Where(x => x.Id == id).Set(x => x.Color, color).Update(new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_Project>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_Project>
          {
            Success = false,
            Data = null,
            Error = $"Update failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_Project>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_Project>> updateImage_ProjectsAsync(string id, string image)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Project>().Where(x => x.Id == id).Set(x => x.ImageString, image).Update(new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_Project>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_Project>
          {
            Success = false,
            Data = null,
            Error = $"Update failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_Project>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<string>> delete_ProjectsAsync(string id)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        await supabase.From<NMK_Supabase_Project>().Where(x => x.Id == id).Delete();

        return new NMK_M_Return<string>
        {
          Success = true,
          Data = null,
          Error = null
        };
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<string>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }



    public static async Task<NMK_M_Return<List<NMK_Supabase_Task>>> getall_admin_TasksAsync()
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Task>().Get();

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<List<NMK_Supabase_Task>>
          {
            Success = true,
            Data = result.Models,
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<List<NMK_Supabase_Task>>
          {
            Success = false,
            Data = null,
            Error = $"Get failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<List<NMK_Supabase_Task>>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<List<NMK_Supabase_Task>>> getstarted_TasksAsync()
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Task>().Where(x => x.Status > 3).Get();

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<List<NMK_Supabase_Task>>
          {
            Success = true,
            Data = result.Models,
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<List<NMK_Supabase_Task>>
          {
            Success = false,
            Data = null,
            Error = $"Get failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<List<NMK_Supabase_Task>>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<List<NMK_Supabase_Task>>> getall_TasksAsync(string createby, string userId)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Task>().Where(x => x.CreateBy == createby || x.UserId == userId).Get();

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<List<NMK_Supabase_Task>>
          {
            Success = true,
            Data = result.Models,
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<List<NMK_Supabase_Task>>
          {
            Success = false,
            Data = null,
            Error = $"Get failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<List<NMK_Supabase_Task>>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<List<NMK_Supabase_Task>>> get_TasksAsync(string createby)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Task>().Where(x => x.CreateBy == createby).Get();

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<List<NMK_Supabase_Task>>
          {
            Success = true,
            Data = result.Models,
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<List<NMK_Supabase_Task>>
          {
            Success = false,
            Data = null,
            Error = $"Get failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<List<NMK_Supabase_Task>>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<List<NMK_Supabase_Task>>> getassignedto_TasksAsync(string assignedto)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Task>().Where(x => x.UserId == assignedto).Get();

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<List<NMK_Supabase_Task>>
          {
            Success = true,
            Data = result.Models,
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<List<NMK_Supabase_Task>>
          {
            Success = false,
            Data = null,
            Error = $"Get failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<List<NMK_Supabase_Task>>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<List<NMK_Supabase_Task>>> getbyid_TasksAsync(string id)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Task>().Where(x => x.Id == id).Get();

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<List<NMK_Supabase_Task>>
          {
            Success = true,
            Data = result.Models,
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<List<NMK_Supabase_Task>>
          {
            Success = false,
            Data = null,
            Error = $"Get failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<List<NMK_Supabase_Task>>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_Task>> insert_TasksAsync(NMK_Supabase_Task Task)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Task>().Insert(Task, new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_Task>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_Task>
          {
            Success = false,
            Data = null,
            Error = $"Insert failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_Task>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_Task>> upsert_TasksAsync(NMK_Supabase_Task Task)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Task>().Upsert(Task, new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_Task>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_Task>
          {
            Success = false,
            Data = null,
            Error = $"Update failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_Task>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_Task>> update0_TasksAsync(string id, NMK_M_Task item, DateTime date)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase
        .From<NMK_Supabase_Task>()
        .Where(x => x.Id == id)
        .Set(x => x.Status, item.Status)
        .Set(x => x.DateComplete, date)
        .Update(new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_Task>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_Task>
          {
            Success = false,
            Data = null,
            Error = $"Update failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_Task>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_Task>> update3_TasksAsync(string id, NMK_M_Task item)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase
        .From<NMK_Supabase_Task>()
        .Where(x => x.Id == id)
        .Set(x => x.Status, item.Status)
        .Set(x => x.FileAttach, JsonConvert.SerializeObject(item.FileAttachs.Select(x => new NMK_M_FileAttach { Name = Path.GetFileName(x.Name), Id = x.Id })))
        .Update(new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_Task>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_Task>
          {
            Success = false,
            Data = null,
            Error = $"Update failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_Task>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_Task>> update45_TasksAsync(string id, NMK_M_Task item, DateTime date)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase
        .From<NMK_Supabase_Task>()
        .Where(x => x.Id == id)
        .Set(x => x.Status, item.Status)
        .Set(x => x.DateChecked, date)
        .Set(x => x.FileAttach, JsonConvert.SerializeObject(item.FileAttachs.Select(x => new NMK_M_FileAttach { Name = Path.GetFileName(x.Name), Id = x.Id }  )))
        .Update(new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_Task>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_Task>
          {
            Success = false,
            Data = null,
            Error = $"Update failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_Task>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_Task>> update6_TasksAsync(string id, NMK_M_Task item, DateTime date)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase
        .From<NMK_Supabase_Task>()
        .Where(x => x.Id == id)
        .Set(x => x.Status, item.Status)
        .Set(x => x.DateStarted, date)
        .Update(new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_Task>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_Task>
          {
            Success = false,
            Data = null,
            Error = $"Update failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_Task>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_Task>> update7_TasksAsync(string id, NMK_M_Task item, DateTime date)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase
        .From<NMK_Supabase_Task>()
        .Where(x => x.Id == id)
        .Set(x => x.Status, item.Status)
        .Set(x => x.DateAccepted, date)
        .Update(new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_Task>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_Task>
          {
            Success = false,
            Data = null,
            Error = $"Update failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_Task>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<string>> delete_TasksAsync(string id)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        await supabase.From<NMK_Supabase_Task>().Where(x => x.Id == id).Delete();

        return new NMK_M_Return<string>
        {
          Success = true,
          Data = null,
          Error = null
        };
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<string>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<string>> deletebyProject_TasksAsync(string projectid)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        await supabase.From<NMK_Supabase_Task>().Where(x => x.ProjectId == projectid).Delete();

        return new NMK_M_Return<string>
        {
          Success = true,
          Data = null,
          Error = null
        };
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<string>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }


    public static async Task<NMK_M_Return<List<NMK_Supabase_Task_Temporary>>> get_Task_TemporarysAsync(string createby)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Task_Temporary>().Where(x => x.CreateBy == createby).Get();
        var cities = result.Models;

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<List<NMK_Supabase_Task_Temporary>>
          {
            Success = true,
            Data = result.Models,
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<List<NMK_Supabase_Task_Temporary>>
          {
            Success = false,
            Data = null,
            Error = $"Get failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<List<NMK_Supabase_Task_Temporary>>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_Task_Temporary>> insert_Task_TemporarysAsync(NMK_Supabase_Task_Temporary Task)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Task_Temporary>().Insert(Task, new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_Task_Temporary>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_Task_Temporary>
          {
            Success = false,
            Data = null,
            Error = $"Insert failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_Task_Temporary>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_Task_Temporary>> upsert_Task_TemporarysAsync(NMK_Supabase_Task_Temporary Task)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Task_Temporary>().Upsert(Task, new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_Task_Temporary>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_Task_Temporary>
          {
            Success = false,
            Data = null,
            Error = $"Update failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_Task_Temporary>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<string>> delete_Task_TemporarysAsync(string id)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        await supabase.From<NMK_Supabase_Task_Temporary>().Where(x => x.Id == id).Delete();

        return new NMK_M_Return<string>
        {
          Success = true,
          Data = null,
          Error = null
        };
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<string>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }



    public static async Task<NMK_M_Return<NMK_Supabase_Task_Backup>> insert_Task_BackupsAsync(NMK_Supabase_Task_Backup Task)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Task_Backup>().Insert(Task, new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_Task_Backup>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_Task_Backup>
          {
            Success = false,
            Data = null,
            Error = $"Insert failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_Task_Backup>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }



    public static async Task<NMK_M_Return<List<NMK_Supabase_Version>>> get_VersionAsync()
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Version>().Get();
        var cities = result.Models;

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<List<NMK_Supabase_Version>>
          {
            Success = true,
            Data = result.Models,
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<List<NMK_Supabase_Version>>
          {
            Success = false,
            Data = null,
            Error = $"Get failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<List<NMK_Supabase_Version>>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_Version>> insert_VersionAsync(NMK_Supabase_Version version)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Version>().Insert(version, new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_Version>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_Version>
          {
            Success = false,
            Data = null,
            Error = $"Insert failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_Version>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<string>> insertfile_VersionAsync(string path, string version)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.Storage.From("RincovitchApp").Upload(path, Path.GetFileName(path));

        return new NMK_M_Return<string>
        {
          Success = true,
          Data = Path.GetFileName(path),
          Error = null
        };

      }
      catch (Exception ex)
      {
        return new NMK_M_Return<string>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }

    public static async Task<NMK_M_Return<byte[]>> downloadfile_VersionAsync(string path)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        // Specify null for the TransformOptions parameter to resolve ambiguity
        var file = await supabase.Storage.From("RincovitchApp").Download(path, transformOptions: null);
        return new NMK_M_Return<byte[]>
        {
          Success = true,
          Data = file,
          Error = null
        };

      }
      catch (Exception ex)
      {
        return new NMK_M_Return<byte[]>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }


    public static async Task<NMK_M_Return<string>> insertfile_AttachAsync(List<NMK_M_FileAttach> files)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var tasks = files.Select(async file =>
        {
          var result = await supabase.Storage.From("BackupFile").Upload(file.Name, file.Id);
        });

        await Task.WhenAll(tasks);

        return new NMK_M_Return<string>
        {
          Success = true,
          Data = "",
          Error = null
        };

      }
      catch (Exception ex)
      {
        return new NMK_M_Return<string>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<string>> downloadfile_AttachAsync(List<NMK_M_FileAttach> files, string path)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var tasks = files.Select(async file =>
        {
          var bytes = await supabase.Storage
              .From("BackupFile")
              .Download(file.Id, transformOptions: null);

          var save = Path.Combine(path, Path.GetFileName(file.Name));

          await File.WriteAllBytesAsync(save, bytes);
        });

        await Task.WhenAll(tasks);

        return new NMK_M_Return<string>
        {
          Success = true,
          Data = "",
          Error = null
        };

      }
      catch (Exception ex)
      {
        return new NMK_M_Return<string>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }



    public static async Task<NMK_M_Return<List<NMK_Supabase_Notify>>> get_notifysAsync(string sendto)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Notify>().Where(x => x.SendTo == sendto).Get();

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<List<NMK_Supabase_Notify>>
          {
            Success = true,
            Data = result.Models,
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<List<NMK_Supabase_Notify>>
          {
            Success = false,
            Data = null,
            Error = $"Get failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<List<NMK_Supabase_Notify>>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_Notify>> insert_notifysAsync(NMK_Supabase_Notify notify)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Notify>().Insert(notify, new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_Notify>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_Notify>
          {
            Success = false,
            Data = null,
            Error = $"Insert failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_Notify>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_Notify>> update_notifysAsync(string id)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Notify>()
          .Where(x => x.Id == id)
        .Set(x => x.IsRead, true)
        .Set(x => x.UpdateAt, DateTime.UtcNow)
        .Update(new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_Notify>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_Notify>
          {
            Success = false,
            Data = null,
            Error = $"Update failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_Notify>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_Notify>> updateall_notifysAsync(List<string> ids)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase
        .From<NMK_Supabase_Notify>()
        .Filter("id", Operator.In, ids)
        .Set(x => x.IsRead, true)
        .Set(x => x.UpdateAt, DateTime.UtcNow)
        .Update(new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_Notify>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_Notify>
          {
            Success = false,
            Data = null,
            Error = $"Update failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_Notify>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }



    public static async Task<NMK_M_Return<List<NMK_Supabase_Leave>>> get_leaveAsync(string userId)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Leave>().Where(x => x.CreateBy == userId).Get();

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<List<NMK_Supabase_Leave>>
          {
            Success = true,
            Data = result.Models,
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<List<NMK_Supabase_Leave>>
          {
            Success = false,
            Data = null,
            Error = $"Get failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<List<NMK_Supabase_Leave>>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<List<NMK_Supabase_Leave>>> get_leave_assignAsync(string userId)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Leave>().Where(x => x.Approval != 2).Where(x => x.SendTo == userId).Get();

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<List<NMK_Supabase_Leave>>
          {
            Success = true,
            Data = result.Models,
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<List<NMK_Supabase_Leave>>
          {
            Success = false,
            Data = null,
            Error = $"Get failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<List<NMK_Supabase_Leave>>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<List<NMK_Supabase_Leave>>> get_leave_assignCCAsync(string userId)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Leave>().Where(x => x.Approval == 0).Where(x => x.CCTo == userId).Get();

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<List<NMK_Supabase_Leave>>
          {
            Success = true,
            Data = result.Models,
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<List<NMK_Supabase_Leave>>
          {
            Success = false,
            Data = null,
            Error = $"Get failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<List<NMK_Supabase_Leave>>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_Leave>> insert_leaveAsync(NMK_Supabase_Leave leave)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase.From<NMK_Supabase_Leave>().Insert(leave, new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_Leave>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_Leave>
          {
            Success = false,
            Data = null,
            Error = $"Insert failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_Leave>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_Leave>> upsert_leaveAsync(List<NMK_Supabase_Leave> leaves)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase
        .From<NMK_Supabase_Leave>()
        .Upsert(leaves, new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_Leave>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_Leave>
          {
            Success = false,
            Data = null,
            Error = $"Update failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_Leave>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<string>> delete_leaveAsync(string id)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        await supabase.From<NMK_Supabase_Leave>().Where(x => x.Id == id).Delete();

        return new NMK_M_Return<string>
        {
          Success = true,
          Data = null,
          Error = null
        };
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<string>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
    public static async Task<NMK_M_Return<NMK_Supabase_Leave>> update_leaveAsync(string id, NMK_M_Leave item)
    {
      try
      {
        var options = new Supabase.SupabaseOptions
        {
          AutoConnectRealtime = true
        };
        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();

        var result = await supabase
        .From<NMK_Supabase_Leave>()
        .Where(x => x.Id == id)
        .Set(x => x.Approval, item.Approval)
        .Set(x => x.UpdateAt, DateTime.UtcNow)
        .Update(new QueryOptions { Returning = ReturnType.Representation });

        if (result.ResponseMessage.IsSuccessStatusCode)
        {
          return new NMK_M_Return<NMK_Supabase_Leave>
          {
            Success = true,
            Data = result.Models[0],
            Error = null
          };
        }
        else
        {
          return new NMK_M_Return<NMK_Supabase_Leave>
          {
            Success = false,
            Data = null,
            Error = $"Update failed :  {(int)result.ResponseMessage.StatusCode}: {result.ResponseMessage.ReasonPhrase}"
          };
        }
      }
      catch (Exception ex)
      {
        return new NMK_M_Return<NMK_Supabase_Leave>
        {
          Success = false,
          Data = null,
          Error = ex.Message
        };
      }
    }
  }
}
