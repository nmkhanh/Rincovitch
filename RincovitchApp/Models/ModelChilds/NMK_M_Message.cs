using Material.Icons;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RincovitchApp.Models.ModelChilds
{
  public class NMK_M_Message : BaseViewModel
  {
    private bool _Show;
    public bool Show
    {
      get => _Show;
      set
      {
        _Show = value;
        OnPropertyChanged();
      }
    }

    private bool _IsProgress;
    public bool IsProgress
    {
      get => _IsProgress;
      set
      {
        _IsProgress = value;
        OnPropertyChanged();
      }
    }

    private string _Title;
    public string Title
    {
      get => _Title;
      set
      {
        _Title = value;
        OnPropertyChanged();
      }
    }

    private string _Message;
    public string Message
    {
      get => _Message;
      set
      {
        _Message = value;
        OnPropertyChanged();
      }
    }

    private string _Validation;
    public string Validation
    {
      get => _Validation;
      set
      {
        _Validation = value;
        OnPropertyChanged();
      }
    }

    private MaterialIconKind _Icon;
    public MaterialIconKind Icon
    {
      get => _Icon;
      set
      {
        _Icon = value;
        OnPropertyChanged();
      }
    }

    public List<MaterialIconKind> Icons = new List<MaterialIconKind>() 
    { 
      MaterialIconKind.SuccessCircle,
      MaterialIconKind.Error,
      MaterialIconKind.Warning
    };

    private string _MainButtonTitle = "Ok";
    public string MainButtonTitle
    {
      get => _MainButtonTitle;
      set
      {
        _MainButtonTitle = value;
        OnPropertyChanged();
      }
    }

    private string _SupportButtonTitle = "Cancel";
    public string SupportButtonTitle
    {
      get => _SupportButtonTitle;
      set
      {
        _SupportButtonTitle = value;
        OnPropertyChanged();
      }
    }

    private ICommand _MainButton;
    public ICommand MainButton
    {
      get => _MainButton;
      set
      {
        _MainButton = value;
        OnPropertyChanged();
      }
    }

    private ICommand _SupportButton;
    public ICommand SupportButton
    {
      get => _SupportButton;
      set
      {
        _SupportButton = value;
        OnPropertyChanged();
      }
    }

    public NMK_M_Message()
    {
      MainButton = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
       Show = false;
      });

      SupportButton = new RelayCommand<object>((p) => { return true; }, (p) =>
      {
        Show = false;
      });
    }
  }

}
