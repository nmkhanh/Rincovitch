using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WPF;
using Newtonsoft.Json;
using RincovitchApp.API;
using RincovitchApp.API.Date;
using RincovitchApp.Models;
using RincovitchApp.Models.ModelChilds;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using static Supabase.Realtime.PostgresChanges.PostgresChangesOptions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;

namespace RincovitchApp
{
  public partial class MainWindowViewModel : ObservableObject
  {
    #region ICommand
    public ICommand ClosingCommand
    {
      get; set;
    }
    public ICommand RefreshCommand
    {
      get; set;
    }
    public ICommand ReadNotifyCommand
    {
      get; set;
    }


    public ICommand FileVersionUpdateCommand
    {
      get; set;
    }
    public ICommand FileVersionSelectCommand
    {
      get; set;
    }
    public ICommand FileVersionUploadCommand
    {
      get; set;
    }

    public ICommand SearchProjectCommand
    {
      get; set;
    }
    public ICommand SearchUserCommand
    {
      get; set;
    }
    public ICommand SearchTaskCommand
    {
      get; set;
    }


    public ICommand TaskNewCommand
    {
      get; set;
    }
    public ICommand TaskEditCommand
    {
      get; set;
    }
    public ICommand TaskDeleteCommand
    {
      get; set;
    }
    public ICommand TaskCompleteCommand
    {
      get; set;
    }
    public ICommand TaskCheckedCommand
    {
      get; set;
    }
    public ICommand TaskReCheckedCommand
    {
      get; set;
    }
    public ICommand TaskStartCommand
    {
      get; set;
    }
    public ICommand TaskAcceptCommand
    {
      get; set;
    }



    public ICommand FilterDayCommand
    {
      get; set;
    }

    public ICommand FilterDayScheduleCommand
    {
      get; set;
    }
    public ICommand ScheduleStatusChangeCommand
    {
      get; set;
    }


    public ICommand EmailSelectAllCommand
    {
      get; set;
    }
    public ICommand EmailSendCommand
    {
      get; set;
    }


    public ICommand UserAddCommand
    {
      get; set;
    }
    public ICommand UserEditCommand
    {
      get; set;
    }
    public ICommand UserDeleteCommand
    {
      get; set;
    }
    public ICommand UserImageCommand
    {
      get; set;
    }

    public ICommand ProjectAddCommand
    {
      get; set;
    }
    public ICommand ProjectEditCommand
    {
      get; set;
    }
    public ICommand ProjectDeleteCommand
    {
      get; set;
    }
    public ICommand ProjectColorCommand
    {
      get; set;
    }
    public ICommand ProjectImageCommand
    {
      get; set;
    }


    public ICommand TemporarySelectAllCommand
    {
      get; set;
    }
    public ICommand TemporarySaveCommand
    {
      get; set;
    }
    public ICommand TemporaryAddTaskCommand
    {
      get; set;
    }
    public ICommand TemporaryDeleteCommand
    {
      get; set;
    }
    public ICommand TemporaryAddCommand
    {
      get; set;
    }
    #endregion

    #region Properties
    NMK_M _NMK_M = new NMK_M();
    public NMK_M NMK_M
    {
      get
      {
        return _NMK_M;
      }
      set
      {
        _NMK_M = value;
        OnPropertyChanged();
      }
    }
    #endregion

    public MainWindowViewModel((string, string) data)
    {
      var mail = data.Item1;
      var avatar = data.Item2;
      Load(mail, avatar);
      InitRealtime();

      ReadNotifyCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        ReadNotifyCommandAsync(p);
      });



      FileVersionUpdateCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        FileVersionUpdateCommandAsync();
      });
      FileVersionSelectCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        FileVersionSelectCommandAsync();
      });
      FileVersionUploadCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        FileVersionUploadCommandAsync();
      });

      //ClosingCommand = new RelayCommand<CancelEventArgs>((p) => { return true; }, (p) => { OnClosing(p); });
      RefreshCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        refreshAsync(mail, avatar);
      });

      SearchTaskCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        //NMK_M.Tasks.Filter(NMK_M.SearchTask, NMK_M.Project.Id, NMK_M.User.Id);
      });
      SearchUserCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        NMK_M.Users.Filter(NMK_M.SearchUser);
      });
      SearchProjectCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        NMK_M.Projects.Filter(NMK_M.SearchProject);
      });


      TaskNewCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        TaskNewCommandAsync();
      });
      TaskEditCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        TaskEditCommandAsync(p);
      });
      TaskDeleteCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        TaskDeleteCommandAsync(p);
      });
      TaskCompleteCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        TaskCompleteCommandAsync(p);
      });
      TaskCheckedCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        TaskCheckedCommandAsync(p);
      });
      TaskReCheckedCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        TaskReCheckedCommandAsync(p);
      });
      TaskStartCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        TaskStartCommandAsync(p);
      });
      TaskAcceptCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        TaskAcceptCommandAsync(p);
      });



      EmailSelectAllCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        EmailSelectAllCommandAsync(p);
      });

      EmailSendCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        EmailSendCommandAsync();
      });






      UserAddCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        UserAddCommandAsync();
      });

      UserEditCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        UserEditCommandAsync(p);
      });

      UserDeleteCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        UserDeleteCommandAsync(p);
      });
      UserImageCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        UserImageCommandAsync(p);
      });





      ProjectAddCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        ProjectAddCommandAsync();
      });

      ProjectEditCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        ProjectEditCommandAsync(p);
      });

      ProjectDeleteCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        ProjectDeleteCommandAsync(p);
      });
      ProjectColorCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        ProjectColorCommandAsync(p);
      });
      ProjectImageCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        ProjectImageCommandAsync(p);
      });





      TemporarySelectAllCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        TemporarySelectAllCommandAsync(p);
      });
      TemporaryAddTaskCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        TemporaryAddTaskCommandAsync();
      });
      TemporarySaveCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        TemporarySaveCommandAsync();
      });
      TemporaryDeleteCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        TemporaryDeleteCommandAsync(p);
      });
      TemporaryAddCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        TemporaryAddCommandAsync(p);
      });
    }

    async void refreshAsync(string mail, string avatar)
    {
      try
      {
        NMK_M.refresh();
        NMK_M.IsDashboard = true;
        await Load(mail, avatar);
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
    }

    private async void InitRealtime()
    {
      try
      {
        await NMK_Supabase.InitializeAsync();

        #region Version Realtime
        var table_Version = NMK_Supabase.Client.From<NMK_Supabase_Version>();
        var is_version_new = false;
        var version_new_ = new NMK_Supabase_Version();
        await table_Version.On(ListenType.Inserts, (sender, change) =>
        {
          // Cách an toàn nhất để lấy dữ liệu:
          var version_new = change.Model<NMK_Supabase_Version>();
          version_new_ = version_new;
          is_version_new = true;

          if (is_version_new)
          {
            is_version_new = false;
            NMK_M.DialogMessage = new NMK_M_Message()
            {
              Show = true,
              Title = "New Version Available",
              Message = $"A new version ({version_new_.Version}) of the application is available. Please update to the latest version for the best experience.",
              Icon = NMK_M.DialogMessage.Icons[0],
              SupportButtonTitle = "Later",
              MainButtonTitle = "Update",
              MainButton = new RelayCommand<object>((p) => { return true; }, async (p) =>
              {
                NMK_M.DialogMessage.IsProgress = true;
                if (NMK_M.TasksTemporary.Items.Any())
                {
                  await TemporarySaveCommandAsync();
                }
                NMK_M.VersionCurrent = version_new_.Clone();
                NMK_M.VersionLast = version_new_.Clone();
                await F_VersionApp.UpdateFromDatabaseByte(version_new_);
                NMK_M.DialogMessage.Show = false;
                NMK_M.DialogMessage.IsProgress = false;
              }),
            };
          }
        });
        #endregion

        #region Leave Realtime
        var table_Leave = NMK_Supabase.Client.From<NMK_Supabase_Leave>();
        await table_Leave.On(ListenType.Updates, (sender, change) =>
        {
          // Cách an toàn nhất để lấy dữ liệu:
          var newLeave = change.Model<NMK_Supabase_Leave>();

          if (NMK_M.UserCurrent == null || string.IsNullOrEmpty(NMK_M.UserCurrent.Id))
            return;
          if (newLeave.Approval == 1)
          {
            if (newLeave.SendTo == NMK_M.UserCurrent.Id)
            {
              System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
              {
                var block = newLeave.Clone();
                block.User = NMK_M.Users.Items.FirstOrDefault(x => x.Id == block.SendTo);
                block.UserCreateBy = NMK_M.Users.Items.FirstOrDefault(x => x.Id == block.CreateBy);
                block.UserCC = NMK_M.Users.Items.FirstOrDefault(x => x.Id == block.CC);

                block.LeaveListCollectionView = new System.Windows.Data.ListCollectionView(block.LeaveList);
                block.LeaveListCollectionView.CustomSort = new F_SortLeaveCollectionList();

                NMK_M.LeaveAssignTo.Items.Add(block);
                update_user(NMK_M.LeaveAssignTo.Items);
                NMK_M.TaskbarOverlay = F_TaskBar.UpdateTaskbarBadge(NMK_M.Notifys.Items.Where(x => !x.IsRead).Count() + NMK_M.LeaveAssignTo.Items.Where(x => x.Approval == 1).Count());
                NMK_M.ReloadTask();
              });
            }
          }
          if (newLeave.Approval == 0)
          {
            if (newLeave.CCTo == NMK_M.UserCurrent.Id)
            {
              System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
              {
                var block = newLeave.Clone();
                block.User = NMK_M.Users.Items.FirstOrDefault(x => x.Id == block.SendTo);
                block.UserCreateBy = NMK_M.Users.Items.FirstOrDefault(x => x.Id == block.CreateBy);
                block.UserCC = NMK_M.Users.Items.FirstOrDefault(x => x.Id == block.CC);

                block.LeaveListCollectionView = new System.Windows.Data.ListCollectionView(block.LeaveList);
                block.LeaveListCollectionView.CustomSort = new F_SortLeaveCollectionList();

                NMK_M.LeaveAssignTo.Items.Add(block);
                update_user(NMK_M.LeaveAssignTo.Items);
                NMK_M.ReloadTask();
              });
            }
            if (newLeave.CreateBy == NMK_M.UserCurrent.Id)
            {
              System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
              {
                var is_leave = NMK_M.Leaves.Items.Any(x => x.Id == newLeave.Id);
                if (is_leave)
                {
                  var leave = NMK_M.Leaves.Items.First(x => x.Id == newLeave.Id);
                  leave.Approval = 0;
                  leave.UpdateAt = newLeave.UpdateAt;
                }
                NMK_M.ReloadTask();
              });
            }
          }
        });
        #endregion

        #region Notify Realtime
        var table_notify = NMK_Supabase.Client.From<NMK_Supabase_Notify>();
        await table_notify.On(ListenType.Inserts, (sender, change) =>
        {
          // Cách an toàn nhất để lấy dữ liệu:
          var newNotify = change.Model<NMK_Supabase_Notify>();

          if (NMK_M.UserCurrent == null || string.IsNullOrEmpty(NMK_M.UserCurrent.Id))
            return;
          if (newNotify.SendTo != NMK_M.UserCurrent.Email)
            return;

          System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
          {
            var block = newNotify.Clone();
            NMK_M.Notifys.Items.Add(block);
            NMK_M.TaskbarOverlay = F_TaskBar.UpdateTaskbarBadge(NMK_M.Notifys.Items.Where(x => !x.IsRead).Count() + NMK_M.LeaveAssignTo.Items.Where(x => x.Approval == 1).Count());
            NMK_M.ReloadTask();
          });
        });
        await table_notify.On(ListenType.Updates, (sender, change) =>
        {
          // Cách an toàn nhất để lấy dữ liệu:
          var newNotify = change.Model<NMK_Supabase_Notify>();

          if (NMK_M.UserCurrent == null || string.IsNullOrEmpty(NMK_M.UserCurrent.Id))
            return;
          if (newNotify.SendTo != NMK_M.UserCurrent.Email)
            return;

          System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
          {
            var is_notify = NMK_M.Notifys.Items.Any(x => x.Id == newNotify.Id);
            if (is_notify)
            {
              var notify = NMK_M.Notifys.Items.First(x => x.Id == newNotify.Id);
              notify.IsRead = newNotify.IsRead;
              notify.UpdateAt = newNotify.UpdateAt;
            }
            NMK_M.TaskbarOverlay = F_TaskBar.UpdateTaskbarBadge(NMK_M.Notifys.Items.Where(x => !x.IsRead).Count() + NMK_M.LeaveAssignTo.Items.Where(x => x.Approval == 1).Count());
            NMK_M.ReloadTask();
          });
        });
        #endregion

        #region Task Realtime
        var table = NMK_Supabase.Client.From<NMK_Supabase_Task>();
        var task_change = false;
        var task_id = "";
        await table.On(ListenType.Updates, (sender, change) =>
        {
          // Cách an toàn nhất để lấy dữ liệu:
          var newTask = change.Model<NMK_Supabase_Task>();

          if (NMK_M.UserCurrent == null || string.IsNullOrEmpty(NMK_M.UserCurrent.Id))
            return;
          if (newTask.Status == 1 || newTask.Status == 2)
            return;

          System.Windows.Application.Current.Dispatcher.BeginInvoke(async () =>
          {
            var blink = true;
            if (newTask.Status == 3 && NMK_M.UserCurrent.Id == newTask.UserId)
            {
              var block = newTask.Clone();
              block.OnlyName = block.Name.Split(" : ").Last();
              block.Project = NMK_M.Projects.Items.FirstOrDefault(x => x.Id == block.ProjectId);
              block.User = NMK_M.Users.Items.FirstOrDefault(x => x.Id == block.UserId);
              block.Day = F_Date.CreateDayListNotWeek(block.DateStart.Date, block.DateEnd.Date).Count();
              block.Width = block.Day * NMK_M.Tasks.PixelsPerDay;
              block.IsAssignedTo = block.UserId == NMK_M.UserCurrent.Id;
              //NMK_M.TasksAssignedto.Items.Add(block);
              NMK_M.Tasks.Items.Add(block);

              ToastService.Show("New Task Assigned", $"A new task has been assigned to you.\n• Task: {newTask.Name}", newTask.Id);
              NMK_M.ReloadTask();
            }
            if (newTask.Status == 0)
            {
              if (NMK_M.Users.Items.Any(x => x.Id == newTask.UserId))
              {
                var block = newTask.Clone();
                NMK_M.Users.Items.First(x => x.Id == block.UserId).ColorStatus = Brushes.LightGreen;
                NMK_M.UsersCollection.Refresh();
                NMK_M.UsersCollectionRole.Refresh();
              }

              if (NMK_M.UserCurrent.Id == newTask.UserId)
              {
                var task = NMK_M.Tasks.Items.First(x => x.Id == newTask.Id);
                task.Status = 0;
                task.DateComplete = newTask.DateComplete;
                blink = false;

                ToastService.Show("Task Completed", $"A task has been marked as completed.\n• Task: {newTask.Name}", newTask.Id);
                NMK_M.ReloadTask();
              }
            }
            if (newTask.Status == 5 && NMK_M.UserCurrent.Id == newTask.UserId)
            {
              var task = NMK_M.Tasks.Items.First(x => x.Id == newTask.Id);
              task.Status = 5;
              task.FileAttachs = !string.IsNullOrEmpty(newTask.FileAttach) ? JsonConvert.DeserializeObject<ObservableCollection<NMK_M_FileAttach>>(newTask.FileAttach) : new ObservableCollection<NMK_M_FileAttach>();

              ToastService.Show("Task Re-Checked", $"A task has been marked as re-checked.\n• Task: {newTask.Name}", newTask.Id);
              NMK_M.ReloadTask();
            }

            if (newTask.Status == 4 && NMK_M.UserCurrent.Email == newTask.CreateBy)
            {
              var task = NMK_M.Tasks.Items.First(x => x.Id == newTask.Id);
              task.Status = 4;
              task.DateChecked = newTask.DateChecked;
              task.FileAttachs = !string.IsNullOrEmpty(newTask.FileAttach) ? JsonConvert.DeserializeObject<ObservableCollection<NMK_M_FileAttach>>(newTask.FileAttach) : new ObservableCollection<NMK_M_FileAttach>();

              ToastService.Show("Task Checked", $"A task has been marked as checked.\n• Task: {newTask.Name}", newTask.Id);
              NMK_M.ReloadTask();
            }
            if (newTask.Status == 6)
            {
              if (NMK_M.Users.Items.Any(x => x.Id == newTask.UserId))
              {
                var block = newTask.Clone();
                NMK_M.Users.Items.First(x => x.Id == block.UserId).ColorStatus = Brushes.OrangeRed;
                NMK_M.UsersCollection.Refresh();
                NMK_M.UsersCollectionRole.Refresh();
              }
              if (NMK_M.UserCurrent.Email == newTask.CreateBy)
              {
                var task = NMK_M.Tasks.Items.First(x => x.Id == newTask.Id);
                task.Status = 6;
                task.DateStarted = newTask.DateStart;

                ToastService.Show("Task Started", $"A task has been marked as started.\n• Task: {newTask.Name}", newTask.Id);
                NMK_M.ReloadTask();
              }
            }
            if (newTask.Status == 7)
            {
              if (NMK_M.Users.Items.Any(x => x.Id == newTask.UserId))
              {
                var block = newTask.Clone();
                NMK_M.Users.Items.First(x => x.Id == block.UserId).ColorStatus = Brushes.OrangeRed;
                NMK_M.UsersCollection.Refresh();
                NMK_M.UsersCollectionRole.Refresh();
              }

              if (NMK_M.UserCurrent.Email == newTask.CreateBy)
              {
                var task = NMK_M.Tasks.Items.First(x => x.Id == newTask.Id);
                task.Status = 7;
                task.DateAccepted = newTask.DateAccepted;

                ToastService.Show("Task Accepted", $"A task has been marked as accepted.\n• Task: {newTask.Name}", newTask.Id);
                NMK_M.ReloadTask();
              }
            }
          });
        });
        #region Delete Task
        await table.On(ListenType.Deletes, (sender, change) =>
        {
          // Cách an toàn nhất để lấy dữ liệu:
          var task_change = change.OldModel<NMK_Supabase_Task>();

          if (NMK_M.UserCurrent == null || string.IsNullOrEmpty(NMK_M.UserCurrent.Id))
            return;



          System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
          {
            var task = NMK_M.Tasks.Items.Any(x => x.Id == task_change.Id);
            if (task)
            {
              var task_remove = NMK_M.Tasks.Items.First(x => x.Id == task_change.Id);
              if (NMK_M.UserCurrent.Id == task_remove.UserId)
              {
                NMK_M.Tasks.Items.Remove(task_remove);
                NMK_M.ReloadTask();
              }
            }
          });
        });

        #endregion
        #endregion

        #region User Realtime
        var table_User = NMK_Supabase.Client.From<NMK_Supabase_User>();
        #region Update User Current
        var is_user_change = false;
        var user_change_ = new NMK_Supabase_User();
        await table_User.On(ListenType.Updates, (sender, change) =>
        {
          // Cách an toàn nhất để lấy dữ liệu:
          var user_change = change.Model<NMK_Supabase_User>();
          user_change_ = user_change;
          is_user_change = true;

          System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
          {
            if (is_user_change)
            {
              is_user_change = false;
              if (user_change_.Id == NMK_M.UserCurrent.Id)
              {
                refreshAsync(user_change_.Email, user_change_.ImageString);
              }
              else
              {
                if (user_change_.UpdateBy != NMK_M.UserCurrent.Email)
                {
                  var user = NMK_M.Users.Items.Any(x => x.Id == user_change_.Id);
                  if (user)
                  {
                    NMK_M.Users.Items.Remove(NMK_M.Users.Items.First(x => x.Id == user_change_.Id));
                    NMK_M.Users.Items.Add(user_change_.Clone());
                  }
                }
              }
            }
          });

        });
        #endregion
        #region New User
        var is_user_new = false;
        var user_new_ = new NMK_Supabase_User();
        await table_User.On(ListenType.Inserts, (sender, change) =>
        {
          // Cách an toàn nhất để lấy dữ liệu:
          var user_new = change.Model<NMK_Supabase_User>();
          user_new_ = user_new;
          is_user_new = true;
          System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
          {
            if (is_user_new && NMK_M.UserCurrent.RoleEnum != F_Role.RoleType.User && user_new_.CreateBy != NMK_M.UserCurrent.Email)
            {
              is_user_new = false;
              NMK_M.Users.Items.Add(user_new_.Clone());
            }
          });
        });

        #endregion
        #region Delete User Current
        var is_user_delete = false;
        var user_delete_ = new NMK_Supabase_User();
        await table_User.On(ListenType.Deletes, (sender, change) =>
        {
          // Cách an toàn nhất để lấy dữ liệu:
          var user_change = change.OldModel<NMK_Supabase_User>();

          if (user_change.Id == NMK_M.UserCurrent.Id)
            System.Windows.Application.Current.Shutdown();

          user_delete_ = user_change;
          is_user_delete = true;

          System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
          {
            if (is_user_delete)
            {
              is_user_delete = false;
              var user = NMK_M.Users.Items.Any(x => x.Id == user_delete_.Id);
              if (user)
                NMK_M.Users.Items.Remove(NMK_M.Users.Items.First(x => x.Id == user_delete_.Id));
            }
          });
        });

        #endregion
        #endregion

        #region Project Realtime
        var table_Project = NMK_Supabase.Client.From<NMK_Supabase_Project>();
        #region Update Project
        var is_project_change = false;
        var project_change_ = new NMK_Supabase_Project();
        await table_Project.On(ListenType.Updates, (sender, change) =>
        {
          // Cách an toàn nhất để lấy dữ liệu:
          var project_change = change.Model<NMK_Supabase_Project>();
          project_change_ = project_change;
          is_project_change = true;

          System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
          {
            if (is_project_change && project_change_.UpdateBy != NMK_M.UserCurrent.Email)
            {
              is_project_change = false;
              var isproject = NMK_M.Projects.Items.Any(x => x.Id == project_change_.Id);
              if (isproject)
              {
                var project = NMK_M.Projects.Items.First(x => x.Id == project_change_.Id);
                project_change_.ImageString = project.ImageString;
                var new_project = project_change_.Clone();
                project.Set(new_project);
                NMK_M.ReloadTask();
              }
            }
          });
        });

        #endregion
        #region New Project
        var is_project_new = false;
        var project_new_ = new NMK_Supabase_Project();
        await table_Project.On(ListenType.Inserts, (sender, change) =>
        {
          // Cách an toàn nhất để lấy dữ liệu:
          var project_new = change.Model<NMK_Supabase_Project>();
          project_new_ = project_new;
          is_project_new = true;

          System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
          {
            if (is_project_new && project_new_.CreateBy != NMK_M.UserCurrent.Email)
            {
              is_project_new = false;
              NMK_M.Projects.Items.Add(project_new_.Clone());

            }
          });
        });

        #endregion
        #region Delete Project
        var is_project_delete = false;
        var project_delete_ = new NMK_Supabase_Project();
        await table_Project.On(ListenType.Deletes, (sender, change) =>
        {
          // Cách an toàn nhất để lấy dữ liệu:
          var project_change = change.OldModel<NMK_Supabase_Project>();
          project_delete_ = project_change;
          is_project_delete = true;

          System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
          {
            if (is_project_delete)
            {
              is_project_delete = false;
              var project = NMK_M.Projects.Items.Any(x => x.Id == project_delete_.Id);
              if (project)
                NMK_M.Projects.Items.Remove(NMK_M.Projects.Items.First(x => x.Id == project_delete_.Id));

            }
          });
        });

        #endregion
        #endregion
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
    }

    #region Load
    async Task LoadUserAndVersion(string mail, string avatar)
    {
      try
      {
        var user = await NMK_Supabase.get_userAsync(mail);
        if (!user.Success)
        {
          NMK_M.DialogMessage = new NMK_M_Message()
          {
            Show = true,
            Title = "Error",
            Message = user.Error,
            Icon = NMK_M.DialogMessage.Icons[1],
          };
          return;
        }
        NMK_M.UserCurrent = user.Data.Clone();

        var versions_result = await NMK_Supabase.get_VersionAsync();
        if (versions_result.Success)
        {
          var versions = versions_result.Data;
          if (versions.Count() > 0)
          {
            var versions_sort = versions.OrderBy(x => x.CreatedAt).ToList();
            NMK_M.VersionCurrent = string.IsNullOrEmpty(Properties.Settings.Default.VersionCurrent) ? versions_sort.Last().Clone() : new NMK_M_Version()
            {
              Version = Properties.Settings.Default.VersionCurrent,
            };
            NMK_M.VersionLast = versions_sort.Last().Clone();
            if (NMK_M.IsVersionUpdate)
            {
              NMK_M.DialogMessage = new NMK_M_Message()
              {
                Show = true,
                Title = "New Version Available",
                Message = $"A new version ({versions_sort.Last().Version}) of the application is available. Please update to the latest version for the best experience.",
                Icon = NMK_M.DialogMessage.Icons[0],
                SupportButtonTitle = "Later",
                MainButtonTitle = "Update",
                MainButton = new RelayCommand<object>((p) => { return true; }, async (p) =>
                {
                  NMK_M.DialogMessage.IsProgress = true;
                  if (NMK_M.TasksTemporary.Items.Any())
                  {
                    await TemporarySaveCommandAsync();
                  }
                  await F_VersionApp.UpdateFromDatabaseByte(versions_sort.Last());
                  NMK_M.VersionCurrent = versions_sort.Last().Clone();
                  NMK_M.VersionLast = versions_sort.Last().Clone();
                  NMK_M.DialogMessage.Show = false;
                  NMK_M.DialogMessage.IsProgress = false;
                }),
              };
            }
          }
        }
        else
        {
          NMK_M.DialogMessage = new NMK_M_Message()
          {
            Show = true,
            Title = "Error",
            Message = versions_result.Error,
            Icon = NMK_M.DialogMessage.Icons[1],
          };
        }
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
    }

    async Task Load(string mail, string avatar)
    {
      NMK_M.IsLoading = true;
      try
      {
        NMK_M.Colors = F_APIGeneral_Color.Generate(false);

        await LoadUserAndVersion(mail, avatar);

        await LoadNotify();
        await LoadProject();

        NMK_M.UsersCollectionRole = new System.Windows.Data.ListCollectionView(NMK_M.Users.Items);
        NMK_M.UsersCollectionRole.CustomSort = new F_SortUser();

        if (NMK_M.UserCurrent.RoleEnum == F_Role.RoleType.User)
        {
          NMK_M.UsersCollectionRole.Filter = (obj) =>
          {
            if (obj is not NMK_M_User task)
              return false;

            if (task.Team != NMK_M.UserCurrent.Team)
              return false;

            if (task.RoleEnum == F_Role.RoleType.Admin || task.RoleEnum == F_Role.RoleType.AdminApp)
              return false;

            return true;
          };
          await LoadUser(mail);
          await LoadTask();
        }
        else if (NMK_M.UserCurrent.RoleEnum == F_Role.RoleType.Leader)
        {
          NMK_M.UsersCollectionRole.Filter = (obj) =>
          {
            if (obj is not NMK_M_User task)
              return false;

            if (task.Team != NMK_M.UserCurrent.Team)
              return false;

            if (task.RoleEnum == F_Role.RoleType.Admin || task.RoleEnum == F_Role.RoleType.AdminApp)
              return false;

            return true;
          };
          await LoadUser(mail);
          await LoadTask();
          await LoadTaskTemporary();
        }
        else if (NMK_M.UserCurrent.RoleEnum == F_Role.RoleType.Admin || NMK_M.UserCurrent.RoleEnum == F_Role.RoleType.AdminApp)
        {
          NMK_M.UsersCollectionRole.Filter = (obj) =>
          {
            if (obj is not NMK_M_User task)
              return false;

            return true;
          };
          await LoadUser(mail);
          await LoadTask();
          await LoadTaskTemporary();
        }
        await LoadLeave();
        if (NMK_M.UserCurrent.RoleEnum == F_Role.RoleType.AdminApp || NMK_M.UserCurrent.RoleEnum == F_Role.RoleType.Admin)
          await LoadTaskAdminApp();
        NMK_M.ReloadTask();
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
      NMK_M.IsLoading = false;
    }


    async Task LoadProject()
    {
      NMK_M.ProjectsCollection = new System.Windows.Data.ListCollectionView(NMK_M.Projects.Items);
      NMK_M.ProjectsCollection.CustomSort = new F_SortProject();
      NMK_M.ProjectsCollection.Filter = (obj) =>
      {
        if (obj is not NMK_M_Project task)
          return false;


        return task.Name.IndexOf(NMK_M.SearchProject, StringComparison.OrdinalIgnoreCase) >= 0 ||
        task.Key.IndexOf(NMK_M.SearchProject, StringComparison.OrdinalIgnoreCase) >= 0 ||
        task.CreateAt.ToString("dd/MM/yyyy").IndexOf(NMK_M.SearchProject, StringComparison.OrdinalIgnoreCase) >= 0 ||
        task.CreateAt.ToString("dddd").IndexOf(NMK_M.SearchProject, StringComparison.OrdinalIgnoreCase) >= 0;
      };

      var projects = await NMK_Supabase.get_ProjectsAsync();
      foreach (var item in projects.Data.OrderBy(x => x.Key))
      {
        NMK_M.Projects.Items.Add(item.Clone());
      }
    }
    async Task LoadUser(string mail)
    {
      var users = await NMK_Supabase.get_usersAsync();
      if (NMK_M.UserCurrent.RoleEnum == F_Role.RoleType.AdminApp)
      {
        foreach (var item in users.Data.Where(x => x.Role != F_Role.RoleType.AdminApp.ToString()).OrderBy(x => x.Team).ThenBy(x => x.Name))
        {
          NMK_M.Users.Items.Add(item.Clone());
        }
      }
      else
      {
        foreach (var item in users.Data.Where(x => x.Role != F_Role.RoleType.AdminApp.ToString()).OrderBy(x => x.Team).ThenBy(x => x.Name))
        {
          NMK_M.Users.Items.Add(item.Clone());
        }
      }

      var tasks = await NMK_Supabase.getstarted_TasksAsync();
      var tasks_all = new List<NMK_M_Task>();
      foreach (var item in tasks.Data)
      {
        var block = item.Clone();
        tasks_all.Add(block);
      }
      foreach (var item in NMK_M.Users.Items)
      {
        item.ColorStatus = tasks_all.Any(x => x.UserId == item.Id) ? Brushes.OrangeRed : Brushes.LightGreen;
      }
    }
    async Task LoadTask()
    {
      var tasks = await NMK_Supabase.getall_TasksAsync(NMK_M.UserCurrent.Email, NMK_M.UserCurrent.Id);
      foreach (var item in tasks.Data)
      {
        var block = item.Clone();
        block.OnlyName = block.Name.Split(" : ").Last();
        block.Project = NMK_M.Projects.Items.FirstOrDefault(x => x.Id == block.ProjectId);
        block.User = NMK_M.Users.Items.FirstOrDefault(x => x.Id == block.UserId);
        block.Day = F_Date.CreateDayListNotWeek(block.DateStart.Date, block.DateEnd.Date).Count();
        block.Width = block.Day * NMK_M.Tasks.PixelsPerDay;
        block.IsAssignedTo = block.UserId == NMK_M.UserCurrent.Id;
        NMK_M.Tasks.Items.Add(block);
      }
    }
    async Task LoadTaskTemporary()
    {
      var task = await NMK_Supabase.get_Task_TemporarysAsync(NMK_M.UserCurrent.Email);
      foreach (var item in task.Data.OrderBy(x => x.DateStart))
      {
        var block = item.Clone();
        block.OnlyName = block.Name.Split(" : ").Last();
        block.Project = NMK_M.Projects.Items.FirstOrDefault(x => x.Id == block.ProjectId);
        block.User = NMK_M.Users.Items.FirstOrDefault(x => x.Id == block.UserId);
        block.Day = F_Date.CreateDayListNotWeek(block.DateStart.Date, block.DateEnd.Date).Count();
        NMK_M.TasksTemporary.Items.Add(block);
      }
    }
    async Task LoadNotify()
    {
      var task = await NMK_Supabase.get_notifysAsync(NMK_M.UserCurrent.Email);
      foreach (var item in task.Data)
      {
        var block = item.Clone();
        NMK_M.Notifys.Items.Add(block);
      }
      NMK_M.TaskbarOverlay = F_TaskBar.UpdateTaskbarBadge(NMK_M.Notifys.Items.Where(x => !x.IsRead).Count() + NMK_M.LeaveAssignTo.Items.Where(x => x.Approval == 1).Count());
    }
    async Task LoadLeave()
    {
      var leave = await NMK_Supabase.get_leaveAsync(NMK_M.UserCurrent.Id);
      int i = 0;
      foreach (var item in leave.Data)
      {
        var block = item.Clone();
        block.Background = NMK_M.Colors[i];
        block.User = NMK_M.Users.Items.FirstOrDefault(x => x.Id == block.SendTo);
        block.UserCreateBy = NMK_M.Users.Items.FirstOrDefault(x => x.Id == block.CreateBy);
        block.UserCC = NMK_M.Users.Items.FirstOrDefault(x => x.Id == block.CC);

        block.LeaveListCollectionView = new System.Windows.Data.ListCollectionView(block.LeaveList);
        block.LeaveListCollectionView.CustomSort = new F_SortLeaveCollectionList();

        NMK_M.Leaves.Items.Add(block);
        i++;
      }

      var leave_assign = await NMK_Supabase.get_leave_assignAsync(NMK_M.UserCurrent.Id);
      foreach (var item in leave_assign.Data)
      {
        var block = item.Clone();
        block.Background = NMK_M.Colors[i];
        block.User = NMK_M.Users.Items.FirstOrDefault(x => x.Id == block.SendTo);
        block.UserCreateBy = NMK_M.Users.Items.FirstOrDefault(x => x.Id == block.CreateBy);
        block.UserCC = NMK_M.Users.Items.FirstOrDefault(x => x.Id == block.CC);

        block.LeaveListCollectionView = new System.Windows.Data.ListCollectionView(block.LeaveList);
        block.LeaveListCollectionView.CustomSort = new F_SortLeaveCollectionList();

        NMK_M.LeaveAssignTo.Items.Add(block);
        i++;
      }
      var leave_assign_cc = await NMK_Supabase.get_leave_assignCCAsync(NMK_M.UserCurrent.Id);
      foreach (var item in leave_assign_cc.Data)
      {
        var block = item.Clone();
        block.Background = NMK_M.Colors[i];
        block.User = NMK_M.Users.Items.FirstOrDefault(x => x.Id == block.SendTo);
        block.UserCreateBy = NMK_M.Users.Items.FirstOrDefault(x => x.Id == block.CreateBy);
        block.UserCC = NMK_M.Users.Items.FirstOrDefault(x => x.Id == block.CC);

        block.LeaveListCollectionView = new System.Windows.Data.ListCollectionView(block.LeaveList);
        block.LeaveListCollectionView.CustomSort = new F_SortLeaveCollectionList();

        NMK_M.LeaveAssignTo.Items.Add(block);
        i++;
      }
      update_user(NMK_M.LeaveAssignTo.Items);
      NMK_M.update_day_leave();
      NMK_M.TaskbarOverlay = F_TaskBar.UpdateTaskbarBadge(NMK_M.Notifys.Items.Where(x => !x.IsRead).Count() + NMK_M.LeaveAssignTo.Items.Where(x => x.Approval == 1).Count());
    }

    async Task LoadTaskAdminApp()
    {
      var tasks = await NMK_Supabase.getall_admin_TasksAsync();
      foreach (var item in tasks.Data)
      {
        var block = item.Clone();
        block.OnlyName = block.Name.Split(" : ").Last();
        block.Project = NMK_M.Projects.Items.FirstOrDefault(x => x.Id == block.ProjectId);
        block.User = NMK_M.Users.Items.FirstOrDefault(x => x.Id == block.UserId);
        block.Day = F_Date.CreateDayListNotWeek(block.DateStart.Date, block.DateEnd.Date).Count();
        block.Width = block.Day * NMK_M.Tasks.PixelsPerDay;
        block.IsAssignedTo = block.UserId == NMK_M.UserCurrent.Id;
        NMK_M.TasksAdmin.Items.Add(block);
      }
    }
    #endregion

    #region Leave
    [RelayCommand]
    async void LeaveCheck(object p)
    {
      try
      {
        if (p is not ToggleButton button)
          return;

        if (button.DataContext is not NMK_M_Day day)
          return;

        if (!string.IsNullOrEmpty(day.Leave.Id))
          if (day.Leave.Id != NMK_M.LeaveSelect.Id)
          {
            button.IsChecked = !button.IsChecked;
            return;
          }

        if (button.IsChecked == true)
        {
          if (!NMK_M.LeaveSelect.LeaveList.Any(x => x.LeaveStart.Date == day.Name.Date))
          {
            NMK_M.LeaveSelect.LeaveList.Add(new NMK_M_LeaveDay()
            {
              Start = day.Name.Date,
              End = day.Name.Date,
              StartH = 8,
              StartM = 30,
              EndH = 17,
              EndM = 30,
            });
            day.Leave = NMK_M.LeaveSelect;
          }
          NMK_M.LeavesCollection.Refresh();
        }
        else
        {
          if (NMK_M.LeaveSelect.LeaveList.Any(x => x.LeaveStart.Date == day.Name.Date))
          {
            NMK_M.LeaveSelect.LeaveList.Remove(NMK_M.LeaveSelect.LeaveList.First(x => x.LeaveStart.Date == day.Name.Date));
            day.Leave = new NMK_M_Leave();
          }
          NMK_M.LeavesCollection.Refresh();
        }
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
    }
    [RelayCommand]
    async void LeaveAdd()
    {
      try
      {
        var item = new NMK_M_Leave()
        {
          Id = Guid.NewGuid().ToString(),
          CreateBy = NMK_M.UserCurrent.Id,
          CreateAt = DateTime.UtcNow,
          User = new NMK_M_User(),
          UserCC = new NMK_M_User(),
          LeaveReason = "Leave",
          Background = NMK_M.Colors[NMK_M.Leaves.Items.Count + 1],
        };
        item.LeaveListCollectionView = new System.Windows.Data.ListCollectionView(item.LeaveList);
        item.LeaveListCollectionView.CustomSort = new F_SortLeaveCollectionList();
        NMK_M.Leaves.Items.Add(item);
        NMK_M.LeaveSelect = new NMK_M_Leave();
        NMK_M.LeaveChooseDay = false;

      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
    }
    [RelayCommand]
    async void LeaveDelete(object p)
    {
      if (p is not NMK_M_Leave leave)
        return;
      try
      {
        if (leave.Approval != 2)
          return;

        leave.IsProgress = true;
        await NMK_Supabase.delete_leaveAsync(leave.Id);

        NMK_M.Leaves.Items.Remove(leave);
        NMK_M.LeaveSelect = new NMK_M_Leave();
        NMK_M.LeaveChooseDay = false;
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
      leave.IsProgress = false;
    }
    [RelayCommand]
    async void LeaveChooseDay(object p)
    {
      try
      {
        if (p is not NMK_M_Leave leave)
          return;

        NMK_M.LeaveSelect = leave;
        NMK_M.LeaveChooseDay = true;
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
    }
    [RelayCommand]
    async void LeaveChooseDayClose()
    {
      try
      {
        NMK_M.LeaveSelect = new NMK_M_Leave();
        NMK_M.LeaveChooseDay = false;
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
    }
    [RelayCommand]
    async void LeaveSave()
    {
      try
      {
        foreach (var item in NMK_M.Leaves.Items)
        {
          item.IsProgress = true;
        }

        await NMK_Supabase.upsert_leaveAsync(NMK_M.Leaves.Items.Where(x => x.Approval == 2).Select(x => new
        NMK_Supabase_Leave()
        {
          Id = x.Id,
          SendTo = x.User != null ? x.User.Id : null,
          CCTo = x.UserCC != null ? x.UserCC.Id : null,
          LeaveReason = x.LeaveReason,
          LeaveList = JsonConvert.SerializeObject(x.LeaveList),
          Approval = x.Approval,
          CreatedAt = x.CreateAt,
          CreateBy = x.CreateBy,
          UpdateAt = DateTime.UtcNow,
          LeaveType = x.LeaveType,
        }).ToList());
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
      foreach (var item in NMK_M.Leaves.Items)
      {
        item.IsProgress = false;
      }
    }
    [RelayCommand]
    async void LeaveSend()
    {
      try
      {
        if (!NMK_M.Leaves.Items.Any(x => x.IsChecked))
        {
          NMK_M.DialogMessage = new NMK_M_Message()
          {
            Show = true,
            Title = "Error",
            Message = "You have unselected leave requests. Please select them before sending.",
            Icon = NMK_M.DialogMessage.Icons[1],
          };
          return;
        }

        if (NMK_M.Leaves.Items.Where(x => x.IsChecked).Any(x => x.User == null))
        {
          NMK_M.DialogMessage = new NMK_M_Message()
          {
            Show = true,
            Title = "Error",
            Message = "You have leave requests without assigned users. Please assign users before sending.",
            Icon = NMK_M.DialogMessage.Icons[1],
          };
          return;
        }

        if (NMK_M.Leaves.Items.Where(x => x.IsChecked).Where(x => x.User != null).Any(x => x.LeaveList == null || !x.LeaveList.Any()))
        {
          NMK_M.DialogMessage = new NMK_M_Message()
          {
            Show = true,
            Title = "Error",
            Message = "You have leave requests without leave details. Please provide leave details before sending.",
            Icon = NMK_M.DialogMessage.Icons[1],
          };
          return;
        }

        foreach (var item in NMK_M.Leaves.Items.Where(x => x.IsChecked))
        {
          item.IsProgress = true;
          await API.Mail.F_Mail.SendLeaveMailAsync(item.User.Email, item.User.Name, item.LeaveType, item.LeaveReason, item.LeaveList, NMK_M.UserCurrent.Name, item.UserCC != null ? item.UserCC.Email : "");
          await Task.Delay(500);
          item.Approval = 1;

          await NMK_Supabase.update_leaveAsync(item.Id, item);

          item.IsChecked = false;
          item.IsProgress = false;
        }
        NMK_M.ReloadTask();

      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
    }
    [RelayCommand]
    async void LeaveApproved(object p)
    {
      try
      {
        if (p is not NMK_M_Leave leave)
          return;

        leave.IsProgress = true;
        await API.Mail.F_Mail.ApprovalLeaveMailAsync(leave.UserCreateBy.Email, leave.UserCreateBy.Name, leave.LeaveType, leave.LeaveReason, leave.LeaveList, NMK_M.UserCurrent.Name, leave.UserCC != null ? leave.UserCC.Email : "");
        await Task.Delay(500);
        leave.Approval = 0;
        await NMK_Supabase.update_leaveAsync(leave.Id, leave);
        leave.IsProgress = false;
        NMK_M.ReloadTask();

      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
    }

    public void update_user(ObservableCollection<NMK_M_Leave> leaves)
    {
      var leaves_ = leaves.Where(x => x.Approval != 2);
      foreach (var item in leaves_)
      {
        double count_team = NMK_M.Users.Items.Where(x => x.Team == item.UserCreateBy.Team).Count();
        foreach (var item_ in item.LeaveList)
        {
          var date = item_.LeaveStart.Date;
          var users = leaves_.Where(x => x.LeaveList.Any(x => x.LeaveStart.Date == date)).Select(x => x.UserCreateBy).ToList();
          item_.Users = new ObservableCollection<NMK_M_User>(users);
          item_.Percent = (int)((double)users.Count / count_team * 100);
        }
      }
    }
    #endregion

    #region Notify
    [RelayCommand]
    async void NotifyReaddAll()
    {
      try
      {
        NMK_M.Notifys_IsReadAll = true;
        var ids = NMK_M.Notifys.Items.Where(x => !x.IsRead).ToList();
        if (ids.Any())
          await NMK_Supabase.updateall_notifysAsync(ids.Select(x => x.Id).ToList());
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
      NMK_M.Notifys_IsReadAll = false;
    }
    async void ReadNotifyCommandAsync(object p)
    {
      if (p is not NMK_M_Notify notify)
        return;

      try
      {
        NMK_M.IsDashboard = true;
        if (NMK_M.Tasks.Items.Any(x => x.Id == notify.TaskId))
        {
          var task = NMK_M.Tasks.Items.First(x => x.Id == notify.TaskId);

          NMK_M.FilterMonth = task.DateStart.Month;
          NMK_M.FilterYear = task.DateStart.Year;
          NMK_M.IsAssignedTo = task.IsAssignedTo;
          NMK_M.Filters_Status.First(x => x.State == task.Status).IsChecked = true;
          NMK_M.Filters_Status.Where(x => x.State != task.Status).ToList().ForEach(x => x.IsChecked = false);
          NMK_M.Project = NMK_M.Projects.Items.First(x => x.Id == task.ProjectId);
        }

        if (!notify.IsRead)
          await NMK_Supabase.update_notifysAsync(notify.Id);
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
    }
    #endregion

    #region Task
    [RelayCommand]
    private async Task TaskDownloadAttach(object p)
    {
      if (p is not NMK_M_Task task)
        return;

      FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
      if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
      {
        string selectedPath = folderBrowserDialog.SelectedPath;
        task.IsProgressAttach = true;
        await NMK_Supabase.downloadfile_AttachAsync(task.FileAttachs.ToList(), selectedPath);
        task.IsProgressAttach = false;
      }
    }
    [RelayCommand]
    private void FilesDropped((object p, object q) data)
    {
      var files = data.q as string[];
      if (files == null)
        return;

      if (data.p is not NMK_M_Task task)
        return;

      task.FileAttachs.Clear();
      foreach (var item in files)
      {
        task.FileAttachs.Add(new NMK_M_FileAttach() { Name = item, Id = $"{Guid.NewGuid().ToString()}{Path.GetExtension(item)}" });
      }
    }
    [RelayCommand]
    private void FilterStatus(object p)
    {
      if (p is NMK_M_Status status)
      {
        if (status.State == 1000)
        {
          NMK_M.Filters_Status.Where(x => x.State != 1000).ToList().ForEach(x => x.IsChecked = status.IsChecked);
        }
      }

      NMK_M.ReloadTask();
    }
    async void TaskNewCommandAsync()
    {
      if (NMK_M.UserCurrent.RoleEnum == F_Role.RoleType.User)
        return;

      NMK_M.DialogNewTask = new NMK_M_NewTask()
      {
        Show = true,
        Folders = MVVMSourceProject.folders,
        Projects = NMK_M.Projects,
        Project = new NMK_M_Project(),
        Users = new NMK_M_User() { Items = new System.Collections.ObjectModel.ObservableCollection<NMK_M_User>(NMK_M.UsersCollectionRole.Cast<NMK_M_User>()) },
        User = new NMK_M_User(),
        MainButton = new RelayCommand<object>((p) => { return string.IsNullOrEmpty(NMK_M.DialogNewTask.Validation); }, async (p) =>
        {
          try
          {
            var item = new NMK_M_Task()
            {
              Id = Guid.NewGuid().ToString(),
              Color = NMK_M.Projects.Items.FirstOrDefault(x => x.Id == NMK_M.DialogNewTask.Project.Id)?.Color,
              ProjectId = NMK_M.DialogNewTask.Project.Id,
              Project = NMK_M.DialogNewTask.Project,
              UserId = NMK_M.DialogNewTask.User.Id,
              User = NMK_M.DialogNewTask.User,
              Name = $"{NMK_M.DialogNewTask.Project.Key} : {NMK_M.DialogNewTask.TitleTask}",
              OnlyName = NMK_M.DialogNewTask.TitleTask,
              Detail = NMK_M.DialogNewTask.Detail,
              Area = NMK_M.DialogNewTask.Area,
              DateStart = NMK_M.DialogNewTask.DateStart.Date.AddHours(NMK_M.DialogNewTask.HourStart).AddMinutes(NMK_M.DialogNewTask.MinutesStart),
              Day = NMK_M.DialogNewTask.Day,
              DateEnd = NMK_M.DialogNewTask.DateEnd.Date.AddHours(NMK_M.DialogNewTask.HourEnd).AddMinutes(NMK_M.DialogNewTask.MinutesEnd),

              Status = 1,
              Folder = NMK_M.DialogNewTask.Folder,
            };

            NMK_M.DialogNewTask.IsProgress = true;

            var item_supabase = new NMK_Supabase_Task()
            {
              Id = item.Id,
              Index = 0,
              CreatedAt = DateTime.UtcNow,
              UpdateAt = DateTime.UtcNow,
              Name = item.Name,
              ProjectId = item.ProjectId,
              UserId = item.UserId,
              DateStart = item.DateStart,
              DateEnd = item.DateEnd,
              Detail = item.Detail,
              Area = double.Parse(item.Area),
              Color = F_Color.BrushToHexRgb(item.Color),
              Status = item.Status,
              CreateBy = NMK_M.UserCurrent.Email,
              UpdateBy = NMK_M.UserCurrent.Email,
              Folder = item.Folder,
            };
            var result = await NMK_Supabase.insert_TasksAsync(item_supabase);

            if (result.Success)
            {
              item.IsChecked = true;
              item.Width = item.Day * NMK_M.Tasks.PixelsPerDay;
              item.IsAssignedTo = item.UserId == NMK_M.UserCurrent.Id;
              NMK_M.Tasks.Items.Add(item);
              NMK_M.DialogNewTask.Show = false;
              NMK_M.ReloadTask();
            }
            else
            {
              NMK_M.DialogMessage = new NMK_M_Message()
              {
                Show = true,
                Title = "Error",
                Message = result.Error,
                Icon = NMK_M.DialogMessage.Icons[1],
              };
            }
          }
          catch (Exception ex)
          {
            NMK_M.DialogMessage = new NMK_M_Message()
            {
              Show = true,
              Title = "Error",
              Message = ex.Message,
              Icon = NMK_M.DialogMessage.Icons[1],
            };
          }
          NMK_M.DialogNewTask.IsProgress = false;
        })
      };
      if (NMK_M.DialogNewTask.DateStart.Date.AddHours(NMK_M.DialogNewTask.HourStart).AddMinutes(NMK_M.DialogNewTask.MinutesStart) <=
        NMK_M.DialogNewTask.DateStart.Date.AddHours(12).AddMinutes(30))
      {
        NMK_M.DialogNewTask.HourEnd = 12;
        NMK_M.DialogNewTask.MinutesEnd = 30;
      }
      else
      {
        {
          NMK_M.DialogNewTask.HourEnd = 17;
          NMK_M.DialogNewTask.MinutesEnd = 30;
        }
      }
    }
    async void TaskEditCommandAsync(object p)
    {
      if (p is not NMK_M_Task task)
        return;
      if (NMK_M.UserCurrent.RoleEnum == F_Role.RoleType.User)
        return;
      if (task.Status == 0)
        return;

      //if (task.Status == 4 || task.Status == 5 || task.Status == 6)
      //  return;

      var items = new System.Collections.ObjectModel.ObservableCollection<NMK_M_Task>(NMK_M.Tasks.Items
          .Where(x => x.Id != task.Id)
          .Where(x => x.Status == 1 || x.Status == 2 || x.Status == 3)
          .Where(x => x.UserId == task.UserId)
          .Where(x => x.DateStart.Date >= task.DateStart.Date)
          .Select(x => new NMK_M_Task()
          {
            Id = x.Id,
            Name = x.Name,
            OnlyName = x.OnlyName,
            ProjectId = x.ProjectId,
            UserId = x.UserId,
            DateStart = x.DateStart,
            DateEnd = x.DateEnd,
            Day = x.Day,
            IsChecked = task.ListInterrupted.Contains(x.Id),
            User = x.User,
            Project = x.Project,
          }));

      NMK_M.DialogNewTask = new NMK_M_NewTask()
      {
        Folders = MVVMSourceProject.folders,
        Folder = task.Folder,
        Show = true,
        IsNew = false,
        Projects = NMK_M.Projects,
        Project = task.Project,
        Users = new NMK_M_User() { Items = new System.Collections.ObjectModel.ObservableCollection<NMK_M_User>(NMK_M.UsersCollectionRole.Cast<NMK_M_User>()) },
        User = task.User,
        TitleTask = task.Name.Split(" : ").Last(),
        Detail = task.Detail,
        Area = task.Area,
        DateStart = task.DateStart,
        Day = task.Day,
        DateEnd = task.DateEnd,
        HourStart = task.DateStart.Hour,
        MinutesStart = task.DateStart.Minute,
        HourEnd = task.DateEnd.Hour,
        MinutesEnd = task.DateEnd.Minute,
        Title = "Edit Task",
        IsDuplicate = task.UserId == NMK_M.UserCurrent.Id,
        IsDuplicateMust = task.UserId != NMK_M.UserCurrent.Id,
        IsInterrupted = task.UserId == NMK_M.UserCurrent.Id ? task.UserId != NMK_M.UserCurrent.Id : task.IsInterrupted,
        Tasks = new NMK_M_Task()
        {
          Items = items
        },
        DayOld = task.Day - items.Where(x => x.IsChecked).Sum(x => x.Day),
        SelectInterruptedCommand = new RelayCommand<object>((p) => { return string.IsNullOrEmpty(NMK_M.DialogNewTask.Validation); }, async (p) =>
        {
          try
          {
            if (p is not NMK_M_Task task)
              return;

            NMK_M.DialogNewTask.Day = NMK_M.DialogNewTask.DayOld + NMK_M.DialogNewTask.Tasks.Items.Where(x => x.IsChecked).Sum(x => x.Day);
          }
          catch (Exception ex)
          {
            NMK_M.DialogMessage = new NMK_M_Message()
            {
              Show = true,
              Title = "Error",
              Message = ex.Message,
              Icon = NMK_M.DialogMessage.Icons[1],
            };
          }
        }),
        MainButton = new RelayCommand<object>((p) => { return string.IsNullOrEmpty(NMK_M.DialogNewTask.Validation); }, async (p) =>
        {
          try
          {
            List<NMK_M_Task> items = new List<NMK_M_Task>();
            if (!NMK_M.DialogNewTask.IsDuplicate)
            {
              var item = new NMK_M_Task()
              {
                Id = task.Id,
                Color = task.Color,
                ProjectId = NMK_M.DialogNewTask.Project.Id,
                Project = NMK_M.DialogNewTask.Project,
                UserId = NMK_M.DialogNewTask.User.Id,
                User = NMK_M.DialogNewTask.User,
                Name = $"{NMK_M.DialogNewTask.Project.Key} : {NMK_M.DialogNewTask.TitleTask}",
                OnlyName = NMK_M.DialogNewTask.TitleTask,
                Detail = NMK_M.DialogNewTask.Detail,
                Area = NMK_M.DialogNewTask.Area,
                DateStart = NMK_M.DialogNewTask.DateStart.Date.AddHours(NMK_M.DialogNewTask.HourStart).AddMinutes(NMK_M.DialogNewTask.MinutesStart),
                Day = NMK_M.DialogNewTask.Day,
                DateEnd = NMK_M.DialogNewTask.DateEnd.Date.AddHours(NMK_M.DialogNewTask.HourEnd).AddMinutes(NMK_M.DialogNewTask.MinutesEnd),

                IsInterrupted = NMK_M.DialogNewTask.IsInterrupted,
                ListInterrupted = NMK_M.DialogNewTask.Tasks.Items.Where(x => x.IsChecked).Select(x => x.Id).ToList(),

                Status = 2,
                CreateAt = task.CreateAt,
                CreateBy = task.CreateBy,
              };
              items.Add(item);
            }
            else
            {
              var item = new NMK_M_Task()
              {
                Id = Guid.NewGuid().ToString(),
                Color = task.Color,
                ProjectId = NMK_M.DialogNewTask.Project.Id,
                Project = NMK_M.DialogNewTask.Project,
                UserId = NMK_M.DialogNewTask.User.Id,
                User = NMK_M.DialogNewTask.User,
                Name = $"{NMK_M.DialogNewTask.Project.Key} : {NMK_M.DialogNewTask.TitleTask}{NMK_M.DialogNewTask.DuplicateSupport}",
                OnlyName = NMK_M.DialogNewTask.TitleTask + NMK_M.DialogNewTask.DuplicateSupport,
                Detail = NMK_M.DialogNewTask.Detail,
                Area = NMK_M.DialogNewTask.Area,
                DateStart = NMK_M.DialogNewTask.DateStart.Date.AddHours(NMK_M.DialogNewTask.HourStart).AddMinutes(NMK_M.DialogNewTask.MinutesStart),
                Day = NMK_M.DialogNewTask.Day,
                DateEnd = NMK_M.DialogNewTask.DateEnd.Date.AddHours(NMK_M.DialogNewTask.HourEnd).AddMinutes(NMK_M.DialogNewTask.MinutesEnd),

                IsInterrupted = NMK_M.DialogNewTask.IsInterrupted,
                ListInterrupted = NMK_M.DialogNewTask.Tasks.Items.Where(x => x.IsChecked).Select(x => x.Id).ToList(),

                Status = 1,
              };
              items.Add(item);
            }

            double PixelsPerDay = NMK_M.Tasks.PixelsPerDay;
            NMK_M.DialogNewTask.IsProgress = true;

            if (!NMK_M.DialogNewTask.IsDuplicate)
            {
              var result_delete = await NMK_Supabase.delete_TasksAsync(task.Id);
              if (result_delete.Success)
                NMK_M.Tasks.Items.Remove(task);
            }

            foreach (var item in items)
            {
              var item_supabase = new NMK_Supabase_Task()
              {
                Id = item.Id,
                Index = 0,
                CreatedAt = item.Status != 1 ? item.CreateAt : DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                Name = item.Name,
                ProjectId = item.ProjectId,
                UserId = item.UserId,
                DateStart = item.DateStart,
                DateEnd = item.DateEnd,
                Detail = item.Detail,
                Area = double.Parse(item.Area),
                Color = F_Color.BrushToHexRgb(item.Color),
                Status = item.Status,
                CreateBy = item.Status != 1 ? item.CreateBy : NMK_M.UserCurrent.Email,
                UpdateBy = NMK_M.UserCurrent.Email,

                IsInterrupted = item.IsInterrupted,
                ListInterrupted = string.Join(",", item.ListInterrupted)
              };
              if (NMK_M.DialogNewTask.IsDuplicate)
                item_supabase.ParentId = task.Id;
              item_supabase.IsOnlyChecked = NMK_M.DialogNewTask.IsOnlyChecked;
              item_supabase.Folder = task.Folder;
              item_supabase.FileAttach = JsonConvert.SerializeObject(task.FileAttachs.Select(x => new NMK_M_FileAttach { Name = Path.GetFileName(x.Name), Id = x.Id }));

              var result = await NMK_Supabase.insert_TasksAsync(item_supabase);

              if (result.Success)
              {
                item.FileAttachs = task.FileAttachs;
                item.Folder = task.Folder;
                item.IsChecked = true;
                item.Width = item.Day * NMK_M.Tasks.PixelsPerDay;

                item.ZIndex = item.IsInterrupted ? -1 : (item.Status == 0 ? -2 : 0);

                item.IsAssignedTo = item.UserId == NMK_M.UserCurrent.Id;
                item.IsOnlyChecked = NMK_M.DialogNewTask.IsOnlyChecked;
                NMK_M.Tasks.Items.Add(item);
              }
              else
              {
                NMK_M.DialogMessage = new NMK_M_Message()
                {
                  Show = true,
                  Title = "Error",
                  Message = result.Error,
                  Icon = NMK_M.DialogMessage.Icons[1],
                };
              }
            }

            NMK_M.DialogNewTask.Show = false;
            NMK_M.ReloadTask();
          }
          catch (Exception ex)
          {
            NMK_M.DialogMessage = new NMK_M_Message()
            {
              Show = true,
              Title = "Error",
              Message = ex.Message,
              Icon = NMK_M.DialogMessage.Icons[1],
            };
          }
        })
      };
      NMK_M.DialogNewTask.TasksCollection = new System.Windows.Data.ListCollectionView(NMK_M.DialogNewTask.Tasks.Items);
      NMK_M.DialogNewTask.TasksCollection.CustomSort = new F_SortByIsCheckedProject();
      NMK_M.DialogNewTask.TasksCollection.Filter = (obj) =>
      {
        if (obj is not NMK_M_Task task)
          return false;


        return task.Name.IndexOf(NMK_M.DialogNewTask.Search, StringComparison.OrdinalIgnoreCase) >= 0 ||
        task.IsChecked.ToString().IndexOf(NMK_M.DialogNewTask.Search, StringComparison.OrdinalIgnoreCase) >= 0 ||
        task.User.Name.IndexOf(NMK_M.DialogNewTask.Search, StringComparison.OrdinalIgnoreCase) >= 0 ||
        task.DateStart.ToString("dd/MM/yyyy").IndexOf(NMK_M.DialogNewTask.Search, StringComparison.OrdinalIgnoreCase) >= 0 ||
        task.DateStart.ToString("dddd").IndexOf(NMK_M.DialogNewTask.Search, StringComparison.OrdinalIgnoreCase) >= 0;
      };

    }
    async void TaskDeleteCommandAsync(object p)
    {
      if (p is not NMK_M_Task task)
        return;

      if (NMK_M.UserCurrent.RoleEnum == F_Role.RoleType.User)
        return;
      try
      {
        task.IsProgress = true;

        var result = await NMK_Supabase.delete_TasksAsync(task.Id);
        if (result.Success)
        {
          var item = task;
          var item_supabase = new NMK_Supabase_Task_Temporary()
          {
            Id = item.Id,
            Index = 0,
            CreatedAt = DateTime.UtcNow,
            UpdateAt = DateTime.UtcNow,
            Name = item.Name,
            ProjectId = item.Project != null ? item.Project.Id : item.ProjectId,
            UserId = item.User != null ? item.User.Id : item.UserId,
            DateStart = item.DateStart,
            DateEnd = item.DateEnd,
            Detail = item.Detail,
            Area = double.Parse(item.Area),
            Color = F_Color.BrushToHexRgb(item.Color),
            Status = 1,
            CreateBy = NMK_M.UserCurrent.Email,
            UpdateBy = NMK_M.UserCurrent.Email,
          };
          var result_temporary = await NMK_Supabase.upsert_Task_TemporarysAsync(item_supabase);

          if (result_temporary.Success)
          {
            item.Set(item_supabase);
            item.OnlyName = item.Name;
            item.Width = F_Date.CreateDayListNotWeek(item.DateStart.Date, item.DateEnd.Date).Count() * NMK_M.Tasks.PixelsPerDay;
            NMK_M.TasksTemporary.Items.Add(item);
          }
          else
          {
            NMK_M.DialogMessage = new NMK_M_Message()
            {
              Show = true,
              Title = "Error",
              Message = result.Error,
              Icon = NMK_M.DialogMessage.Icons[1],
            };
          }
          NMK_M.Tasks.Items.Remove(task);
          NMK_M.ReloadTask();
        }
        else
        {
          NMK_M.DialogMessage = new NMK_M_Message()
          {
            Show = true,
            Title = "Error",
            Message = result.Error,
            Icon = NMK_M.DialogMessage.Icons[1],
          };
        }
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
      task.IsProgress = false;
    }
    async void TaskCompleteCommandAsync(object p)
    {
      if (p is not NMK_M_Task task)
        return;
      if (task.Status == 0)
        return;

      //if (task.Status != 4)
      //  return;
      try
      {
        task.IsProgress = true;

        if (!string.IsNullOrEmpty(task.User.Email))
          await API.Mail.F_Mail.SendTaskMailCompleteAsync(task.User.Email, task.Name, task.OnlyName, task.Detail, DateTime.Now, NMK_M.UserCurrent.Name);
        await Task.Delay(500);
        var date = DateTime.UtcNow;
        task.Status = 0;
        task.DateComplete = date;
        task.ZIndex = -2;


        NMK_Supabase_Notify notify = new NMK_Supabase_Notify()
        {
          UpdateAt = DateTime.UtcNow,
          CreatedAt = DateTime.UtcNow,
          CreateBy = NMK_M.UserCurrent.Email,
          SendTo = task.User.Email,
          Id = Guid.NewGuid().ToString(),
          IsRead = false,
          Message = $"• Task: {task.Name}",
          Title = "Task Completed",
          TaskId = task.Id
        };
        await NMK_Supabase.insert_notifysAsync(notify);
        await NMK_Supabase.update0_TasksAsync(task.Id, task, date);

        NMK_M.ReloadTask();
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
      task.IsProgress = false;
    }
    async void TaskCheckedCommandAsync(object p)
    {
      if (p is not NMK_M_Task task)
        return;

      if (task.Status == 4)
        return;

      if (task.Status != 6 && task.Status != 5)
        return;
      try
      {
        task.IsProgress = true;

        //if (!string.IsNullOrEmpty(task.User.Email))
        //  API.Mail.F_Mail.SendTaskMail(task.User.Email, task.Name, task.Detail, task.DateStart, task.DateEnd, NMK_M.UserCurrent.Name);
        await Task.Delay(500);
        var date = DateTime.UtcNow;
        task.Status = 4;
        task.DateChecked = date;

        NMK_Supabase_Notify notify = new NMK_Supabase_Notify()
        {
          UpdateAt = DateTime.UtcNow,
          CreatedAt = DateTime.UtcNow,
          CreateBy = NMK_M.UserCurrent.Email,
          SendTo = task.CreateBy,
          Id = Guid.NewGuid().ToString(),
          IsRead = false,
          Message = $"• Task: {task.Name}",
          Title = "Task Checked",
          TaskId = task.Id
        };
        await NMK_Supabase.insert_notifysAsync(notify);
        await NMK_Supabase.update45_TasksAsync(task.Id, task, date);
        await NMK_Supabase.insertfile_AttachAsync(task.FileAttachs.ToList());
        if (NMK_M.UserCurrent.Team == "MODELLING")
        {
          var path = !string.IsNullOrEmpty(task.Folder) && !string.IsNullOrWhiteSpace(task.Folder) ? Path.Combine(new[] { MVVMSourceProject.path_ondrive, "00. ISSUE", $"[{task.Project.Key}] {task.Project.Name}", task.Folder, DateTime.Now.ToString("yy.MM.dd") }) :
            Path.Combine(new[] { MVVMSourceProject.path_ondrive, "00. ISSUE", $"[{task.Project.Key}] {task.Project.Name}", DateTime.Now.ToString("yy.MM.dd") });
          try
          {
            if (!Directory.Exists(path))
              Directory.CreateDirectory(path);
            task.FileAttachs.ToList().ForEach(x =>
            {
              var fileName = Path.GetFileName(x.Name);
              var destFile = Path.Combine(path, fileName);
              File.Copy(x.Name, destFile, true);
            });
          }
          catch (Exception)
          {

          }
        }
        task.FileAttachs.Clear();

        NMK_M.ReloadTask();
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
      task.IsProgress = false;
    }
    async void TaskReCheckedCommandAsync(object p)
    {
      if (p is not NMK_M_Task task)
        return;
      if (task.Status == 5)
        return;

      if (task.Status != 4)
        return;
      try
      {
        task.IsProgress = true;

        //if (!string.IsNullOrEmpty(task.User.Email))
        //  API.Mail.F_Mail.SendTaskMail(task.User.Email, task.Name, task.Detail, task.DateStart, task.DateEnd, NMK_M.UserCurrent.Name);
        await Task.Delay(500);
        var date = DateTime.UtcNow;
        task.Status = 5;
        task.DateChecked = date;

        NMK_Supabase_Notify notify = new NMK_Supabase_Notify()
        {
          UpdateAt = DateTime.UtcNow,
          CreatedAt = DateTime.UtcNow,
          CreateBy = NMK_M.UserCurrent.Email,
          SendTo = task.User.Email,
          Id = Guid.NewGuid().ToString(),
          IsRead = false,
          Message = $"• Task: {task.Name}",
          Title = "Task Re-Checked",
          TaskId = task.Id
        };
        await NMK_Supabase.insert_notifysAsync(notify);
        await NMK_Supabase.update45_TasksAsync(task.Id, task, date);
        await NMK_Supabase.insertfile_AttachAsync(task.FileAttachs.ToList());
        task.FileAttachs.Clear();

        NMK_M.ReloadTask();
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
      task.IsProgress = false;
    }
    async void TaskStartCommandAsync(object p)
    {
      if (p is not NMK_M_Task task)
        return;

      if (task.Status == 6)
        return;

      if (task.Status != 3 && task.Status != 7)
        return;
      try
      {
        task.IsProgress = true;

        //if (!string.IsNullOrEmpty(task.User.Email))
        //  API.Mail.F_Mail.SendTaskMail(task.User.Email, task.Name, task.Detail, task.DateStart, task.DateEnd, NMK_M.UserCurrent.Name);
        await Task.Delay(500);
        var date = DateTime.UtcNow;
        task.Status = 6;
        task.DateStarted = date;

        NMK_Supabase_Notify notify = new NMK_Supabase_Notify()
        {
          UpdateAt = DateTime.UtcNow,
          CreatedAt = DateTime.UtcNow,
          CreateBy = NMK_M.UserCurrent.Email,
          SendTo = task.CreateBy,
          Id = Guid.NewGuid().ToString(),
          IsRead = false,
          Message = $"• Task: {task.Name}",
          Title = "Task Started",
          TaskId = task.Id
        };
        await NMK_Supabase.insert_notifysAsync(notify);
        await NMK_Supabase.update6_TasksAsync(task.Id, task, date);
        NMK_M.ReloadTask();
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
      task.IsProgress = false;
    }
    async void TaskAcceptCommandAsync(object p)
    {
      if (p is not NMK_M_Task task)
        return;

      if (task.Status == 7)
        return;

      if (task.Status != 3)
        return;
      try
      {
        task.IsProgress = true;

        //if (!string.IsNullOrEmpty(task.User.Email))
        //  API.Mail.F_Mail.SendTaskMail(task.User.Email, task.Name, task.Detail, task.DateStart, task.DateEnd, NMK_M.UserCurrent.Name);
        await Task.Delay(500);
        var date = DateTime.UtcNow;
        task.Status = 7;
        task.DateAccepted = date;

        NMK_Supabase_Notify notify = new NMK_Supabase_Notify()
        {
          UpdateAt = DateTime.UtcNow,
          CreatedAt = DateTime.UtcNow,
          CreateBy = NMK_M.UserCurrent.Email,
          SendTo = task.CreateBy,
          Id = Guid.NewGuid().ToString(),
          IsRead = false,
          Message = $"• Task: {task.Name}",
          Title = "Task Accepted",
          TaskId = task.Id
        };
        await NMK_Supabase.insert_notifysAsync(notify);
        await NMK_Supabase.update7_TasksAsync(task.Id, task, date);
        NMK_M.ReloadTask();
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
      task.IsProgress = false;
    }
    #endregion

    #region Request
    async void EmailSelectAllCommandAsync(object p)
    {
      try
      {
        if (p is not bool IsChecked)
          return;

        foreach (var item in NMK_M.Tasks.Items)
        {
          item.IsChecked = false;
        }

        foreach (var item in NMK_M.TasksEmailCollection.Cast<NMK_M_Task>())
        {
          item.IsChecked = IsChecked;
        }

      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
    }
    async void EmailSendCommandAsync()
    {
      try
      {
        if (NMK_M.Tasks.Items.Where(x => x.IsChecked).Count() == 0)
        {
          NMK_M.DialogMessage = new NMK_M_Message()
          {
            Show = true,
            Title = "Warning",
            Message = "Please select at least one task to add.",
            Icon = NMK_M.DialogMessage.Icons[2],
          };
          return;
        }

        foreach (var item in NMK_M.Tasks.Items.Where(x => x.IsChecked))
        {
          item.IsProgress = true;
          if (!string.IsNullOrEmpty(item.User.Email))
            await API.Mail.F_Mail.SendTaskMailAsync(item.User.Email, item.Name, item.OnlyName, item.Detail, item.DateStart, item.DateEnd, NMK_M.UserCurrent.Name);
          await Task.Delay(500);
          item.Status = 3;
          item.CreateBy = NMK_M.UserCurrent.Email;

          NMK_Supabase_Notify notify = new NMK_Supabase_Notify()
          {
            UpdateAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreateBy = NMK_M.UserCurrent.Email,
            SendTo = item.User.Email,
            Id = Guid.NewGuid().ToString(),
            IsRead = false,
            Message = $"• Task: {item.Name}",
            Title = "New Task Assigned",
            TaskId = item.Id
          };
          await NMK_Supabase.insert_notifysAsync(notify);
          await NMK_Supabase.update3_TasksAsync(item.Id, item);
          await NMK_Supabase.insertfile_AttachAsync(item.FileAttachs.ToList());
          if (NMK_M.UserCurrent.Team == "MODELLING")
          {
            //var path = Path.Combine(new[] { MVVMSourceProject.path_ondrive, "00. Project", $"[{item.Project.Key}] {item.Project.Name}", item.Folder, DateTime.Now.ToString("yy.MM.dd") });
            var path = Path.Combine(new[] { MVVMSourceProject.path_ondrive, "00. Project", $"[{item.Project.Key}] {item.Project.Name}", "Markup", DateTime.Now.ToString("yy.MM.dd") });
            try
            {
              if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
              item.FileAttachs.ToList().ForEach(x =>
              {
                var fileName = Path.GetFileName(x.Name);
                var destFile = Path.Combine(path, fileName);
                File.Copy(x.Name, destFile, true);
              });
            }
            catch (Exception)
            {

            }
          }
          item.FileAttachs.Clear();

          item.IsChecked = false;
          item.IsProgress = false;
        }
        NMK_M.ReloadTask();
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
    }
    #endregion

    #region User
    async void UserAddCommandAsync()
    {
      NMK_M.DialogNewUser = new NMK_M_NewUser()
      {
        Show = true,
        Teams = NMK_M.Users.Items.Select(x => x.Team).Distinct().ToList(),
        MainButton = new RelayCommand<object>((p) => { return string.IsNullOrEmpty(NMK_M.DialogNewUser.Validation); }, async (p) =>
        {
          try
          {
            NMK_M.DialogNewUser.IsProgress = true;

            var item = new NMK_Supabase_User
            {
              Id = Guid.NewGuid().ToString(),
              Index = 0,
              CreatedAt = DateTime.UtcNow,
              UpdateAt = DateTime.UtcNow,
              Name = NMK_M.DialogNewUser.Name,
              Team = NMK_M.DialogNewUser.Team,
              Color = NMK_M.Users.Items.Count() > 0 ? F_Color.BrushToHexRgb(NMK_M.Users.Items.First(x => x.Team == NMK_M.DialogNewUser.Team).Color) : null,
              Email = NMK_M.DialogNewUser.Email,
              Role = NMK_M.DialogNewUser.Role,
              CreateBy = NMK_M.UserCurrent.Email,
              UpdateBy = NMK_M.UserCurrent.Email,
              ImageString = null,
              Location = "VietNam",
            };
            var result = await NMK_Supabase.insert_usersAsync(item);

            if (result.Success)
            {
              NMK_M.Users.Items.Add(result.Data.Clone());
              NMK_M.UsersCollection.Refresh();

              NMK_M.DialogNewUser.Show = false;
            }
            else
            {
              NMK_M.DialogMessage = new NMK_M_Message()
              {
                Show = true,
                Title = "Error",
                Message = result.Error,
                Icon = NMK_M.DialogMessage.Icons[1],
              };
            }
          }
          catch (Exception ex)
          {
            NMK_M.DialogMessage = new NMK_M_Message()
            {
              Show = true,
              Title = "Error",
              Message = ex.Message,
              Icon = NMK_M.DialogMessage.Icons[1],
            };
          }
          NMK_M.DialogNewUser.IsProgress = false;
        })
      };
    }
    async void UserEditCommandAsync(object p)
    {
      if (p is not NMK_M_User user)
        return;

      if (NMK_M.UserCurrent.RoleEnum == F_Role.RoleType.AdminApp)
        goto NEXT;

      if (user.RoleEnum == F_Role.RoleType.AdminApp || user.RoleEnum == F_Role.RoleType.Admin)
        return;

      NEXT:
      NMK_M.DialogNewUser = new NMK_M_NewUser()
      {
        Title = "Update User",
        MainButtonTitle = "Update",
        Show = true,
        Name = user.Name,
        Email = user.Email,
        Role = user.Role,
        Team = user.Team,
        Teams = NMK_M.Users.Items.Select(x => x.Team).Distinct().ToList(),
        MainButton = new RelayCommand<object>((p) => { return string.IsNullOrEmpty(NMK_M.DialogNewUser.Validation); }, async (p) =>
        {
          try
          {
            NMK_M.DialogNewUser.IsProgress = true;

            var item = new NMK_Supabase_User
            {
              Id = user.Id,
              Index = user.Index,
              CreatedAt = user.CreateAt,
              UpdateAt = DateTime.UtcNow,
              Name = NMK_M.DialogNewUser.Name,
              Team = NMK_M.DialogNewUser.Team,
              Color = user.Color != null ? F_Color.BrushToHexRgb(user.Color) : null,
              Email = NMK_M.DialogNewUser.Email,
              Role = NMK_M.DialogNewUser.Role,
              CreateBy = user.CreateBy,
              UpdateBy = NMK_M.UserCurrent.Email,
              ImageString = user.ImageString,
              Location = user.Location,
            };
            var result = await NMK_Supabase.upsert_usersAsync(item);

            if (result.Success)
            {
              NMK_M.Users.Items.Remove(user);
              NMK_M.Users.Items.Add(result.Data.Clone());
              NMK_M.UsersCollection.Refresh();

              NMK_M.DialogNewUser.Show = false;
            }
            else
            {
              NMK_M.DialogMessage = new NMK_M_Message()
              {
                Show = true,
                Title = "Error",
                Message = result.Error,
                Icon = NMK_M.DialogMessage.Icons[1],
              };
            }
          }
          catch (Exception ex)
          {
            NMK_M.DialogMessage = new NMK_M_Message()
            {
              Show = true,
              Title = "Error",
              Message = ex.Message,
              Icon = NMK_M.DialogMessage.Icons[1],
            };
          }
          NMK_M.DialogNewUser.IsProgress = false;
        })
      };
    }
    async void UserDeleteCommandAsync(object p)
    {
      if (p is not NMK_M_User user)
        return;

      if (NMK_M.UserCurrent.RoleEnum == F_Role.RoleType.AdminApp)
        goto NEXT;

      if (user.RoleEnum == F_Role.RoleType.AdminApp || user.RoleEnum == F_Role.RoleType.Admin)
        return;

      NEXT:
      try
      {
        user.IsProgress = true;

        var result = await NMK_Supabase.delete_usersAsync(user.Id);

        if (result.Success)
        {
          NMK_M.Users.Items.Remove(user);
          NMK_M.UsersCollection.Refresh();
        }
        else
        {
          NMK_M.DialogMessage = new NMK_M_Message()
          {
            Show = true,
            Title = "Error",
            Message = result.Error,
            Icon = NMK_M.DialogMessage.Icons[1],
          };
        }
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
      user.IsProgress = false;
    }
    async void UserImageCommandAsync(object p)
    {
      try
      {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
          var imageString = F_Image.ImagePathToBase64(openFileDialog.FileName);

          var result = await NMK_Supabase.updateImage_usersAsync(NMK_M.UserCurrent.Id, imageString);
          if (result.Success)
          {
            NMK_M.UserCurrent.ImageString = imageString;
          }
          else
          {
            NMK_M.DialogMessage = new NMK_M_Message()
            {
              Show = true,
              Title = "Error",
              Message = result.Error,
              Icon = NMK_M.DialogMessage.Icons[1],
            };
          }
        }
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
    }
    #endregion

    #region Project
    async void ProjectAddCommandAsync()
    {
      NMK_M.DialogNewProject = new NMK_M_NewProject()
      {
        Show = true,
        Project = NMK_M.Projects,
        MainButton = new RelayCommand<object>((p) => { return string.IsNullOrEmpty(NMK_M.DialogNewProject.Validation); }, async (p) =>
        {
          try
          {
            NMK_M.DialogNewProject.IsProgress = true;

            var item = new NMK_Supabase_Project
            {
              Id = Guid.NewGuid().ToString(),
              Index = 0,
              CreatedAt = DateTime.UtcNow,
              UpdateAt = DateTime.UtcNow,
              Name = NMK_M.DialogNewProject.Name,
              Color = NMK_M.DialogNewProject.Color != null ? F_Color.BrushToHexRgb(NMK_M.DialogNewProject.Color) : null,
              CreateBy = NMK_M.UserCurrent.Email,
              UpdateBy = NMK_M.UserCurrent.Email,
              ImageString = MVVMSourceProject.image,
              Key = NMK_M.DialogNewProject.Key,
              Description = NMK_M.DialogNewProject.Description,
              RevitVersion = NMK_M.DialogNewProject.RevitVersion,
            };
            var result = await NMK_Supabase.insert_ProjectsAsync(item);

            if (result.Success)
            {
              NMK_M.Projects.Items.Add(result.Data.Clone());
              NMK_M.ProjectsCollection.Refresh();

              NMK_M.DialogNewProject.Show = false;
            }
            else
            {
              NMK_M.DialogMessage = new NMK_M_Message()
              {
                Show = true,
                Title = "Error",
                Message = result.Error,
                Icon = NMK_M.DialogMessage.Icons[1],
              };
            }
          }
          catch (Exception ex)
          {
            NMK_M.DialogMessage = new NMK_M_Message()
            {
              Show = true,
              Title = "Error",
              Message = ex.Message,
              Icon = NMK_M.DialogMessage.Icons[1],
            };
          }
          NMK_M.DialogNewProject.IsProgress = false;
        })
      };
    }
    async void ProjectEditCommandAsync(object p)
    {
      if (p is not NMK_M_Project Project)
        return;

      NMK_M.DialogNewProject = new NMK_M_NewProject()
      {
        IsNew = false,
        Project = NMK_M.Projects,
        Title = "Update Project",
        MainButtonTitle = "Update",
        Show = true,
        Name = Project.Name,
        Key = Project.Key,
        Description = Project.Description,
        Color = Project.Color,
        RevitVersion = Project.RevitVersion,
        MainButton = new RelayCommand<object>((p) => { return string.IsNullOrEmpty(NMK_M.DialogNewProject.Validation); }, async (p) =>
        {
          try
          {
            NMK_M.DialogNewProject.IsProgress = true;

            var item = new NMK_Supabase_Project
            {
              Key = NMK_M.DialogNewProject.Key,
              Description = NMK_M.DialogNewProject.Description,
              Id = Project.Id,
              Index = Project.Index,
              CreatedAt = Project.CreateAt,
              UpdateAt = DateTime.UtcNow,
              Name = NMK_M.DialogNewProject.Name,
              Color = NMK_M.DialogNewProject.Color != null ? F_Color.BrushToHexRgb(NMK_M.DialogNewProject.Color) : null,
              CreateBy = Project.CreateBy,
              UpdateBy = NMK_M.UserCurrent.Email,
              ImageString = Project.ImageString,
              RevitVersion = NMK_M.DialogNewProject.RevitVersion,
            };
            var result = await NMK_Supabase.upsert_ProjectsAsync(item);

            if (result.Success)
            {
              NMK_M.Projects.Items.Remove(Project);
              NMK_M.Projects.Items.Add(result.Data.Clone());
              NMK_M.ProjectsCollection.Refresh();

              NMK_M.DialogNewProject.Show = false;
            }
            else
            {
              NMK_M.DialogMessage = new NMK_M_Message()
              {
                Show = true,
                Title = "Error",
                Message = result.Error,
                Icon = NMK_M.DialogMessage.Icons[1],
              };
            }
          }
          catch (Exception ex)
          {
            NMK_M.DialogMessage = new NMK_M_Message()
            {
              Show = true,
              Title = "Error",
              Message = ex.Message,
              Icon = NMK_M.DialogMessage.Icons[1],
            };
          }
          NMK_M.DialogNewProject.IsProgress = false;
        })
      };
    }
    async void ProjectDeleteCommandAsync(object p)
    {
      if (p is not NMK_M_Project Project)
        return;

      try
      {
        Project.IsProgress = true;

        var result = await NMK_Supabase.delete_ProjectsAsync(Project.Id);

        if (result.Success)
        {
          NMK_M.Projects.Items.Remove(Project);
          NMK_M.ProjectsCollection.Refresh();
        }
        else
        {
          NMK_M.DialogMessage = new NMK_M_Message()
          {
            Show = true,
            Title = "Error",
            Message = result.Error,
            Icon = NMK_M.DialogMessage.Icons[1],
          };
        }
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
      Project.IsProgress = false;
    }
    async void ProjectColorCommandAsync(object p)
    {
      if (p is not NMK_M_Project Project)
        return;

      try
      {
        NMK_M.colorDialog.Color = System.Drawing.Color.FromArgb(Project.Color is SolidColorBrush scb ? scb.Color.R : (byte)255, Project.Color is SolidColorBrush scb2 ? scb2.Color.G : (byte)255, Project.Color is SolidColorBrush scb3 ? scb3.Color.B : (byte)255);
        NMK_M.colorDialog.CustomColors.ToList().Add(
          BitConverter.ToInt32(new byte[4] { NMK_M.colorDialog.Color.R, NMK_M.colorDialog.Color.G, NMK_M.colorDialog.Color.B, 0 }, 0));
        if (NMK_M.colorDialog.ShowDialog() == DialogResult.OK)
        {
          Project.IsProgress = true;
          var brush = new SolidColorBrush(Color.FromRgb(NMK_M.colorDialog.Color.R, NMK_M.colorDialog.Color.G, NMK_M.colorDialog.Color.B));
          var color = F_Color.BrushToHexRgb(brush);

          var result = await NMK_Supabase.update_ProjectsAsync(Project.Id, color);

          if (result.Success)
          {
            Project.Color = brush;
            NMK_M.ProjectsCollection.Refresh();
          }
          else
          {
            NMK_M.DialogMessage = new NMK_M_Message()
            {
              Show = true,
              Title = "Error",
              Message = result.Error,
              Icon = NMK_M.DialogMessage.Icons[1],
            };
          }
        }
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
      Project.IsProgress = false;
    }
    async void ProjectImageCommandAsync(object p)
    {
      if (p is not NMK_M_Project Project)
        return;
      if (NMK_M.UserCurrent.RoleEnum != F_Role.RoleType.Admin && NMK_M.UserCurrent.RoleEnum != F_Role.RoleType.AdminApp)
        return;
      try
      {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
          var imageString = F_Image.ImagePathToBase64(openFileDialog.FileName);
          Project.IsProgress = true;
          var result = await NMK_Supabase.updateImage_ProjectsAsync(Project.Id, imageString);
          if (result.Success)
          {
            Project.ImageString = imageString;
            NMK_M.ProjectsCollection.Refresh();
          }
          else
          {
            NMK_M.DialogMessage = new NMK_M_Message()
            {
              Show = true,
              Title = "Error",
              Message = result.Error,
              Icon = NMK_M.DialogMessage.Icons[1],
            };
          }
        }
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
      Project.IsProgress = false;
    }
    #endregion

    #region Task temporary
    async void TemporarySelectAllCommandAsync(object p)
    {
      try
      {
        if (p is not bool IsChecked)
          return;

        foreach (var item in NMK_M.TasksTemporary.Items)
        {
          item.IsChecked = false;
        }

        foreach (var item in NMK_M.TasksTemporaryCollection.Cast<NMK_M_Task>())
        {
          item.IsChecked = IsChecked;
        }

      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
    }
    async Task TemporarySaveCommandAsync()
    {
      try
      {
        foreach (var item in NMK_M.TasksTemporary.Items)
        {
          item.IsProgress = true;

          var item_supabase = new NMK_Supabase_Task_Temporary()
          {
            Id = item.Id,
            Index = 0,
            CreatedAt = DateTime.UtcNow,
            UpdateAt = DateTime.UtcNow,
            Name = $"{item.Project.Key} : {item.OnlyName}",
            ProjectId = item.Project != null ? item.Project.Id : item.ProjectId,
            UserId = item.User != null ? item.User.Id : item.UserId,
            DateStart = item.DateStart.Date.AddHours(item.HourStart).AddMinutes(item.MinutesStart),
            DateEnd = item.DateEnd.Date.AddHours(item.HourEnd).AddMinutes(item.MinutesEnd),
            Detail = item.Detail,
            Area = double.Parse(item.Area),
            Color = F_Color.BrushToHexRgb(item.Color),
            Status = 1,
            CreateBy = NMK_M.UserCurrent.Email,
            UpdateBy = NMK_M.UserCurrent.Email,
          };
          var result = await NMK_Supabase.upsert_Task_TemporarysAsync(item_supabase);

          if (result.Success)
          {
            item.Set(item_supabase);
            item.OnlyName = item.Name;
            item.Width = F_Date.CreateDayListNotWeek(item.DateStart.Date, item.DateEnd.Date).Count() * NMK_M.Tasks.PixelsPerDay;
          }
          else
          {
            NMK_M.DialogMessage = new NMK_M_Message()
            {
              Show = true,
              Title = "Error",
              Message = result.Error,
              Icon = NMK_M.DialogMessage.Icons[1],
            };
          }
          item.IsProgress = false;
        }
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
    }
    async void TemporaryAddTaskCommandAsync()
    {
      try
      {
        if (NMK_M.TasksTemporary.Items.Where(x => x.IsChecked).Count() == 0)
        {
          NMK_M.DialogMessage = new NMK_M_Message()
          {
            Show = true,
            Title = "Warning",
            Message = "Please select at least one task to add.",
            Icon = NMK_M.DialogMessage.Icons[2],
          };
          return;
        }

        if (NMK_M.TasksTemporary.Items.Where(x => x.IsChecked).Any(x => x.Project == null) ||
        NMK_M.TasksTemporary.Items.Where(x => x.IsChecked).Any(x => x.User == null) ||
        NMK_M.TasksTemporary.Items.Where(x => x.IsChecked).Any(x => string.IsNullOrEmpty(x.Name)))
        {
          NMK_M.DialogMessage = new NMK_M_Message()
          {
            Show = true,
            Title = "Warning",
            Message = "Please make sure all selected tasks have Project, User and Name filled in.",
            Icon = NMK_M.DialogMessage.Icons[2],
          };
          return;
        }

        foreach (var item in NMK_M.TasksTemporary.Items.Where(x => x.IsChecked))
        {
          item.IsProgress = true;

          var item_supabase = new NMK_Supabase_Task()
          {
            Id = item.Id,
            Index = 0,
            CreatedAt = DateTime.UtcNow,
            UpdateAt = DateTime.UtcNow,
            Name = $"{item.Project.Key} : {item.OnlyName}",
            ProjectId = item.Project.Id,
            UserId = item.User.Id,
            DateStart = item.DateStart.Date.AddHours(item.HourStart).AddMinutes(item.MinutesStart),
            DateEnd = item.DateEnd.Date.AddHours(item.HourEnd).AddMinutes(item.MinutesEnd),
            Detail = item.Detail,
            Area = double.Parse(item.Area),
            Color = F_Color.BrushToHexRgb(item.Color),
            Status = 1,
            CreateBy = NMK_M.UserCurrent.Email,
            UpdateBy = NMK_M.UserCurrent.Email,
          };
          var result = await NMK_Supabase.insert_TasksAsync(item_supabase);

          if (result.Success)
          {
            item.Set(item_supabase);
            item.OnlyName = item.Name.Split(" : ").Last();
            item.Width = F_Date.CreateDayListNotWeek(item.DateStart.Date, item.DateEnd.Date).Count() * NMK_M.Tasks.PixelsPerDay;
            item.Status = 1;
          }
          else
          {
            NMK_M.DialogMessage = new NMK_M_Message()
            {
              Show = true,
              Title = "Error",
              Message = result.Error,
              Icon = NMK_M.DialogMessage.Icons[1],
            };
          }
          item.IsProgress = false;
        }
        var list = new List<NMK_M_Task>(NMK_M.TasksTemporary.Items.Where(x => x.IsChecked));
        foreach (var item in list)
        {
          item.IsProgress = true;
          item.IsAssignedTo = item.UserId == NMK_M.UserCurrent.Id;
          NMK_M.Tasks.Items.Add(item);
          TemporaryDeleteCommandAsync(item);
          item.IsProgress = false;
        }
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
    }
    async void TemporaryDeleteCommandAsync(object p)
    {
      if (p is not NMK_M_Task task)
        return;
      try
      {
        task.IsProgress = true;

        var result = await NMK_Supabase.delete_Task_TemporarysAsync(task.Id);
        if (result.Success)
        {
          NMK_M.TasksTemporary.Items.Remove(task);
        }
        else
        {
          NMK_M.DialogMessage = new NMK_M_Message()
          {
            Show = true,
            Title = "Error",
            Message = result.Error,
            Icon = NMK_M.DialogMessage.Icons[1],
          };
        }
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
      task.IsProgress = false;
    }
    async void TemporaryAddCommandAsync(object p)
    {
      try
      {
        var week = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        var day = F_Date.GetWeekdaysOfWeek(DateTime.Today.Year, week);
        var days = F_Date.CreateDayList(day.Start, day.End);
        NMK_M.TasksTemporary.Items.Add(new NMK_M_Task()
        {
          CreateBy = NMK_M.UserCurrent.Email,
          IsChecked = true,
          Id = Guid.NewGuid().ToString(),
          Name = "NEW TASK",
          LeaveList = new ObservableCollection<NMK_M_LeaveDay>(
            days.Select(x => new NMK_M_LeaveDay()
            {
              Start = x.Name.Date,
              End = x.Name.Date,
              StartH = 8,
              StartM = 30,
              EndH = 17,
              EndM = 30,
            }))
        });
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
    }
    #endregion

    #region Version
    async void FileVersionUpdateCommandAsync()
    {
      try
      {
        NMK_M.IsVersionUpdateProgress = true;
        if (NMK_M.TasksTemporary.Items.Any())
        {
          await TemporarySaveCommandAsync();
        }

        await F_VersionApp.UpdateFromDatabaseByte(NMK_M.VersionLast.Supabase_Version);
        NMK_M.VersionCurrent = NMK_M.VersionLast.Supabase_Version.Clone();
        NMK_M.VersionLast = NMK_M.VersionLast.Supabase_Version.Clone();
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
      NMK_M.IsVersionUpdateProgress = false;
    }
    async void FileVersionSelectCommandAsync()
    {
      try
      {
        OpenFileDialog fileDialog = new OpenFileDialog();
        fileDialog.Filter = "msi files (*.msi)|*.msi";
        if (fileDialog.ShowDialog() == DialogResult.OK)
        {
          NMK_M.VersionFile = fileDialog.FileName;
        }
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
    }
    async void FileVersionUploadCommandAsync()
    {
      try
      {
        if (string.IsNullOrEmpty(NMK_M.VersionFile) || !File.Exists(NMK_M.VersionFile))
          return;

        NMK_M.IsVersionUploadProgress = true;
        var file = await NMK_Supabase.insertfile_VersionAsync(NMK_M.VersionFile, NMK_M.VersionName);
        var version = new NMK_Supabase_Version()
        {
          Id = Guid.NewGuid().ToString(),
          CreatedAt = DateTime.UtcNow.AddYears(1),
          Version = NMK_M.VersionName,
          Data = file.Data,
        };
        var result = await NMK_Supabase.insert_VersionAsync(version);
        if (!result.Success)
        {
          NMK_M.DialogMessage = new NMK_M_Message()
          {
            Show = true,
            Title = "Error",
            Message = result.Error,
            Icon = NMK_M.DialogMessage.Icons[1],
          };
        }
        else
        {
          NMK_M.DialogMessage = new NMK_M_Message()
          {
            Show = true,
            Title = "Success",
            Message = "Version uploaded successfully.",
            Icon = NMK_M.DialogMessage.Icons[0],
          };
        }
      }
      catch (Exception ex)
      {
        NMK_M.DialogMessage = new NMK_M_Message()
        {
          Show = true,
          Title = "Error",
          Message = ex.Message,
          Icon = NMK_M.DialogMessage.Icons[1],
        };
      }
      NMK_M.IsVersionUploadProgress = false;
    }
    #endregion
  }
}
