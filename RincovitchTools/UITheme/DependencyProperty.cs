using Material.Icons;
using Microsoft.Expression.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RincovitchTools.UITheme
{
  public class UI
  {
    public static readonly DependencyProperty IconProperty = DependencyProperty.RegisterAttached("Icon", typeof(MaterialIconKind), typeof(UI), new FrameworkPropertyMetadata(null));
    public static MaterialIconKind GetIcon(DependencyObject obj) => (MaterialIconKind)obj.GetValue(IconProperty);
    public static void SetIcon(DependencyObject obj, MaterialIconKind value) => obj.SetValue(IconProperty, value);

    public static readonly DependencyProperty IconWidthProperty = DependencyProperty.RegisterAttached("IconWidth", typeof(double), typeof(UI), new FrameworkPropertyMetadata(0.0));
    public static double GetIconWidth(DependencyObject obj) => (double)obj.GetValue(IconWidthProperty);
    public static void SetIconWidth(DependencyObject obj, double value) => obj.SetValue(IconWidthProperty, value);

    public static readonly DependencyProperty IconHeightProperty = DependencyProperty.RegisterAttached("IconHeight", typeof(double), typeof(UI), new FrameworkPropertyMetadata(0.0));
    public static double GetIconHeight(DependencyObject obj) => (double)obj.GetValue(IconHeightProperty);
    public static void SetIconHeight(DependencyObject obj, double value) => obj.SetValue(IconHeightProperty, value);

    public static readonly DependencyProperty BDCornerProperty = DependencyProperty.RegisterAttached("BDCorner", typeof(double), typeof(UI), new FrameworkPropertyMetadata(4.0));
    public static double GetBDCorner(DependencyObject obj) => (double)obj.GetValue(BDCornerProperty);
    public static void SetBDCorner(DependencyObject obj, double value) => obj.SetValue(BDCornerProperty, value);

    public static readonly DependencyProperty HintProperty = DependencyProperty.RegisterAttached("Hint", typeof(string), typeof(UI), new FrameworkPropertyMetadata(""));
    public static string GetHint(DependencyObject obj) => (string)obj.GetValue(HintProperty);
    public static void SetHint(DependencyObject obj, string value) => obj.SetValue(HintProperty, value);

    public static readonly DependencyProperty PaddingContentProperty = DependencyProperty.RegisterAttached("PaddingContent", typeof(Thickness), typeof(UI), new FrameworkPropertyMetadata(new Thickness(0)));
    public static Thickness GetPaddingContent(DependencyObject obj) => (Thickness)obj.GetValue(PaddingContentProperty);
    public static void SetPaddingContent(DependencyObject obj, Thickness value) => obj.SetValue(PaddingContentProperty, value);

    public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.RegisterAttached("IsChecked", typeof(bool), typeof(UI), new FrameworkPropertyMetadata(true));
    public static bool GetIsChecked(DependencyObject element) => (bool)element.GetValue(IsCheckedProperty);
    public static void SetIsChecked(DependencyObject element, bool value) => element.SetValue(IsCheckedProperty, value);

    #region ProgressBar
    public static readonly DependencyProperty ThicknessProperty = DependencyProperty.RegisterAttached("Thickness", typeof(double), typeof(UI), new FrameworkPropertyMetadata(1.0));
    public static double GetThickness(DependencyObject obj) => (double)obj.GetValue(ThicknessProperty);
    public static void SetThickness(DependencyObject obj, double value) => obj.SetValue(ThicknessProperty, value);

    public static readonly DependencyProperty TypeProperty = DependencyProperty.RegisterAttached("Type", typeof(UnitType), typeof(UI), new FrameworkPropertyMetadata(UnitType.Pixel));
    public static UnitType GetType(DependencyObject element) => (UnitType)element.GetValue(TypeProperty);
    public static void SetType(DependencyObject element, UnitType value) => element.SetValue(TypeProperty, value);

    public static readonly DependencyProperty IsPixelProperty = DependencyProperty.RegisterAttached("IsPixel", typeof(bool), typeof(UI), new FrameworkPropertyMetadata(true));
    public static bool GetIsPixel(DependencyObject element) => (bool)element.GetValue(IsPixelProperty);
    public static void SetIsPixel(DependencyObject element, bool value) => element.SetValue(IsPixelProperty, value);
    #endregion


    #region ToogleButton
    public static readonly DependencyProperty IconFalseProperty = DependencyProperty.RegisterAttached("IconFalse", typeof(MaterialIconKind), typeof(UI), new FrameworkPropertyMetadata(null));
    public static MaterialIconKind GetIconFalse(DependencyObject obj) => (MaterialIconKind)obj.GetValue(IconFalseProperty);
    public static void SetIconFalse(DependencyObject obj, MaterialIconKind value) => obj.SetValue(IconFalseProperty, value);

    public static readonly DependencyProperty IconTrueProperty = DependencyProperty.RegisterAttached("IconTrue", typeof(MaterialIconKind), typeof(UI), new FrameworkPropertyMetadata(null));
    public static MaterialIconKind GetIconTrue(DependencyObject obj) => (MaterialIconKind)obj.GetValue(IconTrueProperty);
    public static void SetIconTrue(DependencyObject obj, MaterialIconKind value) => obj.SetValue(IconTrueProperty, value);

    public static readonly DependencyProperty ContentFalseProperty = DependencyProperty.RegisterAttached("ContentFalse", typeof(string), typeof(UI), new FrameworkPropertyMetadata(""));
    public static string GetContentFalse(DependencyObject obj) => (string)obj.GetValue(ContentFalseProperty);
    public static void SetContentFalse(DependencyObject obj, string value) => obj.SetValue(ContentFalseProperty, value);

    public static readonly DependencyProperty ContentTrueProperty = DependencyProperty.RegisterAttached("ContentTrue", typeof(string), typeof(UI), new FrameworkPropertyMetadata(""));
    public static string GetContentTrue(DependencyObject obj) => (string)obj.GetValue(ContentTrueProperty);
    public static void SetContentTrue(DependencyObject obj, string value) => obj.SetValue(ContentTrueProperty, value);
    #endregion

  }
}
