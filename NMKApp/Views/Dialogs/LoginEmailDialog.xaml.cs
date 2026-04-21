using System.Windows;

namespace NMKApp.Views.Dialogs;

public partial class LoginEmailDialog : Window
{
  public string Email { get; private set; } = string.Empty;

  public LoginEmailDialog()
  {
    InitializeComponent();
    Loaded += (_, _) => TxtEmail.Focus();
  }

  private void BtnOk_Click(object sender, RoutedEventArgs e) => Accept();
  private void BtnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
  private void TxtEmail_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
  {
    if (e.Key == System.Windows.Input.Key.Return) Accept();
  }

  private void Accept()
  {
    var val = TxtEmail.Text.Trim();
    if (!val.Contains('@'))
    {
      MessageBox.Show("Please enter a valid email address.", "Invalid email",
        MessageBoxButton.OK, MessageBoxImage.Warning);
      return;
    }
    Email = val.ToLowerInvariant();
    DialogResult = true;
  }
}
