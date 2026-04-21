using System.Collections.ObjectModel;
using System.Windows;
using NMKApp.Models;

namespace NMKApp.Views.Dialogs;

public partial class LeaveApplyDialog : Window
{
  public string SendTo { get; set; } = string.Empty;
  public string CC => TxtCC.Text.Trim();
  public string LeaveType =>
    (CmbType.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content as string ?? "Annual Leave";
  public string Reason => TxtReason.Text.Trim();
  public List<DayModel> LeaveDays { get; } = [];

  public LeaveApplyDialog(ObservableCollection<UserModel> users)
  {
    InitializeComponent();
    CmbSendTo.ItemsSource = users;
  }

  private void BtnSubmit_Click(object sender, RoutedEventArgs e)
  {
    if (string.IsNullOrWhiteSpace(SendTo))
    {
      MessageBox.Show("Please specify who to send the request to.", "Validation",
        MessageBoxButton.OK, MessageBoxImage.Warning);
      return;
    }
    if (string.IsNullOrWhiteSpace(Reason))
    {
      MessageBox.Show("Please enter a reason.", "Validation",
        MessageBoxButton.OK, MessageBoxImage.Warning);
      return;
    }
    DialogResult = true;
  }

  private void BtnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
