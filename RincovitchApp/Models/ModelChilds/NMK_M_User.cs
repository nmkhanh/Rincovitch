using RincovitchApp.API;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using Brush = System.Windows.Media.Brush;

namespace RincovitchApp.Models.ModelChilds
{
  public class NMK_M_User : BaseViewModel
  {
    private F_Role.RoleType _RoleEnum;
    public F_Role.RoleType RoleEnum
    {
      get => _RoleEnum;
      set
      {
        _RoleEnum = value;
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

    Brush _Color;
    public Brush Color
    {
      get { return _Color; }
      set
      {
        _Color = value;
        OnPropertyChanged();
      }
    }
    public NMK_M_User()
    {
      NameFilter = NormalizeSearch(Name);
    }

    ObservableCollection<NMK_M_User> _Items = new ObservableCollection<NMK_M_User>();
    public ObservableCollection<NMK_M_User> Items
    {
      get { return _Items; }
      set
      {
        _Items = value;
        OnPropertyChanged();
      }
    }

    ObservableCollection<string> _ProjectIds = new ObservableCollection<string>();
    public ObservableCollection<string> ProjectIds
    {
      get { return _ProjectIds; }
      set
      {
        _ProjectIds = value;
        OnPropertyChanged();
      }
    }

    #region Filter
    private bool _isVisible = true;
    public bool IsVisible
    {
      get => _isVisible;
      set
      {
        _isVisible = value;
        OnPropertyChanged();
      }
    }

    string NormalizeSearch(string text)
    {
      if (string.IsNullOrWhiteSpace(text))
        return string.Empty;

      text = text
          .Replace('đ', 'd')
          .Replace('Đ', 'd');

      var normalized = text.Normalize(NormalizationForm.FormD);
      var sb = new StringBuilder();

      foreach (var c in normalized)
      {
        if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
          sb.Append(c);
      }

      return sb
          .ToString()
          .ToLowerInvariant()
          .Normalize(NormalizationForm.FormC);
    }

    public bool Filter(string keyword)
    {
      bool match = string.IsNullOrWhiteSpace(keyword) || (NameFilter?.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);

      bool childMatch = false;
      foreach (var c in Items)
      {
        childMatch |= c.Filter(keyword);
      }

      IsVisible = match || childMatch;
      return IsVisible;
    }
    #endregion

    string _Name = string.Empty;
    public string Name
    {
      get { return _Name; }
      set
      {
        _Name = value;
        OnPropertyChanged();
        NameFilter = NormalizeSearch(_Name);
        Key = string.Concat(_Name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(x => x[0]));
      }
    }
    string _Key = string.Empty;
    public string Key
    {
      get { return _Key; }
      set
      {
        _Key = value;
        OnPropertyChanged();
      }
    }

    int _CountTask = 0;
    public int CountTask
    {
      get { return _CountTask; }
      set
      {
        _CountTask = value;
        OnPropertyChanged();
      }
    }

    string _NameFilter;
    public string NameFilter
    {
      get { return _NameFilter; }
      set
      {
        _NameFilter = value;
        OnPropertyChanged();
      }
    }

    string _Team = string.Empty;
    public string Team
    {
      get { return _Team; }
      set
      {
        _Team = value;
        OnPropertyChanged();
      }
    }

    string _Role = string.Empty;
    public string Role
    {
      get
      {
        return _Role;
      }
      set
      {
        _Role = value;
        OnPropertyChanged();
      }
    }

    string _CreateBy = string.Empty;
    public string CreateBy
    {
      get
      {
        return _CreateBy;
      }
      set
      {
        _CreateBy = value;
        OnPropertyChanged();
      }
    }

    DateTime _CreateAt;
    public DateTime CreateAt
    {
      get
      {
        return _CreateAt;
      }
      set
      {
        _CreateAt = value;
        OnPropertyChanged();
      }
    }

    ImageSource _Image;
    public ImageSource Image
    {
      get
      {
        return _Image;
      }
      set
      {
        _Image = value;
        OnPropertyChanged();
      }
    }

    string _ImageString;
    public string ImageString
    {
      get
      {
        return _ImageString;
      }
      set
      {
        _ImageString = value;
        OnPropertyChanged();
        if(!string.IsNullOrEmpty(_ImageString))
        {
          Image = F_Image.Base64ToImageSource(_ImageString);
        }
      }
    }

    int _Index = 0;
    public int Index
    {
      get { return _Index; }
      set
      {
        _Index = value;
        OnPropertyChanged();
      }
    }

    string _Id = string.Empty;
    public string Id
    {
      get { return _Id; }
      set
      {
        _Id = value;
        OnPropertyChanged();
      }
    }

    string _Email = string.Empty;
    public string Email
    {
      get { return _Email; }
      set
      {
        _Email = value;
        OnPropertyChanged();
      }
    }

    string _Location = string.Empty;
    public string Location
    {
      get
      {
        return _Location;
      }
      set
      {
        _Location = value;
        OnPropertyChanged();
      }
    }

    public NMK_M_User Clone()
    {
      return new NMK_M_User
      {
        Id = this.Id,
        Name = this.Name,
        Team = this.Team,
        Index = this.Index,
        Color = this.Color,
        Email = this.Email,
        Role = this.Role,
        CreateBy = this.CreateBy,
        CreateAt = this.CreateAt,
        ImageString = this.ImageString,
        Location = this.Location,
        RoleEnum = F_Role.GetRoleTypeFromString(this.Role)
      };
    }

    public void Sort()
    {
      if (this.Items != null && this.Items.Count > 0)
      {
        var sorted = this.Items.OrderBy(i => i.Team).ThenBy(i => i.Name).ToList();
        this.Items = new ObservableCollection<NMK_M_User>(sorted);
      }
    }
  }
}
