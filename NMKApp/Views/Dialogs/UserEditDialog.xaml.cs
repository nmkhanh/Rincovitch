using System.Windows;
using NMKApp.Models;

namespace NMKApp.Views.Dialogs;

public partial class UserEditDialog : Window
{
  public string UserName => TxtName.Text.Trim();
  public string Email => TxtEmail.Text.Trim();
  public string Role => (CmbRole.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Tag as string ?? "user";
  public string Team => TxtTeam.Text.Trim();

  public UserEditDialog()
  {
    InitializeComponent();
    Title = "New User";
    CmbRole.SelectedIndex = 0;
  }

  public UserEditDialog(UserModel existing) : this()
  {
    Title = "Edit User";
    TxtName.Text = existing.Name;
    TxtEmail.Text = existing.Email;
    TxtTeam.Text = existing.Team;
    // Select role
    foreach (System.Windows.Controls.ComboBoxItem item in CmbRole.Items)
      if (item.Tag as string == existing.Role) { CmbRole.SelectedItem = item; break; }
  }

  private void BtnSave_Click(object sender, RoutedEventArgs e)
  {
    if (string.IsNullOrWhiteSpace(UserName) || !Email.Contains('@'))
    {
      MessageBox.Show("Please enter a valid name and email.", "Validation",
        MessageBoxButton.OK, MessageBoxImage.Warning);
      return;
    }
    DialogResult = true;
  }

  private void BtnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
