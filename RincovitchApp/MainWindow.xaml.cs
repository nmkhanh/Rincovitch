using Newtonsoft.Json;
using RincovitchApp.API;
using RincovitchApp.API.Date;
using RincovitchApp.Models;
using RincovitchApp.Models.ModelChilds;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms; // Lưu ý: Cần thêm reference WinForms như đã hướng dẫn

namespace RincovitchApp
{
  public partial class MainWindow : Window
  {
    private NotifyIcon _notifyIcon;
    public MainWindowViewModel VM
    {
      get; set;
    }

    public MainWindow()
    {
      MVVMWindows_Themes.ApplyTheme(this.Resources, "Dark");
      InitializeComponent();

      // QUAN TRỌNG: Lưu tham chiếu vào App để ToastService có thể tìm thấy
      App.MyMainWindow = this;

      InitTrayIcon();
      this.Icon = F_Image.ByteArrayToBitmapImage(Properties.Resources.RincovitchIcon);

      // Dùng Loaded event để tránh deadlock khi await trên UI thread
      this.Loaded += async (s, e) =>
      {
        var data = await checkLoginAsync();
        this.DataContext = VM = new MainWindowViewModel(data);
      };
    }

    public async Task NavigateToTaskView(string id)
    {
      if (VM == null)
        return;

      var task = await NMK_Supabase.getbyid_TasksAsync(id);
      if (task.Success)
      {
        Dashboard.IsChecked = true;
        VM.NMK_M.FilterMonth = task.Data[0].DateStart.Month;
        VM.NMK_M.FilterYear = task.Data[0].DateStart.Year;
        VM.NMK_M.IsAssignedTo = task.Data[0].UserId == VM.NMK_M.UserCurrent.Id;
        VM.NMK_M.Filters_Status.First(x => x.State == task.Data[0].Status).IsChecked = true;
        VM.NMK_M.Filters_Status.Where(x => x.State != task.Data[0].Status).ToList().ForEach(x => x.IsChecked = false);
        VM.NMK_M.Project = VM.NMK_M.Projects.Items.First(x => x.Id == task.Data[0].ProjectId);

        VM.NMK_M.ReloadTask();
      }
      else
      {
        System.Windows.MessageBox.Show(task.Error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    // Hàm này dùng để hiện App từ bất cứ đâu (Toast hoặc Tray Icon)
    public void ShowApp()
    {
      // 1. Nếu đang ẩn thì hiện lại
      if (!this.IsVisible)
      {
        this.Show();
      }

      // 2. Nếu đang thu nhỏ thì phóng to lại
      if (this.WindowState == WindowState.Minimized)
      {
        this.WindowState = WindowState.Normal;
      }

      // 3. Đưa lên trên cùng và tập trung
      this.Visibility = Visibility.Visible;
      this.Activate();
      this.Topmost = true;  // Nháy Topmost để tranh quyền ưu tiên hiển thị
      this.Topmost = false;
      this.Focus();
    }

    private void InitTrayIcon()
    {
      _notifyIcon = new NotifyIcon();
      _notifyIcon.Icon = F_Image.ByteArrayToIcon(Properties.Resources.RincovitchIcon);
      _notifyIcon.Visible = true;
      _notifyIcon.Text = "Rincovitch Task Manager";

      _notifyIcon.DoubleClick += (s, e) => ShowApp();

      var contextMenu = new ContextMenuStrip();
      contextMenu.Items.Add("Open", null, (s, e) => ShowApp());
      contextMenu.Items.Add("Exit", null, (s, e) => ExitApp());
      _notifyIcon.ContextMenuStrip = contextMenu;
    }

    private void ExitApp()
    {
      _notifyIcon.Dispose();
      System.Windows.Application.Current.Shutdown();
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
      // Nếu không phải là thoát hẳn (ExitApp), thì chỉ ẩn đi
      e.Cancel = true;
      this.Hide();

      // Chạy logic backup dữ liệu ngầm của bạn
      BackupDataAsync();
    }

    private async void BackupDataAsync()
    {
      try
      {
        Properties.Settings.Default.VersionCurrent = "";
        Properties.Settings.Default.VersionCurrent = VM.NMK_M.IsVersionUpdate ? VM.NMK_M.VersionCurrent.Version : VM.NMK_M.VersionLast.Version;
        Properties.Settings.Default.Save();

        string rawPath = @"%UserProfile%\AppData\Local\RincocitchApp\Temp";
        string path = Environment.ExpandEnvironmentVariables(rawPath);
        if (!Directory.Exists(path))
          Directory.CreateDirectory(path);

        string file = System.IO.Path.Combine(path, Guid.NewGuid().ToString() + ".json");
        var data_ = new
        {
          Tasks = VM.NMK_M.Tasks.Items.ToList(),
          TasksTemporary = VM.NMK_M.TasksTemporary.Items.ToList(),
          Users = VM.NMK_M.Users.Items.ToList(),
          Projects = VM.NMK_M.Projects.Items.ToList(),
        };

        NMK_M_Task_Backup data = new NMK_M_Task_Backup()
        {
          CreateAt = DateTime.Now,
          CreateBy = VM.NMK_M.UserCurrent.Email,
          Id = Guid.NewGuid().ToString(),
          Data = data_
        };

        File.WriteAllText(file, JsonConvert.SerializeObject(data));

        await NMK_Supabase.insert_Task_BackupsAsync(new NMK_Supabase_Task_Backup()
        {
          Id = data.Id,
          CreatedAt = data.CreateAt,
          CreateBy = data.CreateBy,
          Data = JsonConvert.SerializeObject(data_),
        });
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine("Backup Error: " + ex.Message);
      }
    }

    async Task<(string, string)> checkLoginAsync()
    {
      string mail = "";
      string avatar = "";
      try
      {
        bool isLogin = false;

        // ===== BƯỚC 1: REGISTRY (không cần Outlook mở, không cần token) =====
        mail = API.Mail.F_Mail.GetEmailFromRegistry();
        if (!string.IsNullOrEmpty(mail))
        {
          isLogin = true;
          System.Diagnostics.Debug.WriteLine($"[Login] Registry OK: {mail}");
        }
        else
        {
          System.Diagnostics.Debug.WriteLine("[Login] Registry không tìm được → thử COM");
        }

        // ===== BƯỚC 2: CLASSIC OUTLOOK (COM – chỉ khi Outlook process đang chạy) =====
        if (!isLogin)
        {
          try
          {
            isLogin = API.Mail.F_Mail.IsOutlookLoggedIn(out mail, out avatar);
            if (isLogin)
              System.Diagnostics.Debug.WriteLine($"[Login] COM OK: {mail}");
            else
              System.Diagnostics.Debug.WriteLine("[Login] COM không khả dụng → thử Graph/WAM");
          }
          catch (Exception ex)
          {
            System.Diagnostics.Debug.WriteLine($"[Login] COM exception: {ex.Message}");
            isLogin = false;
          }
        }

        // ===== BƯỚC 3: NEW OUTLOOK / GRAPH (WAM – fallback cuối cùng) =====
        if (!isLogin)
        {
          mail = await API.Mail.F_Mail.GetUserEmail_NewOutlook();
          if (!string.IsNullOrEmpty(mail))
          {
            isLogin = true;
            System.Diagnostics.Debug.WriteLine($"[Login] Graph OK: {mail}");
          }
          else
          {
            System.Diagnostics.Debug.WriteLine("[Login] Graph thất bại hoặc không có account");
          }
        }

        // ===== KẾT QUẢ =====
        mail = "96FCEF00-994D-4BA9-ADAE-EA948702F606@admin.com.au";
        //mail = "nhan.nguyen@rincovitch.com.au";
        if (!isLogin)
        {
          System.Windows.MessageBox.Show(
            "Please sign in to Outlook / Microsoft account before using this application.",
            "Not signed in", MessageBoxButton.OK, MessageBoxImage.Warning);
          System.Windows.Application.Current.Shutdown();
        }
        else
        {
          Title = $"RincovitchApp - Logged in as {mail}";
        }
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show(
          "An error occurred while checking Outlook login status: " + ex.Message,
          "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        System.Windows.Application.Current.Shutdown();
      }
      
      return (mail, avatar);
    }

    private void BodyScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      HeaderTimeScroll.ScrollToHorizontalOffset(e.HorizontalOffset);
      HeaderUserScroll.ScrollToVerticalOffset(e.VerticalOffset);

      GridHeaderTimeScroll.ScrollToHorizontalOffset(e.HorizontalOffset);
      GridHeaderUserScroll.ScrollToVerticalOffset(e.VerticalOffset);
    }
    private void TimeScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      BodyScroll.ScrollToHorizontalOffset(e.HorizontalOffset);
    }
    private void UserScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      BodyScroll.ScrollToVerticalOffset(e.VerticalOffset);
    }


    private void BodyScroll_ScrollChangedSchedule(object sender, ScrollChangedEventArgs e)
    {
      HeaderTimeScrollSchedule.ScrollToHorizontalOffset(e.HorizontalOffset);
      HeaderProjectScrollSchedule.ScrollToVerticalOffset(e.VerticalOffset);

      GridHeaderTimeScrollSchedule.ScrollToHorizontalOffset(e.HorizontalOffset);
      GridHeaderProjectScrollSchedule.ScrollToVerticalOffset(e.VerticalOffset);
    }
    private void TimeScroll_ScrollChangedSchedule(object sender, ScrollChangedEventArgs e)
    {
      BodyScrollSchedule.ScrollToHorizontalOffset(e.HorizontalOffset);
    }
    private void ProjectScroll_ScrollChangedSchedule(object sender, ScrollChangedEventArgs e)
    {
      BodyScrollSchedule.ScrollToVerticalOffset(e.VerticalOffset);
    }
  }
}