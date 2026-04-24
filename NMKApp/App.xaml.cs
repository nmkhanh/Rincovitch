using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.Notifications;
using NMKApp.Core;
using NMKApp.Services;
using NMKApp.ViewModels;
using NMKApp.Views;

namespace NMKApp;

public partial class App : Application
{
  public static MainWindow? AppMainWindow { get; set; }

  /// <summary>
  /// Global service provider — accessed via App.Services
  /// </summary>
  public static IServiceProvider Services { get; private set; } = null!;

  protected override void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);

    // 1. Build DI container
    Services = ConfigureServices();

    // 2. Initialize toast notifications
    var toastService = Services.GetRequiredService<IToastService>();
    toastService.Initialize();

    // 3. Handle toast activation
    ToastNotificationManagerCompat.OnActivated += toastArgs =>
    {
      var args = ToastArguments.Parse(toastArgs.Argument);
      string taskId = args.Get("taskId");
      Current.Dispatcher.Invoke(() =>
      {
        if (AppMainWindow != null)
        {
          AppMainWindow.ShowApp();
          if (!string.IsNullOrEmpty(taskId))
            AppMainWindow.NavigateToTaskView(taskId);
        }
      });
    };

    // 4. Show main window
    var mainWindow = Services.GetRequiredService<MainWindow>();
    AppMainWindow = mainWindow;
    mainWindow.Show();
  }

  private static IServiceProvider ConfigureServices()
  {
    var services = new ServiceCollection();

    // Core services
    services.AddSingleton<ISupabaseService, SupabaseService>();
    services.AddSingleton<IAuthService, AuthService>();
    services.AddSingleton<IToastService, ToastService>();
    services.AddSingleton<IBackupService, BackupService>();
    services.AddSingleton<IRealtimeService, RealtimeService>();
    services.AddSingleton<IMailService, MailService>();
    services.AddSingleton<INavigationService, NavigationService>();

    // ViewModels
    services.AddSingleton<MainWindowViewModel>();

    // Views
    services.AddSingleton<MainWindow>();

    return services.BuildServiceProvider();
  }
}
