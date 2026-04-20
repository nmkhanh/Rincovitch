using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace NMKApp.Models;

/// <summary>
/// Task domain model.
/// </summary>
public partial class TaskModel : ObservableObject
{
  [ObservableProperty] private string _id = string.Empty;
  [ObservableProperty] private int _index;
  [ObservableProperty] private int _indexStatus;
  [ObservableProperty] private string _name = string.Empty;
  [ObservableProperty] private string _onlyName = string.Empty;
  [ObservableProperty] private string _folder = string.Empty;
  [ObservableProperty] private int _status;
  [ObservableProperty] private int _approval;
  [ObservableProperty] private DateTime _dateStart;
  [ObservableProperty] private DateTime _dateEnd;
  [ObservableProperty] private DateTime _createAt;
  [ObservableProperty] private int _day;
  [ObservableProperty] private double _width;
  [ObservableProperty] private bool _isAssignedTo;
  [ObservableProperty] private bool _isProgress;
  [ObservableProperty] private bool _isVisible = true;
  [ObservableProperty] private bool _statusVisible = true;
  [ObservableProperty] private bool _stateAccepted;

  [ObservableProperty] private ProjectModel? _project;
  [ObservableProperty] private UserModel? _user;
  [ObservableProperty] private string? _projectId;
  [ObservableProperty] private string? _userId;

  public string State => Status switch
  {
    0 => "✓",
    1 => "⏳",
    2 => "📋",
    3 => "🔔",
    _ => "?"
  };

  public ObservableCollection<FileAttachModel> FileAttachs { get; set; } = [];
  public ObservableCollection<TaskModel> TaskChild { get; set; } = [];
}

public class FileAttachModel
{
  public string Name { get; set; } = string.Empty;
  public string Id { get; set; } = string.Empty;
}

/// <summary>
/// Container for task collections.
/// </summary>
public class TaskCollection
{
  public ObservableCollection<TaskModel> Items { get; set; } = [];
  public double PixelsPerDay { get; set; } = 40;
}
