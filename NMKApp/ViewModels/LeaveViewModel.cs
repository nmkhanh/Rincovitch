using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NMKApp.Models;
using NMKApp.Services;
using System.Windows.Data;

namespace NMKApp.ViewModels;

/// <summary>
/// Leave management page ViewModel.
/// </summary>
public partial class LeaveViewModel : ObservableObject
{
  private readonly ISupabaseService _supabaseService;
  private readonly IMailService _mailService;
  private MainWindowViewModel? _parent;

  public ListCollectionView? UsersCollectionLeave { get; set; }

  public LeaveViewModel(ISupabaseService supabaseService, IMailService mailService)
  {
    _supabaseService = supabaseService;
    _mailService = mailService;
  }

  public void Initialize(MainWindowViewModel parent)
  {
    _parent = parent;
    UsersCollectionLeave = new ListCollectionView(parent.Users.Items);
  }

  // TODO: Add leave-specific commands (apply, approve, reject)
}
