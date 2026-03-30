using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace TestWPF
{
  public class PickDateTime : Control
  {
    private DatePicker _datePicker;
    private TextBox _timeBox;
    private Popup _popup;
    private ButtonBase _btnClear;

    private bool _internalUpdate;

    static PickDateTime()
    {
      DefaultStyleKeyProperty.OverrideMetadata(
          typeof(PickDateTime),
          new FrameworkPropertyMetadata(typeof(PickDateTime)));
    }

    #region DP

    public DateTime? SelectedDateTime
    {
      get => (DateTime?)GetValue(SelectedDateTimeProperty);
      set => SetValue(SelectedDateTimeProperty, value);
    }

    public static readonly DependencyProperty SelectedDateTimeProperty =
        DependencyProperty.Register(
            nameof(SelectedDateTime),
            typeof(DateTime?),
            typeof(PickDateTime),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnSelectedDateTimeChanged));

    private static void OnSelectedDateTimeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
      try
      {
        ((PickDateTime)d).UpdateVisual();
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex);
      }
    }

    #endregion

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      try
      {
        _datePicker = GetTemplateChild("PART_Date") as DatePicker;
        _timeBox = GetTemplateChild("PART_Time") as TextBox;
        _popup = GetTemplateChild("PART_TimePopup") as Popup;
        _btnClear = GetTemplateChild("PART_Clear") as ButtonBase;

        HookEvents();
        UpdateVisual();
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex);
      }
    }

    private void HookEvents()
    {
      try
      {
        if (_datePicker != null)
          _datePicker.SelectedDateChanged += (_, __) => UpdateFromParts();

        if (_timeBox != null)
        {
          _timeBox.PreviewTextInput += TimeMaskInput;
          _timeBox.LostFocus += (_, __) => NormalizeTime();
          _timeBox.TextChanged += (_, __) => UpdateFromParts();
          DataObject.AddPastingHandler(_timeBox, OnPaste);
        }

        if (_btnClear != null)
          _btnClear.Click += (_, __) => SelectedDateTime = null;
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex);
      }
    }

    #region Time Mask

    private void TimeMaskInput(object sender, TextCompositionEventArgs e)
    {
      try
      {
        e.Handled = !char.IsDigit(e.Text, 0);
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex);
      }
    }

    private void OnPaste(object sender, DataObjectPastingEventArgs e)
    {
      try
      {
        if (!e.DataObject.GetDataPresent(typeof(string)))
          e.CancelCommand();
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex);
      }
    }

    #endregion

    #region Core Logic

    private void UpdateVisual()
    {
      try
      {
        if (_internalUpdate)
          return;
        _internalUpdate = true;

        if (SelectedDateTime == null)
        {
          if (_datePicker != null)
            _datePicker.SelectedDate = null;
          if (_timeBox != null)
            _timeBox.Text = "";
          return;
        }

        var dt = SelectedDateTime.Value;

        if (_datePicker != null)
          _datePicker.SelectedDate = dt.Date;

        if (_timeBox != null)
          _timeBox.Text = dt.ToString("HH:mm");
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex);
      }
      finally
      {
        _internalUpdate = false;
      }
    }

    private void UpdateFromParts()
    {
      try
      {
        if (_internalUpdate)
          return;
        if (_datePicker?.SelectedDate == null)
          return;

        var date = _datePicker.SelectedDate.Value;

        TimeSpan time = TimeSpan.Zero;

        if (_timeBox != null &&
            TryParseTime(_timeBox.Text, out var ts))
        {
          time = ts;
        }

        SelectedDateTime = date.Date + time;
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex);
      }
    }

    private void NormalizeTime()
    {
      try
      {
        if (_timeBox == null)
          return;

        if (TryParseTime(_timeBox.Text, out var ts))
          _timeBox.Text = ts.ToString(@"hh\:mm\:ss");
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex);
      }
    }

    private bool TryParseTime(string text, out TimeSpan time)
    {
      try
      {
        if (TimeSpan.TryParseExact(
                text,
                new[] { @"hh\:mm\:ss", @"hh\:mm" },
                CultureInfo.InvariantCulture,
                out time))
          return true;
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex);
      }

      time = TimeSpan.Zero;
      return false;
    }

    #endregion
  }
}
