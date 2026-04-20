using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Media;

namespace NMKApp.Models;

/// <summary>
/// Leave request domain model.
/// </summary>
public partial class LeaveModel : ObservableObject
{
  [ObservableProperty] private string _id = string.Empty;
  [ObservableProperty] private DateTime _createAt;
  [ObservableProperty] private DateTime _updateAt;
  [ObservableProperty] private string _createBy = string.Empty;
  [ObservableProperty] private string _sendTo = string.Empty;
  [ObservableProperty] private string _cC = string.Empty;
  [ObservableProperty] private string _cCTo = string.Empty;
  [ObservableProperty] private int _approval = 2;
  [ObservableProperty] private bool _isChecked;
  [ObservableProperty] private bool _isProgress;
  [ObservableProperty] private string _type = string.Empty;
  [ObservableProperty] private string _reason = string.Empty;
  [ObservableProperty] private SolidColorBrush _background = new(Colors.White);

  [ObservableProperty] private UserModel? _user;
  [ObservableProperty] private UserModel? _userCreateBy;
  [ObservableProperty] private UserModel? _userCC;

  public ObservableCollection<DayModel> LeaveList { get; set; } = [];
  public ListCollectionView? LeaveListCollectionView { get; set; }
}

public class LeaveCollection
{
  public ObservableCollection<LeaveModel> Items { get; set; } = [];
}
