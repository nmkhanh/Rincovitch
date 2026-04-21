using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMKApp.Models;
using NMKApp.Services;

namespace NMKApp.ViewModels;

/// <summary>
/// User management page ViewModel.
/// </summary>
public partial class UserViewModel : ObservableObject
{
  private readonly ISupabaseService _supabaseService;
  private MainWindowViewModel? _parent;

  public UserViewModel(ISupabaseService supabaseService)
  {
    _supabaseService = supabaseService;
  }

  public void Initialize(MainWindowViewModel parent)
  {
    _parent = parent;
  }

  [ObservableProperty] private UserModel? _selectedUser;

  [RelayCommand]
  private async Task AddAsync()
  {
    if (_parent?.CurrentUser == null) return;
    var dlg = new Views.Dialogs.UserEditDialog();
    if (dlg.ShowDialog() != true) return;
    try
    {
      var entity = new Data.Entities.UserEntity
      {
        Id = Guid.NewGuid().ToString(),
        Name = dlg.UserName,
        Email = dlg.Email,
        Role = dlg.Role,
        Team = dlg.Team,
        CreateBy = _parent.CurrentUser.Email
      };
      var result = await _supabaseService.InsertUserAsync(entity);
      if (result.Success) _parent.Users.Items.Add(result.Data!);
      _parent.RefreshAllViews();
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[User] Add error: {ex.Message}");
    }
  }

  [RelayCommand]
  private async Task EditAsync(object? parameter)
  {
    var user = parameter as UserModel ?? SelectedUser;
    if (user == null || _parent?.CurrentUser == null) return;
    var dlg = new Views.Dialogs.UserEditDialog(user);
    if (dlg.ShowDialog() != true) return;
    try
    {
      var entity = new Data.Entities.UserEntity
      {
        Id = user.Id, Name = dlg.UserName, Email = dlg.Email,
        Role = dlg.Role, Team = dlg.Team,
        CreateBy = user.CreateBy, UpdateBy = _parent.CurrentUser.Email
      };
      var result = await _supabaseService.UpdateUserAsync(entity);
      if (result.Success)
      {
        user.Name = dlg.UserName;
        user.Email = dlg.Email;
        user.Team = dlg.Team;
        user.Role = dlg.Role;
      }
      _parent.RefreshAllViews();
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[User] Edit error: {ex.Message}");
    }
  }

  [RelayCommand]
  private async Task DeleteAsync(object? parameter)
  {
    var user = parameter as UserModel ?? SelectedUser;
    if (user == null || _parent == null) return;
    var confirm = System.Windows.MessageBox.Show(
      $"Delete user {user.Name}?", "Confirm",
      System.Windows.MessageBoxButton.YesNo,
      System.Windows.MessageBoxImage.Warning);
    if (confirm != System.Windows.MessageBoxResult.Yes) return;
    await _supabaseService.DeleteUserAsync(user.Id);
    _parent.Users.Items.Remove(user);
    _parent.RefreshAllViews();
  }

  [RelayCommand]
  private async Task ImageAsync(object? parameter)
  {
    var user = parameter as UserModel ?? SelectedUser;
    if (user == null) return;
    var ofd = new Microsoft.Win32.OpenFileDialog
    {
      Filter = "Image files|*.png;*.jpg;*.jpeg;*.bmp;*.gif",
      Title = "Select profile image"
    };
    if (ofd.ShowDialog() != true) return;
    var bytes = await System.IO.File.ReadAllBytesAsync(ofd.FileName);
    user.ImageString = Convert.ToBase64String(bytes);
    var entity = new Data.Entities.UserEntity
    {
      Id = user.Id, Name = user.Name, Email = user.Email,
      Role = user.Role, Team = user.Team, CreateBy = user.CreateBy,
      ImageString = user.ImageString
    };
    await _supabaseService.UpdateUserAsync(entity);
    _parent?.RefreshAllViews();
  }
}
