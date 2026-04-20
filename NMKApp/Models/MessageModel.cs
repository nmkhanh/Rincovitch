using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;
using System.Windows.Input;

namespace NMKApp.Models;

/// <summary>
/// Dialog/message model for popup dialogs.
/// </summary>
public partial class MessageModel : ObservableObject
{
  [ObservableProperty] private bool _show;
  [ObservableProperty] private bool _isProgress;
  [ObservableProperty] private bool _isClosed;
  [ObservableProperty] private string _title = string.Empty;
  [ObservableProperty] private string _message = string.Empty;
  [ObservableProperty] private string? _validation;
  [ObservableProperty] private MaterialIconKind _icon = MaterialIconKind.InformationOutline;
  [ObservableProperty] private string _mainButtonTitle = "OK";
  [ObservableProperty] private string _supportButtonTitle = "Cancel";
  [ObservableProperty] private ICommand? _mainButton;
  [ObservableProperty] private ICommand? _supportButton;

  public static readonly MaterialIconKind[] Icons =
  [
    MaterialIconKind.InformationOutline,
    MaterialIconKind.AlertCircleOutline,
    MaterialIconKind.CheckCircleOutline,
    MaterialIconKind.CloseCircleOutline
  ];
}
