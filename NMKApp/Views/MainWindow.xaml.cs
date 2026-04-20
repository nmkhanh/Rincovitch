using NMKApp.Helpers;
using NMKApp.Models;
using NMKApp.Services;
using NMKApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace NMKApp.Views;

public partial class MainWindow : Window
{
  private NotifyIcon? _notifyIcon;
  public MainWindowViewModel VM { get; }

  public MainWindow(MainWindowViewModel viewModel)
  {
    InitializeComponent();
    VM = viewModel;
    DataContext = VM;

    App.AppMainWindow = this;
    InitTrayIcon();
  }

  public async Task NavigateToTaskView(string id)
  {
    if (VM == null) return;

    var supabase = App.Services.GetRequiredService<ISupabaseService>();
    var task = await supabase.GetTasksByIdAsync(id);
    if (task.Success && task.Data!.Count > 0)
    {
      NavDashboard.IsChecked = true;
      // TODO: Navigate to specific task
    }
  }

  public void ShowApp()
  {
    if (!IsVisible) Show();
    if (WindowState == WindowState.Minimized) WindowState = WindowState.Normal;

    Visibility = Visibility.Visible;
    Activate();
    Topmost = true;
    Topmost = false;
    Focus();
  }

  private void InitTrayIcon()
  {
    _notifyIcon = new NotifyIcon
    {
      Visible = true,
      Text = "NMK Task Manager"
    };

    _notifyIcon.DoubleClick += (s, e) => ShowApp();

    var contextMenu = new ContextMenuStrip();
    contextMenu.Items.Add("Open", null, (s, e) => ShowApp());
    contextMenu.Items.Add("Exit", null, (s, e) => ExitApp());
    _notifyIcon.ContextMenuStrip = contextMenu;
  }

  private void ExitApp()
  {
    _notifyIcon?.Dispose();
    System.Windows.Application.Current.Shutdown();
  }

  protected override async void OnClosing(System.ComponentModel.CancelEventArgs e)
  {
    e.Cancel = true;
    Hide();
    await VM.BackupDataAsync();
  }
}
