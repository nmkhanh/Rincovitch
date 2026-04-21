using System.Collections.ObjectModel;
using System.Windows;
using NMKApp.Models;

namespace NMKApp.Views.Dialogs;

public partial class TaskEditDialog : Window
{
  // ─── Output properties ────────────────────────────────────────────────────
  public string TaskName => TxtName.Text.Trim();
  public string Description => TxtDescription.Text.Trim();
  public string? SelectedProjectId { get; set; }
  public string? SelectedUserId { get; set; }
  public DateTime DateStart => DpStart.SelectedDate ?? DateTime.Today;
  public DateTime DateEnd => DpEnd.SelectedDate ?? DateTime.Today.AddDays(7);

  // ─── New task ──────────────────────────────────────────────────────────────
  public TaskEditDialog(
    ObservableCollection<UserModel> users,
    ObservableCollection<ProjectModel> projects)
  {
    InitializeComponent();
    Title = "New Task";
    CmbProject.ItemsSource = projects;
    CmbUser.ItemsSource = users;
    DpStart.SelectedDate = DateTime.Today;
    DpEnd.SelectedDate = DateTime.Today.AddDays(7);
  }

  // ─── Edit task ─────────────────────────────────────────────────────────────
  public TaskEditDialog(
    ObservableCollection<UserModel> users,
    ObservableCollection<ProjectModel> projects,
    TaskModel existing) : this(users, projects)
  {
    Title = "Edit Task";
    TxtName.Text = existing.Name;
    TxtDescription.Text = string.Empty;  // description not in model
    CmbProject.SelectedValue = existing.ProjectId;
    CmbUser.SelectedValue = existing.UserId;
    SelectedProjectId = existing.ProjectId;
    SelectedUserId = existing.UserId;
    DpStart.SelectedDate = existing.DateStart;
    DpEnd.SelectedDate = existing.DateEnd;
  }

  private void BtnSave_Click(object sender, RoutedEventArgs e)
  {
    if (string.IsNullOrWhiteSpace(TaskName))
    {
      MessageBox.Show("Please enter a task name.", "Validation",
        MessageBoxButton.OK, MessageBoxImage.Warning);
      return;
    }

    SelectedProjectId = CmbProject.SelectedValue as string;
    SelectedUserId = CmbUser.SelectedValue as string;
    DialogResult = true;
  }

  private void BtnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
