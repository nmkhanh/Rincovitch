using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.SkiaSharpView.Painting;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace NMKApp.Models;

/// <summary>
/// Project domain model.
/// </summary>
public partial class ProjectModel : ObservableObject
{
  [ObservableProperty] private string _id = string.Empty;
  [ObservableProperty] private int _index;
  [ObservableProperty] private string _name = string.Empty;
  [ObservableProperty] private string _key = string.Empty;
  [ObservableProperty] private string _description = string.Empty;
  [ObservableProperty] private string _revitVersion = string.Empty;
  [ObservableProperty] private Brush? _color;
  [ObservableProperty] private SolidColorPaint? _colorString;
  [ObservableProperty] private DateTime _createAt;
  [ObservableProperty] private string _createBy = string.Empty;
  [ObservableProperty] private bool _isProgress;
  [ObservableProperty] private ImageSource? _image;

  public string NameFilter => Name?.ToLowerInvariant() ?? string.Empty;
  public ObservableCollection<string> UserIds { get; set; } = [];
  public ObservableCollection<TaskModel> Tasks { get; set; } = [];
}

/// <summary>
/// Container for project collections.
/// </summary>
public class ProjectCollection
{
  public ObservableCollection<ProjectModel> Items { get; set; } = [];

  public void Filter(string search)
  {
    // Filtering handled by ListCollectionView in ViewModel
  }
}
