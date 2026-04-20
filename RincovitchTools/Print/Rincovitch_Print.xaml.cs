using RincovitchTools._00_General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RincovitchTools.Print
{
  /// <summary>
  /// Interaction logic for Rincovitch_Print.xaml
  /// </summary>
  public partial class Rincovitch_Print : Window
  {
    public Rincovitch_PrintViewModel ViewModel { get; set; }
    public Rincovitch_Print()
    {
      //System.Windows.Application.ResourceAssembly = typeof(Rincovitch_Print).Assembly;
      //MVVMWindows_Themes.ApplyTheme(System.Windows.Application.Current.Resources, "Light");

      
      InitializeComponent();
      this.DataContext = ViewModel = new Rincovitch_PrintViewModel();
      this.Show();
      //this.Loaded += async (s, e) =>
      //{
      //  await Trial();
      //  this.DataContext = ViewModel = new Rincovitch_PrintViewModel();
      //};
    }

    async Task Trial()
    {
      int day = 30;
      try
      {
        var users = await F_CheckTime.GetUserByIdAsync();
        if (users == null || users.Count == 0)
        {
          await F_CheckTime.InsertUserAsync();
        }
        else
        {
          var user = users[0];
          var span = DateTime.Now - user.created_at;
          day = 30 - span.Days;
          if (day <= 0)
          {
            System.Windows.MessageBox.Show("Your trial period has expired. Please contact support to purchase a license.", "Trial Expired", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
          }
          else
            this.Show();
        }
      }
      catch (Exception)
      {

      }
      //this.Title = $"Rincovitch Tools - Print (Trial: {day} days left)";
      this.Title = $"Rincovitch Tools - Print";
    }
  }
}