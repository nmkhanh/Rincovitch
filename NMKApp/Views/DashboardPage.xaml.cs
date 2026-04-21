using System.Windows.Controls;
using NMKApp.Models;
using NMKApp.ViewModels;

namespace NMKApp.Views;

public partial class DashboardPage : UserControl
{
  public DashboardPage()
  {
    InitializeComponent();
  }

  private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    if (DataContext is DashboardViewModel vm && e.AddedItems.Count > 0)
      vm.SelectedTask = e.AddedItems[0] as TaskModel;
  }
}

