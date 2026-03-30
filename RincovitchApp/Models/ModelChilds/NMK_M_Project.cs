using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.IdentityModel.Tokens;
using RincovitchApp.API;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;

namespace RincovitchApp.Models.ModelChilds
{
  public class NMK_M_Project : BaseViewModel
  {
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

    SolidColorPaint _ColorString;
    public SolidColorPaint ColorString 
    {
      get { return _ColorString; }
      set
      {
        _ColorString = value;
        OnPropertyChanged();
      }
    }
    public NMK_M_Project()
    {
      NameFilter = NormalizeSearch(Key);
    }

    ObservableCollection<NMK_M_Project> _Items = new ObservableCollection<NMK_M_Project>();
    public ObservableCollection<NMK_M_Project> Items
    {
      get { return _Items; }
      set
      {
        _Items = value;
        OnPropertyChanged();
      }
    }

    ObservableCollection<NMK_M_Task> _TasksSchedule = new ObservableCollection<NMK_M_Task>();
    public ObservableCollection<NMK_M_Task> TasksSchedule
    {
      get
      {
        return _TasksSchedule;
      }
      set
      {
        _TasksSchedule = value;
        OnPropertyChanged();
      }
    }
    ObservableCollection<NMK_M_Task> _Tasks = new ObservableCollection<NMK_M_Task>();
    public ObservableCollection<NMK_M_Task> Tasks
    {
      get
      {
        return _Tasks;
      }
      set
      {
        _Tasks = value;
        OnPropertyChanged();
      }
    }

    ObservableCollection<string> _UserIds = new ObservableCollection<string>();
    public ObservableCollection<string> UserIds
    {
      get { return _UserIds; }
      set
      {
        _UserIds = value;
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

    private bool _isVisibleDashboard = true;
    public bool IsVisibleDashboard
    {
      get => _isVisibleDashboard;
      set
      {
        _isVisibleDashboard = value;
        OnPropertyChanged();
      }
    }

    string _NameFilter = string.Empty;
    public string NameFilter
    {
      get { return _NameFilter; }
      set
      {
        _NameFilter = value;
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
      }
    }
    string _RevitVersion = string.Empty;
    public string RevitVersion
    {
      get { return _RevitVersion; }
      set
      {
        _RevitVersion = value;
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

    string _Key = string.Empty;
    public string Key
    {
      get { return _Key; }
      set
      {
        _Key = value;
        OnPropertyChanged();
        NameFilter = NormalizeSearch(_Key);
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
        if (!string.IsNullOrEmpty(ImageString))
        {
          Image = F_Image.Base64ToImageSource(ImageString);
        }
      }
    }

    string _Description = string.Empty;
    public string Description
    {
      get
      {
        return _Description;
      }
      set
      {
        _Description = value;
        OnPropertyChanged();
      }
    }

    public NMK_M_Project Clone()
    {
      return new NMK_M_Project
      {
        Id = this.Id,
        Name = this.Name,
        Key = this.Key,
        Index = this.Index,
        Color = this.Color,
        ColorString = this.ColorString,
        Description = this.Description,
        CreateBy = this.CreateBy,
        CreateAt = this.CreateAt,
        ImageString = this.ImageString,
        RevitVersion = this.RevitVersion,
        
      };
    }

    public void Set(NMK_M_Project item)
    {
      Id = item.Id;
      Name = item.Name;
      Key = item.Key;
      Index = item.Index;
      Color = item.Color;
      ColorString = item.ColorString;
      Description = item.Description;
      CreateBy = item.CreateBy;
      CreateAt = item.CreateAt;
      ImageString = item.ImageString;
      RevitVersion = item.RevitVersion;
      NameFilter = item.NameFilter;
      Image = item.Image;
    }

    public void Sort()
    {
      if (this.Items != null && this.Items.Count > 0)
      {
        var sorted = this.Items.OrderBy(i => i.Key).ToList();
        this.Items = new ObservableCollection<NMK_M_Project>(sorted);
      }
    }
  }
}
