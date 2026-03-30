using Autodesk.Revit.DB;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using PaperSize = Autodesk.Revit.DB.PaperSize;

namespace RincovitchTools.Print.Models.ModelChilds
{
  public class NMK_M_Settings_PDF : BaseViewModel
  {
    #region Paper Placement
    bool _paperPlacementCenter = false;
    public bool PaperPlacementCenter
    {
      get { return _paperPlacementCenter; }
      set
      {
        _paperPlacementCenter = value;
        OnPropertyChanged();
      }
    }

    bool _paperPlacementOffsetFromCorner = true;
    public bool PaperPlacementOffsetFromCorner
    {
      get { return _paperPlacementOffsetFromCorner; }
      set
      {
        _paperPlacementOffsetFromCorner = value;
        OnPropertyChanged();
      }
    }

    // Offset Options
    bool _offsetNoMargin = true;
    public bool OffsetNoMargin
    {
      get { return _offsetNoMargin; }
      set
      {
        _offsetNoMargin = value;
        OnPropertyChanged();
      }
    }

    bool _offsetPrinterLimit = false;
    public bool OffsetPrinterLimit
    {
      get { return _offsetPrinterLimit; }
      set
      {
        _offsetPrinterLimit = value;
        OnPropertyChanged();
      }
    }

    bool _offsetUserDefined = false;
    public bool OffsetUserDefined
    {
      get { return _offsetUserDefined; }
      set
      {
        _offsetUserDefined = value;
        OnPropertyChanged();
      }
    }

    double _offsetX = 0;
    public double OffsetX
    {
      get { return _offsetX; }
      set
      {
        _offsetX = value;
        OnPropertyChanged();
      }
    }

    double _offsetY = 0;
    public double OffsetY
    {
      get { return _offsetY; }
      set
      {
        _offsetY = value;
        OnPropertyChanged();
      }
    }
    #endregion

    #region Zoom
    bool _zoomFitToPage = false;
    public bool ZoomFitToPage
    {
      get { return _zoomFitToPage; }
      set
      {
        _zoomFitToPage = value;
        OnPropertyChanged();
      }
    }

    bool _zoomCustom = true;
    public bool ZoomCustom
    {
      get { return _zoomCustom; }
      set
      {
        _zoomCustom = value;
        OnPropertyChanged();
      }
    }

    double _zoomPercentage = 100;
    public double ZoomPercentage
    {
      get { return _zoomPercentage; }
      set
      {
        _zoomPercentage = value;
        OnPropertyChanged();
      }
    }
    #endregion

    #region Printer
    ObservableCollection<string> _printers = new ObservableCollection<string>();
    public ObservableCollection<string> Printers
    {
      get { return _printers; }
      set
      {
        _printers = value;
        OnPropertyChanged();
      }
    }

    string _selectedPrinter = "";
    public string SelectedPrinter
    {
      get { return _selectedPrinter; }
      set
      {
        _selectedPrinter = value;
        OnPropertyChanged();
      }
    }

    public PrinterSettings SelectedPrinter_Window { get; set; }

    ObservableCollection<PaperSize> _printers_PaperSize = new ObservableCollection<PaperSize>();
    public ObservableCollection<PaperSize> Printers_PaperSize
    {
      get { return _printers_PaperSize; }
      set
      {
        _printers_PaperSize = value;
        OnPropertyChanged();
      }
    }

    #endregion

    #region Hidden Line Views
    bool _vectorProcessing = true;
    public bool VectorProcessing
    {
      get { return _vectorProcessing; }
      set
      {
        _vectorProcessing = value;
        OnPropertyChanged();
      }
    }

    bool _rasterProcessing = false;
    public bool RasterProcessing
    {
      get { return _rasterProcessing; }
      set
      {
        _rasterProcessing = value;
        OnPropertyChanged();
      }
    }
    #endregion

    #region Appearance
    ObservableCollection<string> _rasterQualities = new ObservableCollection<string>
    {
      "Low",
      "Medium", 
      "High",
      "Presentation"
    };
    public ObservableCollection<string> RasterQualities
    {
      get { return _rasterQualities; }
      set
      {
        _rasterQualities = value;
        OnPropertyChanged();
      }
    }

    string _selectedRasterQuality = "Presentation";
    public string SelectedRasterQuality
    {
      get { return _selectedRasterQuality; }
      set
      {
        _selectedRasterQuality = value;
        OnPropertyChanged();
      }
    }

    ObservableCollection<string> _colors = new ObservableCollection<string>
    {
      "Black Lines",
      "Gray Scale",
      "Color"
    };
    public ObservableCollection<string> Colors
    {
      get { return _colors; }
      set
      {
        _colors = value;
        OnPropertyChanged();
      }
    }

    string _selectedColor = "Color";
    public string SelectedColor
    {
      get { return _selectedColor; }
      set
      {
        _selectedColor = value;
        OnPropertyChanged();
      }
    }
    #endregion

    #region Options
    bool _viewLinksInBlue = true;
    public bool ViewLinksInBlue
    {
      get { return _viewLinksInBlue; }
      set
      {
        _viewLinksInBlue = value;
        OnPropertyChanged();
      }
    }

    bool _hideRefWorkPlanes = true;
    public bool HideRefWorkPlanes
    {
      get { return _hideRefWorkPlanes; }
      set
      {
        _hideRefWorkPlanes = value;
        OnPropertyChanged();
      }
    }

    bool _hideUnreferencedViewTags = true;
    public bool HideUnreferencedViewTags
    {
      get { return _hideUnreferencedViewTags; }
      set
      {
        _hideUnreferencedViewTags = value;
        OnPropertyChanged();
      }
    }

    bool _hideScopeBoxes = true;
    public bool HideScopeBoxes
    {
      get { return _hideScopeBoxes; }
      set
      {
        _hideScopeBoxes = value;
        OnPropertyChanged();
      }
    }

    bool _hideCropBoundaries = true;
    public bool HideCropBoundaries
    {
      get { return _hideCropBoundaries; }
      set
      {
        _hideCropBoundaries = value;
        OnPropertyChanged();
      }
    }

    bool _replaceHalftoneWithThinLines = false;
    public bool ReplaceHalftoneWithThinLines
    {
      get { return _replaceHalftoneWithThinLines; }
      set
      {
        _replaceHalftoneWithThinLines = value;
        OnPropertyChanged();
      }
    }

    bool _regionEdgesMaskCoincidentLines = false;
    public bool RegionEdgesMaskCoincidentLines
    {
      get { return _regionEdgesMaskCoincidentLines; }
      set
      {
        _regionEdgesMaskCoincidentLines = value;
        OnPropertyChanged();
      }
    }
    #endregion
  }
}
